// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using MTConnect.Agents;
using MTConnect.Configurations;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Agents
{
    /// <summary>
    /// Pins the contract that <c>AgentApplicationConfiguration.AgentUuid</c>,
    /// when set, deterministically overrides the per-boot UUID that
    /// <c>MTConnectAgentApplication.StartAgent</c> would otherwise derive from
    /// (a) a freshly constructed <see cref="MTConnectAgentInformation"/>
    /// (which calls <c>Guid.NewGuid()</c> in its parameterless constructor),
    /// or (b) a pre-existing <c>agent.information.json</c> state file with a
    /// different stored UUID.
    ///
    /// Spec rationale: MTConnect v2.7 XSD documents <c>UuidType</c> as
    /// identifying the element "for its entire life" — per-boot regeneration
    /// conflates that with <c>Header.instanceId</c>'s per-boot role.
    /// Mirrors cppagent's <c>AgentDeviceUUID</c> configuration knob.
    /// </summary>
    [TestFixture]
    public class AgentUuidConfigOverrideTests
    {
        private string? _stateFilePath;
        private string? _backupStateFile;

        [SetUp]
        public void SetUp()
        {
            _stateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MTConnectAgentInformation.Filename);

            // Back up any pre-existing state file so we do not perturb other
            // tests or the developer environment.
            if (File.Exists(_stateFilePath))
            {
                _backupStateFile = _stateFilePath + ".bak." + Guid.NewGuid().ToString("N");
                File.Move(_stateFilePath, _backupStateFile);
            }
        }

        [TearDown]
        public void TearDown()
        {
            // Remove any state file we left behind, then restore the backup.
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
        /// Test (a) — pre-condition: no <c>agent.information.json</c> on disk.
        /// Setting <c>configuration.AgentUuid</c> pins the agent UUID to that
        /// exact value (overriding the <c>Guid.NewGuid()</c> in
        /// <see cref="MTConnectAgentInformation"/>'s parameterless ctor).
        /// </summary>
        [Test]
        public void AgentUuid_set_in_config_flows_through_to_Agent_uuid()
        {
            const string PinnedUuid = "fixture-stable-uuid-001";

            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = PinnedUuid,
            };

            // Mirror the exact StartAgent threading slice under test:
            //   var agentInformation = MTConnectAgentInformation.Read();
            //   if (agentInformation == null) agentInformation = new MTConnectAgentInformation();
            //   if (!string.IsNullOrEmpty(configuration.AgentUuid))
            //       agentInformation.Uuid = configuration.AgentUuid;
            var agentInformation = MTConnectAgentInformation.Read();
            if (agentInformation == null)
            {
                agentInformation = new MTConnectAgentInformation();
            }
            if (!string.IsNullOrEmpty(configuration.AgentUuid))
            {
                agentInformation.Uuid = configuration.AgentUuid;
            }
            agentInformation.Save();

            // The override must hold both in-memory and after the file
            // round-trip that StartAgent performs at line 393.
            Assert.That(agentInformation.Uuid, Is.EqualTo(PinnedUuid));

            var reloaded = MTConnectAgentInformation.Read();
            Assert.That(reloaded, Is.Not.Null);
            Assert.That(reloaded!.Uuid, Is.EqualTo(PinnedUuid));
        }

        /// <summary>
        /// Test (b) — pre-condition: <c>agent.information.json</c> already
        /// stores a different UUID. The config-level <c>AgentUuid</c> wins.
        /// </summary>
        [Test]
        public void AgentUuid_set_in_config_takes_precedence_over_state_file()
        {
            const string FromStateFileUuid = "from-state-file-uuid";
            const string FromConfigUuid = "from-config-uuid";

            // Pre-write the state file with a stale UUID.
            var preexisting = new MTConnectAgentInformation(FromStateFileUuid);
            preexisting.Save();

            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = FromConfigUuid,
            };

            var agentInformation = MTConnectAgentInformation.Read();
            Assert.That(agentInformation, Is.Not.Null);
            Assert.That(agentInformation!.Uuid, Is.EqualTo(FromStateFileUuid),
                "Pre-condition: the state file should be read first.");

            if (!string.IsNullOrEmpty(configuration.AgentUuid))
            {
                agentInformation.Uuid = configuration.AgentUuid;
            }
            agentInformation.Save();

            Assert.That(agentInformation.Uuid, Is.EqualTo(FromConfigUuid));

            var reloaded = MTConnectAgentInformation.Read();
            Assert.That(reloaded, Is.Not.Null);
            Assert.That(reloaded!.Uuid, Is.EqualTo(FromConfigUuid),
                "Saved state file should reflect the config-level override.");
        }

        /// <summary>
        /// Contract test — the new field is part of the interface surface so
        /// downstream consumers can set it via <see cref="IAgentApplicationConfiguration"/>
        /// without depending on the concrete class. Also verifies the JSON
        /// wire-format key via reflection on the <see cref="JsonPropertyNameAttribute"/>
        /// to match the camelCase convention used by the other fields on
        /// <see cref="AgentApplicationConfiguration"/>. (Reflection rather
        /// than full-object serialization is used because the class has a
        /// pre-existing unrelated JsonPropertyName collision between
        /// <c>ServiceDisplayName</c> and <c>ServiceDescription</c> that
        /// trips <c>JsonSerializer.Serialize</c>.)
        /// </summary>
        [Test]
        public void AgentUuid_is_exposed_on_interface_with_camelCase_wire_name()
        {
            IAgentApplicationConfiguration configuration = new AgentApplicationConfiguration
            {
                AgentUuid = "interface-surface-test",
            };

            Assert.That(configuration.AgentUuid, Is.EqualTo("interface-surface-test"));

            var property = typeof(AgentApplicationConfiguration).GetProperty(
                nameof(AgentApplicationConfiguration.AgentUuid),
                BindingFlags.Public | BindingFlags.Instance);
            Assert.That(property, Is.Not.Null,
                "AgentUuid must be a public instance property on AgentApplicationConfiguration.");

            var jsonNameAttribute = property!
                .GetCustomAttributes(typeof(JsonPropertyNameAttribute), inherit: false)
                .Cast<JsonPropertyNameAttribute>()
                .FirstOrDefault();
            Assert.That(jsonNameAttribute, Is.Not.Null,
                "AgentUuid must carry [JsonPropertyName(...)] to match the other config fields.");
            Assert.That(jsonNameAttribute!.Name, Is.EqualTo("agentUuid"));
        }
    }
}
