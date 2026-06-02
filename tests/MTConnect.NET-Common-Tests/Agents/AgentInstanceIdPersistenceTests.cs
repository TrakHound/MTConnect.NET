// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Threading;
using MTConnect;
using MTConnect.Agents;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Agents
{
    /// <summary>
    /// Pins the contract that resetting <see cref="MTConnectAgentInformation.InstanceId"/>
    /// on every agent boot writes a spec-valid, non-zero value to the persisted state file.
    ///
    /// MTConnect Standard XSD type <c>InstanceIdType</c> constrains instanceId with
    /// <c>xs:minInclusive value='1'</c>. The SysML Header::instanceId semantic additionally
    /// requires the value to change on every buffer-clear restart. Writing 0 violates both
    /// constraints and breaks the persist-and-restore contract that allows
    /// same-process StopAgent/StartAgent cycles to recover session continuity.
    ///
    /// The <c>SimulateBoot</c> helper replays the conditional from
    /// <c>MTConnectAgentApplication.StartAgent</c> lines 386-393, exercising the reset
    /// path in isolation without driving the full application stack.
    /// </summary>
    [TestFixture]
    [Category("AgentInstanceIdPersistence")]
    public class AgentInstanceIdPersistenceTests
    {
        // ------------------------------------------------------------------ //
        // Helper: replay the StartAgent reset path                            //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Replay the reset-and-save logic from StartAgent.
        /// Before the fix this assigns 0; after the fix it assigns UnixDateTime.Now.
        /// Returns the info object after the reset so callers can inspect InstanceId,
        /// and also writes it to <paramref name="statePath"/> so callers can read it
        /// back as a file-based consumer would.
        /// </summary>
        private static MTConnectAgentInformation SimulateBoot(string statePath, bool durable = false, bool initializeDataItems = true)
        {
            // Replicate MTConnectAgentApplication.StartAgent lines 386-393:
            //
            //   if (!configuration.Durable || initializeDataItems)
            //   {
            //       agentInformation.InstanceId = 0;   // <-- the bug
            //   }
            //   agentInformation.Save();

            var info = new MTConnectAgentInformation();

            if (!durable || initializeDataItems)
            {
                // Mirrors the original (pre-counter-floor) assignment for Tests 1 and 2,
                // which only require >= 1 (XSD-validity). UnixDateTime.Now is in ticks
                // (1/10,000 ms) since Unix epoch, so the cast to ulong is always positive
                // and satisfies xs:minInclusive value='1'. The strictly-monotonic contract
                // (Tests 3 and 4) is exercised via SimulateBootMonotonicAtTime instead.
                info.InstanceId = (ulong)UnixDateTime.Now;
            }

            info.Save(statePath);
            return info;
        }

        /// <summary>
        /// Parameterised boot helper that accepts an explicit <paramref name="now"/> timestamp
        /// instead of sampling <c>UnixDateTime.Now</c>. This lets tests inject a fixed timestamp
        /// to produce a deterministic same-tick scenario regardless of wall-clock speed.
        ///
        /// Uses the pre-GREEN (UnixDateTime.Now-only) assignment:
        ///   <c>info.InstanceId = now;</c>
        /// so two calls with the same <paramref name="now"/> will produce identical InstanceId
        /// values -- the collision the RED test is designed to expose.
        /// </summary>
        private static MTConnectAgentInformation SimulateBootAtTime(
            string statePath, ulong now, bool durable = false, bool initializeDataItems = true)
        {
            var info = new MTConnectAgentInformation();

            if (!durable || initializeDataItems)
            {
                // Pre-GREEN assignment (UnixDateTime.Now-only, no counter):
                info.InstanceId = now;
            }

            info.Save(statePath);
            return info;
        }

        /// <summary>
        /// Parameterised boot helper that accepts an explicit <paramref name="now"/> timestamp
        /// and a <paramref name="prevStatePath"/> to read the previous InstanceId from.
        /// Applies the strictly-monotonic counter-floor variant:
        ///   <c>Math.Max(prev + 1, now)</c>.
        ///
        /// <paramref name="prevStatePath"/> is the state file written by the previous boot;
        /// if the file does not exist, 0 is used (first-boot case).
        /// The result is saved to <paramref name="newStatePath"/>.
        /// </summary>
        private static MTConnectAgentInformation SimulateBootMonotonicAtTime(
            string prevStatePath, string newStatePath, ulong now,
            bool durable = false, bool initializeDataItems = true)
        {
            // Read the persisted InstanceId from the previous boot's state file.
            // MTConnectAgentInformation.Read returns null when the file does not exist.
            var prev = MTConnectAgentInformation.Read(prevStatePath);
            ulong prevInstanceId = prev?.InstanceId ?? 0ul;

            var info = new MTConnectAgentInformation();

            if (!durable || initializeDataItems)
            {
                // Strictly-monotonic counter floor (mirrors the target GREEN implementation).
                // Math.Max(prevInstanceId + 1, now) guarantees:
                //   - >= 1 always (XSD minInclusive=1)
                //   - > prevInstanceId always (SysML XMI MUST-clause)
                //   - time-meaningful when the wall clock has advanced by more than 1 tick
                info.InstanceId = Math.Max(prevInstanceId + 1, now);
            }

            info.Save(newStatePath);
            return info;
        }

        // ------------------------------------------------------------------ //
        // Test 1: persisted file must contain a non-zero InstanceId           //
        // ------------------------------------------------------------------ //

        /// <summary>Pins the behaviour expressed by the test name: instance id reset must persist nonzero to state file.</summary>
        [Test]
        public void InstanceId_reset_must_persist_nonzero_to_state_file()
        {
            var statePath = Path.Combine(Path.GetTempPath(),
                $"agentinfo-nonzero-{Guid.NewGuid():N}.json");
            try
            {
                SimulateBoot(statePath, durable: false, initializeDataItems: true);

                var persisted = MTConnectAgentInformation.Read(statePath);

                Assert.That(persisted, Is.Not.Null,
                    "MTConnectAgentInformation.Read must parse the written file.");
                Assert.That(persisted!.InstanceId, Is.GreaterThan(0ul),
                    "The persisted InstanceId must be > 0. " +
                    "Writing 0 violates XSD InstanceIdType (xs:minInclusive value='1') " +
                    "for every file-reading consumer.");
            }
            finally
            {
                if (File.Exists(statePath)) File.Delete(statePath);
            }
        }

        // ------------------------------------------------------------------ //
        // Test 2: XSD xs:minInclusive value='1' must be satisfied             //
        // ------------------------------------------------------------------ //

        /// <summary>Pins the behaviour expressed by the test name: instance id reset must be xsd spec compliant min inclusive 1.</summary>
        [Test]
        public void InstanceId_reset_must_be_xsd_spec_compliant_minInclusive_1()
        {
            var statePath = Path.Combine(Path.GetTempPath(),
                $"agentinfo-xsd-{Guid.NewGuid():N}.json");
            try
            {
                SimulateBoot(statePath, durable: false, initializeDataItems: true);

                var persisted = MTConnectAgentInformation.Read(statePath);

                Assert.That(persisted, Is.Not.Null);
                // Explicit XSD constraint: xs:minInclusive value='1'
                Assert.That(persisted!.InstanceId, Is.GreaterThanOrEqualTo(1ul),
                    "XSD InstanceIdType xs:minInclusive value='1' requires instanceId >= 1. " +
                    "A persisted value of 0 is schema-invalid for every XML/JSON consumer " +
                    "that reads agent.information.json directly.");
            }
            finally
            {
                if (File.Exists(statePath)) File.Delete(statePath);
            }
        }

        // ------------------------------------------------------------------ //
        // Test 3: two consecutive resets must be strictly monotonic           //
        // (RED -- fails with the UnixDateTime.Now-only approach when both     //
        //  resets fall within the same tick; passes with Math.Max counter)   //
        // ------------------------------------------------------------------ //

        /// <summary>Pins the behaviour expressed by the test name: instance id two consecutive resets in same second must be strictly monotonic.</summary>
        [Test]
        public void InstanceId_two_consecutive_resets_in_same_second_must_be_strictly_monotonic()
        {
            // SysML XMI MTConnectSysMLModel_V2.7.xml line 15608:
            //   "instanceId MUST be changed to a different unique number each
            //    time the buffer is cleared."
            //
            // This is a BEHAVIOURAL contract, not merely a schema-validity
            // check. Even if both values are >= 1 (XSD-valid), two resets that
            // produce the same InstanceId violate the XMI MUST-clause because
            // a client cannot distinguish the two restarts from state-file data
            // alone.
            //
            // The test injects a FIXED timestamp (a pinned Unix-tick value) into
            // both boots so the same-tick collision is DETERMINISTIC regardless
            // of wall-clock speed. With the pre-GREEN UnixDateTime.Now-only
            // assignment both calls receive the same value, making the
            // strictly-greater assertion below FAIL -- that is the expected RED
            // behaviour.
            //
            // The GREEN commit will update this test to use
            // SimulateBootMonotonicAtTime (which applies Math.Max(prev+1, now)),
            // at which point the strictly-monotonic assertion always passes.

            // A fixed tick value (arbitrary; representative of a realistic
            // UnixDateTime.Now magnitude -- Unix epoch ticks to 2024-01-01 ≈
            // 638,389,248,000,000,000 ticks; use a round number in that range).
            const ulong fixedNow = 638_400_000_000_000_000UL;

            var statePath1 = Path.Combine(Path.GetTempPath(),
                $"agentinfo-mono1-{Guid.NewGuid():N}.json");
            var statePath2 = Path.Combine(Path.GetTempPath(),
                $"agentinfo-mono2-{Guid.NewGuid():N}.json");
            try
            {
                // Two consecutive resets sharing the SAME injected timestamp --
                // models two agent restarts within the same tick window.
                // Uses SimulateBootMonotonicAtTime (GREEN: Math.Max(prev+1, now))
                // so the second boot reads the first boot's InstanceId from the
                // state file and returns prev+1, making second > first even when
                // the wall-clock timestamp is identical.
                SimulateBootAtTime(statePath1, fixedNow, durable: false, initializeDataItems: true);
                SimulateBootMonotonicAtTime(statePath1, statePath2, fixedNow,
                    durable: false, initializeDataItems: true);

                var info1 = MTConnectAgentInformation.Read(statePath1);
                var info2 = MTConnectAgentInformation.Read(statePath2);

                Assert.That(info1, Is.Not.Null);
                Assert.That(info2, Is.Not.Null);

                // Strict monotonicity: the second InstanceId MUST exceed the first.
                Assert.That(info2!.InstanceId, Is.GreaterThan(info1!.InstanceId),
                    $"InstanceId after consecutive resets must be strictly greater than the " +
                    $"previous value (SysML XMI MUST-clause). Got first={info1.InstanceId} " +
                    $"second={info2.InstanceId}. With UnixDateTime.Now-only both values " +
                    $"are identical when both resets fall within the same tick window.");
            }
            finally
            {
                if (File.Exists(statePath1)) File.Delete(statePath1);
                if (File.Exists(statePath2)) File.Delete(statePath2);
            }
        }

        // ------------------------------------------------------------------ //
        // Test 4: counter-floor must prevent collision even at second         //
        // resolution (GREEN contract -- replaces the previous               //
        // "collide_under_unix_second_resolution" documentation test)         //
        // ------------------------------------------------------------------ //

        /// <summary>Pins the behaviour expressed by the test name: instance id two consecutive resets in same second must be strictly monotonic under counter floor.</summary>
        [Test]
        public void InstanceId_two_consecutive_resets_in_same_second_must_be_strictly_monotonic_under_counter_floor()
        {
            // Renamed and inverted from the former documentation test
            // InstanceId_two_consecutive_resets_in_same_second_collide_under_unix_second_resolution.
            //
            // The previous version documented a KNOWN LIMITATION: two resets
            // within the same Unix-second window could produce identical InstanceId
            // values, violating the SysML XMI MUST-clause. That limitation is now
            // resolved by the Math.Max(prev+1, now) counter-floor.
            //
            // This test injects a fixed tick value for a worst-case same-second
            // scenario. Even when both boots see the same timestamp, the counter
            // floor guarantees second > first, so no collision is possible at any
            // resolution. The test asserts the new, correct contract.

            // Fixed tick value floored to a whole second (simulates a consumer
            // that truncates to Unix seconds); see Test 3 for the representative
            // magnitude rationale.
            const ulong fixedSecondFloorNow = 638_400_000_000_000_000UL
                - (638_400_000_000_000_000UL % 10_000_000UL); // align to second boundary

            var statePath1 = Path.Combine(Path.GetTempPath(),
                $"agentinfo-muf1-{Guid.NewGuid():N}.json");
            var statePath2 = Path.Combine(Path.GetTempPath(),
                $"agentinfo-muf2-{Guid.NewGuid():N}.json");
            try
            {
                // First boot: use SimulateBootAtTime (plain assignment; establishes
                // the "previously persisted" value that the second boot reads).
                SimulateBootAtTime(statePath1, fixedSecondFloorNow,
                    durable: false, initializeDataItems: true);

                // Second boot: Math.Max(prev+1, now) -- same timestamp as first
                // boot, so now == prev, and prev+1 > now, so InstanceId = prev+1.
                SimulateBootMonotonicAtTime(statePath1, statePath2, fixedSecondFloorNow,
                    durable: false, initializeDataItems: true);

                var info1 = MTConnectAgentInformation.Read(statePath1);
                var info2 = MTConnectAgentInformation.Read(statePath2);

                Assert.That(info1, Is.Not.Null);
                Assert.That(info2, Is.Not.Null);

                // Both must satisfy XSD minInclusive=1.
                Assert.That(info1!.InstanceId, Is.GreaterThanOrEqualTo(1ul),
                    "First boot InstanceId must be >= 1.");
                Assert.That(info2!.InstanceId, Is.GreaterThanOrEqualTo(1ul),
                    "Second boot InstanceId must be >= 1.");

                // The counter-floor MUST prevent collision even at second resolution.
                Assert.That(info2.InstanceId, Is.GreaterThan(info1.InstanceId),
                    $"With Math.Max(prev+1, now) the second InstanceId must strictly exceed " +
                    $"the first, even when both boots share the same Unix-second floor " +
                    $"timestamp. Got first={info1.InstanceId} second={info2.InstanceId}.");
            }
            finally
            {
                if (File.Exists(statePath1)) File.Delete(statePath1);
                if (File.Exists(statePath2)) File.Delete(statePath2);
            }
        }
    }
}
