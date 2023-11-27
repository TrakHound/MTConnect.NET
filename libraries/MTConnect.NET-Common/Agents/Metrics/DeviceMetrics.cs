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


        public string DeviceUuid { get; set; }

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


        public DeviceMetrics() { }

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


        public IEnumerable<ObservationMetric> GetObservationMetrics()
        {
            lock (_lock)
            {
                return _observations.Values.ToList();
            }
        }

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


        public IEnumerable<AssetMetric> GetAssetMetrics()
        {
            lock (_lock)
            {
                return _assets.Values.ToList();
            }
        }

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