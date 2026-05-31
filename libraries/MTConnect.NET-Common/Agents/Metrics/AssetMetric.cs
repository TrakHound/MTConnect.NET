// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Agents.Metrics
{
    /// <summary>
    /// Metrics handler for MTConnect Assets
    /// </summary>
    public class AssetMetric
    {
        private readonly object _lock = new object();
        private int _lastCount = 0;
        private int _lastDelta = 0;
        private double _lastAverage = 0;


        /// <summary>
        /// The ID of the Asset these metrics track.
        /// </summary>
        public string AssetId { get; set; }

        /// <summary>
        /// The running total number of asset updates recorded.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// The Unix timestamp of the most recent asset update.
        /// </summary>
        public long LastUpdated { get; set; }

        /// <summary>
        /// The number of asset updates recorded since the last rate-update window.
        /// </summary>
        public int Delta
        {
            get
            {
                lock (_lock)
                {
                    return _lastDelta;
                }
            }
        }

        /// <summary>
        /// The exponentially smoothed asset update rate over the configured window.
        /// </summary>
        public double Average
        {
            get
            {
                lock (_lock)
                {
                    return _lastAverage;
                }
            }
        }


        /// <summary>
        /// Initializes an empty metric, typically populated later.
        /// </summary>
        public AssetMetric() { }

        /// <summary>
        /// Initializes a metric bound to the given Asset.
        /// </summary>
        /// <param name="assetId">The ID of the Asset to track.</param>
        public AssetMetric(string assetId)
        {
            AssetId = assetId;
        }


        internal void UpdateRate(double updateInterval, double windowInterval)
        {
            lock (_lock)
            {
                var delta = Count - _lastCount;
                _lastAverage = delta + Math.Exp(-(updateInterval / windowInterval)) * (_lastAverage - delta);
                _lastDelta = delta;
                _lastCount = Count;
            }
        }
    }
}