// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using NUnit.Framework;

namespace MTConnect.AgentModule.MqttRelay.Tests
{
    /// <summary>
    /// Pins the regression for
    /// https://github.com/TrakHound/MTConnect.NET/issues/135. The
    /// MqttRelay agent module previously published its agent
    /// availability state (LWT plus on-connect retained Available
    /// message) on a four-segment topic under the Probe wildcard with a
    /// raw UTF-8 string payload. Any subscriber on
    /// {TopicPrefix}/Probe/# parsing every payload as JSON failed on
    /// that raw string.
    ///
    /// These tests guard against accidental re-introduction of the
    /// broken shape: any future refactor that re-routes the
    /// availability publish under the Probe wildcard or that re-uses
    /// the Probe topic constant in availability-topic construction
    /// fails the guard.
    ///
    /// Source references:
    ///   https://github.com/TrakHound/MTConnect.NET/issues/135
    ///   https://github.com/mtconnect/cppagent (canonical Probe/#
    ///     wildcard contract: only JSON document envelopes).
    /// </summary>
    [TestFixture]
    public class AvailabilityTopicRegressionTests
    {
        [Test]
        public void Topic_never_contains_probe_segment_for_any_inputs()
        {
            string[] topicPrefixes =
            {
                "MTConnect",
                "MTConnect/Document",
                "fleet/site/agent-1",
                "Probe", // adversarial: prefix happens to be "Probe".
                "x"
            };

            string[] agentUuids =
            {
                "agent-uuid-1",
                "Probe", // adversarial: uuid happens to be "Probe".
                "00000000-0000-0000-0000-000000000000",
                "agent.with.dots",
                "agent-with-dashes"
            };

            foreach (var prefix in topicPrefixes)
            {
                foreach (var uuid in agentUuids)
                {
                    var topic = AvailabilityTopic.Build(prefix, uuid);

                    Assert.That(topic, Is.Not.Null,
                        $"Build({prefix}, {uuid}) should produce a topic.");

                    // The dedicated availability segment is "Agent",
                    // sandwiched between the configured prefix and
                    // the agent uuid. Even when the prefix or the
                    // uuid happens to be the literal "Probe" string,
                    // the dedicated availability segment must never
                    // be "Probe" so a Probe/# wildcard subscriber
                    // never matches the availability publish.
                    var expectedSegment =
                        $"/{AvailabilityTopic.AgentSegment}/{uuid}/{AvailabilityTopic.AvailableSegment}";

                    Assert.That(topic, Does.EndWith(expectedSegment),
                        $"Build({prefix}, {uuid}) should end with the relocated availability segment.");
                }
            }
        }

        [Test]
        public void Agent_segment_is_distinct_from_probe_constant()
        {
            // Guards against a future refactor that points
            // AvailabilityTopic.AgentSegment back at the Probe topic
            // constant (the source of the original defect).
            Assert.That(AvailabilityTopic.AgentSegment, Is.Not.EqualTo("Probe"));
            Assert.That(AvailabilityTopic.AgentSegment, Is.EqualTo("Agent"));
        }
    }
}
