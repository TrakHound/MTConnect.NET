// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using MTConnect.Streams.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTConnect.NET_JSON_cppagent_Tests.Streams
{
    // Pins the cppagent JSON v2 array-of-wrappers wire shape for
    // ConditionListType. The XSD declares ConditionListType as
    // <xs:sequence><xs:choice maxOccurs='unbounded'>
    // of Normal|Warning|Fault|Unavailable; cppagent v2 emits one
    // single-key wrapper object per entry.
    //
    // Since 7.0 the type shape IS the wire shape: JsonConditions
    // inherits List<JsonConditionWrapper> and the default S.T.J
    // serializer handles both directions with no custom converter.
    // Ordering on the wire follows list insertion order; the
    // observation-taking ctor preserves the historical
    // Fault -> Warning -> Normal -> Unavailable emission order.
    //
    // Sources:
    // - XSD: https://schemas.mtconnect.org/schemas/MTConnectStreams_2.7.xsd
    //   (complex type ConditionListType).
    // - Prose: MTConnect Standard Part 2 section 13 "Condition".
    // - cppagent reference (v2.7.0.7): printer/json_printer.cpp
    //   function print_condition.
    /// <summary>
    /// Unit + wire coverage for <see cref="JsonConditions"/> — the
    /// list-of-wrappers container that emits cppagent JSON v2 shape by
    /// default S.T.J behavior on the derived <see cref="List{T}"/>.
    /// </summary>
    [TestFixture]
    public class JsonConditionsArrayShapeTests
    {
        private static JsonCondition MakeEntry(string dataItemId, string? type = null) =>
            new JsonCondition { DataItemId = dataItemId, Type = type! };

        /// <summary>
        /// Options that suppress null-valued properties on the wire, matching
        /// cppagent JSON v2 sparse-object shape (dataItemId + populated
        /// fields only). Required for byte-identical wire assertions because
        /// <see cref="JsonCondition"/>'s inner properties do not carry
        /// per-property <c>[JsonIgnore(WhenWritingNull)]</c>; the
        /// null-suppression contract lives on the emission pipeline's
        /// options object.
        /// </summary>
        private static JsonSerializerOptions SparseOptions() =>
            new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

        // ------------------------------------------------------------------
        // Ctor coverage — three overloads.
        // ------------------------------------------------------------------

        /// <summary>Pins the parameterless ctor: creates an empty list.</summary>
        [Test]
        public void Ctor_Default_creates_empty_list()
        {
            var conditions = new JsonConditions();
            Assert.That(conditions.Count, Is.EqualTo(0));
        }

        /// <summary>Pins the copy ctor: seeds the list from an existing sequence, order preserved.</summary>
        [Test]
        public void Ctor_FromWrapperSequence_seeds_in_insertion_order()
        {
            var wrappers = new[]
            {
                JsonConditionWrapper.OfNormal (MakeEntry("n1")),
                JsonConditionWrapper.OfFault  (MakeEntry("f1")),
                JsonConditionWrapper.OfWarning(MakeEntry("w1")),
            };

            var conditions = new JsonConditions(wrappers);

            Assert.That(conditions.Count, Is.EqualTo(3));
            Assert.That(conditions[0].Level, Is.EqualTo("Normal"));
            Assert.That(conditions[1].Level, Is.EqualTo("Fault"));
            Assert.That(conditions[2].Level, Is.EqualTo("Warning"));
        }

        /// <summary>Pins that a null wrapper-sequence ctor argument yields an empty list, not a NullReferenceException.</summary>
        [Test]
        public void Ctor_FromWrapperSequence_null_is_treated_as_empty()
        {
            var conditions = new JsonConditions((IEnumerable<JsonConditionWrapper>)null!);
            Assert.That(conditions.Count, Is.EqualTo(0));
        }

        /// <summary>Pins that a null observation-sequence ctor argument yields an empty list, not a NullReferenceException.</summary>
        [Test]
        public void Ctor_FromObservationSequence_null_is_treated_as_empty()
        {
            var conditions = new JsonConditions((IEnumerable<IObservationOutput>)null!);
            Assert.That(conditions.Count, Is.EqualTo(0));
        }

        /// <summary>Pins that the observation-sequence ctor with no matching entries yields an empty list.</summary>
        [Test]
        public void Ctor_FromObservationSequence_empty_produces_empty_list()
        {
            var conditions = new JsonConditions(Array.Empty<IObservationOutput>());
            Assert.That(conditions.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Pins that the observation-sequence ctor emits in the historical
        /// level-order (Fault, Warning, Normal, Unavailable), source order
        /// within each bucket, so the wire is byte-identical to pre-refactor
        /// converter output for the same input.
        /// </summary>
        [Test]
        public void Ctor_FromObservationSequence_emits_in_level_order()
        {
            var observations = new IObservationOutput[]
            {
                // Deliberately mix input order so the ctor's level-bucketing
                // is what determines emission order, not source order.
                FakeConditionOutput.Create("n1", ConditionLevel.NORMAL),
                FakeConditionOutput.Create("u1", ConditionLevel.UNAVAILABLE),
                FakeConditionOutput.Create("f1", ConditionLevel.FAULT),
                FakeConditionOutput.Create("w1", ConditionLevel.WARNING),
                FakeConditionOutput.Create("f2", ConditionLevel.FAULT),
            };

            var conditions = new JsonConditions(observations);

            Assert.That(conditions.Count, Is.EqualTo(5));
            Assert.That(conditions[0].Level, Is.EqualTo("Fault"));
            Assert.That(conditions[0].Fault!.DataItemId, Is.EqualTo("f1"));
            Assert.That(conditions[1].Level, Is.EqualTo("Fault"));
            Assert.That(conditions[1].Fault!.DataItemId, Is.EqualTo("f2"));
            Assert.That(conditions[2].Level, Is.EqualTo("Warning"));
            Assert.That(conditions[3].Level, Is.EqualTo("Normal"));
            Assert.That(conditions[4].Level, Is.EqualTo("Unavailable"));
        }

        /// <summary>Pins that null entries in the observation sequence are skipped, not rethrown.</summary>
        [Test]
        public void Ctor_FromObservationSequence_skips_null_entries()
        {
            var observations = new IObservationOutput[]
            {
                null!,
                FakeConditionOutput.Create("f1", ConditionLevel.FAULT),
                null!,
            };

            var conditions = new JsonConditions(observations);

            Assert.That(conditions.Count, Is.EqualTo(1));
            Assert.That(conditions[0].Level, Is.EqualTo("Fault"));
        }

        // ------------------------------------------------------------------
        // Observations computed accessor.
        // ------------------------------------------------------------------

        /// <summary>Pins Observations returns an empty list on an empty container.</summary>
        [Test]
        public void Observations_is_empty_on_empty_container()
        {
            Assert.That(new JsonConditions().Observations, Is.Empty);
        }

        /// <summary>
        /// Pins Observations materializes every non-empty wrapper into the
        /// matching strongly-typed condition at the right level, in list
        /// insertion order.
        /// </summary>
        [Test]
        public void Observations_materializes_in_insertion_order()
        {
            var conditions = new JsonConditions
            {
                JsonConditionWrapper.OfFault      (MakeEntry("f1", "TEMPERATURE")),
                JsonConditionWrapper.OfWarning    (MakeEntry("w1", "POSITION")),
                JsonConditionWrapper.OfNormal     (MakeEntry("n1", "AVAILABILITY")),
                JsonConditionWrapper.OfUnavailable(MakeEntry("u1", "ROTATION")),
            };

            var observations = conditions.Observations;

            Assert.That(observations.Count, Is.EqualTo(4));
            Assert.That((observations[0] as IConditionObservation)!.Level, Is.EqualTo(ConditionLevel.FAULT));
            Assert.That((observations[1] as IConditionObservation)!.Level, Is.EqualTo(ConditionLevel.WARNING));
            Assert.That((observations[2] as IConditionObservation)!.Level, Is.EqualTo(ConditionLevel.NORMAL));
            Assert.That((observations[3] as IConditionObservation)!.Level, Is.EqualTo(ConditionLevel.UNAVAILABLE));
            Assert.That(observations[0].DataItemId, Is.EqualTo("f1"));
            Assert.That(observations[1].DataItemId, Is.EqualTo("w1"));
            Assert.That(observations[2].DataItemId, Is.EqualTo("n1"));
            Assert.That(observations[3].DataItemId, Is.EqualTo("u1"));
        }

        /// <summary>Pins that empty wrappers (all four properties null) are skipped by Observations.</summary>
        [Test]
        public void Observations_skips_empty_wrappers()
        {
            var conditions = new JsonConditions
            {
                new JsonConditionWrapper(),                          // empty
                JsonConditionWrapper.OfFault(MakeEntry("f1")),
                null!,                                                // null wrapper
                new JsonConditionWrapper(),                          // empty
                JsonConditionWrapper.OfNormal(MakeEntry("n1")),
            };

            var observations = conditions.Observations;

            Assert.That(observations.Count, Is.EqualTo(2));
            Assert.That(observations[0].DataItemId, Is.EqualTo("f1"));
            Assert.That(observations[1].DataItemId, Is.EqualTo("n1"));
        }

        // ------------------------------------------------------------------
        // Wire-shape pins — serialization.
        // ------------------------------------------------------------------

        /// <summary>Pins that an empty conditions container serializes to <c>[]</c>.</summary>
        [Test]
        public void Serialize_empty_conditions_emits_empty_array()
        {
            Assert.That(JsonSerializer.Serialize(new JsonConditions()), Is.EqualTo("[]"));
        }

        /// <summary>Pins that a single Normal-wrapped condition serializes to a one-element array.</summary>
        [Test]
        public void Serialize_single_normal_emits_one_normal_wrapper()
        {
            var conditions = new JsonConditions
            {
                JsonConditionWrapper.OfNormal(MakeEntry("n1", "TEMPERATURE")),
            };

            var json = JsonSerializer.Serialize(conditions);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.That(root.ValueKind, Is.EqualTo(JsonValueKind.Array));
            Assert.That(root.GetArrayLength(), Is.EqualTo(1));

            var wrapper = root[0];
            Assert.That(wrapper.ValueKind, Is.EqualTo(JsonValueKind.Object));
            Assert.That(wrapper.TryGetProperty("Normal", out var entry), Is.True);
            Assert.That(entry.GetProperty("dataItemId").GetString(), Is.EqualTo("n1"));
        }

        /// <summary>
        /// Pins that programmatic list construction preserves insertion
        /// order on the wire — the wire array follows list insertion
        /// exactly, no re-bucketing. Structural pin (root array + per-index
        /// level key + dataItemId) rather than a byte-identical assertion
        /// because <see cref="JsonCondition"/> emits several default-valued
        /// fields (timestamp, sequence, instanceId) that are irrelevant to
        /// the ordering guarantee under test.
        /// </summary>
        [Test]
        public void Serialize_preserves_insertion_order_across_levels()
        {
            var conditions = new JsonConditions
            {
                JsonConditionWrapper.OfNormal(MakeEntry("n1")),
                JsonConditionWrapper.OfFault (MakeEntry("f1")),
                JsonConditionWrapper.OfNormal(MakeEntry("n2")),
                JsonConditionWrapper.OfFault (MakeEntry("f2")),
            };

            var json = JsonSerializer.Serialize(conditions, SparseOptions());

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.That(root.ValueKind, Is.EqualTo(JsonValueKind.Array));
            Assert.That(root.GetArrayLength(), Is.EqualTo(4));

            var expected = new[]
            {
                (Level: "Normal", DataItemId: "n1"),
                (Level: "Fault",  DataItemId: "f1"),
                (Level: "Normal", DataItemId: "n2"),
                (Level: "Fault",  DataItemId: "f2"),
            };

            for (var i = 0; i < expected.Length; i++)
            {
                var wrapper = root[i];
                Assert.That(wrapper.TryGetProperty(expected[i].Level, out var entry), Is.True,
                    $"index {i} should carry a {expected[i].Level} envelope");
                Assert.That(entry.GetProperty("dataItemId").GetString(), Is.EqualTo(expected[i].DataItemId),
                    $"index {i} dataItemId should be {expected[i].DataItemId}");
            }
        }

        /// <summary>
        /// Pins that the observation-taking ctor's level-order emission
        /// yields the historical Fault, Warning, Normal, Unavailable
        /// sequence on the wire for a mixed-input observation sequence.
        /// </summary>
        [Test]
        public void Serialize_from_observation_ctor_emits_in_fault_warning_normal_unavailable_order()
        {
            var observations = new IObservationOutput[]
            {
                FakeConditionOutput.Create("n1", ConditionLevel.NORMAL),
                FakeConditionOutput.Create("u1", ConditionLevel.UNAVAILABLE),
                FakeConditionOutput.Create("f1", ConditionLevel.FAULT),
                FakeConditionOutput.Create("w1", ConditionLevel.WARNING),
            };

            var json = JsonSerializer.Serialize(new JsonConditions(observations));

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var keys = new List<string>();
            foreach (var element in root.EnumerateArray())
            {
                foreach (var prop in element.EnumerateObject())
                {
                    keys.Add(prop.Name);
                }
            }

            Assert.That(keys, Is.EqualTo(new[] { "Fault", "Warning", "Normal", "Unavailable" }));
        }

        // ------------------------------------------------------------------
        // Wire-shape pins — deserialization.
        // ------------------------------------------------------------------

        /// <summary>Pins that <c>[]</c> deserializes to an empty conditions list.</summary>
        [Test]
        public void Deserialize_empty_array_yields_empty_container()
        {
            var conditions = JsonSerializer.Deserialize<JsonConditions>("[]");
            Assert.That(conditions, Is.Not.Null);
            Assert.That(conditions!.Count, Is.EqualTo(0));
        }

        /// <summary>Pins that a mixed-level wire array deserializes with each wrapper's matching level property populated and the other three null.</summary>
        [Test]
        public void Deserialize_mixed_level_array_populates_each_wrapper_correctly()
        {
            const string wire =
                "[" +
                "{\"Fault\":{\"dataItemId\":\"f1\"}}," +
                "{\"Normal\":{\"dataItemId\":\"n1\",\"sequence\":42}}," +
                "{\"Warning\":{\"dataItemId\":\"w1\",\"type\":\"POSITION\"}}," +
                "{\"Unavailable\":{\"dataItemId\":\"u1\"}}" +
                "]";

            var conditions = JsonSerializer.Deserialize<JsonConditions>(wire);

            Assert.That(conditions!.Count, Is.EqualTo(4));

            Assert.That(conditions[0].Fault, Is.Not.Null); Assert.That(conditions[0].Fault!.DataItemId, Is.EqualTo("f1"));
            Assert.That(conditions[1].Normal, Is.Not.Null); Assert.That(conditions[1].Normal!.DataItemId, Is.EqualTo("n1"));
            Assert.That((ulong)conditions[1].Normal!.Sequence, Is.EqualTo(42UL));
            Assert.That(conditions[2].Warning, Is.Not.Null); Assert.That(conditions[2].Warning!.Type, Is.EqualTo("POSITION"));
            Assert.That(conditions[3].Unavailable, Is.Not.Null); Assert.That(conditions[3].Unavailable!.DataItemId, Is.EqualTo("u1"));

            // Cross-checks — non-matching properties are null.
            Assert.That(conditions[0].Warning, Is.Null); Assert.That(conditions[0].Normal, Is.Null); Assert.That(conditions[0].Unavailable, Is.Null);
        }

        /// <summary>
        /// Pins byte-identical round-trip through serialize -> deserialize ->
        /// serialize on the four-level mixed input, guaranteeing the type
        /// shape is symmetric on the wire.
        /// </summary>
        [Test]
        public void RoundTrip_array_shape_is_byte_identical()
        {
            var original = new JsonConditions
            {
                JsonConditionWrapper.OfFault      (MakeEntry("f1", "TEMPERATURE")),
                JsonConditionWrapper.OfWarning    (MakeEntry("w1", "POSITION")),
                JsonConditionWrapper.OfNormal     (MakeEntry("n1", "AVAILABILITY")),
                JsonConditionWrapper.OfUnavailable(MakeEntry("u1", "ROTATION")),
            };

            var json1 = JsonSerializer.Serialize(original);
            var parsed = JsonSerializer.Deserialize<JsonConditions>(json1);
            var json2 = JsonSerializer.Serialize(parsed);

            Assert.That(json2, Is.EqualTo(json1));
        }

        /// <summary>Pins that root-level null writes as <c>"null"</c> and reads back as a null reference.</summary>
        [Test]
        public void Null_root_writes_and_reads_as_null()
        {
            Assert.That(JsonSerializer.Serialize<JsonConditions>(null!), Is.EqualTo("null"));
            Assert.That(JsonSerializer.Deserialize<JsonConditions>("null"), Is.Null);
        }

        // ------------------------------------------------------------------
        // Regression pins for the intentional 7.0 behavioral drops.
        // These fail loudly if a future refactor accidentally reintroduces
        // legacy compat behavior that the type shape rewrite was meant to
        // simplify away.
        // ------------------------------------------------------------------

        /// <summary>
        /// Regression pin for the intentional drop of legacy MTConnect JSON
        /// v1 object-keyed READ compat. Pre-7.0 the custom converter
        /// accepted <c>{"Fault":[...], "Warning":[...]}</c> on the read
        /// path; after the structural rewrite the default
        /// <see cref="List{T}"/> deserializer only reads arrays. A future
        /// change that reintroduces v1 compat via a new
        /// <see cref="JsonConverter"/> would silently flip this back and
        /// break wire-shape invariants; this test fails first.
        /// </summary>
        [Test]
        public void Deserialize_legacy_v1_object_keyed_shape_throws_JsonException()
        {
            const string legacyV1 =
                "{\"Fault\":[{\"dataItemId\":\"f1\"}]," +
                "\"Warning\":[{\"dataItemId\":\"w1\"}]," +
                "\"Normal\":[{\"dataItemId\":\"n1\"}]," +
                "\"Unavailable\":[{\"dataItemId\":\"u1\"}]}";

            Assert.Throws<JsonException>((Action)(() =>
                JsonSerializer.Deserialize<JsonConditions>(legacyV1)));
        }

        /// <summary>
        /// Regression pin for the intentional drop of strict unknown-level
        /// wrapper rejection. Pre-7.0 the custom converter threw a
        /// <c>JsonException</c> naming the unknown level on any array
        /// entry with a property name outside the set
        /// {Fault, Warning, Normal, Unavailable}. After the structural
        /// rewrite the default deserializer silently ignores unknown
        /// property names on <see cref="JsonConditionWrapper"/>: the
        /// wrapper is materialized with every level property null, so
        /// <see cref="JsonConditionWrapper.Value"/>,
        /// <see cref="JsonConditionWrapper.Level"/>, and
        /// <see cref="JsonConditionWrapper.ToObservation"/> all return
        /// null. A future change that reintroduced strict rejection (for
        /// example via a custom converter or an
        /// <c>UnmappedMemberHandling.Disallow</c> serializer option)
        /// would flip this test to expect the exception; that is a
        /// wire-shape contract change and should be caught here.
        /// </summary>
        [Test]
        public void Deserialize_unknown_level_property_is_silently_ignored()
        {
            const string wire =
                "[{\"Bogus\":{\"dataItemId\":\"x1\"}}," +
                 "{\"Fault\":{\"dataItemId\":\"f1\"}}]";

            var conditions = JsonSerializer.Deserialize<JsonConditions>(wire);

            Assert.That(conditions, Is.Not.Null);
            Assert.That(conditions!.Count, Is.EqualTo(2));

            // First wrapper — unknown "Bogus" is silently dropped, all four
            // known-level properties remain null.
            var bogus = conditions[0];
            Assert.That(bogus.Fault, Is.Null);
            Assert.That(bogus.Warning, Is.Null);
            Assert.That(bogus.Normal, Is.Null);
            Assert.That(bogus.Unavailable, Is.Null);
            Assert.That(bogus.Value, Is.Null);
            Assert.That(bogus.Level, Is.Null);
            Assert.That(bogus.ToObservation(), Is.Null);

            // Second wrapper — known "Fault" still populates correctly.
            Assert.That(conditions[1].Fault, Is.Not.Null);
            Assert.That(conditions[1].Fault!.DataItemId, Is.EqualTo("f1"));

            // Observations accessor skips the empty first wrapper.
            Assert.That(conditions.Observations.Count, Is.EqualTo(1));
            Assert.That(conditions.Observations[0].DataItemId, Is.EqualTo("f1"));
        }

        /// <summary>
        /// Failure-path pin for a non-array root token (number, string,
        /// bool). The default <see cref="List{T}"/> deserializer accepts
        /// only <c>[</c> and <c>null</c>; any other root token yields a
        /// <see cref="JsonException"/>. This mirrors the removed
        /// converter's <c>"Unexpected token '...' when reading
        /// JsonConditions; expected array, object, or null."</c> branch
        /// for the non-object case (v1 object-shape acceptance is
        /// separately regression-pinned as a drop).
        /// </summary>
        [TestCase("123")]
        [TestCase("\"fault\"")]
        [TestCase("true")]
        [TestCase("false")]
        public void Deserialize_non_array_root_throws_JsonException(string wire)
        {
            Assert.Throws<JsonException>((Action)(() => JsonSerializer.Deserialize<JsonConditions>(wire)));
        }

        /// <summary>
        /// Failure-path pin for a scalar element inside the wire array
        /// (e.g. <c>[123]</c>, <c>["fault"]</c>). The default
        /// <see cref="List{T}"/> deserializer requires each element to
        /// parse as <see cref="JsonConditionWrapper"/>, which for scalar
        /// tokens throws <see cref="JsonException"/>. Mirrors the removed
        /// converter's <c>"Unexpected token '...' inside JsonConditions
        /// array; expected object wrapper."</c> branch.
        /// </summary>
        [TestCase("[123]")]
        [TestCase("[\"fault\"]")]
        [TestCase("[true]")]
        [TestCase("[[]]")]
        public void Deserialize_scalar_or_array_element_throws_JsonException(string wire)
        {
            Assert.Throws<JsonException>((Action)(() => JsonSerializer.Deserialize<JsonConditions>(wire)));
        }

        /// <summary>
        /// Failure-path pin for malformed / truncated JSON. Broken input
        /// bytes surface as a <see cref="JsonException"/> from the
        /// default S.T.J parser; no silent-fallback empty-list result.
        /// </summary>
        [TestCase("[")]
        [TestCase("[{\"Fault\":")]
        [TestCase("[{\"Fault\":{\"dataItemId\":\"f1\"}")]
        [TestCase("not-json-at-all")]
        public void Deserialize_malformed_wire_throws_JsonException(string wire)
        {
            Assert.Throws<JsonException>((Action)(() => JsonSerializer.Deserialize<JsonConditions>(wire)));
        }

        /// <summary>
        /// Round-trip pin for each of the four
        /// <see cref="ConditionLevel"/> enum arms individually: a
        /// single-wrapper conditions list at each level serializes to a
        /// one-key envelope on the wire and deserializes back to the
        /// same wrapper populated on the matching property with the
        /// other three null. Enum-arm exhaustiveness pin per the coverage
        /// FLOOR (§1.0d-trigies-novodecies).
        /// </summary>
        [TestCase("Fault")]
        [TestCase("Warning")]
        [TestCase("Normal")]
        [TestCase("Unavailable")]
        public void RoundTrip_single_level_wrapper_preserves_level(string levelName)
        {
            var wrapper = levelName switch
            {
                "Fault" => JsonConditionWrapper.OfFault(MakeEntry("x1", "TEMPERATURE")),
                "Warning" => JsonConditionWrapper.OfWarning(MakeEntry("x1", "TEMPERATURE")),
                "Normal" => JsonConditionWrapper.OfNormal(MakeEntry("x1", "TEMPERATURE")),
                "Unavailable" => JsonConditionWrapper.OfUnavailable(MakeEntry("x1", "TEMPERATURE")),
                _ => throw new System.ArgumentOutOfRangeException(nameof(levelName)),
            };
            var original = new JsonConditions { wrapper };

            var json = JsonSerializer.Serialize(original, SparseOptions());
            var parsed = JsonSerializer.Deserialize<JsonConditions>(json);

            Assert.That(parsed, Is.Not.Null);
            Assert.That(parsed!.Count, Is.EqualTo(1));
            Assert.That(parsed[0].Level, Is.EqualTo(levelName));
            Assert.That(parsed[0].Value!.DataItemId, Is.EqualTo("x1"));

            // Re-emit and confirm byte-identity with the first pass, so
            // the round-trip is symmetric on every level arm.
            var json2 = JsonSerializer.Serialize(parsed, SparseOptions());
            Assert.That(json2, Is.EqualTo(json));
        }

        /// <summary>
        /// Regression pin for the intentional drop of strict single-key
        /// wrapper validation. Pre-7.0 the custom converter rejected
        /// multi-key wrapper envelopes with a named
        /// <c>JsonException</c>; after the structural rewrite the default
        /// deserializer tolerates them silently, populating every named
        /// property on the wrapper. <see cref="JsonConditionWrapper.Value"/>
        /// / <see cref="JsonConditionWrapper.Level"/> resolve by
        /// documented Fault > Warning > Normal > Unavailable precedence.
        /// A future change that reintroduces strict rejection would flip
        /// this test to expect the exception; that is a wire-shape
        /// contract change and should be caught here.
        /// </summary>
        [Test]
        public void Deserialize_multi_key_wrapper_populates_all_and_precedence_wins()
        {
            const string wire =
                "[{\"Fault\":{\"dataItemId\":\"f1\"},\"Warning\":{\"dataItemId\":\"w1\"}}]";

            var conditions = JsonSerializer.Deserialize<JsonConditions>(wire);

            Assert.That(conditions, Is.Not.Null);
            Assert.That(conditions!.Count, Is.EqualTo(1));

            var wrapper = conditions[0];
            Assert.That(wrapper.Fault, Is.Not.Null); Assert.That(wrapper.Fault!.DataItemId, Is.EqualTo("f1"));
            Assert.That(wrapper.Warning, Is.Not.Null); Assert.That(wrapper.Warning!.DataItemId, Is.EqualTo("w1"));
            Assert.That(wrapper.Normal, Is.Null);
            Assert.That(wrapper.Unavailable, Is.Null);

            // Precedence: Fault > Warning > Normal > Unavailable.
            Assert.That(wrapper.Level, Is.EqualTo("Fault"));
            Assert.That(wrapper.Value, Is.SameAs(wrapper.Fault));
            Assert.That(wrapper.ToObservation()!.DataItemId, Is.EqualTo("f1"));
        }

        // ------------------------------------------------------------------
        // Minimal fake IObservationOutput for the observation-taking ctor
        // coverage — real ObservationOutput requires a full ConditionObservation
        // wire-through; this fake exposes only the two fields the ctor reads
        // (DataItemId + the Level value bag entry).
        // ------------------------------------------------------------------

        private sealed class FakeConditionOutput : IObservationOutput
        {
            public static FakeConditionOutput Create(string dataItemId, ConditionLevel level) =>
                new FakeConditionOutput { DataItemId = dataItemId, Level = level };

            public ConditionLevel Level { get; init; }

            public string DataItemId { get; init; } = string.Empty;
            public string DeviceUuid => string.Empty;
            public IDataItem DataItem => null!;
            public DataItemCategory Category => DataItemCategory.CONDITION;
            public string Type => string.Empty;
            public string SubType => string.Empty;
            public string Name => string.Empty;
            public ulong InstanceId => 0;
            public ulong Sequence => 0;
            public DateTime Timestamp => DateTime.UnixEpoch;
            public DateTimeOffset TimeZoneTimestamp => DateTimeOffset.UnixEpoch;
            public string CompositionId => string.Empty;
            public DataItemRepresentation Representation => DataItemRepresentation.VALUE;
            public Quality Quality => Quality.VALID;
            public bool Deprecated => false;
            public bool Extended => false;
            public ObservationValue[] Values => Array.Empty<ObservationValue>();

            public string GetValue(string valueKey) =>
                valueKey == ValueKeys.Level ? Level.ToString() : null!;
        }
    }
}
