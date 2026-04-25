using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    // Tests are parametric on every .xsd file under Schemas/v*/. Adding
    // a new version-directory automatically extends coverage.
    [TestFixture]
    [Category("XsdLoadStrict")]
    public class SchemaLoadTests
    {
        public static IEnumerable<TestCaseData> AllSchemas()
        {
            var schemasRoot = Path.Combine(TestContext.CurrentContext.TestDirectory, "Schemas");
            if (!Directory.Exists(schemasRoot))
            {
                // No schemas tree at runtime — fall back to the project
                // tree so the test still surfaces meaningfully when run
                // outside a packaged build output.
                schemasRoot = LocateSchemasInProject();
            }

            if (!Directory.Exists(schemasRoot))
            {
                yield return new TestCaseData("(none)").SetName("Schemas tree missing");
                yield break;
            }

            foreach (var file in Directory.GetFiles(schemasRoot, "*.xsd", SearchOption.AllDirectories).OrderBy(f => f))
            {
                var rel = Path.GetRelativePath(schemasRoot, file);
                yield return new TestCaseData(file).SetName($"XSD load: {rel}");
            }
        }

        private static string LocateSchemasInProject()
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(typeof(SchemaLoadTests).Assembly.Location)!);
            while (dir != null)
            {
                var candidate = Path.Combine(dir.FullName, "Schemas");
                if (Directory.Exists(candidate)) return candidate;

                var alt = Path.Combine(dir.FullName, "tests", "Compliance", "MTConnect-Compliance-Tests", "Schemas");
                if (Directory.Exists(alt)) return alt;

                dir = dir.Parent;
            }
            return string.Empty;
        }

        [TestCaseSource(nameof(AllSchemas))]
        public void Schema_loads_without_errors(string xsdPath)
        {
            if (xsdPath == "(none)")
            {
                Assert.Inconclusive("Schemas tree not present at runtime; copy Schemas/ to test output via csproj or place at the project root.");
                return;
            }

            var errors = new List<string>();

            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                // Defence-in-depth: refuse to fetch external entities (DTDs,
                // included XSDs over http(s)) at parse time. See OWASP XXE.
                XmlResolver = null
            };

            // Open the XSD file. XmlSchemaSet.Add resolves any <xs:include>
            // / <xs:import> relative to the file's directory.
            using var reader = XmlReader.Create(xsdPath, settings);
            var schemaSet = new XmlSchemaSet
            {
                // Defence-in-depth: prevent SchemaSet from fetching external
                // <xs:include> / <xs:import> URIs over the network. The
                // shipped Schemas/ tree is fully self-contained; an XSD that
                // pulls in (e.g.) the W3C xlink schema can do so via an
                // XmlPreloadedResolver pre-seeded with that XSD when the
                // L1 layer starts validating xlink-importing envelopes.
                XmlResolver = null
            };
            schemaSet.ValidationEventHandler += (s, e) => errors.Add($"[{e.Severity}] {e.Message}");
            schemaSet.Add(null, reader);
            schemaSet.Compile();

            Assert.That(errors, Is.Empty,
                $"XSD '{Path.GetFileName(xsdPath)}' failed to load:\n  - " + string.Join("\n  - ", errors));
        }
    }
}
