// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Buffers;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Devices.DataItems.Samples;
using MTConnect.Errors;
using MTConnect.Headers;
using MTConnect.Observations;
using MTConnect.Observations.Input;
using MTConnect.Observations.Output;
using MTConnect.Streams.Output;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Agents
{
    /// <summary>
    /// Publishes MTConnect information in the form of a Response Document to client software applications.
    /// </summary>
    public class MTConnectAgentBroker : MTConnectAgent, IMTConnectAgentBroker, IDisposable
    {
        private readonly object _lock = new object();
        //private readonly IMTConnectDeviceBuffer _deviceBuffer;
        private readonly IMTConnectObservationBuffer _observationBuffer;
        private readonly IMTConnectAssetBuffer _assetBuffer;
        private readonly List<AssetCount> _deviceAssetCounts = new List<AssetCount>();
        protected Dictionary<string, int> _deviceIndexes = new Dictionary<string, int>();
        protected Dictionary<string, int> _dataItemIndexes = new Dictionary<string, int>();


        #region "Properties"

        /// <summary>
        /// Get the configured size of the Buffer in the number of maximum number of Observations the buffer can hold at one time.
        /// </summary>
        public long BufferSize => _observationBuffer != null ? _observationBuffer.BufferSize : 0;

        /// <summary>
        /// Get the configured size of the Asset Buffer in the number of maximum number of Assets the buffer can hold at one time.
        /// </summary>
        public long AssetBufferSize => _assetBuffer != null ? _assetBuffer.BufferSize : 0;

        /// <summary>
        /// A number representing the current number of Asset Documents that are currently stored in the Agent.
        /// </summary>
        public long AssetCount => _assetBuffer != null ? _assetBuffer.AssetCount : 0;

        /// <summary>
        /// A number representing the sequence number assigned to the oldest Observation stored in the buffer
        /// </summary>
        public long FirstSequence => _observationBuffer != null ? _observationBuffer.FirstSequence : 0;

        /// <summary>
        /// A number representing the sequence number assigned to the last Observation that was added to the buffer
        /// </summary>
        public long LastSequence => _observationBuffer != null ? _observationBuffer.LastSequence : 0;

        /// <summary>
        /// A number representing the sequence number of the next Observation that will be added to the buffer
        /// </summary>
        public long NextSequence => _observationBuffer != null ? _observationBuffer.NextSequence : 0;


        public Dictionary<string, int> DeviceIndexes
        {
            get
            {
                lock (_lock) return _deviceIndexes;
            }
            set
            {
                if (value != null)
                {
                    lock (_lock) _deviceIndexes = value;
                }
            }
        }

        public Dictionary<string, int> DataItemIndexes
        {
            get
            {
                lock (_lock) return _dataItemIndexes;
            }
            set
            {
                if (value != null)
                {
                    lock (_lock) _dataItemIndexes = value;
                }
            }
        }


        /// <summary>
        /// Raised when an MTConnectDevices response Document is requested from the Agent
        /// </summary>
        public MTConnectDevicesRequestedHandler DevicesRequestReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectDevices response Document is sent successfully from the Agent
        /// </summary>
        public MTConnectDevicesHandler DevicesResponseSent { get; set; }

        /// <summary>
        /// Raised when an MTConnectStreams response Document is requested from the Agent
        /// </summary>
        public MTConnectStreamsRequestedHandler StreamsRequestReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectStreams response Document is sent successfully from the Agent
        /// </summary>
        public EventHandler StreamsResponseSent { get; set; }

        /// <summary>
        /// Raised when an MTConnectAssets response Document is requested from the Agent
        /// </summary>
        public MTConnectAssetsRequestedHandler AssetsRequestReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectAssets response Document is requested from the Agent for a specific Device
        /// </summary>
        public MTConnectDeviceAssetsRequestedHandler DeviceAssetsRequestReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectAssets response Document is sent successfully from the Agent
        /// </summary>
        public MTConnectAssetsHandler AssetsResponseSent { get; set; }

        /// <summary>
        /// Raised when an MTConnectError response Document is sent successfully from the Agent
        /// </summary>
        public MTConnectErrorHandler ErrorResponseSent { get; set; }

        #endregion

        #region "Constructors"

        public MTConnectAgentBroker(
            string uuid = null,
            long instanceId = 0,
            long deviceModelChangeTime = 0,
            bool initializeAgentDevice = true
            ) : base(uuid, instanceId, deviceModelChangeTime, false)
        {
            var config = new AgentConfiguration();       
            //_deviceBuffer = new MTConnectDeviceBuffer();
            _observationBuffer = new MTConnectObservationBuffer();
            _assetBuffer = new MTConnectAssetBuffer();
            _assetBuffer.AssetRemoved += AssetRemovedFromBuffer;
            InitializeAgentDevice(initializeAgentDevice);
        }

        public MTConnectAgentBroker(
            IAgentConfiguration configuration,
            string uuid = null,
            long instanceId = 0,
            long deviceModelChangeTime = 0,
            bool initializeAgentDevice = true
            ) : base(configuration, uuid, instanceId, deviceModelChangeTime, false)
        {
            var config = configuration != null ? configuration : new AgentConfiguration();
            //_deviceBuffer = new MTConnectDeviceBuffer();
            _observationBuffer = new MTConnectObservationBuffer(config);
            _assetBuffer = new MTConnectAssetBuffer(config);
            _assetBuffer.AssetRemoved += AssetRemovedFromBuffer;
            InitializeAgentDevice(initializeAgentDevice);
        }

        public MTConnectAgentBroker(
            IMTConnectObservationBuffer observationBuffer,
            IMTConnectAssetBuffer assetBuffer,
            string uuid = null,
            long instanceId = 0,
            long deviceModelChangeTime = 0,
            bool initializeAgentDevice = true
            ) : base(uuid, instanceId, deviceModelChangeTime, initializeAgentDevice)
        {
            var config = new AgentConfiguration();
            _observationBuffer = observationBuffer != null ? observationBuffer : new MTConnectObservationBuffer(config);
            _assetBuffer = assetBuffer != null ? assetBuffer : new MTConnectAssetBuffer(config);
            _assetBuffer.AssetRemoved += AssetRemovedFromBuffer;
        }

        public MTConnectAgentBroker(
            IAgentConfiguration configuration,
            IMTConnectObservationBuffer observationBuffer,
            IMTConnectAssetBuffer assetBuffer,
            string uuid = null,
            long instanceId = 0,
            long deviceModelChangeTime = 0,
            bool initializeAgentDevice = true
            ) : base(configuration, uuid, instanceId, deviceModelChangeTime, initializeAgentDevice)
        {
            var config = configuration != null ? configuration : new AgentConfiguration();
            _observationBuffer = observationBuffer != null ? observationBuffer : new MTConnectObservationBuffer(config);
            _assetBuffer = assetBuffer != null ? assetBuffer : new MTConnectAssetBuffer(config);
            _assetBuffer.AssetRemoved += AssetRemovedFromBuffer;
        }

        #endregion

        #region "Initialization"

        public void InitializeCurrentObservations(IEnumerable<BufferObservation> observations)
        {
            if (!observations.IsNullOrEmpty())
            {
                var lObservations = observations.ToList();
                foreach (var observation in lObservations)
                {
                    var deviceUuid = GetDeviceUuid(observation.DeviceIndex);
                    var dataItemId = GetDataItemId(observation.DataItemIndex);

                    var dataItem = GetDataItem(deviceUuid, dataItemId);
                    if (dataItem != null)
                    {
                        var input = new ObservationInput();
                        input.DeviceKey = deviceUuid;
                        input.DataItemKey = dataItemId;
                        input.Timestamp = observation.Timestamp;
                        input.Values = observation.Values;

                        if (dataItem.Category == DataItemCategory.CONDITION)
                        {
                            UpdateCurrentCondition(deviceUuid, dataItem, input);
                        }
                        else
                        {
                            UpdateCurrentObservation(deviceUuid, dataItem, input);
                        }
                    }
                }
            }
        }


        public void InitializeDeviceIndex(IDictionary<string, int> indexes)
        {
            if (!indexes.IsNullOrEmpty())
            {
                lock (_lock)
                {
                    foreach (var index in indexes)
                    {
                        _deviceUuids.Remove(index.Value);
                        _deviceUuids.Add(index.Value, index.Key);

                        _deviceIndexes.Remove(index.Key);
                        _deviceIndexes.Add(index.Key, index.Value);

                        if (index.Value > _lastDeviceIndex) _lastDeviceIndex = index.Value;
                    }
                }
            }
        }

        public void InitializeDataItemIndex(IDictionary<string, int> indexes)
        {
            if (!indexes.IsNullOrEmpty())
            {
                lock (_lock)
                {
                    foreach (var index in indexes)
                    {
                        // Needs to be optimized
                        var dataItemId = new System.Text.RegularExpressions.Regex(".*:(.*)").Match(index.Key).Groups[1].Value;

                        _dataItemIds.Remove(index.Value);
                        _dataItemIds.Add(index.Value, dataItemId);

                        _dataItemIndexes.Remove(index.Key);
                        _dataItemIndexes.Add(index.Key, index.Value);

                        if (index.Value > _lastDataItemIndex) _lastDataItemIndex = index.Value;
                    }
                }
            }
        }

        #endregion

        #region "Cache Lookup"

        private IEnumerable<int> GenerateBufferKeys(IEnumerable<IDevice> devices)
        {
            if (!devices.IsNullOrEmpty())
            {
                var bufferKeys = new List<int>();
                foreach (var device in devices)
                {
                    bufferKeys.AddRange(GenerateBufferKeys(device));
                }
                return bufferKeys;
            }

            return Enumerable.Empty<int>();
        }

        private IEnumerable<int> GenerateBufferKeys(IDevice device)
        {
            if (device != null)
            {
                // Read Cached DataItemIds for the Device
                _deviceDataItemIds.TryGetValue(device.Uuid, out var dataItemIds);
                
                var bufferKeys = new List<int>();
                if (!dataItemIds.IsNullOrEmpty())
                {
                    bufferKeys.AddRange(GenerateBufferKeys(device.Uuid, dataItemIds));
                }
                return bufferKeys;
            }

            return Enumerable.Empty<int>();
        }

        private IEnumerable<int> GenerateBufferKeys(string deviceUuid, IEnumerable<string> dataItemIds)
        {
            if (!string.IsNullOrEmpty(deviceUuid) && !dataItemIds.IsNullOrEmpty())
            {
                var a = dataItemIds.ToArray();
                var keys = new int[a.Length];
                for (var i = 0; i < a.Length; i++)
                {
                    var key = GenerateBufferKey(deviceUuid, a[i]);
                    keys[i] = key;
                }
                return keys;
            }

            return Enumerable.Empty<int>();
        }

        private int GenerateBufferKey(string deviceUuid, string dataItemId)
        {
            var deviceIndex = GetDeviceIndex(deviceUuid);
            var dataItemIndex = GetDataItemIndex(deviceUuid, dataItemId);

            return GenerateBufferKey(deviceIndex, dataItemIndex);
        }

        private static int GenerateBufferKey(int deviceIndex, int dataItemIndex)
        {
            return (deviceIndex * 10000) + dataItemIndex;
        }


        public string GetDeviceUuid(int deviceIndex)
        {
            if (_deviceUuids.TryGetValue(deviceIndex, out var deviceUuid))
            {
                return deviceUuid;
            }

            return null;
        }

        public int GetDeviceIndex(string deviceUuid)
        {
            var index = 0;

            if (deviceUuid != null)
            {
                lock (_lock)
                {
                    _deviceIndexes.TryGetValue(deviceUuid, out index);
                    if (index < 1)
                    {
                        _lastDeviceIndex++;
                        _deviceUuids.Add(_lastDeviceIndex, deviceUuid);
                        _deviceIndexes.Add(deviceUuid, _lastDeviceIndex);
                        index = _lastDeviceIndex;
                    }
                }
            }

            return index;
        }

        public IEnumerable<int> GetDeviceIndexes(IEnumerable<string> deviceUuids)
        {
            var indexes = new List<int>();

            if (!deviceUuids.IsNullOrEmpty())
            {
                foreach (var deviceUuid in deviceUuids)
                {
                    indexes.Add(GetDeviceIndex(deviceUuid));
                }
            }

            return indexes;
        }


        public string GetDataItemId(int dataItemIndex)
        {
            if (_dataItemIds.TryGetValue(dataItemIndex, out var dataItemId))
            {
                return dataItemId;
            }

            return null;
        }

        public string GetDataItemIdFromBufferKey(int bufferKey)
        {
            var dataItemIndex = BufferObservation.GetDataItemIndexFromBufferKey(bufferKey);
            if (_dataItemIds.TryGetValue(dataItemIndex, out var dataItemId))
            {
                return dataItemId;
            }

            return null;
        }

        public int GetDataItemIndex(string deviceUuid, string dataItemId)
        {
            var index = 0;

            if (dataItemId != null)
            {
                var key = string.Concat(deviceUuid, ":", dataItemId);
                lock (_lock)
                {
                    _dataItemIndexes.TryGetValue(key, out index);
                    if (index < 1)
                    {
                        _lastDataItemIndex++;
                        _dataItemIds.Add(_lastDataItemIndex, dataItemId);
                        _dataItemIndexes.Add(key, _lastDataItemIndex);
                        index = _lastDataItemIndex;
                    }
                }
            }

            return index;
        }

        #endregion

        #region "Headers"

        private MTConnectDevicesHeader GetDevicesHeader(Version mtconnectVersion = null)
        {
            var version = mtconnectVersion != null ? mtconnectVersion : MTConnectVersion;

            var header = new MTConnectDevicesHeader
            {
                BufferSize = _observationBuffer.BufferSize,
                AssetBufferSize = _assetBuffer.BufferSize,
                AssetCount = _assetBuffer.AssetCount,
                CreationTime = DateTime.UtcNow,
                DeviceModelChangeTime = DeviceModelChangeTime.ToString("o"),
                InstanceId = InstanceId,
                Sender = Sender,
                Version = Version.ToString(),
                TestIndicator = null
            };

            if (version < MTConnectVersions.Version17) header.DeviceModelChangeTime = null;
            if (version < MTConnectVersions.Version12) header.AssetBufferSize = -1;
            if (version < MTConnectVersions.Version12) header.AssetCount = -1;

            return header;
        }

        private MTConnectStreamsHeader GetStreamsHeader(IObservationBufferResults results, Version mtconnectVersion = null)
        {
            var version = mtconnectVersion != null ? mtconnectVersion : MTConnectVersion;

            var header = new MTConnectStreamsHeader
            {
                BufferSize = _observationBuffer.BufferSize,
                CreationTime = DateTime.UtcNow,
                DeviceModelChangeTime = DeviceModelChangeTime.ToString("o"),
                InstanceId = InstanceId,
                Sender = Sender,
                Version = Version.ToString(),
                FirstSequence = results.FirstSequence,
                LastSequence = results.LastSequence,
                NextSequence = results.NextSequence,
                TestIndicator = null
            };

            if (version < MTConnectVersions.Version17) header.DeviceModelChangeTime = null;

            return header;
        }

        private MTConnectAssetsHeader GetAssetsHeader(Version mtconnectVersion = null)
        {
            var version = mtconnectVersion != null ? mtconnectVersion : MTConnectVersion;

            var header = new MTConnectAssetsHeader
            {
                AssetBufferSize = _assetBuffer.BufferSize,
                AssetCount = _assetBuffer.AssetCount,
                CreationTime = DateTime.UtcNow,
                DeviceModelChangeTime = DeviceModelChangeTime.ToString("o"),
                InstanceId = InstanceId,
                Sender = Sender,
                Version = Version.ToString(),
                TestIndicator = null
            };

            if (version < MTConnectVersions.Version17) header.DeviceModelChangeTime = null;

            return header;
        }

        private MTConnectErrorHeader GetErrorHeader()
        {
            return new MTConnectErrorHeader
            {
                AssetBufferSize = _assetBuffer.BufferSize,
                CreationTime = DateTime.UtcNow,
                InstanceId = InstanceId,
                Sender = Sender,
                Version = Version.ToString(),
                TestIndicator = null
            };
        }

        #endregion

        #region "Devices"

        /// <summary>
        /// Get an MTConnectDevices Response Document containing all devices.
        /// </summary>
        /// <returns>MTConnectDevices Response Document</returns>
        public IDevicesResponseDocument GetDevicesResponseDocument(Version mtconnectVersion = null, string deviceType = null)
        {
            DevicesRequestReceived?.Invoke(null);

            var version = mtconnectVersion != null ? mtconnectVersion : MTConnectVersion;

            var devices = GetDevices(deviceType, version);
            if (devices != null && devices.Count() > 0)
            {
                var doc = new DevicesResponseDocument();
                doc.Version = version;

                var header = GetDevicesHeader(version);
                header.Version = Version.ToString();

                doc.Header = header;
                doc.Devices = ProcessDevices(devices, version);

                DevicesResponseSent?.Invoke(doc);

                return doc;
            }

            return null;
        }

        /// <summary>
        /// Get an MTConnectDevices Response Document containing the specified device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <returns>MTConnectDevices Response Document</returns>
        public IDevicesResponseDocument GetDevicesResponseDocument(string deviceKey, Version mtconnectVersion = null)
        {
            DevicesRequestReceived?.Invoke(deviceKey);

            if (!string.IsNullOrEmpty(deviceKey))
            {
                var version = mtconnectVersion != null ? mtconnectVersion : MTConnectVersion;

                var device = GetDevice(deviceKey, version);
                if (device != null)
                {
                    var doc = new DevicesResponseDocument();
                    doc.Version = version;

                    var header = GetDevicesHeader(version);
                    header.Version = Version.ToString();

                    doc.Header = header;
                    doc.Devices = ProcessDevices(new List<IDevice> { device }, version);

                    DevicesResponseSent?.Invoke(doc);

                    return doc;
                }
            }

            return null;
        }

        #endregion

        #region "Streams"

        #region "Internal"

        private IObservationBufferResults GetObservations(IEnumerable<int> bufferKeys, long from = -1, long to = -1, long at = -1, int count = 0)
        {
            IObservationBufferResults results;
            if (from > 0 || to > 0)
            {
                results = _observationBuffer.GetObservations(bufferKeys, from, to, count);
            }
            else if (count > 0)
            {
                results = _observationBuffer.GetObservations(bufferKeys, count: count);
            }
            else if (at > 0)
            {
                results = _observationBuffer.GetCurrentObservations(bufferKeys, at);
            }
            else
            {
                results = _observationBuffer.GetCurrentObservations(bufferKeys);
            }

            return results;
        }

        #endregion


        /// <summary>
        /// Get a MTConnectStreams Document containing all devices.
        /// </summary>
        /// <param name="count">The Maximum Number of DataItems to return</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(int count = 0, Version mtconnectVersion = null, string deviceType = null)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_observationBuffer != null)
            {
                var devices = GetDevices(deviceType);
                if (!devices.IsNullOrEmpty())
                {
                    // Create list of BufferKeys
                    var bufferKeys = GenerateBufferKeys(devices);

                    // Query the Observation Buffer 
                    var results = GetObservations(bufferKeys, count: count);

                    // Create Response Document
                    var document = CreateDeviceStreamsDocument(devices, ref results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(this, new EventArgs());
                        return document;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get a MTConnectStreams Document containing all devices.
        /// </summary>
        /// <param name="at">The sequence number to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(long at, int count = 0, Version mtconnectVersion = null, string deviceType = null)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_observationBuffer != null)
            {
                var devices = GetDevices(deviceType);
                if (!devices.IsNullOrEmpty())
                {
                    // Create list of BufferKeys
                    var bufferKeys = GenerateBufferKeys(devices);

                    // Query the Observation Buffer 
                    var results = GetObservations(bufferKeys, at: at, count: count);

                    // Create Response Document
                    var document = CreateDeviceStreamsDocument(devices, ref results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(this, new EventArgs());
                        return document;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get a MTConnectStreams Document containing all devices.
        /// </summary>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="at">The sequence number to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(IEnumerable<string> dataItemIds, long at, int count = 0, Version mtconnectVersion = null, string deviceType = null)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_observationBuffer != null)
            {
                var devices = GetDevices(deviceType);
                if (!devices.IsNullOrEmpty())
                {
                    // Create list of BufferKeys
                    var bufferKeys = new List<int>();
                    foreach (var device in devices)
                    {
                        var deviceBufferKeys = GenerateBufferKeys(device.Uuid, dataItemIds);
                        if (!deviceBufferKeys.IsNullOrEmpty()) bufferKeys.AddRange(deviceBufferKeys);
                    }

                    // Query the Observation Buffer 
                    var results = GetObservations(bufferKeys, at: at, count: count);

                    // Create Response Document
                    var document = CreateDeviceStreamsDocument(devices, ref results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(this, new EventArgs());
                        return document;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get a MTConnectStreams Document containing all devices.
        /// </summary>
        /// <param name="from">The sequence number of the first observation to include in the response</param>
        /// <param name="to">The sequence number of the last observation to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(long from, long to, int count = 0, Version mtconnectVersion = null, string deviceType = null)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_observationBuffer != null)
            {
                var devices = GetDevices(deviceType);
                if (!devices.IsNullOrEmpty())
                {
                    // Create list of BufferKeys
                    var bufferKeys = GenerateBufferKeys(devices);

                    // Query the Observation Buffer 
                    var results = GetObservations(bufferKeys, from: from, to: to, count: count);

                    // Create Response Document
                    var document = CreateDeviceStreamsDocument(devices, ref results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(this, new EventArgs());
                        return document;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get a MTConnectStreams Document containing all devices.
        /// </summary>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="from">The sequence number of the first observation to include in the response</param>
        /// <param name="to">The sequence number of the last observation to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(IEnumerable<string> dataItemIds, long from, long to, int count = 0, Version mtconnectVersion = null, string deviceType = null)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_observationBuffer != null)
            {
                var devices = GetDevices(deviceType);
                if (!devices.IsNullOrEmpty())
                {
                    // Create list of BufferKeys
                    var bufferKeys = GenerateBufferKeys(devices);

                    // Query the Observation Buffer 
                    var results = GetObservations(bufferKeys, from, to, count: count);

                    // Create Response Document
                    var document = CreateDeviceStreamsDocument(devices, ref results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(this, new EventArgs());
                        return document;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Get a MTConnectStreams Document containing the specified Device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(string deviceKey, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(deviceKey);

            if (_observationBuffer != null)
            {
                var device = GetDevice(deviceKey);
                if (device != null)
                {
                    // Create list of BufferKeys
                    var bufferKeys = GenerateBufferKeys(device);

                    // Query the Observation Buffer
                    var results = GetObservations(bufferKeys, count: count);

                    // Create Response Document
                    var document = CreateDeviceStreamsDocument(device, ref results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(this, new EventArgs());
                        return document;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get a MTConnectStreams Document containing the specified Device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="at">The sequence number to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(string deviceKey, long at, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(deviceKey);

            if (_observationBuffer != null)
            {
                var device = GetDevice(deviceKey);
                if (device != null)
                {
                    // Create list of BufferKeys
                    var bufferKeys = GenerateBufferKeys(device);

                    // Query the Observation Buffer 
                    var results = GetObservations(bufferKeys, at: at, count: count);

                    // Create Response Document
                    var document = CreateDeviceStreamsDocument(device, ref results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(this, new EventArgs());
                        return document;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get a MTConnectStreams Document containing the specified Device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(string deviceKey, IEnumerable<string> dataItemIds, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(deviceKey);

            if (_observationBuffer != null)
            {
                var device = GetDevice(deviceKey);
                if (device != null)
                {
                    // Create list of BufferKeys
                    var bufferKeys = GenerateBufferKeys(device.Uuid, dataItemIds);

                    // Query the Observation Buffer 
                    var results = GetObservations(bufferKeys, count: count);

                    // Create Response Document
                    var document = CreateDeviceStreamsDocument(device, ref results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(this, new EventArgs());
                        return document;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get a MTConnectStreams Document containing the specified Device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="at">The sequence number to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(string deviceKey, IEnumerable<string> dataItemIds, long at, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(deviceKey);

            if (_observationBuffer != null)
            {
                var device = GetDevice(deviceKey);
                if (device != null)
                {
                    // Create list of BufferKeys
                    var bufferKeys = GenerateBufferKeys(device.Uuid, dataItemIds);

                    // Query the Observation Buffer 
                    var results = GetObservations(bufferKeys, at: at, count: count);

                    // Create Response Document
                    var document = CreateDeviceStreamsDocument(device, ref results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(this, new EventArgs());
                        return document;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get a MTConnectStreams Document containing the specified Device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="from">The sequence number of the first observation to include in the response</param>
        /// <param name="to">The sequence number of the last observation to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(string deviceKey, long from, long to, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(deviceKey);

            if (_observationBuffer != null)
            {
                var device = GetDevice(deviceKey);
                if (device != null)
                {
                    // Create list of BufferKeys
                    var bufferKeys = GenerateBufferKeys(device);

                    // Query the Observation Buffer 
                    var results = GetObservations(bufferKeys, from, to: to, count: count);

                    // Create Response Document
                    var document = CreateDeviceStreamsDocument(device, ref results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(this, new EventArgs());
                        return document;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get a MTConnectStreams Document containing the specified Device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="from">The sequence number of the first observation to include in the response</param>
        /// <param name="to">The sequence number of the last observation to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public IStreamsResponseOutputDocument GetDeviceStreamsResponseDocument(string deviceKey, IEnumerable<string> dataItemIds, long from, long to, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(deviceKey);

            if (_observationBuffer != null)
            {
                var device = GetDevice(deviceKey);
                if (device != null)
                {
                    // Create list of BufferKeys
                    var bufferKeys = GenerateBufferKeys(device.Uuid, dataItemIds);

                    // Query the Observation Buffer 
                    var results = GetObservations(bufferKeys, from, to: to, count: count);

                    // Create Response Document
                    var document = CreateDeviceStreamsDocument(device, ref results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(this, new EventArgs());
                        return document;
                    }
                }
            }

            return null;
        }


        #region "Create"

        private IStreamsResponseOutputDocument CreateDeviceStreamsDocument(IDevice device, ref IObservationBufferResults results, Version mtconnectVersion)
        {
            if (device != null)
            {
                return CreateDeviceStreamsDocument(new List<IDevice> { device }, ref results, mtconnectVersion);
            }

            return null;
        }

        private IStreamsResponseOutputDocument CreateDeviceStreamsDocument(IEnumerable<IDevice> devices, ref IObservationBufferResults results, Version mtconnectVersion)
        {
            if (results != null)
            {
                var version = mtconnectVersion != null ? mtconnectVersion : MTConnectVersion;
                var lDevices = devices.ToArray();

                // Create list of DeviceStreams to return
                var deviceStreams = new IDeviceStreamOutput[lDevices.Length];
                for (var i = 0; i < lDevices.Length; i++)
                {
                    // Create a DeviceStream based on the query results from the buffer
                    deviceStreams[i] = CreateDeviceStream(lDevices[i], ref results, version);
                }

                if (!deviceStreams.IsNullOrEmpty())
                {
                    // Create MTConnectStreams Document
                    var doc = new StreamsResponseOutputDocument();
                    doc.Version = version;
                    doc.Header = GetStreamsHeader(results, version);
                    doc.Streams = deviceStreams;
                    doc.FirstObservationSequence = results.FirstObservationSequence;
                    doc.LastObservationSequence = results.LastObservationSequence;
                    doc.ObservationCount = results.ObservationCount;

                    return doc;
                }
            }

            return null;
        }

        private IDeviceStreamOutput CreateDeviceStream(IDevice device, ref IObservationBufferResults dataItemResults, Version mtconnectVersion)
        {
            // Create DeviceStream
            var deviceStream = new DeviceStreamOutput();
            deviceStream.Name = device.Name;
            deviceStream.Uuid = device.Uuid;

            // Get a list of All Components for the Device
            var components = device.GetComponents();
            var componentStreams = new List<IComponentStreamOutput>();

            if (!components.IsNullOrEmpty())
            {
                foreach (var component in components)
                {
                    // Process Component (to check MTConnect Version compatibility
                    if (Component.IsCompatible(component, mtconnectVersion))
                    {
                        // Get All DataItems (Component Root DataItems and Composition DataItems)
                        var dataItems = new List<IDataItem>();
                        if (!component.DataItems.IsNullOrEmpty()) dataItems.AddRange(component.DataItems);
                        if (!component.Compositions.IsNullOrEmpty())
                        {
                            foreach (var composition in component.Compositions)
                            {
                                if (!composition.DataItems.IsNullOrEmpty()) dataItems.AddRange(composition.DataItems);
                            }
                        }

                        // Create a ComponentStream for the Component
                        var componentStream = new ComponentStreamOutput();
                        componentStream.ComponentId = component.Id;
                        componentStream.ComponentType = component.Type;
                        componentStream.Component = component;
                        componentStream.Name = component.Name;
                        componentStream.Uuid = component.Uuid;
                        componentStream.Observations = GetObservations(device.Uuid, ref dataItemResults, dataItems, mtconnectVersion);
                        if (componentStream.Observations != null && componentStream.Observations.Length > 0)
                        {
                            componentStreams.Add(componentStream);
                        }
                    }
                }
            }

            // Add ComponentStream for Device
            var deviceComponentStream = new ComponentStreamOutput();
            deviceComponentStream.ComponentId = device.Id;
            deviceComponentStream.ComponentType = device.Type;
            deviceComponentStream.Component = device;
            deviceComponentStream.Name = device.Name;
            deviceComponentStream.Uuid = device.Uuid;
            deviceComponentStream.Observations = GetObservations(device.Uuid, ref dataItemResults, device.DataItems, mtconnectVersion);
            if (deviceComponentStream.Observations != null && deviceComponentStream.Observations.Length > 0)
            {
                componentStreams.Add(deviceComponentStream);
            }

            if (componentStreams.Count > 0)
            {
                deviceStream.ComponentStreams = componentStreams.ToArray();
            }

            return deviceStream;
        }

        private IObservationOutput[] GetObservations(string deviceUuid, ref IObservationBufferResults dataItemResults, IEnumerable<IDataItem> dataItems, Version mtconnectVersion = null)
        {
            if (dataItemResults != null && !dataItemResults.Observations.IsNullOrEmpty() && !dataItems.IsNullOrEmpty())
            {
                var dDataItems = dataItems.ToDictionary(o => o.Id);

                var objs = new List<IObservationOutput>();
                var objIndex = 0;
                var previousIndex = 0;
                var found = false;

                // Create a sorted list of BufferKeys
                var bufferKeys = GenerateBufferKeys(deviceUuid, dDataItems.Keys);
                bufferKeys = bufferKeys.OrderBy(o => o);

                foreach (var bufferKey in bufferKeys)
                {
                    var dataItemId = GetDataItemIdFromBufferKey(bufferKey);
                    found = false;

                    dDataItems.TryGetValue(dataItemId, out var dataItem);
                    if (dataItem != null)
                    {
                        // Check Version Compatibility and Create Derived Class (if found)
                        if (DataItem.IsCompatible(dataItem, mtconnectVersion))
                        {
                            // Loop through StoredObservations
                            // List is already sorted and uses the sorted BufferKeys to set the last start index
                            for (var i = previousIndex; i < dataItemResults.Observations.Length; i++)
                            {
                                if (dataItemResults.Observations[i].Key == bufferKey)
                                {
                                    objs.Add(CreateObservation(dataItem, ref dataItemResults.Observations[i]));
                                    objIndex++;

                                    previousIndex = i;
                                    found = true;
                                }
                                else if (found) break;
                            }
                        }
                    }
                }

                if (objIndex > 0)
                {
                    return objs.ToArray();
                }
            }

            return null;
        }

        private static IObservationOutput CreateObservation(IDataItem dataItem, ref BufferObservation bufferObservation)
        {
            var observation = new ObservationOutput();
            observation._dataItem = dataItem;
            if (dataItem.Device != null) observation._deviceUuid = dataItem.Device.Uuid;
            observation._dataItemId = dataItem.Id;
            observation._category = dataItem.Category;
            observation._representation = dataItem.Representation;
            observation._type = dataItem.Type;
            observation._subType = dataItem.SubType;
            observation._name = dataItem.Name;
            observation._compositionId = dataItem.CompositionId;
            observation._sequence = bufferObservation.Sequence;
            observation._timestamp = bufferObservation.Timestamp.ToDateTime();
            observation._values = bufferObservation.Values;
            return observation;
        }

        #endregion

        #endregion

        #region "Assets"

        /// <summary>
        /// Get an MTConnectAssets Document containing all Assets.
        /// </summary>
        /// <param name="deviceKey">Optional  Device name or uuid. If not given, all devices are returned.</param>
        /// <param name="type">Defines the type of MTConnect Asset to be returned in the MTConnectAssets Response Document.</param>
        /// <param name="removed">
        /// An attribute that indicates whether the Asset has been removed from a piece of equipment.
        /// If the value of the removed parameter in the query is true, then Asset Documents for Assets that have been marked as removed from a piece of equipment will be included in the Response Document.
        /// If the value of the removed parameter in the query is false, then Asset Documents for Assets that have been marked as removed from a piece of equipment will not be included in the Response Document.
        /// </param>
        /// <param name="count">Defines the maximum number of Asset Documents to return in an MTConnectAssets Response Document.</param>
        /// <returns>MTConnectAssets Response Document</returns>
        public IAssetsResponseDocument GetAssetsResponseDocument(string deviceKey = null, string type = null, bool removed = false, int count = 0, Version mtconnectVersion = null)
        {
            DeviceAssetsRequestReceived?.Invoke(deviceKey);

            if (_assetBuffer != null)
            {
                // Set MTConnect Version
                var version = mtconnectVersion != null ? mtconnectVersion : MTConnectVersion;

                var processedAssets = new List<IAsset>();

                // Get Device UUID from deviceKey
                string deviceUuid = GetDeviceUuid(deviceKey);

                // Get Assets from AssetsBuffer
                var assets = _assetBuffer.GetAssets(deviceUuid, type, removed, count);
                if (!assets.IsNullOrEmpty())
                {
                    // Process Assets
                    foreach (var asset in assets)
                    {
                        var processedAsset = asset.Process(version);
                        if (processedAsset != null) processedAssets.Add(processedAsset);
                    }
                }

                // Create AssetsHeader
                var header = GetAssetsHeader(version);
                header.Version = Version.ToString();
                header.InstanceId = InstanceId;

                // Create MTConnectAssets Response Document
                var document = new AssetsResponseDocument();
                document.Version = version;
                document.Header = header;
                document.Assets = processedAssets;

                AssetsResponseSent?.Invoke(document);

                return document;
            }

            return null;
        }

        /// <summary>
        /// Get an MTConnectAssets Document containing the specified Asset
        /// </summary>
        /// <param name="assetIds">The IDs of the Assets to include in the response</param>
        /// <returns>MTConnectAssets Response Document</returns>
        public IAssetsResponseDocument GetAssetsResponseDocument(IEnumerable<string> assetIds, Version mtconnectVersion = null)
        {
            AssetsRequestReceived?.Invoke(assetIds);

            if (_assetBuffer != null)
            {
                // Set MTConnect Version
                var version = mtconnectVersion != null ? mtconnectVersion : MTConnectVersion;

                var processedAssets = new List<IAsset>();

                // Get Assets from AssetsBuffer
                var assets = _assetBuffer.GetAssets(assetIds);
                if (!assets.IsNullOrEmpty())
                {
                    // Process Assets
                    foreach (var asset in assets)
                    {
                        var processedAsset = asset.Process(version);
                        if (processedAsset != null) processedAssets.Add(processedAsset);
                    }
                }

                // Create AssetsHeader
                var header = GetAssetsHeader(version);
                header.Version = Version.ToString();
                header.InstanceId = InstanceId;

                // Create MTConnectAssets Response Document
                var document = new AssetsResponseDocument();
                document.Version = version;
                document.Header = header;
                document.Assets = processedAssets;

                AssetsResponseSent?.Invoke(document);

                return document;
            }

            return null;
        }


        /// <summary>
        /// Remove the Asset with the specified Asset ID
        /// </summary>
        /// <param name="assetId">The ID of the Asset to remove</param>
        /// <param name="timestamp">The Timestamp of when the Asset was removed in Unix Ticks (1/10,000 of a millisecond)</param>
        /// <returns>Returns True if the Asset was successfully removed</returns>
        public override bool RemoveAsset(string assetId, long timestamp = 0)
        {
            if (!string.IsNullOrEmpty(assetId) && _assetBuffer != null)
            {
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

                // Get the Asset from the Buffer
                var asset = _assetBuffer.GetAsset(assetId);
                if (asset != null)
                {
                    var deviceUuid = asset.DeviceUuid;

                    // Remove the Asset from the Buffer
                    if (_assetBuffer.RemoveAsset(assetId))
                    {
                        // Get the Device from the Buffer (to set the AssetRemoved DataItem)
                        var device = GetDevice(deviceUuid);
                        if (device != null)
                        {
                            // Update AssetRemoved DataItem
                            if (!device.DataItems.IsNullOrEmpty())
                            {
                                var assetRemoved = device.DataItems.FirstOrDefault(o => o.Type == AssetRemovedDataItem.TypeId);
                                if (assetRemoved != null)
                                {
                                    AddObservation(deviceUuid, assetRemoved.Id, ValueKeys.Result, asset.AssetId, ts);
                                }
                            }
                        }

                        // Update Asset Count
                        DecrementAssetCount(deviceUuid, asset.Type);

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Remove the Asset with the specified Asset ID
        /// </summary>
        /// <param name="assetId">The ID of the Asset to remove</param>
        /// <param name="timestamp">The Timestamp of when the Asset was removed</param>
        /// <returns>Returns True if the Asset was successfully removed</returns>
        public override bool RemoveAsset(string assetId, DateTime timestamp)
        {
            return RemoveAsset(assetId, timestamp.ToUnixTime());
        }


        /// <summary>
        /// Remove all Assets with the specified Type
        /// </summary>
        /// <param name="assetType">The Type of the Assets to remove</param>
        /// <param name="timestamp">The Timestamp of when the Assets were removed in Unix Ticks (1/10,000 of a millisecond)</param>
        /// <returns>Returns True if the Assets were successfully removed</returns>
        public override bool RemoveAllAssets(string assetType, long timestamp = 0)
        {
            if (!string.IsNullOrEmpty(assetType) && _assetBuffer != null)
            {
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

                // Get the Assets from the Buffer
                var assets = _assetBuffer.GetAssets(assetType);
                if (!assets.IsNullOrEmpty())
                {
                    var deviceUuids = assets.Select(o => o.DeviceUuid).Distinct();

                    // Remove the Assets from the Buffer
                    if (_assetBuffer.RemoveAllAssets(assetType))
                    {
                        foreach (var deviceUuid in deviceUuids)
                        {
                            // Get the Device from the Buffer (to set the AssetRemoved DataItem)
                            var device = GetDevice(deviceUuid);
                            if (device != null)
                            {
                                var deviceAssets = assets.Where(o => o.DeviceUuid == deviceUuid);
                                if (!deviceAssets.IsNullOrEmpty())
                                {
                                    foreach (var asset in deviceAssets)
                                    {
                                        // Update AssetRemoved DataItem
                                        if (!device.DataItems.IsNullOrEmpty())
                                        {
                                            var assetRemoved = device.DataItems.FirstOrDefault(o => o.Type == AssetRemovedDataItem.TypeId);
                                            if (assetRemoved != null)
                                            {
                                                AddObservation(deviceUuid, assetRemoved.Id, ValueKeys.Result, asset.AssetId, ts);
                                            }
                                        }

                                        // Update Asset Count
                                        DecrementAssetCount(deviceUuid, asset.Type);
                                    }
                                }
                            }
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Remove all Assets with the specified Type
        /// </summary>
        /// <param name="assetType">The Type of the Assets to remove</param>
        /// <param name="timestamp">The Timestamp of when the Assets were removed</param>
        /// <returns>Returns True if the Assets were successfully removed</returns>
        public override bool RemoveAllAssets(string assetType, DateTime timestamp)
        {
            return RemoveAllAssets(assetType, timestamp.ToUnixTime());
        }


        private void AssetRemovedFromBuffer(object sender, IAsset asset)
        {
            if (asset != null)
            {
                var device = GetDevice(asset.DeviceUuid);
                if (device != null)
                {
                    // Update AssetRemoved DataItem
                    if (!device.DataItems.IsNullOrEmpty())
                    {
                        var assetRemoved = device.DataItems.FirstOrDefault(o => o.Type == AssetRemovedDataItem.TypeId);
                        if (assetRemoved != null)
                        {
                            var assetChangedObservation = new ObservationInput();
                            assetChangedObservation.DataItemKey = assetRemoved.Id;
                            assetChangedObservation.AddValue(ValueKeys.Result, asset.AssetId);
                            assetChangedObservation.AddValue(ValueKeys.AssetType, asset.Type);
                            assetChangedObservation.Timestamp = asset.Timestamp;
                            AddObservation(asset.DeviceUuid, assetChangedObservation);
                        }
                    }

                    // Update Asset Count
                    DecrementAssetCount(asset.DeviceUuid, asset.Type);
                }
            }
        }

        private void IncrementAssetCount(string deviceUuid, string assetType)
        {
            if (!string.IsNullOrEmpty(deviceUuid) && !string.IsNullOrEmpty(assetType))
            {
                var typeCount = 0;

                lock (_lock)
                {
                    var assetCount = _deviceAssetCounts.FirstOrDefault(o => o.DeviceUuid == deviceUuid && o.AssetType == assetType);
                    if (assetCount.IsValid)
                    {
                        _deviceAssetCounts.RemoveAll(o => o.DeviceUuid == deviceUuid && o.AssetType == assetType);
                        typeCount = assetCount.Count;
                    }
                    typeCount += 1;
                    _deviceAssetCounts.Add(new AssetCount(deviceUuid, assetType, typeCount));
                }

                // Update AssetCount DataItem
                if (_deviceDataItems.TryGetValue(deviceUuid, out var dataItems))
                {
                    if (!dataItems.IsNullOrEmpty())
                    {
                        var assetCountDataItem = dataItems.FirstOrDefault(o => o.Type == AssetCountDataItem.TypeId);
                        if (assetCountDataItem != null)
                        {
                            var entries = new List<IDataSetEntry>();
                            entries.Add(new DataSetEntry(assetType, typeCount));
                            var dataSet = new DataSetObservationInput(assetCountDataItem.Id, entries);

                            // Add Observation with updated DataSet Key for AssetType
                            AddObservation(deviceUuid, dataSet);
                        }
                    }
                }
            }
        }

        private void DecrementAssetCount(string deviceUuid, string assetType)
        {
            if (!string.IsNullOrEmpty(deviceUuid) && !string.IsNullOrEmpty(assetType))
            {
                var typeCount = 0;

                lock (_lock)
                {
                    var assetCount = _deviceAssetCounts.FirstOrDefault(o => o.DeviceUuid == deviceUuid && o.AssetType == assetType);
                    if (assetCount.IsValid)
                    {
                        _deviceAssetCounts.RemoveAll(o => o.DeviceUuid == deviceUuid && o.AssetType == assetType);
                        typeCount = assetCount.Count;
                        typeCount -= 1;

                        if (typeCount > 1)
                        {
                            _deviceAssetCounts.Add(new AssetCount(deviceUuid, assetType, typeCount));
                        }
                    }
                }

                // Update AssetCount DataItem
                if (_deviceDataItems.TryGetValue(deviceUuid, out var dataItems))
                {
                    if (!dataItems.IsNullOrEmpty())
                    {
                        var assetCountDataItem = dataItems.FirstOrDefault(o => o.Type == AssetCountDataItem.TypeId);
                        if (assetCountDataItem != null)
                        {
                            var entries = new List<IDataSetEntry>();

                            if (typeCount > 0) entries.Add(new DataSetEntry(assetType, typeCount));
                            else entries.Add(new DataSetEntry(assetType, true));

                            var dataSet = new DataSetObservationInput(assetCountDataItem.Id, entries);

                            // Add Observation with updated DataSet Key for AssetType
                            AddObservation(deviceUuid, dataSet);
                        }
                    }
                }
            }
        }

        #endregion

        #region "Errors"

        /// <summary>
        /// Get an MTConnectErrors Document containing the specified ErrorCode
        /// </summary>
        /// <param name="errorCode">Provides a descriptive code that indicates the type of error that was encountered by an Agent when attempting to respond to a Request for information.</param>
        /// <param name="value">A textual description of the error and any additional information an Agent is capable of providing regarding a specific error.</param>
        /// <returns>MTConnectError Response Document</returns>
        public IErrorResponseDocument GetErrorResponseDocument(ErrorCode errorCode, string value = null, Version mtconnectVersion = null)
        {
            var version = mtconnectVersion != null ? mtconnectVersion : MTConnectVersion;

            var doc = new ErrorResponseDocument();
            doc.Version = version;

            var header = GetErrorHeader();
            header.Version = Version.ToString();

            doc.Header = header;
            doc.Errors = new List<Error>
            {
                new Error(errorCode, value)
            };

            ErrorResponseSent?.Invoke(doc);

            return doc;
        }

        /// <summary>
        /// Get an MTConnectErrors Document containing the specified Errors
        /// </summary>
        /// <param name="errors">A list of Errors to include in the response Document</param>
        /// <returns>MTConnectError Response Document</returns>
        public IErrorResponseDocument GetErrorResponseDocument(IEnumerable<IError> errors, Version mtconnectVersion = null)
        {
            var version = mtconnectVersion != null ? mtconnectVersion : MTConnectVersion;

            var doc = new ErrorResponseDocument();
            doc.Version = version;

            var header = GetErrorHeader();
            header.Version = Version.ToString();

            doc.Header = header;
            doc.Errors = errors != null ? errors.ToList() : null;

            ErrorResponseSent?.Invoke(doc);

            return doc;
        }

        #endregion

        #region "Add"

        public override void InitializeDataItems(IDevice device, long timestamp = 0)
        {
            if (device != null && _observationBuffer != null)
            {
                // Get All DataItems for the Device
                var dataItems = device.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    var deviceIndex = GetDeviceIndex(device.Uuid);
                    var dataItemIndexes = new List<int>();

                    // Create list of DataItemIndexes
                    foreach (var dataItem in dataItems)
                    {
                        dataItemIndexes.Add(GetDataItemIndex(device.Uuid, dataItem.Id));
                    }

                    // Get the BufferKeys to use for the ObservationBuffer
                    var bufferKeys = GenerateBufferKeys(device);

                    // Get all Current Observations for the Device
                    var results = _observationBuffer.GetObservations(bufferKeys);

                    var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

                    foreach (var dataItem in dataItems)
                    {
                        bool exists = false;

                        // Lookup Buffer Key
                        var bufferKey = GenerateBufferKey(device.Uuid, dataItem.Id);

                        // Check if the DataItem has an observation
                        if (results != null && !results.Observations.IsNullOrEmpty())
                        {
                            exists = results.Observations.Any(o => o.Key == bufferKey);
                        }

                        // If no observation exists, then add an Unavailable observation
                        if (!exists)
                        {
                            var valueType = dataItem.Category == DataItemCategory.CONDITION ? ValueKeys.Level : ValueKeys.Result;
                            var value = !string.IsNullOrEmpty(dataItem.InitialValue) ? dataItem.InitialValue : Observation.Unavailable;

                            // Add Unavailable Observation to ObservationBuffer
                            var observation = Observation.Create(dataItem);
                            observation.DeviceUuid = device.Uuid;
                            observation.Timestamp = ts.ToDateTime();
                            observation.AddValues(new List<ObservationValue>
                            {
                                new ObservationValue(valueType, value)
                            });

                            // Add Required Values
                            switch (dataItem.Representation)
                            {
                                case DataItemRepresentation.DATA_SET: observation.AddValue(ValueKeys.Count, 0); break;
                                case DataItemRepresentation.TABLE: observation.AddValue(ValueKeys.Count, 0); break;
                                case DataItemRepresentation.TIME_SERIES: observation.AddValue(ValueKeys.SampleCount, 0); break;
                            }

                            if (dataItem.Category == DataItemCategory.CONDITION)
                            {
                                UpdateCurrentCondition(device.Uuid, dataItem, new ObservationInput(observation));
                            }
                            else
                            {
                                UpdateCurrentObservation(device.Uuid, dataItem, new ObservationInput(observation));
                            }

                            var bufferObservation = new BufferObservation(bufferKey, observation);
                            _observationBuffer.AddObservation(ref bufferObservation);
                            ObservationAdded?.Invoke(this, observation);
                        }
                    }
                }
            }
        }

        protected override bool OnAddObservation(string deviceUuid, IDataItem dataItem, IObservationInput observationInput)
        {
            if (_observationBuffer != null && dataItem != null && observationInput != null)
            {
                // Get the BufferKey to use for the ObservationBuffer
                var bufferKey = GenerateBufferKey(deviceUuid, dataItem.Id);
                var bufferObservation = new BufferObservation(
                    bufferKey,
                    dataItem.Category,
                    dataItem.Representation,
                    observationInput.Values,
                    0,
                    observationInput.Timestamp
                    );

                // Add Observation to Streaming Buffer
                if (_observationBuffer.AddObservation(ref bufferObservation))
                {
                    return true;
                }
            }

            return false;
        }

        protected override bool OnAssetUpdate(IAsset asset)
        {
            return _assetBuffer.AddAsset(asset);
        }

        protected override bool OnNewAssetAdded(IAsset asset)
        {
            if (asset != null)
            {
                // Update Asset Count
                IncrementAssetCount(asset.DeviceUuid, asset.Type);
            }

            return base.OnNewAssetAdded(asset);
        }

        #endregion
    }
}