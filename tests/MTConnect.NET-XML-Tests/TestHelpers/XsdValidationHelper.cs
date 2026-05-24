// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace MTConnect.Tests.XML.TestHelpers
{
    /// <summary>
    /// Loads a single MTConnect XSD from the compliance test tree and runs an
    /// XML envelope through schema validation.
    /// </summary>
    /// <remarks>
    /// XSDs live under
    /// <c>tests/Compliance/MTConnect-Compliance-Tests/Schemas/v&lt;v&gt;/</c>
    /// in the repo. The helper resolves the absolute path via the
    /// <c>MTConnect.NET.sln</c> walk-up performed by RepoRootLocator.
    /// XmlResolver is pinned to null so external entity resolution is refused
    /// at parse time — defense-in-depth per OWASP "XML External Entities (XXE)".
    /// The W3C xml.xsd and xlink.xsd schemas (sibling files in the Schemas
    /// tree under <c>w3c/</c>) are pre-seeded into the XmlSchemaSet before
    /// the MTConnect XSD is added, so the MTConnect &lt;xs:import
    /// namespace='…/xlink'&gt; reference resolves by target-namespace match
    /// without any resolver traffic. Mirrors the L1 SchemaLoadTests fixture
    /// in MTConnect-Compliance-Tests.
    /// Additionally, a chameleon-style sidecar XSD is added in the same
    /// target namespace as the MTConnect schema under test, declaring a
    /// non-abstract derivation of the spec-abstract
    /// <c>ThreeDimensionalEntryType</c> as
    /// <c>XYZEntryType</c>. The spec leaves that base abstract with no
    /// concrete derivative, so an instance carrying
    /// <c>&lt;Entry key="X"&gt;…&lt;/Entry&gt;</c> inside an
    /// <c>XYZDataSetType</c> sequence cannot satisfy XSD 1.0 validation
    /// without a client-side <c>xsi:type</c>. Test envelopes opt into the
    /// shim by tagging Entry elements
    /// <c>xsi:type="mt:XYZEntryType"</c>.
    /// </remarks>
    internal static class XsdValidationHelper
    {
        public static string GetSchemaPath(string version, string envelopeKind)
        {
            var root = RepoRootLocator.LocateRoot();
            var schemaDir = Path.Combine(
                root,
                "tests",
                "Compliance",
                "MTConnect-Compliance-Tests",
                "Schemas",
                "v" + version.Replace('.', '_'));
            var fileName = $"MTConnect{envelopeKind}_{version}.xsd";
            return Path.Combine(schemaDir, fileName);
        }

        public static IReadOnlyList<string> Validate(string xml, string xsdPath)
        {
            var errors = new List<string>();

            var readerSettings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                XmlResolver = null
            };

            var schemaSet = new XmlSchemaSet
            {
                // Defense-in-depth: prevent SchemaSet from fetching external
                // <xs:include> / <xs:import> URIs. The bundled Schemas/ tree
                // is fully self-contained at build time; sibling W3C XSDs
                // (xml, xlink) are pre-loaded below so their target
                // namespaces are already known when the MTConnect XSD's
                // <xs:import namespace='…/xlink' schemaLocation='xlink.xsd'/>
                // is processed.
                XmlResolver = null
            };
            schemaSet.ValidationEventHandler += (s, e) =>
                errors.Add($"[{e.Severity}] {e.Message}");

            // Pre-load the W3C xml.xsd (target ns http://www.w3.org/XML/
            // 1998/namespace) before xlink.xsd, since xlink imports xml
            // for its xml:lang attribute reference. Order matters: xml
            // first, then xlink, then the MTConnect XSD under test.
            var w3cDir = GetW3cDir(xsdPath);
            AddSchemaFromFile(schemaSet, readerSettings, Path.Combine(w3cDir, "xml.xsd"));
            AddSchemaFromFile(schemaSet, readerSettings, Path.Combine(w3cDir, "xlink.xsd"));

            // Pre-process the XSD source through the library's
            // XsdPreprocessor so the BCL XmlSchemaSet (XSD 1.0 only) can
            // compile the 1.0-compatible subset of the official MTConnect
            // XSDs (which carry XSD 1.1 constructs the BCL reader rejects).
            var preprocessedXsd = XsdPreprocessor.StripXsd11Constructs(File.ReadAllText(xsdPath));
            using (var schemaStringReader = new StringReader(preprocessedXsd))
            using (var schemaReader = XmlReader.Create(schemaStringReader, readerSettings))
            {
                schemaSet.Add(null, schemaReader);
            }

            // Add a chameleon-style sidecar XSD in the MTConnect schema's
            // own target namespace declaring a non-abstract derivation of
            // ThreeDimensionalEntryType. Without this, instance <Entry>
            // elements inside an XYZDataSetType cannot validate against
            // the abstract base; with it, an envelope can tag the entry
            // <Entry xsi:type="mt:XYZEntryType" …/> and the validator
            // accepts it.
            var targetNamespace = ExtractTargetNamespace(preprocessedXsd);
            if (!string.IsNullOrEmpty(targetNamespace))
            {
                var shimXsd = BuildEntryTypeShimXsd(targetNamespace);
                using var shimStringReader = new StringReader(shimXsd);
                using var shimReader = XmlReader.Create(shimStringReader, readerSettings);
                schemaSet.Add(targetNamespace, shimReader);
            }

            schemaSet.Compile();

            var validationSettings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                XmlResolver = null,
                ValidationType = ValidationType.Schema,
                Schemas = schemaSet
            };
            validationSettings.Schemas.XmlResolver = null;
            validationSettings.ValidationEventHandler += (s, e) =>
                errors.Add($"[{e.Severity}] {e.Message}");

            using (var stringReader = new StringReader(xml))
            using (var reader = XmlReader.Create(stringReader, validationSettings))
            {
                while (reader.Read())
                {
                    // drain
                }
            }

            return errors;
        }

        // Resolves the sibling <c>Schemas/w3c/</c> directory from a given
        // MTConnect XSD path (e.g. <c>…/Schemas/v2_7/MTConnectDevices_2.7.xsd</c>).
        private static string GetW3cDir(string xsdPath)
        {
            var schemaVersionDir = Path.GetDirectoryName(xsdPath)
                ?? throw new DirectoryNotFoundException($"Cannot derive directory from XSD path '{xsdPath}'.");
            var schemasRoot = Path.GetDirectoryName(schemaVersionDir)
                ?? throw new DirectoryNotFoundException($"Cannot derive Schemas root from '{schemaVersionDir}'.");
            return Path.Combine(schemasRoot, "w3c");
        }

        private static void AddSchemaFromFile(XmlSchemaSet schemaSet, XmlReaderSettings settings, string path)
        {
            using var fileStream = File.OpenRead(path);
            using var reader = XmlReader.Create(fileStream, settings, path);
            schemaSet.Add(null, reader);
        }

        // Extracts the targetNamespace attribute from an XSD source string.
        // Returns the empty string when no targetNamespace is declared
        // (chameleon schemas) — the caller treats that as "no shim needed".
        private static string ExtractTargetNamespace(string xsdSource)
        {
            using var sr = new StringReader(xsdSource);
            using var reader = XmlReader.Create(sr, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                XmlResolver = null
            });
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element &&
                    reader.LocalName == "schema" &&
                    reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
                {
                    return reader.GetAttribute("targetNamespace") ?? string.Empty;
                }
            }
            return string.Empty;
        }

        // Builds an in-memory sidecar XSD in <paramref name="targetNamespace"/>
        // that declares <c>XYZEntryType</c> as a non-abstract restriction of
        // <c>ThreeDimensionalEntryType</c>. The schema is added to the same
        // namespace as the MTConnect XSD under test, giving instance
        // documents a concrete <c>xsi:type</c> target for <c>Entry</c>
        // elements inside an <c>XYZDataSetType</c>.
        private static string BuildEntryTypeShimXsd(string targetNamespace)
        {
            return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema""
           xmlns:mt=""{targetNamespace}""
           targetNamespace=""{targetNamespace}""
           elementFormDefault=""qualified""
           attributeFormDefault=""unqualified"">
  <xs:complexType name=""XYZEntryType"">
    <xs:simpleContent>
      <xs:restriction base=""mt:ThreeDimensionalEntryType""/>
    </xs:simpleContent>
  </xs:complexType>
</xs:schema>";
        }
    }
}
