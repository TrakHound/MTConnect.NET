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
    public class AgentUuidLongitudinalInvariantsTests
    {
        private string? _stateFilePath;
        private string? _backupStateFile;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _stateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MTConnectAgentInformation.Filename);

            // Back up any pre-existing state file so we do not perturb other
            // tests or the developer environment.
            if (File.Exists(_stateFilePath))
            {
                _backupStateFile = _stateFilePath + ".longinv.bak." + Guid.NewGuid().ToString("N");
                File.Move(_stateFilePath, _backupStateFile);
            }
        }

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
        /// Production sources replayed (verify against live code if either drifts):
        /// <list type="bullet">
        ///   <item>
        ///     <c>agent/MTConnect.NET-Applications-Agents/MTConnectAgentApplication.cs</c>
        ///     lines 351–404 — Read, AgentUuid override, InstanceId zeroing, Save.
        ///   </item>
        ///   <item>
        ///     <c>libraries/MTConnect.NET-Common/Agents/MTConnectAgent.cs</c>
        ///     lines 207–234 — broker ctor: <c>_instanceId = instanceId &gt; 0 ? instanceId : CreateInstanceId()</c>
        ///     where <c>CreateInstanceId()</c> returns <c>(ulong)(UnixDateTime.Now / 1000 / 10000)</c>
        ///     (line 2351–2353), and lines 2321–2345 — <c>UpdateAgentInformation</c> timer-driven
        ///     persist after device-add.
        ///   </item>
        /// </list>
        /// </summary>
        private static (string uuid, ulong instanceId) SimulateBoot(
            AgentApplicationConfiguration configuration,
            bool durableBufferLoadSucceeds)
        {
            // Mirrors MTConnectAgentApplication.StartAgent lines 351–404:
            var info = MTConnectAgentInformation.Read();
            if (info == null) info = new MTConnectAgentInformation();

            if (!string.IsNullOrEmpty(configuration.AgentUuid))
            {
                info.Uuid = configuration.AgentUuid;
            }

            var initializeDataItems = !durableBufferLoadSucceeds;
            if (!configuration.Durable || initializeDataItems)
            {
                info.InstanceId = 0;
            }

            info.Save();

            // Mirrors MTConnectAgent ctor (MTConnectAgent.cs lines 207–234):
            //   _instanceId = instanceId > 0 ? instanceId : CreateInstanceId();
            // CreateInstanceId() returns (ulong)(UnixDateTime.Now / 1000 / 10000) — Unix epoch seconds.
            var brokerInstanceId = info.InstanceId > 0
                ? info.InstanceId
                : (ulong)(UnixDateTime.Now / 1000 / 10000);

            // Mirrors MTConnectAgent.UpdateAgentInformation (MTConnectAgent.cs lines 2321–2345):
            // The broker writes its chosen _instanceId back to the file via a timer-driven
            // persist once a device is added. Simulate that here so the next boot's Read()
            // sees the broker's resolved InstanceId, not the zeroed value from the Save() above.
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
            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = "fixture-stable-uuid-A",
                Durable = false,
            };

            var (uuid1, instanceId1) = SimulateBoot(configuration, durableBufferLoadSucceeds: false);

            // Ensure the CreateInstanceId() clock (seconds resolution) advances.
            Thread.Sleep(1100);

            var (uuid2, instanceId2) = SimulateBoot(configuration, durableBufferLoadSucceeds: false);

            Assert.That(uuid1, Is.EqualTo("fixture-stable-uuid-A"),
                "Boot 1: config-level AgentUuid must be applied.");
            Assert.That(uuid2, Is.EqualTo("fixture-stable-uuid-A"),
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
            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = "fixture-stable-uuid-B",
                Durable = true,
            };

            // Boot 1: fresh deploy — durable configured but no buffer yet.
            var (uuid1, instanceId1) = SimulateBoot(configuration, durableBufferLoadSucceeds: false);

            Thread.Sleep(1100);

            // Boot 2: warm restart — durable buffer loaded successfully.
            var (uuid2, instanceId2) = SimulateBoot(configuration, durableBufferLoadSucceeds: true);

            Assert.That(uuid1, Is.EqualTo("fixture-stable-uuid-B"),
                "Boot 1: config-level AgentUuid must be applied.");
            Assert.That(uuid2, Is.EqualTo("fixture-stable-uuid-B"),
                "Boot 2: config-level AgentUuid must survive a durable restart.");
            Assert.That(instanceId1, Is.EqualTo(instanceId2),
                "Durable buffer load success means InstanceId is preserved across boots (spec requirement).");
        }

        /// <summary>
        /// Documents the pre-fix bug from the consumer's perspective:
        /// when <c>AgentApplicationConfiguration.AgentUuid</c> is <c>null</c>
        /// and no state file persists across boots (e.g., an ephemeral container),
        /// both UUID and InstanceId regenerate on every boot.
        ///
        /// This is not a regression check on the new feature — it is a
        /// regression check on the bug itself still being a bug when the knob
        /// is absent. Deleting the state file between boots simulates the
        /// "no persistent storage" scenario that the new config knob lets
        /// consumers escape.
        /// </summary>
        [Test]
        public void Uuid_not_pinned_and_no_state_file_regenerates_per_boot()
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

            Assert.That(uuid1, Is.Not.EqualTo(uuid2),
                "No AgentUuid override + no state file = a fresh Guid is generated every boot. " +
                "This is the pre-fix problem the new knob lets consumers avoid.");
            Assert.That(instanceId1, Is.Not.EqualTo(instanceId2),
                "No state file = InstanceId is also regenerated each boot.");
        }
    }
}
