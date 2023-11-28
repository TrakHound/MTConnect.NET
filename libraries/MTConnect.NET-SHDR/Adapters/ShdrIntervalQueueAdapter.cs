// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
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


        public ShdrIntervalQueueAdapter(int port = 7878, int heartbeat = 10000, int interval = _defaultInterval) : base(port, heartbeat, interval, true) { }

        public ShdrIntervalQueueAdapter(string deviceKey, int port = 7878, int heartbeat = 10000, int interval = _defaultInterval) : base(deviceKey, port, heartbeat, interval, true) { }

        public ShdrIntervalQueueAdapter(ShdrAdapterClientConfiguration configuration, int interval = _defaultInterval) : base(configuration, interval, true) { }
    }
}