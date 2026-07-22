// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Streams.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTConnect.NET_JSON_cppagent_Tests.Streams
{
    /// <summary>
    /// Unit coverage for <see cref="JsonConditionWrapper"/> — the
    /// single-Condition envelope carried by <see cref="JsonConditions"/>
    /// in the cppagent JSON v2 wire shape.
    /// </summary>
    [TestFixture]
    public class JsonConditionWrapperTests
    {
        private static JsonCondition NewCondition(string dataItemId, string? type = null) =>
            new JsonCondition { DataItemId = dataItemId, Type = type! };

        /// <summary>
        /// Options that suppress null-valued properties on the wire, matching
        /// the cppagent JSON v2 sparse-object shape (dataItemId + populated
        /// fields only). Required for byte-identical wire assertions because
        /// <see cref="JsonCondition"/>'s properties do not carry per-property
        /// <c>[JsonIgnore(WhenWritingNull)]</c>; the null-suppression contract
        /// lives on the emission pipeline's options object, matching how the
        /// pre-refactor converter also relied on caller-supplied options.
        /// </summary>
        private static JsonSerializerOptions SparseOptions() =>
            new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

        // ------------------------------------------------------------------
        // Factory methods — each populates exactly one level, leaves the
        // other three null so the wire envelope is single-key on serialize.
        // ------------------------------------------------------------------

        /// <summary>Pins the OfFault factory contract: only Fault set, others null.</summary>
        [Test]
        public void OfFault_populates_only_Fault()
        {
            var c = NewCondition("f1");

            var wrapper = JsonConditionWrapper.OfFault(c);

            Assert.That(wrapper.Fault,       Is.SameAs(c));
            Assert.That(wrapper.Warning,     Is.Null);
            Assert.That(wrapper.Normal,      Is.Null);
            Assert.That(wrapper.Unavailable, Is.Null);
        }

        /// <summary>Pins the OfWarning factory contract: only Warning set, others null.</summary>
        [Test]
        public void OfWarning_populates_only_Warning()
        {
            var c = NewCondition("w1");

            var wrapper = JsonConditionWrapper.OfWarning(c);

            Assert.That(wrapper.Fault,       Is.Null);
            Assert.That(wrapper.Warning,     Is.SameAs(c));
            Assert.That(wrapper.Normal,      Is.Null);
            Assert.That(wrapper.Unavailable, Is.Null);
        }

        /// <summary>Pins the OfNormal factory contract: only Normal set, others null.</summary>
        [Test]
        public void OfNormal_populates_only_Normal()
        {
            var c = NewCondition("n1");

            var wrapper = JsonConditionWrapper.OfNormal(c);

            Assert.That(wrapper.Fault,       Is.Null);
            Assert.That(wrapper.Warning,     Is.Null);
            Assert.That(wrapper.Normal,      Is.SameAs(c));
            Assert.That(wrapper.Unavailable, Is.Null);
        }

        /// <summary>Pins the OfUnavailable factory contract: only Unavailable set, others null.</summary>
        [Test]
        public void OfUnavailable_populates_only_Unavailable()
        {
            var c = NewCondition("u1");

            var wrapper = JsonConditionWrapper.OfUnavailable(c);

            Assert.That(wrapper.Fault,       Is.Null);
            Assert.That(wrapper.Warning,     Is.Null);
            Assert.That(wrapper.Normal,      Is.Null);
            Assert.That(wrapper.Unavailable, Is.SameAs(c));
        }

        // ------------------------------------------------------------------
        // Value accessor — returns the single non-null condition, with
        // precedence Fault > Warning > Normal > Unavailable on multi-populated
        // wrappers, and null on empty.
        // ------------------------------------------------------------------

        /// <summary>Pins Value on an empty wrapper: returns null.</summary>
        [Test]
        public void Value_is_null_on_empty_wrapper()
        {
            Assert.That(new JsonConditionWrapper().Value, Is.Null);
        }

        /// <summary>Pins Value returns the Fault condition when Fault is set.</summary>
        [Test]
        public void Value_returns_Fault_when_only_Fault_set()
        {
            var c = NewCondition("f1");
            Assert.That(JsonConditionWrapper.OfFault(c).Value, Is.SameAs(c));
        }

        /// <summary>Pins Value returns the Warning condition when only Warning is set.</summary>
        [Test]
        public void Value_returns_Warning_when_only_Warning_set()
        {
            var c = NewCondition("w1");
            Assert.That(JsonConditionWrapper.OfWarning(c).Value, Is.SameAs(c));
        }

        /// <summary>Pins Value returns the Normal condition when only Normal is set.</summary>
        [Test]
        public void Value_returns_Normal_when_only_Normal_set()
        {
            var c = NewCondition("n1");
            Assert.That(JsonConditionWrapper.OfNormal(c).Value, Is.SameAs(c));
        }

        /// <summary>Pins Value returns the Unavailable condition when only Unavailable is set.</summary>
        [Test]
        public void Value_returns_Unavailable_when_only_Unavailable_set()
        {
            var c = NewCondition("u1");
            Assert.That(JsonConditionWrapper.OfUnavailable(c).Value, Is.SameAs(c));
        }

        /// <summary>Pins the Fault>Warning>Normal>Unavailable precedence on multi-populated wrappers.</summary>
        [Test]
        public void Value_precedence_is_Fault_Warning_Normal_Unavailable()
        {
            var f = NewCondition("f1");
            var w = NewCondition("w1");
            var n = NewCondition("n1");
            var u = NewCondition("u1");

            Assert.That(new JsonConditionWrapper { Fault = f, Warning = w, Normal = n, Unavailable = u }.Value, Is.SameAs(f));
            Assert.That(new JsonConditionWrapper {              Warning = w, Normal = n, Unavailable = u }.Value, Is.SameAs(w));
            Assert.That(new JsonConditionWrapper {                           Normal = n, Unavailable = u }.Value, Is.SameAs(n));
            Assert.That(new JsonConditionWrapper {                                       Unavailable = u }.Value, Is.SameAs(u));
        }

        // ------------------------------------------------------------------
        // Level accessor — returns the wire property name, matching Value's
        // precedence, and null on empty.
        // ------------------------------------------------------------------

        /// <summary>Pins Level == null on an empty wrapper.</summary>
        [Test]
        public void Level_is_null_on_empty_wrapper()
        {
            Assert.That(new JsonConditionWrapper().Level, Is.Null);
        }

        /// <summary>Pins Level == "Fault" when only Fault is set.</summary>
        [Test]
        public void Level_is_Fault_when_only_Fault_set()
        {
            Assert.That(JsonConditionWrapper.OfFault(NewCondition("f1")).Level, Is.EqualTo("Fault"));
        }

        /// <summary>Pins Level == "Warning" when only Warning is set.</summary>
        [Test]
        public void Level_is_Warning_when_only_Warning_set()
        {
            Assert.That(JsonConditionWrapper.OfWarning(NewCondition("w1")).Level, Is.EqualTo("Warning"));
        }

        /// <summary>Pins Level == "Normal" when only Normal is set.</summary>
        [Test]
        public void Level_is_Normal_when_only_Normal_set()
        {
            Assert.That(JsonConditionWrapper.OfNormal(NewCondition("n1")).Level, Is.EqualTo("Normal"));
        }

        /// <summary>Pins Level == "Unavailable" when only Unavailable is set.</summary>
        [Test]
        public void Level_is_Unavailable_when_only_Unavailable_set()
        {
            Assert.That(JsonConditionWrapper.OfUnavailable(NewCondition("u1")).Level, Is.EqualTo("Unavailable"));
        }

        // ------------------------------------------------------------------
        // ToObservation — materializes the single non-null level into a
        // strongly-typed ConditionObservation at the matching level enum.
        // ------------------------------------------------------------------

        /// <summary>Pins ToObservation returns null on an empty wrapper.</summary>
        [Test]
        public void ToObservation_returns_null_on_empty_wrapper()
        {
            Assert.That(new JsonConditionWrapper().ToObservation(), Is.Null);
        }

        /// <summary>Pins ToObservation returns a FAULT-level condition when Fault is set.</summary>
        [Test]
        public void ToObservation_returns_FAULT_when_Fault_set()
        {
            var observation = JsonConditionWrapper.OfFault(NewCondition("f1", "TEMPERATURE")).ToObservation();
            Assert.That(observation, Is.Not.Null);
            Assert.That(observation.DataItemId, Is.EqualTo("f1"));
            Assert.That((observation as MTConnect.Observations.IConditionObservation)!.Level, Is.EqualTo(MTConnect.Observations.ConditionLevel.FAULT));
        }

        /// <summary>Pins ToObservation returns a WARNING-level condition when Warning is set.</summary>
        [Test]
        public void ToObservation_returns_WARNING_when_Warning_set()
        {
            var observation = JsonConditionWrapper.OfWarning(NewCondition("w1", "POSITION")).ToObservation();
            Assert.That(observation, Is.Not.Null);
            Assert.That(observation.DataItemId, Is.EqualTo("w1"));
            Assert.That((observation as MTConnect.Observations.IConditionObservation)!.Level, Is.EqualTo(MTConnect.Observations.ConditionLevel.WARNING));
        }

        /// <summary>Pins ToObservation returns a NORMAL-level condition when Normal is set.</summary>
        [Test]
        public void ToObservation_returns_NORMAL_when_Normal_set()
        {
            var observation = JsonConditionWrapper.OfNormal(NewCondition("n1", "AVAILABILITY")).ToObservation();
            Assert.That(observation, Is.Not.Null);
            Assert.That(observation.DataItemId, Is.EqualTo("n1"));
            Assert.That((observation as MTConnect.Observations.IConditionObservation)!.Level, Is.EqualTo(MTConnect.Observations.ConditionLevel.NORMAL));
        }

        /// <summary>Pins ToObservation returns an UNAVAILABLE-level condition when Unavailable is set.</summary>
        [Test]
        public void ToObservation_returns_UNAVAILABLE_when_Unavailable_set()
        {
            var observation = JsonConditionWrapper.OfUnavailable(NewCondition("u1", "ROTATION")).ToObservation();
            Assert.That(observation, Is.Not.Null);
            Assert.That(observation.DataItemId, Is.EqualTo("u1"));
            Assert.That((observation as MTConnect.Observations.IConditionObservation)!.Level, Is.EqualTo(MTConnect.Observations.ConditionLevel.UNAVAILABLE));
        }

        // ------------------------------------------------------------------
        // Serialization — the single-key envelope on the wire, with the
        // three null members suppressed by [JsonIgnore(WhenWritingNull)].
        // ------------------------------------------------------------------

        /// <summary>Pins the single-key wire envelope: only the non-null level property is emitted, and the three sibling level properties are absent from the root object.</summary>
        [Test]
        public void Serialize_wrapper_emits_only_the_non_null_level()
        {
            var wrapper = JsonConditionWrapper.OfNormal(NewCondition("n1"));

            var json = JsonSerializer.Serialize(wrapper, SparseOptions());

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.That(root.ValueKind, Is.EqualTo(JsonValueKind.Object));

            // Exactly one root property, and it's Normal (not Fault/Warning/Unavailable).
            var rootProperties = new List<string>();
            foreach (var prop in root.EnumerateObject()) rootProperties.Add(prop.Name);
            Assert.That(rootProperties, Is.EqualTo(new[] { "Normal" }));

            Assert.That(root.TryGetProperty("Normal", out var normal), Is.True);
            Assert.That(normal.GetProperty("dataItemId").GetString(), Is.EqualTo("n1"));
        }

        /// <summary>Pins that Value and Level are not serialized to the wire.</summary>
        [Test]
        public void Serialize_wrapper_omits_convenience_accessors()
        {
            var wrapper = JsonConditionWrapper.OfFault(NewCondition("f1"));

            var json = JsonSerializer.Serialize(wrapper, SparseOptions());

            Assert.That(json, Does.Not.Contain("\"Value\""));
            Assert.That(json, Does.Not.Contain("\"Level\""));
        }

        /// <summary>Pins that an empty wrapper serializes to <c>{}</c> since every property is suppressed as null.</summary>
        [Test]
        public void Serialize_empty_wrapper_emits_empty_object()
        {
            Assert.That(JsonSerializer.Serialize(new JsonConditionWrapper(), SparseOptions()), Is.EqualTo("{}"));
        }

        /// <summary>
        /// Documents that a wrapper with multiple non-null level properties
        /// emits ALL non-null members. The type does not enforce single-key
        /// invariants at runtime; callers should prefer the <c>Of*</c>
        /// factories to guarantee a single-key envelope.
        /// </summary>
        [Test]
        public void Serialize_multi_populated_wrapper_emits_all_non_null_levels()
        {
            var wrapper = new JsonConditionWrapper
            {
                Fault   = NewCondition("f1"),
                Warning = NewCondition("w1"),
            };

            var json = JsonSerializer.Serialize(wrapper, SparseOptions());

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.That(root.TryGetProperty("Fault", out var fault), Is.True);
            Assert.That(fault.GetProperty("dataItemId").GetString(), Is.EqualTo("f1"));
            Assert.That(root.TryGetProperty("Warning", out var warning), Is.True);
            Assert.That(warning.GetProperty("dataItemId").GetString(), Is.EqualTo("w1"));
            Assert.That(root.TryGetProperty("Normal",      out _), Is.False);
            Assert.That(root.TryGetProperty("Unavailable", out _), Is.False);
        }

        // ------------------------------------------------------------------
        // Deserialization — each of the four single-key envelopes populates
        // the matching property and leaves the other three null.
        // ------------------------------------------------------------------

        /// <summary>Pins that a <c>{"Fault":…}</c> envelope populates Fault only.</summary>
        [Test]
        public void Deserialize_Fault_envelope_populates_only_Fault()
        {
            var wrapper = JsonSerializer.Deserialize<JsonConditionWrapper>("{\"Fault\":{\"dataItemId\":\"f1\"}}");

            Assert.That(wrapper, Is.Not.Null);
            Assert.That(wrapper!.Fault, Is.Not.Null);
            Assert.That(wrapper.Fault.DataItemId, Is.EqualTo("f1"));
            Assert.That(wrapper.Warning,     Is.Null);
            Assert.That(wrapper.Normal,      Is.Null);
            Assert.That(wrapper.Unavailable, Is.Null);
            Assert.That(wrapper.Level, Is.EqualTo("Fault"));
        }

        /// <summary>Pins that a <c>{"Warning":…}</c> envelope populates Warning only.</summary>
        [Test]
        public void Deserialize_Warning_envelope_populates_only_Warning()
        {
            var wrapper = JsonSerializer.Deserialize<JsonConditionWrapper>("{\"Warning\":{\"dataItemId\":\"w1\"}}");

            Assert.That(wrapper!.Warning, Is.Not.Null);
            Assert.That(wrapper.Fault,       Is.Null);
            Assert.That(wrapper.Normal,      Is.Null);
            Assert.That(wrapper.Unavailable, Is.Null);
        }

        /// <summary>Pins that a <c>{"Normal":…}</c> envelope populates Normal only.</summary>
        [Test]
        public void Deserialize_Normal_envelope_populates_only_Normal()
        {
            var wrapper = JsonSerializer.Deserialize<JsonConditionWrapper>("{\"Normal\":{\"dataItemId\":\"n1\"}}");

            Assert.That(wrapper!.Normal, Is.Not.Null);
            Assert.That(wrapper.Fault,       Is.Null);
            Assert.That(wrapper.Warning,     Is.Null);
            Assert.That(wrapper.Unavailable, Is.Null);
        }

        /// <summary>Pins that a <c>{"Unavailable":…}</c> envelope populates Unavailable only.</summary>
        [Test]
        public void Deserialize_Unavailable_envelope_populates_only_Unavailable()
        {
            var wrapper = JsonSerializer.Deserialize<JsonConditionWrapper>("{\"Unavailable\":{\"dataItemId\":\"u1\"}}");

            Assert.That(wrapper!.Unavailable, Is.Not.Null);
            Assert.That(wrapper.Fault,   Is.Null);
            Assert.That(wrapper.Warning, Is.Null);
            Assert.That(wrapper.Normal,  Is.Null);
        }
    }
}
