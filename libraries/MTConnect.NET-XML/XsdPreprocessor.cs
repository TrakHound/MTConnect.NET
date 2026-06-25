// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MTConnect
{
    /// <summary>
    /// Strips XSD 1.1-only constructs from an XSD source document so the
    /// .NET BCL <see cref="System.Xml.Schema.XmlSchemaSet"/> (XSD 1.0 only)
    /// can compile the schema's 1.0-compatible subset.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The official MTConnect XSDs (v1.3 onwards) carry XSD 1.1 constructs —
    /// <c>&lt;xs:assert&gt;</c>, <c>&lt;xs:override&gt;</c>, and the
    /// <c>notNamespace</c> attribute on <c>&lt;xs:any&gt;</c> /
    /// <c>&lt;xs:anyAttribute&gt;</c>. .NET's BCL XSD reader rejects these
    /// outright (<c>XmlSchemaException: notNamespace not supported</c>), so a
    /// downstream consumer that hands the official XSD to
    /// <see cref="System.Xml.Schema.XmlSchema.Read(System.IO.TextReader, System.Xml.Schema.ValidationEventHandler)"/>
    /// never gets to the validation step.
    /// </para>
    /// <para>
    /// This preprocessor removes those constructs (and downgrades
    /// <c>notNamespace</c> by stripping the attribute, which leaves the
    /// <c>xs:any</c>/<c>xs:anyAttribute</c> in its default-permissive
    /// XSD-1.0 form). The resulting schema is a valid XSD 1.0 subset that
    /// the BCL reader accepts; it loses some of the spec's negative-namespace
    /// constraints but keeps every structural rule the 1.0-compatible subset
    /// expresses. For full XSD 1.1 validation, a Saxon-HE-backed validator
    /// is the correct upgrade path.
    /// </para>
    /// </remarks>
    public static class XsdPreprocessor
    {
        private const string XsdNamespace = "http://www.w3.org/2001/XMLSchema";

        /// <summary>
        /// Returns a copy of <paramref name="xsdSourceXml"/> with every XSD
        /// 1.1-only construct removed. Idempotent: re-running on the
        /// preprocessed output is a no-op.
        /// </summary>
        /// <param name="xsdSourceXml">The raw XSD source as XML text.</param>
        /// <returns>An XSD-1.0-compatible XML string.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="xsdSourceXml"/> is null.
        /// </exception>
        public static string StripXsd11Constructs(string xsdSourceXml)
        {
            if (xsdSourceXml == null) throw new ArgumentNullException(nameof(xsdSourceXml));
            if (xsdSourceXml.Length == 0) return xsdSourceXml;

            XDocument doc;
            try
            {
                using (var reader = new StringReader(xsdSourceXml))
                {
                    doc = XDocument.Load(reader, LoadOptions.PreserveWhitespace);
                }
            }
            catch (System.Xml.XmlException)
            {
                // Not well-formed XML — let the downstream BCL reader emit
                // the parse error so the caller's error path stays
                // consistent.
                return xsdSourceXml;
            }

            if (doc.Root == null) return xsdSourceXml;

            // xs:override and xs:assert have no XSD 1.0 equivalent — remove
            // wholesale. Snapshot the descendants list before mutating; the
            // live-axes view would otherwise mis-iterate after Remove().
            XName overrideName = XName.Get("override", XsdNamespace);
            XName assertName = XName.Get("assert", XsdNamespace);
            var toRemove = doc.Descendants()
                .Where(e => e.Name == overrideName || e.Name == assertName)
                .ToList();
            foreach (var element in toRemove)
            {
                element.Remove();
            }

            // notNamespace / notQName are XSD 1.1-only attributes on
            // xs:any / xs:anyAttribute. Two paths:
            //
            //   * xs:anyAttribute carrying notNamespace/notQName — strip the
            //     attribute and let the element fall back to its 1.0 default
            //     of namespace='##any'. Attribute wildcards do not create the
            //     content-model ambiguity below, so the permissive 1.0 form
            //     is structurally equivalent for load-time validation.
            //
            //   * xs:any carrying notNamespace/notQName — REMOVE the element.
            //     Falling back to '##any' would produce a wildcard adjacent
            //     to concrete elements in the target namespace and XSD 1.0
            //     would reject the schema with "Wildcard '##any' allows
            //     element '...' and causes the content model to become
            //     ambiguous" (unique-particle-attribution). Dropping the
            //     wildcard is lossy with respect to the spec's extensibility
            //     contract, but the 1.0-compatible subset is what the BCL
            //     reader can compile — full XSD 1.1 validation is the
            //     Saxon-HE upgrade path.
            XName anyName = XName.Get("any", XsdNamespace);
            XName anyAttributeName = XName.Get("anyAttribute", XsdNamespace);

            var anyAttributeWildcards = doc.Descendants(anyAttributeName).ToList();
            foreach (var wildcard in anyAttributeWildcards)
            {
                wildcard.Attribute("notNamespace")?.Remove();
                wildcard.Attribute("notQName")?.Remove();
            }

            var elementWildcardsToRemove = doc.Descendants(anyName)
                .Where(e => e.Attribute("notNamespace") != null || e.Attribute("notQName") != null)
                .ToList();
            foreach (var wildcard in elementWildcardsToRemove)
            {
                wildcard.Remove();
            }

            // Two more XSD 1.1 features the BCL XSD 1.0 reader rejects when
            // they appear inside xs:all:
            //
            //   * xs:any inside xs:all (XSD 1.1 §3.8.6 group all extension).
            //     XSD 1.0 disallows xs:any as an all-particle entirely; the
            //     reader emits "The 'http://www.w3.org/2001/XMLSchema:any'
            //     element is not supported in this context." Remove the
            //     offending xs:any.
            //
            //   * xs:element with maxOccurs > 1 inside xs:all (XSD 1.1
            //     relaxes the 1.0 constraint that all-particles must have
            //     maxOccurs in {0, 1}). XSD 1.0's reader rejects with "The
            //     'maxOccurs' attribute of all the particles of an 'all'
            //     group must be 0 or 1." Clamp maxOccurs to 1.
            XName allName = XName.Get("all", XsdNamespace);
            XName elementName = XName.Get("element", XsdNamespace);
            XName extensionName = XName.Get("extension", XsdNamespace);
            XName restrictionName = XName.Get("restriction", XsdNamespace);

            // XSD 1.1 §3.8 allows xs:all directly inside an xs:extension /
            // xs:restriction derivation. XSD 1.0 forbids it and the BCL
            // reader rejects with "'all' is not the only particle in a
            // group, or is being used as an extension." Rewrite those
            // xs:all groups to xs:sequence. Sequence is order-sensitive
            // where all is not, so the resulting schema is more restrictive
            // than the 1.1 original — acceptable for load-time validation
            // of the 1.0-compatible subset.
            var allInsideDerivation = doc.Descendants(allName)
                .Where(a => a.Parent != null && (a.Parent.Name == extensionName || a.Parent.Name == restrictionName))
                .ToList();
            foreach (var allGroup in allInsideDerivation)
            {
                allGroup.Name = XName.Get("sequence", XsdNamespace);
            }

            foreach (var allGroup in doc.Descendants(allName))
            {
                var anyParticles = allGroup.Elements(anyName).ToList();
                foreach (var particle in anyParticles)
                {
                    particle.Remove();
                }

                foreach (var elementParticle in allGroup.Elements(elementName))
                {
                    var maxOccurs = elementParticle.Attribute("maxOccurs");
                    if (maxOccurs != null && !IsZeroOrOne(maxOccurs.Value))
                    {
                        maxOccurs.Value = "1";
                    }
                }
            }

            // Use a UTF-8-without-BOM writer so the output stays byte-
            // identical to what the BCL reader expects on the wire.
            var sb = new StringBuilder();
            using (var writer = System.Xml.XmlWriter.Create(sb, new System.Xml.XmlWriterSettings
            {
                OmitXmlDeclaration = doc.Declaration == null,
                Encoding = new UTF8Encoding(false),
                Indent = false,
            }))
            {
                doc.Save(writer);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Convenience batch overload — applies <see cref="StripXsd11Constructs(string)"/>
        /// to each entry in <paramref name="xsdSourceXmls"/>.
        /// </summary>
        public static IEnumerable<string> StripXsd11Constructs(IEnumerable<string> xsdSourceXmls)
        {
            if (xsdSourceXmls == null) throw new ArgumentNullException(nameof(xsdSourceXmls));
            return xsdSourceXmls.Select(StripXsd11Constructs);
        }

        private static bool IsZeroOrOne(string maxOccurs)
        {
            return string.Equals(maxOccurs, "0", StringComparison.Ordinal)
                || string.Equals(maxOccurs, "1", StringComparison.Ordinal);
        }
    }
}
