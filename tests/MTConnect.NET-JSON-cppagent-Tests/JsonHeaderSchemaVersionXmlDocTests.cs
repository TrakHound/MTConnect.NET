// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace MTConnect.Tests.JsonCppagent
{
    /// <summary>
    /// Pins the XML-doc summary requirement on the SchemaVersion property in
    /// the three JSON-cppagent header DTO files. Each property must carry a
    /// `<summary>` block immediately above the `[JsonPropertyName("schemaVersion")]`
    /// attribute so consumers reading IntelliSense see the cppagent v2
    /// wire-shape semantics inline.
    ///
    /// Files covered:
    /// - libraries/MTConnect.NET-JSON-cppagent/Assets/JsonAssetsHeader.cs
    /// - libraries/MTConnect.NET-JSON-cppagent/Streams/JsonStreamsHeader.cs
    /// - libraries/MTConnect.NET-JSON-cppagent/Devices/JsonDevicesHeader.cs
    /// </summary>
    [TestFixture]
    public class JsonHeaderSchemaVersionXmlDocTests
    {
        private static readonly string[] HeaderFileRelativePaths =
        {
            "libraries/MTConnect.NET-JSON-cppagent/Assets/JsonAssetsHeader.cs",
            "libraries/MTConnect.NET-JSON-cppagent/Streams/JsonStreamsHeader.cs",
            "libraries/MTConnect.NET-JSON-cppagent/Devices/JsonDevicesHeader.cs",
        };

        private static string FindRepoRoot()
        {
            var dir = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
            while (dir != null)
            {
                if (Directory.Exists(Path.Combine(dir.FullName, "libraries")) &&
                    Directory.Exists(Path.Combine(dir.FullName, "tests")))
                {
                    return dir.FullName;
                }
                dir = dir.Parent;
            }
            Assert.Fail("Could not locate repo root from test working directory.");
            return string.Empty;
        }

        [TestCaseSource(nameof(HeaderFileRelativePaths))]
        public void Header_dto_schemaVersion_has_xmldoc_summary(string relativePath)
        {
            var fullPath = Path.Combine(FindRepoRoot(), relativePath);
            Assert.That(File.Exists(fullPath), Is.True, $"Header source file not found at {fullPath}.");

            var text = File.ReadAllText(fullPath);

            // Match a `<summary>...</summary>` block followed (with optional
            // whitespace and other attributes) by `[JsonPropertyName("schemaVersion")]`.
            var pattern = new Regex(
                @"<summary>[\s\S]+?</summary>\s*(?:///[^\n]*\n\s*)*\[JsonPropertyName\(""schemaVersion""\)\]",
                RegexOptions.Multiline);

            Assert.That(pattern.IsMatch(text), Is.True,
                $"{relativePath} must precede [JsonPropertyName(\"schemaVersion\")] with a <summary> XML-doc block.");
        }
    }
}
