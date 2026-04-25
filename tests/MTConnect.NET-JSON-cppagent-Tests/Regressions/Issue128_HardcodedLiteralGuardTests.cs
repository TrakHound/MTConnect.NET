// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using NUnit.Framework;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace MTConnect.NET_JSON_cppagent_Tests.Regressions
{
    /// <summary>
    /// Guard test that walks the JSON-cppagent library source tree and
    /// fails if either touched envelope file re-introduces a hardcoded
    /// <c>SchemaVersion = "&lt;literal&gt;"</c> assignment. Catches
    /// copy-paste regressions even when the parametric matrix would
    /// stay green (e.g. someone hardcodes <c>"2.5"</c> and ships it).
    /// </summary>
    [TestFixture]
    public class Issue128_HardcodedLiteralGuardTests
    {
        // SchemaVersion = "<anything>"; — string-literal assignment.
        private static readonly Regex HardcodedSchemaVersion =
            new(@"SchemaVersion\s*=\s*""[^""]+""\s*;", RegexOptions.Compiled);

        [TestCase("Streams/JsonMTConnectStreams.cs")]
        [TestCase("Devices/JsonMTConnectDevices.cs")]
        public void Source_file_must_not_hardcode_schemaVersion_literal(string relativePath)
        {
            var librarySourceDir = LocateLibrarySourceDir();
            var fullPath = Path.Combine(librarySourceDir, relativePath);

            Assert.That(File.Exists(fullPath), Is.True,
                $"expected to find library source at {fullPath} (test must run from a checked-out repo)");

            var source = File.ReadAllText(fullPath);
            var match = HardcodedSchemaVersion.Match(source);

            Assert.That(match.Success, Is.False,
                $"{relativePath} contains a hardcoded `SchemaVersion = \"...\"` literal: '{match.Value}'. " +
                "Issue #128 forbids re-introducing the hardcode — derive the value from the response document.");
        }

        private static string LocateLibrarySourceDir()
        {
            // Test binary lives at .../tests/MTConnect.NET-JSON-cppagent-Tests/bin/Debug/net8.0/.
            // Walk up to the repo root, then descend into the library.
            var dir = AppContext.BaseDirectory;
            for (var i = 0; i < 8; i++)
            {
                var candidate = Path.Combine(dir, "libraries", "MTConnect.NET-JSON-cppagent");
                if (Directory.Exists(candidate))
                {
                    return candidate;
                }
                var parent = Directory.GetParent(dir);
                if (parent == null) break;
                dir = parent.FullName;
            }
            throw new DirectoryNotFoundException("Could not locate MTConnect.NET-JSON-cppagent library source.");
        }
    }
}
