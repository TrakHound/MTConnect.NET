// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Agents.Metrics
{
    /// <summary>
    /// Metrics handler for MTConnect Observations (Sample, Events, Conditions)
    /// </summary>
    public class ObservationMetric
    {
        private readonly object _lock = new object();
        private int _lastCount = 0;
        private int _lastDelta = 0;
        private double _lastAverage = 0;


        /// <summary>
        /// The ID of the DataItem these metrics track.
        /// </summary>
        public string DataItemId { get; set; }

        /// <summary>
        /// The running total number of observations recorded for the DataItem.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// The Unix timestamp of the most recent observation update.
        /// </summary>
        public long LastUpdated { get; set; }

        /// <summary>
        /// The number of observations recorded since the last rate-update window.
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
        /// The exponentially smoothed observation update rate over the configured window.
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
        public ObservationMetric() { }

        /// <summary>
        /// Initializes a metric bound to the given DataItem.
        /// </summary>
        /// <param name="dataItemId">The ID of the DataItem to track.</param>
        public ObservationMetric(string dataItemId)
        {
            DataItemId = dataItemId;
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