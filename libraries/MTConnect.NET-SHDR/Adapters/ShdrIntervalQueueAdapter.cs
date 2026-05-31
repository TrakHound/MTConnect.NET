// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;

namespace MTConnect.Adapters
{
    /// <summary>
    /// An Adapter class for communicating with an MTConnect Agent using the SHDR protocol.
    /// Supports multiple concurrent Agent connections.
    /// Uses a queue to collect changes to Observations and sends the entire buffer at the specified interval.
    /// </summary>
    public class ShdrIntervalQueueAdapter : ShdrAdapter
    {
        private const int _defaultInterval = 100;


        /// <summary>Creates an interval-queue adapter without a device key, on the supplied <paramref name="port"/>, with the supplied <paramref name="heartbeat"/> and flush <paramref name="interval"/> (defaults to 100 ms).</summary>
        public ShdrIntervalQueueAdapter(int port = 7878, int heartbeat = 10000, int interval = _defaultInterval) : base(port, heartbeat, interval, true) { }

        /// <summary>Creates an interval-queue adapter scoped to <paramref name="deviceKey"/>, on the supplied <paramref name="port"/>, with the supplied <paramref name="heartbeat"/> and flush <paramref name="interval"/>.</summary>
        public ShdrIntervalQueueAdapter(string deviceKey, int port = 7878, int heartbeat = 10000, int interval = _defaultInterval) : base(deviceKey, port, heartbeat, interval, true) { }

        /// <summary>Creates an interval-queue adapter from an <see cref="ShdrAdapterClientConfiguration"/> with the supplied flush <paramref name="interval"/>.</summary>
        public ShdrIntervalQueueAdapter(ShdrAdapterClientConfiguration configuration, int interval = _defaultInterval) : base(configuration, interval, true) { }


        /// <summary>Forces an immediate flush of the buffered SHDR lines to every connected agent, bypassing the interval timer; returns <c>true</c> when the send succeeded.</summary>
        public bool SendBuffer()
        {
            return Adapter.SendBuffer();
        }
    }
}