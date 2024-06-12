// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Formatters;
using MTConnect.Headers;
using MTConnect.Http;
using MTConnect.Observations;
using MTConnect.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients
{
    /// <summary>
    /// Client that implements the full MTConnect HTTP REST Api Protocol (Probe, Current, Sample Stream, and Assets)
    /// </summary>
    public class MTConnectHttpClient : IMTConnectClient, IMTConnectEntityClient
    {
        private readonly Dictionary<string, IDevice> _devices = new Dictionary<string, IDevice>();
        private readonly Dictionary<string, IComponent> _cachedComponents = new Dictionary<string, IComponent>();
        private readonly Dictionary<string, IDataItem> _cachedDataItems = new Dictionary<string, IDataItem>();
        private readonly object _lock = new object();

        private CancellationTokenSource _stop;
        private MTConnectHttpClientStream _stream;
        private ulong _lastInstanceId;
        private ulong _lastSequence;
        private long _lastResponse;
        private bool _initializeFromBuffer;
        private string _streamPath;


        /// <summary>
        /// Initializes a new instance of the MTConnectClient class that is used to perform
        /// the full request and stream protocol from an MTConnect Agent using the MTConnect HTTP REST Api protocol
        /// </summary>
        /// <param name="authority">
        /// The authority portion consists of the DNS name or IP address associated with an Agent and an optional
        /// TCP port number[:port] that the Agent is listening to for incoming Requests from client software applications.
        /// If the port number is the default Port 80, port is not required.
        /// </param>
        /// <param name="device">
        /// If present, specifies that only the Equipment Metadata for the piece of equipment represented by the name or uuid will be published.
        /// If not present, Metadata for all pieces of equipment associated with the Agent will be published.
        /// </param>
        /// <param name="documentFormat">Gets or Sets the Document Format to return</param>
        public MTConnectHttpClient(string authority, string device = null, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            Id = Guid.NewGuid().ToString();
            Init();
            Authority = authority;
            Device = device;
            DocumentFormat = documentFormat;
        }

        /// <summary>
        /// Initializes a new instance of the MTConnectClient class that is used to perform
        /// the full request and stream protocol from an MTConnect Agent using the MTConnect HTTP REST Api protocol
        /// </summary>
        /// <param name="hostname">
        /// The Hostname of the MTConnect Agent
        /// </param>
        /// <param name="port">
        /// The Port of the MTConnect Agent
        /// </param>
        /// <param name="device">
        /// If present, specifies that only the Equipment Metadata for the piece of equipment represented by the name or uuid will be published.
        /// If not present, Metadata for all pieces of equipment associated with the Agent will be published.
        /// </param>
        /// <param name="documentFormat">Gets or Sets the Document Format to return</param>
        public MTConnectHttpClient(string hostname, int port, string device = null, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            Id = Guid.NewGuid().ToString();
            Init();
            Authority = CreateUrl(hostname, port);
            Device = device;
            DocumentFormat = documentFormat;
        }

        private void Init()
        {
            Interval = 500;
            Timeout = 5000;
            Heartbeat = 1000;
            MaximumSampleCount = 1000;
            ReconnectionInterval = 10000;
            ContentEncodings = HttpContentEncodings.DefaultAccept;
            ContentType = MimeTypes.Get(DocumentFormat);
            UseStreaming = true;
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
        /// Gets or Sets the Document Format to use (ex. XML, JSON, etc.)
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
        public int ReconnectionInterval { get; set; }

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
        public ulong LastInstanceId => _lastInstanceId;

        /// <summary>
        /// Gets the Last Sequence read from the MTConnect Agent
        /// </summary>
        public ulong LastSequence => _lastSequence;

        /// <summary>
        /// Gets the Unix Timestamp (in Milliseconds) since the last response from the MTConnect Agent
        /// </summary>
        public long LastResponse => _lastResponse;

        /// <summary>
        /// Gets or Sets whether the stream requests a Current (true) or a Sample (false)
        /// </summary>
        public bool CurrentOnly { get; set; }

        /// <summary>
        /// Gets or Sets whether the client should use Streaming (true) or Polling (false)
        /// </summary>
        public bool UseStreaming { get; set; }


        #region "Events"

        /// <summary>
        /// Raised when a Device is received
        /// </summary>
        public event EventHandler<IDevice> DeviceReceived;

        /// <summary>
        /// Raised when an Observation is received
        /// </summary>
        public event EventHandler<IObservation> ObservationReceived;

        /// <summary>
        /// Raised when an Asset is received
        /// </summary>
        public event EventHandler<IAsset> AssetReceived;

        /// <summary>
        /// Raised when an MTConnectDevices Document is received
        /// </summary>
        public event EventHandler<IDevicesResponseDocument> ProbeReceived;

        /// <summary>
        /// Raised when an MTConnectSreams Document is received from a Current Request
        /// </summary>
        public event EventHandler<IStreamsResponseDocument> CurrentReceived;

        /// <summary>
        /// Raised when an MTConnectSreams Document is received from the Samples Stream
        /// </summary>
        public event EventHandler<IStreamsResponseDocument> SampleReceived;

        /// <summary>
        /// Raised when an MTConnectAssets Document is received
        /// </summary>
        public event EventHandler<IAssetsResponseDocument> AssetsReceived;

        /// <summary>
        /// Raised when an MTConnectError Document is received
        /// </summary>
        public event EventHandler<IErrorResponseDocument> MTConnectError;

        /// <summary>
        /// Raised when a Document Formatting Error is received
        /// </summary>
        public event EventHandler<IFormatReadResult> FormatError;

        /// <summary>
        /// Raised when an Connection Error occurs
        /// </summary>
        public event EventHandler<Exception> ConnectionError;

        /// <summary>
        /// Raised when an Internal Error occurs
        /// </summary>
        public event EventHandler<Exception> InternalError;

        /// <summary>
        /// Raised when any Response from the Client is received
        /// </summary>
        public event EventHandler ResponseReceived;

        /// <summary>
        /// Raised when the Client is Starting
        /// </summary>
        public event EventHandler ClientStarting;

        /// <summary>
        /// Raised when the Client is Started
        /// </summary>
        public event EventHandler ClientStarted;

        /// <summary>
        /// Raised when the Client is Stopping
        /// </summary>
        public event EventHandler ClientStopping;

        /// <summary>
        /// Raised when the Client is Stopeed
        /// </summary>
        public event EventHandler ClientStopped;

        /// <summary>
        /// Raised when the Stream is Starting
        /// </summary>
        public event EventHandler<string> StreamStarting;

        /// <summary>
        /// Raised when the Stream is Started
        /// </summary>
        public event EventHandler<string> StreamStarted;

        /// <summary>
        /// Raised when the Stream is Stopped
        /// </summary>
        public event EventHandler<string> StreamStopping;

        /// <summary>
        /// Raised when the Stream is Stopped
        /// </summary>
        public event EventHandler<string> StreamStopped;

        #endregion


        /// <summary>
        /// Starts the MTConnectClient
        /// </summary>
        public void Start()
        {
            _stop = new CancellationTokenSource();

            ClientStarting?.Invoke(this, new EventArgs());

            _initializeFromBuffer = false;
            _lastInstanceId = 0;
            _lastSequence = 0;
            _streamPath = null;

            _ = Task.Run(Worker, _stop.Token);
        }

        /// <summary>
        /// Starts the MTConnectClient
        /// </summary>
        public void Start(string path)
        {
            _stop = new CancellationTokenSource();

            ClientStarting?.Invoke(this, new EventArgs());

            _initializeFromBuffer = false;
            _lastInstanceId = 0;
            _lastSequence = 0;
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

            ClientStarting?.Invoke(this, new EventArgs());

            _initializeFromBuffer = false;
            _lastInstanceId = 0;
            _lastSequence = 0;
            _streamPath = path;

            _ = Task.Run(Worker, _stop.Token);
        }

        /// <summary>
        /// Starts the MTConnectClient from the specified Sequence
        /// </summary>
        public void StartFromSequence(ulong instanceId, ulong sequence, string path = null)
        {
            _stop = new CancellationTokenSource();

            ClientStarting?.Invoke(this, new EventArgs());

            _initializeFromBuffer = true;
            _lastInstanceId = instanceId;
            _lastSequence = sequence;
            _streamPath = path;

            _ = Task.Run(Worker, _stop.Token);
        }

        /// <summary>
        /// Starts the MTConnectClient from the specified Sequence
        /// </summary>
        public void StartFromSequence(ulong instanceId, ulong sequence, CancellationToken cancellationToken, string path = null)
        {
            _stop = new CancellationTokenSource();
            cancellationToken.Register(() => { Stop(); });

            ClientStarting?.Invoke(this, new EventArgs());

            _initializeFromBuffer = true;
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

            ClientStarting?.Invoke(this, new EventArgs());

            _initializeFromBuffer = true;
            _lastInstanceId = 0;
            _lastSequence = 0;
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

            ClientStarting?.Invoke(this, new EventArgs());

            _initializeFromBuffer = true;
            _lastInstanceId = 0;
            _lastSequence = 0;
            _streamPath = path;

            _ = Task.Run(Worker, _stop.Token);
        }

        /// <summary>
        /// Stops the MTConnectClient
        /// </summary>
        public void Stop()
        {
            ClientStopping?.Invoke(this, new EventArgs());

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
            var client = new MTConnectHttpProbeClient(Authority, Device, DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.MTConnectError += (s, doc) => MTConnectError?.Invoke(this, doc);
            client.FormatError += (s, r) => FormatError?.Invoke(this, r);
            client.ConnectionError += (s, ex) => ConnectionError?.Invoke(this, ex);
            client.InternalError += (s, ex) => InternalError?.Invoke(this, ex);
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
            var client = new MTConnectHttpProbeClient(Authority, Device, DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.MTConnectError += (s, doc) => MTConnectError?.Invoke(this, doc);
            client.FormatError += (s, r) => FormatError?.Invoke(this, r);
            client.ConnectionError += (s, ex) => ConnectionError?.Invoke(this, ex);
            client.InternalError += (s, ex) => InternalError?.Invoke(this, ex);
            return await client.GetAsync(cancellationToken);
        }


        /// <summary>
        /// Execute a Current Request to return an MTConnectStreams Response Document
        /// </summary>
        public IStreamsResponseDocument GetCurrent(long at = 0, string path = null)
        {
            var client = new MTConnectHttpCurrentClient(Authority, Device, at: at, path: path, documentFormat: DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.MTConnectError += (s, doc) => MTConnectError?.Invoke(this, doc);
            client.FormatError += (s, r) => FormatError?.Invoke(this, r);
            client.ConnectionError += (s, ex) => ConnectionError?.Invoke(this, ex);
            client.InternalError += (s, ex) => InternalError?.Invoke(this, ex);
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
            var client = new MTConnectHttpCurrentClient(Authority, Device, at: at, path: path, documentFormat: DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.MTConnectError += (s, doc) => MTConnectError?.Invoke(this, doc);
            client.FormatError += (s, r) => FormatError?.Invoke(this, r);
            client.ConnectionError += (s, ex) => ConnectionError?.Invoke(this, ex);
            client.InternalError += (s, ex) => InternalError?.Invoke(this, ex);
            return await client.GetAsync(cancellationToken);
        }


        /// <summary>
        /// Execute a Sample Request to return an MTConnectStreams Response Document
        /// </summary>
        public IStreamsResponseDocument GetSample(long from = 0, long to = 0, int count = 0, string path = null)
        {
            var client = new MTConnectHttpSampleClient(Authority, Device, from: from, to: to, count: count, path: path, documentFormat: DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.MTConnectError += (s, doc) => MTConnectError?.Invoke(this, doc);
            client.FormatError += (s, r) => FormatError?.Invoke(this, r);
            client.ConnectionError += (s, ex) => ConnectionError?.Invoke(this, ex);
            client.InternalError += (s, ex) => InternalError?.Invoke(this, ex);
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
            var client = new MTConnectHttpSampleClient(Authority, Device, from: from, to: to, count: count, path: path, documentFormat: DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.MTConnectError += (s, doc) => MTConnectError?.Invoke(this, doc);
            client.FormatError += (s, r) => FormatError?.Invoke(this, r);
            client.ConnectionError += (s, ex) => ConnectionError?.Invoke(this, ex);
            client.InternalError += (s, ex) => InternalError?.Invoke(this, ex);
            return await client.GetAsync(cancellationToken);
        }


        /// <summary>
        /// Execute a Assets Request to return an MTConnectAssets Response Document
        /// </summary>
        public IAssetsResponseDocument GetAssets(long count = 100)
        {
            var client = new MTConnectHttpAssetClient(Authority, count, null, null, DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.MTConnectError += (s, doc) => MTConnectError?.Invoke(this, doc);
            client.FormatError += (s, r) => FormatError?.Invoke(this, r);
            client.ConnectionError += (s, ex) => ConnectionError?.Invoke(this, ex);
            client.InternalError += (s, ex) => InternalError?.Invoke(this, ex);
            return client.Get();
        }

        /// <summary>
        /// Execute a Assets Request to return an MTConnectAssets Response Document
        /// </summary>
        public async Task<IAssetsResponseDocument> GetAssetsAsync(ulong count = 100)
        {
            return await GetAssetsAsync(CancellationToken.None, count);
        }

        /// <summary>
        /// Execute a Assets Request to return an MTConnectAssets Response Document
        /// </summary>
        public async Task<IAssetsResponseDocument> GetAssetsAsync(CancellationToken cancellationToken, ulong count = 100)
        {
            var client = new MTConnectHttpAssetClient(Authority, (long)count, null, null, DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.MTConnectError += (s, doc) => MTConnectError?.Invoke(this, doc);
            client.FormatError += (s, r) => FormatError?.Invoke(this, r);
            client.ConnectionError += (s, ex) => ConnectionError?.Invoke(this, ex);
            client.InternalError += (s, ex) => InternalError?.Invoke(this, ex);
            return await client.GetAsync(cancellationToken);
        }


        /// <summary>
        /// Execute a Assets Request to return an MTConnectAssets Response Document for the specified AssetId
        /// </summary>
        public IAssetsResponseDocument GetAsset(string assetId)
        {
            var client = new MTConnectHttpAssetClient(Authority, assetId, DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.MTConnectError += (s, doc) => MTConnectError?.Invoke(this, doc);
            client.FormatError += (s, r) => FormatError?.Invoke(this, r);
            client.ConnectionError += (s, ex) => ConnectionError?.Invoke(this, ex);
            client.InternalError += (s, ex) => InternalError?.Invoke(this, ex);
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
            var client = new MTConnectHttpAssetClient(Authority, assetId, DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.MTConnectError += (s, doc) => MTConnectError?.Invoke(this, doc);
            client.FormatError += (s, r) => FormatError?.Invoke(this, r);
            client.ConnectionError += (s, ex) => ConnectionError?.Invoke(this, ex);
            client.InternalError += (s, ex) => InternalError?.Invoke(this, ex);
            return await client.GetAsync(cancellationToken);
        }

        #endregion

        #region "Streams"

        private async Task Worker()
        {
            var initialRequest = true;

            ClientStarted?.Invoke(this, new EventArgs());

            do
            {
                try
                {
                    // Run Probe Request
                    var probe = await GetProbeAsync(_stop.Token);
                    if (probe != null)
                    {
                        _lastResponse = UnixDateTime.Now;
                       ResponseReceived?.Invoke(this, new EventArgs());

                        ProcessProbeDocument(probe);

                        // Get All Assets
                        if (probe.Header.AssetCount > 0)
                        {
                            var assets = await GetAssetsAsync(_stop.Token, probe.Header.AssetCount);
                            if (assets != null)
                            {
                                _lastResponse = UnixDateTime.Now;
                                ResponseReceived?.Invoke(this, new EventArgs());

                                AssetsReceived?.Invoke(this, assets);
                            }
                        }

                        if (UseStreaming)
                        {
                            // Run Current Request
                            var current = await GetCurrentAsync(_stop.Token, path: _streamPath);
                            if (current != null)
                            {
                                _lastResponse = UnixDateTime.Now;
                                ResponseReceived?.Invoke(this, new EventArgs());

                                // Raise CurrentReceived Event
                                ProcessCurrentDocument(current, _stop.Token);

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
                                _stream.Starting += (s, o) => StreamStarting?.Invoke(this, url);
                                _stream.Started += (s, o) => StreamStarted?.Invoke(this, url);
                                _stream.Stopping += (s, o) => StreamStopping?.Invoke(this, url);
                                _stream.Stopped += (s, o) => StreamStopped?.Invoke(this, url);
                                _stream.DocumentReceived += (s, doc) => ProcessSampleDocument(doc, _stop.Token);
                                _stream.ErrorReceived += (s, doc) => ProcessSampleError(doc);
                                _stream.FormatError += (s, r) => FormatError?.Invoke(this, r);
                                _stream.ConnectionError += (s, ex) => ConnectionError?.Invoke(this, ex);
                                _stream.InternalError += (s, ex) => InternalError?.Invoke(this, ex);

                                // Run Stream (Blocking call)
                                await _stream.Run(_stop.Token);

                                initialRequest = false;

                                if (!_stop.Token.IsCancellationRequested)
                                {
                                    await Task.Delay(ReconnectionInterval, _stop.Token);
                                }
                            }
                            else
                            {
                                await Task.Delay(ReconnectionInterval, _stop.Token);
                            }
                        }
                        else
                        {
                            do
                            {
                                // Run Current Request
                                var current = await GetCurrentAsync(_stop.Token, path: _streamPath);
                                if (current != null)
                                {
                                    _lastResponse = UnixDateTime.Now;
                                    ResponseReceived?.Invoke(this, new EventArgs());

                                    // Raise CurrentReceived Event
                                    ProcessCurrentDocument(current, _stop.Token);

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

                                    initialRequest = false;

                                    // Wait for the specified pollingInterval between Current Requests
                                    var pollingInterval = Math.Max(1, Interval);
                                    await Task.Delay(pollingInterval);
                                }
                                else
                                {
                                    break; // Break out of the Current Polling loop and start at a new Probe Request
                                }
                            }
                            while (!_stop.Token.IsCancellationRequested);

                            await Task.Delay(ReconnectionInterval, _stop.Token);
                        }
                    }
                    else
                    {
                        await Task.Delay(ReconnectionInterval, _stop.Token);
                    }
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    InternalError?.Invoke(this, ex);
                }

            } while (!_stop.Token.IsCancellationRequested);

            ClientStopped?.Invoke(this, new EventArgs());
        }

        private void ProcessProbeDocument(IDevicesResponseDocument document)
        {
            if (document != null && !document.Devices.IsNullOrEmpty())
            {
                // Clear Cached DataItems and Components
                lock (_lock)
                {
                    _cachedComponents.Clear();
                    _cachedDataItems.Clear();
                }

                var outputDevices = new List<IDevice>();
                foreach (var device in document.Devices)
                {
                    var outputDevice = ProcessDevice(document.Header, device);

                    // Add to cached list
                    lock (_lock)
                    {
                        _devices.Remove(outputDevice.Uuid);
                        _devices.Add(outputDevice.Uuid, outputDevice);
                    }
                }

                foreach (var outputDevice in outputDevices)
                {
                    DeviceReceived?.Invoke(this, outputDevice);
                }

                // Raise ProbeReceived Event
                ProbeReceived?.Invoke(this, document);
            }
        }

        private void ProcessCurrentDocument(IStreamsResponseDocument document, CancellationToken cancel)
        {
            _lastResponse = UnixDateTime.Now;
            ResponseReceived?.Invoke(this, new EventArgs());

            if (document != null)
            {
                if (document.Header != null && !document.Streams.IsNullOrEmpty())
                {
                    // Recreate Response Document (to set DataItem property for Observations)
                    var response = new StreamsResponseDocument();
                    response.Header = document.Header;

                    var deviceStreams = new List<IDeviceStream>();
                    foreach (var stream in document.Streams)
                    {
                        deviceStreams.Add(ProcessDeviceStream(response.Header, stream));
                    }
                    response.Streams = deviceStreams;


                    CurrentReceived?.Invoke(this, response);


                    // Process Device Streams
                    foreach (var deviceStream in response.Streams)
                    {
                        // Check to see if any Assets have changed
                        CheckAssetChanged(deviceStream.Observations, cancel);

                        // Get Observations from Device Stream
                        var observations = response.GetObservations();
                        if (!observations.IsNullOrEmpty())
                        {
                            foreach (var observation in observations)
                            {
                                ObservationReceived?.Invoke(this, observation);
                            }
                        }
                    }
                }
            }
        }

        private void ProcessSampleDocument(IStreamsResponseDocument document, CancellationToken cancel)
        {
            _lastResponse = UnixDateTime.Now;
            ResponseReceived?.Invoke(this, new EventArgs());

            if (document != null)
            {
                if (document.Header != null && !document.Streams.IsNullOrEmpty())
                {
                    // Set Agent Instance ID
                    _lastInstanceId = document.Header.InstanceId;

                    // Recreate Response Document (to set DataItem property for Observations)
                    var response = new StreamsResponseDocument();
                    response.Header = document.Header;

                    var deviceStreams = new List<IDeviceStream>();
                    foreach (var stream in document.Streams)
                    {
                        deviceStreams.Add(ProcessDeviceStream(response.Header, stream));
                    }
                    response.Streams = deviceStreams;


                    var receivedObservations = new List<IObservation>();

                    // Process Device Streams
                    foreach (var deviceStream in response.Streams)
                    {
                        // Save the most recent Sequence that was read
                        _lastSequence = deviceStream.Observations.Max(o => o.Sequence);

                        // Check to see if any Assets have changed
                        CheckAssetChanged(deviceStream.Observations, cancel);

                        // Get Observations from Device Stream
                        var observations = response.GetObservations();
                        if (!observations.IsNullOrEmpty())
                        {
                            receivedObservations.AddRange(observations);
                        }
                    }

                    SampleReceived?.Invoke(this, response);

                    foreach (var observation in receivedObservations)
                    {
                        ObservationReceived?.Invoke(this, observation);
                    }
                }
            }
        }

        private static IDevice ProcessDevice(IMTConnectDevicesHeader header, IDevice inputDevice)
        {
            var outputDevice = (Device)inputDevice;
            outputDevice.InstanceId = header.InstanceId;

            // Add DataItems
            if (!inputDevice.DataItems.IsNullOrEmpty())
            {
                var outputDataItems = new List<IDataItem>();
                foreach (var inputDataItem in inputDevice.DataItems)
                {
                    outputDataItems.Add(ProcessDataItem(header, inputDataItem));
                }
                outputDevice.DataItems = outputDataItems;
            }

            // Add Compositions
            if (!inputDevice.Compositions.IsNullOrEmpty())
            {
                var outputCompositions = new List<IComposition>();
                foreach (var inputComposition in inputDevice.Compositions)
                {
                    outputCompositions.Add(ProcessComposition(header, inputComposition));
                }
                outputDevice.Compositions = outputCompositions;
            }

            // Add Components
            if (!inputDevice.Components.IsNullOrEmpty())
            {
                var outputSubComponents = new List<IComponent>();
                foreach (var inputSubComponent in inputDevice.Components)
                {
                    outputSubComponents.Add(ProcessComponent(header, inputSubComponent));
                }
                outputDevice.Components = outputSubComponents;
            }

            return outputDevice;
        }

        private static IComponent ProcessComponent(IMTConnectDevicesHeader header, IComponent inputComponent)
        {
            var outputComponent = (Component)inputComponent;
            outputComponent.InstanceId = header.InstanceId;

            // Add DataItems
            if (!inputComponent.DataItems.IsNullOrEmpty())
            {
                var outputDataItems = new List<IDataItem>();
                foreach (var inputDataItem in inputComponent.DataItems)
                {
                    outputDataItems.Add(ProcessDataItem(header, inputDataItem));
                }
                outputComponent.DataItems = outputDataItems;
            }

            // Add Compositions
            if (!inputComponent.Compositions.IsNullOrEmpty())
            {
                var outputCompositions = new List<IComposition>();
                foreach (var inputComposition in inputComponent.Compositions)
                {
                    outputCompositions.Add(ProcessComposition(header, inputComposition));
                }
                outputComponent.Compositions = outputCompositions;
            }

            // Add Components
            if (!inputComponent.Components.IsNullOrEmpty())
            {
                var outputSubComponents = new List<IComponent>();
                foreach (var inputSubComponent in inputComponent.Components)
                {
                    outputSubComponents.Add(ProcessComponent(header, inputSubComponent));
                }
                outputComponent.Components = outputSubComponents;
            }

            return outputComponent;
        }

        private static IComposition ProcessComposition(IMTConnectDevicesHeader header, IComposition inputComposition)
        {
            var outputComposition = (Composition)inputComposition;
            outputComposition.InstanceId = header.InstanceId;

            // Add DataItems
            if (!inputComposition.DataItems.IsNullOrEmpty())
            {
                var outputDataItems = new List<IDataItem>();
                foreach (var inputDataItem in inputComposition.DataItems)
                {
                    outputDataItems.Add(ProcessDataItem(header, inputDataItem));
                }
                outputComposition.DataItems = outputDataItems;
            }

            return outputComposition;
        }

        private static IDataItem ProcessDataItem(IMTConnectDevicesHeader header, IDataItem inputDataItem)
        {
            var outputDataItem = (DataItem)inputDataItem;
            outputDataItem.InstanceId = header.InstanceId;
            return outputDataItem;
        }


        private IDeviceStream ProcessDeviceStream(IMTConnectStreamsHeader header, IDeviceStream inputDeviceStream)
        {
            var outputDeviceStream = new DeviceStream();
            outputDeviceStream.Name = inputDeviceStream.Name;
            outputDeviceStream.Uuid = inputDeviceStream.Uuid;

            var componentStreams = new List<IComponentStream>();
            if (!inputDeviceStream.ComponentStreams.IsNullOrEmpty())
            {
                foreach (var componentStream in inputDeviceStream.ComponentStreams)
                {
                    componentStreams.Add(ProcessComponentStream(header, outputDeviceStream.Uuid, componentStream));
                }
            }
            outputDeviceStream.ComponentStreams = componentStreams;

            return outputDeviceStream;
        }

        private IComponentStream ProcessComponentStream(IMTConnectStreamsHeader header, string deviceUuid, IComponentStream inputComponentStream)
        {
            var outputComponentStream = new ComponentStream();
            outputComponentStream.ComponentId = inputComponentStream.ComponentId;
            outputComponentStream.ComponentType = inputComponentStream.ComponentType;
            outputComponentStream.Name = inputComponentStream.Name;
            outputComponentStream.NativeName = inputComponentStream.NativeName;
            outputComponentStream.Uuid = inputComponentStream.Uuid;

            if (inputComponentStream.ComponentType == Agent.TypeId || inputComponentStream.ComponentType == Devices.Device.TypeId)
            {
                outputComponentStream.Component = GetCachedDevice(deviceUuid);
            }
            else
            {
                outputComponentStream.Component = GetCachedComponent(deviceUuid, inputComponentStream.ComponentId);
            }

            var observations = new List<IObservation>();
            if (!inputComponentStream.Observations.IsNullOrEmpty())
            {
                foreach (var inputObservation in inputComponentStream.Observations)
                {
                    var dataItem = GetCachedDataItem(deviceUuid, inputObservation.DataItemId);
                    if (dataItem != null)
                    {
                        var outputObservation = Observation.Create(dataItem);
                        outputObservation.DeviceUuid = deviceUuid;
                        outputObservation.DataItemId = inputObservation.DataItemId;
                        outputObservation.DataItem = GetCachedDataItem(deviceUuid, inputObservation.DataItemId);
                        outputObservation.CompositionId = inputObservation.CompositionId;
                        outputObservation.Category = inputObservation.Category;
                        outputObservation.Representation = inputObservation.Representation;
                        outputObservation.Type = inputObservation.Type;
                        outputObservation.SubType = inputObservation.SubType;
                        outputObservation.Name = inputObservation.Name;
                        outputObservation.InstanceId = header.InstanceId;
                        outputObservation.Sequence = inputObservation.Sequence;
                        outputObservation.Timestamp = inputObservation.Timestamp;
                        outputObservation.AddValues(inputObservation.Values);
                        observations.Add(outputObservation);
                    }
                }
            }
            outputComponentStream.Observations = observations;

            return outputComponentStream;
        }


        private void ProcessSampleError(IErrorResponseDocument document)
        {
            _lastResponse = UnixDateTime.Now;
            ResponseReceived?.Invoke(this, new EventArgs());

            if (document != null)
            {
                MTConnectError?.Invoke(this, document);
            }
        }

        private async void CheckAssetChanged(IEnumerable<IObservation> observations, CancellationToken cancel)
        {
            if (observations != null && observations.Count() > 0)
            {
                var assetsChanged = observations.Where(o => o.Type.ToUnderscoreUpper() == Devices.DataItems.AssetChangedDataItem.TypeId);
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
                                AssetsReceived?.Invoke(this, doc);

                                if (doc != null && !doc.Assets.IsNullOrEmpty())
                                {
                                    foreach (var asset in doc.Assets)
                                    {
                                        AssetReceived?.Invoke(this, asset);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        public static string CreateUrl(string hostname, int port)
        {
            if (!string.IsNullOrEmpty(hostname))
            {
                var url = hostname;

                // Add Port
                url = Url.AddPort(url, port);

                return url;
            }

            return null;
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

        private static string CreateSampleUrl(string baseUrl, string deviceKey, ulong from, int interval, int heartbeat, long count, string path = null)
        {
            var url = baseUrl;
            if (!string.IsNullOrEmpty(deviceKey)) url = Url.Combine(url, deviceKey + "/sample");
            else url = Url.Combine(url, "sample");

            // Replace 'localhost' with '127.0.0.1' (This is due to a performance issue with .NET Core's System.Net.Http.HttpClient)
            if (url.Contains("localhost")) url = url.Replace("localhost", "127.0.0.1");

            // Check for http
            if (!url.StartsWith("http://") && !url.StartsWith("https://")) url = "http://" + url;

            if (from >= 0) url = Url.AddQueryParameter(url, "from", from);
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
                lock (_lock)
                {
                    _devices.TryGetValue(deviceUuid, out var device);
                    return device;
                }
            }

            return null;
        }

        private IComponent GetCachedComponent(string deviceUuid, string componentId)
        {
            if (!string.IsNullOrEmpty(deviceUuid) && !string.IsNullOrEmpty(componentId))
            {
                var key = $"{deviceUuid}:{componentId}";

                lock (_lock)
                {
                    _cachedComponents.TryGetValue(key, out var component);
                    if (component == null)
                    {
                        _devices.TryGetValue(deviceUuid, out var device);
                        if (device != null)
                        {
                            if (device.Id == componentId)
                            {
                                _cachedComponents.Add(key, device);
                            }
                            else
                            {
                                var components = device.GetComponents();
                                if (!components.IsNullOrEmpty())
                                {
                                    component = components.FirstOrDefault(o => o.Id == componentId);
                                    if (component != null)
                                    {
                                        _cachedComponents.Add(key, component);
                                    }
                                }
                            }
                        }
                    }

                    return component;
                }
            }

            return null;
        }

        private IDataItem GetCachedDataItem(string deviceUuid, string dataItemId)
        {
            if (!string.IsNullOrEmpty(deviceUuid) && !string.IsNullOrEmpty(dataItemId))
            {
                var key = $"{deviceUuid}:{dataItemId}";

                lock (_lock)
                {
                    _cachedDataItems.TryGetValue(key, out var dataItem);
                    if (dataItem == null)
                    {
                        _devices.TryGetValue(deviceUuid, out var device);
                        if (device != null)
                        {
                            var dataItems = device.GetDataItems();
                            if (!dataItems.IsNullOrEmpty())
                            {
                                dataItem = dataItems.FirstOrDefault(o => o.Id == dataItemId);
                                if (dataItem != null)
                                {
                                    _cachedDataItems.Add(key, dataItem);
                                }
                            }
                        }
                    }

                    return dataItem;
                }
            }

            return null;
        }

        #endregion

    }
}