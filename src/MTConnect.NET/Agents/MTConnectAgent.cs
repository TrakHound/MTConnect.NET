// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents.Configuration;
using MTConnect.Agents.Metrics;
using MTConnect.Assets;
using MTConnect.Buffers;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Models;
using MTConnect.Observations;
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
        private readonly MTConnectAgentConfiguration _configuration;
        private readonly IMTConnectDeviceBuffer _deviceBuffer;
        private readonly IMTConnectObservationBuffer _observationBuffer;
        private readonly IMTConnectAssetBuffer _assetBuffer;
        private readonly ConcurrentDictionary<string, IObservation> _currentObservations = new ConcurrentDictionary<string, IObservation>();
        private readonly ConcurrentDictionary<string, IEnumerable<IObservation>> _currentConditions = new ConcurrentDictionary<string, IEnumerable<IObservation>>();
        private readonly MTConnectAgentMetrics _metrics = new MTConnectAgentMetrics(TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(1));


        /// <summary>
        /// Gets the Configuration associated with the Agent
        /// </summary>
        public MTConnectAgentConfiguration Configuration => _configuration;

        /// <summary>
        /// Gets the Metrics associated with the Agent
        /// </summary>
        public MTConnectAgentMetrics Metrics => _metrics;

        /// <summary>
        /// Gets a representation of the specific instance of the Agent.
        /// </summary>
        public long InstanceId { get; }

        /// <summary>
        /// Gets the MTConnect Version that the Agent is using.
        /// </summary>
        public Version Version { get; set; }

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
        public EventHandler<Device> DeviceAdded { get; set; }

        /// <summary>
        /// Raised when a new Observation is added to the Agent
        /// </summary>
        public EventHandler<IObservation> ObservationAdded { get; set; }

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


        public MTConnectAgent()
        {
            InstanceId = CreateInstanceId();
            _deviceBuffer = new MTConnectDeviceBuffer();
            _observationBuffer = new MTConnectObservationBuffer();
            _assetBuffer = new MTConnectAssetBuffer();
            _metrics.DeviceMetricsUpdated += DeviceMetricsUpdated;
        }

        public MTConnectAgent(MTConnectAgentConfiguration configuration)
        {
            InstanceId = CreateInstanceId();
            _configuration = configuration;
            _deviceBuffer = new MTConnectDeviceBuffer();
            _observationBuffer = new MTConnectObservationBuffer(configuration);
            _assetBuffer = new MTConnectAssetBuffer(configuration);
        }

        public MTConnectAgent(
            IMTConnectDeviceBuffer deviceBuffer,
            IMTConnectObservationBuffer streamingBuffer,
            IMTConnectAssetBuffer assetBuffer
            )
        {
            InstanceId = CreateInstanceId();
            _deviceBuffer = deviceBuffer;
            _observationBuffer = streamingBuffer;
            _assetBuffer = assetBuffer;
        }

        public MTConnectAgent(
            MTConnectAgentConfiguration configuration,
            IMTConnectDeviceBuffer deviceBuffer,
            IMTConnectObservationBuffer streamingBuffer,
            IMTConnectAssetBuffer assetBuffer
            )
        {
            InstanceId = CreateInstanceId();
            _configuration = configuration;
            _deviceBuffer = deviceBuffer;
            _observationBuffer = streamingBuffer;
            _assetBuffer = assetBuffer;
        }


        public void Dispose()
        {
            if (_metrics != null) _metrics.Dispose();
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

        private Headers.MTConnectDevicesHeader GetDevicesHeader(Version version)
        {
            var v = version != null ? version : Version;

            var header = new Headers.MTConnectDevicesHeader
            {
                BufferSize = _observationBuffer.BufferSize,
                AssetBufferSize = _assetBuffer.BufferSize,
                AssetCount = _assetBuffer.AssetCount,
                CreationTime = DateTime.UtcNow,
                InstanceId = InstanceId,
                Sender = System.Net.Dns.GetHostName(),
                Version = v.ToString(),
                TestIndicator = null
            };

            if (v < MTConnectVersions.Version14) header.AssetBufferSize = -1;
            if (v < MTConnectVersions.Version14) header.AssetCount = -1;

            return header;
        }

        private Headers.MTConnectStreamsHeader GetStreamsHeader()
        {
            return new Headers.MTConnectStreamsHeader
            {
                BufferSize = _observationBuffer.BufferSize,
                CreationTime = DateTime.UtcNow,
                InstanceId = InstanceId,
                Sender = System.Net.Dns.GetHostName(),
                Version = Version.ToString(),
                TestIndicator = null
            };
        }

        private Headers.MTConnectAssetsHeader GetAssetsHeader()
        {
            return new Headers.MTConnectAssetsHeader
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

        private Headers.MTConnectErrorHeader GetErrorHeader()
        {
            return new Headers.MTConnectErrorHeader
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

        private List<Device> ProcessDevices(IEnumerable<Device> devices, Version version = null)
        {
            var objs = new List<Device>();

            if (!devices.IsNullOrEmpty())
            {
                foreach (var device in devices)
                {
                    objs.Add(Device.Process(device, version != null ? version : Version));
                }
            }

            return objs;
        }

        #endregion


        /// <summary>
        /// Get a MTConnectDevices Document containing all devices.
        /// </summary>
        /// <returns>MTConnectDevices Response Document</returns>
        public DevicesDocument GetDevices(Version version = null)
        {
            DevicesRequestReceived?.Invoke(null);

            if (_deviceBuffer != null)
            {
                var devices = _deviceBuffer.GetDevices();
                if (devices != null && devices.Count() > 0)
                {
                    var doc = new DevicesDocument();
                    doc.Version = Version;

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
        /// Get a MTConnectDevices Document containing all devices.
        /// </summary>
        /// <returns>MTConnectDevices Response Document</returns>
        public async Task<DevicesDocument> GetDevicesAsync(Version version = null)
        {
            DevicesRequestReceived?.Invoke(null);

            if (_deviceBuffer != null)
            {
                var devices = await _deviceBuffer.GetDevicesAsync();
                if (devices != null && devices.Count() > 0)
                {
                    var doc = new DevicesDocument();
                    doc.Version = Version;

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
        /// Get a MTConnectDevices Document containing the specified device.
        /// </summary>
        /// <param name="deviceName">The Name of the requested Device</param>
        /// <returns>MTConnectDevices Response Document</returns>
        public DevicesDocument GetDevices(string deviceName, Version version = null)
        {
            DevicesRequestReceived?.Invoke(deviceName);

            if (_deviceBuffer != null)
            {
                var device = _deviceBuffer.GetDevice(deviceName);
                if (device != null)
                {
                    var doc = new DevicesDocument();
                    doc.Version = Version;

                    var header = GetDevicesHeader(version);
                    header.Version = GetAgentVersion().ToString();

                    doc.Header = header;
                    doc.Devices = ProcessDevices(new List<Device> { device }, version);

                    DevicesResponseSent?.Invoke(doc);

                    return doc;
                }
            }

            return null;
        }

        /// <summary>
        /// Get a MTConnectDevices Document containing the specified device.
        /// </summary>
        /// <param name="deviceName">The Name of the requested Device</param>
        /// <returns>MTConnectDevices Response Document</returns>
        public async Task<DevicesDocument> GetDevicesAsync(string deviceName, Version version = null)
        {
            DevicesRequestReceived?.Invoke(deviceName);

            if (_deviceBuffer != null)
            {
                var device = await _deviceBuffer.GetDeviceAsync(deviceName);
                if (device != null)
                {
                    var doc = new DevicesDocument();
                    doc.Version = Version;

                    var header = GetDevicesHeader(version);
                    header.Version = GetAgentVersion().ToString();

                    doc.Header = header;
                    doc.Devices = ProcessDevices(new List<Device> { device }, version);

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
        public Streams.StreamsDocument GetDeviceStreams(int count = 0)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = _deviceBuffer.GetDevices();
                if (!devices.IsNullOrEmpty())
                {
                    var deviceNames = devices.Select(x => x.Name);
                    var dataItemIds = new List<string>();

                    // Create list of DataItemIds
                    foreach (var device in devices)
                    {
                        var dataItems = device.GetDataItems();
                        if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));
                    }

                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(deviceNames, dataItemIds, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results);
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
        public async Task<Streams.StreamsDocument> GetDeviceStreamsAsync(int count = 0)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = await _deviceBuffer.GetDevicesAsync();
                if (!devices.IsNullOrEmpty())
                {
                    var deviceNames = devices.Select(x => x.Name);
                    var dataItemIds = new List<string>();

                    // Create list of DataItemIds
                    foreach (var device in devices)
                    {
                        var dataItems = device.GetDataItems();
                        if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));
                    }

                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(deviceNames, dataItemIds, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results);
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
        public Streams.StreamsDocument GetDeviceStreams(long at, int count = 0)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = _deviceBuffer.GetDevices();
                if (!devices.IsNullOrEmpty())
                {
                    var deviceNames = devices.Select(x => x.Name);
                    var dataItemIds = new List<string>();

                    // Create list of DataItemIds
                    foreach (var device in devices)
                    {
                        var dataItems = device.GetDataItems();
                        if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));
                    }

                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(deviceNames, dataItemIds, at: at, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results);
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
        public async Task<Streams.StreamsDocument> GetDeviceStreamsAsync(long at, int count = 0)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = await _deviceBuffer.GetDevicesAsync();
                if (!devices.IsNullOrEmpty())
                {
                    var deviceNames = devices.Select(x => x.Name);
                    var dataItemIds = new List<string>();

                    // Create list of DataItemIds
                    foreach (var device in devices)
                    {
                        var dataItems = device.GetDataItems();
                        if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));
                    }

                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(deviceNames, dataItemIds, at: at, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results);
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
        public Streams.StreamsDocument GetDeviceStreams(IEnumerable<string> dataItemIds, long at, int count = 0)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = _deviceBuffer.GetDevices();
                if (!devices.IsNullOrEmpty())
                {
                    var deviceNames = devices.Select(x => x.Name);

                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(deviceNames, dataItemIds, at: at, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results);
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
        public async Task<Streams.StreamsDocument> GetDeviceStreamsAsync(IEnumerable<string> dataItemIds, long at, int count = 0)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = await _deviceBuffer.GetDevicesAsync();
                if (!devices.IsNullOrEmpty())
                {
                    var deviceNames = devices.Select(x => x.Name);

                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(deviceNames, dataItemIds, at: at, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results);
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
        public Streams.StreamsDocument GetDeviceStreams(long from, long to, int count = 0)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = _deviceBuffer.GetDevices();
                if (!devices.IsNullOrEmpty())
                {
                    var deviceNames = devices.Select(x => x.Name);
                    var dataItemIds = new List<string>();

                    // Create list of DataItemIds
                    foreach (var device in devices)
                    {
                        var dataItems = device.GetDataItems();
                        if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));
                    }

                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(deviceNames, dataItemIds, from: from, to: to, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results);
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
        public async Task<Streams.StreamsDocument> GetDeviceStreamsAsync(long from, long to, int count = 0)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = await _deviceBuffer.GetDevicesAsync();
                if (!devices.IsNullOrEmpty())
                {
                    var deviceNames = devices.Select(x => x.Name);
                    var dataItemIds = new List<string>();

                    // Create list of DataItemIds
                    foreach (var device in devices)
                    {
                        var dataItems = device.GetDataItems();
                        if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));
                    }

                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(deviceNames, dataItemIds, from: from, to: to, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results);
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
        public Streams.StreamsDocument GetDeviceStreams(IEnumerable<string> dataItemIds, long from, long to, int count = 0)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = _deviceBuffer.GetDevices();
                if (!devices.IsNullOrEmpty())
                {
                    var deviceNames = devices.Select(x => x.Name);

                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(deviceNames, dataItemIds, from, to, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results);
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
        public async Task<Streams.StreamsDocument> GetDeviceStreamsAsync(IEnumerable<string> dataItemIds, long from, long to, int count = 0)
        {
            StreamsRequestReceived?.Invoke(null);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get list of Devices from the MTConnectDeviceBuffer
                var devices = await _deviceBuffer.GetDevicesAsync();
                if (!devices.IsNullOrEmpty())
                {
                    var deviceNames = devices.Select(x => x.Name);

                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(deviceNames, dataItemIds, from, to, count: count);
                    var document = CreateDeviceStreamsDocument(devices, results);
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
        /// <param name="deviceName">The (name or uuid) of the requested Device</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public Streams.StreamsDocument GetDeviceStream(string deviceName, int count = 0)
        {
            StreamsRequestReceived?.Invoke(deviceName);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get Device from the MTConnectDeviceBuffer
                var device = _deviceBuffer.GetDevice(deviceName);
                if (device != null)
                {
                    // Create list of DataItemIds
                    var dataItems = device.GetDataItems();
                    var dataItemIds = new List<string>();
                    if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));

                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(device.Name, dataItemIds, count: count);
                    var document = CreateDeviceStreamsDocument(device, results);
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
        /// <param name="deviceName">The (name or uuid) of the requested Device</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public async Task<Streams.StreamsDocument> GetDeviceStreamAsync(string deviceName, int count = 0)
        {
            StreamsRequestReceived?.Invoke(deviceName);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get Device from the MTConnectDeviceBuffer
                var device = await _deviceBuffer.GetDeviceAsync(deviceName);
                if (device != null)
                {
                    // Create list of DataItemIds
                    var dataItems = device.GetDataItems();
                    var dataItemIds = new List<string>();
                    if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));

                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(device.Name, dataItemIds, count: count);
                    var document = CreateDeviceStreamsDocument(device, results);
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
        /// <param name="deviceName">The (name or uuid) of the requested Device</param>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public Streams.StreamsDocument GetDeviceStream(string deviceName, long at, int count = 0)
        {
            StreamsRequestReceived?.Invoke(deviceName);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get Device from the MTConnectDeviceBuffer
                var device = _deviceBuffer.GetDevice(deviceName);
                if (device != null)
                {
                    // Create list of DataItemIds
                    var dataItems = device.GetDataItems();
                    var dataItemIds = new List<string>();
                    if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));

                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(device.Name, dataItemIds, at: at, count: count);
                    var document = CreateDeviceStreamsDocument(device, results);
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
        /// <param name="deviceName">The (name or uuid) of the requested Device</param>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public async Task<Streams.StreamsDocument> GetDeviceStreamAsync(string deviceName, long at, int count = 0)
        {
            StreamsRequestReceived?.Invoke(deviceName);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get Device from the MTConnectDeviceBuffer
                var device = await _deviceBuffer.GetDeviceAsync(deviceName);
                if (device != null)
                {
                    // Create list of DataItemIds
                    var dataItems = device.GetDataItems();
                    var dataItemIds = new List<string>();
                    if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));

                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(device.Name, dataItemIds, at: at, count: count);
                    var document = CreateDeviceStreamsDocument(device, results);
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
        /// <param name="deviceName">The (name or uuid) of the requested Device</param>
        /// <param name="at">The sequence number to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public Streams.StreamsDocument GetDeviceStream(string deviceName, IEnumerable<string> dataItemIds, int count = 0)
        {
            StreamsRequestReceived?.Invoke(deviceName);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get Device from the MTConnectDeviceBuffer
                var device = _deviceBuffer.GetDevice(deviceName);
                if (device != null)
                {
                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(device.Name, dataItemIds, count: count);
                    var document = CreateDeviceStreamsDocument(device, results);
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
        /// <param name="deviceName">The (name or uuid) of the requested Device</param>
        /// <param name="at">The sequence number to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public async Task<Streams.StreamsDocument> GetDeviceStreamAsync(string deviceName, IEnumerable<string> dataItemIds, int count = 0)
        {
            StreamsRequestReceived?.Invoke(deviceName);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get Device from the MTConnectDeviceBuffer
                var device = await _deviceBuffer.GetDeviceAsync(deviceName);
                if (device != null)
                {
                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(device.Name, dataItemIds, count: count);
                    var document = CreateDeviceStreamsDocument(device, results);
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
        /// <param name="deviceName">The (name or uuid) of the requested Device</param>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="at">The sequence number to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public Streams.StreamsDocument GetDeviceStream(string deviceName, IEnumerable<string> dataItemIds, long at, int count = 0)
        {
            StreamsRequestReceived?.Invoke(deviceName);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get Device from the MTConnectDeviceBuffer
                var device = _deviceBuffer.GetDevice(deviceName);
                if (device != null)
                {
                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(device.Name, dataItemIds, at: at, count: count);
                    var document = CreateDeviceStreamsDocument(device, results);
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
        /// <param name="deviceName">The (name or uuid) of the requested Device</param>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="at">The sequence number to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public async Task<Streams.StreamsDocument> GetDeviceStreamAsync(string deviceName, IEnumerable<string> dataItemIds, long at, int count = 0)
        {
            StreamsRequestReceived?.Invoke(deviceName);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get Device from the MTConnectDeviceBuffer
                var device = await _deviceBuffer.GetDeviceAsync(deviceName);
                if (device != null)
                {
                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(device.Name, dataItemIds, at: at, count: count);
                    var document = CreateDeviceStreamsDocument(device, results);
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
        /// <param name="deviceName">The (name or uuid) of the requested Device</param>
        /// <param name="from">The sequence number of the first observation to include in the response</param>
        /// <param name="to">The sequence number of the last observation to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public Streams.StreamsDocument GetDeviceStream(string deviceName, long from, long to, int count = 0)
        {
            StreamsRequestReceived?.Invoke(deviceName);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get Device from the MTConnectDeviceBuffer
                var device = _deviceBuffer.GetDevice(deviceName);
                if (device != null)
                {
                    // Create list of DataItemIds
                    var dataItems = device.GetDataItems();
                    var dataItemIds = new List<string>();
                    if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));

                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(device.Name, dataItemIds, from, to: to, count: count);
                    var document = CreateDeviceStreamsDocument(device, results);
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
        /// <param name="deviceName">The (name or uuid) of the requested Device</param>
        /// <param name="from">The sequence number of the first observation to include in the response</param>
        /// <param name="to">The sequence number of the last observation to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public async Task<Streams.StreamsDocument> GetDeviceStreamAsync(string deviceName, long from, long to, int count = 0)
        {
            StreamsRequestReceived?.Invoke(deviceName);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get Device from the MTConnectDeviceBuffer
                var device = await _deviceBuffer.GetDeviceAsync(deviceName);
                if (device != null)
                {
                    // Create list of DataItemIds
                    var dataItems = device.GetDataItems();
                    var dataItemIds = new List<string>();
                    if (!dataItems.IsNullOrEmpty()) dataItemIds.AddRange(dataItems.Select(o => o.Id));

                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(device.Name, dataItemIds, from, to: to, count: count);
                    var document = CreateDeviceStreamsDocument(device, results);
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
        /// <param name="deviceName">The (name or uuid) of the requested Device</param>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="from">The sequence number of the first observation to include in the response</param>
        /// <param name="to">The sequence number of the last observation to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public Streams.StreamsDocument GetDeviceStream(string deviceName, IEnumerable<string> dataItemIds, long from, long to, int count = 0)
        {
            StreamsRequestReceived?.Invoke(deviceName);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get Device from the MTConnectDeviceBuffer
                var device = _deviceBuffer.GetDevice(deviceName);
                if (device != null)
                {
                    // Query the Observation Buffer 
                    var results = _observationBuffer.GetObservations(device.Name, dataItemIds, from, to: to, count: count);
                    var document = CreateDeviceStreamsDocument(device, results);
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
        /// <param name="deviceName">The (name or uuid) of the requested Device</param>
        /// <param name="dataItemIds">A list of DataItemId's to specify what observations to include in the response</param>
        /// <param name="from">The sequence number of the first observation to include in the response</param>
        /// <param name="to">The sequence number of the last observation to include in the response</param>
        /// <param name="count">The maximum number of observations to include in the response</param>
        /// <returns>MTConnectStreams Response Document</returns>
        public async Task<Streams.StreamsDocument> GetDeviceStreamAsync(string deviceName, IEnumerable<string> dataItemIds, long from, long to, int count = 0)
        {
            StreamsRequestReceived?.Invoke(deviceName);

            if (_deviceBuffer != null && _observationBuffer != null)
            {
                // Get Device from the MTConnectDeviceBuffer
                var device = await _deviceBuffer.GetDeviceAsync(deviceName);
                if (device != null)
                {
                    // Query the Observation Buffer 
                    var results = await _observationBuffer.GetObservationsAsync(device.Name, dataItemIds, from, to: to, count: count);
                    var document = CreateDeviceStreamsDocument(device, results);
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

        private Streams.StreamsDocument CreateDeviceStreamsDocument(Device device, IStreamingResults results)
        {
            if (device != null)
            {
                return CreateDeviceStreamsDocument(new List<Device> { device }, results);
            }

            return null;
        }

        private Streams.StreamsDocument CreateDeviceStreamsDocument(IEnumerable<Device> devices, IStreamingResults results)
        {
            if (results != null)
            {
                // Create list of DeviceStreams to return
                var deviceStreams = new List<Streams.DeviceStream>();
                foreach (var device in devices)
                {
                    // Create a DeviceStream based on the query results from the buffer
                    deviceStreams.Add(CreateDeviceStream(device, results));
                }

                if (!deviceStreams.IsNullOrEmpty())
                {
                    // Create MTConnectStreams Document
                    var doc = new Streams.StreamsDocument();
                    doc.Version = Version;

                    var header = GetStreamsHeader();
                    header.Version = GetAgentVersion().ToString();
                    header.FirstSequence = results.FirstSequence;
                    header.LastSequence = results.LastSequence;
                    header.NextSequence = results.NextSequence;

                    doc.Header = header;
                    doc.Streams = deviceStreams.ToList();

                    return doc;
                }
            }

            return null;
        }

        private Streams.DeviceStream CreateDeviceStream(Device device, IStreamingResults dataItemResults)
        {
            if (device != null)
            {
                //var v = version != null ? version : Version;
                var version = Version;

                var deviceStream = new Streams.DeviceStream();
                deviceStream.Name = device.Name;
                deviceStream.Uuid = device.Uuid;
                deviceStream.ComponentStreams = new List<Streams.ComponentStream>();

                var components = device.GetComponents();
                if (!components.IsNullOrEmpty())
                {
                    foreach (var component in components)
                    {
                        // Get All DataItems (Component Root DataItems and Composition DataItems)
                        var dataItems = new List<DataItem>();
                        if (!component.DataItems.IsNullOrEmpty()) dataItems.AddRange(component.DataItems);
                        if (!component.Compositions.IsNullOrEmpty())
                        {
                            foreach (var composition in component.Compositions)
                            {
                                if (!composition.DataItems.IsNullOrEmpty()) dataItems.AddRange(composition.DataItems);
                            }
                        }

                        var componentStream = new Streams.ComponentStream();
                        componentStream.ComponentId = component.Id;
                        componentStream.Component = component.Type;
                        componentStream.Name = component.Name;
                        componentStream.Uuid = component.Uuid;
                        var hasDataItems = false;

                        // Add Samples to ComponentStream
                        var samples = GetSamples(device.Name, dataItemResults, dataItems, version);
                        if (!samples.IsNullOrEmpty())
                        {
                            foreach (var dataItem in samples.OrderBy(o => o.DataItemId).ThenBy(o => o.Sequence))
                            {
                                componentStream.AddSample(dataItem);
                                hasDataItems = true;
                            }
                        }

                        // Add Events to ComponentStream
                        var events = GetEvents(device.Name, dataItemResults, dataItems, version);
                        if (!events.IsNullOrEmpty())
                        {
                            foreach (var dataItem in events.OrderBy(o => o.DataItemId).ThenBy(o => o.Sequence))
                            {
                                componentStream.AddEvent(dataItem);
                                hasDataItems = true;
                            }
                        }

                        // Add Conditions to ComponentStream
                        var conditions = GetConditions(device.Name, dataItemResults, dataItems, version);
                        if (!conditions.IsNullOrEmpty())
                        {
                            foreach (var dataItem in conditions.OrderBy(o => o.DataItemId).ThenBy(o => o.Sequence))
                            {
                                componentStream.AddCondition(dataItem);
                                hasDataItems = true;
                            }
                        }

                        if (hasDataItems)
                        {
                            deviceStream.ComponentStreams.Add(componentStream);
                        }
                    }
                }

                // Add ComponentStream for Device
                var deviceComponentStream = new Streams.ComponentStream();
                deviceComponentStream.ComponentId = device.Id;
                deviceComponentStream.Component = "Device";
                deviceComponentStream.Name = device.Name;
                deviceComponentStream.Uuid = device.Uuid;

                // Add Samples to ComponentStream
                var deviceSamples = GetSamples(device.Name, dataItemResults, device.DataItems, version);
                if (!deviceSamples.IsNullOrEmpty())
                {
                    foreach (var dataItem in deviceSamples.OrderBy(o => o.DataItemId).ThenBy(o => o.Sequence))
                    {
                        deviceComponentStream.AddSample(dataItem);
                    }
                }

                // Add Events to ComponentStream
                var deviceEvents = GetEvents(device.Name, dataItemResults, device.DataItems, version);
                if (!deviceEvents.IsNullOrEmpty())
                {
                    foreach (var dataItem in deviceEvents.OrderBy(o => o.DataItemId).ThenBy(o => o.Sequence))
                    {
                        deviceComponentStream.AddEvent(dataItem);
                    }
                }

                // Add Conditions to ComponentStream
                var deviceConditions = GetConditions(device.Name, dataItemResults, device.DataItems, version);
                if (!deviceConditions.IsNullOrEmpty())
                {
                    foreach (var dataItem in deviceConditions.OrderBy(o => o.DataItemId).ThenBy(o => o.Sequence))
                    {
                        deviceComponentStream.AddCondition(dataItem);
                    }
                }

                deviceStream.ComponentStreams.Add(deviceComponentStream);

                return deviceStream;
            }

            return null;
        }


        #region "Samples"

        private IEnumerable<Streams.Sample> GetSamples(string deviceName, IStreamingResults dataItemResults, IEnumerable<DataItem> dataItems, Version mtconnectVersion = null)
        {
            var objs = new List<Streams.Sample>();

            if (dataItemResults != null && !dataItemResults.Observations.IsNullOrEmpty() && !dataItems.IsNullOrEmpty())
            {
                var filteredDataItems = dataItems.Where(o => o.Category == DataItemCategory.SAMPLE).ToList();
                if (!filteredDataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in filteredDataItems)
                    {
                        var di = DataItem.Create(dataItem.Type);
                        if (di == null) di = dataItem;

                        if (mtconnectVersion >= di.MinimumVersion && mtconnectVersion <= di.MaximumVersion)
                        {
                            // Get list of StoredObservations for the DataItem
                            var observations = dataItemResults.Observations.Where(o => o.DeviceName == deviceName && o.DataItemId == dataItem.Id);
                            if (!observations.IsNullOrEmpty())
                            {
                                foreach (var observation in observations)
                                {
                                    objs.Add(CreateSample(dataItem, observation));
                                }
                            }
                        }
                    }
                }
            }

            return objs;
        }

        private static Streams.Sample CreateSample(DataItem dataItemDefinition, StoredObservation observation)
        {
            var sample = new Streams.Sample();
            sample.DataItemId = dataItemDefinition.Id;
            sample.Type = dataItemDefinition.Type;
            sample.SubType = dataItemDefinition.SubType;
            sample.Name = dataItemDefinition.Name;
            sample.Sequence = observation.Sequence;
            sample.Timestamp = observation.Timestamp.ToDateTime();

            // Set Representation Flags
            switch (dataItemDefinition.Representation)
            {
                case DataItemRepresentation.TIME_SERIES: sample.IsTimeSeries = true; break;
                case DataItemRepresentation.DATA_SET: sample.IsDataSet = true; break;
                case DataItemRepresentation.TABLE: sample.IsTable = true; break;
            }

            // Get ResetTriggered
            sample.ResetTriggered = observation.GetValue(ValueTypes.ResetTriggered).ConvertEnum<Streams.ResetTriggered>();

            // Get Duration
            sample.Duration = observation.GetValue(ValueTypes.Duration).ToDouble();

            if (!observation.Values.IsNullOrEmpty())
            {
                IEnumerable<ObservationValue> observationValues = null;

                switch (dataItemDefinition.Representation)
                {
                    // Representation of VALUE
                    case DataItemRepresentation.VALUE:

                        // Get CDATA
                        sample.CDATA = observation.GetValue(ValueTypes.CDATA);
                        break;

                    // Representation of TIME_SERIES
                    case DataItemRepresentation.TIME_SERIES:

                        // Get SampleRate
                        sample.SampleRate = observation.GetValue(ValueTypes.SampleRate).ToDouble();

                        // Get All TimeSeries Values
                        observationValues = observation.GetValues(ValueTypes.TimeSeriesPrefix);
                        if (!observationValues.IsNullOrEmpty())
                        {
                            var timeSeriesSamples = new List<string>();
                            foreach (var observationValue in observationValues.OrderBy(o => ValueTypes.GetTimeSeriesIndex(o.ValueType)))
                            {
                                if (observationValue.Value != null)
                                {
                                    timeSeriesSamples.Add(observationValue.Value.ToString());
                                }
                            }
                            sample.CDATA = string.Join(" ", timeSeriesSamples);
                        }

                        break;

                    // Representation of DATA_SET
                    case DataItemRepresentation.DATA_SET:

                        // Get All Entry Values
                        observationValues = observation.GetValues(ValueTypes.DataSetPrefix);
                        if (!observationValues.IsNullOrEmpty())
                        {
                            var dataSetEntries = new List<Streams.Entry>();
                            foreach (var observationValue in observationValues)
                            {
                                var key = ValueTypes.GetDataSetKey(observationValue.ValueType);
                                var removed = observationValue.Value == DataSetObservation.EntryRemovedValue;
                                if (removed)
                                {
                                    dataSetEntries.Add(new Streams.Entry(key, null, true));
                                }
                                else
                                {
                                    dataSetEntries.Add(new Streams.Entry(key, observationValue.Value));
                                }
                            }

                            sample.Entries = dataSetEntries;
                        }

                        break;

                    case DataItemRepresentation.TABLE:

                        // Get All Entry Values
                        observationValues = observation.GetValues(ValueTypes.TablePrefix);
                        if (!observationValues.IsNullOrEmpty())
                        {
                            var entries = new List<Streams.Entry>();

                            // Get distinct list of Keys
                            var keys = observationValues.Select(o => ValueTypes.GetTableKey(o.ValueType)).Distinct();
                            foreach (var key in keys.OrderBy(o => o))
                            {
                                var entry = new Streams.Entry();
                                entry.Key = key;

                                // Get list of Items with this Key
                                var keyItems = observationValues.Where(o => ValueTypes.GetTableKey(o.ValueType) == key);
                                if (!keyItems.IsNullOrEmpty())
                                {
                                    var cells = new List<Streams.Cell>();

                                    foreach (var item in keyItems)
                                    {
                                        cells.Add(new Streams.Cell(ValueTypes.GetTableValue(item.ValueType, key), item.Value));
                                    }

                                    entry.Cells = cells;
                                }

                                entries.Add(entry);
                            }

                            sample.Entries = entries;
                        }

                        break;
                }
            }

            return sample;
        }

        #endregion

        #region "Events"

        private IEnumerable<Streams.Event> GetEvents(string deviceName, IStreamingResults dataItemResults, IEnumerable<DataItem> dataItems, Version mtconnectVersion = null)
        {
            var objs = new List<Streams.Event>();

            if (dataItemResults != null && !dataItemResults.Observations.IsNullOrEmpty() && !dataItems.IsNullOrEmpty())
            {
                var filteredDataItems = dataItems.Where(o => o.Category == DataItemCategory.EVENT).ToList();
                if (!filteredDataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in filteredDataItems)
                    {
                        var di = DataItem.Create(dataItem.Type);
                        if (di == null) di = dataItem;

                        if (mtconnectVersion >= di.MinimumVersion && mtconnectVersion <= di.MaximumVersion)
                        {
                            // Get list of StoredObservations for the DataItem
                            var observations = dataItemResults.Observations.Where(o => o.DeviceName == deviceName && o.DataItemId == dataItem.Id);
                            if (!observations.IsNullOrEmpty())
                            {
                                foreach (var observation in observations)
                                {
                                    objs.Add(CreateEvent(dataItem, observation));
                                }
                            }
                        }
                    }
                }
            }

            return objs;
        }

        private static Streams.Event CreateEvent(DataItem dataItemDefinition, StoredObservation observation)
        {
            var e = new Streams.Event();
            e.DataItemId = dataItemDefinition.Id;
            e.Type = dataItemDefinition.Type;
            e.SubType = dataItemDefinition.SubType;
            e.Name = dataItemDefinition.Name;
            e.Sequence = observation.Sequence;
            e.Timestamp = observation.Timestamp.ToDateTime();

            // Set Representation Flags
            switch (dataItemDefinition.Representation)
            {
                case DataItemRepresentation.DATA_SET: e.IsDataSet = true; break;
                case DataItemRepresentation.TABLE: e.IsTable = true; break;
            }

            // Get ResetTriggered
            e.ResetTriggered = observation.GetValue(ValueTypes.ResetTriggered).ConvertEnum<Streams.ResetTriggered>();

            if (!observation.Values.IsNullOrEmpty())
            {
                IEnumerable<ObservationValue> observationValues = null;

                switch (dataItemDefinition.Representation)
                {
                    // Representation of VALUE
                    case DataItemRepresentation.VALUE:

                        // Get CDATA
                        e.CDATA = observation.GetValue(ValueTypes.CDATA);
                        break;

                    // Representation of DATA_SET
                    case DataItemRepresentation.DATA_SET:

                        // Get All Entry Values
                        observationValues = observation.GetValues(ValueTypes.DataSetPrefix);
                        if (!observationValues.IsNullOrEmpty())
                        {
                            var dataSetEntries = new List<Streams.Entry>();
                            foreach (var observationValue in observationValues)
                            {
                                var key = ValueTypes.GetDataSetKey(observationValue.ValueType);
                                var removed = observationValue.Value == DataSetObservation.EntryRemovedValue;
                                if (removed)
                                {
                                    dataSetEntries.Add(new Streams.Entry(key, null, true));
                                }
                                else
                                {
                                    dataSetEntries.Add(new Streams.Entry(key, observationValue.Value));
                                }
                            }

                            e.Entries = dataSetEntries;
                        }

                        break;

                    case DataItemRepresentation.TABLE:

                        // Get All Entry Values
                        observationValues = observation.GetValues(ValueTypes.TablePrefix);
                        if (!observationValues.IsNullOrEmpty())
                        {
                            var entries = new List<Streams.Entry>();

                            // Get distinct list of Keys
                            var keys = observationValues.Select(o => ValueTypes.GetTableKey(o.ValueType)).Distinct();
                            foreach (var key in keys.OrderBy(o => o))
                            {
                                var entry = new Streams.Entry();
                                entry.Key = key;

                                // Get list of Items with this Key
                                var keyItems = observationValues.Where(o => ValueTypes.GetTableKey(o.ValueType) == key);
                                if (!keyItems.IsNullOrEmpty())
                                {
                                    var cells = new List<Streams.Cell>();

                                    foreach (var item in keyItems)
                                    {
                                        cells.Add(new Streams.Cell(ValueTypes.GetTableValue(item.ValueType, key), item.Value));
                                    }

                                    entry.Cells = cells;
                                }

                                entries.Add(entry);
                            }

                            e.Entries = entries;
                        }

                        break;
                }
            }

            return e;
        }

        #endregion

        private IEnumerable<Streams.Condition> GetConditions(string deviceName, IStreamingResults dataItemResults, IEnumerable<DataItem> dataItems, Version mtconnectVersion = null)
        {
            var objs = new List<Streams.Condition>();

            if (dataItemResults != null && !dataItemResults.Observations.IsNullOrEmpty() && !dataItems.IsNullOrEmpty())
            {
                var filteredDataItems = dataItems.Where(o => o.Category == DataItemCategory.CONDITION);
                if (!filteredDataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in filteredDataItems)
                    {
                        var di = DataItem.Create(dataItem.Type);
                        if (di == null) di = dataItem;

                        if (mtconnectVersion >= di.MinimumVersion && mtconnectVersion <= di.MaximumVersion)
                        {
                            // Get list of StoredObservations for the DataItem
                            var observations = dataItemResults.Observations.Where(o => o.DeviceName == deviceName && o.DataItemId == dataItem.Id);
                            if (!observations.IsNullOrEmpty())
                            {
                                foreach (var observation in observations)
                                {
                                    if (!observation.Values.IsNullOrEmpty())
                                    {
                                        var levelValue = observation.Values.FirstOrDefault(o => o.ValueType == ValueTypes.Level).Value?.ToString();
                                        if (!string.IsNullOrEmpty(levelValue))
                                        {
                                            if (Enum.TryParse<Streams.ConditionLevel>(levelValue, true, out var level))
                                            {
                                                var obj = new Streams.Condition();
                                                obj.DataItemId = dataItem.Id;
                                                obj.Type = dataItem.Type;
                                                obj.SubType = dataItem.SubType;
                                                obj.Name = dataItem.Name;
                                                obj.Sequence = observation.Sequence;
                                                obj.Timestamp = observation.Timestamp.ToDateTime();

                                                obj.Level = level;
                                                obj.NativeCode = observation.Values.FirstOrDefault(o => o.ValueType == ValueTypes.NativeCode).Value?.ToString();
                                                obj.NativeSeverity = observation.Values.FirstOrDefault(o => o.ValueType == ValueTypes.NativeSeverity).Value?.ToString();

                                                var qualiferValue = observation.Values.FirstOrDefault(o => o.ValueType == ValueTypes.Qualifier).Value?.ToString();
                                                if (Enum.TryParse<Streams.ConditionQualifier>(qualiferValue, true, out var qualifier))
                                                {
                                                    obj.Qualifier = qualifier;
                                                }

                                                obj.CDATA = observation.Values.FirstOrDefault(o => o.ValueType == ValueTypes.CDATA).Value?.ToString();
                                                objs.Add(obj);
                                            }
                                        }
                                        else
                                        {
                                            // Check if CDATA is only observation set Unavailable
                                            var cdata = observation.Values.FirstOrDefault(o => o.ValueType == ValueTypes.CDATA).Value?.ToString();
                                            if (cdata == Streams.DataItem.Unavailable)
                                            {
                                                var obj = new Streams.Condition();
                                                obj.DataItemId = dataItem.Id;
                                                obj.Type = dataItem.Type;
                                                obj.SubType = dataItem.SubType;
                                                obj.Name = dataItem.Name;
                                                obj.Sequence = observation.Sequence;
                                                obj.Timestamp = observation.Timestamp.ToDateTime();

                                                obj.Level = Streams.ConditionLevel.UNAVAILABLE;
                                                objs.Add(obj);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return objs;
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
        public AssetsDocument GetAssets(string type = null, bool removed = false, int count = 0, Version mtconnectVersion = null)
        {
            AssetsRequestReceived?.Invoke(null);

            if (_assetBuffer != null)
            {
                // Get Assets from AssetsBuffer
                var assets = _assetBuffer.GetAssets(type, removed, count);
                if (!assets.IsNullOrEmpty())
                {
                    // Process Assets
                    var processedAssets = new List<IAsset>();
                    foreach (var asset in assets)
                    {
                        var processedAsset = asset.Process(mtconnectVersion);
                        if (processedAsset != null) processedAssets.Add(processedAsset);
                    }

                    // Create AssetsHeader
                    var header = GetAssetsHeader();
                    header.Version = GetAgentVersion().ToString();
                    header.InstanceId = InstanceId;

                    // Create MTConnectAssets Response Document
                    var document = new AssetsDocument();
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
        public async Task<AssetsDocument> GetAssetsAsync(string type = null, bool removed = false, int count = 0, Version mtconnectVersion = null)
        {
            AssetsRequestReceived?.Invoke(null);

            if (_assetBuffer != null)
            {
                // Get Assets from AssetsBuffer
                var assets = await _assetBuffer.GetAssetsAsync(type, removed, count);
                if (!assets.IsNullOrEmpty())
                {
                    // Process Assets
                    var processedAssets = new List<IAsset>();
                    foreach (var asset in assets)
                    {
                        var processedAsset = asset.Process(mtconnectVersion);
                        if (processedAsset != null) processedAssets.Add(processedAsset);
                    }

                    // Create AssetsHeader
                    var header = GetAssetsHeader();
                    header.Version = GetAgentVersion().ToString();
                    header.InstanceId = InstanceId;

                    // Create MTConnectAssets Response Document
                    var document = new AssetsDocument();
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
        public AssetsDocument GetAsset(string assetId, Version mtconnectVersion = null)
        {
            AssetsRequestReceived?.Invoke(assetId);

            if (_assetBuffer != null)
            {
                // Get Asset from AssetsBuffer
                var asset = _assetBuffer.GetAsset(assetId);
                if (asset != null)
                {
                    // Process Asset
                    var processedAsset = asset.Process(mtconnectVersion);

                    // Create AssetsHeader
                    var header = GetAssetsHeader();
                    header.Version = GetAgentVersion().ToString();
                    header.InstanceId = InstanceId;

                    // Create MTConnectAssets Response Document
                    var document = new AssetsDocument();
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
        public async Task<AssetsDocument> GetAssetAsync(string assetId, Version mtconnectVersion = null)
        {
            AssetsRequestReceived?.Invoke(assetId);

            if (_assetBuffer != null)
            {
                // Get Asset from AssetsBuffer
                var asset = await _assetBuffer.GetAssetAsync(assetId);
                if (asset != null)
                {
                    // Process Asset
                    var processedAsset = asset.Process(mtconnectVersion);

                    // Create AssetsHeader
                    var header = GetAssetsHeader();
                    header.Version = GetAgentVersion().ToString();
                    header.InstanceId = InstanceId;

                    // Create MTConnectAssets Response Document
                    var document = new AssetsDocument();
                    document.Version = Version;
                    document.Header = header;
                    document.Assets = new List<IAsset> { processedAsset };

                    AssetsResponseSent?.Invoke(document);

                    return document;
                }
            }

            return null;
        }

        #endregion

        #region "Errors"

        /// <summary>
        /// Get an MTConnectErrors Document containing the specified ErrorCode
        /// </summary>
        /// <param name="errorCode">Provides a descriptive code that indicates the type of error that was encountered by an Agent when attempting to respond to a Request for information.</param>
        /// <param name="cdata">The CDATA for Error contains a textual description of the error and any additional information an Agent is capable of providing regarding a specific error.</param>
        /// <returns>MTConnectError Response Document</returns>
        public ErrorDocument GetError(ErrorCode errorCode, string cdata = null)
        {
            var doc = new ErrorDocument();
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
        public async Task<ErrorDocument> GetErrorAsync(ErrorCode errorCode, string cdata = null)
        {
            var doc = new ErrorDocument();
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
        public ErrorDocument GetError(IEnumerable<Error> errors)
        {
            var doc = new ErrorDocument();
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
        public async Task<ErrorDocument> GetErrorAsync(IEnumerable<Error> errors)
        {
            var doc = new ErrorDocument();
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

        private void InitializeDataItems(Device device, long timestamp = 0)
        {
            if (device != null && _observationBuffer != null)
            {
                // Get All DataItems for the Device
                var dataItems = device.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    // Get all Current Observations for the Device
                    var results = _observationBuffer.GetObservations(device.Name);

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
                            var valueType = dataItem.Category == DataItemCategory.CONDITION ? ValueTypes.Level : ValueTypes.CDATA;
                            var value = !string.IsNullOrEmpty(dataItem.InitialValue) ? dataItem.InitialValue : Streams.DataItem.Unavailable;

                            // Add Unavailable Observation to ObservationBuffer
                            var observation = new Observation();
                            observation.Timestamp = ts;
                            observation.Values = new List<ObservationValue>
                            {
                                new ObservationValue(valueType, value)
                            };

                            _observationBuffer.AddObservation(device.Name, dataItem, observation);
                        }
                    }
                }
            }
        }

        private async Task InitializeDataItemsAsync(Device device, long timestamp = 0)
        {
            if (device != null && _observationBuffer != null)
            {
                // Get All DataItems for the Device
                var dataItems = device.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    // Get all Current Observations for the Device
                    var results = await _observationBuffer.GetObservationsAsync(device.Name);

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
                            var valueType = dataItem.Category == DataItemCategory.CONDITION ? ValueTypes.Level : ValueTypes.CDATA;
                            var value = !string.IsNullOrEmpty(dataItem.InitialValue) ? dataItem.InitialValue : Streams.DataItem.Unavailable;

                            // Add Unavailable Observation to ObservationBuffer
                            var observation = new Observation();
                            observation.Timestamp = ts;
                            observation.Values = new List<ObservationValue>
                            {
                                new ObservationValue(valueType, value)
                            };

                            await _observationBuffer.AddObservationAsync(device.Name, dataItem, observation);
                        }
                    }
                }
            }
        }

        private bool UpdateCurrentObservation(string deviceName, DataItem dataItem, IObservation observation)
        {
            if (_currentObservations != null && !string.IsNullOrEmpty(deviceName) && dataItem != null)
            {
                var hash = StoredObservation.CreateHash(deviceName, dataItem.Id);

                _currentObservations.TryGetValue(hash, out var existingObservation);
                if (observation != null && existingObservation != null)
                {
                    // Check Filters
                    var update = FilterPeriod(dataItem, observation.Timestamp, existingObservation.Timestamp);
                    if (update) update = FilterDelta(dataItem, observation, existingObservation);

                    if (update)
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

        private bool UpdateCurrentCondition(string deviceName, DataItem dataItem, IObservation observation)
        {
            if (_currentConditions != null && observation != null && !string.IsNullOrEmpty(deviceName) && dataItem != null)
            {
                var hash = StoredObservation.CreateHash(deviceName, dataItem.Id);
                var observations = new List<IObservation>();
                observations.Add(observation);

                _currentConditions.TryGetValue(hash, out var existingObservations);
                if (observation != null && !existingObservations.IsNullOrEmpty())
                {
                    var conditionLevel = observation.GetValue(ValueTypes.Level);
                    if (conditionLevel != Streams.ConditionLevel.NORMAL.ToString() && 
                        conditionLevel != Streams.ConditionLevel.UNAVAILABLE.ToString())
                    {
                        observations.InsertRange(0, existingObservations);
                    }
                }

                _currentConditions.TryRemove(hash, out var _);
                return _currentConditions.TryAdd(hash, observations);
            }

            return false;
        }

        private static bool FilterPeriod(DataItem dataItem, long newTimestamp, long existingTimestamp)
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

        private static bool FilterDelta(DataItem dataItem, IObservation newObservation, IObservation existingObservation)
        {
            if (dataItem != null)
            {
                if (newObservation != existingObservation)
                {
                    if (!dataItem.Filters.IsNullOrEmpty() && dataItem.Representation == DataItemRepresentation.VALUE)
                    {
                        foreach (var filter in dataItem.Filters)
                        {
                            if (filter.Type == DataItemFilterType.MINIMUM_DELTA)
                            {
                                if (filter.Value > 0)
                                {
                                    var x = newObservation.GetValue(ValueTypes.CDATA).ToDouble();
                                    var y = existingObservation.GetValue(ValueTypes.CDATA).ToDouble();

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


        private DataItem GetDataItemFromKey(string deviceName, string key)
        {
            if (!string.IsNullOrEmpty(deviceName) && !string.IsNullOrEmpty(key))
            {
                // Get Device From DeviceBuffer
                var device = _deviceBuffer.GetDevice(deviceName);
                return GetDataItemFromKey(device, key);
            }

            return null;
        }

        private async Task<DataItem> GetDataItemFromKeyAsync(string deviceName, string key)
        {
            if (!string.IsNullOrEmpty(deviceName) && !string.IsNullOrEmpty(key))
            {
                // Get Device From DeviceBuffer
                var device = await _deviceBuffer.GetDeviceAsync(deviceName);
                return GetDataItemFromKey(device, key);
            }

            return null;
        }

        private DataItem GetDataItemFromKey(Device device, string key)
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

        private Device NormalizeDevice(Device device)
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
                    if (obj.DataItems.IsNullOrEmpty() || !obj.DataItems.Any(o => o.Type == Devices.Events.AvailabilityDataItem.TypeId))
                    {
                        var availability = new Devices.Events.AvailabilityDataItem(obj.Id);
                        availability.Name = Devices.Events.AvailabilityDataItem.NameId;
                        obj.DataItems.Add(availability);
                    }

                    // Add Required AssetChanged DataItem
                    if (obj.DataItems.IsNullOrEmpty() || !obj.DataItems.Any(o => o.Type == Devices.Events.AssetChangedDataItem.TypeId))
                    {
                        var assetChanged = new Devices.Events.AssetChangedDataItem(obj.Id);
                        assetChanged.Name = Devices.Events.AssetChangedDataItem.NameId;
                        obj.DataItems.Add(assetChanged);
                    }

                    // Add Required AssetRemoved DataItem
                    if (obj.DataItems.IsNullOrEmpty() || !obj.DataItems.Any(o => o.Type == Devices.Events.AssetRemovedDataItem.TypeId))
                    {
                        var assetRemoved = new Devices.Events.AssetRemovedDataItem(obj.Id);
                        assetRemoved.Name = Devices.Events.AssetRemovedDataItem.NameId;
                        obj.DataItems.Add(assetRemoved);
                    }

                    return obj;
                } 
            }

            return null;
        }

        private List<Component> NormalizeComponents(IEnumerable<Component> components)
        {
            if (!components.IsNullOrEmpty())
            {
                var objs = new List<Component>();

                foreach (var component in components)
                {
                    var obj = new Component();
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

            return new List<Component>();
        }

        private List<Composition> NormalizeCompositions(IEnumerable<Composition> compositions)
        {
            if (!compositions.IsNullOrEmpty())
            {
                var objs = new List<Composition>();

                foreach (var composition in compositions)
                {
                    var obj = new Composition();
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

            return new List<Composition>();
        }

        private List<DataItem> NormalizeDataItems(IEnumerable<DataItem> dataItems)
        {
            if (!dataItems.IsNullOrEmpty())
            {
                var objs = new List<DataItem>();

                foreach (var dataItem in dataItems)
                {
                    var obj = new DataItem();
                    obj.Category = dataItem.Category;
                    obj.Id = dataItem.Id;
                    obj.Name = dataItem.Name;
                    obj.Type = dataItem.Type;
                    obj.SubType = dataItem.SubType;
                    obj.NativeUnits = dataItem.NativeUnits;
                    obj.NativeScale = dataItem.NativeScale;
                    obj.SampleRate = dataItem.SampleRate;
                    obj.Source = dataItem.Source;
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

            return new List<DataItem>();
        }

        private Description NormalizeDescription(Description description)
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



        private bool AddDeviceAddedObservation(Device device, long timestamp = 0)
        {
            if (device != null && _observationBuffer != null)
            {
                var dataItems = device.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    var dataItem = dataItems.FirstOrDefault(o => o.Type == Devices.Events.DeviceAddedDataItem.TypeId);
                    if (dataItem != null)
                    {
                        // Create new Observation
                        var observation = new Observation();
                        observation.Values = new List<ObservationValue>
                        {
                            new ObservationValue(ValueTypes.CDATA, device.Uuid)
                        };

                        // Add to Streaming Buffer
                        return _observationBuffer.AddObservation(device.Name, dataItem, observation);
                    }
                }
            }

            return false;
        }

        private async Task<bool> AddDeviceAddedObservationAsync(Device device, long timestamp = 0)
        {
            if (device != null && _observationBuffer != null)
            {
                var dataItems = device.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    var dataItem = dataItems.FirstOrDefault(o => o.Type == Devices.Events.DeviceAddedDataItem.TypeId);
                    if (dataItem != null)
                    {
                        // Create new Observation
                        var observation = new Observation();
                        observation.Values = new List<ObservationValue>
                        {
                            new ObservationValue(ValueTypes.CDATA, device.Uuid)
                        };

                        // Add to Streaming Buffer
                        return await _observationBuffer.AddObservationAsync(device.Name, dataItem, observation);
                    }
                }
            }

            return false;
        }


        private bool AddDeviceChangedObservation(Device device, long timestamp = 0)
        {
            if (device != null && _observationBuffer != null)
            {
                var dataItems = device.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    var dataItem = dataItems.FirstOrDefault(o => o.Type == Devices.Events.DeviceChangedDataItem.TypeId);
                    if (dataItem != null)
                    {
                        // Create new Observation
                        var observation = new Observation();
                        observation.Values = new List<ObservationValue>
                        {
                            new ObservationValue(ValueTypes.CDATA, device.Uuid)
                        };

                        // Add to Streaming Buffer
                        return _observationBuffer.AddObservation(device.Name, dataItem, observation);
                    }
                }
            }

            return false;
        }

        private async Task<bool> AddDeviceChangedObservationAsync(Device device, long timestamp = 0)
        {
            if (device != null && _observationBuffer != null)
            {
                var dataItems = device.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    var dataItem = dataItems.FirstOrDefault(o => o.Type == Devices.Events.DeviceChangedDataItem.TypeId);
                    if (dataItem != null)
                    {
                        // Create new Observation
                        var observation = new Observation();
                        observation.Values = new List<ObservationValue>
                        {
                            new ObservationValue(ValueTypes.CDATA, device.Uuid)
                        };

                        // Add to Streaming Buffer
                        return await _observationBuffer.AddObservationAsync(device.Name, dataItem, observation);
                    }
                }
            }

            return false;
        }


        private bool AddDeviceRemovedObservation(Device device, long timestamp = 0)
        {
            if (device != null && _observationBuffer != null)
            {
                var dataItems = device.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    var dataItem = dataItems.FirstOrDefault(o => o.Type == Devices.Events.DeviceRemovedDataItem.TypeId);
                    if (dataItem != null)
                    {
                        // Create new Observation
                        var observation = new Observation();
                        observation.Values = new List<ObservationValue>
                        {
                            new ObservationValue(ValueTypes.CDATA, device.Uuid)
                        };

                        // Add to Streaming Buffer
                        return _observationBuffer.AddObservation(device.Name, dataItem, observation);
                    }
                }
            }

            return false;
        }

        private async Task<bool> AddDeviceRemovedObservationAsync(Device device, long timestamp = 0)
        {
            if (device != null && _observationBuffer != null)
            {
                var dataItems = device.GetDataItems();
                if (!dataItems.IsNullOrEmpty())
                {
                    var dataItem = dataItems.FirstOrDefault(o => o.Type == Devices.Events.DeviceRemovedDataItem.TypeId);
                    if (dataItem != null)
                    {
                        // Create new Observation
                        var observation = new Observation();
                        observation.Values = new List<ObservationValue>
                        {
                            new ObservationValue(ValueTypes.CDATA, device.Uuid)
                        };

                        // Add to Streaming Buffer
                        return await _observationBuffer.AddObservationAsync(device.Name, dataItem, observation);
                    }
                }
            }

            return false;
        }

        #endregion


        /// <summary>
        /// Add a new MTConnectDevice to the Agent's Buffer
        /// </summary>
        public bool AddDevice(Device device)
        {
            if (device != null && _deviceBuffer != null)
            {
                // Create new object (to validate and prevent derived classes that won't serialize right with XML)
                var obj = NormalizeDevice(device);

                // Get Existing Device (if exists)
                var existingDevice = _deviceBuffer.GetDevice(obj.Name);

                // Check if Device Already Exists in the Device Buffer and is changed
                if (existingDevice != null && obj.ChangeId == existingDevice.ChangeId)
                {
                    return true;
                }

                // Add the Device to the Buffer
                var success = _deviceBuffer.AddDevice(obj);
                if (success)
                {
                    if (existingDevice != null)
                    {
                        AddDeviceRemovedObservation(obj);
                        AddDeviceChangedObservation(obj);
                    }
                    else
                    {
                        AddDeviceAddedObservation(obj);
                    }

                    InitializeDataItems(obj);

                    DeviceAdded?.Invoke(this, obj);
                }

                return success;
            }

            return false;
        }

        /// <summary>
        /// Add a new MTConnectDevice to the Agent's Buffer
        /// </summary>
        public async Task<bool> AddDeviceAsync(Device device)
        {
            if (device != null && _deviceBuffer != null)
            {
                // Create new object (to validate and prevent derived classes that won't serialize right with XML)
                var obj = NormalizeDevice(device);

                // Get Existing Device (if exists)
                var existingDevice = await _deviceBuffer.GetDeviceAsync(obj.Name);

                // Check if Device Already Exists in the Device Buffer and is changed
                if (existingDevice != null && obj.ChangeId == existingDevice.ChangeId)
                {
                    return true;
                }

                // Add the Device to the Buffer
                var success = await _deviceBuffer.AddDeviceAsync(obj);
                if (success)
                {
                    if (existingDevice != null)
                    {
                        await AddDeviceRemovedObservationAsync(obj);
                        await AddDeviceChangedObservationAsync(obj);
                    }
                    else
                    {
                        await AddDeviceAddedObservationAsync(obj);
                    }

                    await InitializeDataItemsAsync(obj);

                    DeviceAdded?.Invoke(this, obj);
                }

                return success;
            }

            return false;
        }

        /// <summary>
        /// Add new MTConnectDevices to the Agent's Buffer
        /// </summary>
        public bool AddDevices(IEnumerable<Device> devices)
        {
            if (!devices.IsNullOrEmpty() && _deviceBuffer != null)
            {
                bool success = false;

                foreach (var device in devices)
                {
                    success = AddDevice(device);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        /// <summary>
        /// Add new MTConnectDevices to the Agent's Buffer
        /// </summary>
        public async Task<bool> AddDevicesAsync(IEnumerable<Device> devices)
        {
            if (!devices.IsNullOrEmpty() && _deviceBuffer != null)
            {
                bool success = false;

                foreach (var device in devices)
                {
                    success = await AddDeviceAsync(device);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        #endregion

        #region "Observations"

        #region "Internal"

        private IObservation ConvertObservationValue(DataItem dataItem, IObservation observation)
        {
            if (dataItem != null && observation != null)
            {
                if (dataItem.Category == DataItemCategory.SAMPLE)
                {
                    // Get the CDATA Value
                    var cdata = observation.GetValue(ValueTypes.CDATA);
                    if (!string.IsNullOrEmpty(cdata))
                    {
                        var units = dataItem.Units;
                        var nativeUnits = dataItem.NativeUnits;
                        var value = cdata.ToDouble();

                        // Get the SampleValue for the DataItem Type
                        if (dataItem.Units == Units.DEGREE_3D)
                        {
                            // Remove the "_3D" suffix from the Units and NativeUnits
                            units = Remove3dSuffix(units);
                            nativeUnits = Remove3dSuffix(nativeUnits);

                            // Create a new Degree3D object to parse the CDATA
                            var degree3d = Streams.Degree3D.FromString(cdata);
                            degree3d.A = Streams.DataItem.ConvertUnits(degree3d.A, units, nativeUnits);
                            degree3d.B = Streams.DataItem.ConvertUnits(degree3d.B, units, nativeUnits);
                            degree3d.C = Streams.DataItem.ConvertUnits(degree3d.C, units, nativeUnits);

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
                        else if (dataItem.Units == Units.MILLIMETER_3D || dataItem.Units == Units.UNIT_VECTOR_3D)
                        {
                            // Remove the "_3D" suffix from the Units and NativeUnits
                            units = Remove3dSuffix(units);
                            nativeUnits = Remove3dSuffix(nativeUnits);

                            // Create a new Position3D object to parse the CDATA
                            var position3d = Streams.Position3D.FromString(cdata);
                            position3d.X = Streams.DataItem.ConvertUnits(position3d.X, units, nativeUnits);
                            position3d.Y = Streams.DataItem.ConvertUnits(position3d.Y, units, nativeUnits);
                            position3d.Z = Streams.DataItem.ConvertUnits(position3d.Z, units, nativeUnits);

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
                        else
                        {
                            // Directly convert the Units if no SampleValue class is found
                            value = Streams.DataItem.ConvertUnits(cdata.ToDouble(), units, nativeUnits);

                            // Apply the NativeScale
                            if (dataItem.NativeScale > 0) value = value / dataItem.NativeScale;

                            // Set CDATA to value
                            cdata = value.ToString();
                        }

                        // Replace the CDATA value in the Observation
                        observation.AddValue(ValueTypes.CDATA, cdata);
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
        public bool AddObservation(string deviceName, string dataItemId, object value)
        {
            return AddObservation(deviceName, new Observation
            {
                DeviceName = deviceName,
                Key = dataItemId,
                Values = new List<ObservationValue> { new ObservationValue(ValueTypes.CDATA, value) },
                Timestamp = UnixDateTime.Now
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem of category EVENT or SAMPLE to the Agent
        /// </summary>
        public async Task<bool> AddObservationAsync(string deviceName, string dataItemId, object value)
        {
            return await AddObservationAsync(deviceName, new Observation
            {
                DeviceName = deviceName,
                Key = dataItemId,
                Values = new List<ObservationValue> { new ObservationValue(ValueTypes.CDATA, value) },
                Timestamp = UnixDateTime.Now
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem of category EVENT or SAMPLE to the Agent
        /// </summary>
        public bool AddObservation(string deviceName, string dataItemId, object value, long timestamp)
        {
            return AddObservation(deviceName, new Observation
            {
                DeviceName = deviceName,
                Key = dataItemId,
                Values = new List<ObservationValue> { new ObservationValue(ValueTypes.CDATA, value) },
                Timestamp = timestamp
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem of category EVENT or SAMPLE to the Agent
        /// </summary>
        public async Task<bool> AddObservationAsync(string deviceName, string dataItemId, object value, long timestamp)
        {
            return await AddObservationAsync(deviceName, new Observation
            {
                DeviceName = deviceName,
                Key = dataItemId,
                Values = new List<ObservationValue> { new ObservationValue(ValueTypes.CDATA, value) },
                Timestamp = timestamp
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem of category EVENT or SAMPLE to the Agent
        /// </summary>
        public bool AddObservation(string deviceName, string dataItemId, object value, DateTime timestamp)
        {
            return AddObservation(deviceName, new Observation
            {
                DeviceName = deviceName,
                Key = dataItemId,
                Values = new List<ObservationValue> { new ObservationValue(ValueTypes.CDATA, value) },
                Timestamp = timestamp.ToUnixTime()
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem of category EVENT or SAMPLE to the Agent
        /// </summary>
        public async Task<bool> AddObservationAsync(string deviceName, string dataItemId, object value, DateTime timestamp)
        {
            return await AddObservationAsync(deviceName, new Observation
            {
                DeviceName = deviceName,
                Key = dataItemId,
                Values = new List<ObservationValue> { new ObservationValue(ValueTypes.CDATA, value) },
                Timestamp = timestamp.ToUnixTime()
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem to the Agent
        /// </summary>
        public bool AddObservation(string deviceName, string dataItemId, string valueType, object value)
        {
            return AddObservation(deviceName, new Observation
            {
                DeviceName = deviceName,
                Key = dataItemId,
                Values = new List<ObservationValue> { new ObservationValue(valueType, value) },
                Timestamp = UnixDateTime.Now
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem to the Agent
        /// </summary>
        public async Task<bool> AddObservationAsync(string deviceName, string dataItemId, string valueType, object value)
        {
            return await AddObservationAsync(deviceName, new Observation
            {
                DeviceName = deviceName,
                Key = dataItemId,
                Values = new List<ObservationValue> { new ObservationValue(valueType, value) },
                Timestamp = UnixDateTime.Now
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem to the Agent
        /// </summary>
        public bool AddObservation(string deviceName, string dataItemId, string valueType, object value, long timestamp)
        {
            return AddObservation(deviceName, new Observation
            {
                DeviceName = deviceName,
                Key = dataItemId,
                Values = new List<ObservationValue> { new ObservationValue(valueType, value) },
                Timestamp = timestamp
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem to the Agent
        /// </summary>
        public async Task<bool> AddObservationAsync(string deviceName, string dataItemId, string valueType, object value, long timestamp)
        {
            return await AddObservationAsync(deviceName, new Observation
            {
                DeviceName = deviceName,
                Key = dataItemId,
                Values = new List<ObservationValue> { new ObservationValue(valueType, value) },
                Timestamp = timestamp
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem to the Agent
        /// </summary>
        public bool AddObservation(string deviceName, string dataItemId, string valueType, object value, DateTime timestamp)
        {
            return AddObservation(deviceName, new Observation
            {
                DeviceName = deviceName,
                Key = dataItemId,
                Values = new List<ObservationValue> { new ObservationValue(valueType, value) },
                Timestamp = timestamp.ToUnixTime()
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem of to the Agent
        /// </summary>
        public async Task<bool> AddObservationAsync(string deviceName, string dataItemId, string valueType, object value, DateTime timestamp)
        {
            return await AddObservationAsync(deviceName, new Observation
            {
                DeviceName = deviceName,
                Key = dataItemId,
                Values = new List<ObservationValue> { new ObservationValue(valueType, value) },
                Timestamp = timestamp.ToUnixTime()
            });
        }

        /// <summary>
        /// Add a new Observation for a DataItem to the Agent
        /// </summary>
        public bool AddObservation(string deviceName, IObservation observation)
        {
            if (observation != null)
            {
                observation.DeviceName = deviceName;
                var timestamp = observation.Timestamp > 0 ? observation.Timestamp : UnixDateTime.Now;

                // Get DataItem based on Observation's Key
                var dataItem = GetDataItemFromKey(deviceName, observation.Key);
                if (dataItem != null)
                {
                    // Get Derived DataItem by Type
                    var di = DataItem.Create(dataItem.Type);
                    if (di != null)
                    {
                        di.Category = dataItem.Category;
                        di.Id = dataItem.Id;
                        di.Name = dataItem.Name;
                        di.Type = dataItem.Type;
                        di.SubType = dataItem.SubType;
                        di.NativeUnits = dataItem.NativeUnits;
                        di.NativeScale = dataItem.NativeScale;
                        di.SampleRate = dataItem.SampleRate;
                        di.Source = dataItem.Source;
                        di.Relationships = dataItem.Relationships;
                        di.Representation = dataItem.Representation;
                        di.ResetTrigger = dataItem.ResetTrigger;
                        di.CoordinateSystem = dataItem.CoordinateSystem;
                        di.Constraints = dataItem.Constraints;
                        di.Definition = dataItem.Definition;
                        di.Units = dataItem.Units;
                        di.Statistic = dataItem.Statistic;
                        di.SignificantDigits = dataItem.SignificantDigits;
                        di.Filters = dataItem.Filters;
                        di.InitialValue = dataItem.InitialValue;
                        dataItem = di;
                    }

                    // Validate Observation with DataItem type
                    var validationResults = dataItem.IsValid(Version, observation);
                    if (validationResults.IsValid)
                    {
                        // Convert Units (if needed)
                        observation = ConvertObservationValue(dataItem, observation);

                        bool update;

                        // Check if Observation Needs to be Updated
                        if (dataItem.Category == DataItemCategory.CONDITION)
                        {
                            update = UpdateCurrentCondition(deviceName, dataItem, observation);
                        }
                        else
                        {
                            update = UpdateCurrentObservation(deviceName, dataItem, observation);
                        }

                        // Check if Observation Needs to be Updated
                        if (update)
                        {
                            // Add Observation to Streaming Buffer
                            if (_observationBuffer.AddObservation(deviceName, dataItem, observation))
                            {
                                if (dataItem.Type != Devices.Samples.ObservationUpdateRateDataItem.TypeId &&
                                    dataItem.Type != Devices.Samples.AssetUpdateRateDataItem.TypeId)
                                {
                                    // Update Agent Metrics
                                    _metrics.UpdateObservation(deviceName, dataItem.Id);
                                }

                                ObservationAdded?.Invoke(this, observation);

                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (InvalidDataItemAdded != null) InvalidDataItemAdded.Invoke(dataItem, validationResults);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Add a new Observation for a DataItem to the Agent
        /// </summary>
        public async Task<bool> AddObservationAsync(string deviceName, IObservation observation)
        {
            if (observation != null)
            {
                observation.DeviceName = deviceName;
                var timestamp = observation.Timestamp > 0 ? observation.Timestamp : UnixDateTime.Now;

                // Get DataItem based on Observation's Key
                var dataItem = await GetDataItemFromKeyAsync(deviceName, observation.Key);
                if (dataItem != null)
                {
                    // Get Derived DataItem by Type
                    var di = DataItem.Create(dataItem.Type);
                    if (di != null)
                    {
                        di.Category = dataItem.Category;
                        di.Id = dataItem.Id;
                        di.Name = dataItem.Name;
                        di.Type = dataItem.Type;
                        di.SubType = dataItem.SubType;
                        di.NativeUnits = dataItem.NativeUnits;
                        di.NativeScale = dataItem.NativeScale;
                        di.SampleRate = dataItem.SampleRate;
                        di.Source = dataItem.Source;
                        di.Relationships = dataItem.Relationships;
                        di.Representation = dataItem.Representation;
                        di.ResetTrigger = dataItem.ResetTrigger;
                        di.CoordinateSystem = dataItem.CoordinateSystem;
                        di.Constraints = dataItem.Constraints;
                        di.Definition = dataItem.Definition;
                        di.Units = dataItem.Units;
                        di.Statistic = dataItem.Statistic;
                        di.SignificantDigits = dataItem.SignificantDigits;
                        di.Filters = dataItem.Filters;
                        di.InitialValue = dataItem.InitialValue;
                        dataItem = di;
                    }

                    // Validate Observation with DataItem type
                    var validationResults = dataItem.IsValid(Version, observation);
                    if (validationResults.IsValid)
                    {
                        // Convert Units (if needed)
                        observation = ConvertObservationValue(dataItem, observation);

                        bool update;

                        // Check if Observation Needs to be Updated
                        if (dataItem.Category == DataItemCategory.CONDITION)
                        {
                            update = UpdateCurrentCondition(deviceName, dataItem, observation);
                        }
                        else
                        {
                            update = UpdateCurrentObservation(deviceName, dataItem, observation);
                        }

                        // Check if Observation Needs to be Updated
                        if (update)
                        {
                            if (observation.GetValue(ValueTypes.ResetTriggered) != null)
                            {
                                if (update)
                                {

                                }
                            }

                            // Add Observation to Streaming Buffer
                            if (_observationBuffer.AddObservation(deviceName, dataItem, observation))
                            {
                                if (dataItem.Type != Devices.Samples.ObservationUpdateRateDataItem.TypeId &&
                                    dataItem.Type != Devices.Samples.AssetUpdateRateDataItem.TypeId)
                                {
                                    // Update Agent Metrics
                                    _metrics.UpdateObservation(deviceName, dataItem.Id);
                                }

                                ObservationAdded?.Invoke(this, observation);

                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (InvalidDataItemAdded != null) InvalidDataItemAdded.Invoke(dataItem, validationResults);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Add new Observations for DataItems to the Agent
        /// </summary>
        public bool AddObservations(string deviceName, IEnumerable<IObservation> observations)
        {
            if (!observations.IsNullOrEmpty())
            {
                bool success = false;

                foreach (var observation in observations)
                {
                    success = AddObservation(deviceName, observation);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        /// <summary>
        /// Add new Observations for DataItems to the Agent
        /// </summary>
        public async Task<bool> AddObservationsAsync(string deviceName, IEnumerable<IObservation> observations)
        {
            if (!observations.IsNullOrEmpty())
            {
                bool success = false;

                foreach (var observation in observations)
                {
                    success = await AddObservationAsync(deviceName, observation);
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
        public bool AddAsset(string deviceName, IAsset asset)
        {
            if (_deviceBuffer != null && _assetBuffer != null)
            {
                // Get Device from DeviceBuffer
                var device = _deviceBuffer.GetDevice(deviceName);
                if (device != null)
                {
                    // Set Device UUID Property
                    asset.DeviceUuid = device.Uuid;

                    // Validate Asset based on Device's MTConnectVersion
                    var validationResults = asset.IsValid(device.MTConnectVersion);
                    if (validationResults.IsValid)
                    {
                        // Update ASSET_CHANGED for device
                        var assetChangedId = DataItem.CreateId(device.Id, Devices.Events.AssetChangedDataItem.NameId);
                        AddObservation(deviceName, assetChangedId, ValueTypes.CDATA, asset.AssetId, asset.Timestamp);

                        // Add Asset to AssetBuffer
                        return _assetBuffer.AddAsset(asset);
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
        public async Task<bool> AddAssetAsync(string deviceName, IAsset asset)
        {
            if (_assetBuffer != null)
            {
                // Get Device from DeviceBuffer
                var device = await _deviceBuffer.GetDeviceAsync(deviceName);
                if (device != null)
                {
                    // Set Device UUID Property
                    asset.DeviceUuid = device.Uuid;

                    // Validate Asset based on Device's MTConnectVersion
                    var validationResults = asset.IsValid(device.MTConnectVersion);
                    if (validationResults.IsValid)
                    {
                        // Update ASSET_CHANGED for device
                        var assetChangedId = DataItem.CreateId(device.Id, Devices.Events.AssetChangedDataItem.NameId);
                        AddObservation(deviceName, assetChangedId, ValueTypes.CDATA, asset.AssetId, asset.Timestamp);

                        // Add Asset to AssetBuffer
                        return await _assetBuffer.AddAssetAsync(asset);
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
        public bool AddAssets(string deviceName, IEnumerable<IAsset> assets)
        {
            if (_assetBuffer != null && !assets.IsNullOrEmpty())
            {
                var success = false;

                foreach (var asset in assets)
                {
                    success = AddAsset(deviceName, asset);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        /// <summary>
        /// Add new Assets to the Agent
        /// </summary>
        public async Task<bool> AddAssetsAsync(string deviceName, IEnumerable<IAsset> assets)
        {
            if (_assetBuffer != null && !assets.IsNullOrEmpty())
            {
                var success = false;

                foreach (var asset in assets)
                {
                    success = await AddAssetAsync(deviceName, asset);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        #endregion

        #region "DeviceModels"

        /// <summary>
        /// Add a new DeviceModel to the Agent's Buffer. This adds all of the data contained in the Device Model (Device and Observations).
        /// </summary>
        public bool AddDeviceModel(DeviceModel deviceModel)
        {
            if (deviceModel != null)
            {
                bool success;

                success = AddDevice(deviceModel);
                if (success) success = AddObservations(deviceModel.Name, deviceModel.GetObservations());

                return success;
            }

            return false;
        }

        /// <summary>
        /// Add a new DeviceModel to the Agent's Buffer. This adds all of the data contained in the Device Model (Device and Observations).
        /// </summary>
        public async Task<bool> AddDeviceModelAsync(DeviceModel deviceModel)
        {
            if (deviceModel != null)
            {
                bool success;

                success = await AddDeviceAsync(deviceModel);
                if (success) success = await AddObservationsAsync(deviceModel.Name, deviceModel.GetObservations());

                return success;
            }

            return false;
        }

        /// <summary>
        /// Add new DeviceModels to the Agent's Buffer. This adds all of the data contained in the Device Models (Device and Observations).
        /// </summary>
        public bool AddDeviceModels(IEnumerable<DeviceModel> deviceModels)
        {
            if (!deviceModels.IsNullOrEmpty())
            {
                var success = false;

                foreach (var deviceModel in deviceModels)
                {
                    success = AddDeviceModel(deviceModel);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        /// <summary>
        /// Add new DeviceModels to the Agent's Buffer. This adds all of the data contained in the Device Models (Device and Observations).
        /// </summary>
        public async Task<bool> AddDeviceModelsAsync(IEnumerable<DeviceModel> deviceModels)
        {
            if (!deviceModels.IsNullOrEmpty())
            {
                var success = false;

                foreach (var deviceModel in deviceModels)
                {
                    success = await AddDeviceModelAsync(deviceModel);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }

        #endregion

        #endregion

        #region "Metrics"

        private void DeviceMetricsUpdated(object sender, DeviceMetrics deviceMetrics)
        {
            if (deviceMetrics != null && _deviceBuffer != null)
            {
                var device = _deviceBuffer.GetDevice(deviceMetrics.DeviceName);
                if (device != null)
                {
                    var dataItems = device.GetDataItems();
                    if (!dataItems.IsNullOrEmpty())
                    {
                        // Update ObservationUpdateRate DataItem
                        var observationUpdateRate = dataItems.FirstOrDefault(o => o.Type == Devices.Samples.ObservationUpdateRateDataItem.TypeId);
                        if (observationUpdateRate != null)
                        {
                            AddObservation(device.Name, observationUpdateRate.Id, ValueTypes.CDATA, deviceMetrics.ObservationAverage);
                        }

                        // Update ObservationUpdateRate DataItem
                        var assetUpdateRate = dataItems.FirstOrDefault(o => o.Type == Devices.Samples.AssetUpdateRateDataItem.TypeId);
                        if (assetUpdateRate != null)
                        {
                            AddObservation(device.Name, assetUpdateRate.Id, ValueTypes.CDATA, deviceMetrics.AssetAverage);
                        }
                    }
                }
            }
        }

        #endregion

    }
}
