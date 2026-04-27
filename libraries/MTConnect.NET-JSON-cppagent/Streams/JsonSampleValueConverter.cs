// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using MTConnect.Observations;

namespace MTConnect.NET_JSON_cppagent.Streams
{
    /// <summary>
    /// Serializes a Sample observation's <c>value</c> property to the JSON
    /// token kind expected by the cppagent reference implementation:
    ///
    /// - numeric payloads (numeric primitives or strings that parse as a
    ///   <see cref="double"/> using <see cref="CultureInfo.InvariantCulture"/>)
    ///   are written as JSON number tokens via
    ///   <see cref="Utf8JsonWriter.WriteNumberValue(double)"/>.
    /// - the <see cref="Observation.Unavailable"/> sentinel ("UNAVAILABLE")
    ///   is written as a JSON string token.
    /// - any other non-numeric payload (e.g. a space-separated three-space
    ///   sample, a category enum string) is written as a JSON string token,
    ///   preserving backward compatibility for non-numeric Sample carriers.
    ///
    /// Read returns the boxed <see cref="double"/> for a JSON number token
    /// and the unboxed string for a JSON string token. Any other token
    /// kind (boolean, array, object) signals feed corruption and surfaces
    /// as a <see cref="JsonException"/> so the failure is visible to the
    /// caller instead of being silently dropped.
    /// </summary>
    public class JsonSampleValueConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Number:
                    return reader.GetDouble();
                case JsonTokenType.String:
                    return reader.GetString();
                default:
                    throw new JsonException(
                        $"Unsupported token kind '{reader.TokenType}' for a Sample value; " +
                        "expected a JSON number or string token.");
            }
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            // Sentinel always writes as a string token, never as a number,
            // even though "UNAVAILABLE" cannot parse as a double anyway. Keep
            // the explicit branch so the contract is unambiguous to readers.
            if (value is string s)
            {
                if (s == Observation.Unavailable)
                {
                    writer.WriteStringValue(s);
                    return;
                }

                // Empty / whitespace-only strings serialize as a JSON null
                // token: the carrier omits the property entirely for null
                // Values via JsonIgnoreCondition.WhenWritingDefault, so the
                // wire shape stays consistent with "no value supplied".
                if (string.IsNullOrWhiteSpace(s))
                {
                    writer.WriteNullValue();
                    return;
                }

                // ThreeSpaceSampleValueType payloads contain a space — short
                // circuit before double.TryParse walks the whole string so
                // multi-component values stay on the cheap path.
                if (s.IndexOf(' ') < 0
                    && double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
                    && double.IsFinite(parsed))
                {
                    writer.WriteNumberValue(parsed);
                    return;
                }

                writer.WriteStringValue(s);
                return;
            }

            // Numeric primitives serialize as JSON number tokens. Cover the
            // common cases explicitly so the converter does not depend on a
            // round-trip through ToString().
            switch (value)
            {
                case double d: writer.WriteNumberValue(d); return;
                case float f: writer.WriteNumberValue(f); return;
                case decimal m: writer.WriteNumberValue(m); return;
                case int i: writer.WriteNumberValue(i); return;
                case long l: writer.WriteNumberValue(l); return;
                case uint u: writer.WriteNumberValue(u); return;
                case ulong ul: writer.WriteNumberValue(ul); return;
                case short sh: writer.WriteNumberValue(sh); return;
                case ushort us: writer.WriteNumberValue(us); return;
                case byte b: writer.WriteNumberValue(b); return;
                case sbyte sb: writer.WriteNumberValue(sb); return;
            }

            // Final fallback: a non-string, non-numeric payload (e.g. a
            // bool or a custom struct). Format invariantly and emit as a
            // string token so the wire shape stays predictable.
            writer.WriteStringValue(Convert.ToString(value, CultureInfo.InvariantCulture));
        }
    }
}
