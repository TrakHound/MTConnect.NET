// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Globalization;
using System.Text.Json;
using MTConnect.Observations;
using MTConnect.Streams.Json;
using NUnit.Framework;

namespace MTConnect.NET_JSON_cppagent_Tests.Streams
{
    /// <summary>
    /// Pins the cppagent-JSON Sample-value token kind:
    ///   - numeric Sample values serialize as JSON number tokens
    ///   - the "UNAVAILABLE" sentinel still serializes as a JSON string token
    ///
    /// Sources:
    ///   - XSD: https://schemas.mtconnect.org/schemas/MTConnectStreams_2.7.xsd
    ///       FloatSampleValueType simpleType (xs:union of xs:float and the
    ///       enumeration containing only "UNAVAILABLE").
    ///   - Prose: https://docs.mtconnect.org Part_2.0 section "Sample Value
    ///       Types" - numeric Sample values are floating-point values.
    ///   - Reference: https://github.com/mtconnect/cppagent v2.7.0.7
    ///       (JsonPrinter Sample-value formatting) - emits a number token.
    /// </summary>
    [Category("NumericSampleNumberToken")]
    [TestFixture]
    public class JsonSampleValueNumericTokenTests
    {
        private static JsonElement SerializeAndGetValue(object value)
        {
            var sample = new JsonSampleValue { Value = value };
            var json = JsonFunctions.Convert(sample);
            Assert.That(json, Is.Not.Null, "JsonFunctions.Convert returned null");
            using var doc = JsonDocument.Parse(json!);
            return doc.RootElement.GetProperty("value").Clone();
        }

        [TestCase("0")]
        [TestCase("42.5")]
        [TestCase("-17.0")]
        [TestCase("1586.66")]
        [TestCase("0.000001")]
        [TestCase("1e6")]
        public void Numeric_string_sample_value_emits_number_token(string numericLiteral)
        {
            var token = SerializeAndGetValue(numericLiteral);

            Assert.That(token.ValueKind, Is.EqualTo(JsonValueKind.Number),
                $"Expected JSON number token, got {token.ValueKind} for '{numericLiteral}'");
            var expected = double.Parse(numericLiteral, CultureInfo.InvariantCulture);
            Assert.That(token.GetDouble(), Is.EqualTo(expected).Within(1e-9));
        }

        [TestCase(42.5)]
        [TestCase(-17.0)]
        [TestCase(0.0)]
        public void Double_sample_value_emits_number_token(double numeric)
        {
            var token = SerializeAndGetValue(numeric);

            Assert.That(token.ValueKind, Is.EqualTo(JsonValueKind.Number));
            Assert.That(token.GetDouble(), Is.EqualTo(numeric).Within(1e-9));
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(2147483647)]
        public void Integer_sample_value_emits_number_token(int numeric)
        {
            var token = SerializeAndGetValue(numeric);

            Assert.That(token.ValueKind, Is.EqualTo(JsonValueKind.Number));
            Assert.That(token.GetInt32(), Is.EqualTo(numeric));
        }

        [Test]
        public void Three_space_string_sample_value_emits_string_token()
        {
            // ThreeSpaceSampleValueType is space-separated (e.g. "1.5 -2.5 3.5").
            // The whole string is non-numeric; preserve the string token to
            // round-trip the value as cppagent does.
            var token = SerializeAndGetValue("1.5 -2.5 3.5");

            Assert.That(token.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(token.GetString(), Is.EqualTo("1.5 -2.5 3.5"));
        }

        [Test]
        public void Unavailable_sample_value_emits_string_token()
        {
            var token = SerializeAndGetValue(Observation.Unavailable);

            Assert.That(token.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(token.GetString(), Is.EqualTo("UNAVAILABLE"));
        }

        [Test]
        public void Null_sample_value_is_omitted()
        {
            var sample = new JsonSampleValue { Value = null };
            var json = JsonFunctions.Convert(sample);
            using var doc = JsonDocument.Parse(json!);
            Assert.That(doc.RootElement.TryGetProperty("value", out _), Is.False,
                "Null Value should be omitted by JsonIgnoreCondition.WhenWritingDefault");
        }
    }
}
