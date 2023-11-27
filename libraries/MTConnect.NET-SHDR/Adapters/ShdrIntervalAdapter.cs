// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Adapters
{
    /// <summary>
    /// An Adapter class for communicating with an MTConnect Agent using the SHDR protocol.
    /// Supports multiple concurrent Agent connections.
    /// Sends the most recent changes at the specified interval.
    /// </summary>
    public class ShdrIntervalAdapter : ShdrAdapter
    {
        private const int _defualtInterval = 100;


        /// <summary>
        /// The interval (in milliseconds) at which new data is sent to the Agent
        /// </summary>
        public int Interval { get; set; }


        public ShdrIntervalAdapter(int port = 7878, int heartbeat = 10000, int interval = _defualtInterval) : base(port, heartbeat)
        {
            Interval = interval;
        }

        public ShdrIntervalAdapter(string deviceKey, int port = 7878, int heartbeat = 10000, int interval = _defualtInterval) : base(deviceKey, port, heartbeat)
        {
            Interval = interval;
        }

        public ShdrIntervalAdapter(ShdrAdapterClientConfiguration configuration, int interval = _defualtInterval) : base(configuration) 
        {
            Interval = interval;
        }


        protected override void OnStart()
        {
            // Start Write Queue
            _ = Task.Run(() => Worker(StopToken.Token));
        }


        private async Task Worker(CancellationToken cancellationToken)
        {
            try
            {
                do
                {
                    int interval = Math.Max(1, Interval); // Set Minimum of 1ms to prevent high CPU usage

                    var stpw = System.Diagnostics.Stopwatch.StartNew();

                    // Call Overridable Method
                    OnIntervalElapsed();

                    stpw.Stop();

                    if (stpw.ElapsedMilliseconds < interval)
                    {
                        var waitInterval = interval - (int)stpw.ElapsedMilliseconds;

                        await Task.Delay(waitInterval, cancellationToken);
                    }

                } while (!cancellationToken.IsCancellationRequested);
            }
            catch (TaskCanceledException) { }
            catch (Exception) { }
        }

        protected virtual void OnIntervalElapsed()
        {
            // Send only the changed items
            SendChanged();
        }
    }
}