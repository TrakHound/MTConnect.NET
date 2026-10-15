// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Errors;
using MTConnect.Formatters;
using MTConnect.Http;
using MTConnect.Streams;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private static readonly HttpClient _defaultHttpClient;
        private readonly HttpClient _httpClient;

        static MTConnectHttpClientStream()
        {
            _defaultHttpClient = new HttpClient();
            _defaultHttpClient.Timeout = TimeSpan.FromMilliseconds(DefaultTimeout);
        }

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
            _httpClient = _defaultHttpClient;
            Id = Guid.NewGuid().ToString();
            Url = url;
            Timeout = DefaultTimeout;
            _documentFormat = documentFormat;
            ContentEncodings = HttpContentEncodings.DefaultAccept;
            ContentType = MimeTypes.Get(documentFormat);
        }

        /// <summary>
        /// Constructs an HTTP streaming reader for the MTConnect long-poll <c>sample</c> response
        /// at <paramref name="url"/>. The stream assigns itself a fresh <see cref="Id"/>, defaults
        /// the read timeout to five minutes, advertises the standard
        /// <see cref="HttpContentEncodings.DefaultAccept"/> on <c>Accept-Encoding</c>, and picks
        /// the matching <c>Accept</c> MIME type for <paramref name="documentFormat"/>.
        /// </summary>
        /// <param name="url">The fully built <c>sample</c> URL with <c>interval</c> / <c>heartbeat</c> parameters.</param>
        /// <param name="documentFormat">Document format key (e.g. <c>xml</c>, <c>json</c>) to request and to parse the response with.</param>
        public MTConnectHttpClientStream(HttpClient httpClient, string url, string documentFormat = DocumentFormat.XML)
        {
            _httpClient = httpClient != null ? httpClient : _defaultHttpClient;
            Id = Guid.NewGuid().ToString();
            Url = url;
            Timeout = DefaultTimeout;
            _documentFormat = documentFormat;
            ContentEncodings = HttpContentEncodings.DefaultAccept;
            ContentType = MimeTypes.Get(documentFormat);
        }

        /// <summary>
        /// Releases the stream. The underlying <see cref="HttpClient"/> is not disposed since it is either
        /// supplied by the caller or shared across all streams. The stream itself should be <see cref="Stop"/>ped first;
        /// <see cref="Dispose"/> does not cancel pending reads.
        /// </summary>
        public void Dispose() { }


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
            // Linked to the caller's token (no registration is left behind), and a restart stops the previous loop
            var stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var previous = Interlocked.Exchange(ref _stop, stop);
            CancelQuietly(previous);

            // Raise Starting Event
            Starting.Raise(this, EventArgs.Empty, InternalError);

            _ = Task.Run(() => Run(stop.Token));
        }

        /// <summary>
        /// Signals the background read loop to exit. Raises <see cref="Stopping"/> first, then
        /// cancels the internal token. <see cref="Stopped"/> is raised by the background task
        /// when it finally returns.
        /// </summary>
        public void Stop()
        {
            // Raise Stopping Event
            Stopping.Raise(this, EventArgs.Empty, InternalError);

            CancelQuietly(_stop);
        }

        private static void CancelQuietly(CancellationTokenSource source)
        {
            try
            {
                source?.Cancel();
            }
            catch (ObjectDisposedException) { }
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
                // Read timeout: the HttpClient response stream has none of its own, so every read is bounded by
                // a token that is re-armed (CancelAfter) whenever data arrives
                using (var timeout = new CancellationTokenSource())
                using (var stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token))
                {
                    if (Timeout > 0) timeout.CancelAfter(Timeout);

                    try
                    {
                        stop.Token.ThrowIfCancellationRequested();

                        // Raise Started Event
                        Started.Raise(this, EventArgs.Empty, InternalError);

                        using (var httpRequest = CreateRequest())
                        using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, stop.Token).ConfigureAwait(false))
#if NET5_0_OR_GREATER
                        using (var stream = await response.Content.ReadAsStreamAsync(stop.Token).ConfigureAwait(false))
#else
                        using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
#endif
                        {
                            var reader = new MultipartReader(stream, () => { if (Timeout > 0) timeout.CancelAfter(Timeout); });

                            while (!stop.Token.IsCancellationRequested)
                            {
                                var part = await reader.ReadPartAsync(stop.Token).ConfigureAwait(false);
                                if (part == null) break; // The Agent closed the stream

                                // Call overridable method to process the bytes contained in the body of the response
                                ProcessResponseBody(part.Body, part.ContentEncoding);
                            }
                        }
                    }
                    catch (OperationCanceledException) when (timeout.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                    {
                        ConnectionError.Raise(this, new TimeoutException($"HTTP Stream Timeout Exceeded ({Timeout})"), InternalError);
                    }
                    catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
                    {
                        ConnectionError.Raise(this, ex, InternalError);
                    }
                    catch (OperationCanceledException) { /* Ignore Task Cancelled */  }
                    catch (HttpRequestException ex)
                    {
                        ConnectionError.Raise(this, ex, InternalError);
                    }
                    catch (IOException ex)
                    {
                        // The connection was reset or closed mid-read
                        ConnectionError.Raise(this, ex, InternalError);
                    }
                    catch (Exception ex)
                    {
                        InternalError.Raise(this, ex, InternalError);
                    }
                }
            }

            Stopped.Raise(this, EventArgs.Empty, InternalError);
        }

        private HttpRequestMessage CreateRequest()
        {
            var httpRequest = new HttpRequestMessage();
            httpRequest.RequestUri = new Uri(Url);
            httpRequest.Method = HttpMethod.Get;

            // Headers are set on the Request (not HttpClient.DefaultRequestHeaders) since the HttpClient may be shared

            // Add 'Accept' HTTP Header
            httpRequest.Headers.Add(HttpHeaders.Accept, ContentType);

            // Add 'Accept-Encoding' HTTP Header
            if (!ContentEncodings.IsNullOrEmpty())
            {
                foreach (var acceptEncoding in ContentEncodings)
                {
                    httpRequest.Headers.Add(HttpHeaders.AcceptEncoding, acceptEncoding.ToString().ToLower());
                }
            }

            return httpRequest;
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
                if (formatResult.Success && formatResult.Content != null)
                {
                    DocumentReceived.Raise(this, formatResult.Content, InternalError);
                    return;
                }

                // Not an MTConnectStreams document: the Agent may have sent an MTConnectError part (e.g. OUT_OF_RANGE)
                if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
                var errorFormatResult = ResponseDocumentFormatter.CreateErrorResponseDocument(_documentFormat, stream);
                if (errorFormatResult.Success && errorFormatResult.Content != null)
                {
                    ErrorReceived.Raise(this, errorFormatResult.Content, InternalError);
                }
                else
                {
                    // Raise Format Error (for the Streams document that was expected)
                    FormatError.Raise(this, formatResult, InternalError);
                }
            }
        }


        /// <summary>
        /// One multipart part: its decoded headers and exactly Content-length bytes of body.
        /// </summary>
        internal sealed class MultipartPart
        {
            public string ContentEncoding { get; set; }
            public MemoryStream Body { get; set; }
        }

        /// <summary>
        /// Buffered reader for an MTConnect multipart/x-mixed-replace stream. Each part is a boundary line
        /// (starting with "--"), header lines in any order and case, a blank line, then Content-length bytes.
        /// </summary>
        internal sealed class MultipartReader
        {
            private const int BufferSize = 16384;

            private readonly Stream _stream;
            private readonly Action _onData;
            private readonly byte[] _buffer = new byte[BufferSize];
            private int _position;
            private int _length;


            public MultipartReader(Stream stream, Action onData = null)
            {
                _stream = stream;
                _onData = onData;
            }


            /// <summary>
            /// Reads the next part, or returns <c>null</c> when the stream ends.
            /// </summary>
            public async Task<MultipartPart> ReadPartAsync(CancellationToken cancellationToken)
            {
                while (true)
                {
                    // Find the next boundary line
                    string line;
                    do
                    {
                        line = await ReadLineAsync(cancellationToken).ConfigureAwait(false);
                        if (line == null) return null;
                    }
                    while (!line.TrimStart().StartsWith("--"));

                    // Read the part headers up to the blank line
                    var contentLength = -1;
                    string contentEncoding = null;

                    while (true)
                    {
                        line = await ReadLineAsync(cancellationToken).ConfigureAwait(false);
                        if (line == null) return null;
                        if (line.Length == 0) break;

                        var separator = line.IndexOf(':');
                        if (separator <= 0) continue;

                        var name = line.Substring(0, separator).Trim();
                        var value = line.Substring(separator + 1).Trim();

                        if (string.Equals(name, HttpHeaders.ContentLength, StringComparison.OrdinalIgnoreCase))
                        {
                            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var length)) contentLength = length;
                        }
                        else if (string.Equals(name, HttpHeaders.ContentEncoding, StringComparison.OrdinalIgnoreCase))
                        {
                            contentEncoding = value.ToLowerInvariant();
                        }
                    }

                    // A part without a usable Content-length cannot be framed; look for the next boundary
                    if (contentLength <= 0) continue;

                    var body = new MemoryStream(contentLength);
                    if (!await ReadExactlyAsync(body, contentLength, cancellationToken).ConfigureAwait(false)) return null;

                    body.Seek(0, SeekOrigin.Begin);
                    return new MultipartPart { ContentEncoding = contentEncoding, Body = TrimLeadingNewLines(body) };
                }
            }

            // Reads one line terminated by LF (a preceding CR is removed); null at end of stream
            private async Task<string> ReadLineAsync(CancellationToken cancellationToken)
            {
                var line = new MemoryStream();

                while (true)
                {
                    if (_position >= _length && !await FillAsync(cancellationToken).ConfigureAwait(false))
                    {
                        return line.Length > 0 ? Decode(line) : null;
                    }

                    var newLine = Array.IndexOf(_buffer, (byte)'\n', _position, _length - _position);
                    if (newLine < 0)
                    {
                        line.Write(_buffer, _position, _length - _position);
                        _position = _length;
                        continue;
                    }

                    line.Write(_buffer, _position, newLine - _position);
                    _position = newLine + 1;
                    return Decode(line);
                }
            }

            private async Task<bool> ReadExactlyAsync(Stream destination, int count, CancellationToken cancellationToken)
            {
                var remaining = count;
                while (remaining > 0)
                {
                    if (_position >= _length && !await FillAsync(cancellationToken).ConfigureAwait(false)) return false;

                    var n = Math.Min(remaining, _length - _position);
                    destination.Write(_buffer, _position, n);
                    _position += n;
                    remaining -= n;
                }

                return true;
            }

            private async Task<bool> FillAsync(CancellationToken cancellationToken)
            {
                _position = 0;
                _length = await _stream.ReadAsync(_buffer, 0, _buffer.Length, cancellationToken).ConfigureAwait(false);
                if (_length <= 0)
                {
                    _length = 0;
                    return false;
                }

                _onData?.Invoke();
                return true;
            }

            private static string Decode(MemoryStream line)
            {
                var bytes = line.GetBuffer();
                var length = (int)line.Length;
                if (length > 0 && bytes[length - 1] == '\r') length--;
                return Encoding.UTF8.GetString(bytes, 0, length);
            }

            // Remove blank lines before the document (the XML Declaration must be the first line)
            private static MemoryStream TrimLeadingNewLines(MemoryStream body)
            {
                var bytes = body.GetBuffer();
                var length = (int)body.Length;
                var start = 0;
                while (start < length && (bytes[start] == LineFeed || bytes[start] == CarriageReturn)) start++;

                if (start == 0) return body;
                return new MemoryStream(bytes, start, length - start, false);
            }
        }
    }
}
