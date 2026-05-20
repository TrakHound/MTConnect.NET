// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Input;
using MTConnect.Observations;
using MTConnect.Shdr;
using System.Linq;

namespace MTConnect.Adapters
{
    /// <summary>
    /// A client to connect to MTConnect Adapters using TCP and communicating using the SHDR Protocol
    /// </summary>
    public class ShdrAdapterClient : ShdrClient
    {
        private readonly IShdrAdapterClientConfiguration _configuration;
        private readonly IMTConnectAgent _agent;
        private IDevice _device;


        /// <summary>The device the SHDR client feeds observations into; replaced when the upstream adapter pushes a new device document.</summary>
        public IDevice Device => _device;


        /// <summary>Constructs an adapter-client wired to <paramref name="agent"/> and <paramref name="device"/> using the supplied <paramref name="configuration"/>'s hostname/port; <paramref name="idSuffix"/> is appended to the auto-generated <see cref="ShdrClient.Id"/> when supplied.</summary>
        public ShdrAdapterClient(
            IShdrAdapterClientConfiguration configuration,
            IMTConnectAgent agent,
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

        /// <summary>Overload that lets the caller override the configuration's hostname and port with explicit values (useful for multi-adapter setups that share a single configuration template).</summary>
        public ShdrAdapterClient(
            IShdrAdapterClientConfiguration configuration,
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


        /// <summary>Marks every DataItem of the bound device Unavailable when the adapter connection drops, so the agent's last-value cache reflects the loss of upstream telemetry.</summary>
        protected override void OnDisconnect()
        {
            if (_device != null)
            {
                // Set DataItems to Unavailable if disconnected from Adapter
                SetDeviceUnavailable(UnixDateTime.Now);
            }
        }


        /// <summary>Resolves <paramref name="dataItemKey"/> against the bound device's model so the SHDR parser can inspect the DataItem's representation and Constraints before dispatching the observation.</summary>
        protected override IDataItem OnGetDataItem(string dataItemKey)
        {
            if (_device != null && !string.IsNullOrEmpty(dataItemKey))
            {
                // Return the DataItem matching the DataItemKey
                return _agent.GetDataItem(_device.Uuid, dataItemKey);
            }

            return null;
        }


        /// <summary>Forwards a parsed scalar Sample/Event DataItem observation to the agent, applying the configuration's <c>IgnoreTimestamps</c>, <c>ConvertUnits</c>, and <c>IgnoreObservationCase</c> overrides.</summary>
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

        /// <summary>Forwards a parsed Condition fault state to the agent, applying the configuration's <c>IgnoreTimestamps</c> override.</summary>
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

        /// <summary>Forwards a parsed Message event to the agent, applying the configuration's <c>IgnoreTimestamps</c> override.</summary>
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

        /// <summary>Forwards a parsed Table observation to the agent, applying the configuration's <c>IgnoreTimestamps</c>, <c>ConvertUnits</c>, and <c>IgnoreObservationCase</c> overrides.</summary>
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

        /// <summary>Forwards a parsed DataSet observation to the agent, applying the configuration's <c>IgnoreTimestamps</c>, <c>ConvertUnits</c>, and <c>IgnoreObservationCase</c> overrides.</summary>
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

        /// <summary>Forwards a parsed TimeSeries observation to the agent, applying the configuration's <c>IgnoreTimestamps</c> and <c>ConvertUnits</c> overrides.</summary>
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


        /// <summary>Forwards a parsed asset to the agent, applying the configuration's <c>IgnoreTimestamps</c> override.</summary>
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

        /// <summary>Removes the asset identified by <paramref name="assetId"/> from the agent at <paramref name="timestamp"/>; the timestamp is zeroed when the configuration's <c>IgnoreTimestamps</c> is set so the agent uses its own clock.</summary>
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

        /// <summary>Removes every asset of <paramref name="assetType"/> from the agent at <paramref name="timestamp"/>; <c>IgnoreTimestamps</c> zeroes the timestamp so the agent uses its own clock.</summary>
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


        /// <summary>Replaces the bound device with <paramref name="device"/> and registers it with the agent; subsequent observations are routed against this new device.</summary>
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
                            var condition = new ConditionFaultStateObservationInput(dataItem.Id, ConditionLevel.UNAVAILABLE, timestamp);
                            _agent.AddObservation(_device.Uuid, condition, ignoreTimestamp);
                        }
                    }
                }
            }
        }
    }
}