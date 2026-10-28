// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Mqtt;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MTConnect.AgentModule.MqttRelay.Tests
{
    /// <summary>
    /// Pins the ordering, bounding and pausing contract of <see cref="MqttPublishQueue{T}"/>, which
    /// decouples agent observation events from MQTT publishing. Completions must be reported in
    /// enqueue order even when publishes finish out of order, so the durable last-sent sequence
    /// only advances over delivered sequences.
    /// </summary>
    [TestFixture]
    public class MqttPublishQueueTests
    {
        /// <summary>Pins the behaviour expressed by the test name: completions are reported in order when publishes finish out of order.</summary>
        [Test]
        public async Task PublishInOrder_reports_completions_in_enqueue_order_when_publishes_finish_out_of_order()
        {
            var queue = new MqttPublishQueue<int>(100, 8);
            var completed = new List<int>();

            // Earlier items take longer, so they finish after later ones
            Func<int, Task<bool>> publish = async i =>
            {
                await Task.Delay((10 - i) * 10);
                return true;
            };

            var items = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var result = await queue.PublishInOrderAsync(items, publish, (i, success) => completed.Add(i));

            Assert.That(result, Is.True);
            Assert.That(completed, Is.EqualTo(items));
        }

        /// <summary>Pins the behaviour expressed by the test name: no more than max in flight publishes run at once.</summary>
        [Test]
        public async Task PublishInOrder_limits_publishes_in_flight()
        {
            const int maxInFlight = 3;
            var queue = new MqttPublishQueue<int>(100, maxInFlight);
            var active = 0;
            var maxActive = 0;

            Func<int, Task<bool>> publish = async i =>
            {
                var now = Interlocked.Increment(ref active);
                lock (this) maxActive = Math.Max(maxActive, now);
                await Task.Delay(10);
                Interlocked.Decrement(ref active);
                return true;
            };

            var items = new List<int>();
            for (var i = 0; i < 20; i++) items.Add(i);

            await queue.PublishInOrderAsync(items, publish, null);

            Assert.That(maxActive, Is.LessThanOrEqualTo(maxInFlight));
            Assert.That(maxActive, Is.GreaterThan(1), "Publishes should overlap up to the in-flight limit.");
        }

        /// <summary>Pins the behaviour expressed by the test name: a failed or throwing publish is reported as failed and later items still complete.</summary>
        [Test]
        public async Task PublishInOrder_reports_failures_and_exceptions_as_unsuccessful()
        {
            var queue = new MqttPublishQueue<int>(100, 4);
            var results = new List<KeyValuePair<int, bool>>();

            Func<int, Task<bool>> publish = i =>
            {
                if (i == 1) return Task.FromResult(false);
                if (i == 2) throw new InvalidOperationException("broker gone");
                return Task.FromResult(true);
            };

            var result = await queue.PublishInOrderAsync(new List<int> { 0, 1, 2, 3 }, publish, (i, success) => results.Add(new KeyValuePair<int, bool>(i, success)));

            Assert.That(result, Is.False);
            Assert.That(results, Is.EqualTo(new[]
            {
                new KeyValuePair<int, bool>(0, true),
                new KeyValuePair<int, bool>(1, false),
                new KeyValuePair<int, bool>(2, false),
                new KeyValuePair<int, bool>(3, true)
            }));
        }

        /// <summary>Pins the behaviour expressed by the test name: enqueue fails once the queue is at capacity.</summary>
        [Test]
        public void TryEnqueue_rejects_items_when_full()
        {
            var queue = new MqttPublishQueue<int>(2, 1);

            Assert.That(queue.TryEnqueue(1), Is.True);
            Assert.That(queue.TryEnqueue(2), Is.True);
            Assert.That(queue.TryEnqueue(3), Is.False);
            Assert.That(queue.Count, Is.EqualTo(2));
        }

        /// <summary>Pins the behaviour expressed by the test name: the consumer publishes queued items in order and runs idle once drained.</summary>
        [Test]
        public async Task RunAsync_publishes_queued_items_in_order_and_runs_idle_when_drained()
        {
            var queue = new MqttPublishQueue<int>(1000, 4);
            var completed = new ConcurrentQueue<int>();
            var idleCount = 0;

            for (var i = 0; i < 50; i++) queue.TryEnqueue(i);

            using (var cts = new CancellationTokenSource())
            {
                var run = queue.RunAsync(i => Task.FromResult(true), (i, s) => completed.Enqueue(i), () => { Interlocked.Increment(ref idleCount); return Task.CompletedTask; }, 7, cts.Token);

                WaitUntil(() => completed.Count == 50 && Volatile.Read(ref idleCount) > 0);

                cts.Cancel();
                await run;
            }

            var expected = new List<int>();
            for (var i = 0; i < 50; i++) expected.Add(i);
            Assert.That(completed, Is.EqualTo(expected));
            Assert.That(queue.Count, Is.EqualTo(0));
        }

        /// <summary>Pins the behaviour expressed by the test name: holding the gate pauses publishing until released.</summary>
        [Test]
        public async Task Holding_the_gate_pauses_publishing()
        {
            var queue = new MqttPublishQueue<int>(1000, 4);
            var completed = new ConcurrentQueue<int>();

            using (var cts = new CancellationTokenSource())
            {
                var run = queue.RunAsync(i => Task.FromResult(true), (i, s) => completed.Enqueue(i), null, 10, cts.Token);

                await queue.Gate.WaitAsync();
                try
                {
                    queue.TryEnqueue(1);
                    queue.TryEnqueue(2);
                    await Task.Delay(200);
                    Assert.That(completed, Is.Empty, "Nothing may be published while the gate is held (e.g. during catch-up).");
                }
                finally
                {
                    queue.Gate.Release();
                }

                WaitUntil(() => completed.Count == 2);

                cts.Cancel();
                await run;
            }

            Assert.That(completed, Is.EqualTo(new[] { 1, 2 }));
        }

        private static void WaitUntil(Func<bool> condition)
        {
            var sw = Stopwatch.StartNew();
            while (!condition())
            {
                if (sw.ElapsedMilliseconds > 5000) Assert.Fail("Timed out waiting for the condition.");
                Thread.Sleep(10);
            }
        }
    }
}
