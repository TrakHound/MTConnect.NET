// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Linq;
using MTConnect.Buffers;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Observations;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Buffers
{
    /// <summary>
    /// Pins the durable (file) observation buffer across a restart: every observation is written, active
    /// conditions are restored as conditions, a clean shutdown is distinguished from a crash, and sequence
    /// lookups work when the reloaded buffer is not full or has gaps. Also pins "at" queries for conditions.
    /// </summary>
    [TestFixture]
    [Category("ObservationFileBuffer")]
    public class DurableObservationBufferTests
    {
        private const int EventKey = 10001;
        private const int ConditionKey = 10002;

        private string _dir = string.Empty;

        [SetUp]
        public void SetUp()
        {
            _dir = Path.Combine(Path.GetTempPath(), "mtconnect-durable-" + Guid.NewGuid().ToString("N"));
        }

        [TearDown]
        public void TearDown()
        {
            try { if (Directory.Exists(_dir)) Directory.Delete(_dir, true); } catch { }
        }


        /// <summary>Pins the behaviour expressed by the test name: more than the old 50,000 write limit is written.</summary>
        [Test]
        public void Every_observation_is_written_when_more_than_50000_are_queued()
        {
            const int count = 120000;

            var buffer = CreateBuffer(count);
            for (var i = 0; i < count; i++) AddEvent(buffer, "V" + i);
            buffer.Dispose();

            var reloaded = CreateBuffer(count);
            try
            {
                Assert.That(reloaded.Load(), Is.True);
                Assert.That(reloaded.LastSequence, Is.EqualTo((ulong)count));
                Assert.That(reloaded.FirstSequence, Is.EqualTo(1UL));
                Assert.That(reloaded.LoadIsComplete, Is.True);
            }
            finally
            {
                reloaded.Dispose();
            }
        }

        /// <summary>Pins the behaviour expressed by the test name: all active conditions come back as conditions.</summary>
        [Test]
        public void Load_restores_every_active_condition()
        {
            var buffer = CreateBuffer(1000);
            AddCondition(buffer, "FAULT", "A");
            AddCondition(buffer, "FAULT", "B");
            AddEvent(buffer, "X");
            buffer.Dispose();

            var reloaded = CreateBuffer(1000);
            try
            {
                reloaded.Load();

                Assert.That(reloaded.CurrentConditions.TryGetValue(ConditionKey, out var conditions), Is.True);
                Assert.That(conditions!.Select(o => o.GetValue(ValueKeys.NativeCode)), Is.EquivalentTo(new[] { "A", "B" }));
                Assert.That(reloaded.CurrentObservations.ContainsKey(ConditionKey), Is.False, "A condition must not be restored as a regular observation.");
            }
            finally
            {
                reloaded.Dispose();
            }
        }

        /// <summary>Pins the behaviour expressed by the test name: a clean shutdown reloads as complete.</summary>
        [Test]
        public void Clean_shutdown_reloads_as_complete()
        {
            var buffer = CreateBuffer(1000);
            for (var i = 0; i < 10; i++) AddEvent(buffer, "V" + i);
            buffer.Dispose();

            var reloaded = CreateBuffer(1000);
            try
            {
                Assert.That(reloaded.Load(), Is.True);
                Assert.That(reloaded.LoadIsComplete, Is.True);
            }
            finally
            {
                reloaded.Dispose();
            }
        }

        /// <summary>Pins the behaviour expressed by the test name: without a clean shutdown the reload is incomplete.</summary>
        [Test]
        public void Reload_without_clean_shutdown_is_incomplete()
        {
            var first = CreateBuffer(1000);
            for (var i = 0; i < 10; i++) AddEvent(first, "V" + i);
            first.WriteAllItems(); // Written to disk, but the buffer is never shut down (as after a crash)

            var reloaded = CreateBuffer(1000);
            try
            {
                Assert.That(reloaded.Load(), Is.True);
                Assert.That(reloaded.LastSequence, Is.EqualTo(10UL));
                Assert.That(reloaded.LoadIsComplete, Is.False);
            }
            finally
            {
                reloaded.Dispose();
                first.Dispose();
            }
        }

        /// <summary>Pins the behaviour expressed by the test name: disposing through the base class still flushes.</summary>
        [Test]
        public void Dispose_through_the_base_class_flushes()
        {
            MTConnectObservationBuffer buffer = CreateBuffer(1000);
            for (var i = 0; i < 10; i++) AddEvent(buffer, "V" + i);
            buffer.Dispose();

            var reloaded = CreateBuffer(1000);
            try
            {
                Assert.That(reloaded.Load(), Is.True);
                Assert.That(reloaded.LastSequence, Is.EqualTo(10UL));
                Assert.That(reloaded.LoadIsComplete, Is.True);
            }
            finally
            {
                reloaded.Dispose();
            }
        }

        /// <summary>Pins the behaviour expressed by the test name: a reload into a larger buffer is queried by sequence.</summary>
        [Test]
        public void Sample_after_reloading_into_a_larger_buffer_starts_at_the_oldest_observation()
        {
            // 1500 observations into a buffer of 1000 keeps sequences 501..1500
            var buffer = CreateBuffer(1000);
            for (var i = 0; i < 1500; i++) AddEvent(buffer, "V" + i);
            buffer.Dispose();

            // Remove the pages retention would have removed (sequences 1..500, pages of 100)
            for (var page = 100; page <= 500; page += 100) File.Delete(Path.Combine(_dir, "observations", page.ToString()));

            var reloaded = CreateBuffer(5000);
            try
            {
                reloaded.Load();
                var first = reloaded.FirstSequence;
                Assert.That(first, Is.GreaterThan(1UL));

                var results = reloaded.GetObservations(new[] { EventKey }, first, 0, 3);
                Assert.That(results.FirstSequence, Is.EqualTo(first));
                Assert.That(results.Observations.Select(o => o.Sequence), Is.EqualTo(new[] { first, first + 1, first + 2 }));

                results = reloaded.GetObservations(new[] { EventKey }, 1400, 1402, 0);
                Assert.That(results.Observations.Select(o => o.Sequence), Is.EqualTo(new ulong[] { 1400, 1401, 1402 }));
            }
            finally
            {
                reloaded.Dispose();
            }
        }

        /// <summary>Pins the behaviour expressed by the test name: a gap in the sequences does not shift lookups.</summary>
        [Test]
        public void Sample_after_a_sequence_gap_returns_the_requested_sequence()
        {
            var buffer = new MTConnectObservationBuffer(new AgentConfiguration { ObservationBufferSize = 1000 });
            AddEvent(buffer, "A");          // sequence 1
            buffer.IncrementSequence(5);    // sequences 2..6 are skipped
            AddEvent(buffer, "B");          // sequence 7
            AddEvent(buffer, "C");          // sequence 8

            var results = buffer.GetObservations(new[] { EventKey }, 7, 0, 10);
            Assert.That(results.Observations.Select(o => o.Sequence), Is.EqualTo(new ulong[] { 7, 8 }));

            var current = buffer.GetCurrentObservations(new[] { EventKey }, 7);
            Assert.That(current.Observations.Select(o => o.GetValue(ValueKeys.Result)), Is.EqualTo(new[] { "B" }));
        }

        /// <summary>Pins the behaviour expressed by the test name: a sample cut short by "count" continues after the last observation returned.</summary>
        [Test]
        public void NextSequence_follows_the_last_observation_when_count_limits_the_results()
        {
            var buffer = new MTConnectObservationBuffer(new AgentConfiguration { ObservationBufferSize = 100 });
            for (var i = 0; i < 250; i++) AddEvent(buffer, "V" + i); // full: holds 151..250

            var results = buffer.GetObservations(new[] { EventKey }, 160, 0, 10);
            Assert.That(results.Observations.Select(o => o.Sequence), Is.EqualTo(Enumerable.Range(160, 10).Select(o => (ulong)o)));
            Assert.That(results.NextSequence, Is.EqualTo(170UL));

            // Not cut short: the next request continues at the agent's next sequence
            results = buffer.GetObservations(new[] { EventKey }, 245, 0, 100);
            Assert.That(results.NextSequence, Is.EqualTo(251UL));

            // Ended by "to": the next request continues after the range
            results = buffer.GetObservations(new[] { EventKey }, 200, 209, 100);
            Assert.That(results.Observations.Select(o => o.Sequence), Is.EqualTo(Enumerable.Range(200, 10).Select(o => (ulong)o)));
            Assert.That(results.NextSequence, Is.EqualTo(210UL));
        }

        /// <summary>Pins the behaviour expressed by the test name: "at" returns every active condition once the archive has rolled past them.</summary>
        [Test]
        public void At_query_returns_every_active_condition_older_than_the_buffer()
        {
            var buffer = new MTConnectObservationBuffer(new AgentConfiguration { ObservationBufferSize = 10 });
            AddCondition(buffer, "FAULT", "A"); // sequence 1
            AddCondition(buffer, "FAULT", "B"); // sequence 2
            for (var i = 0; i < 20; i++) AddEvent(buffer, "V" + i); // the archive now holds 13..22

            var results = buffer.GetCurrentObservations(new[] { ConditionKey, EventKey }, 22);

            Assert.That(results.Observations.Where(o => o.Key == ConditionKey).Select(o => o.GetValue(ValueKeys.NativeCode)), Is.EquivalentTo(new[] { "A", "B" }));
            Assert.That(results.Observations.Where(o => o.Key == EventKey).Select(o => o.Sequence), Is.EqualTo(new ulong[] { 22 }));
            Assert.That(results.Observations.All(o => o.IsValid), Is.True, "Keys without an observation must be left out, not returned empty.");
        }


        private MTConnectObservationFileBuffer CreateBuffer(uint size)
        {
            return new MTConnectObservationFileBuffer(new AgentConfiguration { ObservationBufferSize = size }, _dir);
        }

        private static void AddEvent(MTConnectObservationBuffer buffer, string value)
        {
            var observation = new BufferObservation(EventKey, DataItemCategory.EVENT, DataItemRepresentation.VALUE,
                new[] { new ObservationValue(ValueKeys.Result, value) }, 0, UnixDateTime.Now, Quality.VALID, false, false);
            buffer.AddObservation(ref observation);
        }

        private static void AddCondition(MTConnectObservationBuffer buffer, string level, string nativeCode)
        {
            var observation = new BufferObservation(ConditionKey, DataItemCategory.CONDITION, DataItemRepresentation.VALUE,
                new[] { new ObservationValue(ValueKeys.Level, level), new ObservationValue(ValueKeys.NativeCode, nativeCode) }, 0, UnixDateTime.Now, Quality.VALID, false, false);
            buffer.AddObservation(ref observation);
        }
    }
}
