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
    /// end-to-end. Instead, <see cref="SimulateBoot"/> replays the fresh-construction
    /// path deterministically so the invariants are pinned at the unit level.
    /// </summary>
    [TestFixture]
    public class AgentUuidDeterministicDefaultTests
    {
        private string? _stateFilePath;
        private string? _backupStateFile;

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
        /// <c>cfbff0d1-9375-5685-968a-48ce8b50a653</c>.
        ///
        /// Passing this test confirms that the RFC 4122 §4.3 byte-order
        /// conversion and version/variant masking are correct.
        /// </summary>
        [Test]
        public void DeriveFromSeed_matches_python_uuid_v5_NAMESPACE_DNS_example_com_vector()
        {
            var derived = DeterministicAgentUuid.DeriveFromSeed("example.com");
            Assert.AreEqual("cfbff0d1-9375-5685-968a-48ce8b50a653", derived,
                "DeriveFromSeed must reproduce the canonical UUID v5(NAMESPACE_DNS, 'example.com') vector.");
        }

        // ---------------------------------------------------------------
        // Determinism invariants
        // ---------------------------------------------------------------

        /// <summary>
        /// Two consecutive fresh boots (no state file, no config override)
        /// with the same <paramref name="agentName"/> must produce identical
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
        /// Two fresh boots with distinct <paramref name="agentName"/> values must
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
    }
}
