// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents.Configuration;
using MTConnect.Agents.Metrics;
using MTConnect.Assets;
using MTConnect.Buffers;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Devices.DataItems.Samples;
using MTConnect.Devices.Components;
using MTConnect.Errors;
using MTConnect.Headers;
//using MTConnect.Models;
using MTConnect.Observations;
using MTConnect.Observations.Input;
using MTConnect.Streams;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MTConnect.Agents
{
    /// <summary>
    /// An Agent is the centerpiece of an MTConnect implementation. 
    /// It provides two primary functions:
    /// Organizes and manages individual pieces of information published by one or more pieces of equipment.
    /// Publishes that information in the form of a Response Document to client software applications.
    /// </summary>
    public class MTConnectAgent : IMTConnectAgent, IDisposable
    {
        private const string AdaptersId = "__adapters__";
        private const int InformationUpdateInterval = 1000;


        private readonly MTConnectAgentConfiguration _configuration;
        private readonly MTConnectAgentInformation _information;
        private readonly IMTConnectDeviceBuffer _deviceBuffer;
        private readonly IMTConnectObservationBuffer _observationBuffer;
        private readonly IMTConnectAssetBuffer _assetBuffer;
        private readonly ConcurrentDictionary<string, string> _deviceKeys = new ConcurrentDictionary<string, string>(); // Resolves either the Device Name or UUID to the Device UUID
        private readonly ConcurrentDictionary<string, IObservationInput> _currentObservations = new ConcurrentDictionary<string, IObservationInput>();
        private readonly ConcurrentDictionary<string, IEnumerable<IObservationInput>> _currentConditions = new ConcurrentDictionary<string, IEnumerable<IObservationInput>>();
        private readonly MTConnectAgentMetrics _metrics = new MTConnectAgentMetrics(TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(1));
        private readonly long _instanceId;
        private readonly string _uuid;
        private long _deviceModelChangeTime;
        private Version _mtconnectVersion;
        private Agent _agent;
        private System.Timers.Timer _informationUpdateTimer;
        private bool _updateInformation;

        /// <summary>
        /// Gets the Configuration associated with the Agent
        /// </summary>
        public MTConnectAgentConfiguration Configuration => _configuration;

        /// <summary>
        /// Gets the Information associated with the Agent
        /// </summary>
        public MTConnectAgentInformation Information => _information;

        /// <summary>
        /// Gets the Metrics associated with the Agent
        /// </summary>
        public MTConnectAgentMetrics Metrics => _metrics;

        /// <summary>
        /// Gets or Sets the Agent Device that represents the MTConnect Agent
        /// </summary>
        public Agent Agent => _agent;

        /// <summary>
        /// Gets the UUID that uniquely identifies the Agent
        /// </summary>
        public string Uuid => _uuid;

        /// <summary>
        /// Gets a representation of the specific instance of the Agent.
        /// </summary>
        public long InstanceId => _instanceId;

        /// <summary>
        /// Gets the MTConnect Version that the Agent is using.
        /// </summary>
        public Version Version
        {
            get => _mtconnectVersion;
            set
            {
                _mtconnectVersion = value;
                if (_agent != null) _agent.MTConnectVersion = _mtconnectVersion;
            }
        }

        /// <summary>
        /// Get the configured size of the Buffer in the number of maximum number of Observations the buffer can hold at one time.
        /// </summary>
        public long BufferSize => _observationBuffer != null ? _observationBuffer.BufferSize : 0;

        /// <summary>
        /// Get the configured size of the Asset Buffer in the number of maximum number of Assets the buffer can hold at one time.
        /// </summary>
        public long AssetBufferSize => _assetBuffer != null ? _assetBuffer.BufferSize : 0;

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

        /// <summary>
        /// Raised when a new Device is added to the Agent
        /// </summary>
        public EventHandler<IDevice> DeviceAdded { get; set; }

        /// <summary>
        /// Raised when a new Observation is attempted to be added to the Agent
        /// </summary>
        public EventHandler<IObservationInput> ObservationReceived { get; set; }

        /// <summary>
        /// Raised when a new Observation is added to the Agent
        /// </summary>
        public EventHandler<IObservation> ObservationAdded { get; set; }

        /// <summary>
        /// Raised when a new Asset is attempted to be added to the Agent
        /// </summary>
        public EventHandler<IAsset> AssetReceived { get; set; }

        /// <summary>
        /// Raised when a new Asset is added to the Agent
        /// </summary>
        public EventHandler<IAsset> AssetAdded { get; set; }

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
        public MTConnectStreamsHandler StreamsResponseSent { get; set; }

        /// <summary>
        /// Raised when an MTConnectAssets response Document is requested from the Agent
        /// </summary>
        public MTConnectAssetsRequestedHandler AssetsRequestReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectAssets response Document is sent successfully from the Agent
        /// </summary>
        public MTConnectAssetsHandler AssetsResponseSent { get; set; }

        /// <summary>
        /// Raised when an MTConnectError response Document is sent successfully from the Agent
        /// </summary>
        public MTConnectErrorHandler ErrorResponseSent { get; set; }

        /// <summary>
        /// Raised when an Invalid DataItem Observation is Added
        /// </summary>
        public MTConnectDataItemValidationHandler InvalidDataItemAdded { get; set; }

        /// <summary>
        /// Raised when an Invalid Asset is Added
        /// </summary>
        public MTConnectAssetValidationHandler InvalidAssetAdded { get; set; }


        public MTConnectAgent(
            string uuid = null,
            long instanceId = 0,
            long deviceModelChangeTime = 0,
            bool initializeAgentDevice = true
            )
        {
            _uuid = !string.IsNullOrEmpty(uuid) ? uuid : Guid.NewGuid().ToString();
            _configuration = new MTConnectAgentConfiguration();
            _information = new MTConnectAgentInformation(_uuid, _instanceId, _deviceModelChangeTime);
            _instanceId = instanceId > 0 ? instanceId : CreateInstanceId();
            _deviceModelChangeTime = deviceModelChangeTime;
            _mtconnectVersion = MTConnectVersions.Max;
            _deviceBuffer = new MTConnectDeviceBuffer();
            _observationBuffer = new MTConnectObservationBuffer();
            _assetBuffer = new MTConnectAssetBuffer();
            _metrics.DeviceMetricsUpdated += DeviceMetricsUpdated;
            if (initializeAgentDevice) InitializeAgentDevice();
            StartAgentInformationUpdateTimer();
        }

        public MTConnectAgent(
            MTConnectAgentConfiguration configuration,
            string uuid = null,
            long instanceId = 0,
            long deviceModelChangeTime = 0,
            bool initializeAgentDevice = true
            )
        {
            _uuid = !string.IsNullOrEmpty(uuid) ? uuid : Guid.NewGuid().ToString();
            _configuration = configuration != null ? configuration : new MTConnectAgentConfiguration();
            _information = new MTConnectAgentInformation(_uuid, _instanceId, _deviceModelChangeTime);
            _deviceModelChangeTime = deviceModelChangeTime;
            _mtconnectVersion = _configuration != null ? _configuration.DefaultVersion : MTConnectVersions.Max;
            _instanceId = instanceId > 0 ? instanceId : CreateInstanceId();
            _deviceBuffer = new MTConnectDeviceBuffer();
            _observationBuffer = new MTConnectObservationBuffer(_configuration);
            _assetBuffer = new MTConnectAssetBuffer(_configuration);
            _metrics.DeviceMetricsUpdated += DeviceMetricsUpdated;
            if (initializeAgentDevice) InitializeAgentDevice();
            StartAgentInformationUpdateTimer();
        }

        public MTConnectAgent(
            IMTConnectDeviceBuffer deviceBuffer,
            IMTConnectObservationBuffer observationBuffer,
            IMTConnectAssetBuffer assetBuffer,
            string uuid = null,
            long instanceId = 0,
            long deviceModelChangeTime = 0,
            bool initializeAgentDevice = true
            )
        {
            _uuid = !string.IsNullOrEmpty(uuid) ? uuid : Guid.NewGuid().ToString();
            _configuration = new MTConnectAgentConfiguration();
            _information = new MTConnectAgentInformation(_uuid, _instanceId, _deviceModelChangeTime);
            _instanceId = instanceId > 0 ? instanceId : CreateInstanceId();
            _deviceModelChangeTime = deviceModelChangeTime;
            _mtconnectVersion = MTConnectVersions.Max;
            _deviceBuffer = deviceBuffer != null ? deviceBuffer : new MTConnectDeviceBuffer();
            _observationBuffer = observationBuffer != null ? observationBuffer : new MTConnectObservationBuffer(_configuration);
            _assetBuffer = assetBuffer != null ? assetBuffer : new MTConnectAssetBuffer(_configuration);
            _metrics.DeviceMetricsUpdated += DeviceMetricsUpdated;
            if (initializeAgentDevice) InitializeAgentDevice();
            StartAgentInformationUpdateTimer();
        }

        public MTConnectAgent(
            MTConnectAgentConfiguration configuration,
            IMTConnectDeviceBuffer deviceBuffer,
            IMTConnectObservationBuffer observationBuffer,
            IMTConnectAssetBuffer assetBuffer,
            string uuid = null,
            long instanceId = 0,
            long deviceModelChangeTime = 0,
            bool initializeAgentDevice = true
            )
        {
            _uuid = !string.IsNullOrEmpty(uuid) ? uuid : Guid.NewGuid().ToString();
            _configuration = configuration != null ? configuration : new MTConnectAgentConfiguration();
            _instanceId = instanceId > 0 ? instanceId : CreateInstanceId();
            _deviceModelChangeTime = deviceModelChangeTime;
            _information = new MTConnectAgentInformation(_uuid, _instanceId, _deviceModelChangeTime);
            _mtconnectVersion = _configuration != null ? _configuration.DefaultVersion : MTConnectVersions.Max;
            _deviceBuffer = deviceBuffer != null ? deviceBuffer : new MTConnectDeviceBuffer();
            _observationBuffer = observationBuffer != null ? observationBuffer : new MTConnectObservationBuffer(_configuration);
            _assetBuffer = assetBuffer != null ? assetBuffer : new MTConnectAssetBuffer(_configuration);
            _metrics.DeviceMetricsUpdated += DeviceMetricsUpdated;
            if (initializeAgentDevice) InitializeAgentDevice();
            StartAgentInformationUpdateTimer();
        }


        public void Dispose()
        {
            if (_metrics != null) _metrics.Dispose();
            StopAgentInformationUpdateTimer();
        }


        private static long CreateInstanceId()
        {
            return UnixDateTime.Now / 1000;
        }

        private static Version GetAgentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }


        #region "Headers"

        private MTConnectDevicesHeader GetDevicesHeader(Version mtconnectVersion = null)
        {
            var version = mtconnectVersion != null ? mtconnectVersion : Version;

            var header = new MTConnectDevicesHeader
            {
                BufferSize = _observationBuffer.BufferSize,
                AssetBufferSize = _assetBuffer.BufferSize,
                AssetCount = _assetBuffer.AssetCount,
                CreationTime = DateTime.UtcNow,
                DeviceModelChangeTime = _deviceModelChangeTime.ToDateTime().ToString("o"),
                InstanceId = InstanceId,
                Sender = System.Net.Dns.GetHostName(),
                Version = GetAgentVersion().ToString(),
                TestIndicator = null
            };

            if (version < MTConnectVersions.Version17) header.DeviceModelChangeTime = null;
            if (version < MTConnectVersions.Version14) header.AssetBufferSize = -1;
            if (version < MTConnectVersions.Version14) header.AssetCount = -1;

            return header;
        }

        private MTConnectStreamsHeader GetStreamsHeader(IStreamingResults results, Version mtconnectVersion = null)
        {
            var v = mtconnectVersion != null ? mtconnectVersion : Version;

            return new MTConnectStreamsHeader
            {
                BufferSize = _observationBuffer.BufferSize,
                CreationTime = DateTime.UtcNow,
                DeviceModelChangeTime = _deviceModelChangeTime.ToDateTime().ToString("o"),
                InstanceId = InstanceId,
                Sender = System.Net.Dns.GetHostName(),
                Version = GetAgentVersion().ToString(),
                FirstSequence = results.FirstSequence,
                LastSequence = results.LastSequence,
                NextSequence = results.NextSequence,
                TestIndicator = null
            };
        }

        private MTConnectAssetsHeader GetAssetsHeader()
        {
            return new MTConnectAssetsHeader
            {
                AssetBufferSize = _assetBuffer.BufferSize,
                AssetCount = _assetBuffer.AssetCount,
                CreationTime = DateTime.UtcNow,
                InstanceId = InstanceId,
                Sender = System.Net.Dns.GetHostName(),
                Version = Version.ToString(),
                TestIndicator = null
            };
        }

        private MTConnectErrorHeader GetErrorHeader()
        {
            return new MTConnectErrorHeader
            {
                AssetBufferSize = _assetBuffer.BufferSize,
                CreationTime = DateTime.UtcNow,
                InstanceId = InstanceId,
                Sender = System.Net.Dns.GetHostName(),
                Version = Version.ToString(),
                TestIndicator = null
            };
        }

        #endregion

        #region "Devices"

        #region "Internal"

        private List<IDevice> ProcessDevices(IEnumerable<IDevice> devices, Version mtconnectVersion = null)
        {
            var objs = new List<IDevice>();

            if (!devices.IsNullOrEmpty())
            {
                foreach (var device in devices)
                {
                    var processedDevice = Device.Process(device, mtconnectVersion != null ? mtconnectVersion : Version);
                    if (processedDevice != null) objs.Add(processedDevice);
                }
            }

            return objs;
        }

        #endregion


        /// <summary>
        /// Get an MTConnectDevices Response Document containing all devices.
        /// </summary>
        /// <returns>MTConnectDevices Response Document</returns>
        public IDevicesResponseDocument GetDevices(Version mtconnectVersion = null)
        {
            DevicesRequestReceived?.Invoke(null);

            if (_deviceBuffer != null)
            {
                var version = mtconnectVersion != null ? mtconnectVersion : Version;

                var devices = new List<IDevice>();
                devices.Add(_agent);
                devices.AddRange(_deviceBuffer.GetDevices());

                if (devices != null && devices.Count() > 0)
                {
                    var doc = new DevicesResponseDocument();
                    doc.Version = version;

                    var header = GetDevicesHeader(version);
                    header.Version = GetAgentVersion().ToString();

                    doc.Header = header;
                    doc.Devices = ProcessDevices(devices, version);

                    DevicesResponseSent?.Invoke(doc);

                    return doc;
                }
            }

            return null;
        }

        /// <summary>
        /// Get an MTConnectDevices Response Document containing all devices.
        /// </summary>
        /// <returns>MTConnectDevices Response Document</returns>
        public async Task<IDevicesResponseDocument> GetDevicesAsync(Version mtconnectVersion = null)
        {
            DevicesRequestReceived?.Invoke(null);

            if (_deviceBuffer != null)
            {
                var version = mtconnectVersion != null ? mtconnectVersion : Version;

                var devices = new List<IDevice>();
                devices.Add(_agent);
                devices.AddRange(await _deviceBuffer.GetDevicesAsync());

                if (devices != null && devices.Count() > 0)
                {
                    var doc = new DevicesResponseDocument();
                    doc.Version = version;

                    var header = GetDevicesHeader(version);
                    header.Version = GetAgentVersion().ToString();

                    doc.Header = header;
                    doc.Devices = ProcessDevices(devices, version);

                    DevicesResponseSent?.Invoke(doc);

                    return doc;
                }
            }

            return null;
        }

        /// <summary>
        /// Get an MTConnectDevices Response Document containing the specified device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <returns>MTConnectDevices Response Document</returns>
        public IDevicesResponseDocument GetDevices(string deviceKey, Version mtconnectVersion = null)
        {
            DevicesRequestReceived?.Invoke(deviceKey);

            if (_deviceBuffer != null && !string.IsNullOrEmpty(deviceKey))
            {
                _deviceKeys.TryGetValue(deviceKey, out var deviceUuid);

                var version = mtconnectVersion != null ? mtconnectVersion : Version;

                IDevice device;
                if (deviceUuid == _agent.Uuid) device = _agent;
                else device = _deviceBuffer.GetDevice(deviceUuid);

                if (device != null)
                {
                    var doc = new DevicesResponseDocument();
                    doc.Version = version;

                    var header = GetDevicesHeader(version);
                    header.Version = GetAgentVersion().ToString();

                    doc.Header = header;
                    doc.Devices = ProcessDevices(new List<IDevice> { device }, version);

                    DevicesResponseSent?.Invoke(doc);

                    return doc;
                }
            }

            return null;
        }

        /// <summary>
        /// Get an MTConnectDevices Response Document containing the specified device.
        /// </summary>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <returns>MTConnectDevices Response Document</returns>
        public async Task<IDevicesResponseDocument> GetDevicesAsync(string deviceKey, Version mtconnectVersion = null)
        {
            DevicesRequestReceived?.Invoke(deviceKey);

            if (_deviceBuffer != null && !string.IsNullOrEmpty(deviceKey))
            {
                _deviceKeys.TryGetValue(deviceKey, out var deviceUuid);

                var version = mtconnectVersion != null ? mtconnectVersion : Version;

                IDevice device;
                if (deviceUuid == _agent.Uuid) device = _agent;
                else device = await _deviceBuffer.GetDeviceAsync(deviceUuid);

                if (device != null)
                {
                    var doc = new DevicesResponseDocument();
                    doc.Version = version;

                    var header = GetDevicesHeader(version);
                    header.Version = GetAgentVersion().ToString();

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

        /// <summary>
        /// Get a MTConnectStreams Document containing all devices.
        /// </summary>
        /// <param name="count">The Maximum Number of DataItems to return</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public IStreamsResponseDocument GetDeviceStreams(int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = new List<IDevice>();
                devices.Add(_agent);
                devices.AddRange(_deviceBuffer.GetDevices());

                if (!devices.IsNullOrEmpty())
                {
                    var deviceUuids = devices.Select(x => x.Uuid);
                    var dataItemIds = new List<string>();

                    // Create list of DataItemIds
                    foreach (var device in devices)
                    {
                        var dataItems = device.GetDataItems();
                        if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));
                    }

                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(deviceUuids, dataItemIds, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
                        return document;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get a MTConnectStreams Document containing all devices.
        /// </summary>
        /// <param name="count">The Maximum Number of DataItems to return</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public async Task<IStreamsResponseDocument> GetDeviceStreamsAsync(int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = new List<IDevice>();
                devices.Add(_agent);
                devices.AddRange(await _deviceBuffer.GetDevicesAsync());

                if (!devices.IsNullOrEmpty())
                {
                    var deviceUuids = devices.Select(x => x.Uuid);
                    var dataItemIds = new List<string>();

                    // Create list of DataItemIds
                    foreach (var device in devices)
                    {
                        var dataItems = device.GetDataItems();
                        if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));
                    }

                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(deviceUuids, dataItemIds, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public IStreamsResponseDocument GetDeviceStreams(long at, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = new List<IDevice>();
                devices.Add(_agent);
                devices.AddRange(_deviceBuffer.GetDevices());

                if (!devices.IsNullOrEmpty())
                {
                    var deviceUuids = devices.Select(x => x.Uuid);
                    var dataItemIds = new List<string>();

                    // Create list of DataItemIds
                    foreach (var device in devices)
                    {
                        var dataItems = device.GetDataItems();
                        if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));
                    }

                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(deviceUuids, dataItemIds, at: at, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public async Task<IStreamsResponseDocument> GetDeviceStreamsAsync(long at, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = new List<IDevice>();
                devices.Add(_agent);
                devices.AddRange(await _deviceBuffer.GetDevicesAsync());

                if (!devices.IsNullOrEmpty())
                {
                    var deviceUuids = devices.Select(x => x.Uuid);
                    var dataItemIds = new List<string>();

                    // Create list of DataItemIds
                    foreach (var device in devices)
                    {
                        var dataItems = device.GetDataItems();
                        if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));
                    }

                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(deviceUuids, dataItemIds, at: at, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public IStreamsResponseDocument GetDeviceStreams(IEnumerable<string> dataItemIds, long at, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = new List<IDevice>();
                devices.Add(_agent);
                devices.AddRange(_deviceBuffer.GetDevices());

                if (!devices.IsNullOrEmpty())
                {
                    var deviceUuids = devices.Select(x => x.Uuid);

                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(deviceUuids, dataItemIds, at: at, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public async Task<IStreamsResponseDocument> GetDeviceStreamsAsync(IEnumerable<string> dataItemIds, long at, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = new List<IDevice>();
                devices.Add(_agent);
                devices.AddRange(await _deviceBuffer.GetDevicesAsync());

                if (!devices.IsNullOrEmpty())
                {
                    var deviceUuids = devices.Select(x => x.Uuid);

                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(deviceUuids, dataItemIds, at: at, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public IStreamsResponseDocument GetDeviceStreams(long from, long to, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = new List<IDevice>();
                devices.Add(_agent);
                devices.AddRange(_deviceBuffer.GetDevices());

                if (!devices.IsNullOrEmpty())
                {
                    var deviceUuids = devices.Select(x => x.Uuid);
                    var dataItemIds = new List<string>();

                    // Create list of DataItemIds
                    foreach (var device in devices)
                    {
                        var dataItems = device.GetDataItems();
                        if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));
                    }

                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(deviceUuids, dataItemIds, from: from, to: to, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public async Task<IStreamsResponseDocument> GetDeviceStreamsAsync(long from, long to, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = new List<IDevice>();
                devices.Add(_agent);
                devices.AddRange(await _deviceBuffer.GetDevicesAsync());

                if (!devices.IsNullOrEmpty())
                {
                    var deviceUuids = devices.Select(x => x.Uuid);
                    var dataItemIds = new List<string>();

                    // Create list of DataItemIds
                    foreach (var device in devices)
                    {
                        var dataItems = device.GetDataItems();
                        if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));
                    }

                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(deviceUuids, dataItemIds, from: from, to: to, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public IStreamsResponseDocument GetDeviceStreams(IEnumerable<string> dataItemIds, long from, long to, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = new List<IDevice>();
                devices.Add(_agent);
                devices.AddRange(_deviceBuffer.GetDevices());

                if (!devices.IsNullOrEmpty())
                {
                    var deviceUuids = devices.Select(x => x.Uuid);

                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(deviceUuids, dataItemIds, from, to, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public async Task<IStreamsResponseDocument> GetDeviceStreamsAsync(IEnumerable<string> dataItemIds, long from, long to, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = new List<IDevice>();
                devices.Add(_agent);
                devices.AddRange(await _deviceBuffer.GetDevicesAsync());

                if (!devices.IsNullOrEmpty())
                {
                    var deviceUuids = devices.Select(x => x.Uuid);

                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(deviceUuids, dataItemIds, from, to, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public IStreamsResponseDocument GetDeviceStream(string deviceKey, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(deviceKey);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                _deviceKeys.TryGetValue(deviceKey, out var deviceUuid);

                // Get Device from the MTConnectDeviceBuffer
                IDevice device;
                if (deviceUuid == _agent.Uuid) device = _agent;
                else device = _deviceBuffer.GetDevice(deviceUuid);

                if (device != null)
                {
                    // Create list of DataItemIds
                    var dataItems = device.GetDataItems();
                    var dataItemIds = new List<string>();
                    if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));

                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(deviceUuid, dataItemIds, count: count);
                    var document = CreateDeviceStreamsDocument(device, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public async Task<IStreamsResponseDocument> GetDeviceStreamAsync(string deviceKey, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(deviceKey);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                _deviceKeys.TryGetValue(deviceKey, out var deviceUuid);

                // Get Device from the MTConnectDeviceBuffer
                IDevice device;
                if (deviceUuid == _agent.Uuid) device = _agent;
                else device = await _deviceBuffer.GetDeviceAsync(deviceUuid);

                if (device != null)
                {
                    // Create list of DataItemIds
                    var dataItems = device.GetDataItems();
                    var dataItemIds = new List<string>();
                    if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));

                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(deviceUuid, dataItemIds, count: count);
                    var document = CreateDeviceStreamsDocument(device, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public IStreamsResponseDocument GetDeviceStream(string deviceKey, long at, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(deviceKey);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                _deviceKeys.TryGetValue(deviceKey, out var deviceUuid);

                // Get Device from the MTConnectDeviceBuffer
                IDevice device;
                if (deviceUuid == _agent.Uuid) device = _agent;
                else device = _deviceBuffer.GetDevice(deviceUuid);

                if (device != null)
                {
                    // Create list of DataItemIds
                    var dataItems = device.GetDataItems();
                    var dataItemIds = new List<string>();
                    if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));

                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(deviceUuid, dataItemIds, at: at, count: count);
                    var document = CreateDeviceStreamsDocument(device, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public async Task<IStreamsResponseDocument> GetDeviceStreamAsync(string deviceKey, long at, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(deviceKey);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                _deviceKeys.TryGetValue(deviceKey, out var deviceUuid);

                // Get Device from the MTConnectDeviceBuffer
                IDevice device;
                if (deviceUuid == _agent.Uuid) device = _agent;
                else device = await _deviceBuffer.GetDeviceAsync(deviceUuid);

                if (device != null)
                {
                    // Create list of DataItemIds
                    var dataItems = device.GetDataItems();
                    var dataItemIds = new List<string>();
                    if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));

                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(deviceUuid, dataItemIds, at: at, count: count);
                    var document = CreateDeviceStreamsDocument(device, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public IStreamsResponseDocument GetDeviceStream(string deviceKey, IEnumerable<string> dataItemIds, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(deviceKey);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                _deviceKeys.TryGetValue(deviceKey, out var deviceUuid);

                // Get Device from the MTConnectDeviceBuffer
                IDevice device;
                if (deviceUuid == _agent.Uuid) device = _agent;
                else device = _deviceBuffer.GetDevice(deviceUuid);

                if (device != null)
                {
                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(deviceUuid, dataItemIds, count: count);
                    var document = CreateDeviceStreamsDocument(device, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public async Task<IStreamsResponseDocument> GetDeviceStreamAsync(string deviceKey, IEnumerable<string> dataItemIds, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(deviceKey);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                _deviceKeys.TryGetValue(deviceKey, out var deviceUuid);

                // Get Device from the MTConnectDeviceBuffer
                IDevice device;
                if (deviceUuid == _agent.Uuid) device = _agent;
                else device = await _deviceBuffer.GetDeviceAsync(deviceUuid);

                if (device != null)
                {
                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(deviceUuid, dataItemIds, count: count);
                    var document = CreateDeviceStreamsDocument(device, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public IStreamsResponseDocument GetDeviceStream(string deviceKey, IEnumerable<string> dataItemIds, long at, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(deviceKey);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                _deviceKeys.TryGetValue(deviceKey, out var deviceUuid);

                // Get Device from the MTConnectDeviceBuffer
                IDevice device;
                if (deviceUuid == _agent.Uuid) device = _agent;
                else device = _deviceBuffer.GetDevice(deviceUuid);

                if (device != null)
                {
                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(deviceUuid, dataItemIds, at: at, count: count);
                    var document = CreateDeviceStreamsDocument(device, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public async Task<IStreamsResponseDocument> GetDeviceStreamAsync(string deviceKey, IEnumerable<string> dataItemIds, long at, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(deviceKey);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                _deviceKeys.TryGetValue(deviceKey, out var deviceUuid);

                // Get Device from the MTConnectDeviceBuffer
                IDevice device;
                if (deviceUuid == _agent.Uuid) device = _agent;
                else device = await _deviceBuffer.GetDeviceAsync(deviceUuid);

                if (device != null)
                {
                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(deviceUuid, dataItemIds, at: at, count: count);
                    var document = CreateDeviceStreamsDocument(device, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public IStreamsResponseDocument GetDeviceStream(string deviceKey, long from, long to, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(deviceKey);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                _deviceKeys.TryGetValue(deviceKey, out var deviceUuid);

                // Get Device from the MTConnectDeviceBuffer
                IDevice device;
                if (deviceUuid == _agent.Uuid) device = _agent;
                else device = _deviceBuffer.GetDevice(deviceUuid);

                if (device != null)
                {
                    // Create list of DataItemIds
                    var dataItems = device.GetDataItems();
                    var dataItemIds = new List<string>();
                    if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));

                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(deviceUuid, dataItemIds, from, to: to, count: count);
                    var document = CreateDeviceStreamsDocument(device, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public async Task<IStreamsResponseDocument> GetDeviceStreamAsync(string deviceKey, long from, long to, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(deviceKey);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                _deviceKeys.TryGetValue(deviceKey, out var deviceUuid);

                // Get Device from the MTConnectDeviceBuffer
                IDevice device;
                if (deviceUuid == _agent.Uuid) device = _agent;
                else device = await _deviceBuffer.GetDeviceAsync(deviceUuid);

                if (device != null)
                {
                    // Create list of DataItemIds
                    var dataItems = device.GetDataItems();
                    var dataItemIds = new List<string>();
                    if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));

                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(deviceUuid, dataItemIds, from, to: to, count: count);
                    var document = CreateDeviceStreamsDocument(device, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public IStreamsResponseDocument GetDeviceStream(string deviceKey, IEnumerable<string> dataItemIds, long from, long to, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(deviceKey);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                _deviceKeys.TryGetValue(deviceKey, out var deviceUuid);

                // Get Device from the MTConnectDeviceBuffer
                IDevice device;
                if (deviceUuid == _agent.Uuid) device = _agent;
                else device = _deviceBuffer.GetDevice(deviceUuid);

                if (device != null)
                {
                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(deviceUuid, dataItemIds, from, to: to, count: count);
                    var document = CreateDeviceStreamsDocument(device, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
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
        public async Task<IStreamsResponseDocument> GetDeviceStreamAsync(string deviceKey, IEnumerable<string> dataItemIds, long from, long to, int count = 0, Version mtconnectVersion = null)
        {
            StreamsRequestReceived?.Invoke(deviceKey);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                _deviceKeys.TryGetValue(deviceKey, out var deviceUuid);

                // Get Device from the MTConnectDeviceBuffer
                IDevice device;
                if (deviceUuid == _agent.Uuid) device = _agent;
                else device = await _deviceBuffer.GetDeviceAsync(deviceUuid);

                if (device != null)
                {
                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(deviceUuid, dataItemIds, from, to: to, count: count);
                    var document = CreateDeviceStreamsDocument(device, results, mtconnectVersion);
                    if (document != null)
                    {
                        StreamsResponseSent?.Invoke(document);
                        return document;
                    }
                }
            }

            return null;
        }


        #region "Create"

        private IStreamsResponseDocument CreateDeviceStreamsDocument(IDevice device, IStreamingResults results, Version mtconnectVersion)
        {
            if (device != null)
            {
                return CreateDeviceStreamsDocument(new List<IDevice> { device }, results, mtconnectVersion);
            }

            return null;
        }

        private IStreamsResponseDocument CreateDeviceStreamsDocument(IEnumerable<IDevice> devices, IStreamingResults results, Version mtconnectVersion)
        {
            if (results != null)
            {
                var version = mtconnectVersion != null ? mtconnectVersion : Version;

                // Create list of DeviceStreams to return
                var deviceStreams = new List<IDeviceStream>();
                foreach (var device in devices)
                {
                    // Create a DeviceStream based on the query results from the buffer
                    deviceStreams.Add(CreateDeviceStream(device, results, version));
                }

                if (!deviceStreams.IsNullOrEmpty())
                {
                    // Create MTConnectStreams Document
                    var doc = new StreamsResponseDocument();
                    doc.Version = version;
                    doc.Header = GetStreamsHeader(results, version);
                    doc.Streams = deviceStreams.ToList();

                    return doc;
                }
            }

            return null;
        }

        private IDeviceStream CreateDeviceStream(IDevice device, IStreamingResults dataItemResults, Version mtconnectVersion)
        {
            // Process Device (to check MTConnect Version compatibility
            var deviceObj = Device.Process(device, mtconnectVersion);
            if (deviceObj != null)
            {
                // Create DeviceStream
                var deviceStream = new DeviceStream();
                deviceStream.Name = device.Name;
                deviceStream.Uuid = device.Uuid;
                var componentStreams = new List<IComponentStream>();

                // Get a list of All Components for the Device
                var components = device.GetComponents();
                if (!components.IsNullOrEmpty())
                {
                    foreach (var component in components)
                    {
                        // Process Component (to check MTConnect Version compatibility
                        var componentObj = Component.Process(component, mtconnectVersion);
                        if (componentObj != null)
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
                            var componentStream = new ComponentStream();
                            componentStream.ComponentId = component.Id;
                            componentStream.ComponentType = component.Type;
                            componentStream.Component = component;
                            componentStream.Name = component.Name;
                            componentStream.Uuid = component.Uuid;
                            componentStream.Samples = GetSamples(device.Uuid, dataItemResults, dataItems, mtconnectVersion);
                            componentStream.Events = GetEvents(device.Uuid, dataItemResults, dataItems, mtconnectVersion);
                            componentStream.Conditions = GetConditions(device.Uuid, dataItemResults, dataItems, mtconnectVersion);
                            componentStreams.Add(componentStream);
                        }
                    }
                }

                // Add ComponentStream for Device
                var deviceComponentStream = new ComponentStream();
                deviceComponentStream.ComponentId = device.Id;
                deviceComponentStream.ComponentType = device.Type;
                deviceComponentStream.Component = device;
                deviceComponentStream.Name = device.Name;
                deviceComponentStream.Uuid = device.Uuid;
                deviceComponentStream.Samples = GetSamples(device.Uuid, dataItemResults, device.DataItems, mtconnectVersion);
                deviceComponentStream.Events = GetEvents(device.Uuid, dataItemResults, device.DataItems, mtconnectVersion);
                deviceComponentStream.Conditions = GetConditions(device.Uuid, dataItemResults, device.DataItems, mtconnectVersion);
                componentStreams.Add(deviceComponentStream);

                deviceStream.ComponentStreams = componentStreams;

                return deviceStream;
            }

            return null;
        }


        private IEnumerable<SampleObservation> GetSamples(string deviceUuid, IStreamingResults dataItemResults, IEnumerable<IDataItem> dataItems, Version mtconnectVersion = null)
        {
            var objs = new List<SampleObservation>();

            if (dataItemResults != null && !dataItemResults.Observations.IsNullOrEmpty() && !dataItems.IsNullOrEmpty())
            {
                var filteredDataItems = dataItems.Where(o => o.Category == DataItemCategory.SAMPLE).ToList();
                if (!filteredDataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in filteredDataItems)
                    {
                        // Check Version Compatibility and Create Derived Class (if found)
                        var di = DataItem.Process(dataItem, mtconnectVersion);
                        if (di != null)
                        {
                            // Get list of StoredObservations for the DataItem
                            var observations = dataItemResults.Observations.Where(o => o.DeviceUuid == deviceUuid && o.DataItemId == dataItem.Id);
                            if (!observations.IsNullOrEmpty())
                            {
                                foreach (var observation in observations)
                                {
                                    objs.Add(CreateSample(di, observation));
                                }
                            }
                        }
                    }
                }
            }

            return objs;
        }

        private static SampleObservation CreateSample(IDataItem dataItem, StoredObservation storedObservation)
        {
            var observation = SampleObservation.Create(dataItem.Type, dataItem.Representation);
            observation.DataItem = dataItem;
            observation.SetProperty(nameof(Observation.DataItemId), dataItem.Id);
            observation.SetProperty(nameof(Observation.Representation), dataItem.Representation);
            observation.SetProperty(nameof(Observation.Type), dataItem.Type);
            observation.SetProperty(nameof(Observation.SubType), dataItem.SubType);
            observation.SetProperty(nameof(Observation.Name), dataItem.Name);
            observation.SetProperty(nameof(Observation.CompositionId), dataItem.CompositionId);
            observation.SetProperty(nameof(Observation.Sequence), storedObservation.Sequence);
            observation.SetProperty(nameof(Observation.Timestamp), storedObservation.Timestamp.ToDateTime());
            observation.AddValues(storedObservation.Values);
            return observation;
        }


        private IEnumerable<EventObservation> GetEvents(string deviceUuid, IStreamingResults dataItemResults, IEnumerable<IDataItem> dataItems, Version mtconnectVersion = null)
        {
            var objs = new List<EventObservation>();

            if (dataItemResults != null && !dataItemResults.Observations.IsNullOrEmpty() && !dataItems.IsNullOrEmpty())
            {
                var filteredDataItems = dataItems.Where(o => o.Category == DataItemCategory.EVENT).ToList();
                if (!filteredDataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in filteredDataItems)
                    {
                        // Check Version Compatibility and Create Derived Class (if found)
                        var di = DataItem.Process(dataItem, mtconnectVersion);
                        if (di != null)
                        {
                            // Get list of StoredObservations for the DataItem
                            var observations = dataItemResults.Observations.Where(o => o.DeviceUuid == deviceUuid && o.DataItemId == dataItem.Id);
                            if (!observations.IsNullOrEmpty())
                            {
                                foreach (var observation in observations)
                                {
                                    objs.Add(CreateEvent(di, observation));
                                }
                            }
                        }
                    }
                }
            }

            return objs;
        }

        private static EventObservation CreateEvent(IDataItem dataItem, StoredObservation storedObservation)
        {
            var observation = EventObservation.Create(dataItem.Type, dataItem.Representation);
            observation.DataItem = dataItem;
            observation.SetProperty(nameof(Observation.DataItemId), dataItem.Id);
            observation.SetProperty(nameof(Observation.Representation), dataItem.Representation);
            observation.SetProperty(nameof(Observation.Type), dataItem.Type);
            observation.SetProperty(nameof(Observation.SubType), dataItem.SubType);
            observation.SetProperty(nameof(Observation.Name), dataItem.Name);
            observation.SetProperty(nameof(Observation.CompositionId), dataItem.CompositionId);
            observation.SetProperty(nameof(Observation.Sequence), storedObservation.Sequence);
            observation.SetProperty(nameof(Observation.Timestamp), storedObservation.Timestamp.ToDateTime());
            observation.AddValues(storedObservation.Values);
            return observation;
        }


        private IEnumerable<ConditionObservation> GetConditions(string deviceUuid, IStreamingResults dataItemResults, IEnumerable<IDataItem> dataItems, Version mtconnectVersion = null)
        {
            var objs = new List<ConditionObservation>();

            if (dataItemResults != null && !dataItemResults.Observations.IsNullOrEmpty() && !dataItems.IsNullOrEmpty())
            {
                var filteredDataItems = dataItems.Where(o => o.Category == DataItemCategory.CONDITION);
                if (!filteredDataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in filteredDataItems)
                    {
                        // Check Version Compatibility and Create Derived Class (if found)
                        var di = DataItem.Process(dataItem, mtconnectVersion);
                        if (di != null)
                        {
                            // Get list of StoredObservations for the DataItem
                            var observations = dataItemResults.Observations.Where(o => o.DeviceUuid == deviceUuid && o.DataItemId == dataItem.Id);
                            if (!observations.IsNullOrEmpty())
                            {
                                foreach (var observation in observations)
                                {
                                    if (!observation.Values.IsNullOrEmpty())
                                    {
                                        var condition = CreateCondition(di, observation);
                                        if (condition != null) objs.Add(condition);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return objs;
        }

        private static ConditionObservation CreateCondition(IDataItem dataItem, StoredObservation storedObservation)
        {
            var observation = ConditionObservation.Create(dataItem.Type, dataItem.Representation);

            var levelValue = storedObservation.Values.FirstOrDefault(o => o.Key == ValueKeys.Level).Value?.ToString();
            if (!string.IsNullOrEmpty(levelValue))
            {
                if (Enum.TryParse<ConditionLevel>(levelValue, true, out var level))
                {
                    observation.DataItem = dataItem;
                    observation.SetProperty(nameof(Observation.DataItemId), dataItem.Id);
                    observation.SetProperty(nameof(Observation.Representation), dataItem.Representation);
                    observation.SetProperty(nameof(Observation.Type), dataItem.Type);
                    observation.SetProperty(nameof(Observation.SubType), dataItem.SubType);
                    observation.SetProperty(nameof(Observation.Name), dataItem.Name);
                    observation.SetProperty(nameof(Observation.CompositionId), dataItem.CompositionId);
                    observation.SetProperty(nameof(Observation.Sequence), storedObservation.Sequence);
                    observation.SetProperty(nameof(Observation.Timestamp), storedObservation.Timestamp.ToDateTime());
                    observation.AddValues(storedObservation.Values);

                    return observation;
                }
            }
            else
            {
                // Check if CDATA is only observation set Unavailable
                var cdata = storedObservation.Values.FirstOrDefault(o => o.Key == ValueKeys.CDATA).Value?.ToString();
                if (cdata == Observation.Unavailable)
                {
                    observation.SetProperty(nameof(Observation.Name), dataItem.Id);
                    observation.SetProperty(nameof(Observation.Representation), dataItem.Representation);
                    observation.SetProperty(nameof(Observation.Type), dataItem.Type);
                    observation.SetProperty(nameof(Observation.SubType), dataItem.SubType);
                    observation.SetProperty(nameof(Observation.Name), dataItem.Name);
                    observation.SetProperty(nameof(Observation.Sequence), storedObservation.Sequence);
                    observation.SetProperty(nameof(Observation.Timestamp), storedObservation.Timestamp.ToDateTime());

                    observation.AddValues(storedObservation.Values);

                    return observation;
                }
            }
            return null;
        }

        #endregion

        #endregion

        #region "Assets"

        /// <summary>
        /// Get an MTConnectAssets Document containing all Assets.
        /// </summary>
        /// <param name="type">Defines the type of MTConnect Asset to be returned in the MTConnectAssets Response Document.</param>
        /// <param name="removed">
        /// An attribute that indicates whether the Asset has been removed from a piece of equipment.
        /// If the value of the removed parameter in the query is true, then Asset Documents for Assets that have been marked as removed from a piece of equipment will be included in the Response Document.
        /// If the value of the removed parameter in the query is false, then Asset Documents for Assets that have been marked as removed from a piece of equipment will not be included in the Response Document.
        /// </param>
        /// <param name="count">Defines the maximum number of Asset Documents to return in an MTConnectAssets Response Document.</param>
        /// <returns>MTConnectAssets Response Document</returns>
        public IAssetsResponseDocument GetAssets(string type = null, bool removed = false, int count = 0, Version mtconnectVersion = null)
        {
            AssetsRequestReceived?.Invoke(null);

            if (_assetBuffer != null)
            {
                // Get Assets from AssetsBuffer
                var assets = _assetBuffer.GetAssets(type, removed, count);
                if (!assets.IsNullOrEmpty())
                {
                    // Set MTConnect Version
                    var version = mtconnectVersion != null ? mtconnectVersion : Version;

                    // Process Assets
                    var processedAssets = new List<IAsset>();
                    foreach (var asset in assets)
                    {
                        var processedAsset = asset.Process(version);
                        if (processedAsset != null) processedAssets.Add(processedAsset);
                    }

                    // Create AssetsHeader
                    var header = GetAssetsHeader();
                    header.Version = GetAgentVersion().ToString();
                    header.InstanceId = InstanceId;

                    // Create MTConnectAssets Response Document
                    var document = new AssetsResponseDocument();
                    document.Version = Version;
                    document.Header = header;
                    document.Assets = processedAssets;

                    AssetsResponseSent?.Invoke(document);

                    return document;
                }
            }

            return null;
        }

        /// <summary>
        /// Get an MTConnectAssets Document containing all Assets.
        /// </summary>
        /// <param name="type">Defines the type of MTConnect Asset to be returned in the MTConnectAssets Response Document.</param>
        /// <param name="removed">
        /// An attribute that indicates whether the Asset has been removed from a piece of equipment.
        /// If the value of the removed parameter in the query is true, then Asset Documents for Assets that have been marked as removed from a piece of equipment will be included in the Response Document.
        /// If the value of the removed parameter in the query is false, then Asset Documents for Assets that have been marked as removed from a piece of equipment will not be included in the Response Document.
        /// </param>
        /// <param name="count">Defines the maximum number of Asset Documents to return in an MTConnectAssets Response Document.</param>
        /// <returns>MTConnectAssets Response Document</returns>
        public async Task<IAssetsResponseDocument> GetAssetsAsync(string type = null, bool removed = false, int count = 0, Version mtconnectVersion = null)
        {
            AssetsRequestReceived?.Invoke(null);

            if (_assetBuffer != null)
            {
                // Get Assets from AssetsBuffer
                var assets = await _assetBuffer.GetAssetsAsync(type, removed, count);
                if (!assets.IsNullOrEmpty())
                {
                    // Set MTConnect Version
                    var version = mtconnectVersion != null ? mtconnectVersion : Version;

                    // Process Assets
                    var processedAssets = new List<IAsset>();
                    foreach (var asset in assets)
                    {
                        var processedAsset = asset.Process(version);
                        if (processedAsset != null) processedAssets.Add(processedAsset);
                    }

                    // Create AssetsHeader
                    var header = GetAssetsHeader();
                    header.Version = GetAgentVersion().ToString();
                    header.InstanceId = InstanceId;

                    // Create MTConnectAssets Response Document
                    var document = new AssetsResponseDocument();
                    document.Version = Version;
                    document.Header = header;
                    document.Assets = processedAssets;

                    AssetsResponseSent?.Invoke(document);

                    return document;
                }
            }

            return null;
        }


        /// <summary>
        /// Get an MTConnectAssets Document containing the specified Asset
        /// </summary>
        /// <param name="assetId">The ID of the Asset to include in the response</param>
        /// <returns>MTConnectAssets Response Document</returns>
        public IAssetsResponseDocument GetAsset(string assetId, Version mtconnectVersion = null)
        {
            AssetsRequestReceived?.Invoke(assetId);

            if (_assetBuffer != null)
            {
                // Get Asset from AssetsBuffer
                var asset = _assetBuffer.GetAsset(assetId);
                if (asset != null)
                {
                    // Set MTConnect Version
                    var version = mtconnectVersion != null ? mtconnectVersion : Version;

                    // Process Asset
                    var processedAsset = asset.Process(version);

                    // Create AssetsHeader
                    var header = GetAssetsHeader();
                    header.Version = GetAgentVersion().ToString();
                    header.InstanceId = InstanceId;

                    // Create MTConnectAssets Response Document
                    var document = new AssetsResponseDocument();
                    document.Version = Version;
                    document.Header = header;
                    document.Assets = new List<IAsset> { processedAsset };

                    AssetsResponseSent?.Invoke(document);

                    return document;
                }
            }

            return null;
        }

        /// <summary>
        /// Get an MTConnectAssets Document containing the specified Asset
        /// </summary>
        /// <param name="assetId">The ID of the Asset to include in the response</param>
        /// <returns>MTConnectAssets Response Document</returns>
        public async Task<IAssetsResponseDocument> GetAssetAsync(string assetId, Version mtconnectVersion = null)
        {
            AssetsRequestReceived?.Invoke(assetId);

            if (_assetBuffer != null)
            {
                // Get Asset from AssetsBuffer
                var asset = await _assetBuffer.GetAssetAsync(assetId);
                if (asset != null)
                {
                    // Set MTConnect Version
                    var version = mtconnectVersion != null ? mtconnectVersion : Version;

                    // Process Asset
                    var processedAsset = asset.Process(version);

                    // Create AssetsHeader
                    var header = GetAssetsHeader();
                    header.Version = GetAgentVersion().ToString();
                    header.InstanceId = InstanceId;

                    // Create MTConnectAssets Response Document
                    var document = new AssetsResponseDocument();
                    document.Version = Version;
                    document.Header = header;
                    document.Assets = new List<IAsset> { processedAsset };

                    AssetsResponseSent?.Invoke(document);

                    return document;
                }
            }

            return null;
        }


        /// <summary>
        /// Remove the Asset with the specified Asset ID
        /// </summary>
        /// <param name="assetId">The ID of the Asset to remove</param>
        /// <returns>Returns True if the Asset was successfully removed</returns>
        public bool RemoveAsset(string assetId)
        {
            if (!string.IsNullOrEmpty(assetId) && _assetBuffer != null)
            {
                return _assetBuffer.RemoveAsset(assetId);
            }

            return false;
        }

        /// <summary>
        /// Remove the Asset with the specified Asset ID
        /// </summary>
        /// <param name="assetId">The ID of the Asset to remove</param>
        /// <returns>Returns True if the Asset was successfully removed</returns>
        public async Task<bool> RemoveAssetAsync(string assetId)
        {
            if (!string.IsNullOrEmpty(assetId) && _assetBuffer != null)
            {
                return await _assetBuffer.RemoveAssetAsync(assetId);
            }

            return false;
        }


        /// <summary>
        /// Remove all Assets with the specified Type
        /// </summary>
        /// <param name="assetType">The Type of the Asset(s) to remove</param>
        /// <returns>Returns True if the Asset(s) was successfully removed</returns>
        public bool RemoveAllAssets(string assetType)
        {
            if (!string.IsNullOrEmpty(assetType) && _assetBuffer != null)
            {
                return _assetBuffer.RemoveAllAssets(assetType);
            }

            return false;
        }

        /// <summary>
        /// Remove all Assets with the specified Type
        /// </summary>
        /// <param name="assetType">The Type of the Asset(s) to remove</param>
        /// <returns>Returns True if the Asset(s) was successfully removed</returns>
        public async Task<bool> RemoveAllAssetsAsync(string assetType)
        {
            if (!string.IsNullOrEmpty(assetType) && _assetBuffer != null)
            {
                return await _assetBuffer.RemoveAllAssetsAsync(assetType);
            }

            return false;
        }

        #endregion

        #region "Errors"

        /// <summary>
        /// Get an MTConnectErrors Document containing the specified ErrorCode
        /// </summary>
        /// <param name="errorCode">Provides a descriptive code that indicates the type of error that was encountered by an Agent when attempting to respond to a Request for information.</param>
        /// <param name="cdata">The CDATA for Error contains a textual description of the error and any additional information an Agent is capable of providing regarding a specific error.</param>
        /// <returns>MTConnectError Response Document</returns>
        public IErrorResponseDocument GetError(ErrorCode errorCode, string cdata = null)
        {
            var doc = new ErrorResponseDocument();
            doc.Version = Version;

            var header = GetErrorHeader();
            header.Version = GetAgentVersion().ToString();

            doc.Header = header;
            doc.Errors = new List<Error>
            {
                new Error(errorCode, cdata)
            };

            ErrorResponseSent?.Invoke(doc);

            return doc;
        }

        /// <summary>
        /// Get an MTConnectErrors Document containing the specified ErrorCode
        /// </summary>
        /// <param name="errorCode">Provides a descriptive code that indicates the type of error that was encountered by an Agent when attempting to respond to a Request for information.</param>
        /// <param name="cdata">The CDATA for Error contains a textual description of the error and any additional information an Agent is capable of providing regarding a specific error.</param>
        /// <returns>MTConnectError Response Document</returns>
        public async Task<IErrorResponseDocument> GetErrorAsync(ErrorCode errorCode, string cdata = null)
        {
            var doc = new ErrorResponseDocument();
            doc.Version = Version;

            var header = GetErrorHeader();
            header.Version = GetAgentVersion().ToString();

            doc.Header = header;
            doc.Errors = new List<Error>
            {
                new Error(errorCode, cdata)
            };

            ErrorResponseSent?.Invoke(doc);

            return doc;
        }

        /// <summary>
        /// Get an MTConnectErrors Document containing the specified Errors
        /// </summary>
        /// <param name="errors">A list of Errors to include in the response Document</param>
        /// <returns>MTConnectError Response Document</returns>
        public IErrorResponseDocument GetError(IEnumerable<IError> errors)
        {
            var doc = new ErrorResponseDocument();
            doc.Version = Version;

            var header = GetErrorHeader();
            header.Version = GetAgentVersion().ToString();

            doc.Header = header;
            doc.Errors = errors != null ? errors.ToList() : null;

            ErrorResponseSent?.Invoke(doc);

            return doc;
        }

        /// <summary>
        /// Get an MTConnectErrors Document containing the specified Errors
        /// </summary>
        /// <param name="errors">A list of Errors to include in the response Document</param>
        /// <returns>MTConnectError Response Document</returns>
        public async Task<IErrorResponseDocument> GetErrorAsync(IEnumerable<IError> errors)
        {
            var doc = new ErrorResponseDocument();
            doc.Version = Version;

            var header = GetErrorHeader();
            header.Version = GetAgentVersion().ToString();

            doc.Header = header;
            doc.Errors = errors != null ? errors.ToList() : null;

            ErrorResponseSent?.Invoke(doc);

            return doc;
        }

        #endregion


        #region "Add"

        #region "Internal"

        public void InitializeDataItems(IDevice device, long timestamp = 0)
        {
            if (device != null && _observationBuffer != null)
            {
                // Get All DataItems for the Device
                var dataItems = device.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    // Get all Current Observations for the Device
                    var results = _observationBuffer.GetObservations(device.Uuid, dataItems.Select(o => o.Id));

                    var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

                    foreach (var dataItem in dataItems)
                    {
                        bool exists = false;

                        // Check if the DataItem has an observation
                        if (results != null && !results.Observations.IsNullOrEmpty())
                        {
                            exists = results.Observations.Any(o => o.DataItemId == dataItem.Id);
                        }

                        // If no observation exists, then add an Unavailable observation
                        if (!exists)
                        {
                            var valueType = dataItem.Category == DataItemCategory.CONDITION ? ValueKeys.Level : ValueKeys.CDATA;
                            var value = !string.IsNullOrEmpty(dataItem.InitialValue) ? dataItem.InitialValue : Observation.Unavailable;

                            // Add Unavailable Observation to ObservationBuffer
                            var observation = new Observation();
                            observation.SetProperty(nameof(Observation.DeviceUuid), device.Uuid);
                            observation.SetProperty(nameof(Observation.DataItemId), dataItem.Id);
                            observation.SetProperty(nameof(Observation.Timestamp), ts.ToDateTime());
                            observation.AddValues(new List<ObservationValue>
                            {
                                new ObservationValue(valueType, value)
                            });

                            _observationBuffer.AddObservation(device.Uuid, dataItem, observation);

                            ObservationAdded?.Invoke(this, observation);
                        }
                    }
                }
            }
        }

        public async Task InitializeDataItemsAsync(IDevice device, long timestamp = 0)
        {
            if (device != null && _observationBuffer != null)
            {
                // Get All DataItems for the Device
                var dataItems = device.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    // Get all Current Observations for the Device
                    var results = await _observationBuffer.GetObservationsAsync(device.Uuid, dataItems.Select(o => o.Id));

                    var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

                    foreach (var dataItem in dataItems)
                    {
                        bool exists = false;

                        // Check if the DataItem has an observation
                        if (results != null && !results.Observations.IsNullOrEmpty())
                        {
                            exists = results.Observations.Any(o => o.DataItemId == dataItem.Id);
                        }

                        // If no observation exists, then add an Unavailable observation
                        if (!exists)
                        {
                            var valueType = dataItem.Category == DataItemCategory.CONDITION ? ValueKeys.Level : ValueKeys.CDATA;
                            var value = !string.IsNullOrEmpty(dataItem.InitialValue) ? dataItem.InitialValue : Observation.Unavailable;

                            // Add Unavailable Observation to ObservationBuffer
                            var observation = new Observation();
                            observation.SetProperty(nameof(Observation.DeviceUuid), device.Uuid);
                            observation.SetProperty(nameof(Observation.DataItemId), dataItem.Id);
                            observation.SetProperty(nameof(Observation.Timestamp), ts.ToDateTime());
                            observation.AddValues(new List<ObservationValue>
                            {
                                new ObservationValue(valueType, value)
                            });

                            await _observationBuffer.AddObservationAsync(device.Uuid, dataItem, observation);

                            ObservationAdded?.Invoke(this, observation);
                        }
                    }
                }
            }
        }

        private bool UpdateCurrentObservation(string deviceUuid, IDataItem dataItem, IObservationInput observation)
        {
            if (_currentObservations != null && observation != null && !string.IsNullOrEmpty(deviceUuid) && dataItem != null)
            {
                var hash = StoredObservation.CreateHash(deviceUuid, dataItem.Id);

                _currentObservations.TryGetValue(hash, out var existingObservation);
                if (observation != null && existingObservation != null)
                {
                    // Check Filters
                    var update = FilterPeriod(dataItem, observation.Timestamp, existingObservation.Timestamp);
                    if (update) update = FilterDelta(dataItem, observation, existingObservation);

                    // Update if Filters are passed or if the DataItem is set to Discrete
                    if (update || dataItem.Discrete)
                    {
                        _currentObservations.TryRemove(hash, out var _);
                        return _currentObservations.TryAdd(hash, observation);
                    }
                }
                else
                {
                    _currentObservations.TryRemove(hash, out var _);
                    return _currentObservations.TryAdd(hash, observation);
                }
            }

            return false;
        }

        private bool UpdateCurrentCondition(string deviceUuid, IDataItem dataItem, IObservationInput observation)
        {
            if (_currentConditions != null && observation != null && !string.IsNullOrEmpty(deviceUuid) && dataItem != null)
            {
                var hash = StoredObservation.CreateHash(deviceUuid, dataItem.Id);
                var observations = new List<IObservationInput>();
                observations.Add(observation);

                string existingHash = null;

                _currentConditions.TryGetValue(hash, out var existingObservations);
                if (observation != null && !existingObservations.IsNullOrEmpty())
                {
                    existingHash = StringFunctions.ToMD5Hash(existingObservations.Select(o => o.ChangeId).ToArray());

                    var conditionLevel = observation.GetValue(ValueKeys.Level);
                    if (conditionLevel != ConditionLevel.NORMAL.ToString() && 
                        conditionLevel != ConditionLevel.UNAVAILABLE.ToString())
                    {
                        observations.InsertRange(0, existingObservations);
                    }
                }

                string newHash = StringFunctions.ToMD5Hash(observations.Select(o => o.ChangeId).ToArray());

                if (newHash != existingHash)
                {
                    _currentConditions.TryRemove(hash, out var _);
                    return _currentConditions.TryAdd(hash, observations);
                }
            }

            return false;
        }

        private static bool FilterPeriod(IDataItem dataItem, long newTimestamp, long existingTimestamp)
        {
            if (dataItem != null)
            {
                if (newTimestamp > existingTimestamp)
                {
                    if (!dataItem.Filters.IsNullOrEmpty())
                    {
                        foreach (var filter in dataItem.Filters)
                        {
                            if (filter.Type == DataItemFilterType.PERIOD)
                            {
                                if (filter.Value > 0)
                                {
                                    // Get Period based on Seconds specified in Filter
                                    var period = TimeSpan.FromSeconds(filter.Value);

                                    // Get Duration between newTimestamp and existingTimestamp
                                    var duration = TimeSpan.FromMilliseconds(newTimestamp - existingTimestamp);

                                    return duration > period;
                                }
                            }
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        private static bool FilterDelta(IDataItem dataItem, IObservationInput newObservation, IObservationInput existingObservation)
        {
            if (dataItem != null)
            {
                if (newObservation.ChangeId != existingObservation.ChangeId)
                {
                    if (!dataItem.Filters.IsNullOrEmpty() && dataItem.Representation == DataItemRepresentation.VALUE)
                    {
                        foreach (var filter in dataItem.Filters)
                        {
                            if (filter.Type == DataItemFilterType.MINIMUM_DELTA)
                            {
                                if (filter.Value > 0)
                                {
                                    var x = newObservation.GetValue(ValueKeys.CDATA).ToDouble();
                                    var y = existingObservation.GetValue(ValueKeys.CDATA).ToDouble();

                                    // If difference between New and Existing exceeds Filter Minimum Delta Value
                                    return Math.Abs(x - y) > filter.Value;
                                }
                            }
                        }
                    }

                    return true;
                }
            }

            return false;
        }


        private IDataItem GetDataItemFromKey(string deviceUuid, string key)
        {
            if (!string.IsNullOrEmpty(deviceUuid) && !string.IsNullOrEmpty(key))
            {
                // Get Device From DeviceBuffer
                var device = _deviceBuffer.GetDevice(deviceUuid);
                if (device == null && deviceUuid == _uuid) device = _agent;
                return GetDataItemFromKey(device, key);
            }

            return null;
        }

        private async Task<IDataItem> GetDataItemFromKeyAsync(string deviceUuid, string key)
        {
            if (!string.IsNullOrEmpty(deviceUuid) && !string.IsNullOrEmpty(key))
            {
                // Get Device From DeviceBuffer
                var device = await _deviceBuffer.GetDeviceAsync(deviceUuid);
                if (device == null && deviceUuid == _uuid) device = _agent;
                return GetDataItemFromKey(device, key);
            }

            return null;
        }

        private IDataItem GetDataItemFromKey(IDevice device, string key)
        {
            if (device != null)
            {
                // Get list of all DataItems for Device
                var dataItems = device.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    // Check DataItem ID
                    var dataItem = dataItems.FirstOrDefault(o => o.Id == key);

                    // Check DataItem Name
                    if (dataItem == null) dataItem = dataItems.FirstOrDefault(o => o.Name == key);

                    // Check DataItem Source DataItemId
                    if (dataItem == null) dataItem = dataItems.FirstOrDefault(o => o.Source != null && o.Source.DataItemId == key);

                    // Check DataItem Source CDATA
                    if (dataItem == null) dataItem = dataItems.FirstOrDefault(o => o.Source != null && o.Source.CDATA == key);

                    // Return DataItem
                    return dataItem;
                }
            }

            return null;
        }

        #endregion


        #region "Devices"

        #region "Internal"

        private Device NormalizeDevice(IDevice device)
        {
            if (device != null)
            {
                Device obj = null;

                if (device.Type == Device.TypeId) obj = new Device();
                else if (device.Type == Agent.TypeId) obj = new Agent();

                if (obj != null)
                {
                    obj.Id = device.Id;
                    obj.Name = device.Name;
                    obj.NativeName = device.NativeName;
                    obj.Uuid = device.Uuid;
                    obj.Type = device.Type;
                    obj.SampleRate = device.SampleRate;
                    obj.SampleInterval = device.SampleInterval;
                    obj.Iso841Class = device.Iso841Class;
                    obj.CoordinateSystemIdRef = device.CoordinateSystemIdRef;
                    obj.MTConnectVersion = device.MTConnectVersion;
                    obj.Configuration = device.Configuration;
                    obj.References = device.References;
                    obj.Description = NormalizeDescription(device.Description);
                    obj.DataItems = NormalizeDataItems(device.DataItems);
                    obj.Compositions = NormalizeCompositions(device.Compositions);
                    obj.Components = NormalizeComponents(device.Components);

                    // Add Required Availability DataItem
                    if (obj.DataItems.IsNullOrEmpty() || !obj.DataItems.Any(o => o.Type == AvailabilityDataItem.TypeId))
                    {
                        var availability = new AvailabilityDataItem(obj.Id);
                        availability.Name = AvailabilityDataItem.NameId;
                        var x = obj.DataItems.ToList();
                        x.Add(availability);
                        obj.DataItems = x;
                    }

                    // Add Required AssetChanged DataItem
                    if (obj.DataItems.IsNullOrEmpty() || !obj.DataItems.Any(o => o.Type == AssetChangedDataItem.TypeId))
                    {
                        var assetChanged = new AssetChangedDataItem(obj.Id);
                        assetChanged.Name = AssetChangedDataItem.NameId;
                        var x = obj.DataItems.ToList();
                        x.Add(assetChanged);
                        obj.DataItems = x;
                    }

                    // Add Required AssetRemoved DataItem
                    if (obj.DataItems.IsNullOrEmpty() || !obj.DataItems.Any(o => o.Type == AssetRemovedDataItem.TypeId))
                    {
                        var assetRemoved = new AssetRemovedDataItem(obj.Id);
                        assetRemoved.Name = AssetRemovedDataItem.NameId;
                        var x = obj.DataItems.ToList();
                        x.Add(assetRemoved);
                        obj.DataItems = x;
                    }

                    return obj;
                } 
            }

            return null;
        }

        private List<IComponent> NormalizeComponents(IEnumerable<IComponent> components)
        {
            if (!components.IsNullOrEmpty())
            {
                var objs = new List<IComponent>();

                foreach (var component in components)
                {
                    var obj = Component.Create(component.Type);
                    obj.Id = component.Id;
                    obj.Uuid = component.Uuid;
                    obj.Name = component.Name;
                    obj.NativeName = component.NativeName;
                    obj.Type = component.Type;
                    obj.Description = component.Description;
                    obj.SampleRate = component.SampleRate;
                    obj.SampleInterval = component.SampleInterval;
                    obj.References = component.References;
                    obj.Configuration = component.Configuration;

                    obj.Components = NormalizeComponents(component.Components);
                    obj.Compositions = NormalizeCompositions(component.Compositions);
                    obj.DataItems = NormalizeDataItems(component.DataItems);

                    objs.Add(obj);
                }

                return objs;
            }

            return new List<IComponent>();
        }

        private List<IComposition> NormalizeCompositions(IEnumerable<IComposition> compositions)
        {
            if (!compositions.IsNullOrEmpty())
            {
                var objs = new List<IComposition>();

                foreach (var composition in compositions)
                {
                    var obj = Composition.Create(composition.Type);
                    obj.Id = composition.Id;
                    obj.Uuid = composition.Uuid;
                    obj.Name = composition.Name;
                    obj.NativeName = composition.NativeName;
                    obj.Type = composition.Type;
                    obj.Description = composition.Description;
                    obj.SampleRate = composition.SampleRate;
                    obj.SampleInterval = composition.SampleInterval;
                    obj.References = composition.References;
                    obj.Configuration = composition.Configuration;

                    obj.DataItems = NormalizeDataItems(composition.DataItems);

                    objs.Add(obj);
                }

                return objs;
            }

            return new List<IComposition>();
        }

        private List<IDataItem> NormalizeDataItems(IEnumerable<IDataItem> dataItems)
        {
            if (!dataItems.IsNullOrEmpty())
            {
                var objs = new List<IDataItem>();

                foreach (var dataItem in dataItems)
                {
                    var obj = DataItem.Create(dataItem.Type);
                    obj.Category = dataItem.Category;
                    obj.Id = dataItem.Id;
                    obj.Name = dataItem.Name;
                    obj.Type = dataItem.Type;
                    obj.SubType = dataItem.SubType;
                    obj.NativeUnits = dataItem.NativeUnits;
                    obj.NativeScale = dataItem.NativeScale;
                    obj.SampleRate = dataItem.SampleRate;
                    obj.Source = dataItem.Source;
                    obj.CompositionId = dataItem.CompositionId;
                    obj.Relationships = dataItem.Relationships;
                    obj.Representation = dataItem.Representation;
                    obj.ResetTrigger = dataItem.ResetTrigger;
                    obj.CoordinateSystem = dataItem.CoordinateSystem;
                    obj.Constraints = dataItem.Constraints;
                    obj.Definition = dataItem.Definition;
                    obj.Units = dataItem.Units;
                    obj.Statistic = dataItem.Statistic;
                    obj.SignificantDigits = dataItem.SignificantDigits;
                    obj.Filters = dataItem.Filters;
                    obj.InitialValue = dataItem.InitialValue;
                    obj.Discrete = dataItem.Discrete;

                    objs.Add(obj);
                }

                return objs;
            }

            return new List<IDataItem>();
        }

        private Description NormalizeDescription(IDescription description)
        {
            if (description != null)
            {
                var obj = new Description();
                obj.Manufacturer = !string.IsNullOrEmpty(description.Manufacturer) ? description.Manufacturer.Trim() : null;
                obj.Model = !string.IsNullOrEmpty(description.Model) ? description.Model.Trim() : null;
                obj.SerialNumber = !string.IsNullOrEmpty(description.SerialNumber) ? description.SerialNumber.Trim() : null;
                obj.Station = !string.IsNullOrEmpty(description.Station) ? description.Station.Trim() : null;
                obj.CDATA = !string.IsNullOrEmpty(description.CDATA) ? description.CDATA.Trim() : null;
                return obj;
            }

            return null;
        }



        private bool AddDeviceAddedObservation(IDevice device, long timestamp = 0)
        {
            if (_agent != null && device != null && _observationBuffer != null)
            {
                var dataItems = _agent.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    var dataItem = dataItems.FirstOrDefault(o => o.Type == DeviceAddedDataItem.TypeId);
                    if (dataItem != null)
                    {
                        // Create new Observation
                        var observation = new Observation();
                        observation.SetProperty(nameof(Observation.DeviceUuid), _agent.Uuid);
                        observation.SetProperty(nameof(Observation.DataItemId), dataItem.Id);
                        observation.SetProperty(nameof(Observation.Timestamp), timestamp.ToDateTime());
                        observation.AddValues(new List<ObservationValue>
                        {
                            new ObservationValue(ValueKeys.CDATA, device.Uuid)
                        });

                        ObservationAdded?.Invoke(this, observation);

                        // Add to Streaming Buffer
                        return _observationBuffer.AddObservation(_agent.Uuid, dataItem, observation);
                    }
                }
            }

            return false;
        }

        private async Task<bool> AddDeviceAddedObservationAsync(IDevice device, long timestamp = 0)
        {
            if (_agent != null && device != null && _observationBuffer != null)
            {
                var dataItems = _agent.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    var dataItem = dataItems.FirstOrDefault(o => o.Type == DeviceAddedDataItem.TypeId);
                    if (dataItem != null)
                    {
                        // Create new Observation
                        var observation = new Observation();
                        observation.SetProperty(nameof(Observation.DeviceUuid), _agent.Uuid);
                        observation.SetProperty(nameof(Observation.DataItemId), dataItem.Id);
                        observation.SetProperty(nameof(Observation.Timestamp), timestamp.ToDateTime());
                        observation.AddValues(new List<ObservationValue>
                        {
                            new ObservationValue(ValueKeys.CDATA, device.Uuid)
                        });

                        ObservationAdded?.Invoke(this, observation);

                        // Add to Streaming Buffer
                        return await _observationBuffer.AddObservationAsync(_agent.Uuid, dataItem, observation);
                    }
                }
            }

            return false;
        }


        private bool AddDeviceChangedObservation(IDevice device, long timestamp = 0)
        {
            if (_agent != null && device != null && _observationBuffer != null)
            {
                var dataItems = _agent.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    var dataItem = dataItems.FirstOrDefault(o => o.Type == DeviceChangedDataItem.TypeId);
                    if (dataItem != null)
                    {
                        // Create new Observation
                        var observation = new Observation();
                        observation.SetProperty(nameof(Observation.DeviceUuid), _agent.Uuid);
                        observation.SetProperty(nameof(Observation.DataItemId), dataItem.Id);
                        observation.SetProperty(nameof(Observation.Timestamp), timestamp.ToDateTime());
                        observation.AddValues(new List<ObservationValue>
                        {
                            new ObservationValue(ValueKeys.CDATA, device.Uuid)
                        });

                        ObservationAdded?.Invoke(this, observation);

                        // Add to Streaming Buffer
                        return _observationBuffer.AddObservation(_agent.Uuid, dataItem, observation);
                    }
                }
            }

            return false;
        }

        private async Task<bool> AddDeviceChangedObservationAsync(IDevice device, long timestamp = 0)
        {
            if (_agent != null && device != null && _observationBuffer != null)
            {
                var dataItems = _agent.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    var dataItem = dataItems.FirstOrDefault(o => o.Type == DeviceChangedDataItem.TypeId);
                    if (dataItem != null)
                    {
                        // Create new Observation
                        var observation = new Observation();
                        observation.SetProperty(nameof(Observation.DeviceUuid), _agent.Uuid);
                        observation.SetProperty(nameof(Observation.DataItemId), dataItem.Id);
                        observation.SetProperty(nameof(Observation.Timestamp), timestamp.ToDateTime());
                        observation.AddValues(new List<ObservationValue>
                        {
                            new ObservationValue(ValueKeys.CDATA, device.Uuid)
                        });

                        ObservationAdded?.Invoke(this, observation);

                        // Add to Streaming Buffer
                        return await _observationBuffer.AddObservationAsync(_agent.Uuid, dataItem, observation);
                    }
                }
            }

            return false;
        }


        private bool AddDeviceRemovedObservation(IDevice device, long timestamp = 0)
        {
            if (_agent != null && device != null && _observationBuffer != null)
            {
                var dataItems = _agent.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    var dataItem = dataItems.FirstOrDefault(o => o.Type == DeviceRemovedDataItem.TypeId);
                    if (dataItem != null)
                    {
                        // Create new Observation
                        var observation = new Observation();
                        observation.SetProperty(nameof(Observation.DeviceUuid), _agent.Uuid);
                        observation.SetProperty(nameof(Observation.DataItemId), dataItem.Id);
                        observation.SetProperty(nameof(Observation.Timestamp), timestamp.ToDateTime());
                        observation.AddValues(new List<ObservationValue>
                        {
                            new ObservationValue(ValueKeys.CDATA, device.Uuid)
                        });

                        ObservationAdded?.Invoke(this, observation);

                        // Add to Streaming Buffer
                        return _observationBuffer.AddObservation(_agent.Uuid, dataItem, observation);
                    }
                }
            }

            return false;
        }

        private async Task<bool> AddDeviceRemovedObservationAsync(IDevice device, long timestamp = 0)
        {
            if (_agent != null && device != null && _observationBuffer != null)
            {
                var dataItems = _agent.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    var dataItem = dataItems.FirstOrDefault(o => o.Type == DeviceRemovedDataItem.TypeId);
                    if (dataItem != null)
                    {
                        // Create new Observation
                        var observation = new Observation();
                        observation.SetProperty(nameof(Observation.DeviceUuid), _agent.Uuid);
                        observation.SetProperty(nameof(Observation.DataItemId), dataItem.Id);
                        observation.SetProperty(nameof(Observation.Timestamp), timestamp.ToDateTime());
                        observation.AddValues(new List<ObservationValue>
                        {
                            new ObservationValue(ValueKeys.CDATA, device.Uuid)
                        });

                        ObservationAdded?.Invoke(this, observation);

                        // Add to Streaming Buffer
                        return await _observationBuffer.AddObservationAsync(_agent.Uuid, dataItem, observation);
                    }
                }
            }

            return false;
        }

        #endregion


        /// <summary>
        /// Add a new MTConnectDevice to the Agent's Buffer
        /// </summary>
        public bool AddDevice(IDevice device, bool initializeDataItems = true)
        {
            if (device != null && _deviceBuffer != null)
            {
                // Create new object (to validate and prevent derived classes that won't serialize right with XML)
                var obj = NormalizeDevice(device);

                // Get Existing Device (if exists)
                var existingDevice = _deviceBuffer.GetDevice(obj.Uuid);

                // Check if Device Already Exists in the Device Buffer and is changed
                if (existingDevice != null && obj.ChangeId == existingDevice.ChangeId)
                {
                    return true;
                }

                // Add the Device to the Buffer
                var success = _deviceBuffer.AddDevice(obj);
                if (success)
                {
                    // Add Name and UUID to DeviceKey dictionary
                    _deviceKeys.TryAdd(obj.Name, obj.Uuid);
                    _deviceKeys.TryAdd(obj.Uuid, obj.Uuid);

                    if (initializeDataItems)
                    {
                        var timestamp = UnixDateTime.Now;

                        if (existingDevice != null)
                        {
                            //AddDeviceRemovedObservation(obj);
                            AddDeviceChangedObservation(obj, timestamp);
                        }
                        else
                        {
                            AddDeviceAddedObservation(obj, timestamp);
                        }

                        InitializeDataItems(obj);

                        _deviceModelChangeTime = timestamp;
                        _updateInformation = true;
                    }

                    DeviceAdded?.Invoke(this, obj);
                }

                return success;
            }

            return false;
        }

        /// <summary>
        /// Add a new MTConnectDevice to the Agent's Buffer
        /// </summary>
        public async Task<bool> AddDeviceAsync(IDevice device, bool initializeDataItems = true)
        {
            if (device != null && _deviceBuffer != null)
            {
                // Create new object (to validate and prevent derived classes that won't serialize right with XML)
                var obj = NormalizeDevice(device);

                // Get Existing Device (if exists)
                var existingDevice = await _deviceBuffer.GetDeviceAsync(obj.Uuid);

                // Check if Device Already Exists in the Device Buffer and is changed
                if (existingDevice != null && obj.ChangeId == existingDevice.ChangeId)
                {
                    return true;
                }

                // Add the Device to the Buffer
                var success = await _deviceBuffer.AddDeviceAsync(obj);
                if (success)
                {
                    // Add Name and UUID to DeviceKey dictionary
                    _deviceKeys.TryAdd(obj.Name, obj.Uuid);
                    _deviceKeys.TryAdd(obj.Uuid, obj.Uuid);

                    if (initializeDataItems)
                    {
                        var timestamp = UnixDateTime.Now;

                        if (existingDevice != null)
                        {
                            //await AddDeviceRemovedObservationAsync(obj);
                            await AddDeviceChangedObservationAsync(obj, timestamp);
                        }
                        else
                        {
                            await AddDeviceAddedObservationAsync(obj, timestamp);
                        }

                        await InitializeDataItemsAsync(obj);

                        _deviceModelChangeTime = timestamp;
                        _updateInformation = true;
                    }

                    DeviceAdded?.Invoke(this, obj);
                }

                return success;
            }

            return false;
        }

        /// <summary>
        /// Add new MTConnectDevices to the Agent's Buffer
        /// </summary>
        public bool AddDevices(IEnumerable<IDevice> devices, bool initializeDataItems = true)
        {
            if (!devices.IsNullOrEmpty() && _deviceBuffer != null)
            {
                bool success = false;

                foreach (var device in devices)
                {
                    success = AddDevice(device, initializeDataItems);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        /// <summary>
        /// Add new MTConnectDevices to the Agent's Buffer
        /// </summary>
        public async Task<bool> AddDevicesAsync(IEnumerable<IDevice> devices, bool initializeDataItems = true)
        {
            if (!devices.IsNullOrEmpty() && _deviceBuffer != null)
            {
                bool success = false;

                foreach (var device in devices)
                {
                    success = await AddDeviceAsync(device, initializeDataItems);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        #endregion

        #region "Observations"

        #region "Internal"

        private IObservationInput ConvertObservationValue(IDataItem dataItem, IObservationInput observation)
        {
            if (dataItem != null && observation != null)
            {
                if (dataItem.Category == DataItemCategory.SAMPLE)
                {
                    // Get the CDATA Value
                    var cdata = observation.GetValue(ValueKeys.CDATA);
                    if (!string.IsNullOrEmpty(cdata) && cdata != Observation.Unavailable)
                    {
                        var units = dataItem.Units;
                        var nativeUnits = dataItem.NativeUnits;

                        // Get the SampleValue for the DataItem Type
                        if (dataItem.Units == Units.DEGREE_3D)
                        {
                            // Remove the "_3D" suffix from the Units and NativeUnits
                            units = Remove3dSuffix(units);
                            nativeUnits = Remove3dSuffix(nativeUnits);

                            // Create a new Degree3D object to parse the CDATA
                            var degree3d = Degree3D.FromString(cdata);
                            if (degree3d != null)
                            {
                                degree3d.A = Observation.ConvertUnits(degree3d.A, units, nativeUnits);
                                degree3d.B = Observation.ConvertUnits(degree3d.B, units, nativeUnits);
                                degree3d.C = Observation.ConvertUnits(degree3d.C, units, nativeUnits);

                                // Apply the NativeScale
                                if (dataItem.NativeScale > 0)
                                {
                                    degree3d.A = degree3d.A / dataItem.NativeScale;
                                    degree3d.B = degree3d.B / dataItem.NativeScale;
                                    degree3d.C = degree3d.C / dataItem.NativeScale;
                                }

                                // Convert _3D back to string using the appropriate format and set to CDATA
                                cdata = degree3d.ToString();
                            }
                        }
                        else if (dataItem.Units == Units.MILLIMETER_3D || dataItem.Units == Units.UNIT_VECTOR_3D)
                        {
                            // Remove the "_3D" suffix from the Units and NativeUnits
                            units = Remove3dSuffix(units);
                            nativeUnits = Remove3dSuffix(nativeUnits);

                            // Create a new Position3D object to parse the CDATA
                            var position3d = Position3D.FromString(cdata);
                            if (position3d != null)
                            {
                                position3d.X = Observation.ConvertUnits(position3d.X, units, nativeUnits);
                                position3d.Y = Observation.ConvertUnits(position3d.Y, units, nativeUnits);
                                position3d.Z = Observation.ConvertUnits(position3d.Z, units, nativeUnits);

                                // Apply the NativeScale
                                if (dataItem.NativeScale > 0)
                                {
                                    position3d.X = position3d.X / dataItem.NativeScale;
                                    position3d.Y = position3d.Y / dataItem.NativeScale;
                                    position3d.Z = position3d.Z / dataItem.NativeScale;
                                }

                                // Convert _3D back to string using the appropriate format and set CDATA
                                cdata = position3d.ToString();
                            }
                        }
                        else
                        {
                            // Directly convert the Units if no SampleValue class is found
                            var value = Observation.ConvertUnits(cdata.ToDouble(), units, nativeUnits);

                            // Apply the NativeScale
                            if (dataItem.NativeScale > 0) value = value / dataItem.NativeScale;

                            // Set CDATA to value
                            cdata = value.ToString();
                        }

                        // Replace the CDATA value in the Observation
                        observation.AddValue(ValueKeys.CDATA, cdata);
                    }
                }
            }

            return observation;
        }

        private string Remove3dSuffix(string s)
        {
            var i = s.IndexOf("_3D");
            if (i >= 0)
            {
                s = s.Substring(0, i);
            }
            return s;
        }

        #endregion


        /// <summary>
        /// Add a new Observation for a DataItem of category EVENT or SAMPLE to the Agent
        /// </summary>
        public bool AddObservation(string deviceKey, string dataItemKey, object value)
        {
            return AddObservation(deviceKey, new ObservationInput
            {
                DeviceKey = deviceKey,
                DataItemKey = dataItemKey,
                Values = new List<ObservationValue> { new ObservationValue(ValueKeys.CDATA, value) },
                Timestamp = UnixDateTime.Now
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem of category EVENT or SAMPLE to the Agent
        /// </summary>
        public async Task<bool> AddObservationAsync(string deviceKey, string dataItemKey, object value)
        {
            return await AddObservationAsync(deviceKey, new ObservationInput
            {
                DeviceKey = deviceKey,
                DataItemKey = dataItemKey,
                Values = new List<ObservationValue> { new ObservationValue(ValueKeys.CDATA, value) },
                Timestamp = UnixDateTime.Now
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem of category EVENT or SAMPLE to the Agent
        /// </summary>
        public bool AddObservation(string deviceKey, string dataItemKey, object value, long timestamp)
        {
            return AddObservation(deviceKey, new ObservationInput
            {
                DeviceKey = deviceKey,
                DataItemKey = dataItemKey,
                Values = new List<ObservationValue> { new ObservationValue(ValueKeys.CDATA, value) },
                Timestamp = timestamp
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem of category EVENT or SAMPLE to the Agent
        /// </summary>
        public async Task<bool> AddObservationAsync(string deviceKey, string dataItemKey, object value, long timestamp)
        {
            return await AddObservationAsync(deviceKey, new ObservationInput
            {
                DeviceKey = deviceKey,
                DataItemKey = dataItemKey,
                Values = new List<ObservationValue> { new ObservationValue(ValueKeys.CDATA, value) },
                Timestamp = timestamp
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem of category EVENT or SAMPLE to the Agent
        /// </summary>
        public bool AddObservation(string deviceKey, string dataItemKey, object value, DateTime timestamp)
        {
            return AddObservation(deviceKey, new ObservationInput
            {
                DeviceKey = deviceKey,
                DataItemKey = dataItemKey,
                Values = new List<ObservationValue> { new ObservationValue(ValueKeys.CDATA, value) },
                Timestamp = timestamp.ToUnixTime()
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem of category EVENT or SAMPLE to the Agent
        /// </summary>
        public async Task<bool> AddObservationAsync(string deviceKey, string dataItemKey, object value, DateTime timestamp)
        {
            return await AddObservationAsync(deviceKey, new ObservationInput
            {
                DeviceKey = deviceKey,
                DataItemKey = dataItemKey,
                Values = new List<ObservationValue> { new ObservationValue(ValueKeys.CDATA, value) },
                Timestamp = timestamp.ToUnixTime()
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem to the Agent
        /// </summary>
        public bool AddObservation(string deviceKey, string dataItemKey, string valueKey, object value)
        {
            return AddObservation(deviceKey, new ObservationInput
            {
                DeviceKey = deviceKey,
                DataItemKey = dataItemKey,
                Values = new List<ObservationValue> { new ObservationValue(valueKey, value) },
                Timestamp = UnixDateTime.Now
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem to the Agent
        /// </summary>
        public async Task<bool> AddObservationAsync(string deviceKey, string dataItemKey, string valueKey, object value)
        {
            return await AddObservationAsync(deviceKey, new ObservationInput
            {
                DeviceKey = deviceKey,
                DataItemKey = dataItemKey,
                Values = new List<ObservationValue> { new ObservationValue(valueKey, value) },
                Timestamp = UnixDateTime.Now
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem to the Agent
        /// </summary>
        public bool AddObservation(string deviceKey, string dataItemKey, string valueKey, object value, long timestamp)
        {
            return AddObservation(deviceKey, new ObservationInput
            {
                DeviceKey = deviceKey,
                DataItemKey = dataItemKey,
                Values = new List<ObservationValue> { new ObservationValue(valueKey, value) },
                Timestamp = timestamp
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem to the Agent
        /// </summary>
        public async Task<bool> AddObservationAsync(string deviceKey, string dataItemKey, string valueKey, object value, long timestamp)
        {
            return await AddObservationAsync(deviceKey, new ObservationInput
            {
                DeviceKey = deviceKey,
                DataItemKey = dataItemKey,
                Values = new List<ObservationValue> { new ObservationValue(valueKey, value) },
                Timestamp = timestamp
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem to the Agent
        /// </summary>
        public bool AddObservation(string deviceKey, string dataItemKey, string valueKey, object value, DateTime timestamp)
        {
            return AddObservation(deviceKey, new ObservationInput
            {
                DeviceKey = deviceKey,
                DataItemKey = dataItemKey,
                Values = new List<ObservationValue> { new ObservationValue(valueKey, value) },
                Timestamp = timestamp.ToUnixTime()
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem of to the Agent
        /// </summary>
        public async Task<bool> AddObservationAsync(string deviceKey, string dataItemKey, string valueKey, object value, DateTime timestamp)
        {
            return await AddObservationAsync(deviceKey, new ObservationInput
            {
                DeviceKey = deviceKey,
                DataItemKey = dataItemKey,
                Values = new List<ObservationValue> { new ObservationValue(valueKey, value) },
                Timestamp = timestamp.ToUnixTime()
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem to the Agent
        /// </summary>
        public bool AddObservation(string deviceKey, IObservationInput observationInput)
        {
            if (observationInput != null)
            {
                ObservationReceived?.Invoke(this, observationInput);

                var input = new ObservationInput();
                input.DeviceKey = deviceKey;
                input.DataItemKey = observationInput.DataItemKey;

                // Convert Case (if Ignored)
                if (_configuration.IgnoreObservationCase) input.Values = Observation.UppercaseValues(observationInput.Values);
                else input.Values = observationInput.Values;

                // Set Timestamp
                if (_configuration.IgnoreTimestamps) input.Timestamp = UnixDateTime.Now;
                else input.Timestamp = observationInput.Timestamp > 0 ? observationInput.Timestamp : UnixDateTime.Now;

                // Get Device UUID from deviceKey
                _deviceKeys.TryGetValue(deviceKey, out var deviceUuid);

                // Get DataItem based on Observation's Key
                var dataItem = GetDataItemFromKey(deviceUuid, input.DataItemKey);
                if (dataItem != null)
                {
                    var success = false;
                    var validationResult = new DataItemValidationResult(true);

                    if (_configuration.ValidationLevel > ValidationLevel.Ignore)
                    {
                        // Validate Observation Input with DataItem type
                        validationResult = dataItem.IsValid(Version, input);
                        if (!validationResult.IsValid) validationResult.Message = $"{dataItem.Type} : {dataItem.Id} : {validationResult.Message}";
                    }

                    if (validationResult.IsValid || _configuration.ValidationLevel != ValidationLevel.Strict)
                    {
                        // Convert Units (if needed)
                        if (_configuration.ConvertUnits)
                        {
                            observationInput = ConvertObservationValue(dataItem, input);
                        }

                        bool update;

                        // Check if Observation Needs to be Updated
                        if (dataItem.Category == DataItemCategory.CONDITION)
                        {
                            update = UpdateCurrentCondition(deviceUuid, dataItem, input);
                        }
                        else
                        {
                            update = UpdateCurrentObservation(deviceUuid, dataItem, input);
                        }

                        // Check if Observation Needs to be Updated
                        if (update)
                        {
                            var observation = new Observation();
                            observation.SetProperty(nameof(Observation.DeviceUuid), deviceUuid);
                            observation.SetProperty(nameof(Observation.DataItemId), dataItem.Id);
                            observation.SetProperty(nameof(Observation.Representation), dataItem.Representation);
                            observation.SetProperty(nameof(Observation.Type), dataItem.Type);
                            observation.SetProperty(nameof(Observation.SubType), dataItem.SubType);
                            observation.SetProperty(nameof(Observation.Name), dataItem.Name);
                            observation.SetProperty(nameof(Observation.CompositionId), dataItem.CompositionId);
                            observation.SetProperty(nameof(Observation.Timestamp), input.Timestamp.ToDateTime());
                            observation.AddValues(observationInput.Values);

                            // Add Observation to Streaming Buffer
                            if (_observationBuffer.AddObservation(deviceUuid, dataItem, observation))
                            {
                                if (dataItem.Type != ObservationUpdateRateDataItem.TypeId &&
                                    dataItem.Type != AssetUpdateRateDataItem.TypeId)
                                {
                                    // Update Agent Metrics
                                    _metrics.UpdateObservation(deviceUuid, dataItem.Id);
                                }

                                ObservationAdded?.Invoke(this, observation);

                                success = true;
                            }
                        }
                        else success = true; // Return true if no update needed
                    }

                    if (!validationResult.IsValid && InvalidDataItemAdded != null) InvalidDataItemAdded.Invoke(dataItem, validationResult);

                    return success;
                }
            }

            return false;
        }

        /// <summary>
        /// Add a new Observation for a DataItem to the Agent
        /// </summary>
        public async Task<bool> AddObservationAsync(string deviceKey, IObservationInput observationInput)
        {
            if (observationInput != null)
            {
                ObservationReceived?.Invoke(this, observationInput);

                var input = new ObservationInput();
                input.DeviceKey = deviceKey;
                input.DataItemKey = observationInput.DataItemKey;

                // Convert Case (if Ignored)
                if (_configuration.IgnoreObservationCase) input.Values = Observation.UppercaseValues(observationInput.Values);
                else input.Values = observationInput.Values;

                // Set Timestamp
                if (_configuration.IgnoreTimestamps) input.Timestamp = UnixDateTime.Now;
                else input.Timestamp = observationInput.Timestamp > 0 ? observationInput.Timestamp : UnixDateTime.Now;

                // Get Device UUID from deviceKey
                _deviceKeys.TryGetValue(deviceKey, out var deviceUuid);

                // Get DataItem based on Observation's Key
                var dataItem = await GetDataItemFromKeyAsync(deviceUuid, input.DataItemKey);
                if (dataItem != null)
                {
                    var success = false;
                    var validationResult = new DataItemValidationResult(true);

                    if (_configuration.ValidationLevel > ValidationLevel.Ignore)
                    {
                        // Validate Observation Input with DataItem type
                        validationResult = dataItem.IsValid(Version, input);
                        if (!validationResult.IsValid) validationResult.Message = $"{dataItem.Type} : {dataItem.Id} : {validationResult.Message}";
                    }

                    if (validationResult.IsValid || _configuration.ValidationLevel != ValidationLevel.Strict)
                    {
                        // Convert Units (if needed)
                        if (_configuration.ConvertUnits)
                        {
                            observationInput = ConvertObservationValue(dataItem, input);
                        }

                        bool update;

                        // Check if Observation Needs to be Updated
                        if (dataItem.Category == DataItemCategory.CONDITION)
                        {
                            update = UpdateCurrentCondition(deviceUuid, dataItem, input);
                        }
                        else
                        {
                            update = UpdateCurrentObservation(deviceUuid, dataItem, input);
                        }

                        // Check if Observation Needs to be Updated
                        if (update)
                        {
                            var observation = new Observation();
                            observation.SetProperty(nameof(Observation.DeviceUuid), deviceUuid);
                            observation.SetProperty(nameof(Observation.DataItemId), dataItem.Id);
                            observation.SetProperty(nameof(Observation.Representation), dataItem.Representation);
                            observation.SetProperty(nameof(Observation.Type), dataItem.Type);
                            observation.SetProperty(nameof(Observation.SubType), dataItem.SubType);
                            observation.SetProperty(nameof(Observation.Name), dataItem.Name);
                            observation.SetProperty(nameof(Observation.CompositionId), dataItem.CompositionId);
                            observation.SetProperty(nameof(Observation.Timestamp), input.Timestamp.ToDateTime());
                            observation.AddValues(observationInput.Values);

                            // Add Observation to Streaming Buffer
                            if (_observationBuffer.AddObservation(deviceUuid, dataItem, observation))
                            {
                                if (dataItem.Type != ObservationUpdateRateDataItem.TypeId &&
                                    dataItem.Type != AssetUpdateRateDataItem.TypeId)
                                {
                                    // Update Agent Metrics
                                    _metrics.UpdateObservation(deviceUuid, dataItem.Id);
                                }

                                ObservationAdded?.Invoke(this, observation);

                                success = true;
                            }
                        }
                        else success = true; // Return true if no update needed
                    }

                    if (!validationResult.IsValid && InvalidDataItemAdded != null) InvalidDataItemAdded.Invoke(dataItem, validationResult);

                    return success;
                }
            }

            return false;
        }

        /// <summary>
        /// Add new Observations for DataItems to the Agent
        /// </summary>
        public bool AddObservations(string deviceKey, IEnumerable<IObservationInput> observations)
        {
            if (!observations.IsNullOrEmpty())
            {
                bool success = false;

                foreach (var observation in observations)
                {
                    success = AddObservation(deviceKey, observation);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        /// <summary>
        /// Add new Observations for DataItems to the Agent
        /// </summary>
        public async Task<bool> AddObservationsAsync(string deviceKey, IEnumerable<IObservationInput> observations)
        {
            if (!observations.IsNullOrEmpty())
            {
                bool success = false;

                foreach (var observation in observations)
                {
                    success = await AddObservationAsync(deviceKey, observation);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        #endregion

        #region "Assets"

        /// <summary>
        /// Add a new Asset to the Agent
        /// </summary>
        public bool AddAsset(string deviceKey, IAsset asset)
        {
            if (_deviceBuffer != null && _assetBuffer != null)
            {
                // Get Device UUID from deviceKey
                _deviceKeys.TryGetValue(deviceKey, out var deviceUuid);

                // Get Device from DeviceBuffer
                var device = _deviceBuffer.GetDevice(deviceUuid);
                if (device != null)
                {
                    // Set Device UUID Property
                    asset.DeviceUuid = device.Uuid;

                    // Validate Asset based on Device's MTConnectVersion
                    var validationResults = asset.IsValid(device.MTConnectVersion);
                    if (validationResults.IsValid)
                    {
                        // Update ASSET_CHANGED for device
                        var assetChangedId = DataItem.CreateId(device.Id, AssetChangedDataItem.NameId);
                        AddObservation(deviceUuid, assetChangedId, ValueKeys.CDATA, asset.AssetId, asset.Timestamp);

                        // Add Asset to AssetBuffer
                        if (_assetBuffer.AddAsset(asset))
                        {
                            // Update Agent Metrics
                            _metrics.UpdateAsset(deviceUuid, asset.AssetId);

                            AssetAdded?.Invoke(this, asset);
                            return true;
                        }
                    }
                    else
                    {
                        if (InvalidAssetAdded != null) InvalidAssetAdded.Invoke(asset, validationResults);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Add a new Asset to the Agent
        /// </summary>
        public async Task<bool> AddAssetAsync(string deviceKey, IAsset asset)
        {
            if (_assetBuffer != null)
            {
                // Get Device UUID from deviceKey
                _deviceKeys.TryGetValue(deviceKey, out var deviceUuid);

                // Get Device from DeviceBuffer
                var device = await _deviceBuffer.GetDeviceAsync(deviceUuid);
                if (device != null)
                {
                    // Set Device UUID Property
                    asset.DeviceUuid = device.Uuid;

                    // Validate Asset based on Device's MTConnectVersion
                    var validationResults = asset.IsValid(device.MTConnectVersion);
                    if (validationResults.IsValid)
                    {
                        // Update ASSET_CHANGED for device
                        var assetChangedId = DataItem.CreateId(device.Id, AssetChangedDataItem.NameId);
                        AddObservation(deviceUuid, assetChangedId, ValueKeys.CDATA, asset.AssetId, asset.Timestamp);

                        // Add Asset to AssetBuffer
                        if (await _assetBuffer.AddAssetAsync(asset))
                        {
                            // Update Agent Metrics
                            _metrics.UpdateAsset(deviceUuid, asset.AssetId);

                            AssetAdded?.Invoke(this, asset);
                            return true;
                        }
                    }
                    else
                    {
                        if (InvalidAssetAdded != null) InvalidAssetAdded.Invoke(asset, validationResults);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Add new Assets to the Agent
        /// </summary>
        public bool AddAssets(string deviceKey, IEnumerable<IAsset> assets)
        {
            if (_assetBuffer != null && !assets.IsNullOrEmpty())
            {
                var success = false;

                foreach (var asset in assets)
                {
                    success = AddAsset(deviceKey, asset);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        /// <summary>
        /// Add new Assets to the Agent
        /// </summary>
        public async Task<bool> AddAssetsAsync(string deviceKey, IEnumerable<IAsset> assets)
        {
            if (_assetBuffer != null && !assets.IsNullOrEmpty())
            {
                var success = false;

                foreach (var asset in assets)
                {
                    success = await AddAssetAsync(deviceKey, asset);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        #endregion

        #region "DeviceModels"

        ///// <summary>
        ///// Add a new DeviceModel to the Agent's Buffer. This adds all of the data contained in the Device Model (Device and Observations).
        ///// </summary>
        //public bool AddDeviceModel(DeviceModel deviceModel)
        //{
        //    //if (deviceModel != null)
        //    //{
        //    //    bool success;

        //    //    success = AddDevice(deviceModel);
        //    //    if (success) success = AddObservations(deviceModel.Name, deviceModel.GetObservations());

        //    //    return success;
        //    //}

        //    return false;
        //}

        ///// <summary>
        ///// Add a new DeviceModel to the Agent's Buffer. This adds all of the data contained in the Device Model (Device and Observations).
        ///// </summary>
        //public async Task<bool> AddDeviceModelAsync(DeviceModel deviceModel)
        //{
        //    //if (deviceModel != null)
        //    //{
        //    //    bool success;

        //    //    success = await AddDeviceAsync(deviceModel);
        //    //    if (success) success = await AddObservationsAsync(deviceModel.Name, deviceModel.GetObservations());

        //    //    return success;
        //    //}

        //    return false;
        //}

        ///// <summary>
        ///// Add new DeviceModels to the Agent's Buffer. This adds all of the data contained in the Device Models (Device and Observations).
        ///// </summary>
        //public bool AddDeviceModels(IEnumerable<DeviceModel> deviceModels)
        //{
        //    if (!deviceModels.IsNullOrEmpty())
        //    {
        //        var success = false;

        //        foreach (var deviceModel in deviceModels)
        //        {
        //            success = AddDeviceModel(deviceModel);
        //            if (!success) break;
        //        }

        //        return success;
        //    }

        //    return false;
        //}

        ///// <summary>
        ///// Add new DeviceModels to the Agent's Buffer. This adds all of the data contained in the Device Models (Device and Observations).
        ///// </summary>
        //public async Task<bool> AddDeviceModelsAsync(IEnumerable<DeviceModel> deviceModels)
        //{
        //    if (!deviceModels.IsNullOrEmpty())
        //    {
        //        var success = false;

        //        foreach (var deviceModel in deviceModels)
        //        {
        //            success = await AddDeviceModelAsync(deviceModel);
        //            if (!success) break;
        //        }

        //        return success;
        //    }

        //    return false;
        //}

        #endregion

        #endregion

        #region "Metrics"

        private void DeviceMetricsUpdated(object sender, DeviceMetrics deviceMetrics)
        {
            if (deviceMetrics != null && _deviceBuffer != null)
            {
                var device = _deviceBuffer.GetDevice(deviceMetrics.DeviceUuid);
                if (device != null)
                {
                    var dataItems = device.GetDataItems();
                    if (!dataItems.IsNullOrEmpty())
                    {
                        // Update ObservationUpdateRate DataItem
                        var observationUpdateRate = dataItems.FirstOrDefault(o => o.Type == ObservationUpdateRateDataItem.TypeId);
                        if (observationUpdateRate != null)
                        {
                            AddObservation(device.Name, observationUpdateRate.Id, ValueKeys.CDATA, deviceMetrics.ObservationAverage);
                        }

                        // Update AssetUpdateRate DataItem
                        var assetUpdateRate = dataItems.FirstOrDefault(o => o.Type == AssetUpdateRateDataItem.TypeId);
                        if (assetUpdateRate != null)
                        {
                            AddObservation(device.Name, assetUpdateRate.Id, ValueKeys.CDATA, deviceMetrics.AssetAverage);
                        }
                    }
                }
            }
        }

        #endregion

        #region "Agent Device"

        /// <summary>
        /// Create a new Agent Device to represent the MTConnect Agent
        /// </summary>
        public void InitializeAgentDevice(bool initializeDataItems = true)
        {
            var agent = new Agent();
            agent.Id = $"agent_{_uuid.ToMD5Hash().Substring(0, 10)}";
            agent.Name = "Agent";
            agent.Uuid = _uuid;
            agent.MTConnectVersion = Version;

            var dataItems = new List<IDataItem>();

            // Add Availibility DataItem to Agent
            var availabilityDataItem = new AvailabilityDataItem(agent.Id);
            dataItems.Add(availabilityDataItem);

            // Add Device Added DataItem to Agent
            dataItems.Add(new DeviceAddedDataItem(agent.Id));

            // Add Device Removed DataItem to Agent
            dataItems.Add(new DeviceRemovedDataItem(agent.Id));

            // Add Device Changed DataItem to Agent
            dataItems.Add(new DeviceChangedDataItem(agent.Id));

            // Add Asset Added DataItem to Agent
            dataItems.Add(new AssetChangedDataItem(agent.Id));

            // Add Asset Removed DataItem to Agent
            dataItems.Add(new AssetRemovedDataItem(agent.Id));

            agent.DataItems = dataItems;

            _agent = agent;

            // Add Name and UUID to DeviceKey dictionary
            _deviceKeys.TryAdd(agent.Name, agent.Uuid);
            _deviceKeys.TryAdd(agent.Uuid, agent.Uuid);

            if (initializeDataItems)
            {
                // Initialize Availability
                AddAgentObservation(availabilityDataItem.Id, Observations.Events.Values.Availability.AVAILABLE);

                InitializeDataItems(agent);
            }
        }

        /// <summary>
        /// Add a new Adapter Component to the Agent Device
        /// </summary>
        public void AddAdapterComponent(AdapterConfiguration configuration, bool initializeDataItems = true)
        {
            var agent = Agent;
            if (agent != null && configuration != null && !string.IsNullOrEmpty(configuration.Host))
            {
                // Create a new Adapter Component
                var adapter = new AdapterComponent();
                adapter.Id = configuration.Id;
                adapter.Name = $"{configuration.Host}:{configuration.Port}";

                // Adapters Organizer Component
                Component adapters = null;
                if (agent.Components.IsNullOrEmpty())
                {
                    // Create New Adapters Organizer Component
                    adapters = new AdaptersComponent() { Id = AdaptersId };

                    var agentComponents = new List<IComponent>();
                    agentComponents.Add(adapters);
                    agent.Components = agentComponents;
                }
                else
                {
                    adapters = agent.Components.FirstOrDefault(o => o.Id == AdaptersId) as Component;
                }

                if (adapters != null)
                {
                    var dataItems = new List<IDataItem>();

                    // Add Connection Status
                    var connectionStatusDataItem = new ConnectionStatusDataItem(adapter.Id);
                    dataItems.Add(connectionStatusDataItem);

                    // Add Adapter URI
                    var adapterUri = $"shdr://{configuration.Host}:{configuration.Port}";
                    AdapterUriDataItem adapterUriDataItem = null;
                    if (!configuration.SuppressIpAddress)
                    {
                        adapterUriDataItem = new AdapterUriDataItem(adapter.Id);
                        var adapterUriConstraint = new Constraints();
                        adapterUriConstraint.Values = new List<string> { adapterUri };
                        adapterUriDataItem.Constraints = adapterUriConstraint;
                        dataItems.Add(adapterUriDataItem);
                    }

                    // Add Observation Update Rate
                    dataItems.Add(new ObservationUpdateRateDataItem(adapter.Id));

                    // Add Asset Update Rate
                    dataItems.Add(new AssetUpdateRateDataItem(adapter.Id));

                    // Add Adapter Software Version
                    AdapterSoftwareVersionDataItem adapterSoftwareVersionDataItem = null;
                    if (configuration.ShdrVersion != null)
                    {
                        adapterSoftwareVersionDataItem = new AdapterSoftwareVersionDataItem(adapter.Id);
                        dataItems.Add(adapterSoftwareVersionDataItem);
                    }

                    // Add MTConnect Version
                    //if (Version != null) dataItems.Add(new MTConnectVersionDataItem(adapter.Id));

                    adapter.DataItems = dataItems;

                    var adapterComponents = new List<IComponent>();
                    adapterComponents.Add(adapter);
                    adapters.Components = adapterComponents;

                    var components = new List<IComponent>();
                    components.Add(adapters);
                    agent.Components = components;


                    if (initializeDataItems)
                    {
                        // Add Connection Status Observation
                        AddAgentObservation(connectionStatusDataItem.Id, Observations.Events.Values.ConnectionStatus.LISTEN);

                        // Add Adapter Uri Observation
                        if (adapterUri != null)
                        {
                            AddAgentObservation(adapterUriDataItem.Id, adapterUri);
                        }

                        // Add Adapter Software Version Observation
                        if (adapterSoftwareVersionDataItem != null)
                        {
                            AddAgentObservation(adapterSoftwareVersionDataItem.Id, configuration.ShdrVersion);
                        }
                    }
                }
            }
        }


        public void UpdateAdapterConnectionStatus(string adapterId, Observations.Events.Values.ConnectionStatus connectionStatus)
        {
            if (_agent != null)
            {
                var dataItemKey = DataItem.CreateId(adapterId, ConnectionStatusDataItem.NameId);
                AddAgentObservation(dataItemKey, connectionStatus);
            }
        }

        public void UpdateAdapterObservationRate(string adapterId, double value)
        {
            if (_agent != null)
            {
                var dataItemKey = DataItem.CreateId(adapterId, ObservationUpdateRateDataItem.NameId);
                AddAgentObservation(dataItemKey, value);
            }
        }


        /// <summary>
        /// Add a new Observation for a DataItem to the Agent Device
        /// </summary>
        public bool AddAgentObservation(string dataItemKey, object value)
        {
            if (_agent != null && !string.IsNullOrEmpty(dataItemKey) && value != null)
            {
                var input = new ObservationInput();
                input.DeviceKey = _agent.Uuid;
                input.DataItemKey = dataItemKey;
                input.AddValue(ValueKeys.CDATA, value);
                input.Timestamp = UnixDateTime.Now;

                var dataItem = _agent.GetDataItemByKey(dataItemKey);
                if (dataItem != null)
                {
                    // Check if Observation Needs to be Updated
                    bool update = UpdateCurrentObservation(_agent.Uuid, dataItem, input);
                    if (update)
                    {
                        var observation = new Observation();
                        observation.SetProperty(nameof(Observation.DeviceUuid), _agent.Uuid);
                        observation.SetProperty(nameof(Observation.DataItemId), dataItem.Id);
                        observation.SetProperty(nameof(Observation.Representation), dataItem.Representation);
                        observation.SetProperty(nameof(Observation.Type), dataItem.Type);
                        observation.SetProperty(nameof(Observation.SubType), dataItem.SubType);
                        observation.SetProperty(nameof(Observation.Name), dataItem.Name);
                        observation.SetProperty(nameof(Observation.CompositionId), dataItem.CompositionId);
                        observation.SetProperty(nameof(Observation.Timestamp), input.Timestamp.ToDateTime());
                        observation.AddValues(input.Values);

                        // Add Observation to Streaming Buffer
                        return _observationBuffer.AddObservation(_agent.Uuid, dataItem, observation);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Add a new Observation for a DataItem to the Agent Device
        /// </summary>
        public async Task<bool> AddAgentObservationAsync(string dataItemKey, object value)
        {
            if (_agent != null && !string.IsNullOrEmpty(dataItemKey) && value != null)
            {
                var input = new ObservationInput();
                input.DeviceKey = _agent.Uuid;
                input.DataItemKey = dataItemKey;
                input.AddValue(ValueKeys.CDATA, value);
                input.Timestamp = UnixDateTime.Now;

                var dataItem = _agent.GetDataItemByKey(dataItemKey);
                if (dataItem != null)
                {
                    // Check if Observation Needs to be Updated
                    bool update = UpdateCurrentObservation(_agent.Uuid, dataItem, input);
                    if (update)
                    {
                        var observation = new Observation();
                        observation.SetProperty(nameof(Observation.DeviceUuid), _agent.Uuid);
                        observation.SetProperty(nameof(Observation.DataItemId), dataItem.Id);
                        observation.SetProperty(nameof(Observation.Representation), dataItem.Representation);
                        observation.SetProperty(nameof(Observation.Type), dataItem.Type);
                        observation.SetProperty(nameof(Observation.SubType), dataItem.SubType);
                        observation.SetProperty(nameof(Observation.Name), dataItem.Name);
                        observation.SetProperty(nameof(Observation.CompositionId), dataItem.CompositionId);
                        observation.SetProperty(nameof(Observation.Timestamp), input.Timestamp.ToDateTime());
                        observation.AddValues(input.Values);

                        // Add Observation to Streaming Buffer
                        return await _observationBuffer.AddObservationAsync(_agent.Uuid, dataItem, observation);
                    }
                }
            }

            return false;
        }

        #endregion

        #region "Agent Information"

        private void StartAgentInformationUpdateTimer()
        {
            if (_informationUpdateTimer != null) _informationUpdateTimer.Dispose();
            _informationUpdateTimer = new System.Timers.Timer();
            _informationUpdateTimer.Interval = InformationUpdateInterval;
            _informationUpdateTimer.Elapsed += UpdateAgentInformation;
            _informationUpdateTimer.Enabled = true;
        }

        private void StopAgentInformationUpdateTimer()
        {
            if (_informationUpdateTimer != null) _informationUpdateTimer.Dispose();
        }

        private void UpdateAgentInformation(object sender, EventArgs args)
        {
            if (!_updateInformation)
            {
                // Check if InstanceId is the same
                if (_information.InstanceId != _instanceId)
                {
                    _information.InstanceId = _instanceId;
                    _updateInformation = true;
                }

                // Check if DeviceModelChangeTime is the same
                if (_information.DeviceModelChangeTime != _deviceModelChangeTime)
                {
                    _information.DeviceModelChangeTime = _deviceModelChangeTime;
                    _updateInformation = true;
                }
            }

            if (_updateInformation)
            {
                // Save to File
                _information.Save();
                _updateInformation = false;
            }
        }

        #endregion
    }
}
