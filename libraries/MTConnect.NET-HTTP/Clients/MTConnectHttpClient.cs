// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients
{
    /// <summary>
    /// Client that implements the full MTConnect HTTP REST Api Protocol (Probe, Current, Sample Stream, and Assets)
    /// </summary>
    public class MTConnectHttpClient : IMTConnectClient, IMTConnectEntityClient
    {
        private readonly HttpClient _httpClient;
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
            Init(documentFormat);
            Authority = authority;
            Device = device;
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
            Init(documentFormat);
            Authority = CreateUrl(hostname, port);
            Device = device;
        }

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
        public MTConnectHttpClient(HttpClient httpClient, string authority, string device = null, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            _httpClient = httpClient;
            Id = Guid.NewGuid().ToString();
            Init(documentFormat);
            Authority = authority;
            Device = device;
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
        public MTConnectHttpClient(HttpClient httpClient, string hostname, int port, string device = null, string documentFormat = MTConnect.DocumentFormat.XML)
        {
            _httpClient = httpClient;
            Id = Guid.NewGuid().ToString();
            Init(documentFormat);
            Authority = CreateUrl(hostname, port);
            Device = device;
        }

        private void Init(string documentFormat)
        {
            DocumentFormat = documentFormat;
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

        /// <summary>
        /// A snapshot of the device model received from the most recent Probe response,
        /// keyed by device UUID. The dictionary is empty until the first probe succeeds.
        /// Each call returns a fresh snapshot reflecting the most recently completed probe;
        /// previously returned snapshots are unaffected by subsequent probes, and devices
        /// that disappear between probes are evicted at the start of the next probe so the
        /// snapshot never carries entries the agent no longer advertises. Each
        /// <see cref="IDevice"/>'s DataItems carry the fully wired
        /// <see cref="IDataItem.Container"/> and <see cref="IDataItem.Device"/>
        /// back-pointers set during parsing, so consumers can walk the component ancestry
        /// of any DataItem without re-parsing the document.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The dictionary returned by each call is a fresh allocation independent of the
        /// cache, wrapped in a <see cref="ReadOnlyDictionary{TKey, TValue}"/>
        /// so consumers cannot mutate the snapshot through a downcast. The <see cref="IDevice"/>
        /// values within are shared references to cached instances that the client replaces
        /// wholesale on each probe. The accessor acquires the client's internal lock, so
        /// callers may enumerate the snapshot without synchronizing against the worker thread
        /// that processes subsequent probes.
        /// </para>
        /// <para>
        /// Each access allocates a fresh dictionary. Cache the returned reference if you
        /// read repeatedly between probes.
        /// </para>
        /// <para>
        /// Empty probe responses (where the agent answers with no devices) do not clear the
        /// cache; the previous snapshot stays presented until the next non-empty probe.
        /// </para>
        /// </remarks>
        public IReadOnlyDictionary<string, IDevice> Devices
        {
            get
            {
                lock (_lock)
                {
                    return new ReadOnlyDictionary<string, IDevice>(
                        new Dictionary<string, IDevice>(_devices));
                }
            }
        }


        #region "Events"

        /// <summary>
        /// Raised once per parsed device for every Probe response the client receives,
        /// carrying the fully wired <see cref="IDevice"/> instance—the same instance the
        /// <see cref="Devices"/> snapshot accessor would return for that UUID—with its
        /// DataItems' <see cref="IDataItem.Container"/> and <see cref="IDataItem.Device"/>
        /// back-pointers set, and the agent's <c>InstanceId</c> stamped on each DataItem.
        /// </summary>
        /// <remarks>
        /// Handlers fire in document order while the cache is still being populated.
        /// Subscribe to <see cref="ProbeReceived"/> to receive notification after the full
        /// probe response has been processed. A handler that throws is isolated by the
        /// client: the exception is forwarded through <see cref="InternalError"/> and the
        /// cache fill plus subsequent fan-out continue normally.
        /// </remarks>
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
            StartWorker(false, 0, 0, null, CancellationToken.None);
        }

        /// <summary>
        /// Starts the MTConnectClient
        /// </summary>
        public void Start(string path)
        {
            StartWorker(false, 0, 0, path, CancellationToken.None);
        }

        /// <summary>
        /// Starts the MTConnectClient
        /// </summary>
        public void Start(CancellationToken cancellationToken, string path = null)
        {
            StartWorker(false, 0, 0, path, cancellationToken);
        }

        /// <summary>
        /// Starts the MTConnectClient from the specified Sequence
        /// </summary>
        public void StartFromSequence(ulong instanceId, ulong sequence, string path = null)
        {
            StartWorker(true, instanceId, sequence, path, CancellationToken.None);
        }

        /// <summary>
        /// Starts the MTConnectClient from the specified Sequence
        /// </summary>
        public void StartFromSequence(ulong instanceId, ulong sequence, CancellationToken cancellationToken, string path = null)
        {
            StartWorker(true, instanceId, sequence, path, cancellationToken);
        }

        /// <summary>
        /// Starts the MTConnectClient from the beginning of the MTConnect Agent's Buffer
        /// </summary>
        public void StartFromBuffer(string path = null)
        {
            StartWorker(true, 0, 0, path, CancellationToken.None);
        }

        /// <summary>
        /// Starts the MTConnectClient from the beginning of the MTConnect Agent's Buffer
        /// </summary>
        public void StartFromBuffer(CancellationToken cancellationToken, string path = null)
        {
            StartWorker(true, 0, 0, path, cancellationToken);
        }

        /// <summary>
        /// Stops the MTConnectClient
        /// </summary>
        public void Stop()
        {
            ClientStopping.Raise(this, EventArgs.Empty, InternalError);

            CancelQuietly(_stop);
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

        private void StartWorker(bool initializeFromBuffer, ulong instanceId, ulong sequence, string path, CancellationToken cancellationToken)
        {
            // Linked to the caller's token (no registration is left behind). Starting again stops the previous worker,
            // which only ever checks its own token, so it cannot keep running alongside the new one.
            var stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var previous = Interlocked.Exchange(ref _stop, stop);
            CancelQuietly(previous);

            ClientStarting.Raise(this, EventArgs.Empty, InternalError);

            _initializeFromBuffer = initializeFromBuffer;
            _lastInstanceId = instanceId;
            _lastSequence = sequence;
            _streamPath = path;

            _ = Task.Run(() => Worker(stop.Token));
        }

        private static void CancelQuietly(CancellationTokenSource source)
        {
            try
            {
                source?.Cancel();
            }
            catch (ObjectDisposedException) { }
        }


        #region "Requests"

        /// <summary>
        /// Execute a Probe Request to return an MTConnectDevices Response Document
        /// </summary>
        public IDevicesResponseDocument GetProbe()
        {
            return CreateProbeClient().Get();
        }

        /// <summary>
        /// Execute a Probe Request to return an MTConnectDevices Response Document
        /// </summary>
        public Task<IDevicesResponseDocument> GetProbeAsync()
        {
            return GetProbeAsync(CancellationToken.None);
        }

        /// <summary>
        /// Execute a Probe Request to return an MTConnectDevices Response Document
        /// </summary>
        public Task<IDevicesResponseDocument> GetProbeAsync(CancellationToken cancellationToken)
        {
            return CreateProbeClient().GetAsync(cancellationToken);
        }


        /// <summary>
        /// Execute a Current Request to return an MTConnectStreams Response Document
        /// </summary>
        public IStreamsResponseDocument GetCurrent(long at = 0, string path = null)
        {
            return CreateCurrentClient(at, path).Get();
        }

        /// <summary>
        /// Execute a Current Request to return an MTConnectStreams Response Document
        /// </summary>
        public Task<IStreamsResponseDocument> GetCurrentAsync(long at = 0, string path = null)
        {
            return GetCurrentAsync(CancellationToken.None, at, path);
        }

        /// <summary>
        /// Execute a Current Request to return an MTConnectStreams Response Document
        /// </summary>
        public Task<IStreamsResponseDocument> GetCurrentAsync(CancellationToken cancellationToken, long at = 0, string path = null)
        {
            return CreateCurrentClient(at, path).GetAsync(cancellationToken);
        }


        /// <summary>
        /// Execute a Sample Request to return an MTConnectStreams Response Document
        /// </summary>
        public IStreamsResponseDocument GetSample(long from = 0, long to = 0, int count = 0, string path = null)
        {
            return CreateSampleClient(from, to, count, path).Get();
        }

        /// <summary>
        /// Execute a Sample Request to return an MTConnectStreams Response Document
        /// </summary>
        public Task<IStreamsResponseDocument> GetSampleAsync(long from = 0, long to = 0, int count = 0, string path = null)
        {
            return GetSampleAsync(CancellationToken.None, from, to, count, path);
        }

        /// <summary>
        /// Execute a Sample Request to return an MTConnectStreams Response Document
        /// </summary>
        public Task<IStreamsResponseDocument> GetSampleAsync(CancellationToken cancellationToken, long from = 0, long to = 0, int count = 0, string path = null)
        {
            return CreateSampleClient(from, to, count, path).GetAsync(cancellationToken);
        }


        /// <summary>
        /// Execute a Assets Request to return an MTConnectAssets Response Document
        /// </summary>
        public IAssetsResponseDocument GetAssets(long count = 100)
        {
            return CreateAssetClient(new MTConnectHttpAssetClient(_httpClient, Authority, count, null, null, DocumentFormat)).Get();
        }

        /// <summary>
        /// Execute a Assets Request to return an MTConnectAssets Response Document
        /// </summary>
        public Task<IAssetsResponseDocument> GetAssetsAsync(ulong count = 100)
        {
            return GetAssetsAsync(CancellationToken.None, count);
        }

        /// <summary>
        /// Execute a Assets Request to return an MTConnectAssets Response Document
        /// </summary>
        public Task<IAssetsResponseDocument> GetAssetsAsync(CancellationToken cancellationToken, ulong count = 100)
        {
            return CreateAssetClient(new MTConnectHttpAssetClient(_httpClient, Authority, (long)count, null, null, DocumentFormat)).GetAsync(cancellationToken);
        }


        /// <summary>
        /// Execute a Assets Request to return an MTConnectAssets Response Document for the specified AssetId
        /// </summary>
        public IAssetsResponseDocument GetAsset(string assetId)
        {
            return CreateAssetClient(new MTConnectHttpAssetClient(_httpClient, Authority, assetId, DocumentFormat)).Get();
        }

        /// <summary>
        /// Execute a Assets Request to return an MTConnectAssets Response Document for the specified AssetId
        /// </summary>
        public Task<IAssetsResponseDocument> GetAssetAsync(string assetId)
        {
            return GetAssetAsync(assetId, CancellationToken.None);
        }

        /// <summary>
        /// Execute a Assets Request to return an MTConnectAssets Response Document for the specified AssetId
        /// </summary>
        public Task<IAssetsResponseDocument> GetAssetAsync(string assetId, CancellationToken cancellationToken)
        {
            return CreateAssetClient(new MTConnectHttpAssetClient(_httpClient, Authority, assetId, DocumentFormat)).GetAsync(cancellationToken);
        }


        // The request clients share Timeout / ContentEncodings / ContentType and report through this client's events

        private MTConnectHttpProbeClient CreateProbeClient()
        {
            var client = new MTConnectHttpProbeClient(_httpClient, Authority, Device, DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.MTConnectError += (s, doc) => MTConnectError.Raise(this, doc, InternalError);
            client.FormatError += (s, r) => FormatError.Raise(this, r, InternalError);
            client.ConnectionError += (s, ex) => ConnectionError.Raise(this, ex, InternalError);
            client.InternalError += (s, ex) => InternalError.Raise(this, ex, InternalError);
            return client;
        }

        private MTConnectHttpCurrentClient CreateCurrentClient(long at, string path)
        {
            var client = new MTConnectHttpCurrentClient(_httpClient, Authority, Device, at: at, path: path, documentFormat: DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.MTConnectError += (s, doc) => MTConnectError.Raise(this, doc, InternalError);
            client.FormatError += (s, r) => FormatError.Raise(this, r, InternalError);
            client.ConnectionError += (s, ex) => ConnectionError.Raise(this, ex, InternalError);
            client.InternalError += (s, ex) => InternalError.Raise(this, ex, InternalError);
            return client;
        }

        private MTConnectHttpSampleClient CreateSampleClient(long from, long to, int count, string path)
        {
            var client = new MTConnectHttpSampleClient(_httpClient, Authority, Device, from: from, to: to, count: count, path: path, documentFormat: DocumentFormat);
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.MTConnectError += (s, doc) => MTConnectError.Raise(this, doc, InternalError);
            client.FormatError += (s, r) => FormatError.Raise(this, r, InternalError);
            client.ConnectionError += (s, ex) => ConnectionError.Raise(this, ex, InternalError);
            client.InternalError += (s, ex) => InternalError.Raise(this, ex, InternalError);
            return client;
        }

        private MTConnectHttpAssetClient CreateAssetClient(MTConnectHttpAssetClient client)
        {
            client.Timeout = Timeout;
            client.ContentEncodings = ContentEncodings;
            client.ContentType = ContentType;
            client.MTConnectError += (s, doc) => MTConnectError.Raise(this, doc, InternalError);
            client.FormatError += (s, r) => FormatError.Raise(this, r, InternalError);
            client.ConnectionError += (s, ex) => ConnectionError.Raise(this, ex, InternalError);
            client.InternalError += (s, ex) => InternalError.Raise(this, ex, InternalError);
            return client;
        }

        #endregion

        #region "Streams"

        private async Task Worker(CancellationToken cancellationToken)
        {
            var initialRequest = true;

            ClientStarted.Raise(this, EventArgs.Empty, InternalError);

            do
            {
                try
                {
                    // Run Probe Request
                    var probe = await GetProbeAsync(cancellationToken).ConfigureAwait(false);
                    if (probe != null)
                    {
                        _lastResponse = UnixDateTime.Now;
                        ResponseReceived.Raise(this, EventArgs.Empty, InternalError);

                        ProcessProbeDocument(probe);

                        // Get All Assets
                        if (probe.Header.AssetCount > 0)
                        {
                            var assets = await GetAssetsAsync(cancellationToken, probe.Header.AssetCount).ConfigureAwait(false);
                            if (assets != null)
                            {
                                _lastResponse = UnixDateTime.Now;
                                ResponseReceived.Raise(this, EventArgs.Empty, InternalError);

                                AssetsReceived.Raise(this, assets, InternalError);
                            }
                        }

                        if (UseStreaming)
                        {
                            // Run Current Request
                            var current = await GetCurrentAsync(cancellationToken, path: _streamPath).ConfigureAwait(false);
                            if (current != null)
                            {
                                ProcessCurrentResponse(current, initialRequest, cancellationToken);

                                // Create the Url to use for the Sample Stream
                                string url = CreateSampleUrl(Authority, Device, _lastSequence, Interval, Heartbeat, MaximumSampleCount, _streamPath, DocumentFormat);
                                if (CurrentOnly) url = CreateCurrentUrl(Authority, Device, Interval, _streamPath, DocumentFormat);

                                // Create and Start the Stream
                                using (_stream = new MTConnectHttpClientStream(_httpClient, url, DocumentFormat))
                                {
                                    _stream.Timeout = Heartbeat * 3;
                                    _stream.ContentEncodings = ContentEncodings;
                                    _stream.ContentType = ContentType;
                                    _stream.Starting += (s, o) => StreamStarting.Raise(this, url, InternalError);
                                    _stream.Started += (s, o) => StreamStarted.Raise(this, url, InternalError);
                                    _stream.Stopping += (s, o) => StreamStopping.Raise(this, url, InternalError);
                                    _stream.Stopped += (s, o) => StreamStopped.Raise(this, url, InternalError);
                                    _stream.DocumentReceived += (s, doc) => { if (!cancellationToken.IsCancellationRequested) ProcessSampleDocument(doc, cancellationToken); };
                                    _stream.ErrorReceived += (s, doc) => { if (!cancellationToken.IsCancellationRequested) ProcessSampleError(doc); };
                                    _stream.FormatError += (s, r) => FormatError.Raise(this, r, InternalError);
                                    _stream.ConnectionError += (s, ex) => ConnectionError.Raise(this, ex, InternalError);
                                    _stream.InternalError += (s, ex) => InternalError.Raise(this, ex, InternalError);

                                    // Run Stream (Blocking call)
                                    await _stream.Run(cancellationToken).ConfigureAwait(false);
                                }

                                initialRequest = false;
                            }
                        }
                        else
                        {
                            do
                            {
                                // Run Current Request
                                var current = await GetCurrentAsync(cancellationToken, path: _streamPath).ConfigureAwait(false);
                                if (current == null) break; // Break out of the Current Polling loop and start at a new Probe Request

                                ProcessCurrentResponse(current, initialRequest, cancellationToken);
                                initialRequest = false;

                                // Wait for the specified pollingInterval between Current Requests
                                var pollingInterval = Math.Max(1, Interval);
                                await Task.Delay(pollingInterval, cancellationToken).ConfigureAwait(false);
                            }
                            while (!cancellationToken.IsCancellationRequested);
                        }
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(ReconnectionInterval, cancellationToken).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    InternalError.Raise(this, ex, InternalError);
                }

            } while (!cancellationToken.IsCancellationRequested);

            ClientStopped.Raise(this, EventArgs.Empty, InternalError);
        }

        // Shared by the streaming and polling paths: raise the Current events and work out where the Sample stream resumes
        private void ProcessCurrentResponse(IStreamsResponseDocument current, bool initialRequest, CancellationToken cancellationToken)
        {
            _lastResponse = UnixDateTime.Now;
            ResponseReceived.Raise(this, EventArgs.Empty, InternalError);

            // Raise CurrentReceived Event (also checks for changed Assets)
            ProcessCurrentDocument(current, cancellationToken);

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
        }

        private void ProcessProbeDocument(IDevicesResponseDocument document)
        {
            if (document != null && !document.Devices.IsNullOrEmpty())
            {
                // Clear cached DataItems, Components, and the device snapshot so devices
                // the agent no longer advertises are evicted before the new probe is loaded.
                lock (_lock)
                {
                    _cachedComponents.Clear();
                    _cachedDataItems.Clear();
                    _devices.Clear();
                }

                foreach (var device in document.Devices)
                {
                    var outputDevice = ProcessDevice(document.Header, device);

                    // Add to cached list
                    lock (_lock)
                    {
                        _devices.Remove(outputDevice.Uuid);
                        _devices.Add(outputDevice.Uuid, outputDevice);
                    }

                    // Fire per-device inside the populate loop so the cache and event stay in lockstep.
                    // Isolate subscriber exceptions per delegate so one bad handler cannot abort the
                    // populate loop, suppress ProbeReceived, or short-circuit later subscribers in the
                    // invocation list; route each fault through InternalError instead.
                    DeviceReceived.Raise(this, outputDevice, InternalError);
                }

                // Raise ProbeReceived Event
                ProbeReceived.Raise(this, document, InternalError);
            }
        }

        private void ProcessCurrentDocument(IStreamsResponseDocument document, CancellationToken cancel)
        {
            _lastResponse = UnixDateTime.Now;
            ResponseReceived.Raise(this, EventArgs.Empty, InternalError);

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


                    CurrentReceived.Raise(this, response, InternalError);


                    // Check to see if any Assets have changed
                    foreach (var deviceStream in response.Streams)
                    {
                        CheckAssetChanged(deviceStream.Observations, cancel);
                    }

                    // GetObservations returns the Observations of every Device Stream, so raise them once
                    var observations = response.GetObservations();
                    if (!observations.IsNullOrEmpty())
                    {
                        foreach (var observation in observations)
                        {
                            ObservationReceived.Raise(this, observation, InternalError);
                        }
                    }
                }
            }
        }

        private void ProcessSampleDocument(IStreamsResponseDocument document, CancellationToken cancel)
        {
            _lastResponse = UnixDateTime.Now;
            ResponseReceived.Raise(this, EventArgs.Empty, InternalError);

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

                    // Check to see if any Assets have changed
                    foreach (var deviceStream in response.Streams)
                    {
                        CheckAssetChanged(deviceStream.Observations, cancel);
                    }

                    // GetObservations returns the Observations of every Device Stream
                    var receivedObservations = response.GetObservations()?.ToList() ?? new List<IObservation>();

                    // Resume after the newest Observation received across all Device Streams ('from' is inclusive)
                    if (receivedObservations.Count > 0)
                    {
                        var nextSequence = receivedObservations.Max(o => o.Sequence) + 1;
                        if (nextSequence > _lastSequence) _lastSequence = nextSequence;
                    }

                    SampleReceived.Raise(this, response, InternalError);

                    foreach (var observation in receivedObservations)
                    {
                        ObservationReceived.Raise(this, observation, InternalError);
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

            // Fully-qualified to disambiguate from the Devices property below; the namespace using above is shadowed inside this class.
            if (inputComponentStream.ComponentType == Agent.TypeId || inputComponentStream.ComponentType == MTConnect.Devices.Device.TypeId)
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
            ResponseReceived.Raise(this, EventArgs.Empty, InternalError);

            if (document != null)
            {
                MTConnectError.Raise(this, document, InternalError);
            }
        }

        // Requests changed Assets in the background; failures are reported through InternalError and nothing is
        // raised once the client has been stopped
        private void CheckAssetChanged(IEnumerable<IObservation> observations, CancellationToken cancel)
        {
            if (observations.IsNullOrEmpty() || cancel.IsCancellationRequested) return;

            // Fully-qualified to disambiguate from the Devices property below; the namespace using above is shadowed inside this class.
            var assetIds = observations
                .Where(o => o.Type != null && o.Type.ToUnderscoreUpper() == MTConnect.Devices.DataItems.AssetChangedDataItem.TypeId)
                .Select(o => o.GetValue(ValueKeys.Result))
                .Where(o => !string.IsNullOrEmpty(o) && o != Observation.Unavailable)
                .ToList();

            if (assetIds.Count > 0) _ = ReadChangedAssetsAsync(assetIds, cancel);
        }

        private async Task ReadChangedAssetsAsync(IEnumerable<string> assetIds, CancellationToken cancel)
        {
            try
            {
                foreach (var assetId in assetIds)
                {
                    var doc = await GetAssetAsync(assetId, cancel).ConfigureAwait(false);
                    if (doc != null && !cancel.IsCancellationRequested)
                    {
                        AssetsReceived.Raise(this, doc, InternalError);

                        if (!doc.Assets.IsNullOrEmpty())
                        {
                            foreach (var asset in doc.Assets)
                            {
                                AssetReceived.Raise(this, asset, InternalError);
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                InternalError.Raise(this, ex, InternalError);
            }
        }


        /// <summary>
        /// Builds the agent base URL string (without trailing slash) by appending
        /// <paramref name="port"/> to <paramref name="hostname"/> when the port is positive.
        /// Returns <c>null</c> for null or empty <paramref name="hostname"/>.
        /// </summary>
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

        private static string CreateCurrentUrl(string baseUrl, string deviceKey, int interval, string path = null, string documentFormat = null)
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
            url = AddDocumentFormat(url, documentFormat);

            return url;
        }

        private static string CreateSampleUrl(string baseUrl, string deviceKey, ulong from, int interval, int heartbeat, long count, string path = null, string documentFormat = null)
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
            url = AddDocumentFormat(url, documentFormat);

            return url;
        }

        // Like the Probe / Current / Sample request URIs, ask for a non-XML format explicitly rather than relying on the Accept header
        private static string AddDocumentFormat(string url, string documentFormat)
        {
            if (!string.IsNullOrEmpty(documentFormat) && documentFormat != MTConnect.DocumentFormat.XML)
            {
                url = Url.AddQueryParameter(url, "documentFormat", documentFormat.ToLower());
            }

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