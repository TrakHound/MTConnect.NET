// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json;
using MTConnect.NET_JSON_cppagent.Streams;
using MTConnect.Streams.Json;
using NUnit.Framework;

namespace MTConnect.NET_JSON_cppagent_Tests.Streams
{
    /// <summary>
    /// Pins the converter's edge-case branches:
    ///   - non-finite numeric strings (NaN, Infinity) round-trip as JSON
    ///     string tokens, not number tokens (Utf8JsonWriter.WriteNumberValue
    ///     rejects non-finite doubles).
    ///   - empty / whitespace strings emit as a JSON null token (explicit
    ///     policy — see issue #129 review pass).
    ///   - the ThreeSpace value path short-circuits before walking each
    ///     character to parse as double.
    ///   - the non-string/non-numeric fallback uses
    ///     <c>Convert.ToString(value, InvariantCulture)</c>.
    ///   - the converter Read default branch throws a JsonException for
    ///     unsupported token kinds (bool/array/object) so callers see feed
    ///     corruption instead of silent <c>null</c>s.
    ///
    /// Sources:
    ///   - XSD: https://schemas.mtconnect.org/schemas/MTConnectStreams_2.7.xsd
    ///       FloatSampleValueType simpleType (xs:union of xs:float and the
    ///       "UNAVAILABLE" enumeration).
    ///   - Reference: https://github.com/mtconnect/cppagent v2.7.0.7
    ///       (JsonPrinter Sample-value formatting).
    ///   - Issue: https://github.com/TrakHound/MTConnect.NET/issues/129
    /// </summary>
    [Category("NumericSampleAsJsonNumber")]
    [TestFixture]
    public class JsonSampleValueConverterEdgeCaseTests
    {
        private static JsonElement SerializeValue(object value)
        {
            var sample = new JsonSampleValue { Value = value };
            var json = JsonFunctions.Convert(sample);
            using var doc = JsonDocument.Parse(json!);
            return doc.RootElement.GetProperty("value").Clone();
        }

        // ---- non-finite numeric strings round-trip as JSON string tokens ----

        [TestCase("NaN")]
        [TestCase("Infinity")]
        [TestCase("-Infinity")]
        public void Non_finite_numeric_string_emits_as_string_token(string nonFiniteLiteral)
        {
            var token = SerializeValue(nonFiniteLiteral);

            Assert.That(token.ValueKind, Is.EqualTo(JsonValueKind.String),
                $"Non-finite literal '{nonFiniteLiteral}' must round-trip as a JSON " +
                "string token; emitting it as a JSON number would crash the writer.");
            Assert.That(token.GetString(), Is.EqualTo(nonFiniteLiteral));
        }

        // ---- empty / whitespace-only strings emit a JSON null token ----

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("\t")]
        [TestCase("   \t\n")]
        public void Empty_or_whitespace_string_emits_as_null_token(string blank)
        {
            // The carrier's Value property uses
            // [JsonIgnoreCondition.WhenWritingDefault] for null but NOT for
            // empty strings. Pin policy: empty / whitespace-only strings
            // emit as a JSON null token rather than a string token, so the
            // wire shape stays consistent with "no value". Apply the
            // converter directly so the carrier's IgnoreCondition does not
            // mask the policy.
            var converter = new JsonSampleValueConverter();
            var options = new JsonSerializerOptions();
            options.Converters.Add(converter);

            var json = JsonSerializer.Serialize<object?>(blank, options);

            Assert.That(json, Is.EqualTo("null"),
                $"Empty/whitespace string ('{blank}') must serialize as a JSON null " +
                "token; got '{json}' instead.");
        }

        // ---- non-string / non-numeric fallback uses InvariantCulture ----

        [Test]
        public void Bool_value_fallback_uses_invariant_culture()
        {
            // The fallback shape must be the same one
            // Convert.ToString(value, InvariantCulture) produces — pin the
            // contract so a future converter swap stays predictable across
            // locales.
            var token = SerializeValue(true);

            Assert.That(token.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(token.GetString(), Is.EqualTo("True"));
        }

        // ---- ThreeSpace value path short-circuits before TryParse(double) ----

        [TestCase("1.5 -2.5 3.5")]
        [TestCase("0 0 0")]
        [TestCase("12345 0 0")]
        public void ThreeSpace_string_emits_as_string_token(string threeSpace)
        {
            // ThreeSpaceSampleValueType payloads contain a space - the
            // converter must not feed them through double.TryParse (which
            // walks every character before failing). The wire-shape check
            // is the same as the existing TheeSpace test; the perf
            // contract is enforced by code review on the converter.
            var token = SerializeValue(threeSpace);

            Assert.That(token.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(token.GetString(), Is.EqualTo(threeSpace));
        }

        // ---- Read default branch throws JsonException on unsupported tokens ----

        [TestCase("true")]
        [TestCase("false")]
        [TestCase("[1,2,3]")]
        [TestCase("{\"k\":\"v\"}")]
        public void Read_throws_for_unsupported_token(string payload)
        {
            var converter = new JsonSampleValueConverter();
            var options = new JsonSerializerOptions();
            options.Converters.Add(converter);

            Assert.That(
                () => JsonSerializer.Deserialize<object>(payload, options),
                Throws.InstanceOf<JsonException>(),
                $"Unsupported token payload '{payload}' must surface as a " +
                "JsonException so feed corruption is visible to callers.");
        }
    }
}
