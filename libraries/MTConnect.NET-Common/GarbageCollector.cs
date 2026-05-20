// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect
{
    /// <summary>
    /// Background coordinator that debounces explicit GC requests: callers signal a low- or high-priority need to collect, and a worker performs full collections no more often than the configured intervals, then tapers off with an exponentially backing-off "run" series so memory is reclaimed without thrashing.
    /// </summary>
    public class GarbageCollector : IDisposable
    {
        private const int _interval = 250;
        private const int _runInitialInterval = 500 * 10000;
        private const double _runMultiple = 2;
        private const int _maxRunCount = 4;

        private static GarbageCollector _instance;

        private readonly int _lowPriorityInterval;
        private readonly int _highPriorityInterval;
        private readonly CancellationTokenSource _stop;
        private readonly object _lock = new object();

        private int _runCount = 0;
        private bool _runCollect = false;
        private long _lastRunCollected = 0;
        private double _runInterval = _runInitialInterval;

        private long _lastCollected = 0;
        private bool _lowPriorityCollect = false;
        private bool _highPriorityCollect = false;


        /// <summary>
        /// Starts the background worker that performs debounced collections, with separate minimum spacings (in milliseconds) for low- and high-priority requests.
        /// </summary>
        /// <param name="lowPriorityInterval">Minimum milliseconds between collections triggered by low-priority requests.</param>
        /// <param name="highPriorityInterval">Minimum milliseconds between collections triggered by high-priority requests.</param>
        public GarbageCollector(int lowPriorityInterval = 10000, int highPriorityInterval = 500)
        {
            _lowPriorityInterval = lowPriorityInterval * 10000; // Convert ms to ticks
            _highPriorityInterval = highPriorityInterval * 10000; // Convert ms to ticks
            _stop = new CancellationTokenSource();
            _ = Task.Run(Worker);
        }

        /// <summary>
        /// Signals the background worker to stop.
        /// </summary>
        public void Dispose()
        {
            if (_stop != null) _stop.Cancel();
        }


        private async void Worker()
        {
            long now = UnixDateTime.Now;
            _lastCollected = now;

            do
            {
                now = UnixDateTime.Now;

                bool lowCollect = false;
                bool highCollect = false;
                bool runCollect = false;

                lock (_lock)
                {
                    highCollect = _highPriorityCollect && now - _lastCollected > _highPriorityInterval;
                    if (!highCollect) lowCollect = _lowPriorityCollect && now - _lastCollected > _lowPriorityInterval;
                    if (!highCollect && !lowCollect && _runCollect)
                    {
                        runCollect = now - _lastRunCollected > _runInterval;
                    }
                }

                if (lowCollect || highCollect || runCollect)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();


                    lock (_lock)
                    {
                        if (lowCollect || highCollect)
                        {
                            _lastCollected = now;
                            _highPriorityCollect = false;
                            _lowPriorityCollect = false;
                        }

                        if (runCollect) _lastRunCollected = now;

                        _runCount++;
                        _runInterval = _runInterval * _runMultiple;

                        if (highCollect)
                        {
                            _runCollect = true;
                            _runCount = 0;
                            _runInterval = _runInitialInterval;
                        }
                        else if (lowCollect)
                        {
                            _runCount = 0;
                            _runInterval = _runInitialInterval;
                        }
                        else if (_runCount >= _maxRunCount)
                        {
                            _runCollect = false;
                            _runCount = 0;
                        }
                    }
                }

                await Task.Delay(_interval);
            }
            while (!_stop.IsCancellationRequested);
        }


        /// <summary>
        /// Lazily creates the shared singleton collector with the given intervals; subsequent calls have no effect.
        /// </summary>
        /// <param name="lowPriorityInterval">Minimum milliseconds between low-priority collections.</param>
        /// <param name="highPriorityInterval">Minimum milliseconds between high-priority collections.</param>
        public static void Initialize(int lowPriorityInterval = 30000, int highPriorityInterval = 500)
        {
            if (_instance == null) _instance = new GarbageCollector(lowPriorityInterval, highPriorityInterval);
        }

        /// <summary>
        /// Stops the shared singleton collector if it has been created.
        /// </summary>
        public static void DisposeInstance()
        {
            if (_instance != null) _instance.Dispose();
        }

        /// <summary>
        /// Requests a low-priority collection on the shared collector; the actual collection is deferred until the low-priority interval has elapsed.
        /// </summary>
        public static void LowPriorityCollect()
        {
            if (_instance != null)
            {
                lock (_instance._lock)
                {
                    _instance._lowPriorityCollect = true;
                }
            }
        }

        /// <summary>
        /// Requests a high-priority collection on the shared collector; collections are deferred only by the shorter high-priority interval and take precedence over low-priority requests.
        /// </summary>
        public static void HighPriorityCollect()
        {
            if (_instance != null)
            {
                lock (_instance._lock)
                {
                    _instance._highPriorityCollect = true;
                }
            }
        }
    }
}