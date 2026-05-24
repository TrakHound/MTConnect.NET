// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace MTConnect.Compliance.Tests.L1_XsdValidation
{
    /// <summary>
    /// Parses each XSD on the <see cref="SchemaLoadTests.Xsd11BlockedSchemas"/>
    /// roster as raw XML and asserts the XSD 1.1 features the schema is
    /// documented to carry are present and well-formed. Pairs with
    /// <see cref="SchemaLoadTests"/>: the load fixture proves the
    /// 1.0-compatible subset compiles cleanly after
    /// <c>MTConnect.XsdPreprocessor</c>; this fixture proves the 1.1-only
    /// constructs the preprocessor strips are actually there to strip.
    /// Together the two fixtures fence the schema source against silent
    /// upstream drift in either direction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// XSD 1.1 features surveyed (from §3.10 of the XSD 1.1 spec):
    /// </para>
    /// <list type="bullet">
    /// <item><c>xs:any[@notNamespace]</c> — negative-namespace wildcard
    /// on element / attribute particles. Dominant in Assets / Devices
    /// from v1.3 onwards.</item>
    /// <item><c>xs:any</c> as a particle inside <c>xs:all</c> — the
    /// XSD 1.1 §3.8.6 group-all extension.</item>
    /// <item><c>xs:element[@maxOccurs!='1']</c> inside <c>xs:all</c> —
    /// the XSD 1.1 cardinality relaxation. Dominant in Streams from
    /// v1.7 onwards.</item>
    /// <item><c>xs:all</c> directly inside <c>xs:extension</c> /
    /// <c>xs:restriction</c> — the XSD 1.1 derivation extension.
    /// Present in Assets v1.7+.</item>
    /// </list>
    /// <para>
    /// Not surveyed (absent from current MTConnect XSDs):
    /// <c>xs:assert</c>, <c>xs:override</c>, <c>notQName</c>. The
    /// preprocessor handles them for forward-compatibility but the
    /// shipped schemas do not yet exercise them. A sentinel test below
    /// fails if that changes — so the absence assumption is testable.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class Xsd11FeaturePresenceTests
    {
        private const string XsdNamespace = "http://www.w3.org/2001/XMLSchema";
        private const string ResourcePrefix = "MTConnect.Compliance.Tests.Schemas.";

        public static IEnumerable<TestCaseData> Xsd11BlockedDisplayPaths()
        {
            // Drive the parametrisation off SchemaLoadTests.Xsd11BlockedSchemas
            // so this fixture and the load fixture share a single source
            // of truth for which schemas need 1.1 handling.
            foreach (var displayPath in SchemaLoadTests.Xsd11BlockedSchemas.OrderBy(s => s, StringComparer.Ordinal))
            {
                yield return new TestCaseData(displayPath)
                    .SetName($"XSD 1.1 markers present: {displayPath}");
            }
        }

        // Each schema on the XsdLoadStrict roster must carry at least one
        // XSD 1.1 feature — otherwise it was tagged unnecessarily. The
        // OR-disjunction across known XSD 1.1 markers covers per-family
        // editorial spread (Assets/Devices: notNamespace; Streams: xs:all
        // + maxOccurs > 1; Error: xs:any inside xs:all) without per-family
        // case-pinning that would fragility-couple to upstream's editing
        // style.
        [TestCaseSource(nameof(Xsd11BlockedDisplayPaths))]
        public void Schema_carries_at_least_one_xsd_1_1_feature(string displayPath)
        {
            var doc = LoadSchemaAsXDocument(displayPath);
            Assert.That(doc.Root, Is.Not.Null, $"{displayPath}: schema root element missing.");

            var ns = (XNamespace)XsdNamespace;
            var anyName = ns + "any";
            var allName = ns + "all";
            var elementName = ns + "element";
            var extensionName = ns + "extension";
            var restrictionName = ns + "restriction";
            var anyAttributeName = ns + "anyAttribute";

            var hasNotNamespace = doc.Descendants()
                .Where(e => e.Name == anyName || e.Name == anyAttributeName)
                .Any(e => e.Attribute("notNamespace") != null);

            var hasAnyInsideAll = doc.Descendants(allName)
                .SelectMany(a => a.Elements(anyName))
                .Any();

            var hasUnboundedAllElement = doc.Descendants(allName)
                .SelectMany(a => a.Elements(elementName))
                .Any(e => e.Attribute("maxOccurs") != null
                          && e.Attribute("maxOccurs")!.Value != "0"
                          && e.Attribute("maxOccurs")!.Value != "1");

            var hasAllInDerivation = doc.Descendants(allName)
                .Any(a => a.Parent != null
                          && (a.Parent.Name == extensionName || a.Parent.Name == restrictionName));

            var markers = new List<string>();
            if (hasNotNamespace) markers.Add("xs:any[@notNamespace]");
            if (hasAnyInsideAll) markers.Add("xs:all > xs:any");
            if (hasUnboundedAllElement) markers.Add("xs:all > xs:element[maxOccurs>1]");
            if (hasAllInDerivation) markers.Add("xs:all inside xs:extension/xs:restriction");

            Assert.That(markers, Is.Not.Empty,
                $"{displayPath}: schema is on the XsdLoadStrict roster but carries no "
                + "detectable XSD 1.1 features. Either drop the roster tag (the BCL "
                + "reader handles this schema unaided) or update the marker survey if "
                + "a new XSD 1.1 construct appeared.");
        }

        // Sanity test: xs:assert and xs:override are not present in any
        // current MTConnect XSD. If a future spec revision introduces
        // them, this test will fail and the preprocessor's handling can
        // be exercised end-to-end (the preprocessor already strips both;
        // the test surfaces that the schemas now exercise the path).
        [Test]
        public void No_schema_carries_xs_assert_or_xs_override_yet()
        {
            var ns = (XNamespace)XsdNamespace;
            var assertName = ns + "assert";
            var overrideName = ns + "override";

            var offenders = new List<string>();
            foreach (var displayPath in SchemaLoadTests.Xsd11BlockedSchemas)
            {
                var doc = LoadSchemaAsXDocument(displayPath);
                if (doc.Descendants(assertName).Any()
                    || doc.Descendants(overrideName).Any())
                {
                    offenders.Add(displayPath);
                }
            }

            Assert.That(offenders, Is.Empty,
                "An upstream XSD now carries xs:assert or xs:override. The preprocessor "
                + "already strips both, but this sentinel should be removed and the "
                + "Schema_carries_at_least_one_xsd_1_1_feature OR-set should add the new "
                + "marker. Offending schemas: " + string.Join(", ", offenders));
        }

        // xs:any with notNamespace, where present, must carry the
        // '##targetNamespace' token (the negation of the schema's own
        // target namespace) — i.e., "any extension element not in MY
        // namespace". A future WG-side change to '##other' or to a
        // literal URI list is structurally fine but worth pinning so a
        // typo (e.g. '#targetNamespace') surfaces here.
        [Test]
        public void notNamespace_attribute_value_is_targetNamespace_token()
        {
            var ns = (XNamespace)XsdNamespace;
            var anyName = ns + "any";
            var anyAttributeName = ns + "anyAttribute";

            var offenders = new List<string>();
            foreach (var displayPath in SchemaLoadTests.Xsd11BlockedSchemas)
            {
                var doc = LoadSchemaAsXDocument(displayPath);
                var wildcards = doc.Descendants()
                    .Where(e => e.Name == anyName || e.Name == anyAttributeName)
                    .Where(e => e.Attribute("notNamespace") != null);
                foreach (var wildcard in wildcards)
                {
                    var value = wildcard.Attribute("notNamespace")!.Value;
                    if (value != "##targetNamespace")
                    {
                        offenders.Add($"{displayPath}: notNamespace='{value}' (expected '##targetNamespace')");
                    }
                }
            }

            Assert.That(offenders, Is.Empty,
                "A notNamespace attribute uses an unexpected value. The preprocessor still "
                + "strips it correctly, but the WG editorial convention has drifted. "
                + "Offenders:\n  - " + string.Join("\n  - ", offenders));
        }

        private static XDocument LoadSchemaAsXDocument(string displayPath)
        {
            // displayPath is e.g. "v1_7/MTConnectAssets_1.7.xsd"; the
            // manifest name flattens '/' to '.' and prepends the
            // resource prefix.
            var manifestName = ResourcePrefix + displayPath.Replace('/', '.');

            var asm = typeof(Xsd11FeaturePresenceTests).Assembly;
            using var stream = asm.GetManifestResourceStream(manifestName);
            if (stream == null)
            {
                throw new FileNotFoundException(
                    $"Embedded XSD resource not found: {manifestName} (from displayPath '{displayPath}'). " +
                    "Check the EmbeddedResource glob in MTConnect-Compliance-Tests.csproj.");
            }
            return XDocument.Load(stream, LoadOptions.None);
        }
    }
}
