// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Errors;
using MTConnect.Formatters;
using MTConnect.Http;
using MTConnect.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients
{
    /// <summary>
    /// An Http Stream for reading MTConnect Sample or Current streams and returns MTConnectStreamsResponse documents
    /// </summary>
    public class MTConnectHttpClientStream : IDisposable
    {
        private const int DefaultTimeout = 300000;
        private const byte LineFeed = 10;
        private const byte CarriageReturn = 13;
        private const byte Dash = 45;
        private static readonly byte[] _trimBytes = new byte[] { LineFeed, CarriageReturn };
        private readonly HttpClient _httpClient;

        private CancellationTokenSource _stop;
        private string _documentFormat = DocumentFormat.XML;


        /// <summary>
        /// Constructs an HTTP streaming reader for the MTConnect long-poll <c>sample</c> response
        /// at <paramref name="url"/>. The stream assigns itself a fresh <see cref="Id"/>, defaults
        /// the read timeout to five minutes, advertises the standard
        /// <see cref="HttpContentEncodings.DefaultAccept"/> on <c>Accept-Encoding</c>, and picks
        /// the matching <c>Accept</c> MIME type for <paramref name="documentFormat"/>.
        /// </summary>
        /// <param name="url">The fully built <c>sample</c> URL with <c>interval</c> / <c>heartbeat</c> parameters.</param>
        /// <param name="documentFormat">Document format key (e.g. <c>xml</c>, <c>json</c>) to request and to parse the response with.</param>
        public MTConnectHttpClientStream(string url, string documentFormat = DocumentFormat.XML)
        {
            Id = Guid.NewGuid().ToString();
            Url = url;
            Timeout = DefaultTimeout;
            _documentFormat = documentFormat;
            ContentEncodings = HttpContentEncodings.DefaultAccept;
            ContentType = MimeTypes.Get(documentFormat);

            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMilliseconds(DefaultTimeout);
        }

        /// <summary>Disposes the underlying <see cref="HttpClient"/>. The stream itself should be <see cref="Stop"/>ped first; <see cref="Dispose"/> does not cancel pending reads.</summary>
        public void Dispose()
        {
            if (_httpClient != null) _httpClient.Dispose();
        }


        /// <summary>
        /// The unique ID of the Stream
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The URL that the stream will read from
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The Timeout (in milliseconds) that the stream will wait for information before exiting
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// List of Encodings (ex. gzip, br, deflate) to pass to the Accept-Encoding HTTP Header
        /// </summary>
        public IEnumerable<HttpContentEncoding> ContentEncodings { get; set; }

        /// <summary>
        /// Gets or Sets the Content-type (or MIME-type) to pass to the Accept HTTP Header
        /// </summary>
        public string ContentType { get; set; }


        /// <summary>Raised for every successfully parsed <c>MTConnectStreams</c> document received from the agent.</summary>
        public event EventHandler<IStreamsResponseDocument> DocumentReceived;

        /// <summary>Raised when the agent returns a parsed <c>MTConnectError</c> document (e.g. invalid sequence number, no such device).</summary>
        public event EventHandler<IErrorResponseDocument> ErrorReceived;

        /// <summary>Raised when an incoming multipart part could not be parsed in the configured <see cref="ContentType"/>.</summary>
        public event EventHandler<IFormatReadResult> FormatError;

        /// <summary>Raised for unexpected exceptions inside the stream pump (parsing, timer callbacks, dispatch); the loop tries to keep running.</summary>
        public event EventHandler<Exception> InternalError;

        /// <summary>Raised for transport-level failures (DNS, refused connection, TLS error) while opening or reading the HTTP response.</summary>
        public event EventHandler<Exception> ConnectionError;

        /// <summary>Raised before the background read task is scheduled.</summary>
        public event EventHandler Starting;
        /// <summary>Raised once the background read task has begun consuming the response body.</summary>
        public event EventHandler Started;

        /// <summary>Raised when <see cref="Stop"/> is invoked, before the cancellation token is cancelled.</summary>
        public event EventHandler Stopping;
        /// <summary>Raised after the background read task has exited cleanly.</summary>
        public event EventHandler Stopped;


        /// <summary>
        /// Starts the background read loop. <paramref name="cancellationToken"/> is wired so that
        /// the stream stops cleanly when the caller's token is cancelled; <see cref="Stop"/> can
        /// also be called directly. Raises <see cref="Starting"/> before the task is scheduled.
        /// </summary>
        public void Start(CancellationToken cancellationToken)
        {
            _stop = new CancellationTokenSource();
            cancellationToken.Register(() => Stop());

            // Raise Starting Event
            RaiseEvent(Starting, EventArgs.Empty);

            _ = Task.Run(() => Run(_stop.Token));
        }

        /// <summary>
        /// Signals the background read loop to exit. Raises <see cref="Stopping"/> first, then
        /// cancels the internal token. <see cref="Stopped"/> is raised by the background task
        /// when it finally returns.
        /// </summary>
        public void Stop()
        {
            // Raise Stopping Event
            RaiseEvent(Stopping, EventArgs.Empty);

            if (_stop != null) _stop.Cancel();
        }


        /// <summary>
        /// The background read loop: opens the long-poll <c>sample</c> request to
        /// <see cref="Url"/>, parses each MIME multipart part as an MTConnect document, raises
        /// the appropriate <see cref="DocumentReceived"/> / <see cref="ErrorReceived"/> event,
        /// and continues until <paramref name="cancellationToken"/> is cancelled or the agent
        /// closes the response. Transport failures raise <see cref="ConnectionError"/>; the loop
        /// then reconnects after a short delay so transient outages do not require restart.
        /// </summary>
        public async Task Run(CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(Url))
            {
                var stop = new CancellationTokenSource();
                cancellationToken.Register(() => stop.Cancel());

                // Response Timer to Timeout Stream Read
                // : For some reason the HttpClient stream doesn't have a timeout
                var responseTimer = new System.Timers.Timer();
                if (Timeout > 0)
                {
                    responseTimer.AutoReset = false;
                    responseTimer.Interval = Timeout;
                    responseTimer.Elapsed += (o, e) =>
                    {
                        stop.Cancel();
                        RaiseEvent(ConnectionError, new TimeoutException($"HTTP Stream Timeout Exceeded ({Timeout})"));
                    };
                    responseTimer.Start();
                }

                try
                {
                    stop.Token.ThrowIfCancellationRequested();

                    // Raise Started Event
                    RaiseEvent(Started, EventArgs.Empty);


                    // Add 'Accept' HTTP Header
                    _httpClient.DefaultRequestHeaders.Add(HttpHeaders.Accept, ContentType);

                    // Add 'Accept-Encoding' HTTP Header 
                    if (!ContentEncodings.IsNullOrEmpty())
                    {
                        foreach (var acceptEncoding in ContentEncodings)
                        {
                            _httpClient.DefaultRequestHeaders.Add(HttpHeaders.AcceptEncoding, acceptEncoding.ToString().ToLower());
                        }
                    }


                    var httpRequest = new HttpRequestMessage();
                    httpRequest.RequestUri = new Uri(Url);
                    httpRequest.Method = HttpMethod.Get;


                    using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, stop.Token))
#if NET5_0_OR_GREATER
                    using (var stream = await response.Content.ReadAsStreamAsync(stop.Token))
#else
                    using (var stream = await response.Content.ReadAsStreamAsync())
#endif
                    {
                        var header = new List<byte>();
                        var headerActive = false;
                        var cr = false;
                        var lf = false;

                        var prevByte = -1;
                        var prevNewLine = false;
                        var line = new List<byte>();
                        var contentLength = 0;
                        string contentEncoding = null;
                        string lineStr = null;


                        var readBuffer = new byte[1];
                        var read = await stream.ReadAsync(readBuffer, 0, readBuffer.Length, stop.Token);
                        int b = read > 0 ? readBuffer[0] : -1;

                        if (Timeout > 0)
                        {
                            responseTimer.Stop();
                            responseTimer.Start();
                        }

                        while (b > -1 && !stop.Token.IsCancellationRequested)
                        {
                            // Look for Start of Http Multipart boundary
                            if (b == Dash && prevByte == Dash) // 45 = UTF-8 '-'
                            {
                                headerActive = true;
                            }

                            // Look for New Line characters
                            if (b == CarriageReturn) cr = true; // 13 = UTF-8 Carriage Return
                            else if (b == LineFeed) lf = true; // 10 = UTF-8 Line Feed

                            if (cr && lf)
                            {
                                // Get the current line as a UTF-8 string
                                lineStr = Encoding.UTF8.GetString(line.ToArray());

                                if (headerActive)
                                {
                                    // Add Header
                                    header.AddRange(line.ToArray());

                                    if (prevNewLine)
                                    {
                                        // If 2 consecutive new lines, then signal the end of the Http Header
                                        headerActive = false;
                                        header.Clear();
                                    }
                                }

                                // Read Content Length
                                var headerValue = GetHeaderValue(lineStr, HttpHeaders.ContentLength.ToLower());
                                if (headerValue != null) contentLength = headerValue.ToInt();

                                // Read Content Encoding
                                headerValue = GetHeaderValue(lineStr, HttpHeaders.ContentEncoding.ToLower());
                                if (headerValue != null) contentEncoding = headerValue;

                                if (!headerActive && contentLength > 0)
                                {
                                    // Once the Header is no longer active and a Content-length has been read
                                    // Read the response body based on the Content-length
                                    var bodyBytes = ReadBody(stream, contentLength);
                                    if (bodyBytes != null)
                                    {
                                        // Call overridable method to process the bytes contained in the body of the response
                                        ProcessResponseBody(bodyBytes, contentEncoding);
                                    }

                                    contentLength = 0;
                                }

                                cr = false;
                                lf = false;
                                line.Clear();
                                prevNewLine = true;
                            }

                            // Add byte to Line Buffer
                            line.Add((byte)b);

                            prevByte = b;

                            // Read the next Byte
                            read = await stream.ReadAsync(readBuffer, 0, readBuffer.Length, stop.Token);
                            b = read > 0 ? readBuffer[0] : -1;

                            if (Timeout > 0)
                            {
                                responseTimer.Stop();
                                responseTimer.Start();
                            }
                        }
                    }
                }
                catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
                {
                    RaiseEvent(ConnectionError, ex);
                }
                catch (TaskCanceledException) { /* Ignore Task Cancelled */  }
                catch (HttpRequestException ex)
                {
                    RaiseEvent(ConnectionError, ex);
                }
                catch (Exception ex)
                {
                    RaiseEvent(InternalError, ex);
                }
                finally
                {
                    responseTimer.Dispose();
                }
            }

            RaiseEvent(Stopped, EventArgs.Empty);
        }

        private static string GetHeaderValue(string s, string name)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var x = s.ToLower().TrimStart();
                if (x.StartsWith(name + ":"))
                {
                    var i = x.IndexOf(':');
                    if (i > 0)
                    {
                        return x.Substring(i + 1).Trim();
                    }
                }
            }

            return null;
        }

        private static Stream ReadBody(Stream stream, int length)
        {
            if (stream != null && length > 0)
            {
                int i = 0;
                int size = length;
                bool isHeader = true;
                int j;
                int k;

                // Create a buffer to contain body of the response
                // based on the size of the content-length received in the Http Headers
                var body = new MemoryStream();

                // Create a 255 byte buffer
                var chunk = new byte[255];

                while (i < size)
                {
                    // Read from the Network stream and store in the chunk buffer
                    j = stream.Read(chunk, 0, chunk.Length);

                    // Remove blank lines before header (can cause XML deserialization error if Xml Declaration is not the first line)
                    if (isHeader)
                    {
                        k = ObjectExtensions.TrimStartBytes(ref chunk, _trimBytes);
                        j -= k;
                    }

                    // Add the chunk bytes to the body buffer
                    body.Write(chunk, 0, j);

                    // Increment the index of the body buffer based on the number of bytes read in this chunk
                    i += j;

                    isHeader = false;
                }

                return body;
            }

            return null;
        }

        /// <summary>
        /// Decompresses <paramref name="responseBody"/> according to <paramref name="contentEncoding"/>
        /// (the agent's <c>Content-Encoding</c> response header) and dispatches the resulting
        /// MTConnect document to <see cref="DocumentReceived"/>, <see cref="ErrorReceived"/>, or
        /// <see cref="FormatError"/> depending on the document type. Override to inject custom
        /// per-part processing while preserving the standard event surface.
        /// </summary>
        /// <param name="responseBody">A single multipart part containing one formatted MTConnect document.</param>
        /// <param name="contentEncoding">The agent-reported content encoding, or <c>null</c> when the part is uncompressed.</param>
        protected virtual void ProcessResponseBody(Stream responseBody, string contentEncoding = null)
        {
            if (responseBody != null && responseBody.Length > 0)
            {
                // Handle Compression Encoding
                var stream = MTConnectHttpResponse.HandleContentEncoding(contentEncoding, responseBody);
                if (stream.Position > 0) stream.Seek(0, SeekOrigin.Begin);

                var formatResult = ResponseDocumentFormatter.CreateStreamsResponseDocument(_documentFormat, stream);
                if (formatResult.Success)
                {
                    // Process MTConnectDevices Document
                    var document = formatResult.Content;
                    if (document != null)
                    {
                        RaiseEvent(DocumentReceived, document);
                    }
                    else
                    {
                        // Process MTConnectError Document (if MTConnectStreams fails)
                        var errorFormatResult = ResponseDocumentFormatter.CreateErrorResponseDocument(_documentFormat, stream);
                        if (errorFormatResult.Success)
                        {
                            var errorDocument = errorFormatResult.Content;
                            if (errorDocument != null) RaiseEvent(ErrorReceived, errorDocument);
                        }
                        else
                        {
                            // Raise Format Error
                            RaiseEvent(FormatError, errorFormatResult);
                        }
                    }
                }
                else
                {
                    // Raise Format Error
                    RaiseEvent(FormatError, formatResult);
                }
            }
        }

        // Iterate the invocation list so one throwing subscriber cannot short-circuit
        // the multicast and starve later subscribers. Each fault is forwarded through
        // InternalError; if InternalError itself faults, swallow that secondary fault
        // so the remaining subscribers in the invocation list still receive the event.
        private void RaiseEvent<T>(EventHandler<T> handler, T arg)
        {
            if (handler == null) return;

            foreach (var subscriber in handler.GetInvocationList())
            {
                try
                {
                    ((EventHandler<T>)subscriber).Invoke(this, arg);
                }
                catch (Exception ex)
                {
                    RouteSubscriberFault(ex);
                }
            }
        }

        // Non-generic sibling of RaiseEvent&lt;T&gt; for EventHandler events that carry no
        // typed payload (Starting, Started, Stopping, Stopped). Same multicast-isolation
        // contract: a throwing subscriber cannot starve later subscribers, and a
        // faulting InternalError handler cannot break the fan-out either.
        private void RaiseEvent(EventHandler handler, EventArgs arg)
        {
            if (handler == null) return;

            foreach (var subscriber in handler.GetInvocationList())
            {
                try
                {
                    ((EventHandler)subscriber).Invoke(this, arg);
                }
                catch (Exception ex)
                {
                    RouteSubscriberFault(ex);
                }
            }
        }

        // Forwards a subscriber fault to InternalError and swallows any secondary
        // fault raised by InternalError itself so the originating event's fan-out
        // can keep running. Shared between the generic and non-generic RaiseEvent
        // helpers above.
        private void RouteSubscriberFault(Exception ex)
        {
            try
            {
                InternalError?.Invoke(this, ex);
            }
            catch
            {
                // A faulting InternalError handler must not break the event fan-out.
            }
        }
    }
}