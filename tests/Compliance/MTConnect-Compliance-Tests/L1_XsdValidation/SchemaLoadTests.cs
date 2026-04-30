using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;

namespace MTConnect.Compliance.Tests.L1_XsdValidation
{
    // Loads every MTConnect XSD shipped under Schemas/v*/ and reports any
    // schema-load errors. This is the most basic compliance gate — if a
    // shipped XSD isn't well-formed (broken includes, missing types,
    // namespace mismatch) we surface the problem before any envelope-
    // shape validation runs.
    //
    // Tests are parametric on every .xsd file under Schemas/v*/. XSDs are
    // bundled as embedded resources (see csproj) and read via the assembly
    // manifest, so adding a new version-directory automatically extends
    // coverage on the next clean build.
    //
    // Two pre-load wrinkles are handled here:
    //
    // 1. The MTConnect XSDs from v1.3 onwards reference xlink:hrefType /
    //    xlink:type and declare an <xs:import namespace='…/xlink'
    //    schemaLocation='xlink.xsd'/> pointing at a sibling file the
    //    upstream source tree never shipped. With XmlResolver = null
    //    (defense-in-depth, OWASP XXE) the schemaLocation cannot be
    //    fetched, so this fixture seeds the W3C xlink schema (and the
    //    xml.xsd it itself imports) into the XmlSchemaSet before adding
    //    the MTConnect XSD. The targetNamespace match is what makes the
    //    import resolve without resolver traffic.
    //
    // 2. A subset of the MTConnect XSDs (mostly the v1.7+ "non-_1.0"
    //    variants and some v1.3-v1.6 variants) use XSD 1.1 features that
    //    .NET's XSD-1.0-only XmlSchemaSet rejects: <xs:any notNamespace>,
    //    maxOccurs > 1 on xs:all members, xs:any in unsupported contexts.
    //    Those XSDs are tagged [Category("XsdLoadStrict")] per case so
    //    the default-sweep CI run (Category!=XsdLoadStrict) skips them
    //    while leaving them visible to opt-in runs.
    //
    // Source for the embedded W3C XSDs:
    //   https://www.w3.org/1999/xlink.xsd
    //   https://www.w3.org/2001/xml.xsd
    [TestFixture]
    public class SchemaLoadTests
    {
        private const string ResourcePrefix = "MTConnect.Compliance.Tests.Schemas.";
        private const string XlinkResourceName = ResourcePrefix + "w3c.xlink.xsd";
        private const string XmlResourceName = ResourcePrefix + "w3c.xml.xsd";
        private const string W3cPrefix = "w3c/";
        private const string XsdLoadStrictCategory = "XsdLoadStrict";

        // XSDs whose root cause is an XSD 1.1 feature (notNamespace,
        // maxOccurs>1 on xs:all, xs:any in unsupported context). Even
        // with the W3C xlink + xml schemas pre-loaded they cannot be
        // compiled by .NET's XSD-1.0-only XmlSchemaSet. Tagged with
        // XsdLoadStrict so the default CI sweep skips them; opt-in
        // runs see the genuine blocker.
        //
        // Display paths use the format produced by ToDisplayPath
        // (e.g. "v1_7/MTConnectDevices_1.7.xsd").
        private static readonly HashSet<string> Xsd11BlockedSchemas = new(StringComparer.Ordinal)
        {
            "v1_3/MTConnectAssets_1.3.xsd",
            "v1_3/MTConnectDevices_1.3.xsd",
            "v1_4/MTConnectAssets_1.4.xsd",
            "v1_4/MTConnectDevices_1.4.xsd",
            "v1_5/MTConnectAssets_1.5.xsd",
            "v1_5/MTConnectDevices_1.5.xsd",
            "v1_6/MTConnectAssets_1.6.xsd",
            "v1_6/MTConnectDevices_1.6.xsd",
            "v1_7/MTConnectAssets_1.7.xsd",
            "v1_7/MTConnectDevices_1.7.xsd",
            "v1_7/MTConnectError_1.7.xsd",
            "v1_7/MTConnectStreams_1.7.xsd",
            "v1_8/MTConnectAssets_1.8.xsd",
            "v1_8/MTConnectDevices_1.8.xsd",
            "v1_8/MTConnectError_1.8.xsd",
            "v1_8/MTConnectStreams_1.8.xsd",
            "v2_0/MTConnectAssets_2.0.xsd",
            "v2_0/MTConnectDevices_2.0.xsd",
            "v2_1/MTConnectAssets_2.1.xsd",
            "v2_1/MTConnectDevices_2.1.xsd",
            "v2_2/MTConnectAssets_2.2.xsd",
            "v2_2/MTConnectDevices_2.2.xsd",
            "v2_3/MTConnectAssets_2.3.xsd",
            "v2_3/MTConnectDevices_2.3.xsd",
            "v2_4/MTConnectAssets_2.4.xsd",
            "v2_4/MTConnectDevices_2.4.xsd",
            "v2_5/MTConnectAssets_2.5.xsd",
            "v2_5/MTConnectDevices_2.5.xsd",
            "v2_6/MTConnectAssets_2.6.xsd",
            "v2_6/MTConnectDevices_2.6.xsd",
            "v2_7/MTConnectAssets_2.7.xsd",
            "v2_7/MTConnectDevices_2.7.xsd"
        };

        public static IEnumerable<TestCaseData> AllSchemas()
        {
            var asm = typeof(SchemaLoadTests).Assembly;

            var names = asm.GetManifestResourceNames()
                .Where(n => n.StartsWith(ResourcePrefix, StringComparison.Ordinal)
                            && n.EndsWith(".xsd", StringComparison.Ordinal))
                .OrderBy(n => n, StringComparer.Ordinal)
                .ToList();

            if (names.Count == 0)
            {
                yield return new TestCaseData("(none)").SetName("Schemas tree missing");
                yield break;
            }

            foreach (var name in names)
            {
                var displayPath = ToDisplayPath(name);

                // The W3C XSDs ship as helper resources for the import-
                // resolution wiring, not as MTConnect XSDs in their own
                // right. Skip them from the test sweep — they are
                // exercised indirectly every time an MTConnect XSD is
                // loaded.
                if (displayPath.StartsWith(W3cPrefix, StringComparison.Ordinal))
                    continue;

                var data = new TestCaseData(name).SetName($"XSD load: {displayPath}");
                if (Xsd11BlockedSchemas.Contains(displayPath))
                    data.SetCategory(XsdLoadStrictCategory);

                yield return data;
            }
        }

        // Reverse-map a manifest name like
        //   MTConnect.Compliance.Tests.Schemas.v2_7.MTConnectStreams_2.7.xsd
        // to a display path like
        //   v2_7/MTConnectStreams_2.7.xsd
        // The mapping relies on the on-disk convention that version
        // directories use underscore separators (e.g. v2_7, never v2.7),
        // so the first dot after the resource prefix always terminates the
        // version-dir segment regardless of how many dots the filename has.
        private static string ToDisplayPath(string resourceName)
        {
            var rest = resourceName.Substring(ResourcePrefix.Length);
            var firstDot = rest.IndexOf('.');
            if (firstDot < 0) return rest;
            return rest.Substring(0, firstDot) + "/" + rest.Substring(firstDot + 1);
        }

        [TestCaseSource(nameof(AllSchemas))]
        public void Schema_loads_without_errors(string resourceName)
        {
            if (resourceName == "(none)")
            {
                Assert.Inconclusive("No XSDs found in the test assembly manifest; check the EmbeddedResource glob in MTConnect-Compliance-Tests.csproj.");
                return;
            }

            var asm = typeof(SchemaLoadTests).Assembly;
            using var stream = asm.GetManifestResourceStream(resourceName);
            Assert.That(stream, Is.Not.Null, $"Embedded resource '{resourceName}' not found.");

            var errors = new List<string>();

            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                // Defense-in-depth: refuse to fetch external entities (DTDs,
                // included XSDs over http(s)) at parse time. See OWASP XXE.
                XmlResolver = null
            };

            var schemaSet = new XmlSchemaSet
            {
                // Defense-in-depth: prevent SchemaSet from fetching external
                // <xs:include> / <xs:import> URIs. The bundled Schemas/ tree
                // is fully self-contained at build time; sibling W3C XSDs
                // (xml, xlink) are pre-loaded below so their target
                // namespaces are already known when the MTConnect XSD's
                // <xs:import> is processed.
                XmlResolver = null
            };
            schemaSet.ValidationEventHandler += (s, e) => errors.Add($"[{e.Severity}] {e.Message}");

            // Pre-load the W3C xml.xsd (target ns http://www.w3.org/XML/
            // 1998/namespace) before xlink.xsd, since xlink imports xml
            // for its xml:lang attribute reference. Order matters: xml
            // first, then xlink, then the MTConnect XSD under test.
            AddEmbeddedSchema(asm, schemaSet, settings, XmlResourceName, "w3c/xml.xsd");
            AddEmbeddedSchema(asm, schemaSet, settings, XlinkResourceName, "w3c/xlink.xsd");

            using var reader = XmlReader.Create(stream!, settings, ToDisplayPath(resourceName));
            schemaSet.Add(null, reader);
            schemaSet.Compile();

            Assert.That(errors, Is.Empty,
                $"XSD '{ToDisplayPath(resourceName)}' failed to load:\n  - " + string.Join("\n  - ", errors));
        }

        private static void AddEmbeddedSchema(System.Reflection.Assembly asm, XmlSchemaSet schemaSet, XmlReaderSettings settings, string resourceName, string baseUri)
        {
            using var s = asm.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found; check the EmbeddedResource glob in MTConnect-Compliance-Tests.csproj.");
            using var reader = XmlReader.Create(s, settings, baseUri);
            schemaSet.Add(null, reader);
        }
    }
}
