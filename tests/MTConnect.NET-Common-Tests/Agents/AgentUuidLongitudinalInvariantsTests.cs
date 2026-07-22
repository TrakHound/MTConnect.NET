// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Threading;
using MTConnect.Agents;
using MTConnect.Configurations;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Agents
{
    /// <summary>
    /// Longitudinal behavioural invariants for the
    /// <c>AgentApplicationConfiguration.AgentUuid</c> config-override knob,
    /// across simulated multi-boot cycles.
    ///
    /// These tests do not drive <c>MTConnectAgentApplication.StartAgent</c>
    /// end-to-end (that loads modules, opens a config-file watcher, and starts
    /// the HTTP listener — impractical without full integration infrastructure).
    /// Instead, <see cref="SimulateBoot"/> replays the UUID/InstanceId-handling
    /// sequence deterministically so the invariants can be pinned at the unit
    /// level.
    ///
    /// Achieves the behavioural RED required by CONVENTIONS §1.0d-vicies-semel.
    /// The compile-error RED in the previous commit proved the API absence;
    /// this file pins the longitudinal invariant.
    /// </summary>
    [TestFixture]
    [NonParallelizable]
    public class AgentUuidLongitudinalInvariantsTests
    {
        private string? _stateFilePath;
        private string? _backupStateFile;

        /// <summary>Sets up the fixture before all tests in the class.</summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _stateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MTConnectAgentInformation.Filename);
            _backupStateFile = null;

            // Sweep orphan .longinv.bak.* files from a prior crashed test run.
            var directory = Path.GetDirectoryName(_stateFilePath);
            if (directory != null && Directory.Exists(directory))
            {
                foreach (var stale in Directory.EnumerateFiles(directory, MTConnectAgentInformation.Filename + ".longinv.bak.*"))
                {
                    try { File.Delete(stale); } catch { /* best-effort */ }
                }
            }

            // Back up any pre-existing state file so we do not perturb other
            // tests or the developer environment.
            if (File.Exists(_stateFilePath))
            {
                _backupStateFile = _stateFilePath + ".longinv.bak." + Guid.NewGuid().ToString("N");
                File.Move(_stateFilePath, _backupStateFile);
            }
        }

        /// <summary>Sets up the fixture before each test.</summary>
        [SetUp]
        public void SetUp()
        {
            // Ensure each test begins with no state file — reentrant by design
            // so a crash mid-test leaves the suite in a defined state.
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
        /// Simulates the UUID/InstanceId-handling sequence executed during one
        /// call to <c>MTConnectAgentApplication.StartAgent</c>, followed by the
        /// broker's post-device-add persist.
        ///
        /// The UUID resolution slice routes through
        /// <see cref="AgentUuidResolver.Resolve"/> — the same call production
        /// makes — so this fixture cannot silently drift from StartAgent
        /// semantics (branch order, guard shape, validation).
        ///
        /// InstanceId handling remains inline because it has no shared
        /// helper: it depends on <c>configuration.Durable</c> +
        /// <c>durableBufferLoadSucceeds</c> and mirrors the broker ctor's
        /// <c>_instanceId = instanceId &gt; 0 ? instanceId : CreateInstanceId()</c>
        /// contract (<c>libraries/MTConnect.NET-Common/Agents/MTConnectAgent.cs</c>).
        /// </summary>
        private static (string uuid, ulong instanceId) SimulateBoot(
            AgentApplicationConfiguration configuration,
            bool durableBufferLoadSucceeds)
        {
            var existing = MTConnectAgentInformation.Read();
            var freshlyConstructed = existing == null;
            var info = existing ?? new MTConnectAgentInformation();

            info.Uuid = AgentUuidResolver.Resolve(
                operatorSuppliedUuid: configuration.AgentUuid,
                persistedUuid: freshlyConstructed ? null : info.Uuid,
                agentName: configuration.ServiceName,
                hostname: Environment.MachineName);

            var initializeDataItems = !durableBufferLoadSucceeds;
            if (!configuration.Durable || initializeDataItems)
            {
                info.InstanceId = 0;
            }

            info.Save();

            // Mirrors MTConnectAgent ctor:
            //   _instanceId = instanceId > 0 ? instanceId : CreateInstanceId();
            // CreateInstanceId() = (ulong)(UnixDateTime.Now / 1000 / 10000) — Unix epoch seconds.
            var brokerInstanceId = info.InstanceId > 0
                ? info.InstanceId
                : (ulong)(UnixDateTime.Now / 1000 / 10000);

            // Broker writes _instanceId back via UpdateAgentInformation once a
            // device is added; simulate that so the next boot's Read() sees
            // the broker's resolved InstanceId, not the zeroed value.
            info.InstanceId = brokerInstanceId;
            info.Save();

            return (info.Uuid, brokerInstanceId);
        }

        /// <summary>
        /// When <c>AgentApplicationConfiguration.AgentUuid</c> is set and the
        /// buffer is non-durable, the config-pinned UUID must survive both boots
        /// unchanged. The InstanceId MUST differ between boots because the buffer
        /// is not preserved (no durable load success).
        /// </summary>
        [Test]
        public void Uuid_pinned_via_config_survives_two_non_durable_boots()
        {
            const string PinnedUuid = "aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa";

            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = PinnedUuid,
                Durable = false,
            };

            var (uuid1, instanceId1) = SimulateBoot(configuration, durableBufferLoadSucceeds: false);

            // Ensure the CreateInstanceId() clock (seconds resolution) advances.
            Thread.Sleep(1100);

            var (uuid2, instanceId2) = SimulateBoot(configuration, durableBufferLoadSucceeds: false);

            Assert.That(uuid1, Is.EqualTo(PinnedUuid),
                "Boot 1: config-level AgentUuid must be applied.");
            Assert.That(uuid2, Is.EqualTo(PinnedUuid),
                "Boot 2: config-level AgentUuid must survive a non-durable restart.");
            Assert.That(instanceId1, Is.Not.EqualTo(instanceId2),
                "Non-durable buffer means the InstanceId resets each boot — the UUIDs are equal but InstanceIds differ.");
        }

        /// <summary>
        /// When <c>AgentApplicationConfiguration.AgentUuid</c> is set and the
        /// second boot loads its durable buffer successfully, both the UUID and
        /// the InstanceId must be identical across the two boots (the durable
        /// buffer preserves InstanceId per spec).
        ///
        /// Boot 1 simulates a fresh deploy (durable-configured but no prior
        /// buffer data yet, so load "fails" and InstanceId is cleared then
        /// assigned by the broker). Boot 2 simulates a successful durable
        /// reload of that same buffer.
        /// </summary>
        [Test]
        public void Uuid_pinned_via_config_survives_durable_boot_with_buffer_load_success()
        {
            const string PinnedUuid = "bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb";

            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = PinnedUuid,
                Durable = true,
            };

            // Boot 1: fresh deploy — durable configured but no buffer yet.
            var (uuid1, instanceId1) = SimulateBoot(configuration, durableBufferLoadSucceeds: false);

            Thread.Sleep(1100);

            // Boot 2: warm restart — durable buffer loaded successfully.
            var (uuid2, instanceId2) = SimulateBoot(configuration, durableBufferLoadSucceeds: true);

            Assert.That(uuid1, Is.EqualTo(PinnedUuid),
                "Boot 1: config-level AgentUuid must be applied.");
            Assert.That(uuid2, Is.EqualTo(PinnedUuid),
                "Boot 2: config-level AgentUuid must survive a durable restart.");
            Assert.That(instanceId1, Is.EqualTo(instanceId2),
                "Durable buffer load success means InstanceId is preserved across boots (spec requirement).");
        }

        /// <summary>
        /// Post-fix longitudinal invariant: when
        /// <c>AgentApplicationConfiguration.AgentUuid</c> is <see langword="null"/>
        /// and no state file persists across boots (e.g. an ephemeral container),
        /// the meta-device UUID is nevertheless stable across boots because
        /// <see cref="AgentUuidResolver.Resolve"/> Path 3 derives it
        /// deterministically from <c>(agentName ?? hostname, hostname, port: 0)</c>.
        /// The <c>InstanceId</c> still resets each boot because the durable
        /// buffer did not load (spec-correct behaviour for <c>Header.instanceId</c>).
        /// </summary>
        [Test]
        public void Uuid_not_pinned_and_no_state_file_is_stable_via_deterministic_derivation()
        {
            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = null,
                Durable = false,
            };

            var (uuid1, instanceId1) = SimulateBoot(configuration, durableBufferLoadSucceeds: false);

            // Simulate ephemeral container / no persistent storage: delete state file
            // so the next boot cannot read the UUID that the first boot stored.
            if (_stateFilePath != null && File.Exists(_stateFilePath))
            {
                File.Delete(_stateFilePath);
            }

            Thread.Sleep(1100);

            var (uuid2, instanceId2) = SimulateBoot(configuration, durableBufferLoadSucceeds: false);

            Assert.That(uuid1, Is.EqualTo(uuid2),
                "No override + no state file, but Path 3 derives deterministically " +
                "from (agentName ?? hostname, hostname, port) — the meta-device UUID is stable " +
                "across boots. This is the whole point of the AgentUuidResolver + " +
                "DeterministicAgentUuid.Derive stack introduced by #168.");
            Assert.That(instanceId1, Is.Not.EqualTo(instanceId2),
                "No state file = InstanceId is still regenerated each boot " +
                "(Header.instanceId is per-boot by spec; separate concern from UUID stability).");
        }
    }
}
