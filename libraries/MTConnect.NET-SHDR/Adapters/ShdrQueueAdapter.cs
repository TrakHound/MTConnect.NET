// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
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
        /// <summary>Creates a queue-mode adapter without a device key, on the supplied <paramref name="port"/>, with the supplied <paramref name="heartbeat"/> interval in milliseconds.</summary>
        public ShdrQueueAdapter(int port = 7878, int heartbeat = 10000) : base(port, heartbeat, null, true) { }

        /// <summary>Creates a queue-mode adapter scoped to <paramref name="deviceKey"/>, on the supplied <paramref name="port"/>, with the supplied <paramref name="heartbeat"/>.</summary>
        public ShdrQueueAdapter(string deviceKey, int port = 7878, int heartbeat = 10000) : base(deviceKey, port, heartbeat, null, true) { }

        /// <summary>Creates a queue-mode adapter from an <see cref="ShdrAdapterClientConfiguration"/>.</summary>
        public ShdrQueueAdapter(ShdrAdapterClientConfiguration configuration) : base(configuration, null, true) { }


        /// <summary>Flushes the buffered SHDR lines to every connected agent in a single batch; returns <c>true</c> when the send succeeded.</summary>
        public bool SendBuffer()
        {
            return Adapter.SendBuffer();
        }
    }
}