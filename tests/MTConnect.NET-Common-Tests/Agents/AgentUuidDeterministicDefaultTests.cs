// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using MTConnect.Agents;
using MTConnect.Configurations;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Agents
{
    /// <summary>
    /// Pins the deterministic UUID v5 default behavior introduced to close the
    /// MTConnect v2.7 <c>UuidType</c> "for it's entire life" compliance gap in
    /// ephemeral-container deployments where neither <c>configuration.AgentUuid</c>
    /// nor a persisted <c>agent.information.json</c> state file is present.
    ///
    /// Without this feature, <c>MTConnectAgentInformation</c>'s parameterless ctor
    /// calls <c>Guid.NewGuid()</c>, producing a fresh identity on every container
    /// restart — violating the spec annotation. The fix derives a UUID v5
    /// (RFC 4122 §4.3, DNS namespace, SHA-1) from
    /// <c>"agent:" + agentName + ":" + port</c>, mirroring cppagent's
    /// <c>name_generator</c> prior art.
    ///
    /// These tests do not drive <c>MTConnectAgentApplication.StartAgent</c>
    /// end-to-end. Instead, a <c>SimulateBoot</c> helper replays the fresh-construction
    /// path deterministically so the invariants are pinned at the unit level.
    /// </summary>
    [TestFixture]
    public class AgentUuidDeterministicDefaultTests
    {
        private string? _stateFilePath;
        private string? _backupStateFile;

        /// <summary>Sets up the fixture before each test.</summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _stateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MTConnectAgentInformation.Filename);

            // Back up any pre-existing state file to avoid perturbing other
            // tests or the developer environment.
            if (File.Exists(_stateFilePath))
            {
                _backupStateFile = _stateFilePath + ".detdef.bak." + Guid.NewGuid().ToString("N");
                File.Move(_stateFilePath, _backupStateFile);
            }
        }

        /// <summary>Sets up the fixture before each test.</summary>
        [SetUp]
        public void SetUp()
        {
            // Each test begins with no state file — reentrant so a crash
            // mid-test leaves the suite in a defined state.
            _stateFilePath ??= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MTConnectAgentInformation.Filename);
            if (File.Exists(_stateFilePath))
            {
                File.Delete(_stateFilePath);
            }
        }

        /// <summary>Tears down the fixture after each test.</summary>
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (_stateFilePath != null && File.Exists(_stateFilePath))
            {
                File.Delete(_stateFilePath);
            }
            if (_backupStateFile != null && File.Exists(_backupStateFile))
            {
                File.Move(_backupStateFile, _stateFilePath!);
                _backupStateFile = null;
            }
        }

        /// <summary>
        /// Simulates the fresh-construction path: no state file on disk,
        /// no <c>AgentUuid</c> config override. Returns the UUID that would
        /// be stored in <c>agentInformation</c> after the deterministic-default
        /// gate fires.
        /// </summary>
        private static string SimulateFreshBoot(string agentName, int port = 0)
        {
            // Mirrors the gate added to MTConnectAgentApplication.StartAgent:
            //   bool freshlyConstructed = (MTConnectAgentInformation.Read() == null);
            //   var info = MTConnectAgentInformation.Read() ?? new MTConnectAgentInformation();
            //   if (string.IsNullOrEmpty(configuration.AgentUuid))  // no config override
            //   if (freshlyConstructed)
            //       info.Uuid = DeterministicAgentUuid.Derive(agentName, Environment.MachineName, port);
            var existingInfo = MTConnectAgentInformation.Read();
            var freshlyConstructed = (existingInfo == null);
            var info = existingInfo ?? new MTConnectAgentInformation();

            // No configuration.AgentUuid — this is the ephemeral-container path.
            if (freshlyConstructed && string.IsNullOrEmpty(null /* configuration.AgentUuid */))
            {
                info.Uuid = DeterministicAgentUuid.Derive(agentName, Environment.MachineName, port);
            }

            info.Save();
            return info.Uuid;
        }

        // ---------------------------------------------------------------
        // RFC 4122 §4.3 known-vector test
        // ---------------------------------------------------------------

        /// <summary>
        /// Validates the implementation against the canonical Python
        /// <c>uuid.uuid5(uuid.NAMESPACE_DNS, "example.com")</c> vector
        /// <c>cfbff0d1-9375-5685-968c-48ce8b15ae17</c>.
        ///
        /// Passing this test confirms that the RFC 4122 §4.3 byte-order
        /// conversion and version/variant masking are correct.
        ///
        /// (The task spec cited <c>cfbff0d1-9375-5685-968a-48ce8b50a653</c>;
        /// Python 3 and the C# implementation both produce
        /// <c>968c-48ce8b15ae17</c> — the spec vector was incorrect.)
        /// </summary>
        [Test]
        public void DeriveFromSeed_matches_python_uuid_v5_NAMESPACE_DNS_example_com_vector()
        {
            var derived = DeterministicAgentUuid.DeriveFromSeed("example.com");
            Assert.That(derived, Is.EqualTo("cfbff0d1-9375-5685-968c-48ce8b15ae17"),
                "DeriveFromSeed must reproduce the canonical UUID v5(NAMESPACE_DNS, 'example.com') vector.");
        }

        // ---------------------------------------------------------------
        // Determinism invariants
        // ---------------------------------------------------------------

        /// <summary>
        /// Two consecutive fresh boots (no state file, no config override)
        /// with the same <c>agentName</c> must produce identical
        /// UUIDs — satisfying <c>UuidType</c>'s "for it's entire life" annotation
        /// across ephemeral-container restarts.
        /// </summary>
        [Test]
        public void Default_agent_uuid_is_deterministic_across_two_starts_with_same_agentName_and_no_state_file()
        {
            const string agentName = "fixture-det-agent-A";

            var uuid1 = SimulateFreshBoot(agentName, port: 5000);

            // Delete state file to simulate ephemeral-container re-start.
            if (_stateFilePath != null && File.Exists(_stateFilePath))
                File.Delete(_stateFilePath);

            var uuid2 = SimulateFreshBoot(agentName, port: 5000);

            Assert.That(uuid1, Is.EqualTo(uuid2),
                "Deterministic UUID v5 must be identical across two fresh boots with the same agentName.");
        }

        /// <summary>
        /// The derived UUID must be a valid UUID v5: parseable as <see cref="Guid"/>
        /// and the version digit (first character of the third hyphen-group) must
        /// be <c>'5'</c>.
        /// </summary>
        [Test]
        public void Default_agent_uuid_is_valid_uuid_v5_format()
        {
            const string agentName = "fixture-det-agent-B";

            var uuid = SimulateFreshBoot(agentName, port: 5000);

            // Must be parseable as a Guid.
            Assert.That(Guid.TryParse(uuid, out _), Is.True,
                $"Derived value '{uuid}' must parse as a Guid.");

            // UUID v5: the version digit is the first char of the third group
            // (the 'time_hi_and_version' field, high nibble = 5).
            // Layout: xxxxxxxx-xxxx-5xxx-xxxx-xxxxxxxxxxxx
            var parts = uuid.Split('-');
            Assert.That(parts.Length, Is.EqualTo(5), "Standard UUID must have 5 hyphen-separated groups.");
            Assert.That(parts[2][0], Is.EqualTo('5'),
                $"Version digit (first char of group 3) must be '5' for UUID v5; got '{parts[2][0]}'.");
        }

        /// <summary>
        /// Two fresh boots with distinct <c>agentName</c> values must
        /// produce distinct UUIDs, confirming that the seed differentiates agents.
        /// </summary>
        [Test]
        public void Default_agent_uuid_changes_when_agentName_changes()
        {
            const string agentNameA = "fixture-det-agent-C";
            const string agentNameB = "fixture-det-agent-D";

            var uuidA = DeterministicAgentUuid.Derive(agentNameA, Environment.MachineName, 5000);
            var uuidB = DeterministicAgentUuid.Derive(agentNameB, Environment.MachineName, 5000);

            Assert.That(uuidA, Is.Not.EqualTo(uuidB),
                "Different agentName values must produce distinct UUID v5 values.");
        }

        // ---------------------------------------------------------------
        // RFC 4122 §4.3 bit-layout invariants — the sibling test pins
        // only the version digit; these extend the pin to the variant
        // bits (byte 8) that <see cref="DeterministicAgentUuid.DeriveFromSeed"/>
        // sets to <c>0b10xx_xxxx</c>. Together the two tests characterise
        // every masked bit in the RFC 4122 §4.3 layout.
        // ---------------------------------------------------------------

        /// <summary>
        /// The 9th octet of a derived UUID v5 must have its top two bits
        /// set to <c>10</c> — the "RFC 4122 variant" that
        /// <see cref="DeterministicAgentUuid.DeriveFromSeed"/> stamps into
        /// <c>clock_seq_hi_and_reserved</c>. Decodes the third hex byte of
        /// the 4th hyphen group and asserts the top two bits directly, so
        /// a regression that swaps the mask (e.g. <c>0x40</c> for the
        /// deprecated NCS variant) is caught by an obvious contradiction.
        /// </summary>
        [Test]
        public void DeriveFromSeed_output_has_RFC_4122_variant_high_bits_10()
        {
            var uuid = DeterministicAgentUuid.DeriveFromSeed("example.com");

            var parts = uuid.Split('-');
            Assert.That(parts.Length, Is.EqualTo(5));

            // Group 4 is clock_seq_hi_and_reserved (1 byte) + clock_seq_low (1 byte).
            // The variant bits sit in the top 2 bits of clock_seq_hi_and_reserved,
            // i.e. the first hex byte of parts[3].
            var clockSeqHi = Convert.ToByte(parts[3].Substring(0, 2), 16);
            var variantBits = (clockSeqHi & 0xC0) >> 6;

            Assert.That(variantBits, Is.EqualTo(0b10),
                $"RFC 4122 variant requires top two bits of octet 9 = '10'; got 0x{clockSeqHi:X2} " +
                $"(variant bits = 0b{Convert.ToString(variantBits, 2).PadLeft(2, '0')}).");
        }

        /// <summary>
        /// Deterministic derivation must be sensitive to the port
        /// component of the seed: same <c>agentName</c> and
        /// <c>hostname</c> but different <c>port</c> ⇒ different UUID.
        /// Pins the port participation the class doc-comment promises
        /// ("<c>agent:name:port</c>") — a regression that drops the port
        /// from the seed would collide co-located agents on the same
        /// host + name that only differ by listener port.
        /// </summary>
        [Test]
        public void Derive_port_change_produces_different_uuid_for_same_agent_name()
        {
            const string AgentName = "fixture-det-agent-port";
            const string Hostname = "canonical-host";

            var derivedAt5000 = DeterministicAgentUuid.Derive(AgentName, Hostname, port: 5000);
            var derivedAt8080 = DeterministicAgentUuid.Derive(AgentName, Hostname, port: 8080);

            Assert.That(derivedAt5000, Is.Not.EqualTo(derivedAt8080),
                "Changing the port MUST change the derived UUID — port is part of the seed contract.");
        }

        /// <summary>
        /// <c>port: 0</c> — the documented sentinel used when the listener
        /// port is not available at the call site — must not collide with
        /// any positive port for the same <c>agentName</c> / hostname.
        /// Pins the sentinel's uniqueness in the port axis so a regression
        /// that treats <c>0</c> as "omit" cannot silently collide with a
        /// real port-1 deployment.
        /// </summary>
        [Test]
        public void Derive_port_zero_sentinel_does_not_collide_with_any_positive_port()
        {
            const string AgentName = "fixture-det-agent-port-zero";
            const string Hostname = "canonical-host";

            var derivedAtZero = DeterministicAgentUuid.Derive(AgentName, Hostname, port: 0);
            var derivedAtOne = DeterministicAgentUuid.Derive(AgentName, Hostname, port: 1);

            Assert.That(derivedAtZero, Is.Not.EqualTo(derivedAtOne),
                "port: 0 sentinel must be distinct from port: 1 — 0 is not silently 'omit'.");
        }
    }
}
