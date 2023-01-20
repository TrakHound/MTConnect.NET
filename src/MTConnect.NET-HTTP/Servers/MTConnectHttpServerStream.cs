// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Http;
using MTConnect.Streams.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Servers.Http
{
    public class MTConnectHttpServerStream
    {
        private const int _minInterval = 1; // 1 millisecond minimum interval


        private readonly string _id = StringFunctions.RandomString(10);
        private readonly string _boundary = UnixDateTime.Now.ToString().ToMD5Hash();
        private readonly IHttpAgentConfiguration _configuration;
        private readonly IMTConnectAgentBroker _mtconnectAgent;
        private readonly string _deviceKey;
        private readonly IEnumerable<string> _dataItemIds;
        private readonly long _from;
        private readonly int _count;
        private readonly int _interval;
        private readonly int _heartbeat;
        private readonly string _documentFormat;
        private readonly IEnumerable<string> _acceptEncodings;
        private readonly IEnumerable<KeyValuePair<string, string>> _formatOptions;
        private readonly StringBuilder _multipartBuilder = new StringBuilder();

        private CancellationTokenSource _stop;
        private bool _isConnected;
        private bool _currentOnly;


        public string Id => _id;

        public string Boundary => _boundary;

        public IEnumerable<string> AcceptEncodings => _acceptEncodings;

        public bool IsConnected => _isConnected;

        public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }

        public EventHandler<string> StreamStarted { get; set; }

        public EventHandler<string> StreamStopped { get; set; }

        public EventHandler<Exception> StreamException { get; set; }

        public EventHandler<MTConnectHttpStreamArgs> DocumentReceived { get; set; }

        public EventHandler<MTConnectHttpStreamArgs> HeartbeatReceived { get; set; }



        public MTConnectHttpServerStream(
            IHttpAgentConfiguration configuration,
            IMTConnectAgentBroker mtconnectAgent,
            string deviceKey,
            IEnumerable<string> dataItemIds = null,
            long from = -1,
            int count = 0,
            int interval = 500,
            int heartbeat = 10000,
            string documentFormat = DocumentFormat.XML,
            IEnumerable<string> acceptEncodings = null,
            IEnumerable<KeyValuePair<string, string>> formatOptions = null
            )
        {
            _configuration = configuration;
            _mtconnectAgent = mtconnectAgent;
            _deviceKey = deviceKey;
            _dataItemIds = dataItemIds;
            _from = from;
            _count = count;
            _interval = Math.Max(_minInterval, interval);
            _heartbeat = heartbeat * 10000; // Convert from ms to Ticks
            _documentFormat = documentFormat;
            _acceptEncodings = acceptEncodings;
            _formatOptions = formatOptions;
        }


        public void StartSample(CancellationToken cancellationToken)
        {
            _stop = new CancellationTokenSource();
            cancellationToken.Register(() => { Stop(); });

            _currentOnly = false;

            _= Task.Run(Worker, _stop.Token);

            _isConnected = true;
        }

        public void StartCurrent(CancellationToken cancellationToken)
        {
            _stop = new CancellationTokenSource();
            cancellationToken.Register(() => { Stop(); });

            _currentOnly = true;

            _ = Task.Run(Worker, _stop.Token);

            _isConnected = true;
        }

        public void Stop()
        {
            if (_stop != null) _stop.Cancel();
            _isConnected = false;
        }

        public void RunSample()
        {
            _stop = new CancellationTokenSource();
            _currentOnly = false;

            Worker();

            _isConnected = true;
        }

        public void RunCurrent()
        {
            _stop = new CancellationTokenSource();
            _currentOnly = true;

            Worker();

            _isConnected = true;
        }


        private void Worker()
        {
            if (_mtconnectAgent != null)
            {
                StreamStarted?.Invoke(this, _id);

                // Set Content Type based documentFormat specified
                var contentType = MimeTypes.Get(_documentFormat);

                // Set Content Encoding
                var contentEncoding = GetContentEncoding();

                try
                {
                    var stpw = new System.Diagnostics.Stopwatch();

                    long lastSequence = Math.Max(0, _from);
                    long nextSequence = lastSequence;
                    long lastDocumentSent = 0;
                    long lastHeartbeatSent = 0;
                    long now = UnixDateTime.Now;
                    IStreamsResponseOutputDocument document = null;
                    byte[] chunk = null;
                    byte[] multipartChunk = null;

                    while (!_stop.IsCancellationRequested)
                    {
                        stpw.Restart();

                        // Read the MTConnectStreams document from the IMTConnectAgent
                        if (!string.IsNullOrEmpty(_deviceKey))
                        {
                            if (_currentOnly)
                            {
                                if (!_dataItemIds.IsNullOrEmpty()) document = _mtconnectAgent.GetDeviceStreamsResponseDocument(_deviceKey, _dataItemIds);
                                else document = _mtconnectAgent.GetDeviceStreamsResponseDocument(_deviceKey);
                            }
                            else
                            {
                                if (!_dataItemIds.IsNullOrEmpty()) document = _mtconnectAgent.GetDeviceStreamsResponseDocument(_deviceKey, _dataItemIds, nextSequence, long.MaxValue, _count);
                                else document = _mtconnectAgent.GetDeviceStreamsResponseDocument(_deviceKey, nextSequence, long.MaxValue, _count);
                            }
                        }
                        else
                        {
                            if (_currentOnly) document = _mtconnectAgent.GetDeviceStreamsResponseDocument();
                            else document = _mtconnectAgent.GetDeviceStreamsResponseDocument(nextSequence, long.MaxValue, _count);
                        }

                        if (document != null)
                        {
                            now = UnixDateTime.Now;
                            chunk = CreateChunk(document, _documentFormat, contentEncoding, _formatOptions);

                            // Create the HTTP Multipart Chunk (with boundary)
                            multipartChunk = CreateMultipartChunk(chunk, _boundary, contentType, contentEncoding);

                            if (document.Streams.IsNullOrEmpty() || document.ObservationCount < 1)
                            {
                                // Check if document is null and time since last heartbeat exceeds the heartbeat specified
                                if ((now - lastHeartbeatSent) > _heartbeat)
                                {
                                    stpw.Stop();

                                    // Raise heartbeat event and include the Multipart Chunk
                                    HeartbeatReceived?.Invoke(this, new MTConnectHttpStreamArgs(_id, multipartChunk, stpw.ElapsedMilliseconds));

                                    // Reset the heartbeat timestamp
                                    lastHeartbeatSent = now;
                                }
                            }
                            else
                            {
                                stpw.Stop();

                                // Raise heartbeat event and include the Multipart Chunk
                                DocumentReceived?.Invoke(this, new MTConnectHttpStreamArgs(_id, multipartChunk, stpw.ElapsedMilliseconds));

                                // Reset the document timestamp
                                lastDocumentSent = now;

                                if (document.ObservationCount > 0)
                                {
                                    lastSequence = document.LastObservationSequence;
                                    nextSequence = lastSequence + 1;
                                }
                            }
                        }

                        document = null;
                        chunk = null;
                        multipartChunk = null;


                        // Pause the stream for the specified Interval
                        Thread.Sleep(_interval);
                    }
                }
                catch (Exception ex)
                {
                    StreamException?.Invoke(this, ex);
                    throw new Exception();
                }

                StreamStopped?.Invoke(this, _id);
            }
        }

        private string GetContentEncoding()
        {
            if (_configuration != null && !_configuration.ResponseCompression.IsNullOrEmpty())
            {
                // Gzip
                if (_configuration.ResponseCompression.Contains(HttpResponseCompression.Gzip) && 
                    !_acceptEncodings.IsNullOrEmpty() && _acceptEncodings.Contains(HttpContentEncodings.Gzip))
                {
                    return HttpContentEncodings.Gzip;
                }

#if NET5_0_OR_GREATER
                // Brotli
                else if (_configuration.ResponseCompression.Contains(HttpResponseCompression.Br) &&
                    !_acceptEncodings.IsNullOrEmpty() && _acceptEncodings.Contains(HttpContentEncodings.Brotli))
                {
                    return HttpContentEncodings.Brotli;
                }
#endif

                // Deflate
                else if (_configuration.ResponseCompression.Contains(HttpResponseCompression.Deflate) &&
                    !_acceptEncodings.IsNullOrEmpty() && _acceptEncodings.Contains(HttpContentEncodings.Deflate))
                {
                    return HttpContentEncodings.Deflate;
                }
            }

            return null;
        }

        private static byte[] CreateChunk(IStreamsResponseOutputDocument document, string documentFormat, string contentEncoding, IEnumerable<KeyValuePair<string, string>> formatOptions)
        {
            if (document != null)
            {
                // Format Response Document using the specified DocumentFormatter
                var result = Formatters.ResponseDocumentFormatter.Format(documentFormat, ref document, formatOptions);
                if (result.Success)
                {
                    var bytes = result.Content;

                    try
                    {
                        switch (contentEncoding)
                        {
                            case HttpContentEncodings.Gzip:

                                using (var ms = new MemoryStream())
                                {
                                    using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
                                    {
                                        zip.Write(bytes, 0, bytes.Length);
                                    }
                                    bytes = ms.ToArray();
                                }
                                break;

#if NET5_0_OR_GREATER
                            case HttpContentEncodings.Brotli:

                                using (var ms = new MemoryStream())
                                {
                                    using (var zip = new BrotliStream(ms, CompressionMode.Compress, true))
                                    {
                                        zip.Write(bytes, 0, bytes.Length);
                                    }
                                    bytes = ms.ToArray();
                                }
                                break;
#endif

                            case HttpContentEncodings.Deflate:

                                using (var ms = new MemoryStream())
                                {
                                    using (var zip = new DeflateStream(ms, CompressionMode.Compress, true))
                                    {
                                        zip.Write(bytes, 0, bytes.Length);
                                    }
                                    bytes = ms.ToArray();
                                }
                                break;
                        }
                    }
                    catch { }

                    return bytes;
                }
            }

            return null;
        }

        private byte[] CreateMultipartChunk(byte[] body, string boundary, string contentType, string contentEncoding)
        {
            _multipartBuilder.Clear();
            var newLine = Encoding.UTF8.GetString(new byte[] { 13, 10 });

            // Add Boundary Line
            _multipartBuilder.Append("--");
            _multipartBuilder.Append(boundary);
            _multipartBuilder.Append(newLine);

            // Add Content Type Line
            _multipartBuilder.Append("Content-type: ");
            _multipartBuilder.Append(contentType);
            _multipartBuilder.Append(newLine);

            if (!string.IsNullOrEmpty(contentEncoding))
            {
                // Add Content Encoding Line
                _multipartBuilder.Append("Content-encoding: ");
                _multipartBuilder.Append(contentEncoding);
                _multipartBuilder.Append(newLine);
            }

            // Add Content Length Line
            _multipartBuilder.Append("Content-length: ");
            _multipartBuilder.Append(body.Length + 3); // Set Content-length to body.Length + 3 (newline)
            _multipartBuilder.Append(newLine);
            _multipartBuilder.Append(newLine);

            // Get Bytes from StringBuilder
            char[] a = new char[_multipartBuilder.Length];
            _multipartBuilder.CopyTo(0, a, 0, _multipartBuilder.Length);

            // Convert StringBuilder result to UTF8 Bytes
            var headerBytes = Encoding.UTF8.GetBytes(a);

            // Create Array to return that is the size of the Header + body + newline
            byte[] bytes = new byte[headerBytes.Length + body.Length + 3];

            // Add Header Bytes
            Array.Copy(headerBytes, 0, bytes, 0, headerBytes.Length);

            // Add Body bytes
            Array.Copy(body, 0, bytes, headerBytes.Length, body.Length);

            // Add NewLine
            bytes[bytes.Length - 3] = 10;
            bytes[bytes.Length - 2] = 13;
            bytes[bytes.Length - 1] = 10;

            return bytes;
        }
    }
}
