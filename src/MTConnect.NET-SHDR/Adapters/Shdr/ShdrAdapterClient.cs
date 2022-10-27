// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using MTConnect.Observations.Input;
using MTConnect.Shdr;
using System.Linq;

namespace MTConnect.Adapters.Shdr
{
    /// <summary>
    /// A client to connect to MTConnect Adapters using TCP and communicating using the SHDR Protocol
    /// </summary>
    public class ShdrAdapterClient : ShdrClient
    {
        private readonly IShdrAdapterConfiguration _configuration;
        private readonly IMTConnectAgentBroker _agent;
        private IDevice _device;


        public ShdrAdapterClient(
            IShdrAdapterConfiguration configuration,
            IMTConnectAgentBroker agent,
            IDevice device,
            string idSuffix = null
            )
        {
            _configuration = configuration;
            Id = configuration != null ? configuration.Id : StringFunctions.RandomString(10);
            if (!string.IsNullOrEmpty(idSuffix)) Id = Id != null ? $"{Id}_{idSuffix}" : idSuffix;
            _agent = agent;
            _device = device;

            if (_configuration != null)
            {
                Hostname = _configuration.Hostname;
                Port = _configuration.Port;
                ReconnectInterval = _configuration.ReconnectInterval;
            }
        }

        public ShdrAdapterClient(
            IShdrAdapterConfiguration configuration,
            IMTConnectAgentBroker agent,
            IDevice device,
            string hostname,
            int port
            )
        {
            _configuration = configuration;
            _agent = agent;
            _device = device;
            Hostname = hostname;
            Port = port;

            if (_configuration != null)
            {
                ReconnectInterval = _configuration.ReconnectInterval;
            }
        }


        protected override void OnDisconnect()
        {
            if (_device != null)
            {
                // Set DataItems to Unavailable if disconnected from Adapter
                SetDeviceUnavailable(UnixDateTime.Now);
            }
        }


        protected override IDataItem OnGetDataItem(string dataItemKey)
        {
            if (_device != null && !string.IsNullOrEmpty(dataItemKey))
            {
                // Return the DataItem matching the DataItemKey
                return _agent.GetDataItem(_device.Uuid, dataItemKey);
            }

            return null;
        }


        protected override void OnDataItemReceived(ShdrDataItem dataItem)
        {
            if (_device != null && _configuration != null)
            {
                // Set Ignore Timestamp Override
                bool? ignoreTimestamp = null;
                if (_configuration.IgnoreTimestamps) ignoreTimestamp = true;

                // Set Convert Units Override
                bool? convertUnits = null;
                if (_configuration.ConvertUnits) convertUnits = true;

                // Set Ignore Case Override
                bool? ignoreCase = null;
                if (_configuration.IgnoreObservationCase) ignoreCase = true;

                _agent.AddObservation(_device.Uuid, dataItem, ignoreTimestamp, convertUnits, ignoreCase);
            }
        }

        protected override void OnConditionFaultStateReceived(ShdrFaultState faultState)
        {
            if (_device != null)
            {
                // Set Ignore Timestamp Override
                bool? ignoreTimestamp = null;
                if (_configuration.IgnoreTimestamps) ignoreTimestamp = true;

                _agent.AddObservation(_device.Uuid, faultState, ignoreTimestamp);
            }
        }

        protected override void OnMessageReceived(ShdrMessage message)
        {
            if (_device != null)
            {
                // Set Ignore Timestamp Override
                bool? ignoreTimestamp = null;
                if (_configuration.IgnoreTimestamps) ignoreTimestamp = true;

                _agent.AddObservation(_device.Uuid, message, ignoreTimestamp);
            }
        }

        protected override void OnTableReceived(ShdrTable table)
        {
            if (_device != null)
            {
                // Set Ignore Timestamp Override
                bool? ignoreTimestamp = null;
                if (_configuration.IgnoreTimestamps) ignoreTimestamp = true;

                // Set Convert Units Override
                bool? convertUnits = null;
                if (_configuration.ConvertUnits) convertUnits = true;

                // Set Ignore Case Override
                bool? ignoreCase = null;
                if (_configuration.IgnoreObservationCase) ignoreCase = true;

                _agent.AddObservation(_device.Uuid, table, ignoreTimestamp, convertUnits, ignoreCase);
            }
        }

        protected override void OnDataSetReceived(ShdrDataSet dataSet)
        {
            if (_device != null)
            {
                // Set Ignore Timestamp Override
                bool? ignoreTimestamp = null;
                if (_configuration.IgnoreTimestamps) ignoreTimestamp = true;

                // Set Convert Units Override
                bool? convertUnits = null;
                if (_configuration.ConvertUnits) convertUnits = true;

                // Set Ignore Case Override
                bool? ignoreCase = null;
                if (_configuration.IgnoreObservationCase) ignoreCase = true;

                _agent.AddObservation(_device.Uuid, dataSet, ignoreTimestamp, convertUnits, ignoreCase);
            }
        }

        protected override void OnTimeSeriesReceived(ShdrTimeSeries timeSeries)
        {
            if (_device != null)
            {
                // Set Ignore Timestamp Override
                bool? ignoreTimestamp = null;
                if (_configuration.IgnoreTimestamps) ignoreTimestamp = true;

                // Set Convert Units Override
                bool? convertUnits = null;
                if (_configuration.ConvertUnits) convertUnits = true;

                _agent.AddObservation(_device.Uuid, timeSeries, ignoreTimestamp, convertUnits);
            }
        }


        protected override void OnAssetReceived(IAsset asset)
        {
            if (_device != null)
            {
                // Set Ignore Timestamp Override
                bool? ignoreTimestamp = null;
                if (_configuration.IgnoreTimestamps) ignoreTimestamp = true;

                _agent.AddAsset(_device.Uuid, asset, ignoreTimestamp);
            }
        }

        protected override void OnRemoveAssetReceived(string assetId, long timestamp)
        {
            if (_device != null)
            {
                // Set Ignore Timestamp Override
                bool? ignoreTimestamp = null;
                if (_configuration.IgnoreTimestamps) ignoreTimestamp = true;

                var ts = timestamp;
                if (ignoreTimestamp.HasValue && ignoreTimestamp.Value) ts = 0;

                _agent.RemoveAsset(assetId, ts);
            }
        }

        protected override void OnRemoveAllAssetsReceived(string assetType, long timestamp)
        {
            if (_device != null)
            {
                // Set Ignore Timestamp Override
                bool? ignoreTimestamp = null;
                if (_configuration.IgnoreTimestamps) ignoreTimestamp = true;

                var ts = timestamp;
                if (ignoreTimestamp.HasValue && ignoreTimestamp.Value) ts = 0;

                _agent.RemoveAllAssets(assetType, ts);
            }
        }


        protected override void OnDeviceReceived(IDevice device)
        {
            if (device != null)
            {
                _device = device;
                _agent.AddDevice(device);
            }
        }


        private void SetDeviceUnavailable(long timestamp = 0)
        {
            SetDataItemsUnavailable(timestamp);
            SetConditionsUnavailable(timestamp);
        }

        private void SetDataItemsUnavailable(long timestamp = 0)
        {
            if (_agent != null && _device != null)
            {
                // Set Ignore Timestamp Override
                bool? ignoreTimestamp = null;
                if (_configuration.IgnoreTimestamps) ignoreTimestamp = true;

                var dataItems = _device.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    dataItems = dataItems.Where(o => o.Category != DataItemCategory.CONDITION);
                    if (!dataItems.IsNullOrEmpty())
                    {
                        foreach (var dataItem in dataItems)
                        {
                            var observation = new ObservationInput(dataItem.Id, Observation.Unavailable, timestamp);
                            _agent.AddObservation(_device.Uuid, observation, ignoreTimestamp);
                        }
                    }
                }
            }
        }

        private void SetConditionsUnavailable(long timestamp = 0)
        {
            if (_agent != null && _device != null)
            {
                // Set Ignore Timestamp Override
                bool? ignoreTimestamp = null;
                if (_configuration.IgnoreTimestamps) ignoreTimestamp = true;

                var dataItems = _device.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    dataItems = dataItems.Where(o => o.Category == DataItemCategory.CONDITION);
                    if (!dataItems.IsNullOrEmpty())
                    {
                        foreach (var dataItem in dataItems)
                        {
                            var condition = new ConditionObservationInput(dataItem.Id, ConditionLevel.UNAVAILABLE, timestamp);
                            _agent.AddObservation(_device.Uuid, condition, ignoreTimestamp);
                        }
                    }
                }
            }
        }
    }
}
