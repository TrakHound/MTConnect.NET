// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using NUnit.Framework;

namespace MTConnect.AgentModule.MqttRelay.Tests
{
    /// <summary>
    /// Pins the MqttRelay last-sent-sequence persistence policy under
    /// DurableRelay. Module.cs previously synchronously wrote to disk
    /// from every successful observation publish in
    /// AgentObservationAdded:
    ///
    ///   File.WriteAllText(path, seq.ToString());
    ///
    /// Under high-rate observation arrival that put a synchronous disk
    /// write on every event-handler invocation, throttling the relay.
    /// LastSentSequencePersister introduces an in-memory tracker plus
    /// an explicit dirty bit; the caller flushes to disk on a timer,
    /// on shutdown, and after every batch boundary, instead of after
    /// every observation.
    /// </summary>
    [TestFixture]
    public class LastSentSequencePersisterTests
    {
        [Test]
        public void Update_marks_dirty_and_round_trips_through_read()
        {
            var persister = new LastSentSequencePersister();

            persister.Update(42UL);

            Assert.That(persister.Read(), Is.EqualTo(42UL));
            Assert.That(persister.IsDirty, Is.True,
                "An in-memory update must mark the persister dirty so the caller knows to flush.");
        }

        [Test]
        public void Read_does_not_clear_dirty_bit()
        {
            var persister = new LastSentSequencePersister();
            persister.Update(7UL);

            _ = persister.Read();

            Assert.That(persister.IsDirty, Is.True,
                "Read must not clear dirty: only Flush establishes durable persistence.");
        }

        [Test]
        public void TryFlush_writes_only_when_dirty_and_clears_dirty_on_success()
        {
            var persister = new LastSentSequencePersister();
            persister.Update(99UL);

            ulong? written = null;
            var flushed = persister.TryFlush(seq => written = seq);

            Assert.That(flushed, Is.True,
                "TryFlush returns true when a write was actually emitted.");
            Assert.That(written, Is.EqualTo(99UL));
            Assert.That(persister.IsDirty, Is.False,
                "After a successful flush the persister must be clean so the next timer tick does not redundantly write.");
        }

        [Test]
        public void TryFlush_no_ops_when_not_dirty()
        {
            var persister = new LastSentSequencePersister();
            // Never updated; persister is clean.
            var written = false;
            var flushed = persister.TryFlush(_ => written = true);

            Assert.That(flushed, Is.False);
            Assert.That(written, Is.False,
                "A clean persister must not invoke the writer; that would burn IOPS for no progress.");
        }

        [Test]
        public void TryFlush_keeps_dirty_when_writer_throws()
        {
            // A failing write must not clear dirty: the caller wants
            // the next timer tick to retry.
            var persister = new LastSentSequencePersister();
            persister.Update(123UL);

            Assert.Throws<System.IO.IOException>(
                () => persister.TryFlush(_ => throw new System.IO.IOException("disk full")));

            Assert.That(persister.IsDirty, Is.True,
                "A failed write must leave the persister dirty so the next flush retries.");
            Assert.That(persister.Read(), Is.EqualTo(123UL));
        }

        [Test]
        public void Update_is_a_last_write_wins_overwrite()
        {
            var persister = new LastSentSequencePersister();
            persister.Update(100UL);
            persister.Update(50UL);

            Assert.That(persister.Read(), Is.EqualTo(50UL),
                "Last write wins; the persister is not a max-watermark.");
        }

        [Test]
        public void Initialize_seeds_value_without_marking_dirty()
        {
            // Used at startup to load the persisted last-sent-sequence
            // from disk: the in-memory value must reflect the on-disk
            // value but the persister must be clean (no flush needed
            // until a real update arrives).
            var persister = new LastSentSequencePersister();
            persister.Initialize(500UL);

            Assert.That(persister.Read(), Is.EqualTo(500UL));
            Assert.That(persister.IsDirty, Is.False,
                "Initialize seeds the in-memory value from disk and must not request a redundant flush.");
        }

        [Test]
        public void TryFlush_no_ops_when_writer_is_null()
        {
            var persister = new LastSentSequencePersister();
            persister.Update(7UL);

            // A null writer means the caller has not wired persistence
            // (e.g. DurableRelay disabled at runtime); the persister
            // must not throw.
            Assert.DoesNotThrow(() => persister.TryFlush(null));
            // Dirty bit unchanged because no write happened.
            Assert.That(persister.IsDirty, Is.True);
        }
    }
}
