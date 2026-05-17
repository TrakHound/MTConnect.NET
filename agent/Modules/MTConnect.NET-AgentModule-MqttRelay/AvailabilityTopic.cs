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
        /// <c>null</c> when either input is null, empty, or whitespace-
        /// only, or when either input contains an MQTT-reserved
        /// character so callers can short-circuit before configuring an
        /// MQTT publish.
        /// </summary>
        /// <param name="topicPrefix">
        /// The configured <c>TopicPrefix</c> value, e.g.
        /// <c>MTConnect</c> or <c>MTConnect/Document</c>. Leading and
        /// trailing <c>/</c> separators are stripped so the resulting
        /// topic stays canonical.
        /// </param>
        /// <param name="agentUuid">
        /// The agent's <c>Uuid</c> identifier. The agentUuid is a
        /// single topic segment so it must not contain <c>/</c>.
        /// </param>
        /// <remarks>
        /// Per MQTT 3.1.1 OASIS standard section 4.7.1.1, MQTT topic
        /// names must not contain the wildcard characters <c>+</c> or
        /// <c>#</c> nor the null character <c>\0</c>; supplying any of
        /// those characters in either input produces a <c>null</c>
        /// return.
        /// </remarks>
        public static string Build(string topicPrefix, string agentUuid)
        {
            // IsNullOrWhiteSpace rejects whitespace-only inputs such as
            // "   ": Trim('/') leaves the whitespace intact and would
            // otherwise produce a malformed topic like "   /Agent/uuid/
            // Available" with a leading-whitespace segment.
            if (string.IsNullOrWhiteSpace(topicPrefix)) return null;
            if (string.IsNullOrWhiteSpace(agentUuid)) return null;

            // MQTT 3.1.1 section 4.7.1.1: '+' and '#' are wildcard
            // characters and the null character is reserved. None of
            // them are legal inside a topic name.
            if (ContainsReservedCharacter(topicPrefix)) return null;
            if (ContainsReservedCharacter(agentUuid)) return null;

            // The agentUuid is a single topic segment so it must not
            // embed a '/'; otherwise the {TopicPrefix}/Agent/{AgentUuid}
            // /Available shape silently fragments across additional
            // segments.
            if (agentUuid.IndexOf('/') >= 0) return null;

            // Canonicalise the prefix: strip a leading or trailing '/'
            // so the resulting topic does not begin with '/' nor
            // contain a stray empty segment.
            var canonicalPrefix = topicPrefix.Trim('/');
            if (canonicalPrefix.Length == 0) return null;

            return $"{canonicalPrefix}/{AgentSegment}/{agentUuid}/{AvailableSegment}";
        }

        private static bool ContainsReservedCharacter(string value)
        {
            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (c == '+' || c == '#' || c == '\0') return true;
            }
            return false;
        }
    }
}
