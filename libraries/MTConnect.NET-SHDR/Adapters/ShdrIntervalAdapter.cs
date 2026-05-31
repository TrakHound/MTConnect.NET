// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;

namespace MTConnect.Adapters
{
    /// <summary>
    /// An Adapter class for communicating with an MTConnect Agent using the SHDR protocol.
    /// Supports multiple concurrent Agent connections.
    /// Sends the most recent changes at the specified interval.
    /// </summary>
    public class ShdrIntervalAdapter : ShdrAdapter
    {
        private const int _defaultInterval = 100;


        /// <summary>Creates an interval-mode adapter without a device key, on the supplied <paramref name="port"/>, with the supplied <paramref name="heartbeat"/>, and the supplied flush <paramref name="interval"/> (defaults to 100 ms).</summary>
        public ShdrIntervalAdapter(int port = 7878, int heartbeat = 10000, int interval = _defaultInterval) : base(port, heartbeat, interval) { }

        /// <summary>Creates an interval-mode adapter scoped to <paramref name="deviceKey"/>, on the supplied <paramref name="port"/>, with the supplied <paramref name="heartbeat"/>, and the supplied flush <paramref name="interval"/>.</summary>
        public ShdrIntervalAdapter(string deviceKey, int port = 7878, int heartbeat = 10000, int interval = _defaultInterval) : base(deviceKey, port, heartbeat, interval) { }

        /// <summary>Creates an interval-mode adapter from an <see cref="ShdrAdapterClientConfiguration"/> with the supplied flush <paramref name="interval"/>.</summary>
        public ShdrIntervalAdapter(ShdrAdapterClientConfiguration configuration, int interval = _defaultInterval) : base(configuration, interval) { }
    }
}