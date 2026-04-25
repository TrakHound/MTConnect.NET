// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect
{
    /// <summary>
    /// Builds the MQTT topic on which the MqttRelay agent module
    /// publishes the agent's Availability state (the MQTT Last Will and
    /// Testament topic plus the on-connect retained Available message).
    /// </summary>
    internal static class AvailabilityTopic
    {
        /// <summary>
        /// Constant topic segment that separates the agent-availability
        /// publishes from the document-envelope publishes (Probe /
        /// Current / Sample / Asset). Subscribers wildcarding on
        /// <c>{TopicPrefix}/Probe/#</c> therefore never receive the raw
        /// availability payload.
        /// </summary>
        public const string AgentSegment = "Agent";

        /// <summary>
        /// Constant trailing topic segment that names the availability
        /// publish.
        /// </summary>
        public const string AvailableSegment = "Available";

        /// <summary>
        /// Builds the full MQTT topic the MqttRelay module uses to
        /// publish the agent's Availability state. Returns
        /// <c>null</c> when either input is null or empty so callers
        /// can short-circuit before configuring an MQTT publish.
        /// </summary>
        /// <param name="topicPrefix">
        /// The configured <c>TopicPrefix</c> value, e.g.
        /// <c>MTConnect</c> or <c>MTConnect/Document</c>.
        /// </param>
        /// <param name="agentUuid">
        /// The agent's <c>Uuid</c> identifier.
        /// </param>
        public static string Build(string topicPrefix, string agentUuid)
        {
            if (string.IsNullOrEmpty(topicPrefix)) return null;
            if (string.IsNullOrEmpty(agentUuid)) return null;

            return $"{topicPrefix}/{AgentSegment}/{agentUuid}/{AvailableSegment}";
        }
    }
}
