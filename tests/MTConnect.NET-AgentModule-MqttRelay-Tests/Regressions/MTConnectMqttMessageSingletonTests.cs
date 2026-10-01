// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Reflection;
using System.Text.Json;
using MTConnect.Mqtt;
using NUnit.Framework;

namespace MTConnect.AgentModule.MqttRelay.Tests.Regressions
{
    /// <summary>
    /// Sibling-site structural pin for the DIME-connector native-heap leak
    /// (peer diagnosis dated 2026-08-21). <see cref="MTConnectMqttMessage"/>
    /// (in MTConnect.NET-MQTT) is the fifth per-call-<c>new JsonSerializerOptions</c>
    /// site flipped to a shared static readonly field in PR #249; it lives in
    /// a library MTConnect.NET-Common-Tests does not reference, so its
    /// structural pin lives here in the MqttRelay test project which
    /// transitively references MTConnect.NET-MQTT.
    /// <para/>
    /// Hosted under an MQTT-flavored publisher path — every /agent
    /// information republish flows through
    /// <c>MTConnectMqttMessage.CreateAgentInformation</c>, and its
    /// <c>_agentInformationOptions</c> must be a shared singleton, not a
    /// per-call allocation. See the sibling fixture
    /// <c>JsonSerializerOptionsSiblingSingletonTests</c> in
    /// <c>MTConnect.NET-Common-Tests</c> for the rationale and the twin
    /// pins on the four Common sibling sites.
    /// </summary>
    [TestFixture]
    public class MTConnectMqttMessageSingletonTests
    {
        /// <summary>
        /// Pin: <see cref="MTConnectMqttMessage"/> serializes agent
        /// information into every /Agents/{uuid}/Information republish.
        /// Its <c>_agentInformationOptions</c> must be a shared singleton,
        /// not a per-call allocation, so the LCG DynamicMethod
        /// property-accessor emit is paid once at assembly load rather
        /// than once per publish.
        /// </summary>
        [Test]
        public void MTConnectMqttMessage_holds_a_static_readonly_JsonSerializerOptions_field()
        {
            var fields = typeof(MTConnectMqttMessage).GetFields(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            var optionsFields = System.Array.FindAll(
                fields,
                f => f.FieldType == typeof(JsonSerializerOptions) && f.IsInitOnly);
            Assert.That(optionsFields.Length, Is.GreaterThanOrEqualTo(1),
                "MTConnectMqttMessage must declare at least one static readonly JsonSerializerOptions field so " +
                "the instance outlives each publish. A per-call `new JsonSerializerOptions(...)` re-emits LCG " +
                "DynamicMethod property accessors on every call, and those emits accumulate in the runtime's " +
                "loader heap where the GC cannot reclaim them (+3.2-3.8 MB/h RSS in production).");
        }
    }
}
