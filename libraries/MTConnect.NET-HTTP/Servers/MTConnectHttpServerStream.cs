// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Http;
using MTConnect.Streams.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Servers.Http
{
    /// <summary>
    /// Server-side pump that produces the body of an MTConnect long-poll <c>sample</c> response
    /// (or a repeating <c>current</c> if <see cref="StartCurrent"/> is used). The stream pulls
    /// fresh <c>MTConnectStreams</c> documents from the agent on each tick, formats them in the
    /// negotiated <see cref="MTConnect.DocumentFormat"/>, wraps the result in a multipart-mixed
    /// chunk, and raises <see cref="DocumentReceived"/>/<see cref="HeartbeatReceived"/> so the
    /// hosting HTTP framework can write the chunk to the open response.
    /// </summary>
    public class MTConnectHttpServerStream
    {
        private const int _minInterval = 1; // 1 millisecond minimum interval


        private readonly string _id = StringFunctions.RandomString(10);
        private readonly string _boundary = UnixDateTime.Now.ToString().ToMD5Hash();
        private readonly IHttpServerConfiguration _configuration;
        private readonly IMTConnectAgentBroker _mtconnectAgent;
        private readonly string _deviceKey;
        private readonly IEnumerable<string> _dataItemIds;
        private readonly ulong _from;
        private readonly uint _count;
        private readonly int _interval;
        private readonly int _heartbeat;
        private readonly string _documentFormat;
        private readonly IEnumerable<string> _acceptEncodings;
        private readonly IEnumerable<KeyValuePair<string, string>> _formatOptions;

        private CancellationTokenSource _stop;
        private bool _isConnected;
        private bool _currentOnly;


        /// <summary>Unique random identifier for this stream instance; included in event payloads so multiple concurrent streams can be distinguished.</summary>
        public string Id => _id;

        /// <summary>The MIME multipart boundary string emitted between chunks; derived from a fresh timestamp hash so each stream has its own boundary.</summary>
        public string Boundary => _boundary;

        /// <summary>The content codings the client advertised via <c>Accept-Encoding</c>; the stream selects from this set when deciding whether to compress chunks.</summary>
        public IEnumerable<string> AcceptEncodings => _acceptEncodings;

        /// <summary>True while the worker task is running and emitting chunks; flipped to false by <see cref="Stop"/> or once the task exits.</summary>
        public bool IsConnected => _isConnected;

        /// <summary>Extra HTTP response headers the hosting framework should send alongside the multipart body (e.g. CORS or cache directives).</summary>
        public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }

        /// <summary>Raised once the worker task begins emitting chunks; the argument is <see cref="Id"/>.</summary>
        public event EventHandler<string> StreamStarted;

        /// <summary>Raised after the worker task exits; the argument is <see cref="Id"/>.</summary>
        public event EventHandler<string> StreamStopped;

        /// <summary>Raised when the worker task aborts because of an exception; the worker then rethrows so the caller can tear down the connection.</summary>
        public event EventHandler<Exception> StreamException;

        /// <summary>Raised for each chunk that carries new observations; the args expose the formatted multipart body and the time spent producing it.</summary>
        public event EventHandler<MTConnectHttpStreamArgs> DocumentReceived;

        /// <summary>Raised for each empty-document keep-alive chunk emitted after the configured heartbeat interval elapses with no observations.</summary>
        public event EventHandler<MTConnectHttpStreamArgs> HeartbeatReceived;



        /// <summary>
        /// Builds the server-side stream pump from the agent, device scope, and the request
        /// parameters parsed from the long-poll URL (<c>from</c>, <c>count</c>, <c>interval</c>,
        /// <c>heartbeat</c>, <c>documentFormat</c>, <c>path</c>-resolved DataItem ids, and
        /// <c>Accept-Encoding</c>). The pump itself is not started until <see cref="StartSample"/>
        /// or <see cref="StartCurrent"/> is called.
        /// </summary>
        /// <param name="configuration">HTTP server configuration used to pick a response compression coding.</param>
        /// <param name="mtconnectAgent">The agent broker observations are read from.</param>
        /// <param name="deviceKey">Device key the stream is scoped to; null requests an agent-wide stream.</param>
        /// <param name="dataItemIds">Optional filter of DataItem ids resolved from a <c>path</c> query parameter.</param>
        /// <param name="from">Initial sequence number passed to the agent; zero means start from the earliest available observation.</param>
        /// <param name="count">Maximum observations per chunk; zero means no per-chunk limit.</param>
        /// <param name="interval">Polling interval in milliseconds between chunks (clamped at 1 ms minimum).</param>
        /// <param name="heartbeat">Heartbeat interval in milliseconds; if no observations appear within this window, an empty document is emitted instead.</param>
        /// <param name="documentFormat">MTConnect document format used to serialise each chunk.</param>
        /// <param name="acceptEncodings">The <c>Accept-Encoding</c> tokens the client offered, used to negotiate compression.</param>
        /// <param name="formatOptions">Additional document-format options passed through to the registered formatter.</param>
        public MTConnectHttpServerStream(
            IHttpServerConfiguration configuration,
            IMTConnectAgentBroker mtconnectAgent,
            string deviceKey,
            IEnumerable<string> dataItemIds = null,
            ulong from = 0,
            uint count = 0,
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


        /// <summary>
        /// Launches the streaming worker on a background task in <c>sample</c> mode: each tick
        /// reads new observations starting from the most recent acknowledged sequence number.
        /// <paramref name="cancellationToken"/> is wired so the stream stops when the request
        /// is aborted.
        /// </summary>
        public void StartSample(CancellationToken cancellationToken)
        {
            _stop = new CancellationTokenSource();
            cancellationToken.Register(() => { Stop(); });

            _currentOnly = false;

            _ = Task.Run(Worker, _stop.Token);

            _isConnected = true;
        }

        /// <summary>
        /// Launches the streaming worker on a background task in <c>current</c> mode: each tick
        /// emits a fresh <c>MTConnectStreams</c> snapshot rather than incremental observations,
        /// useful for periodic-refresh dashboards. <paramref name="cancellationToken"/> stops the
        /// stream when the request is aborted.
        /// </summary>
        public void StartCurrent(CancellationToken cancellationToken)
        {
            _stop = new CancellationTokenSource();
            cancellationToken.Register(() => { Stop(); });

            _currentOnly = true;

            _ = Task.Run(Worker, _stop.Token);

            _isConnected = true;
        }

        /// <summary>Cancels the worker task and marks the stream disconnected. Safe to call from any thread, including before <c>Start*</c>.</summary>
        public void Stop()
        {
            if (_stop != null) _stop.Cancel();
            _isConnected = false;
        }

        /// <summary>
        /// Synchronous equivalent of <see cref="StartSample(CancellationToken)"/>: runs the
        /// worker loop on the caller's thread until <paramref name="cancellationToken"/> is
        /// cancelled. Useful for HTTP frameworks that own the response thread directly.
        /// </summary>
        public void RunSample(CancellationToken cancellationToken)
        {
            _stop = new CancellationTokenSource();
            cancellationToken.Register(() => { Stop(); });
            _currentOnly = false;

            Worker();

            _isConnected = true;
        }

        /// <summary>
        /// Synchronous equivalent of <see cref="StartCurrent(CancellationToken)"/>: emits the
        /// periodic-snapshot stream on the caller's thread. <see cref="Stop"/> must be called
        /// from another thread to break the loop.
        /// </summary>
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
                StreamStarted.Raise(this, _id, StreamException);

                // Set Content Type based documentFormat specified
                var contentType = MimeTypes.Get(_documentFormat);

                // Set Content Encoding
                var contentEncoding = GetContentEncoding();

                try
                {
                    var stpw = new System.Diagnostics.Stopwatch();

                    ulong lastSequence = Math.Max(0, _from);
                    ulong nextSequence = lastSequence;
                    long lastDocumentSent = 0;
                    long lastHeartbeatSent = 0;
                    long now = UnixDateTime.Now;
                    IStreamsResponseOutputDocument document = null;

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
                                else document = _mtconnectAgent.GetDeviceStreamsResponseDocument(_deviceKey, nextSequence, ulong.MaxValue, _count);
                            }
                        }
                        else
                        {
                            if (_currentOnly) document = _mtconnectAgent.GetDeviceStreamsResponseDocument();
                            else document = _mtconnectAgent.GetDeviceStreamsResponseDocument(nextSequence, ulong.MaxValue, _count);
                        }

                        if (document != null)
                        {
                            now = UnixDateTime.Now;

                            var chunkStream = CreateChunk(document, _documentFormat, contentEncoding, _formatOptions);

                            // Create the HTTP Multipart Chunk (with boundary)
                            var outputStream = CreateMultipartChunk(ref chunkStream, _boundary, contentType, contentEncoding);
                            if (outputStream.Position > 0) outputStream.Seek(0, SeekOrigin.Begin);

                            if (document.Streams.IsNullOrEmpty() || document.ObservationCount < 1)
                            {
                                // Check if document is null and time since last heartbeat exceeds the heartbeat specified
                                if ((now - lastHeartbeatSent) > _heartbeat)
                                {
                                    stpw.Stop();

                                    // Raise heartbeat event and include the Multipart Chunk
                                    HeartbeatReceived.Raise(this, new MTConnectHttpStreamArgs(_id, outputStream, stpw.ElapsedMilliseconds), StreamException);

                                    // Reset the heartbeat timestamp
                                    lastHeartbeatSent = now;
                                }
                            }
                            else
                            {
                                stpw.Stop();

                                // Raise heartbeat event and include the Multipart Chunk
                                DocumentReceived.Raise(this, new MTConnectHttpStreamArgs(_id, outputStream, stpw.ElapsedMilliseconds), StreamException);

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

                        // Pause the stream for the specified Interval
                        Thread.Sleep(_interval);
                    }
                }
                catch (Exception ex)
                {
                    StreamException.Raise(this, ex, null);
                    throw new Exception();
                }

                StreamStopped.Raise(this, _id, StreamException);
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

        private static Stream CreateChunk(IStreamsResponseOutputDocument document, string documentFormat, string contentEncoding, IEnumerable<KeyValuePair<string, string>> formatOptions)
        {
            if (document != null)
            {
                // Format Response Document using the specified DocumentFormatter
                var result = Formatters.ResponseDocumentFormatter.Format(documentFormat, ref document, formatOptions);
                if (result.Success)
                {
                    var bytes = result.Content;
                    MemoryStream outputStream;

                    try
                    {
                        switch (contentEncoding)
                        {
                            case HttpContentEncodings.Gzip:

                                outputStream = new MemoryStream();
                                using (var zip = new GZipStream(outputStream, CompressionMode.Compress, true))
                                {
                                    zip.CopyTo(bytes);
                                }
                                return outputStream;

#if NET5_0_OR_GREATER
                            case HttpContentEncodings.Brotli:

                                outputStream = new MemoryStream();
                                using (var zip = new BrotliStream(outputStream, CompressionMode.Compress, true))
                                {
                                    zip.CopyTo(bytes);
                                }
                                return outputStream;
#endif

                            case HttpContentEncodings.Deflate:

                                outputStream = new MemoryStream();
                                using (var zip = new DeflateStream(outputStream, CompressionMode.Compress, true))
                                {
                                    zip.CopyTo(bytes);
                                }
                                return outputStream;
                        }
                    }
                    catch { }

                    return bytes;
                }
            }

            return null;
        }

        private static Stream CreateMultipartChunk(ref Stream body, string boundary, string contentType, string contentEncoding)
        {
            var outputStream = new MemoryStream();
            using (var writer = new StreamWriter(outputStream, System.Text.Encoding.UTF8, (int)body.Length + 1000, leaveOpen: true))
            {
                writer.NewLine = "\r\n";

                // Add Boundary Line
                writer.Write("--");
                writer.Write(boundary);
                writer.WriteLine();

                // Add Content Type Line
                writer.Write("Content-type: ");
                writer.Write(contentType);
                writer.WriteLine();

                // Add Content Encoding
                if (!string.IsNullOrEmpty(contentEncoding))
                {
                    // Add Content Encoding Line
                    writer.Write("Content-encoding: ");
                    writer.Write(contentEncoding);
                    writer.WriteLine();
                }

                // Add Content Length Line
                writer.Write("Content-length: ");
                writer.Write(body.Length + 3); // Set Content-length to body.Length + 3 (newline)
                writer.WriteLine();
                writer.WriteLine();
                writer.Flush();

                // Copy Body to OutputStream
                if (body.Position > 0) body.Seek(0, SeekOrigin.Begin);
                body.CopyTo(outputStream);

                // Add NewLine
                outputStream.WriteByte(10);
                outputStream.WriteByte(13);
                outputStream.WriteByte(10);
            }

            return outputStream;
        }
    }
}