using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;

namespace MTConnect.Compliance.Tests.L1_XsdValidation
{
    // Loads every MTConnect XSD shipped under Schemas/ and reports any
    // schema-load errors. This is the most basic compliance gate — if a
    // downloaded XSD isn't well-formed (broken includes, missing types,
    // namespace mismatch) we surface the problem before any envelope-
    // shape validation runs.
    //
    // Tests are parametric on every .xsd file under Schemas/v*/. XSDs are
    // bundled as embedded resources (see csproj) and read via the assembly
    // manifest, so adding a new version-directory automatically extends
    // coverage on the next clean build.
    [TestFixture]
    [Category("XsdLoadStrict")]
    public class SchemaLoadTests
    {
        private const string ResourcePrefix = "MTConnect.Compliance.Tests.Schemas.";

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
                yield return new TestCaseData(name).SetName($"XSD load: {ToDisplayPath(name)}");
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

            using var reader = XmlReader.Create(stream!, settings, ToDisplayPath(resourceName));
            var schemaSet = new XmlSchemaSet
            {
                // Defense-in-depth: prevent SchemaSet from fetching external
                // <xs:include> / <xs:import> URIs. The bundled Schemas/ tree
                // is fully self-contained at build time but with embedded
                // resources there is no filesystem to traverse, so any
                // <xs:include schemaLocation="…"> would attempt resolution
                // through this resolver — a custom XmlResolver that streams
                // sibling resources from the manifest is the unblock for
                // the L1 layer's full include-aware validation gate. The
                // follow-up XSD-1.1 PR will provide it; the strict-load
                // failures this test surfaces document the gap.
                XmlResolver = null
            };
            schemaSet.ValidationEventHandler += (s, e) => errors.Add($"[{e.Severity}] {e.Message}");
            schemaSet.Add(null, reader);
            schemaSet.Compile();

            Assert.That(errors, Is.Empty,
                $"XSD '{ToDisplayPath(resourceName)}' failed to load:\n  - " + string.Join("\n  - ", errors));
        }
    }
}
