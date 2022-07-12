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
using System.Threading.Tasks;

namespace MTConnect.Adapters.Shdr
{
    /// <summary>
    /// A client to connect to MTConnect Adapters using TCP and communicating using the SHDR Protocol
    /// </summary>
    public class ShdrAdapterClient : ShdrClient
    {
        private readonly ShdrAdapterConfiguration _configuration;
        private readonly IMTConnectAgent _agent;
        private IDevice _device;


        public ShdrAdapterClient(ShdrAdapterConfiguration configuration, IMTConnectAgent agent, IDevice device, string idSuffix = null)
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
            ShdrAdapterConfiguration configuration,
            IMTConnectAgent agent,
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


        protected override async Task OnDisconnect()
        {
            if (_device != null)
            {
                // Set DataItems to Unavailable if disconnected from Adapter
                await SetDeviceUnavailable(UnixDateTime.Now);
            }
        }


        protected override async Task<IDataItem> OnGetDataItem(string dataItemKey)
        {
            if (_device != null && !string.IsNullOrEmpty(dataItemKey))
            {
                // Return the DataItem matching the DataItemKey
                return _device.GetDataItemByKey(dataItemKey);
            }

            return null;
        }

        protected override async Task OnDataItemReceived(ShdrDataItem dataItem)
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


                await _agent.AddObservationAsync(_device.Uuid, dataItem, ignoreTimestamp, convertUnits, ignoreCase);
            }
        }

        protected override async Task OnConditionFaultStateReceived(ShdrFaultState faultState)
        {
            if (_device != null)
            {
                // Set Ignore Timestamp Override
                bool? ignoreTimestamp = null;
                if (_configuration.IgnoreTimestamps) ignoreTimestamp = true;

                await _agent.AddObservationAsync(_device.Uuid, faultState, ignoreTimestamp);
            }
        }

        protected override async Task OnMessageReceived(ShdrMessage message)
        {
            if (_device != null)
            {
                // Set Ignore Timestamp Override
                bool? ignoreTimestamp = null;
                if (_configuration.IgnoreTimestamps) ignoreTimestamp = true;

                await _agent.AddObservationAsync(_device.Uuid, message, ignoreTimestamp);
            }
        }

        protected override async Task OnTableReceived(ShdrTable table)
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

                await _agent.AddObservationAsync(_device.Uuid, table, ignoreTimestamp, convertUnits, ignoreCase);
            }
        }

        protected override async Task OnDataSetReceived(ShdrDataSet dataSet)
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

                await _agent.AddObservationAsync(_device.Uuid, dataSet, ignoreTimestamp, convertUnits, ignoreCase);
            }
        }

        protected override async Task OnTimeSeriesReceived(ShdrTimeSeries timeSeries)
        {
            if (_device != null)
            {
                // Set Ignore Timestamp Override
                bool? ignoreTimestamp = null;
                if (_configuration.IgnoreTimestamps) ignoreTimestamp = true;

                // Set Convert Units Override
                bool? convertUnits = null;
                if (_configuration.ConvertUnits) convertUnits = true;

                await _agent.AddObservationAsync(_device.Uuid, timeSeries, ignoreTimestamp, convertUnits);
            }
        }


        protected override async Task OnAssetReceived(IAsset asset)
        {
            if (_device != null)
            {
                // Set Ignore Timestamp Override
                bool? ignoreTimestamp = null;
                if (_configuration.IgnoreTimestamps) ignoreTimestamp = true;

                await _agent.AddAssetAsync(_device.Uuid, asset, ignoreTimestamp);
            }
        }

        protected override async Task OnRemoveAssetReceived(string assetId, long timestamp)
        {
            if (_device != null)
            {
                // Set Ignore Timestamp Override
                bool? ignoreTimestamp = null;
                if (_configuration.IgnoreTimestamps) ignoreTimestamp = true;

                var ts = timestamp;
                if (ignoreTimestamp.HasValue && ignoreTimestamp.Value) ts = 0;

                await _agent.RemoveAssetAsync(assetId, ts);
            }
        }

        protected override async Task OnRemoveAllAssetsReceived(string assetType, long timestamp)
        {
            if (_device != null)
            {
                // Set Ignore Timestamp Override
                bool? ignoreTimestamp = null;
                if (_configuration.IgnoreTimestamps) ignoreTimestamp = true;

                var ts = timestamp;
                if (ignoreTimestamp.HasValue && ignoreTimestamp.Value) ts = 0;

                await _agent.RemoveAllAssetsAsync(assetType, ts);
            }
        }


        protected override async Task OnDeviceReceived(IDevice device)
        {
            if (device != null)
            {
                _device = device;
                await _agent.AddDeviceAsync(device);
            }
        }


        private async Task SetDeviceUnavailable(long timestamp = 0)
        {
            await SetDataItemsUnavailable(timestamp);
            await SetConditionsUnavailable(timestamp);
        }

        private async Task SetDataItemsUnavailable(long timestamp = 0)
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
                            await _agent.AddObservationAsync(_device.Uuid, observation, ignoreTimestamp);
                        }
                    }
                }
            }
        }

        private async Task SetConditionsUnavailable(long timestamp = 0)
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
                            await _agent.AddObservationAsync(_device.Uuid, condition, ignoreTimestamp);
                        }
                    }
                }
            }
        }
    }
}
