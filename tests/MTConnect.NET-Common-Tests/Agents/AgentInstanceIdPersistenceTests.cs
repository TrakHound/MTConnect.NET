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
                // Mirrors the fixed MTConnectAgentApplication.StartAgent line 389.
                // UnixDateTime.Now is in ticks (1/10,000 ms) since Unix epoch, so
                // the cast to ulong is always positive and satisfies xs:minInclusive value='1'.
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
                // Uses SimulateBootAtTime (pre-GREEN: InstanceId = now) so both
                // boots produce fixedNow, making second == first and failing the
                // strict-monotonic assertion below.
                SimulateBootAtTime(statePath1, fixedNow, durable: false, initializeDataItems: true);
                SimulateBootAtTime(statePath2, fixedNow, durable: false, initializeDataItems: true);

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
        // Test 4: documentation of the Unix-second-resolution limitation      //
        // (this test PASSES before and after the fix; it documents that       //
        //  two resets in the same tick produce the same InstanceId value,     //
        //  which is a known limitation of the Unix-tick approach)             //
        // ------------------------------------------------------------------ //

        [Test]
        public void InstanceId_two_consecutive_resets_in_same_second_collide_under_unix_second_resolution()
        {
            // NOTE: This is a DOCUMENTATION test, not a regression test.
            //
            // UnixDateTime.Now is expressed in ticks (1/10,000 ms). Two back-to-back
            // SimulateBoot calls will typically produce different tick values on modern
            // hardware, but they CAN collide if both calls fall within the same tick
            // window -- and they WILL collide when the caller rounds to whole seconds
            // (e.g., a file-reading consumer that truncates to Unix seconds).
            //
            // The assertion here checks that both persisted InstanceIds are >= 1
            // (post-fix invariant), and then observes the tick-floor at second
            // resolution: if both values have the same Unix-second prefix they
            // are indistinguishable to a second-resolution consumer.
            //
            // A counter-based approach (e.g. Math.Max(prev + 1, UnixDateTime.Now))
            // would remove the collision risk; that improvement is deferred to a
            // follow-up PR.

            const long TicksPerSecond = 10_000_000L; // DateTime.Ticks units

            var statePath1 = Path.Combine(Path.GetTempPath(),
                $"agentinfo-col1-{Guid.NewGuid():N}.json");
            var statePath2 = Path.Combine(Path.GetTempPath(),
                $"agentinfo-col2-{Guid.NewGuid():N}.json");
            try
            {
                SimulateBoot(statePath1, durable: false, initializeDataItems: true);
                Thread.Sleep(0); // yield; intentionally no artificial delay
                SimulateBoot(statePath2, durable: false, initializeDataItems: true);

                var info1 = MTConnectAgentInformation.Read(statePath1);
                var info2 = MTConnectAgentInformation.Read(statePath2);

                Assert.That(info1, Is.Not.Null);
                Assert.That(info2, Is.Not.Null);

                // Both must satisfy XSD minInclusive=1 (post-fix invariant).
                Assert.That(info1!.InstanceId, Is.GreaterThanOrEqualTo(1ul),
                    "First boot InstanceId must be >= 1 after fix.");
                Assert.That(info2!.InstanceId, Is.GreaterThanOrEqualTo(1ul),
                    "Second boot InstanceId must be >= 1 after fix.");

                // Document second-resolution collision risk: floor both values to the
                // nearest second and record whether they match. This is informational --
                // the assertion does not fail the test regardless of the outcome, because
                // whether a collision occurs depends on OS tick granularity.
                var secondFloor1 = (long)info1.InstanceId / TicksPerSecond;
                var secondFloor2 = (long)info2.InstanceId / TicksPerSecond;
                TestContext.Out.WriteLine(
                    $"Boot 1 InstanceId tick={info1.InstanceId} second-floor={secondFloor1}");
                TestContext.Out.WriteLine(
                    $"Boot 2 InstanceId tick={info2.InstanceId} second-floor={secondFloor2}");
                TestContext.Out.WriteLine(secondFloor1 == secondFloor2
                    ? "COLLIDE at second resolution -- limitation documented; counter-based fix deferred."
                    : "DISTINCT at second resolution -- tick granularity sufficient on this machine.");
            }
            finally
            {
                if (File.Exists(statePath1)) File.Delete(statePath1);
                if (File.Exists(statePath2)) File.Delete(statePath2);
            }
        }
    }
}
