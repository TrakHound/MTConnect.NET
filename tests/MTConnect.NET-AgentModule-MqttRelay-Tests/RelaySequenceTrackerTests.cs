// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using NUnit.Framework;

namespace MTConnect.AgentModule.MqttRelay.Tests
{
    /// <summary>
    /// Pins the durable-relay rule that the last-sent sequence only advances over delivered
    /// sequences: after a publish failure (or a dropped observation) nothing is recorded until a
    /// catch-up clears the gap, and queued items the catch-up already delivered are not recorded again.
    /// </summary>
    [TestFixture]
    public class RelaySequenceTrackerTests
    {
        /// <summary>Pins the behaviour expressed by the test name: successful publishes advance the sequence.</summary>
        [Test]
        public void Successful_publish_without_gap_advances()
        {
            var tracker = new RelaySequenceTracker();

            Assert.That(tracker.Completed(success: true, skipped: false), Is.True);
            Assert.That(tracker.HasGap, Is.False);
        }

        /// <summary>Pins the behaviour expressed by the test name: a failure opens a gap and blocks later advances.</summary>
        [Test]
        public void Failure_opens_gap_and_blocks_later_successes()
        {
            var tracker = new RelaySequenceTracker();

            Assert.That(tracker.Completed(success: false, skipped: false), Is.False);
            Assert.That(tracker.HasGap, Is.True);

            Assert.That(tracker.Completed(success: true, skipped: false), Is.False,
                "A later success must not move the last-sent sequence past the undelivered one.");
        }

        /// <summary>Pins the behaviour expressed by the test name: a dropped observation blocks advances until cleared.</summary>
        [Test]
        public void MarkGap_blocks_advances_until_cleared()
        {
            var tracker = new RelaySequenceTracker();

            tracker.MarkGap();
            Assert.That(tracker.Completed(success: true, skipped: false), Is.False);

            tracker.ClearGap();
            Assert.That(tracker.Completed(success: true, skipped: false), Is.True);
        }

        /// <summary>Pins the behaviour expressed by the test name: skipped items never advance the sequence.</summary>
        [Test]
        public void Skipped_items_do_not_advance()
        {
            var tracker = new RelaySequenceTracker();

            Assert.That(tracker.Completed(success: true, skipped: true), Is.False);
            Assert.That(tracker.HasGap, Is.False);
        }

        /// <summary>Pins the behaviour expressed by the test name: sequences at or below the catch-up point are already delivered.</summary>
        [Test]
        public void IsDelivered_uses_catch_up_point()
        {
            var tracker = new RelaySequenceTracker();

            Assert.That(tracker.IsDelivered(1), Is.False);

            tracker.SetCatchUpThrough(100);

            Assert.That(tracker.IsDelivered(99), Is.True);
            Assert.That(tracker.IsDelivered(100), Is.True);
            Assert.That(tracker.IsDelivered(101), Is.False);
        }
    }
}
