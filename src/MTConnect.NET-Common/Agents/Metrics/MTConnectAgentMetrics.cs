// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Agents.Metrics
{
    /// <summary>
    /// Metrics handler for MTConnectAgent Observations and Assets
    /// </summary>
    public class MTConnectAgentMetrics : IDisposable
    {
        private readonly object _lock = new object();
        private readonly Dictionary<string, DeviceMetrics> _devices = new Dictionary<string, DeviceMetrics>();
        private readonly TimeSpan _updateInterval;
        private readonly TimeSpan _windowInterval;
        private System.Timers.Timer _updateTimer;

        private int _lastObservationCount = 0;
        private int _lastObservationDelta = 0;
        private double _lastObservationAverage = 0;

        private int _lastAssetCount = 0;
        private int _lastAssetDelta = 0;
        private double _lastAssetAverage = 0;


        public TimeSpan UpdateInterval => _updateInterval;

        public TimeSpan WindowInterval => _windowInterval;

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

        public EventHandler<DeviceMetrics> DeviceMetricsUpdated { get; set; }


        public MTConnectAgentMetrics(TimeSpan updateInterval, TimeSpan windowInterval)
        {
            _updateTimer = new System.Timers.Timer();
            _updateTimer.Interval = updateInterval.TotalMilliseconds;
            _updateTimer.Elapsed += (s, e) =>
            {
                UpdateRate(updateInterval.TotalSeconds, windowInterval.TotalSeconds);
            };
            _updateTimer.Start();

            _updateInterval = updateInterval;
            _windowInterval = windowInterval;
        }


        public void Dispose()
        {
            if (_updateTimer != null) _updateTimer.Dispose();
        }



        private void UpdateRate(double updateInterval, double windowInterval)
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

            // Update Device Metrics
            var deviceMetrics = GetDeviceMetrics();
            if (!deviceMetrics.IsNullOrEmpty())
            {
                foreach (var deviceMetric in deviceMetrics)
                {
                    deviceMetric.UpdateRate(updateInterval, windowInterval);

                    if (DeviceMetricsUpdated != null) DeviceMetricsUpdated.Invoke(this, deviceMetric);
                }
            }
        }


        public IEnumerable<DeviceMetrics> GetDeviceMetrics()
        {
            lock (_lock)
            {
                return _devices.Values.ToList();
            }
        }

        public DeviceMetrics GetDeviceMetric(string deviceName)
        {
            if (!string.IsNullOrEmpty(deviceName))
            {
                lock (_lock)
                {
                    if (_devices.TryGetValue(deviceName, out var deviceStatistics))
                    {
                        return deviceStatistics;
                    }
                }
            }

            return null;
        }


        public int GetObservationCount()
        {
            int count = 0;

            var metrics = GetDeviceMetrics();
            if (!metrics.IsNullOrEmpty())
            {
                foreach (var metric in metrics)
                {
                    count += metric.GetObservationCount();
                }
            }

            return count;
        }

        public int GetAssetCount()
        {
            int count = 0;

            var metrics = GetDeviceMetrics();
            if (!metrics.IsNullOrEmpty())
            {
                foreach (var metric in metrics)
                {
                    count += metric.GetAssetCount();
                }
            }

            return count;
        }


        public void UpdateObservation(string deviceName, string dataItemId)
        {
            if (!string.IsNullOrEmpty(deviceName) && !string.IsNullOrEmpty(dataItemId))
            {
                lock (_lock)
                {
                    _devices.TryGetValue(deviceName, out var deviceStatistics);

                    if (deviceStatistics == null)
                    {
                        deviceStatistics = new DeviceMetrics(deviceName);
                        _devices.Add(deviceName, deviceStatistics);
                    }

                    deviceStatistics.UpdateObservation(dataItemId);
                }
            }
        }

        public void UpdateAsset(string deviceName, string assetId)
        {
            if (!string.IsNullOrEmpty(deviceName) && !string.IsNullOrEmpty(assetId))
            {
                lock (_lock)
                {
                    _devices.TryGetValue(deviceName, out var deviceStatistics);

                    if (deviceStatistics == null)
                    {
                        deviceStatistics = new DeviceMetrics(deviceName);
                        _devices.Add(deviceName, deviceStatistics);
                    }

                    deviceStatistics.UpdateAsset(assetId);
                }
            }
        }
    }
}
