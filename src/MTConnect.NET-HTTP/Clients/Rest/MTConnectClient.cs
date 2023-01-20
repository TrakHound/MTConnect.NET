// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Http;
using MTConnect.Observations;
using MTConnect.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients.Rest
{
    /// <summary>
    /// Client that implements the full MTConnect REST Api Protocol (Probe, Current, Sample Stream, and Assets)
    /// </summary>
    public class MTConnectClient : IMTConnectClient
    {
        private readonly Dictionary<string, IDevice> _devices = new Dictionary<string, IDevice>();
        private readonly object _lock = new object();

        private CancellationTokenSource _stop;
        private MTConnectHttpClientStream _stream;
        private long _lastInstanceId;
        private long _lastSequence;
        private long _lastResponse;
        private bool _initializeFromBuffer;
        private string _streamPath;


        /// <summary>
        /// Initializes a new instance of the MTConnectClient class that is used to perform
        /// the full request and stream protocol from an MTConnect Agent using the MTConnect REST Api protocol
        /// </summary>
        /// <param name="authority">
        /// The authority portion consists of the DNS name or IP address associated with an Agent and an optional
        /// TCP port number[:port] that the Agent is listening to for incoming Requests from client software applications.
        /// If the port number is the default Port 80, port is not required.
        /// </param>
        public MTConnectClient(string authority, string device = null, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            Id = Guid.NewGuid().ToString();
            Init();
            Authority = authority;
            Device = device;
            DocumentFormat = documentFormat;
        }

        private void Init()
        {
            Interval = 500;
            Timeout = 5000;
            Heartbeat = 1000;
            MaximumSampleCount = 1000;
            RetryInterval = 10000;
            ContentEncodings = HttpContentEncodings.DefaultAccept;
            ContentType = MimeTypes.Get(DocumentFormat);
        }


        /// <summary>
        /// A unique Identifier used to indentify this instance of the MTConnectClient class
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The authority portion consists of the DNS name or IP address associated with an Agent and an optional
        /// TCP port number[:port] that the Agent is listening to for incoming Requests from client software applications.
        /// If the port number is the default Port 80, port is not required.
        /// </summary>
        public string Authority { get; }

        /// <summary>
        /// If present, specifies that only the Equipment Metadata for the piece of equipment represented by the name or uuid will be published.
        /// If not present, Metadata for all pieces of equipment associated with the Agent will be published.
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        /// Gets or Sets the Document Format to use
        /// </summary>
        public string DocumentFormat { get; set; }

        /// <summary>
        /// Gets or Sets the Interval in Milliseconds for the Sample Stream
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Gets or Sets the connection timeout for the request
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Gets or Sets the MTConnect Agent Heartbeat for the request
        /// </summary>
        public int Heartbeat { get; set; }

        /// <summary>
        /// Gets or Sets the Interval in Milliseconds that the Client will attempt to reconnect if the connection fails
        /// </summary>
        public int RetryInterval { get; set; }

        /// <summary>
        /// Gets or Sets the Maximum Number of Samples returned per interval from the Sample Stream
        /// </summary>
        public int MaximumSampleCount { get; set; }

        /// <summary>
        /// Gets or Sets the List of Encodings (ex. gzip, br, deflate) to pass to the Accept-Encoding HTTP Header
        /// </summary>
        public IEnumerable<HttpContentEncoding> ContentEncodings { get; set; }

        /// <summary>
        /// Gets or Sets the Content-type (or MIME-type) to pass to the Accept HTTP Header
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets the Last Instance ID read from the MTConnect Agent
        /// </summary>
        public long LastInstanceId => _lastInstanceId;

        /// <summary>
        /// Gets the Last Sequence read from the MTConnect Agent
        /// </summary>
        public long LastSequence => _lastSequence;

        /// <summary>
        /// Gets the Unix Timestamp (in Milliseconds) since the last response from the MTConnect Agent
        /// </summary>
        public long LastResponse => _lastResponse;

        /// <summary>
        /// Gets or Sets whether the stream requests a Current (true) or a Sample (false)
        /// </summary>
        public bool CurrentOnly { get; set; }


        #region "Events"

        /// <summary>
        /// Raised when an MTConnectDevices Document is received
        /// </summary>
        public EventHandler<IDevicesResponseDocument> OnProbeReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectSreams Document is received from a Current Request
        /// </summary>
        public EventHandler<IStreamsResponseDocument> OnCurrentReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectSreams Document is received from the Samples Stream
        /// </summary>
        public EventHandler<IStreamsResponseDocument> OnSampleReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectAssets Document is received
        /// </summary>
        public EventHandler<IAssetsResponseDocument> OnAssetsReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectError Document is received
        /// </summary>
        public EventHandler<IErrorResponseDocument> OnMTConnectError { get; set; }

        /// <summary>
        /// Raised when an Connection Error occurs
        /// </summary>
        public EventHandler<Exception> OnConnectionError { get; set; }

        /// <summary>
        /// Raised when an Internal Error occurs
        /// </summary>
        public EventHandler<Exception> OnInternalError { get; set; }

        /// <summary>
        /// Raised when any Response from the Client is received
        /// </summary>
        public EventHandler OnResponseReceived { get; set; }

        /// <summary>
        /// Raised when the Client is Starting
        /// </summary>
        public EventHandler OnClientStarting { get; set; }

        /// <summary>
        /// Raised when the Client is Started
        /// </summary>
        public EventHandler OnClientStarted { get; set; }

        /// <summary>
        /// Raised when the Client is Stopping
        /// </summary>
        public EventHandler OnClientStopping { get; set; }

        /// <summary>
        /// Raised when the Client is Stopeed
        /// </summary>
        public EventHandler OnClientStopped { get; set; }

        /// <summary>
        /// Raised when the Stream is Starting
        /// </summary>
        public EventHandler<string> OnStreamStarting { get; set; }

        /// <summary>
        /// Raised when the Stream is Started
        /// </summary>
        public EventHandler<string> OnStreamStarted { get; set; }

        /// <summary>
        /// Raised when the Stream is Stopped
        /// </summary>
        public EventHandler<string> OnStreamStopping { get; set; }

        /// <summary>
        /// Raised when the Stream is Stopped
        /// </summary>
        public EventHandler<string> OnStreamStopped { get; set; }

        #endregion


        /// <summary>
        /// Starts the MTConnectClient
        /// </summary>
        public void Start(string path = null)
        {
            _stop = new CancellationTokenSource();

            OnClientStarting?.Invoke(this, new EventArgs());

            _streamPath = path;

            _ = Task.Run(Worker, _stop.Token);
        }

        /// <summary>
        /// Starts the MTConnectClient
        /// </summary>
        public void Start(CancellationToken cancellationToken, string path = null)
        {
            _stop = new CancellationTokenSource();
            cancellationToken.Register(() => { Stop(); });

            OnClientStarting?.Invoke(this, new EventArgs());

            _streamPath = path;

            _ = Task.Run(Worker, _stop.Token);
        }

        /// <summary>
        /// Starts the MTConnectClient from the specified Sequence
        /// </summary>
        public void StartFromSequence(long instanceId, long sequence, string path = null)
        {
            _stop = new CancellationTokenSource();

            OnClientStarting?.Invoke(this, new EventArgs());

            _lastInstanceId = instanceId;
            _lastSequence = sequence;
            _streamPath = path;

            _ = Task.Run(Worker, _stop.Token);
        }

        /// <summary>
        /// Starts the MTConnectClient from the specified Sequence
        /// </summary>
        public void StartFromSequence(long instanceId, long sequence, CancellationToken cancellationToken, string path = null)
        {
            _stop = new CancellationTokenSource();
            cancellationToken.Register(() => { Stop(); });

            OnClientStarting?.Invoke(this, new EventArgs());

            _lastInstanceId = instanceId;
            _lastSequence = sequence;
            _streamPath = path;

            _ = Task.Run(Worker, _stop.Token);
        }

        /// <summary>
        /// Starts the MTConnectClient from the beginning of the MTConnect Agent's Buffer
        /// </summary>
        public void StartFromBuffer(string path = null)
        {
            _stop = new CancellationTokenSource();

            OnClientStarting?.Invoke(this, new EventArgs());

            _initializeFromBuffer = true;
            _streamPath = path;

            _ = Task.Run(Worker, _stop.Token);
        }

        /// <summary>
        /// Starts the MTConnectClient from the beginning of the MTConnect Agent's Buffer
        /// </summary>
        public void StartFromBuffer(CancellationToken cancellationToken, string path = null)
        {
            _stop = new CancellationTokenSource();
            cancellationToken.Register(() => { Stop(); });

            OnClientStarting?.Invoke(this, new EventArgs());

            _initializeFromBuffer = true;
            _streamPath = path;

            _ = Task.Run(Worker, _stop.Token);
        }

        /// <summary>
        /// Stops the MTConnectClient
        /// </summary>
        public void Stop()
        {
            OnClientStopping?.Invoke(this, new EventArgs());

            if (_stop != null) _stop.Cancel();
        }

        /// <summary>
        /// Resets the MTConnect Client to read from the beginning of the Agent Buffer
        /// </summary>
        public void Reset()
        {
            _lastInstanceId = 0;
            _lastSequence = 0;
            _lastResponse = 0;
        }


        #region "Requests"

        /// <summary>
        /// Execute a Probe Request to return an MTConnectDevices Response Document
        /// </summary>
        public IDevicesResponseDocument GetProbe()
        {
            var client = new MTConnectProbeClient(Authority, Device, DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.OnMTConnectError += (s, doc) => OnMTConnectError?.Invoke(this, doc);
            client.OnConnectionError += (s, ex) => OnConnectionError?.Invoke(this, ex);
            client.OnInternalError += (s, ex) => OnInternalError?.Invoke(this, ex);
            return client.Get();
        }

        /// <summary>
        /// Execute a Probe Request to return an MTConnectDevices Response Document
        /// </summary>
        public async Task<IDevicesResponseDocument> GetProbeAsync()
        {
            return await GetProbeAsync(CancellationToken.None);
        }

        /// <summary>
        /// Execute a Probe Request to return an MTConnectDevices Response Document
        /// </summary>
        public async Task<IDevicesResponseDocument> GetProbeAsync(CancellationToken cancellationToken)
        {
            var client = new MTConnectProbeClient(Authority, Device, DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.OnMTConnectError += (s, doc) => OnMTConnectError?.Invoke(this, doc);
            client.OnConnectionError += (s, ex) => OnConnectionError?.Invoke(this, ex);
            client.OnInternalError += (s, ex) => OnInternalError?.Invoke(this, ex);
            return await client.GetAsync(cancellationToken);
        }


        /// <summary>
        /// Execute a Current Request to return an MTConnectStreams Response Document
        /// </summary>
        public IStreamsResponseDocument GetCurrent(long at = 0, string path = null)
        {
            var client = new MTConnectCurrentClient(Authority, Device, at: at, path: path, documentFormat: DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.OnMTConnectError += (s, doc) => OnMTConnectError?.Invoke(this, doc);
            client.OnConnectionError += (s, ex) => OnConnectionError?.Invoke(this, ex);
            client.OnInternalError += (s, ex) => OnInternalError?.Invoke(this, ex);
            return client.Get();
        }

        /// <summary>
        /// Execute a Current Request to return an MTConnectStreams Response Document
        /// </summary>
        public async Task<IStreamsResponseDocument> GetCurrentAsync(long at = 0, string path = null)
        {
            return await GetCurrentAsync(CancellationToken.None, at, path);
        }

        /// <summary>
        /// Execute a Current Request to return an MTConnectStreams Response Document
        /// </summary>
        public async Task<IStreamsResponseDocument> GetCurrentAsync(CancellationToken cancellationToken, long at = 0, string path = null)
        {
            var client = new MTConnectCurrentClient(Authority, Device, at: at, path: path, documentFormat: DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.OnMTConnectError += (s, doc) => OnMTConnectError?.Invoke(this, doc);
            client.OnConnectionError += (s, ex) => OnConnectionError?.Invoke(this, ex);
            client.OnInternalError += (s, ex) => OnInternalError?.Invoke(this, ex);
            return await client.GetAsync(cancellationToken);
        }


        /// <summary>
        /// Execute a Sample Request to return an MTConnectStreams Response Document
        /// </summary>
        public IStreamsResponseDocument GetSample(long from = 0, long to = 0, int count = 0, string path = null)
        {
            var client = new MTConnectSampleClient(Authority, Device, from: from, to: to, count: count, path: path, documentFormat: DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.OnMTConnectError += (s, doc) => OnMTConnectError?.Invoke(this, doc);
            client.OnConnectionError += (s, ex) => OnConnectionError?.Invoke(this, ex);
            client.OnInternalError += (s, ex) => OnInternalError?.Invoke(this, ex);
            return client.Get();
        }

        /// <summary>
        /// Execute a Sample Request to return an MTConnectStreams Response Document
        /// </summary>
        public async Task<IStreamsResponseDocument> GetSampleAsync(long from = 0, long to = 0, int count = 0, string path = null)
        {
            return await GetSampleAsync(CancellationToken.None, from, to, count, path);
        }

        /// <summary>
        /// Execute a Sample Request to return an MTConnectStreams Response Document
        /// </summary>
        public async Task<IStreamsResponseDocument> GetSampleAsync(CancellationToken cancellationToken, long from = 0, long to = 0, int count = 0, string path = null)
        {
            var client = new MTConnectSampleClient(Authority, Device, from: from, to: to, count: count, path: path, documentFormat: DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.OnMTConnectError += (s, doc) => OnMTConnectError?.Invoke(this, doc);
            client.OnConnectionError += (s, ex) => OnConnectionError?.Invoke(this, ex);
            client.OnInternalError += (s, ex) => OnInternalError?.Invoke(this, ex);
            return await client.GetAsync(cancellationToken);
        }


        /// <summary>
        /// Execute a Assets Request to return an MTConnectAssets Response Document
        /// </summary>
        public IAssetsResponseDocument GetAssets(long count = 100)
        {
            var client = new MTConnectAssetClient(Authority, count, null, null, DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.OnMTConnectError += (s, doc) => OnMTConnectError?.Invoke(this, doc);
            client.OnConnectionError += (s, ex) => OnConnectionError?.Invoke(this, ex);
            client.OnInternalError += (s, ex) => OnInternalError?.Invoke(this, ex);
            return client.Get();
        }

        /// <summary>
        /// Execute a Assets Request to return an MTConnectAssets Response Document
        /// </summary>
        public async Task<IAssetsResponseDocument> GetAssetsAsync(long count = 100)
        {
            return await GetAssetsAsync(CancellationToken.None, count);
        }

        /// <summary>
        /// Execute a Assets Request to return an MTConnectAssets Response Document
        /// </summary>
        public async Task<IAssetsResponseDocument> GetAssetsAsync(CancellationToken cancellationToken, long count = 100)
        {
            var client = new MTConnectAssetClient(Authority, count, null, null, DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.OnMTConnectError += (s, doc) => OnMTConnectError?.Invoke(this, doc);
            client.OnConnectionError += (s, ex) => OnConnectionError?.Invoke(this, ex);
            client.OnInternalError += (s, ex) => OnInternalError?.Invoke(this, ex);
            return await client.GetAsync(cancellationToken);
        }


        /// <summary>
        /// Execute a Assets Request to return an MTConnectAssets Response Document for the specified AssetId
        /// </summary>
        public IAssetsResponseDocument GetAsset(string assetId)
        {
            var client = new MTConnectAssetClient(Authority, assetId, DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.OnMTConnectError += (s, doc) => OnMTConnectError?.Invoke(this, doc);
            client.OnConnectionError += (s, ex) => OnConnectionError?.Invoke(this, ex);
            client.OnInternalError += (s, ex) => OnInternalError?.Invoke(this, ex);
            return client.Get();
        }

        /// <summary>
        /// Execute a Assets Request to return an MTConnectAssets Response Document for the specified AssetId
        /// </summary>
        public async Task<IAssetsResponseDocument> GetAssetAsync(string assetId)
        {
            return await GetAssetAsync(assetId, CancellationToken.None);
        }

        /// <summary>
        /// Execute a Assets Request to return an MTConnectAssets Response Document for the specified AssetId
        /// </summary>
        public async Task<IAssetsResponseDocument> GetAssetAsync(string assetId, CancellationToken cancellationToken)
        {
            var client = new MTConnectAssetClient(Authority, assetId, DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.OnMTConnectError += (s, doc) => OnMTConnectError?.Invoke(this, doc);
            client.OnConnectionError += (s, ex) => OnConnectionError?.Invoke(this, ex);
            client.OnInternalError += (s, ex) => OnInternalError?.Invoke(this, ex);
            return await client.GetAsync(cancellationToken);
        }

        #endregion

        #region "Streams"

        private async Task Worker()
        {
            var initialRequest = true;
            _lastInstanceId = 0;

            if (OnClientStarted != null) OnClientStarted?.Invoke(this, new EventArgs());

            do
            {
                try
                {
                    // Run Probe Request
                    var probe = await GetProbeAsync(_stop.Token);
                    if (probe != null)
                    {
                        _lastResponse = UnixDateTime.Now;
                        OnResponseReceived?.Invoke(this, new EventArgs());

                        // Raise ProbeReceived Event
                        OnProbeReceived?.Invoke(this, probe);

                        // Get All Assets
                        if (probe.Header.AssetCount > 0)
                        {
                            var assets = await GetAssetsAsync(_stop.Token, probe.Header.AssetCount);
                            if (assets != null)
                            {
                                _lastResponse = UnixDateTime.Now;
                                OnResponseReceived?.Invoke(this, new EventArgs());

                                OnAssetsReceived?.Invoke(this, assets);
                            }
                        }

                        // Add to cached list
                        if (!probe.Devices.IsNullOrEmpty())
                        {
                            foreach (var device in probe.Devices)
                            {
                                lock (_lock)
                                {
                                    _devices.Remove(device.Uuid);
                                    _devices.Add(device.Uuid, device);
                                }
                            }
                        }

                        // Run Current Request
                        var current = await GetCurrentAsync(_stop.Token, path: _streamPath);
                        if (current != null)
                        {
                            _lastResponse = UnixDateTime.Now;
                            OnResponseReceived?.Invoke(this, new EventArgs());

                            // Raise CurrentReceived Event
                            OnCurrentReceived?.Invoke(this, current);

                            // Check Assets
                            if (!current.Streams.IsNullOrEmpty())
                            {
                                var deviceStream = current.Streams.FirstOrDefault(o => o.Uuid == Device || o.Name == Device);
                                if (deviceStream != null && deviceStream.Observations != null) CheckAssetChanged(deviceStream.Observations, _stop.Token);
                            }

                            // Read the Next Available Sequence from the MTConnect Agent
                            if (initialRequest && !_initializeFromBuffer && _lastSequence == 0)
                            {
                                _lastSequence = current.Header.NextSequence;
                            }

                            // Check if Agent Instance ID has changed (Agent has been reset)
                            if (_lastInstanceId != 0 && _lastInstanceId != current.Header.InstanceId)
                            {
                                // Reset 'From' query parameter
                                _lastSequence = 0;
                            }

                            // Read & Save the Instance ID of the MTConnect Agent
                            _lastInstanceId = current.Header.InstanceId;

                            // If the LastSequence is not within the current sequence range then reset Sequeunce to 0
                            if (current.Header.FirstSequence > _lastSequence ||
                                current.Header.NextSequence < _lastSequence)
                            {
                                _lastSequence = 0;
                            }

                            // Create the Url to use for the Sample Stream
                            string url = CreateSampleUrl(Authority, Device, _lastSequence, Interval, Heartbeat, MaximumSampleCount, _streamPath);
                            if (CurrentOnly) url = CreateCurrentUrl(Authority, Device, Interval, _streamPath);

                            // Create and Start the Stream
                            _stream = new MTConnectHttpClientStream(url, DocumentFormat);
                            _stream.Timeout = Heartbeat * 3;
                            _stream.ContentEncodings = ContentEncodings;
                            _stream.ContentType = ContentType;
                            _stream.Starting += (s, o) => OnStreamStarting?.Invoke(this, url);
                            _stream.Started += (s, o) => OnStreamStarted?.Invoke(this, url);
                            _stream.Stopping += (s, o) => OnStreamStopping?.Invoke(this, url);
                            _stream.Stopped += (s, o) => OnStreamStopped?.Invoke(this, url);
                            _stream.DocumentReceived += (s, doc) => ProcessSampleDocument(doc, _stop.Token);
                            _stream.ErrorReceived += (s, doc) => ProcessSampleError(doc);
                            _stream.OnConnectionError += (s, ex) => OnConnectionError?.Invoke(this, ex);
                            _stream.OnInternalError += (s, ex) => OnInternalError?.Invoke(this, ex);

                            // Run Stream (Blocking call)
                            await _stream.Run(_stop.Token);

                            initialRequest = false;

                            if (!_stop.Token.IsCancellationRequested)
                            {
                                await Task.Delay(RetryInterval, _stop.Token);
                            }
                        }
                        else
                        {
                            await Task.Delay(RetryInterval, _stop.Token);
                        }
                    }
                    else
                    {
                        await Task.Delay(RetryInterval, _stop.Token);
                    }
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    OnInternalError?.Invoke(this, ex);
                }

            } while (!_stop.Token.IsCancellationRequested);

            OnClientStopped?.Invoke(this, new EventArgs());
        }

        private void ProcessSampleDocument(IStreamsResponseDocument document, CancellationToken cancel)
        {
            _lastResponse = UnixDateTime.Now;
            OnResponseReceived?.Invoke(this, new EventArgs());

            if (document != null)
            {
                // Set Agent Instance ID
                if (document.Header != null) _lastInstanceId = document.Header.InstanceId;

                if (!document.Streams.IsNullOrEmpty())
                {
                    IDeviceStream deviceStream = null;

                    // Get the DeviceStream for the Device or default to the first
                    if (!string.IsNullOrEmpty(Device)) deviceStream = document.Streams.FirstOrDefault(o => o.Uuid == Device || o.Name == Device);
                    else deviceStream = document.Streams.FirstOrDefault();

                    if (deviceStream != null && deviceStream.Observations != null && deviceStream.Observations.Count() > 0)
                    {
                        // Recreate Response Document (to set DataItem property for Observations)
                        var response = new StreamsResponseDocument();
                        response.Header = document.Header;

                        var deviceStreams = new List<IDeviceStream>();
                        foreach (var stream in document.Streams)
                        {
                            deviceStreams.Add(ProcessDeviceStream(stream));
                        }
                        response.Streams = deviceStreams;

                        CheckAssetChanged(deviceStream.Observations, cancel);

                        // Save the most recent Sequence that was read
                        _lastSequence = deviceStream.Observations.Max(o => o.Sequence);

                        OnSampleReceived?.Invoke(this, response);
                    }
                }
            }
        }

        private IDeviceStream ProcessDeviceStream(IDeviceStream inputDeviceStream)
        {
            var outputDeviceStream = new DeviceStream();
            outputDeviceStream.Name = inputDeviceStream.Name;
            outputDeviceStream.Uuid = inputDeviceStream.Uuid;

            var componentStreams = new List<IComponentStream>();
            if (!inputDeviceStream.ComponentStreams.IsNullOrEmpty())
            {
                foreach (var componentStream in inputDeviceStream.ComponentStreams)
                {
                    componentStreams.Add(ProcessComponentStream(outputDeviceStream.Uuid, componentStream));
                }
            }
            outputDeviceStream.ComponentStreams = componentStreams;

            return outputDeviceStream;
        }

        private IComponentStream ProcessComponentStream(string deviceUuid, IComponentStream inputComponentStream)
        {
            var outputComponentStream = new ComponentStream();
            outputComponentStream.Name = inputComponentStream.Name;
            outputComponentStream.NativeName = inputComponentStream.NativeName;
            outputComponentStream.Uuid = inputComponentStream.Uuid;
            outputComponentStream.Component = GetCachedComponent(deviceUuid, inputComponentStream.ComponentId);

            var observations = new List<IObservation>();
            if (!inputComponentStream.Observations.IsNullOrEmpty())
            {
                foreach (var inputObservation in inputComponentStream.Observations)
                {
                    var outputObservation = new Observation();
                    outputObservation.DeviceUuid = inputObservation.DeviceUuid;
                    outputObservation.DataItemId = inputObservation.DataItemId;
                    outputObservation.DataItem = GetCachedDataItem(deviceUuid, inputObservation.DataItemId);
                    outputObservation.CompositionId = inputObservation.CompositionId;
                    outputObservation.Category = inputObservation.Category;
                    outputObservation.Representation = inputObservation.Representation;
                    outputObservation.Type = inputObservation.Type;
                    outputObservation.SubType = inputObservation.SubType;
                    outputObservation.Name = inputObservation.Name;
                    outputObservation.Sequence = inputObservation.Sequence;
                    outputObservation.Timestamp = inputObservation.Timestamp;
                    outputObservation.AddValues(inputObservation.Values);
                    observations.Add(outputObservation);
                }
            }
            outputComponentStream.Observations = observations;

            return outputComponentStream;
        }


        private void ProcessSampleError(IErrorResponseDocument document)
        {
            _lastResponse = UnixDateTime.Now;
            OnResponseReceived?.Invoke(this, new EventArgs());

            if (document != null)
            {
                OnMTConnectError?.Invoke(this, document);
            }
        }

        private async void CheckAssetChanged(IEnumerable<IObservation> observations, CancellationToken cancel)
        {
            if (observations != null && observations.Count() > 0)
            {
                var assetsChanged = observations.Where(o => o.Type == Devices.DataItems.Events.AssetChangedDataItem.TypeId.ToPascalCase());
                if (assetsChanged != null)
                {
                    foreach (var assetChanged in assetsChanged)
                    {
                        string assetId = assetChanged.GetValue(ValueKeys.Result);
                        if (assetId != Observation.Unavailable)
                        {
                            var doc = await GetAssetAsync(assetId, cancel);
                            if (doc != null)
                            {
                                OnAssetsReceived?.Invoke(this, doc);
                            }
                        }
                    }
                }
            }
        }


        private static string CreateCurrentUrl(string baseUrl, string deviceKey, int interval, string path = null)
        {
            var url = baseUrl;
            if (!string.IsNullOrEmpty(deviceKey)) url = Url.Combine(url, deviceKey + "/current");
            else url = Url.Combine(url, "current");

            // Replace 'localhost' with '127.0.0.1' (This is due to a performance issue with .NET Core's System.Net.Http.HttpClient)
            if (url.Contains("localhost")) url = url.Replace("localhost", "127.0.0.1");

            // Check for http
            if (!url.StartsWith("http://") && !url.StartsWith("https://")) url = "http://" + url;

            if (interval > -1) url = Url.AddQueryParameter(url, "interval", interval);
            if (!string.IsNullOrEmpty(path)) url = Url.AddQueryParameter(url, "path", path);

            return url;
        }

        private static string CreateSampleUrl(string baseUrl, string deviceKey, long from, int interval, int heartbeat, long count, string path = null)
        {
            var url = baseUrl;
            if (!string.IsNullOrEmpty(deviceKey)) url = Url.Combine(url, deviceKey + "/sample");
            else url = Url.Combine(url, "sample");

            // Replace 'localhost' with '127.0.0.1' (This is due to a performance issue with .NET Core's System.Net.Http.HttpClient)
            if (url.Contains("localhost")) url = url.Replace("localhost", "127.0.0.1");

            // Check for http
            if (!url.StartsWith("http://") && !url.StartsWith("https://")) url = "http://" + url;

            if (from > 0) url = Url.AddQueryParameter(url, "from", from);
            if (count > 0) url = Url.AddQueryParameter(url, "count", count);
            if (interval > -1) url = Url.AddQueryParameter(url, "interval", interval);
            if (heartbeat > 0) url = Url.AddQueryParameter(url, "heartbeat", heartbeat);
            if (!string.IsNullOrEmpty(path)) url = Url.AddQueryParameter(url, "path", path);

            return url;
        }

        #endregion

        #region "Cache"

        private IDevice GetCachedDevice(string deviceUuid)
        {
            if (!string.IsNullOrEmpty(deviceUuid))
            {
                lock (_lock) return _devices.GetValueOrDefault(deviceUuid);
            }

            return null;
        }

        private IComponent GetCachedComponent(string deviceUuid, string componentId)
        {
            if (!string.IsNullOrEmpty(deviceUuid) && !string.IsNullOrEmpty(componentId))
            {
                lock (_lock)
                {
                    var device = _devices.GetValueOrDefault(deviceUuid);
                    if (device != null && !device.Components.IsNullOrEmpty())
                    {
                        return device.Components.FirstOrDefault(o => o.Id == componentId);
                    }
                }
            }

            return null;
        }

        private IDataItem GetCachedDataItem(string deviceUuid, string dataItemId)
        {
            if (!string.IsNullOrEmpty(deviceUuid) && !string.IsNullOrEmpty(dataItemId))
            {
                lock (_lock)
                {
                    var device = _devices.GetValueOrDefault(deviceUuid);
                    if (device != null)
                    {
                        var dataItems = device.GetDataItems();
                        if (!dataItems.IsNullOrEmpty())
                        {
                            return dataItems.FirstOrDefault(o => o.Id == dataItemId);
                        }
                    }
                }
            }

            return null;
        }

        #endregion

    }
}
