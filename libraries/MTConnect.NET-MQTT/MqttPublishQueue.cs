// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Mqtt
{
    /// <summary>
    /// Bounded queue that decouples agent observation events from MQTT publishing. Items are
    /// published by a single consumer with up to <c>maxInFlight</c> publishes outstanding, and
    /// completions are reported strictly in enqueue order so a durable last-sent sequence only
    /// ever moves forward over sequences that were actually delivered.
    /// </summary>
    public sealed class MqttPublishQueue<T>
    {
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);
        private readonly int _capacity;
        private readonly int _maxInFlight;
        private int _count;
        private int _signaled;


        /// <summary>Number of items waiting to be published.</summary>
        public int Count => Volatile.Read(ref _count);

        /// <summary>Held by the consumer while it publishes a batch; hold it to pause publishing.</summary>
        public SemaphoreSlim Gate { get; } = new SemaphoreSlim(1, 1);


        /// <summary>
        /// Creates a queue holding at most <paramref name="capacity"/> items, publishing up to <paramref name="maxInFlight"/> at once.
        /// </summary>
        public MqttPublishQueue(int capacity, int maxInFlight)
        {
            _capacity = Math.Max(1, capacity);
            _maxInFlight = Math.Max(1, maxInFlight);
        }


        /// <summary>
        /// Adds an item to the queue. Returns <c>false</c> (and drops the item) when the queue is full.
        /// </summary>
        public bool TryEnqueue(T item)
        {
            if (Interlocked.Increment(ref _count) > _capacity)
            {
                Interlocked.Decrement(ref _count);
                return false;
            }

            _queue.Enqueue(item);

            // Wake the consumer (at most one pending signal)
            if (Interlocked.Exchange(ref _signaled, 1) == 0) _signal.Release();
            return true;
        }

        /// <summary>
        /// Consumer loop. Drains the queue in batches of up to <paramref name="batchSize"/> while holding
        /// <see cref="Gate"/>, publishing each item with <paramref name="publish"/> and reporting the result
        /// to <paramref name="completed"/> in enqueue order. <paramref name="idle"/> runs (under the Gate)
        /// whenever the queue has been drained.
        /// </summary>
        public async Task RunAsync(Func<T, Task<bool>> publish, Action<T, bool> completed, Func<Task> idle, int batchSize, CancellationToken cancellationToken)
        {
            var batch = new List<T>(batchSize);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await _signal.WaitAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) { break; }

                Interlocked.Exchange(ref _signaled, 0);

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await Gate.WaitAsync(cancellationToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) { break; }

                    try
                    {
                        batch.Clear();
                        while (batch.Count < batchSize && _queue.TryDequeue(out var item))
                        {
                            Interlocked.Decrement(ref _count);
                            batch.Add(item);
                        }

                        if (batch.Count > 0)
                        {
                            await PublishInOrderAsync(batch, publish, completed).ConfigureAwait(false);
                        }
                        else if (idle != null)
                        {
                            await idle().ConfigureAwait(false);
                        }
                    }
                    finally
                    {
                        Gate.Release();
                    }

                    if (batch.Count == 0) break;
                }
            }
        }

        /// <summary>
        /// Publishes <paramref name="items"/> with up to the configured number in flight, reporting each
        /// result to <paramref name="completed"/> in list order. Returns <c>true</c> when every publish succeeded.
        /// </summary>
        public async Task<bool> PublishInOrderAsync(IReadOnlyList<T> items, Func<T, Task<bool>> publish, Action<T, bool> completed)
        {
            var allSucceeded = true;
            var inFlight = new Queue<KeyValuePair<T, Task<bool>>>(Math.Min(items.Count, _maxInFlight));
            var i = 0;

            while (i < items.Count || inFlight.Count > 0)
            {
                while (i < items.Count && inFlight.Count < _maxInFlight)
                {
                    inFlight.Enqueue(new KeyValuePair<T, Task<bool>>(items[i], StartPublish(publish, items[i])));
                    i++;
                }

                var head = inFlight.Dequeue();

                bool success;
                try
                {
                    success = await head.Value.ConfigureAwait(false);
                }
                catch
                {
                    success = false;
                }

                if (!success) allSucceeded = false;
                completed?.Invoke(head.Key, success);
            }

            return allSucceeded;
        }

        private static Task<bool> StartPublish(Func<T, Task<bool>> publish, T item)
        {
            try
            {
                return publish(item) ?? Task.FromResult(false);
            }
            catch (Exception ex)
            {
                var tcs = new TaskCompletionSource<bool>();
                tcs.SetException(ex);
                return tcs.Task;
            }
        }
    }
}
