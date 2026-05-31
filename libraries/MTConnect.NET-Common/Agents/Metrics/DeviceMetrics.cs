// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Agents.Metrics
{
    /// <summary>
    /// Metrics handler for MTConnect Devices
    /// </summary>
    public class DeviceMetrics
    {
        private readonly object _lock = new object();
        private readonly Dictionary<string, ObservationMetric> _observations = new Dictionary<string, ObservationMetric>();
        private readonly Dictionary<string, AssetMetric> _assets = new Dictionary<string, AssetMetric>();

        private int _lastObservationCount = 0;
        private int _lastObservationDelta = 0;
        private double _lastObservationAverage = 0;

        private int _lastAssetCount = 0;
        private int _lastAssetDelta = 0;
        private double _lastAssetAverage = 0;


        /// <summary>
        /// The UUID of the Device these metrics aggregate.
        /// </summary>
        public string DeviceUuid { get; set; }

        /// <summary>
        /// The number of observations recorded across the device since the last rate-update window.
        /// </summary>
        public int ObservationDelta
        {
            get
            {
                lock (_lock)
                {
                    return _lastObservationDelta;
                }
            }
        }

        /// <summary>
        /// The exponentially smoothed observation update rate across the device.
        /// </summary>
        public double ObservationAverage
        {
            get
            {
                lock (_lock)
                {
                    return _lastObservationAverage;
                }
            }
        }

        /// <summary>
        /// The number of asset updates recorded across the device since the last rate-update window.
        /// </summary>
        public int AssetDelta
        {
            get
            {
                lock (_lock)
                {
                    return _lastAssetDelta;
                }
            }
        }

        /// <summary>
        /// The exponentially smoothed asset update rate across the device.
        /// </summary>
        public double AssetAverage
        {
            get
            {
                lock (_lock)
                {
                    return _lastAssetAverage;
                }
            }
        }


        /// <summary>
        /// Initializes an empty device metrics aggregate, typically populated later.
        /// </summary>
        public DeviceMetrics() { }

        /// <summary>
        /// Initializes a device metrics aggregate bound to the given Device.
        /// </summary>
        /// <param name="deviceUuid">The UUID of the Device to track.</param>
        public DeviceMetrics(string deviceUuid)
        {
            DeviceUuid = deviceUuid;
        }


        internal void UpdateRate(double updateInterval, double windowInterval)
        {
            var observationCount = GetObservationCount();
            var assetCount = GetAssetCount();

            lock (_lock)
            {
                // Calculate Observation Average
                var observationDelta = observationCount - _lastObservationCount;
                _lastObservationAverage = observationDelta + Math.Exp(-(updateInterval / windowInterval)) * (_lastObservationAverage - observationDelta);
                _lastObservationDelta = observationDelta;
                _lastObservationCount = observationCount;

                // Calculate Asset Average
                var assetDelta = assetCount - _lastAssetCount;
                _lastAssetAverage = assetDelta + Math.Exp(-(updateInterval / windowInterval)) * (_lastAssetAverage - assetDelta);
                _lastAssetDelta = assetDelta;
                _lastAssetCount = assetCount;
            }

            // Observations
            var observations = GetObservationMetrics();
            if (!observations.IsNullOrEmpty())
            {
                foreach (var observation in observations)
                {
                    observation.UpdateRate(updateInterval, windowInterval);
                }
            }

            // Assets
            var assets = GetAssetMetrics();
            if (!assets.IsNullOrEmpty())
            {
                foreach (var asset in assets)
                {
                    asset.UpdateRate(updateInterval, windowInterval);
                }
            }
        }


        /// <summary>
        /// Returns the total observation count summed across every tracked DataItem.
        /// </summary>
        public int GetObservationCount()
        {
            int count = 0;

            var metrics = GetObservationMetrics();
            if (!metrics.IsNullOrEmpty())
            {
                foreach (var metric in metrics)
                {
                    count += metric.Count;
                }
            }

            return count;
        }

        /// <summary>
        /// Returns the total asset-update count summed across every tracked Asset.
        /// </summary>
        public int GetAssetCount()
        {
            int count = 0;

            var metrics = GetAssetMetrics();
            if (!metrics.IsNullOrEmpty())
            {
                foreach (var metric in metrics)
                {
                    count += metric.Count;
                }
            }

            return count;
        }


        /// <summary>
        /// Returns a snapshot of the per-DataItem observation metrics.
        /// </summary>
        public IEnumerable<ObservationMetric> GetObservationMetrics()
        {
            lock (_lock)
            {
                return _observations.Values.ToList();
            }
        }

        /// <summary>
        /// Returns the observation metric for the given DataItem, or null when none is tracked.
        /// </summary>
        /// <param name="dataItemId">The DataItem ID to look up.</param>
        public ObservationMetric GetObservationMetric(string dataItemId)
        {
            if (!string.IsNullOrEmpty(dataItemId))
            {
                lock (_lock)
                {
                    if (_observations.TryGetValue(dataItemId, out var metric))
                    {
                        return metric;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Returns a snapshot of the per-Asset update metrics.
        /// </summary>
        public IEnumerable<AssetMetric> GetAssetMetrics()
        {
            lock (_lock)
            {
                return _assets.Values.ToList();
            }
        }

        /// <summary>
        /// Returns the asset metric for the given Asset, or null when none is tracked.
        /// </summary>
        /// <param name="assetId">The Asset ID to look up.</param>
        public AssetMetric GetAssetMetric(string assetId)
        {
            if (!string.IsNullOrEmpty(assetId))
            {
                lock (_lock)
                {
                    if (_assets.TryGetValue(assetId, out var metric))
                    {
                        return metric;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Records an observation for the given DataItem, creating its metric on first sighting and bumping its count and last-updated timestamp.
        /// </summary>
        /// <param name="dataItemId">The DataItem ID that produced an observation.</param>
        public void UpdateObservation(string dataItemId)
        {
            if (!string.IsNullOrEmpty(dataItemId))
            {
                lock (_lock)
                {
                    if (_observations.TryGetValue(dataItemId, out var metric))
                    {
                        // Increment Count
                        metric.Count++;

                        // Set Last Updated Timestamp
                        metric.LastUpdated = UnixDateTime.Now;
                    }
                    else
                    {
                        metric = new ObservationMetric(dataItemId);

                        // Initialize Count
                        metric.Count = 1;

                        // Set Last Updated Timestamp
                        metric.LastUpdated = UnixDateTime.Now;

                        _observations.Add(dataItemId, metric);
                    }
                }
            }
        }

        /// <summary>
        /// Records an update for the given Asset, creating its metric on first sighting and bumping its count and last-updated timestamp.
        /// </summary>
        /// <param name="assetId">The Asset ID that was updated.</param>
        public void UpdateAsset(string assetId)
        {
            if (!string.IsNullOrEmpty(assetId))
            {
                lock (_lock)
                {
                    if (_assets.TryGetValue(assetId, out var metric))
                    {
                        // Increment Count
                        metric.Count++;

                        // Set Last Updated Timestamp
                        metric.LastUpdated = UnixDateTime.Now;
                    }
                    else
                    {
                        metric = new AssetMetric(assetId);

                        // Initialize Count
                        metric.Count = 1;

                        // Set Last Updated Timestamp
                        metric.LastUpdated = UnixDateTime.Now;

                        _assets.Add(assetId, metric);
                    }
                }
            }
        }
    }
}