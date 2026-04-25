// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace MTConnect.NET_JSON_cppagent_Tests.Regressions
{
    /// <summary>
    /// Catches a copy-paste regression of
    /// https://github.com/TrakHound/MTConnect.NET/issues/129 by scanning the
    /// scalar Sample-value carrier file for an unconditional
    /// <c>WriteStringValue</c> call against the value's runtime payload.
    ///
    /// The guard does NOT prohibit <c>WriteStringValue</c> in general — the
    /// "UNAVAILABLE" sentinel branch and the non-numeric-string fallback
    /// branch both legitimately emit a string token. It targets the carrier
    /// surface (<c>JsonSampleValue.cs</c>) and asserts that the file does
    /// not write a string token directly from <c>sample.Value</c> /
    /// <c>this.Value</c> / <c>Value</c>.
    /// </summary>
    [TestFixture]
    public class SampleValueWriteStringValueGuardTests
    {
        private static readonly string[] CandidateRoots = new[]
        {
            // Walk up from the test assembly's bin/Debug/net8.0/ to the
            // repo root. AppContext.BaseDirectory points at that bin
            // folder; the repo root is six levels up
            // (.../tests/<proj>/bin/Debug/net8.0).
            "../../../../../",
            "../../../../",
            "../../../",
        };

        private static string LocateRepoRoot()
        {
            var bin = AppContext.BaseDirectory;
            foreach (var rel in CandidateRoots)
            {
                var candidate = Path.GetFullPath(Path.Combine(bin, rel));
                if (Directory.Exists(Path.Combine(candidate, "libraries", "MTConnect.NET-JSON-cppagent")))
                    return candidate;
            }

            throw new DirectoryNotFoundException(
                "Could not locate the libraries/MTConnect.NET-JSON-cppagent " +
                "directory from the test bin folder.");
        }

        [Test]
        public void JsonSampleValue_does_not_write_value_directly_as_string()
        {
            var root = LocateRepoRoot();
            var jsonSampleValuePath = Path.Combine(
                root, "libraries", "MTConnect.NET-JSON-cppagent", "Streams", "JsonSampleValue.cs");

            Assert.That(File.Exists(jsonSampleValuePath), Is.True,
                $"Expected {jsonSampleValuePath} to exist.");

            var source = File.ReadAllText(jsonSampleValuePath);

            // The carrier file should not call WriteStringValue at all —
            // serialization goes through JsonSampleValueConverter on the
            // [JsonConverter] attribute, never through inline writer calls
            // in the carrier.
            var pattern = new Regex(@"WriteStringValue\s*\(", RegexOptions.CultureInvariant);
            var matches = pattern.Matches(source);

            Assert.That(matches.Count, Is.EqualTo(0),
                $"Found {matches.Count} unexpected WriteStringValue call(s) in JsonSampleValue.cs " +
                "- numeric Sample values must serialize via JsonSampleValueConverter, " +
                "not inline string-token writes.");
        }

        [Test]
        public void Sample_value_carrier_uses_dedicated_converter_attribute()
        {
            var root = LocateRepoRoot();
            var jsonSampleValuePath = Path.Combine(
                root, "libraries", "MTConnect.NET-JSON-cppagent", "Streams", "JsonSampleValue.cs");

            var source = File.ReadAllText(jsonSampleValuePath);

            Assert.That(source, Does.Contain("[JsonConverter(typeof(JsonSampleValueConverter))]"),
                "JsonSampleValue.Value must be annotated with " +
                "[JsonConverter(typeof(JsonSampleValueConverter))] so numeric " +
                "Sample values emit JSON number tokens (issue #129).");
        }
    }
}
