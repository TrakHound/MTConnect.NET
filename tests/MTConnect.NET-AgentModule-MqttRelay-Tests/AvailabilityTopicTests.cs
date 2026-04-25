// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using NUnit.Framework;

namespace MTConnect.AgentModule.MqttRelay.Tests
{
    /// <summary>
    /// Pins the MQTT topic on which the MqttRelay agent module publishes
    /// the agent's Availability state. Background and motivation:
    ///
    ///   https://github.com/TrakHound/MTConnect.NET/issues/135
    ///
    /// The MqttRelay's MQTT Last Will and Testament plus on-connect
    /// retained Available message previously emitted on
    ///   {TopicPrefix}/Probe/{AgentUuid}/Available
    /// with a raw "AVAILABLE" / "UNAVAILABLE" UTF-8 string payload.
    /// That four-segment topic falls under the
    ///   {TopicPrefix}/Probe/#
    /// wildcard, breaking the contract that every payload under that
    /// wildcard is a JSON document envelope. The cppagent reference
    /// implementation
    ///   https://github.com/mtconnect/cppagent
    /// publishes only JSON document envelopes under the Probe wildcard
    /// and emits the agent availability state outside the wildcard
    /// (typically on a dedicated agent / status topic).
    ///
    /// These tests pin the corrected shape:
    ///   {TopicPrefix}/Agent/{AgentUuid}/Available
    /// so the Probe wildcard remains pure-JSON for any subscriber.
    /// </summary>
    [TestFixture]
    public class AvailabilityTopicTests
    {
        [Test]
        public void Build_returns_topic_outside_probe_wildcard()
        {
            // Arrange / Act
            var topic = AvailabilityTopic.Build("MTConnect", "agent-uuid-1");

            // Assert: never emit under the Probe/# wildcard.
            Assert.That(topic, Does.Not.Contain("/Probe/"),
                "MqttRelay availability topic must not fall under the Probe/# wildcard.");
        }

        [Test]
        public void Build_uses_dedicated_agent_segment()
        {
            // Arrange
            const string topicPrefix = "MTConnect";
            const string agentUuid = "agent-uuid-1";

            // Act
            var topic = AvailabilityTopic.Build(topicPrefix, agentUuid);

            // Assert: shape matches the relocated contract.
            Assert.That(topic, Is.EqualTo("MTConnect/Agent/agent-uuid-1/Available"));
        }

        [Test]
        public void Build_preserves_multi_segment_topic_prefix()
        {
            // Arrange
            const string topicPrefix = "MTConnect/Document";
            const string agentUuid = "agent-uuid-2";

            // Act
            var topic = AvailabilityTopic.Build(topicPrefix, agentUuid);

            // Assert
            Assert.That(topic, Is.EqualTo("MTConnect/Document/Agent/agent-uuid-2/Available"));
            Assert.That(topic, Does.Not.Contain("/Probe/"));
        }

        [Test]
        public void Build_returns_null_when_topic_prefix_is_null()
        {
            Assert.That(AvailabilityTopic.Build(null, "agent-uuid-1"), Is.Null);
        }

        [Test]
        public void Build_returns_null_when_topic_prefix_is_empty()
        {
            Assert.That(AvailabilityTopic.Build(string.Empty, "agent-uuid-1"), Is.Null);
        }

        [Test]
        public void Build_returns_null_when_agent_uuid_is_null()
        {
            Assert.That(AvailabilityTopic.Build("MTConnect", null), Is.Null);
        }

        [Test]
        public void Build_returns_null_when_agent_uuid_is_empty()
        {
            Assert.That(AvailabilityTopic.Build("MTConnect", string.Empty), Is.Null);
        }

        [Test]
        public void AvailableSegment_constant_pins_trailing_topic_segment()
        {
            Assert.That(AvailabilityTopic.AvailableSegment, Is.EqualTo("Available"));
        }
    }
}
