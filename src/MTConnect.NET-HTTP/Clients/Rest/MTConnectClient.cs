// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Errors;
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
    /// Client that implements the full MTConnect REST Api Protocol (Probe, Current, Sample Stream)
    /// </summary>
    public class MTConnectClient : IMTConnectClient
    {
        private CancellationTokenSource _stop;
        private MTConnectSampleXmlStream _sampleXmlStream;

        private long _lastInstanceId;
        private long _lastSequence;
        private long _lastResponse;
        private bool _initializeFromBuffer;


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
        public string Device { get; }

        /// <summary>
        /// Gets or Sets the Document Format to use
        /// </summary>
        public string DocumentFormat { get; }

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
        public long MaximumSampleCount { get; set; }

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
        public void Start()
        {
            _stop = new CancellationTokenSource();

            OnClientStarting?.Invoke(this, new EventArgs());

            _ = Task.Run(() => Worker(_stop.Token));
        }

        /// <summary>
        /// Starts the MTConnectClient
        /// </summary>
        public void Start(CancellationToken cancellationToken)
        {
            _stop = new CancellationTokenSource();
            cancellationToken.Register(() => { Stop(); });

            OnClientStarting?.Invoke(this, new EventArgs());

            _ = Task.Run(() => Worker(_stop.Token));
        }

        /// <summary>
        /// Starts the MTConnectClient from the specified Sequence
        /// </summary>
        public void StartFromSequence(long sequence)
        {
            _stop = new CancellationTokenSource();

            OnClientStarting?.Invoke(this, new EventArgs());

            _lastSequence = sequence;

            _ = Task.Run(() => Worker(_stop.Token));
        }

        /// <summary>
        /// Starts the MTConnectClient from the specified Sequence
        /// </summary>
        public void StartFromSequence(long sequence, CancellationToken cancellationToken)
        {
            _stop = new CancellationTokenSource();
            cancellationToken.Register(() => { Stop(); });

            OnClientStarting?.Invoke(this, new EventArgs());

            _lastSequence = sequence;

            _ = Task.Run(() => Worker(_stop.Token));
        }

        /// <summary>
        /// Starts the MTConnectClient from the beginning of the MTConnect Agent's Buffer
        /// </summary>
        public void StartFromBuffer()
        {
            _stop = new CancellationTokenSource();

            OnClientStarting?.Invoke(this, new EventArgs());

            _initializeFromBuffer = true;

            _ = Task.Run(() => Worker(_stop.Token));
        }

        /// <summary>
        /// Starts the MTConnectClient from the beginning of the MTConnect Agent's Buffer
        /// </summary>
        public void StartFromBuffer(CancellationToken cancellationToken)
        {
            _stop = new CancellationTokenSource();
            cancellationToken.Register(() => { Stop(); });

            OnClientStarting?.Invoke(this, new EventArgs());

            _initializeFromBuffer = true;

            _ = Task.Run(() => Worker(_stop.Token));
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


        private async Task<IDevicesResponseDocument> RunProbe(CancellationToken cancellationToken)
        {
            var client = new MTConnectProbeClient(Authority, Device, DocumentFormat);
            client.Timeout = Timeout;
            client.OnMTConnectError += (s, doc) => OnMTConnectError?.Invoke(this, doc);
            client.OnConnectionError += (s, ex) => OnConnectionError?.Invoke(this, ex);
            client.OnInternalError += (s, ex) => OnInternalError?.Invoke(this, ex);
            return await client.GetAsync(cancellationToken);
        }

        private async Task<IAssetsResponseDocument> RunAssets(CancellationToken cancellationToken)
        {
            var client = new MTConnectAssetClient(Authority, null, DocumentFormat);
            client.Timeout = Timeout;
            client.OnMTConnectError += (s, doc) => OnMTConnectError?.Invoke(this, doc);
            client.OnConnectionError += (s, ex) => OnConnectionError?.Invoke(this, ex);
            client.OnInternalError += (s, ex) => OnInternalError?.Invoke(this, ex);
            return await client.GetAsync(cancellationToken);
        }

        private async Task<IAssetsResponseDocument> RunAssets(string assetId, CancellationToken cancellationToken)
        {
            var client = new MTConnectAssetClient(Authority, assetId, DocumentFormat);
            client.Timeout = Timeout;
            client.OnMTConnectError += (s, doc) => OnMTConnectError?.Invoke(this, doc);
            client.OnConnectionError += (s, ex) => OnConnectionError?.Invoke(this, ex);
            client.OnInternalError += (s, ex) => OnInternalError?.Invoke(this, ex);
            return await client.GetAsync(cancellationToken);
        }

        private async Task<IStreamsResponseDocument> RunCurrent(CancellationToken cancellationToken)
        {
            var client = new MTConnectCurrentClient(Authority, Device, documentFormat: DocumentFormat);
            client.Timeout = Timeout;
            client.OnMTConnectError += (s, doc) => OnMTConnectError?.Invoke(this, doc);
            client.OnConnectionError += (s, ex) => OnConnectionError?.Invoke(this, ex);
            client.OnInternalError += (s, ex) => OnInternalError?.Invoke(this, ex);
            return await client.GetAsync(cancellationToken);
        }


        private async Task Worker(CancellationToken cancellationToken)
        {
            var initialRequest = true;
            _lastInstanceId = 0;

            if (OnClientStarted != null) OnClientStarted?.Invoke(this, new EventArgs());

            do
            {
                try
                {
                    // Run Probe Request
                    var probe = await RunProbe(cancellationToken);
                    if (probe != null)
                    {
                        _lastResponse = UnixDateTime.Now;
                        OnResponseReceived?.Invoke(this, new EventArgs());

                        // Raise ProbeReceived Event
                        OnProbeReceived?.Invoke(this, probe);

                        // Get All Assets
                        var assets = await RunAssets(cancellationToken);
                        if (assets != null)
                        {
                            _lastResponse = UnixDateTime.Now;
                            OnResponseReceived?.Invoke(this, new EventArgs());

                            OnAssetsReceived?.Invoke(this, assets);
                        }

                        // Run Current Request
                        var current = await RunCurrent(cancellationToken);
                        if (current != null)
                        {
                            _lastResponse = UnixDateTime.Now;
                            OnResponseReceived?.Invoke(this, new EventArgs());

                            // Raise CurrentReceived Event
                            OnCurrentReceived?.Invoke(this, current);

                            // Check Assets
                            if (current.Streams.Count() > 0)
                            {
                                var deviceStream = current.Streams.FirstOrDefault(o => o.Uuid == Device || o.Name == Device);
                                if (deviceStream != null && deviceStream.Observations != null) CheckAssetChanged(deviceStream.Observations, cancellationToken);
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
                            string url = CreateSampleUrl(Authority, Device, _lastSequence, Interval, Heartbeat, MaximumSampleCount);
                            if (CurrentOnly) url = CreateCurrentUrl(Authority, Device, Interval);

                            // Create and Start the Sample Stream
                            _sampleXmlStream = new MTConnectSampleXmlStream(url);
                            _sampleXmlStream.Timeout = Heartbeat * 3;
                            _sampleXmlStream.Starting += (s, o) => OnStreamStarting?.Invoke(this, url);
                            _sampleXmlStream.Started += (s, o) => OnStreamStarted?.Invoke(this, url);
                            _sampleXmlStream.Stopping += (s, o) => OnStreamStopping?.Invoke(this, url);
                            _sampleXmlStream.Stopped += (s, o) => OnStreamStopped?.Invoke(this, url);
                            _sampleXmlStream.DocumentReceived += (s, doc) => ProcessSampleDocument(doc, cancellationToken);
                            _sampleXmlStream.ErrorReceived += (s, doc) => ProcessSampleError(doc);
                            _sampleXmlStream.OnConnectionError += (s, ex) => OnConnectionError?.Invoke(this, ex);
                            _sampleXmlStream.OnInternalError += (s, ex) => OnInternalError?.Invoke(this, ex);

                            // Run Sample Stream (Blocking call)
                            await _sampleXmlStream.Run(cancellationToken);

                            initialRequest = false;
                        }
                        else
                        {
                            await Task.Delay(RetryInterval, cancellationToken);
                        }
                    }
                    else
                    {
                        await Task.Delay(RetryInterval, cancellationToken);
                    }
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    OnInternalError?.Invoke(this, ex);
                }

            } while (!cancellationToken.IsCancellationRequested);

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
                        CheckAssetChanged(deviceStream.Observations, cancel);

                        // Save the most recent Sequence that was read
                        _lastSequence = deviceStream.Observations.Max(o => o.Sequence);

                        OnSampleReceived?.Invoke(this, document);
                    }
                }
            }
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
                            var doc = await RunAssets(assetId, cancel);
                            if (doc != null)
                            {
                                OnAssetsReceived?.Invoke(this, doc);
                            }
                        }
                    }
                }
            }
        }


        private static string CreateCurrentUrl(string baseUrl, string deviceKey, int interval)
        {
            var url = baseUrl;
            if (!string.IsNullOrEmpty(deviceKey)) url = Url.Combine(url, deviceKey + "/current");
            else url = Url.Combine(url, "current");

            // Replace 'localhost' with '127.0.0.1' (This is due to a performance issue with .NET Core's System.Net.Http.HttpClient)
            if (url.Contains("localhost")) url = url.Replace("localhost", "127.0.0.1");

            // Check for http
            if (!url.StartsWith("http://") && !url.StartsWith("https://")) url = "http://" + url;

            return $"{url}?interval={interval}";
        }

        private static string CreateSampleUrl(string baseUrl, string deviceKey, long from, int interval, int heartbeat, long count)
        {
            var url = baseUrl;
            if (!string.IsNullOrEmpty(deviceKey)) url = Url.Combine(url, deviceKey + "/sample");
            else url = Url.Combine(url, "sample");

            // Replace 'localhost' with '127.0.0.1' (This is due to a performance issue with .NET Core's System.Net.Http.HttpClient)
            if (url.Contains("localhost")) url = url.Replace("localhost", "127.0.0.1");

            // Check for http
            if (!url.StartsWith("http://") && !url.StartsWith("https://")) url = "http://" + url;

            if (from > 0) url = $"{url}?from={from}&interval={interval}&count={count}";
            else url = $"{url}?interval={interval}&count={count}";

            if (heartbeat > 0) url += $"&heartbeat={heartbeat}";

            return url;
        }
    }
}
