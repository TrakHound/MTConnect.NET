// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect
{
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


        public GarbageCollector(int lowPriorityInterval = 10000, int highPriorityInterval = 500)
        {
            _lowPriorityInterval = lowPriorityInterval * 10000; // Convert ms to ticks
            _highPriorityInterval = highPriorityInterval * 10000; // Convert ms to ticks
            _stop = new CancellationTokenSource();
            _ = Task.Run(Worker);
        }

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

                    var currentProc = Process.GetCurrentProcess();
                    var bytesInUse = currentProc.PrivateMemorySize64;

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


        public static void Initialize(int lowPriorityInterval = 30000, int highPriorityInterval = 500)
        {
            if (_instance == null) _instance = new GarbageCollector(lowPriorityInterval, highPriorityInterval);
        }

        public static void DisposeInstance()
        {
            if (_instance != null) _instance.Dispose();
        }

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
