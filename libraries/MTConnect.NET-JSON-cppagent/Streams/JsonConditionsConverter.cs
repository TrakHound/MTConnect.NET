// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    // Serializes JsonConditions in the cppagent JSON v2 wire shape: an
    // array of single-key wrapper objects, one per Condition entry
    // (e.g. [{"Normal": {...}}, {"Warning": {...}}]). The XSD
    // ConditionListType is <xs:sequence><xs:choice maxOccurs='unbounded'>
    // of Normal|Warning|Fault|Unavailable.
    //
    // Ordering: the typed JsonConditions POCO buckets entries by level
    // (Fault, Warning, Normal, Unavailable). The Write path always emits
    // in that fixed level order (Fault first, then Warning, then Normal,
    // then Unavailable), with source order preserved within each bucket.
    // Mixed-level interleaving on the wire is therefore NOT round-trip
    // preserved: reading [{Fault:f1},{Normal:n1},{Fault:f2}] yields
    // Fault=[f1,f2], Normal=[n1] and re-serializes as
    // [{Fault:f1},{Fault:f2},{Normal:n1}]. Round-trip byte-identity
    // holds only when each level's entries are already contiguous on
    // the input wire.
    //
    // The legacy MTConnect JSON v1 object-keyed shape
    // ({"Fault": [...], "Warning": [...], ...}) is still accepted on the
    // read path for back-compat.
    internal sealed class JsonConditionsConverter : JsonConverter<JsonConditions>
    {
        private const string FaultLevel = "Fault";
        private const string WarningLevel = "Warning";
        private const string NormalLevel = "Normal";
        private const string UnavailableLevel = "Unavailable";

        public override JsonConditions Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;

                case JsonTokenType.StartArray:
                    return ReadArrayShape(ref reader, options);

                case JsonTokenType.StartObject:
                    return ReadObjectShape(ref reader, options);

                default:
                    throw new JsonException(
                        $"Unexpected token '{reader.TokenType}' when reading JsonConditions; expected array, object, or null.");
            }
        }

        public override void Write(Utf8JsonWriter writer, JsonConditions value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartArray();

            WriteLevel(writer, FaultLevel, value.Fault, options);
            WriteLevel(writer, WarningLevel, value.Warning, options);
            WriteLevel(writer, NormalLevel, value.Normal, options);
            WriteLevel(writer, UnavailableLevel, value.Unavailable, options);

            writer.WriteEndArray();
        }

        private static void WriteLevel(Utf8JsonWriter writer, string levelName, IEnumerable<JsonCondition> entries, JsonSerializerOptions options)
        {
            if (entries == null) return;

            foreach (var entry in entries)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(levelName);
                JsonSerializer.Serialize(writer, entry, options);
                writer.WriteEndObject();
            }
        }

        private static JsonConditions ReadArrayShape(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var faults = new List<JsonCondition>();
            var warnings = new List<JsonCondition>();
            var normals = new List<JsonCondition>();
            var unavailables = new List<JsonCondition>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray) break;

                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException(
                        $"Unexpected token '{reader.TokenType}' inside JsonConditions array; expected object wrapper.");
                }

                if (!reader.Read() || reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("Expected property name inside JsonConditions wrapper object.");
                }

                var levelName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException("Expected value after Condition level name in JsonConditions wrapper object.");
                }
                var entry = JsonSerializer.Deserialize<JsonCondition>(ref reader, options);
                if (entry == null)
                {
                    throw new JsonException("Null Condition entry value in JsonConditions wrapper.");
                }

                if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
                {
                    throw new JsonException("Expected end of JsonConditions wrapper object after entry.");
                }

                switch (levelName)
                {
                    case FaultLevel:
                        faults.Add(entry);
                        break;
                    case WarningLevel:
                        warnings.Add(entry);
                        break;
                    case NormalLevel:
                        normals.Add(entry);
                        break;
                    case UnavailableLevel:
                        unavailables.Add(entry);
                        break;
                    default:
                        throw new JsonException(
                            $"Unknown Condition level '{levelName}' in JsonConditions array; expected Fault, Warning, Normal, or Unavailable.");
                }
            }

            return new JsonConditions
            {
                Fault = faults.Count > 0 ? faults : null,
                Warning = warnings.Count > 0 ? warnings : null,
                Normal = normals.Count > 0 ? normals : null,
                Unavailable = unavailables.Count > 0 ? unavailables : null,
            };
        }

        // Reads the legacy MTConnect JSON v1 object-keyed shape:
        // {"Fault": [...], "Warning": [...], ...}. Duplicate level keys
        // on the input (e.g. {"Fault":[a],"Fault":[b]}) are by-design
        // last-write-wins: each occurrence overwrites the previous
        // entry list. This is accepted asymmetry with the array path,
        // which appends across all occurrences. Legacy producers that
        // emit each level key exactly once are unaffected; the asymmetry
        // only surfaces on malformed-or-duplicate legacy input, where
        // last-write-wins is a deterministic, documented behavior.
        private static JsonConditions ReadObjectShape(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var result = new JsonConditions();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) break;

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException(
                        $"Unexpected token '{reader.TokenType}' inside JsonConditions object; expected property name.");
                }

                var levelName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException("Expected value after Condition level name in JsonConditions wrapper object.");
                }
                var entries = JsonSerializer.Deserialize<List<JsonCondition>>(ref reader, options);

                switch (levelName)
                {
                    case FaultLevel:
                        result.Fault = entries;
                        break;
                    case WarningLevel:
                        result.Warning = entries;
                        break;
                    case NormalLevel:
                        result.Normal = entries;
                        break;
                    case UnavailableLevel:
                        result.Unavailable = entries;
                        break;
                    default:
                        throw new JsonException(
                            $"Unknown Condition level '{levelName}' in JsonConditions object; expected Fault, Warning, Normal, or Unavailable.");
                }
            }

            return result;
        }
    }
}
