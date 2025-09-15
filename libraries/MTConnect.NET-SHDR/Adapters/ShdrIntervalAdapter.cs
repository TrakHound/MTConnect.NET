// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
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


        public ShdrIntervalAdapter(
            int port = 7878, 
            int heartbeat = 10000, 
            int interval = _defaultInterval,
            ILogger logger = null) 
            : base(
                port, 
                heartbeat, 
                interval,
                logger: logger) { }

        public ShdrIntervalAdapter(
            string deviceKey, 
            int port = 7878, 
            int heartbeat = 10000, 
            int interval = _defaultInterval,
            ILogger logger = null) 
            : base(
                deviceKey, 
                port, 
                heartbeat, 
                interval,
                logger: logger) { }

        public ShdrIntervalAdapter(
            ShdrAdapterClientConfiguration configuration, 
            int interval = _defaultInterval,
            ILogger logger = null) 
            : base(
                configuration, 
                interval,
                logger: logger) { }
    }
}