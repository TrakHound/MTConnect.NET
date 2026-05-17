// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using NUnit.Framework;

namespace MTConnect.AgentModule.MqttRelay.Tests
{
    /// <summary>
    /// Pins MQTT topic input validation for
    /// <see cref="AvailabilityTopic.Build(string, string)"/> against the
    /// MQTT 3.1.1 reserved-character rules. The MQTT specification (3.1.1
    /// section 4.7.1.1) reserves '+', '#', and the null character within
    /// topic names; '/' is the topic separator and must not appear inside
    /// a single segment such as a topicPrefix or agentUuid input.
    ///
    /// If a caller supplies a value containing any of those reserved
    /// characters the resulting topic would be malformed (or would alter
    /// the wildcard contract under which other subscribers operate), so
    /// <see cref="AvailabilityTopic.Build(string, string)"/> rejects the
    /// input by returning <c>null</c>.
    ///
    /// Source reference:
    ///   MQTT 3.1.1, OASIS standard, section 4.7.1.1 (topic name and
    ///   topic filter format).
    /// </summary>
    [TestFixture]
    public class AvailabilityTopicValidationTests
    {
        [TestCase("MTConnect+", "agent-uuid-1")]
        [TestCase("MTConnect#", "agent-uuid-1")]
        [TestCase("MTConnect\0", "agent-uuid-1")]
        [TestCase("MTConnect", "agent+uuid")]
        [TestCase("MTConnect", "agent#uuid")]
        [TestCase("MTConnect", "agent\0uuid")]
        public void Build_returns_null_when_reserved_character_present(
            string topicPrefix, string agentUuid)
        {
            Assert.That(
                AvailabilityTopic.Build(topicPrefix, agentUuid),
                Is.Null,
                $"Build({topicPrefix}, {agentUuid}) should reject MQTT-reserved characters.");
        }

        [TestCase("   ", "agent-uuid-1")]
        [TestCase("MTConnect", "   ")]
        public void Build_returns_null_when_input_is_whitespace_only(
            string topicPrefix, string agentUuid)
        {
            // Whitespace-only inputs would otherwise survive Trim('/')
            // intact and produce a malformed topic such as
            // "   /Agent/uuid/Available". IsNullOrWhiteSpace catches
            // both empty and whitespace-only inputs in a single check.
            Assert.That(
                AvailabilityTopic.Build(topicPrefix, agentUuid),
                Is.Null,
                $"Build({topicPrefix}, {agentUuid}) should reject whitespace-only input.");
        }

        [Test]
        public void Build_rejects_slash_inside_agent_uuid_segment()
        {
            // The agentUuid is a single topic segment; embedding '/'
            // would split it across multiple segments and break the
            // {TopicPrefix}/Agent/{AgentUuid}/Available shape.
            Assert.That(
                AvailabilityTopic.Build("MTConnect", "agent/uuid"),
                Is.Null);
        }

        [Test]
        public void Build_strips_leading_slash_from_topic_prefix()
        {
            // Optional canonicalisation: a leading '/' in topicPrefix
            // would yield a topic beginning with '/', which is legal in
            // MQTT 3.1.1 but produces a confusing empty leading
            // segment. Strip it so the resulting topic stays canonical.
            Assert.That(
                AvailabilityTopic.Build("/MTConnect", "agent-1"),
                Is.EqualTo("MTConnect/Agent/agent-1/Available"));
        }

        [Test]
        public void Build_strips_trailing_slash_from_topic_prefix()
        {
            // Optional canonicalisation: a trailing '/' would yield
            // "MTConnect//Agent/agent-1/Available" with a stray empty
            // segment.
            Assert.That(
                AvailabilityTopic.Build("MTConnect/", "agent-1"),
                Is.EqualTo("MTConnect/Agent/agent-1/Available"));
        }
    }
}
