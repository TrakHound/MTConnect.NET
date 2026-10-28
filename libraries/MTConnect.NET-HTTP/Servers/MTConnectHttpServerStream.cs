// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Http;
using MTConnect.Streams.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        // Chunk headers are ASCII; Encoding.UTF8 would prefix every chunk with a byte order mark
        private static readonly Encoding _headerEncoding = new UTF8Encoding(false);


        private readonly string _id = StringFunctions.RandomString(10);
        private readonly string _boundary = UnixDateTime.Now.ToString().ToMD5Hash();
        private readonly IHttpServerConfiguration _configuration;
        private readonly IMTConnectAgentBroker _mtconnectAgent;
        private readonly string _deviceKey;
        private readonly IEnumerable<string> _dataItemIds;
        private readonly ulong _from;
        private readonly uint _count;
        private readonly int _interval;
        private readonly long _heartbeat; // Ticks
        private readonly string _documentFormat;
        private readonly IEnumerable<string> _acceptEncodings;
        private readonly IEnumerable<KeyValuePair<string, string>> _formatOptions;

        private CancellationTokenSource _stop;
        private readonly SemaphoreSlim _dataSignal = new SemaphoreSlim(0, 1);
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

        /// <summary>Optional Device type filter (for example <c>Device</c> or <c>Agent</c>) applied to agent-wide streams; ignored when a device key is set.</summary>
        public string DeviceType { get; set; }

        /// <summary>The MTConnect version each chunk is produced for; <c>null</c> uses the Agent's version.</summary>
        public Version MTConnectVersion { get; set; }

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
            _heartbeat = (long)heartbeat * TimeSpan.TicksPerMillisecond; // long: an int overflows above ~214 seconds
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
            var stop = Begin(false, cancellationToken);
            _ = Task.Run(() => WorkerAsync(null, stop.Token));
        }

        /// <summary>
        /// Launches the streaming worker on a background task in <c>current</c> mode: each tick
        /// emits a fresh <c>MTConnectStreams</c> snapshot rather than incremental observations,
        /// useful for periodic-refresh dashboards. <paramref name="cancellationToken"/> stops the
        /// stream when the request is aborted.
        /// </summary>
        public void StartCurrent(CancellationToken cancellationToken)
        {
            var stop = Begin(true, cancellationToken);
            _ = Task.Run(() => WorkerAsync(null, stop.Token));
        }

        /// <summary>Cancels the worker task and marks the stream disconnected. Safe to call from any thread, including before <c>Start*</c>.</summary>
        public void Stop()
        {
            try
            {
                _stop?.Cancel();
            }
            catch (ObjectDisposedException) { }

            _isConnected = false;
        }

        /// <summary>
        /// Synchronous equivalent of <see cref="StartSample(CancellationToken)"/>: runs the
        /// worker loop on the caller's thread until <paramref name="cancellationToken"/> is
        /// cancelled. Useful for HTTP frameworks that own the response thread directly.
        /// </summary>
        public void RunSample(CancellationToken cancellationToken)
        {
            RunSampleAsync(null, cancellationToken).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Synchronous equivalent of <see cref="StartCurrent(CancellationToken)"/>: emits the
        /// periodic-snapshot stream on the caller's thread. <see cref="Stop"/> must be called
        /// from another thread to break the loop.
        /// </summary>
        public void RunCurrent()
        {
            RunCurrent(CancellationToken.None);
        }

        /// <summary>
        /// Synchronous equivalent of <see cref="StartCurrent(CancellationToken)"/> that also stops when
        /// <paramref name="cancellationToken"/> is cancelled.
        /// </summary>
        public void RunCurrent(CancellationToken cancellationToken)
        {
            RunCurrentAsync(null, cancellationToken).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Runs the <c>sample</c> stream until it is stopped or <paramref name="cancellationToken"/> is cancelled,
        /// without blocking a thread between chunks. When <paramref name="writeChunk"/> is supplied each chunk is
        /// awaited before the next one is produced, so a slow client never receives overlapping writes.
        /// <see cref="DocumentReceived"/> / <see cref="HeartbeatReceived"/> are still raised for every chunk.
        /// </summary>
        public Task RunSampleAsync(Func<MTConnectHttpStreamArgs, Task> writeChunk, CancellationToken cancellationToken)
        {
            var stop = Begin(false, cancellationToken);
            return WorkerAsync(writeChunk, stop.Token);
        }

        /// <summary>
        /// Runs the <c>current</c> stream until it is stopped or <paramref name="cancellationToken"/> is cancelled;
        /// see <see cref="RunSampleAsync"/>.
        /// </summary>
        public Task RunCurrentAsync(Func<MTConnectHttpStreamArgs, Task> writeChunk, CancellationToken cancellationToken)
        {
            var stop = Begin(true, cancellationToken);
            return WorkerAsync(writeChunk, stop.Token);
        }


        private CancellationTokenSource Begin(bool currentOnly, CancellationToken cancellationToken)
        {
            // Linked to the caller's token, so no registration is left on it after the stream ends
            var previous = _stop;
            _stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            previous?.Cancel();

            _currentOnly = currentOnly;
            _isConnected = true;

            return _stop;
        }

        private async Task WorkerAsync(Func<MTConnectHttpStreamArgs, Task> writeChunk, CancellationToken cancellationToken)
        {
            if (_mtconnectAgent != null)
            {
                StreamStarted.Raise(this, _id, StreamException);

                // Set Content Type based documentFormat specified
                var contentType = MimeTypes.Get(_documentFormat);

                // Set Content Encoding
                var contentEncoding = HttpContentEncoder.Select(_configuration, _acceptEncodings);

                // Wake the stream when the agent adds an observation
                EventHandler<Observations.IObservation> observationAdded = (s, o) => SignalData();
                _mtconnectAgent.ObservationAdded += observationAdded;

                try
                {
                    var stpw = new System.Diagnostics.Stopwatch();

                    ulong lastSequence = Math.Max(0, _from);
                    ulong nextSequence = lastSequence;
                    long lastHeartbeatSent = 0;
                    long now = UnixDateTime.Now;

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        stpw.Restart();

                        // Skip querying/formatting when the agent has nothing new and no heartbeat is due
                        var skip = !_currentOnly &&
                            nextSequence > 0 &&
                            _mtconnectAgent.LastSequence < nextSequence &&
                            (UnixDateTime.Now - lastHeartbeatSent) <= _heartbeat;

                        // Read the MTConnectStreams document from the IMTConnectAgent
                        var document = skip ? null : GetDocument(nextSequence);
                        if (document != null)
                        {
                            now = UnixDateTime.Now;

                            var isHeartbeat = document.Streams.IsNullOrEmpty() || document.ObservationCount < 1;

                            // Heartbeats are only sent once the heartbeat period has elapsed
                            if (!isHeartbeat || (now - lastHeartbeatSent) > _heartbeat)
                            {
                                var chunkStream = CreateChunk(document, _documentFormat, contentEncoding, _formatOptions);

                                // Create the HTTP Multipart Chunk (with boundary)
                                var outputStream = CreateMultipartChunk(ref chunkStream, _boundary, contentType, contentEncoding);
                                if (outputStream.Position > 0) outputStream.Seek(0, SeekOrigin.Begin);

                                stpw.Stop();
                                var args = new MTConnectHttpStreamArgs(_id, outputStream, stpw.ElapsedMilliseconds);

                                if (isHeartbeat)
                                {
                                    HeartbeatReceived.Raise(this, args, StreamException);
                                    lastHeartbeatSent = now;
                                }
                                else
                                {
                                    DocumentReceived.Raise(this, args, StreamException);

                                    if (document.ObservationCount > 0)
                                    {
                                        lastSequence = document.LastObservationSequence;
                                        nextSequence = lastSequence + 1;
                                    }
                                }

                                // Wait for the chunk to be written before producing the next one
                                if (writeChunk != null) await writeChunk(args);
                            }
                        }

                        // Pause the stream for the specified Interval (the minimum time between parts)
                        try
                        {
                            await Task.Delay(_interval, cancellationToken);

                            // Then, when there is nothing new, sleep until an observation is added or the
                            // heartbeat is due instead of waking every interval
                            if (!_currentOnly && nextSequence > 0 && _mtconnectAgent.LastSequence < nextSequence)
                            {
                                var untilHeartbeat = (_heartbeat - (UnixDateTime.Now - lastHeartbeatSent)) / TimeSpan.TicksPerMillisecond;
                                if (untilHeartbeat > 0)
                                {
                                    await _dataSignal.WaitAsync((int)Math.Min(untilHeartbeat, int.MaxValue), cancellationToken);
                                }
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    StreamException.Raise(this, ex, null);
                    throw;
                }
                finally
                {
                    _mtconnectAgent.ObservationAdded -= observationAdded;
                    _isConnected = false;
                }

                StreamStopped.Raise(this, _id, StreamException);
            }
        }

        private void SignalData()
        {
            // At most one pending signal
            if (_dataSignal.CurrentCount == 0)
            {
                try { _dataSignal.Release(); } catch (SemaphoreFullException) { }
            }
        }

        private IStreamsResponseOutputDocument GetDocument(ulong nextSequence)
        {
            var hasDataItemIds = !_dataItemIds.IsNullOrEmpty();

            if (!string.IsNullOrEmpty(_deviceKey))
            {
                if (_currentOnly)
                {
                    if (hasDataItemIds) return _mtconnectAgent.GetDeviceStreamsResponseDocument(_deviceKey, _dataItemIds, mtconnectVersion: MTConnectVersion);
                    return _mtconnectAgent.GetDeviceStreamsResponseDocument(_deviceKey, mtconnectVersion: MTConnectVersion);
                }

                if (hasDataItemIds) return _mtconnectAgent.GetDeviceStreamsResponseDocument(_deviceKey, _dataItemIds, nextSequence, ulong.MaxValue, _count, MTConnectVersion);
                return _mtconnectAgent.GetDeviceStreamsResponseDocument(_deviceKey, nextSequence, ulong.MaxValue, _count, MTConnectVersion);
            }

            // Agent-wide streams apply the path filter and device type too
            if (_currentOnly)
            {
                if (hasDataItemIds) return _mtconnectAgent.GetDeviceStreamsResponseDocument(_dataItemIds, mtconnectVersion: MTConnectVersion, deviceType: DeviceType);
                return _mtconnectAgent.GetDeviceStreamsResponseDocument(mtconnectVersion: MTConnectVersion, deviceType: DeviceType);
            }

            if (hasDataItemIds) return _mtconnectAgent.GetDeviceStreamsResponseDocument(_dataItemIds, nextSequence, ulong.MaxValue, _count, MTConnectVersion, DeviceType);
            return _mtconnectAgent.GetDeviceStreamsResponseDocument(nextSequence, ulong.MaxValue, _count, MTConnectVersion, DeviceType);
        }

        private static Stream CreateChunk(IStreamsResponseOutputDocument document, string documentFormat, string contentEncoding, IEnumerable<KeyValuePair<string, string>> formatOptions)
        {
            if (document != null)
            {
                // Format Response Document using the specified DocumentFormatter
                var result = Formatters.ResponseDocumentFormatter.Format(documentFormat, ref document, formatOptions);
                if (result.Success && result.Content != null)
                {
                    return EncodeChunk(result.Content, contentEncoding);
                }

                // Surface the formatter's reasons rather than failing later on a null chunk
                var errors = result.Errors.IsNullOrEmpty() ? "no details" : string.Join("; ", result.Errors);
                throw new InvalidOperationException(string.Format("Failed to format {0} stream chunk: {1}", documentFormat, errors));
            }

            return null;
        }

        // Compresses the formatted chunk with the negotiated content encoding (or returns it unchanged when there is none)
        private static Stream EncodeChunk(Stream content, string contentEncoding)
        {
            return HttpContentEncoder.Encode(content, contentEncoding);
        }

        private static Stream CreateMultipartChunk(ref Stream body, string boundary, string contentType, string contentEncoding)
        {
            // Sized for the part headers + body + CRLF so the buffer is not regrown while copying the body
            var outputStream = new MemoryStream((int)Math.Min(int.MaxValue, body.Length + 256));
            using (var writer = new StreamWriter(outputStream, _headerEncoding, 1024, leaveOpen: true))
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
                writer.Write(body.Length); // The CRLF after the body ends the part and is not part of its content
                writer.WriteLine();
                writer.WriteLine();
                writer.Flush();

                // Copy Body to OutputStream
                if (body.Position > 0) body.Seek(0, SeekOrigin.Begin);
                body.CopyTo(outputStream);

                // End the part with CRLF
                outputStream.WriteByte(13);
                outputStream.WriteByte(10);
            }

            body.Dispose();

            return outputStream;
        }
    }
}
