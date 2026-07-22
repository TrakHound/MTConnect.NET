// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Agents
{
    /// <summary>
    /// Resolves the Agent meta-device UUID from the three canonical sources —
    /// operator-supplied config override, persisted <c>agent.information.json</c>
    /// state, and a deterministic UUID v5 derivation — with RFC 4122 validation
    /// applied uniformly on both the override and the persisted paths.
    ///
    /// <para>
    /// Shared between <c>MTConnectAgentApplication.StartAgent</c> and the test
    /// fixtures so the boot-time resolution is exercised by the same code in
    /// both places; the tests cannot silently drift from production semantics.
    /// </para>
    ///
    /// <para>
    /// Resolution order:
    /// </para>
    /// <list type="number">
    ///   <item>
    ///     <b>Path 1 — operator-supplied override.</b> If
    ///     <paramref name="operatorSuppliedUuid"/> parses as an RFC 4122 UUID
    ///     (via <see cref="DeterministicAgentUuid.TryValidate"/>), the canonical
    ///     hyphenated form wins. Malformed input logs a warning via
    ///     <paramref name="warn"/> and falls through to Path 2 / Path 3.
    ///   </item>
    ///   <item>
    ///     <b>Path 2 — persisted state.</b> If
    ///     <paramref name="persistedUuid"/> parses as an RFC 4122 UUID, the
    ///     canonical form wins. Malformed persisted state (e.g. a pre-hardening
    ///     agent version wrote a non-UUID string, or the file was hand-edited)
    ///     logs a warning and falls through to Path 3 so a spec-conformant UUID
    ///     always reaches the wire.
    ///   </item>
    ///   <item>
    ///     <b>Path 3 — deterministic derivation.</b>
    ///     <see cref="DeterministicAgentUuid.Derive"/> over
    ///     <c>(agentName ?? hostname, hostname, port: 0)</c>. Port is <c>0</c>
    ///     because <c>IAgentApplicationConfiguration</c> does not surface a
    ///     listener-port property; the seed is still unique per agent name.
    ///   </item>
    /// </list>
    ///
    /// <para>
    /// Spec rationale — MTConnect Part 1 types the <c>uuid</c> attribute as the
    /// <c>UUID</c> DataType (RFC 4122). Silently forwarding a non-UUID string
    /// (from any source) breaks XSD validation on every typed enum/decimal
    /// DataItem and diverges from the cppagent reference implementation, which
    /// rejects malformed input at ingress.
    /// </para>
    /// </summary>
    public static class AgentUuidResolver
    {
        /// <summary>
        /// Maximum length of an <c>AgentUuid</c> value echoed to the warning
        /// log. Valid UUIDs are 32–38 characters; anything longer is almost
        /// certainly a paste-in-wrong-field mistake (e.g. an API key) whose
        /// full value should not persist in log archives.
        /// </summary>
        private const int LogValueMaxLength = 64;

        /// <summary>
        /// Resolves the Agent meta-device UUID per the three-path algorithm.
        /// </summary>
        /// <param name="operatorSuppliedUuid">
        /// Raw value from <c>AgentApplicationConfiguration.AgentUuid</c>; may
        /// be <see langword="null"/>, empty, or malformed.
        /// </param>
        /// <param name="persistedUuid">
        /// Raw value from <c>MTConnectAgentInformation.Read().Uuid</c>, or
        /// <see langword="null"/> when no <c>agent.information.json</c> exists
        /// (freshly constructed lifecycle). May itself be malformed if a prior
        /// agent boot wrote non-UUID content.
        /// </param>
        /// <param name="agentName">
        /// The logical agent name (typically <c>configuration.ServiceName</c>).
        /// Passed verbatim to <see cref="DeterministicAgentUuid.Derive"/>,
        /// which falls back to <paramref name="hostname"/> when this is
        /// <see langword="null"/> or empty.
        /// </param>
        /// <param name="hostname">
        /// The machine host name (typically
        /// <see cref="Environment.MachineName"/>). Used by
        /// <see cref="DeterministicAgentUuid.Derive"/> as both the fallback
        /// seed component and the deterministic derivation input.
        /// </param>
        /// <param name="warn">
        /// Optional delegate invoked with a human-readable message when Path 1
        /// or Path 2 rejects malformed input. Kept as a plain
        /// <see cref="Action{T}"/> so <c>MTConnect.NET-Common</c> does not
        /// take a hard dependency on any logging framework; the caller adapts
        /// it to NLog, Serilog, or <c>Microsoft.Extensions.Logging</c>. Raw
        /// values echoed via this delegate are pre-sanitised (CRLF stripped,
        /// truncated to <see cref="LogValueMaxLength"/> characters) to guard
        /// against log-injection and secret leakage.
        /// </param>
        /// <returns>
        /// The canonical hyphenated RFC 4122 UUID string that the agent must
        /// adopt for its meta-device.
        /// </returns>
        public static string Resolve(
            string operatorSuppliedUuid,
            string persistedUuid,
            string agentName,
            string hostname,
            Action<string> warn = null)
        {
            // Path 1 — validated operator override wins.
            if (DeterministicAgentUuid.TryValidate(operatorSuppliedUuid, out var normalizedOverride))
            {
                return normalizedOverride;
            }

            // Path 1 rejected but operator supplied something → warn.
            if (!string.IsNullOrEmpty(operatorSuppliedUuid))
            {
                var fallbackKind = DeterministicAgentUuid.TryValidate(persistedUuid, out _)
                    ? "persisted"
                    : "derived";
                warn?.Invoke(string.Format(
                    "AgentUuid override '{0}' is not a valid RFC 4122 UUID; falling back to {1} UUID.",
                    SanitiseForLog(operatorSuppliedUuid),
                    fallbackKind));
            }

            // Path 2 — validated persisted state wins over derivation.
            if (DeterministicAgentUuid.TryValidate(persistedUuid, out var normalizedPersisted))
            {
                return normalizedPersisted;
            }

            // Path 2 rejected but persisted state carried something → warn.
            if (!string.IsNullOrEmpty(persistedUuid))
            {
                warn?.Invoke(
                    "Persisted AgentUuid in agent.information.json is not a valid RFC 4122 UUID; falling back to derived UUID.");
            }

            // Path 3 — deterministic derivation.
            return DeterministicAgentUuid.Derive(agentName, hostname, port: 0);
        }

        /// <summary>
        /// Strips CR/LF (log-injection guard) and truncates to
        /// <see cref="LogValueMaxLength"/> characters (secret-leakage guard)
        /// before echoing an operator-supplied value to the warning log.
        /// </summary>
        private static string SanitiseForLog(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            var stripped = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
            return stripped.Length > LogValueMaxLength
                ? stripped.Substring(0, LogValueMaxLength) + "…"
                : stripped;
        }
    }
}
