// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace MTConnect.Agents
{
    /// <summary>
    /// Derives a deterministic RFC 4122 §4.3 UUID v5 for an MTConnect Agent's
    /// meta-device UUID when neither operator configuration nor persisted state
    /// supplies one.
    ///
    /// <para>
    /// Mirrors cppagent's <c>name_generator</c> (agent.cpp) and satisfies the
    /// MTConnect v2.7 XSD <c>UuidType</c> annotation "uniquely identifies the
    /// element for it's entire life" across ephemeral-container restarts where
    /// <c>agent.information.json</c> is not preserved between boots.
    /// </para>
    ///
    /// <para>
    /// Algorithm: UUID v5 over the RFC 4122 DNS namespace UUID
    /// <c>6ba7b810-9dad-11d1-80b4-00c04fd430c8</c> with seed
    /// <c>"agent:" + agentName + ":" + port</c>.  Falls back to
    /// <c>"agent:" + hostname + ":" + port</c> when <paramref name="agentName"/>
    /// is <see langword="null"/> or empty.
    /// </para>
    ///
    /// <para>
    /// <b>Port sentinel:</b> when the listener port is not available at the
    /// call site (e.g., <c>IAgentApplicationConfiguration</c> does not surface
    /// a port property), pass <c>0</c> as the port.  The seed remains unique
    /// per agent name and the UUID is still fully deterministic across restarts.
    /// </para>
    /// </summary>
    public static class DeterministicAgentUuid
    {
        // RFC 4122 DNS namespace UUID in big-endian byte order.
        private static readonly byte[] DnsNamespaceBytes = new byte[]
        {
            0x6b, 0xa7, 0xb8, 0x10, 0x9d, 0xad, 0x11, 0xd1,
            0x80, 0xb4, 0x00, 0xc0, 0x4f, 0xd4, 0x30, 0xc8
        };

        /// <summary>
        /// Derives a UUID v5 from
        /// <c>"agent:" + (agentName ?? hostname) + ":" + port</c>.
        /// </summary>
        /// <param name="agentName">
        /// The logical agent name (e.g., <c>ServiceName</c> from configuration).
        /// When <see langword="null"/> or empty, <paramref name="hostname"/> is used.
        /// </param>
        /// <param name="hostname">
        /// The machine host name (typically <c>Environment.MachineName</c>).
        /// Used as fallback when <paramref name="agentName"/> is absent.
        /// </param>
        /// <param name="port">
        /// The agent's listener port.  Pass <c>0</c> when not available at the
        /// call site — the UUID is still deterministic, just port-agnostic.
        /// </param>
        /// <returns>
        /// A lowercase, hyphen-separated UUID v5 string, e.g.
        /// <c>cfbff0d1-9375-5685-968a-48ce8b50a653</c>.
        /// </returns>
        public static string Derive(string agentName, string hostname, int port)
        {
            var nameComponent = !string.IsNullOrEmpty(agentName) ? agentName : hostname;
            var seed = "agent:" + nameComponent + ":" + port.ToString(CultureInfo.InvariantCulture);
            return DeriveFromSeed(seed);
        }

        /// <summary>
        /// Low-level UUID v5 derivation from an arbitrary seed string over the
        /// RFC 4122 DNS namespace UUID.  Exposed as <see langword="public"/> so
        /// tests can verify against canonical cross-language vectors without
        /// requiring <c>[InternalsVisibleTo]</c>.
        /// </summary>
        /// <remarks>
        /// Known-good vector (verified against Python 3 <c>uuid</c> module):
        /// <c>uuid.uuid5(uuid.NAMESPACE_DNS, "example.com")</c>
        /// → <c>cfbff0d1-9375-5685-968c-48ce8b15ae17</c>.
        /// </remarks>
        public static string DeriveFromSeed(string seed)
        {
            var seedBytes = Encoding.UTF8.GetBytes(seed);
            var combined = new byte[DnsNamespaceBytes.Length + seedBytes.Length];
            Buffer.BlockCopy(DnsNamespaceBytes, 0, combined, 0, DnsNamespaceBytes.Length);
            Buffer.BlockCopy(seedBytes, 0, combined, DnsNamespaceBytes.Length, seedBytes.Length);

            byte[] hash;
            using (var sha1 = SHA1.Create())
            {
                hash = sha1.ComputeHash(combined);
            }

            // Take first 16 bytes; apply RFC 4122 §4.3 version + variant bits.
            var uuid = new byte[16];
            Buffer.BlockCopy(hash, 0, uuid, 0, 16);

            // time_hi_and_version (byte 6): clear high nibble, set to 0x50 (version 5).
            uuid[6] = (byte)((uuid[6] & 0x0F) | 0x50);
            // clock_seq_hi_and_reserved (byte 8): clear top 2 bits, set to 0x80 (RFC 4122 variant).
            uuid[8] = (byte)((uuid[8] & 0x3F) | 0x80);

            // .NET Guid uses little-endian for the first three fields; convert
            // from the big-endian RFC 4122 layout before passing to the ctor.
            return new Guid(BigEndianToGuidBytes(uuid)).ToString();
        }

        /// <summary>
        /// Converts a 16-byte RFC 4122 big-endian UUID to the byte order
        /// expected by the <see cref="Guid(byte[])"/> constructor, which uses
        /// little-endian for the first three fields (time_low, time_mid,
        /// time_hi_and_version).
        /// </summary>
        private static byte[] BigEndianToGuidBytes(byte[] beBytes)
        {
            var result = new byte[16];
            // time_low (4 bytes): reverse byte order.
            result[0] = beBytes[3]; result[1] = beBytes[2];
            result[2] = beBytes[1]; result[3] = beBytes[0];
            // time_mid (2 bytes): reverse byte order.
            result[4] = beBytes[5]; result[5] = beBytes[4];
            // time_hi_and_version (2 bytes): reverse byte order.
            result[6] = beBytes[7]; result[7] = beBytes[6];
            // clock_seq_hi_and_reserved + clock_seq_low + node (8 bytes): copy as-is.
            Buffer.BlockCopy(beBytes, 8, result, 8, 8);
            return result;
        }
    }
}
