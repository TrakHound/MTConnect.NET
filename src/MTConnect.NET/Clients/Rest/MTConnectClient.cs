// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients.Rest
{
    public class MTConnectClient : IMTConnectClient
    {
        private CancellationTokenSource _stop;
        private MTConnectSampleXmlStream _sampleXmlStream;
        //private MTConnectSampleJsonStream _sampleJsonStream;

        private long _lastInstanceId;
        private long _lastSequence;
        private long _lastResponse;
        private bool _initializeFromBuffer;


        public MTConnectClient()
        {
            Init();
        }

        public MTConnectClient(string baseUrl)
        {
            Init();
            BaseUrl = baseUrl;
        }

        public MTConnectClient(string baseUrl, string deviceName)
        {
            Init();
            BaseUrl = baseUrl;
            DeviceName = deviceName;
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
        /// The base URL for the MTConnect Client Requests
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// (Optional) The name of the requested device
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Gets or Sets the Document Format to use
        /// </summary>
        public DocumentFormat DocumentFormat { get; set; }

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


        public bool CurrentOnly { get; set; }


        #region "Events"

        /// <summary>
        /// Raised when an MTConnectDevices Document is received
        /// </summary>
        public EventHandler<Devices.DevicesDocument> OnProbeReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectSreams Document is received from a Current Request
        /// </summary>
        public EventHandler<Streams.StreamsDocument> OnCurrentReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectSreams Document is received from the Samples Stream
        /// </summary>
        public EventHandler<Streams.StreamsDocument> OnSampleReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectAssets Document is received
        /// </summary>
        public EventHandler<Assets.AssetsDocument> OnAssetsReceived { get; set; }

        /// <summary>
        /// Raised when an MTConnectError Document is received
        /// </summary>
        public EventHandler<Errors.ErrorDocument> OnMTConnectError { get; set; }

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


        private async Task<Devices.DevicesDocument> RunProbe(CancellationToken cancellationToken)
        {
            var client = new MTConnectProbeClient(BaseUrl, DeviceName, DocumentFormat);
            client.Timeout = Timeout;
            client.OnMTConnectError += (s, doc) => OnMTConnectError?.Invoke(this, doc);
            client.OnConnectionError += (s, ex) => OnConnectionError?.Invoke(this, ex);
            client.OnInternalError += (s, ex) => OnInternalError?.Invoke(this, ex);
            return await client.GetAsync(cancellationToken);
        }

        private async Task<Assets.AssetsDocument> RunAssets(CancellationToken cancellationToken)
        {
            var client = new MTConnectAssetClient(BaseUrl, DocumentFormat);
            client.Timeout = Timeout;
            client.OnMTConnectError += (s, doc) => OnMTConnectError?.Invoke(this, doc);
            client.OnConnectionError += (s, ex) => OnConnectionError?.Invoke(this, ex);
            client.OnInternalError += (s, ex) => OnInternalError?.Invoke(this, ex);
            return await client.GetAsync(cancellationToken);
        }

        private async Task<Assets.AssetsDocument> RunAssets(string assetId, CancellationToken cancellationToken)
        {
            var client = new MTConnectAssetClient(BaseUrl, assetId, DocumentFormat);
            client.Timeout = Timeout;
            client.OnMTConnectError += (s, doc) => OnMTConnectError?.Invoke(this, doc);
            client.OnConnectionError += (s, ex) => OnConnectionError?.Invoke(this, ex);
            client.OnInternalError += (s, ex) => OnInternalError?.Invoke(this, ex);
            return await client.GetAsync(cancellationToken);
        }

        private async Task<Streams.StreamsDocument> RunCurrent(CancellationToken cancellationToken)
        {
            var client = new MTConnectCurrentClient(BaseUrl, DeviceName, DocumentFormat);
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
                            if (current.Streams.Count > 0)
                            {
                                var deviceStream = current.Streams.Find(o => o.Name == DeviceName);
                                if (deviceStream != null && deviceStream.DataItems != null) CheckAssetChanged(deviceStream.DataItems, cancellationToken);
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
                            string url = CreateSampleUrl(BaseUrl, DeviceName, _lastSequence, Interval, Heartbeat, MaximumSampleCount);
                            if (CurrentOnly) url = CreateCurrentUrl(BaseUrl, DeviceName, Interval);

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

        private void ProcessSampleDocument(Streams.StreamsDocument document, CancellationToken cancel)
        {
            _lastResponse = UnixDateTime.Now;
            OnResponseReceived?.Invoke(this, new EventArgs());

            if (document != null)
            {
                // Set Agent Instance ID
                if (document.Header != null) _lastInstanceId = document.Header.InstanceId;

                if (!document.Streams.IsNullOrEmpty())
                {
                    Streams.DeviceStream deviceStream = null;

                    // Get the DeviceStream for the DeviceName or default to the first
                    if (!string.IsNullOrEmpty(DeviceName)) deviceStream = document.Streams.Find(o => o.Name == DeviceName);
                    else deviceStream = document.Streams[0];

                    if (deviceStream != null & deviceStream.DataItems != null && deviceStream.DataItems.Count > 0)
                    {
                        CheckAssetChanged(deviceStream.DataItems, cancel);

                        // Save the most recent Sequence that was read
                        _lastSequence = deviceStream.DataItems.Max(o => o.Sequence);

                        OnSampleReceived?.Invoke(this, document);
                    }
                }
            }

            //if (!string.IsNullOrEmpty(xml))
            //{
            //    // Process MTConnectStreams Document
            //    var doc = Streams.Document.FromXml(xml);

            //    else
            //    {
            //        // Process MTConnectError Document (if MTConnectDevices fails)
            //        var errorDoc = Errors.Document.Create(xml);
            //        if (errorDoc != null) OnMTConnectError?.Invoke(this, errorDoc);
            //    }

            //    doc = null;
            //}
        }

        private void ProcessSampleError(Errors.ErrorDocument document)
        {
            _lastResponse = UnixDateTime.Now;
            OnResponseReceived?.Invoke(this, new EventArgs());

            if (document != null)
            {
                OnMTConnectError?.Invoke(this, document);
            }
        }

        //private void ProcessSampleResponse(string xml, CancellationToken cancel)
        //{
        //    _lastResponse = UnixDateTime.Now;
        //    OnResponseReceived?.Invoke(this, new EventArgs());

        //    if (!string.IsNullOrEmpty(xml))
        //    {
        //        // Process MTConnectStreams Document
        //        var doc = Streams.Document.FromXml(xml);
        //        if (doc != null)
        //        {
        //            // Set Agent Instance ID
        //            if (doc.Header != null) _lastInstanceId = doc.Header.InstanceId;

        //            if (doc.DeviceStreams.Count > 0)
        //            {
        //                Streams.DeviceStream deviceStream = null;

        //                // Get the DeviceStream for the DeviceName or default to the first
        //                if (!string.IsNullOrEmpty(DeviceName)) deviceStream = doc.DeviceStreams.Find(o => o.Name == DeviceName);
        //                else deviceStream = doc.DeviceStreams[0];

        //                if (deviceStream != null & deviceStream.DataItems != null && deviceStream.DataItems.Count > 0)
        //                {
        //                    CheckAssetChanged(deviceStream.DataItems, cancel);

        //                    // Save the most recent Sequence that was read
        //                    _lastSequence = deviceStream.DataItems.Max(o => o.Sequence);

        //                    OnSampleReceived?.Invoke(this, doc);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            // Process MTConnectError Document (if MTConnectDevices fails)
        //            var errorDoc = Errors.Document.Create(xml);
        //            if (errorDoc != null) OnMTConnectError?.Invoke(this, errorDoc);
        //        }

        //        doc = null;
        //    }
        //}

        private async void CheckAssetChanged(IEnumerable<Streams.IDataItem> dataItems, CancellationToken cancel)
        {
            if (dataItems != null && dataItems.Count() > 0)
            {
                var assetsChanged = dataItems.Where(o => o.Type == "AssetChanged");
                if (assetsChanged != null)
                {
                    foreach (var assetChanged in assetsChanged)
                    {
                        string assetId = assetChanged.CDATA;
                        if (assetId != "UNAVAILABLE")
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


        private static string CreateCurrentUrl(string baseUrl, string deviceName, int interval)
        {
            var url = baseUrl;
            if (!string.IsNullOrEmpty(deviceName)) url = Url.Combine(url, deviceName + "/current");
            else url = Url.Combine(url, "current");

            return $"{url}?interval={interval}";
        }

        private static string CreateSampleUrl(string baseUrl, string deviceName, long from, int interval, int heartbeat, long count)
        {
            var url = baseUrl;
            if (!string.IsNullOrEmpty(deviceName)) url = Url.Combine(url, deviceName + "/sample");
            else url = Url.Combine(url, "sample");

            // Check for http
            if (!url.StartsWith("http://") && !url.StartsWith("https://")) url = "http://" + url;

            if (from > 0) return $"{url}?from={from}&interval={interval}&heartbeat={heartbeat}&count={count}";
            else return $"{url}?interval={interval}&heartbeat={heartbeat}&count={count}";
        }
    }
}
