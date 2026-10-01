// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Reflection;
using System.Text.Json;
using MTConnect.Agents;
using MTConnect.Buffers;
using MTConnect.Clients;
using MTConnect.Configurations;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.Regressions
{
    /// <summary>
    /// Sibling-site structural pin for the DIME-connector native-heap leak
    /// (peer diagnosis dated 2026-08-21). The primary singleton refactor
    /// landed on <c>MTConnect.NET-JSON.JsonFunctions</c> and its cppagent
    /// twin, but the same per-call <c>new JsonSerializerOptions(...)</c>
    /// misuse existed at four other sites inside MTConnect.NET-Common that
    /// also allocate an options object once per Save / Read / Write call:
    /// <see cref="MTConnectAgentInformation"/>,
    /// <see cref="MTConnectClientInformation"/>,
    /// <see cref="MTConnectAssetFileBuffer"/>,
    /// <see cref="AdapterApplicationConfiguration"/>, and
    /// <see cref="AgentConfiguration"/>. Each was flipped to a
    /// <c>private static readonly JsonSerializerOptions</c> field in
    /// PR #249 so the LCG DynamicMethod property-accessor emit is paid
    /// once per assembly-load instead of once per call.
    /// <para/>
    /// This fixture pins the STRUCTURAL shape (a private static
    /// readonly JsonSerializerOptions field exists) rather than an
    /// instance-identity via <c>ReferenceEquals</c>, because the fields
    /// are private and un-exposed. A regression that either
    /// (a) removes the field and re-inlines <c>new JsonSerializerOptions(...)</c>
    /// per call, or (b) flips the field to instance / non-readonly, would
    /// silently reintroduce the LCG leak; each pin below fails cleanly
    /// on such a refactor. The MTConnect.NET-MQTT sibling
    /// (<c>MTConnectMqttMessage._agentInformationOptions</c>) is pinned
    /// in the MqttRelay test project because MTConnect.NET-Common-Tests
    /// does not reference MTConnect.NET-MQTT.
    /// </summary>
    [TestFixture]
    public class JsonSerializerOptionsSiblingSingletonTests
    {
        private static void AssertHoldsStaticReadonlyJsonSerializerOptionsField(System.Type type)
        {
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            var optionsFields = System.Array.FindAll(
                fields,
                f => f.FieldType == typeof(JsonSerializerOptions) && f.IsInitOnly);
            Assert.That(optionsFields.Length, Is.GreaterThanOrEqualTo(1),
                $"{type.FullName} must declare at least one static readonly JsonSerializerOptions field so " +
                "the instance outlives each serialization call. A per-call `new JsonSerializerOptions(...)` " +
                "re-emits LCG DynamicMethod property accessors on every call, and those emits accumulate in " +
                "the runtime's loader heap where the GC cannot reclaim them (+3.2-3.8 MB/h RSS in production).");
        }

        /// <summary>
        /// Pin: <see cref="MTConnectAgentInformation"/> serializes itself to
        /// disk on every Save call. Its <c>_saveOptions</c> must be a shared
        /// singleton, not a per-call allocation.
        /// </summary>
        [Test]
        public void MTConnectAgentInformation_holds_a_static_readonly_JsonSerializerOptions_field()
        {
            AssertHoldsStaticReadonlyJsonSerializerOptionsField(typeof(MTConnectAgentInformation));
        }

        /// <summary>
        /// Pin: <see cref="MTConnectClientInformation"/> mirrors the Agent
        /// information persistence pattern client-side. Its <c>_saveOptions</c>
        /// must be a shared singleton, not a per-call allocation.
        /// </summary>
        [Test]
        public void MTConnectClientInformation_holds_a_static_readonly_JsonSerializerOptions_field()
        {
            AssertHoldsStaticReadonlyJsonSerializerOptionsField(typeof(MTConnectClientInformation));
        }

        /// <summary>
        /// Pin: <see cref="MTConnectAssetFileBuffer"/> writes one JSON file
        /// per asset — potentially many per second under load. Its
        /// <c>_writeOptions</c> must be a shared singleton, not a per-call
        /// allocation.
        /// </summary>
        [Test]
        public void MTConnectAssetFileBuffer_holds_a_static_readonly_JsonSerializerOptions_field()
        {
            AssertHoldsStaticReadonlyJsonSerializerOptionsField(typeof(MTConnectAssetFileBuffer));
        }

        /// <summary>
        /// Pin: <see cref="AdapterApplicationConfiguration"/> deserializes
        /// its config file on startup and on every reload. Its
        /// <c>_readOptions</c> must be a shared singleton, not a per-call
        /// allocation — hot-reload watchers can trigger many parses per
        /// minute.
        /// </summary>
        [Test]
        public void AdapterApplicationConfiguration_holds_a_static_readonly_JsonSerializerOptions_field()
        {
            AssertHoldsStaticReadonlyJsonSerializerOptionsField(typeof(AdapterApplicationConfiguration));
        }

        /// <summary>
        /// Pin: <see cref="AgentConfiguration"/> deserializes its config file
        /// on startup and on every reload. Same singleton discipline as
        /// <see cref="AdapterApplicationConfiguration"/>.
        /// </summary>
        [Test]
        public void AgentConfiguration_holds_a_static_readonly_JsonSerializerOptions_field()
        {
            AssertHoldsStaticReadonlyJsonSerializerOptionsField(typeof(AgentConfiguration));
        }
    }
}
