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
    [NonParallelizable]
    public class AgentUuidConfigOverrideTests
    {
        private string? _stateFilePath;
        private string? _backupStateFile;

        /// <summary>Sets up the fixture before each test.</summary>
        [SetUp]
        public void SetUp()
        {
            _stateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MTConnectAgentInformation.Filename);
            _backupStateFile = null;

            // Sweep orphan .bak.* files from a prior crashed test run so
            // successive TearDowns cannot restore stale state.
            var directory = Path.GetDirectoryName(_stateFilePath);
            if (directory != null && Directory.Exists(directory))
            {
                foreach (var stale in Directory.EnumerateFiles(directory, MTConnectAgentInformation.Filename + ".bak.*"))
                {
                    try { File.Delete(stale); } catch { /* best-effort */ }
                }
            }

            // Back up any pre-existing state file so we do not perturb other
            // tests or the developer environment.
            if (File.Exists(_stateFilePath))
            {
                _backupStateFile = _stateFilePath + ".bak." + Guid.NewGuid().ToString("N");
                File.Move(_stateFilePath, _backupStateFile);
            }
        }

        /// <summary>Tears down the fixture after each test.</summary>
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
        /// Setting <c>configuration.AgentUuid</c> to a valid RFC 4122 UUID
        /// pins the agent UUID to that exact value (overriding the
        /// <c>Guid.NewGuid()</c> in
        /// <see cref="MTConnectAgentInformation"/>'s parameterless ctor).
        /// </summary>
        [Test]
        public void AgentUuid_set_in_config_flows_through_to_Agent_uuid()
        {
            const string PinnedUuid = "11111111-1111-4111-8111-111111111111";

            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = PinnedUuid,
            };

            // Route through the production resolver so the test exercises the
            // same code path StartAgent uses (see
            // agent/MTConnect.NET-Applications-Agents/MTConnectAgentApplication.cs
            // RunAgent — resolves via AgentUuidResolver.Resolve).
            var agentInformation = ResolveViaProduction(configuration);
            agentInformation.Save();

            // The override must hold both in-memory and after the file
            // round-trip that StartAgent performs.
            Assert.That(agentInformation.Uuid, Is.EqualTo(PinnedUuid));

            var reloaded = MTConnectAgentInformation.Read();
            Assert.That(reloaded, Is.Not.Null);
            Assert.That(reloaded!.Uuid, Is.EqualTo(PinnedUuid));
        }

        /// <summary>
        /// Test (b) — pre-condition: <c>agent.information.json</c> already
        /// stores a different (valid) UUID. The config-level <c>AgentUuid</c>
        /// wins.
        /// </summary>
        [Test]
        public void AgentUuid_set_in_config_takes_precedence_over_state_file()
        {
            const string FromStateFileUuid = "22222222-2222-4222-8222-222222222222";
            const string FromConfigUuid    = "33333333-3333-4333-8333-333333333333";

            // Pre-write the state file with a stale (but valid) UUID.
            var preexisting = new MTConnectAgentInformation(FromStateFileUuid);
            preexisting.Save();

            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = FromConfigUuid,
            };

            var initial = MTConnectAgentInformation.Read();
            Assert.That(initial, Is.Not.Null);
            Assert.That(initial!.Uuid, Is.EqualTo(FromStateFileUuid),
                "Pre-condition: the state file should be read first.");

            var agentInformation = ResolveViaProduction(configuration);
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
            const string InterfaceProbe = "44444444-4444-4444-8444-444444444444";

            IAgentApplicationConfiguration configuration = new AgentApplicationConfiguration
            {
                AgentUuid = InterfaceProbe,
            };

            Assert.That(configuration.AgentUuid, Is.EqualTo(InterfaceProbe));

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

        /// <summary>
        /// Replays the RunAgent UUID resolution slice via the shared
        /// production helper so this fixture cannot silently drift from
        /// StartAgent semantics. Returns the populated
        /// <see cref="MTConnectAgentInformation"/> ready for
        /// <see cref="MTConnectAgentInformation.Save"/>.
        /// </summary>
        private static MTConnectAgentInformation ResolveViaProduction(AgentApplicationConfiguration configuration)
        {
            var existing = MTConnectAgentInformation.Read();
            var freshlyConstructed = existing == null;
            var info = existing ?? new MTConnectAgentInformation();

            info.Uuid = AgentUuidResolver.Resolve(
                operatorSuppliedUuid: configuration.AgentUuid,
                persistedUuid: freshlyConstructed ? null : info.Uuid,
                agentName: configuration.ServiceName,
                hostname: Environment.MachineName);

            return info;
        }
    }
}
