// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Streams.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text.Json;

namespace MTConnect.NET_JSON_cppagent_Tests.Streams
{
    // Pins the cppagent JSON v2 array-of-wrappers wire shape for
    // ConditionListType. The XSD declares ConditionListType as
    // <xs:sequence><xs:choice maxOccurs='unbounded'>
    // of Normal|Warning|Fault|Unavailable; cppagent v2 emits one
    // single-key wrapper object per entry.
    //
    // Ordering: the typed JsonConditions POCO buckets entries by level
    // (Fault, Warning, Normal, Unavailable). The converter emits in
    // that fixed level order, with source order preserved within each
    // bucket. Mixed-level interleaving on the wire is therefore NOT
    // round-trip preserved through the typed model — see the
    // Read_ArrayShape_MixedLevelInterleaving_BucketsByLevel test for
    // the explicit pin.
    //
    // Sources:
    // - XSD: https://schemas.mtconnect.org/schemas/MTConnectStreams_2.7.xsd
    //   (complex type ConditionListType).
    // - Prose: MTConnect Standard Part 2 section 13 "Condition".
    // - cppagent reference (v2.7.0.7): printer/json_printer.cpp
    //   function print_condition.
    [TestFixture]
    public class JsonConditionsArrayShapeTests
    {
        private static JsonCondition MakeEntry(string dataItemId, string type)
        {
            return new JsonCondition
            {
                DataItemId = dataItemId,
                Type = type,
            };
        }

        private static JsonSerializerOptions Options() => new JsonSerializerOptions();

        private static string FirstPropertyName(JsonElement element)
        {
            using var enumerator = element.EnumerateObject();
            enumerator.MoveNext();
            return enumerator.Current.Name;
        }

        // Case 1 — empty conditions serialize as the array shape, not an object shape.
        [Test]
        public void Write_EmptyConditions_EmitsEmptyArray()
        {
            var conditions = new JsonConditions();

            var json = JsonSerializer.Serialize(conditions, Options());

            Assert.That(json, Is.EqualTo("[]"));
        }

        // Case 2 — one Normal entry produces a 1-element array with a Normal wrapper.
        [Test]
        public void Write_SingleNormal_EmitsOneNormalWrapper()
        {
            var conditions = new JsonConditions
            {
                Normal = new List<JsonCondition> { MakeEntry("n1", "TEMPERATURE") },
            };

            var json = JsonSerializer.Serialize(conditions, Options());

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.That(root.ValueKind, Is.EqualTo(JsonValueKind.Array));
            Assert.That(root.GetArrayLength(), Is.EqualTo(1));

            var wrapper = root[0];
            Assert.That(wrapper.ValueKind, Is.EqualTo(JsonValueKind.Object));
            Assert.That(wrapper.TryGetProperty("Normal", out var entry), Is.True);
            Assert.That(entry.GetProperty("dataItemId").GetString(), Is.EqualTo("n1"));
        }

        // Case 3 — Fault + Warning emit in Fault, Warning order per the converter.
        [Test]
        public void Write_FaultThenWarning_EmitsInDeclaredEnumerationOrder()
        {
            var conditions = new JsonConditions
            {
                Fault = new List<JsonCondition> { MakeEntry("f1", "TEMPERATURE") },
                Warning = new List<JsonCondition> { MakeEntry("w1", "POSITION") },
            };

            var json = JsonSerializer.Serialize(conditions, Options());

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.That(root.GetArrayLength(), Is.EqualTo(2));

            Assert.That(FirstPropertyName(root[0]), Is.EqualTo("Fault"));
            Assert.That(FirstPropertyName(root[1]), Is.EqualTo("Warning"));
        }

        // Case 4 — all four levels populated emit in Fault, Warning, Normal, Unavailable order.
        [Test]
        public void Write_AllFourLevels_EmitsInFaultWarningNormalUnavailableOrder()
        {
            var conditions = new JsonConditions
            {
                Fault = new List<JsonCondition> { MakeEntry("f1", "TEMPERATURE") },
                Warning = new List<JsonCondition> { MakeEntry("w1", "POSITION") },
                Normal = new List<JsonCondition> { MakeEntry("n1", "AVAILABILITY") },
                Unavailable = new List<JsonCondition> { MakeEntry("u1", "ROTATION") },
            };

            var json = JsonSerializer.Serialize(conditions, Options());

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.That(root.GetArrayLength(), Is.EqualTo(4));

            var keys = new List<string>();
            for (var i = 0; i < root.GetArrayLength(); i++)
            {
                foreach (var prop in root[i].EnumerateObject())
                {
                    keys.Add(prop.Name);
                }
            }

            Assert.That(keys, Is.EqualTo(new[] { "Fault", "Warning", "Normal", "Unavailable" }));
        }

        // Case 5 — multiple entries on one level produce one wrapper each in source order.
        [Test]
        public void Write_MultipleFaults_EmitsOneWrapperPerEntry()
        {
            var conditions = new JsonConditions
            {
                Fault = new List<JsonCondition>
                {
                    MakeEntry("f1", "TEMPERATURE"),
                    MakeEntry("f2", "POSITION"),
                    MakeEntry("f3", "AVAILABILITY"),
                },
            };

            var json = JsonSerializer.Serialize(conditions, Options());

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.That(root.GetArrayLength(), Is.EqualTo(3));

            var ids = new List<string>();
            foreach (var element in root.EnumerateArray())
            {
                Assert.That(element.TryGetProperty("Fault", out var entry), Is.True);
                ids.Add(entry.GetProperty("dataItemId").GetString()!);
            }

            Assert.That(ids, Is.EqualTo(new[] { "f1", "f2", "f3" }));
        }

        // Case 5b — mixed-level interleaving on the input wire is bucketed
        // by level on read and re-emitted in level order (Fault, Warning,
        // Normal, Unavailable) on write. Pins the documented non-byte-
        // identical round-trip for interleaved input — see the type
        // comment on JsonConditionsConverter for the design rationale.
        [Test]
        public void Read_ArrayShape_MixedLevelInterleaving_BucketsByLevel()
        {
            const string interleaved =
                "[{\"Fault\":{\"dataItemId\":\"f1\"}}," +
                "{\"Normal\":{\"dataItemId\":\"n1\"}}," +
                "{\"Fault\":{\"dataItemId\":\"f2\"}}]";

            var parsed = JsonSerializer.Deserialize<JsonConditions>(interleaved, Options());

            Assert.That(parsed, Is.Not.Null);
            Assert.That(parsed!.Fault, Is.Not.Null);
            Assert.That(parsed.Normal, Is.Not.Null);
            Assert.That(parsed.Warning, Is.Null);
            Assert.That(parsed.Unavailable, Is.Null);

            var faultIds = new List<string>();
            foreach (var entry in parsed.Fault!) faultIds.Add(entry.DataItemId);
            Assert.That(faultIds, Is.EqualTo(new[] { "f1", "f2" }));

            var normalIds = new List<string>();
            foreach (var entry in parsed.Normal!) normalIds.Add(entry.DataItemId);
            Assert.That(normalIds, Is.EqualTo(new[] { "n1" }));

            var rewritten = JsonSerializer.Serialize(parsed, Options());
            using var rewrittenDoc = JsonDocument.Parse(rewritten);
            var rewrittenRoot = rewrittenDoc.RootElement;
            Assert.That(rewrittenRoot.ValueKind, Is.EqualTo(JsonValueKind.Array));
            Assert.That(rewrittenRoot.GetArrayLength(), Is.EqualTo(3));

            var rewrittenKeys = new List<string>();
            var rewrittenDataItemIds = new List<string>();
            for (var i = 0; i < rewrittenRoot.GetArrayLength(); i++)
            {
                foreach (var prop in rewrittenRoot[i].EnumerateObject())
                {
                    rewrittenKeys.Add(prop.Name);
                    rewrittenDataItemIds.Add(prop.Value.GetProperty("dataItemId").GetString()!);
                }
            }

            Assert.That(rewrittenKeys, Is.EqualTo(new[] { "Fault", "Fault", "Normal" }));
            Assert.That(rewrittenDataItemIds, Is.EqualTo(new[] { "f1", "f2", "n1" }));
        }

        // Case 6 — array JSON round-trips through Deserialize/Serialize without drift.
        [Test]
        public void RoundTrip_ArrayShape_IsByteIdenticalModuloWhitespace()
        {
            var original = new JsonConditions
            {
                Fault = new List<JsonCondition> { MakeEntry("f1", "TEMPERATURE") },
                Warning = new List<JsonCondition> { MakeEntry("w1", "POSITION") },
                Normal = new List<JsonCondition> { MakeEntry("n1", "AVAILABILITY") },
                Unavailable = new List<JsonCondition> { MakeEntry("u1", "ROTATION") },
            };

            var json = JsonSerializer.Serialize(original, Options());
            var parsed = JsonSerializer.Deserialize<JsonConditions>(json, Options());
            var json2 = JsonSerializer.Serialize(parsed, Options());

            Assert.That(json2, Is.EqualTo(json));
        }

        // Case 7 — legacy MTConnect JSON v1 object-keyed shape parses into the typed POCO.
        [Test]
        public void Read_LegacyObjectShape_PopulatesTypedProperties()
        {
            const string legacy =
                "{\"Normal\":[{\"dataItemId\":\"n1\",\"type\":\"TEMPERATURE\"}]," +
                "\"Fault\":[{\"dataItemId\":\"f1\",\"type\":\"POSITION\"}]}";

            var parsed = JsonSerializer.Deserialize<JsonConditions>(legacy, Options());

            Assert.That(parsed, Is.Not.Null);
            Assert.That(parsed!.Normal, Is.Not.Null);
            Assert.That(parsed.Fault, Is.Not.Null);

            using var normalEnumerator = parsed.Normal!.GetEnumerator();
            Assert.That(normalEnumerator.MoveNext(), Is.True);
            Assert.That(normalEnumerator.Current.DataItemId, Is.EqualTo("n1"));

            using var faultEnumerator = parsed.Fault!.GetEnumerator();
            Assert.That(faultEnumerator.MoveNext(), Is.True);
            Assert.That(faultEnumerator.Current.DataItemId, Is.EqualTo("f1"));
        }

        // Case 8 — null write emits "null" and round-trips back to a null reference.
        [Test]
        public void Null_WriteAndRead_RoundTripsToNull()
        {
            var json = JsonSerializer.Serialize<JsonConditions>(null!, Options());
            Assert.That(json, Is.EqualTo("null"));

            var parsed = JsonSerializer.Deserialize<JsonConditions>("null", Options());
            Assert.That(parsed, Is.Null);
        }

        // Case 9 — invalid root token (number) raises JsonException with a recognisable message.
        [Test]
        public void Read_InvalidRootToken_ThrowsJsonException()
        {
            var ex = Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<JsonConditions>("123", Options()));
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Does.Contain("Unexpected token"));
        }

        // Coverage filler — the array-shape read path also handles all four levels
        // and rejects unknown level names + malformed wrapper objects.
        [Test]
        public void Read_ArrayShape_PopulatesAllFourLevels()
        {
            const string json =
                "[{\"Fault\":{\"dataItemId\":\"f1\"}}," +
                "{\"Warning\":{\"dataItemId\":\"w1\"}}," +
                "{\"Normal\":{\"dataItemId\":\"n1\"}}," +
                "{\"Unavailable\":{\"dataItemId\":\"u1\"}}]";

            var parsed = JsonSerializer.Deserialize<JsonConditions>(json, Options());

            Assert.That(parsed, Is.Not.Null);
            Assert.That(parsed!.Fault, Is.Not.Null);
            Assert.That(parsed.Warning, Is.Not.Null);
            Assert.That(parsed.Normal, Is.Not.Null);
            Assert.That(parsed.Unavailable, Is.Not.Null);
        }

        [Test]
        public void Read_ArrayShape_UnknownLevel_ThrowsJsonException()
        {
            const string json = "[{\"Bogus\":{\"dataItemId\":\"x1\"}}]";

            var ex = Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<JsonConditions>(json, Options()));
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Does.Contain("Unknown Condition level"));
        }

        [Test]
        public void Read_ArrayShape_NonObjectElement_ThrowsJsonException()
        {
            const string json = "[42]";

            var ex = Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<JsonConditions>(json, Options()));
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Does.Contain("expected object wrapper"));
        }

        [Test]
        public void Read_ArrayShape_WrapperWithoutPropertyName_ThrowsJsonException()
        {
            const string json = "[{}]";

            var ex = Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<JsonConditions>(json, Options()));
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Does.Contain("Expected property name"));
        }

        [Test]
        public void Read_ArrayShape_WrapperWithMultipleProperties_ThrowsJsonException()
        {
            const string json = "[{\"Fault\":{\"dataItemId\":\"f1\"},\"Warning\":{\"dataItemId\":\"w1\"}}]";

            var ex = Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<JsonConditions>(json, Options()));
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Does.Contain("end of JsonConditions wrapper"));
        }

        [Test]
        public void Read_ArrayShape_NullEntry_ThrowsJsonException()
        {
            const string json = "[{\"Normal\":null}]";

            var ex = Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<JsonConditions>(json, Options()));
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Does.Contain("Null Condition entry"));
        }

        [Test]
        public void Read_ObjectShape_UnknownLevel_ThrowsJsonException()
        {
            const string json = "{\"Bogus\":[{\"dataItemId\":\"x1\"}]}";

            var ex = Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<JsonConditions>(json, Options()));
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Does.Contain("Unknown Condition level"));
        }

        [Test]
        public void Read_ObjectShape_PopulatesAllFourLevels()
        {
            const string json =
                "{\"Fault\":[{\"dataItemId\":\"f1\"}]," +
                "\"Warning\":[{\"dataItemId\":\"w1\"}]," +
                "\"Normal\":[{\"dataItemId\":\"n1\"}]," +
                "\"Unavailable\":[{\"dataItemId\":\"u1\"}]}";

            var parsed = JsonSerializer.Deserialize<JsonConditions>(json, Options());

            Assert.That(parsed, Is.Not.Null);
            Assert.That(parsed!.Fault, Is.Not.Null);
            Assert.That(parsed.Warning, Is.Not.Null);
            Assert.That(parsed.Normal, Is.Not.Null);
            Assert.That(parsed.Unavailable, Is.Not.Null);
        }
    }
}
