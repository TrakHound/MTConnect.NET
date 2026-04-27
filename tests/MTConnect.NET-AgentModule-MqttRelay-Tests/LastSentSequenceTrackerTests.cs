// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MTConnect.AgentModule.MqttRelay.Tests
{
    /// <summary>
    /// Pins the atomic-access policy for the MqttRelay relay-progress
    /// counter. Module.cs previously read and wrote
    /// <c>_lastSentSequence</c> (a 64-bit field) without
    /// <see cref="Interlocked"/>, which on 32-bit hosts is not an
    /// atomic operation: two 32-bit halves are read or written
    /// independently and a concurrent reader can observe a torn value.
    ///
    /// MqttRelay reads <c>_lastSentSequence</c> from observation event
    /// handlers (multiple ThreadPool threads) while the durable-relay
    /// Worker writes it. A torn read could log a wildly wrong "unsent"
    /// figure and, worse, be propagated to the persisted last-sent
    /// sequence file, causing buffered observations to be skipped or
    /// re-sent on the next reconnect.
    ///
    /// LastSentSequenceTracker centralises the atomic read/write so the
    /// policy is unit-testable and so any future caller cannot
    /// regress to a torn-read pattern.
    /// </summary>
    [TestFixture]
    public class LastSentSequenceTrackerTests
    {
        [Test]
        public void Read_returns_zero_on_fresh_tracker()
        {
            var tracker = new LastSentSequenceTracker();
            Assert.That(tracker.Read(), Is.EqualTo(0UL));
        }

        [Test]
        public void Write_round_trips_through_read()
        {
            var tracker = new LastSentSequenceTracker();
            tracker.Write(42UL);
            Assert.That(tracker.Read(), Is.EqualTo(42UL));
        }

        [Test]
        public void Write_supports_full_ulong_range_round_trip()
        {
            // The broker sequence is unsigned; the tracker must round
            // trip values whose bit pattern is negative when reinterpreted
            // as a signed long (Interlocked operates on long internally).
            var tracker = new LastSentSequenceTracker();
            tracker.Write(ulong.MaxValue);
            Assert.That(tracker.Read(), Is.EqualTo(ulong.MaxValue));
        }

        [Test]
        public void Write_overwrites_previous_value()
        {
            var tracker = new LastSentSequenceTracker();
            tracker.Write(100UL);
            tracker.Write(50UL);
            Assert.That(tracker.Read(), Is.EqualTo(50UL),
                "Last write wins; the tracker is not a max-watermark.");
        }

        [Test]
        public void Concurrent_writes_and_reads_observe_only_written_values()
        {
            // Smoke-test for non-atomic torn read: launch a writer
            // ramping through the high ulong range and a reader
            // sampling the value; the reader must never observe a
            // value that was never written. With Interlocked the
            // assertion is trivially true; with naive long field the
            // assertion would fail on 32-bit hosts.
            var tracker = new LastSentSequenceTracker();
            const int iterations = 5000;

            var writer = Task.Run(() =>
            {
                for (int i = 1; i <= iterations; i++)
                {
                    tracker.Write((ulong)i | 0xFFFFFFFF00000000UL);
                }
            });

            var observed = 0;
            var reader = Task.Run(() =>
            {
                while (!writer.IsCompleted)
                {
                    var v = tracker.Read();
                    // Either pre-write zero or any value the writer set.
                    var low = v & 0x00000000FFFFFFFFUL;
                    var high = v & 0xFFFFFFFF00000000UL;
                    if (v != 0UL)
                    {
                        Assert.That(high, Is.EqualTo(0xFFFFFFFF00000000UL),
                            "Torn read: high half not as written.");
                        Assert.That(low, Is.LessThanOrEqualTo((ulong)iterations));
                        observed++;
                    }
                }
            });

            Task.WaitAll(writer, reader);

            // Sanity: writer ran, reader did sample at least once.
            Assert.That(tracker.Read(), Is.EqualTo((ulong)iterations | 0xFFFFFFFF00000000UL));
            Assert.That(observed, Is.GreaterThan(0));
        }
    }
}
