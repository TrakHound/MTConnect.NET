// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Globalization;
using System.Text.Json;
using MTConnect.Observations;
using MTConnect.Streams.Json;
using NUnit.Framework;

namespace MTConnect.NET_JSON_cppagent_Tests.Regressions
{
    /// <summary>
    /// Regression pin for
    /// https://github.com/TrakHound/MTConnect.NET/issues/129 — JSON-cppagent
    /// formatter previously emitted numeric Sample values as JSON string
    /// tokens, breaking parity with the cppagent reference implementation
    /// and the XSD <c>FloatSampleValueType</c> union (xs:float |
    /// "UNAVAILABLE").
    ///
    /// Sources:
    ///   - XSD: https://schemas.mtconnect.org/schemas/MTConnectStreams_2.7.xsd
    ///   - Reference: https://github.com/mtconnect/cppagent v2.7.0.7
    ///   - Issue: https://github.com/TrakHound/MTConnect.NET/issues/129
    /// </summary>
    [TestFixture]
    public class Issue129_NumericSampleNumberTokenTests
    {
        private static JsonElement Serialize(object value)
        {
            var sample = new JsonSampleValue { Value = value };
            var json = JsonFunctions.Convert(sample);
            using var doc = JsonDocument.Parse(json!);
            return doc.RootElement.GetProperty("value").Clone();
        }

        [TestCase("1586.66", 1586.66)]
        [TestCase("-42.0", -42.0)]
        [TestCase("0", 0.0)]
        public void Float_sample_emits_as_json_number(string input, double expected)
        {
            var token = Serialize(input);

            Assert.That(token.ValueKind, Is.EqualTo(JsonValueKind.Number));
            Assert.That(token.GetDouble(), Is.EqualTo(expected).Within(1e-9));
        }

        [Test]
        public void Unavailable_still_emits_as_string()
        {
            var token = Serialize(Observation.Unavailable);

            Assert.That(token.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(token.GetString(), Is.EqualTo("UNAVAILABLE"));
        }

        [TestCase("0 0 0")]
        [TestCase("1.5 -2.5 3.5")]
        public void ThreeSpace_string_value_stays_a_string_token(string input)
        {
            // ThreeSpaceSampleValueType is space-separated; the whole
            // payload is non-numeric so the converter preserves it as a
            // string token. cppagent itself emits the same shape.
            var token = Serialize(input);

            Assert.That(token.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(token.GetString(), Is.EqualTo(input));
        }

        [Test]
        public void Boxed_double_emits_as_json_number()
        {
            var token = Serialize(3.14159);

            Assert.That(token.ValueKind, Is.EqualTo(JsonValueKind.Number));
            Assert.That(token.GetDouble(), Is.EqualTo(3.14159).Within(1e-9));
        }

        [Test]
        public void Invariant_culture_parsing_is_unaffected_by_thread_culture()
        {
            // Pin invariant-culture behavior: a comma-decimal locale must
            // not turn "42.5" into a string token.
            var prior = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("de-DE");
                var token = Serialize("42.5");

                Assert.That(token.ValueKind, Is.EqualTo(JsonValueKind.Number));
                Assert.That(token.GetDouble(), Is.EqualTo(42.5).Within(1e-9));
            }
            finally
            {
                CultureInfo.CurrentCulture = prior;
            }
        }
    }
}
