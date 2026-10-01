// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;

namespace MTConnect
{
    /// <summary>
    /// Resolves a user-configured list of SSL/TLS protocol names -
    /// e.g. <c>["Tls12", "Tls13"]</c> - into a bitwise-OR'd
    /// <see cref="SslProtocols"/> value the MQTT client stack accepts.
    ///
    /// <para>The resolver validates the user's input at module load so
    /// a misconfiguration surfaces as a clear, immediate error rather
    /// than a silent downgrade. An empty list, an unknown protocol
    /// name, or a protocol name the running framework does not
    /// support all raise <see cref="MqttRelayConfigurationException"/>.
    /// The resolver never returns <see cref="SslProtocols.None"/>.</para>
    ///
    /// <para>The <c>List&lt;string&gt;</c> shape was chosen because
    /// no existing MQTT-module configuration in this codebase exposes
    /// an SslProtocols surface (every module hard-codes
    /// <c>SslProtocols.Tls12</c>). A string list is idiomatic YAML,
    /// round-trips through YamlDotNet without a custom converter, and
    /// lets a user express any subset of protocols without inventing
    /// per-version bool fields.</para>
    /// </summary>
    internal static class MqttRelayTlsProtocolResolver
    {
        /// <summary>
        /// Resolves a sequence of protocol-name strings into a
        /// bitwise-OR'd <see cref="SslProtocols"/> value.
        /// </summary>
        /// <param name="protocolNames">The user-supplied names, each
        /// matching a member of <see cref="SslProtocols"/>
        /// (case-insensitive).</param>
        /// <returns>The resolved bitmask; never
        /// <see cref="SslProtocols.None"/>.</returns>
        /// <exception cref="MqttRelayConfigurationException">
        /// The list is null / empty, contains an unknown or
        /// runtime-unsupported name, contains an entry that resolves
        /// to <see cref="SslProtocols.None"/>, or resolves to an
        /// empty bitmask.</exception>
        public static SslProtocols Resolve(IEnumerable<string> protocolNames)
        {
            if (protocolNames == null)
            {
                throw new MqttRelayConfigurationException(
                    "MqttRelay SslProtocols is not configured. Supply at least one protocol name (e.g. 'Tls12' or 'Tls13').");
            }

            var names = protocolNames.ToList();
            if (names.Count == 0)
            {
                throw new MqttRelayConfigurationException(
                    "MqttRelay SslProtocols list is empty. Supply at least one protocol name (e.g. 'Tls12' or 'Tls13').");
            }

            SslProtocols resolved = SslProtocols.None;
            foreach (var raw in names)
            {
                if (string.IsNullOrWhiteSpace(raw))
                {
                    throw new MqttRelayConfigurationException(
                        "MqttRelay SslProtocols contains a null or blank entry. Every entry must be a non-empty protocol name.");
                }

                var trimmed = raw.Trim();

                // A comma inside a single entry is the most likely YAML
                // typing error (e.g. `sslProtocols: [Tls12,Tls13]`
                // where the user meant `[Tls12, Tls13]`). Enum.TryParse
                // accepts the comma-separated string on some runtimes,
                // which would silently widen the negotiated set;
                // reject it up front with a targeted hint so the user
                // sees what the fix is rather than "unknown value".
                if (trimmed.IndexOf(',') >= 0)
                {
                    throw new MqttRelayConfigurationException(
                        $"MqttRelay SslProtocols entry '{trimmed}' contains a comma. Each entry must name a single SslProtocols member; use separate list entries (e.g. ['Tls12', 'Tls13']) instead of a comma-separated string.");
                }

                // Enum.TryParse with a numeric literal succeeds even
                // when the number is not a defined enum value. Reject
                // numeric input outright: the config surface is
                // named-only so a user cannot accidentally rely on
                // whatever integer happens to be assigned to a
                // particular version.
                if (long.TryParse(trimmed, out _))
                {
                    throw new MqttRelayConfigurationException(
                        $"MqttRelay SslProtocols entry '{trimmed}' is numeric. Only enum member names are accepted (e.g. 'Tls12', 'Tls13').");
                }

                // Enum.TryParse<T> resolves identically across every
                // TFM this project targets (net461 → net8.0) so a
                // per-TFM #if split adds no behavior and only invites
                // one branch to drift from the other.
                if (!Enum.TryParse<SslProtocols>(trimmed, ignoreCase: true, out var parsed)
                    || !Enum.IsDefined(typeof(SslProtocols), parsed))
                {
                    throw new MqttRelayConfigurationException(
                        $"MqttRelay SslProtocols entry '{trimmed}' is not a known SslProtocols value on this .NET runtime. Valid names on this runtime: {DescribeValidNames()}.");
                }

                if (parsed == SslProtocols.None)
                {
                    throw new MqttRelayConfigurationException(
                        "MqttRelay SslProtocols entry 'None' is not permitted. Supply one or more concrete protocol names (e.g. 'Tls12' or 'Tls13').");
                }

                resolved |= parsed;
            }

            if (resolved == SslProtocols.None)
            {
                // Guarded above by the empty-list and None-entry
                // checks; this final guard exists so a future edit
                // that changes the loop cannot silently ship a
                // no-op protocol set.
                throw new MqttRelayConfigurationException(
                    "MqttRelay SslProtocols resolved to no protocols. Supply at least one concrete protocol name (e.g. 'Tls12' or 'Tls13').");
            }

            return resolved;
        }

        /// <summary>
        /// Formats the SslProtocols enum members supported on the
        /// current runtime as a human-readable, comma-separated
        /// string. Used to enrich configuration-error messages so a
        /// user sees which names their framework accepts.
        /// </summary>
        private static string DescribeValidNames()
        {
            var names = Enum.GetNames(typeof(SslProtocols))
                .Where(n => !string.Equals(n, "Default", StringComparison.OrdinalIgnoreCase)
                            && !string.Equals(n, "None", StringComparison.OrdinalIgnoreCase))
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase);
            return string.Join(", ", names);
        }
    }

    /// <summary>
    /// Raised when the MQTT relay module's configuration cannot be
    /// resolved into a usable runtime state - for example when the
    /// user-supplied SslProtocols list is empty or contains an
    /// unknown / unsupported protocol name.
    /// </summary>
    public class MqttRelayConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance with the supplied message.
        /// </summary>
        /// <param name="message">Human-readable diagnostic that
        /// identifies the misconfigured field and describes the
        /// accepted shape.</param>
        public MqttRelayConfigurationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance with the supplied message and
        /// inner exception.
        /// </summary>
        /// <param name="message">Human-readable diagnostic that
        /// identifies the misconfigured field and describes the
        /// accepted shape.</param>
        /// <param name="innerException">The underlying exception that
        /// triggered this diagnostic, if any.</param>
        public MqttRelayConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
