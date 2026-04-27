// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json;
using MTConnect.Observations;
using MTConnect.Streams.Json;
using NUnit.Framework;

namespace MTConnect.NET_JSON_cppagent_Tests.E2E
{
    /// <summary>
    /// Wire-format end-to-end tests that exercise the full
    /// <c>IObservation -> JsonSampleValue -> JsonSerializer</c> path used
    /// by the JSON-cppagent HTTP and MQTT formatters. These tests do not
    /// require Docker; Docker-gated SHDR -> agent -> MQTT round-trip
    /// scenarios live in a separate compliance project and are out of
    /// scope here.
    ///
    /// Sources:
    ///   - XSD: https://schemas.mtconnect.org/schemas/MTConnectStreams_2.7.xsd
    ///       FloatSampleValueType simpleType (xs:union).
    ///   - Reference: https://github.com/mtconnect/cppagent v2.7.0.7
    ///       (JsonPrinter Sample-value formatting on a current request).
    ///   - Issue: https://github.com/TrakHound/MTConnect.NET/issues/129
    /// </summary>
    [Category("NumericSampleAsJsonNumber")]
    [TestFixture]
    public class SampleValueWireFormatE2ETests
    {
        private static SampleValueObservation NewSample(string dataItemId, string result)
        {
            var obs = new SampleValueObservation
            {
                DataItemId = dataItemId,
                Type = "Temperature",
                Result = result,
            };
            return obs;
        }

        private static JsonElement Serialize(IObservation observation)
        {
            var sample = new JsonSampleValue(observation);
            var json = JsonFunctions.Convert(sample);
            using var doc = JsonDocument.Parse(json!);
            return doc.RootElement.GetProperty("value").Clone();
        }

        [TestCase("temp", "863.7060")]
        [TestCase("temp", "0.0001")]
        [TestCase("rotary-velocity", "12345")]
        [TestCase("amperage", "-5.5")]
        public void Numeric_observation_serializes_as_json_number_token(string dataItemId, string result)
        {
            var token = Serialize(NewSample(dataItemId, result));

            Assert.That(token.ValueKind, Is.EqualTo(JsonValueKind.Number),
                $"DataItem '{dataItemId}' result '{result}' should serialize as a JSON number token; " +
                $"got {token.ValueKind} instead.");
        }

        [Test]
        public void Unavailable_observation_serializes_as_json_string_token()
        {
            var token = Serialize(NewSample("temp", Observation.Unavailable));

            Assert.That(token.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(token.GetString(), Is.EqualTo("UNAVAILABLE"));
        }

        [Test]
        public void Three_space_observation_preserves_string_token()
        {
            // ThreeSpaceSampleValueType encodes three numerics
            // space-separated. cppagent emits this as a JSON string;
            // MT.NET matches.
            var token = Serialize(NewSample("position", "1.5 -2.5 3.5"));

            Assert.That(token.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(token.GetString(), Is.EqualTo("1.5 -2.5 3.5"));
        }

        [Test]
        public void Numeric_value_round_trips_to_observation()
        {
            // Pin the round-trip path: serialize a numeric observation,
            // deserialize it back, and confirm the Result is preserved.
            var observation = NewSample("temp", "42.5");
            var sample = new JsonSampleValue(observation);
            var json = JsonFunctions.Convert(sample);
            Assert.That(json, Is.Not.Null);

            var roundTripped = JsonSerializer.Deserialize<JsonSampleValue>(json!);
            Assert.That(roundTripped, Is.Not.Null);
            Assert.That(roundTripped!.Value, Is.Not.Null);

            // The converter's Read returns a boxed double for a number
            // token; the carrier's ToObservation(...) will then call
            // Value.ToString() which formats invariantly.
            Assert.That(roundTripped.Value, Is.InstanceOf<double>());
            Assert.That((double)roundTripped.Value!, Is.EqualTo(42.5).Within(1e-9));
        }
    }
}
