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


        public string AssetId { get; set; }

        public int Count { get; set; }

        public long LastUpdated { get; set; }

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


        public AssetMetric() { }

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