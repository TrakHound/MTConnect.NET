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

            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                XmlResolver = null,
                ValidationType = ValidationType.Schema
            };

            settings.Schemas.XmlResolver = null;
            using (var schemaReader = XmlReader.Create(xsdPath, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                XmlResolver = null
            }))
            {
                settings.Schemas.Add(null, schemaReader);
            }

            settings.ValidationEventHandler += (s, e) =>
                errors.Add($"[{e.Severity}] {e.Message}");

            using (var stringReader = new StringReader(xml))
            using (var reader = XmlReader.Create(stringReader, settings))
            {
                while (reader.Read())
                {
                    // drain
                }
            }

            return errors;
        }
    }
}
