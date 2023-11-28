// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;

namespace MTConnect.Adapters
{
    /// <summary>
    /// An Adapter class for communicating with an MTConnect Agent using the SHDR protocol.
    /// Supports multiple concurrent Agent connections.
    /// Uses a queue to collect changes to Observations and sends all of the buffered items on demand
    /// </summary>
    public class ShdrQueueAdapter : ShdrAdapter
    {
        public ShdrQueueAdapter(int port = 7878, int heartbeat = 10000) : base(port, heartbeat, null, true) { }

        public ShdrQueueAdapter(string deviceKey, int port = 7878, int heartbeat = 10000) : base(deviceKey, port, heartbeat, null, true) { }

        public ShdrQueueAdapter(ShdrAdapterClientConfiguration configuration) : base(configuration, null, true) { }
    }
}