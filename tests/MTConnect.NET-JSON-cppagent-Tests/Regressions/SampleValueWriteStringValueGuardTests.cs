// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Text.RegularExpressions;
using MTConnect.NET_JSON_cppagent_Tests.TestHelpers;
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
        [Test]
        public void JsonSampleValue_does_not_write_value_directly_as_string()
        {
            var root = RepoRootLocator.LocateRoot();
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
            var root = RepoRootLocator.LocateRoot();
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
