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
    public class MTConnectHttpClientStream
    {
        private const int DefaultTimeout = 300000;
        private const byte LineFeed = 10;
        private const byte CarriageReturn = 13;
        private const byte Dash = 45;
        private static readonly HttpClient _httpClient;
        private static readonly byte[] _trimBytes = new byte[] { 10, 13 };

        private CancellationTokenSource _stop;
        private string _documentFormat = DocumentFormat.XML;


        static MTConnectHttpClientStream()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMilliseconds(DefaultTimeout);
        }

        public MTConnectHttpClientStream(string url, string documentFormat = DocumentFormat.XML)
        {
            Id = Guid.NewGuid().ToString();
            Url = url;
            Timeout = DefaultTimeout;
            _documentFormat = documentFormat;
            ContentEncodings = HttpContentEncodings.DefaultAccept;
            ContentType = MimeTypes.Get(documentFormat);
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


        public event EventHandler<IStreamsResponseDocument> DocumentReceived;

        public event EventHandler<IErrorResponseDocument> ErrorReceived;

        public event EventHandler<IFormatReadResult> FormatError;

        public event EventHandler<Exception> InternalError;

        public event EventHandler<Exception> ConnectionError;

        public event EventHandler Starting;
        public event EventHandler Started;

        public event EventHandler Stopping;
        public event EventHandler Stopped;


        public void Start(CancellationToken cancellationToken)
        {
            _stop = new CancellationTokenSource();
            cancellationToken.Register(() => Stop());

            // Raise Starting Event
            Starting?.Invoke(this, new EventArgs());

            _ = Task.Run(() => Run(_stop.Token));
        }

        public void Stop()
        {
            // Raise Stopping Event
            Stopping?.Invoke(this, new EventArgs());

            if (_stop != null) _stop.Cancel();
        }


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
                        ConnectionError?.Invoke(this, new TimeoutException($"HTTP Stream Timeout Exceeded ({Timeout})"));
                    };
                    responseTimer.Start();
                }

                try
                {
                    stop.Token.ThrowIfCancellationRequested();

                    // Raise Started Event
                    Started?.Invoke(this, new EventArgs());


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
                    ConnectionError?.Invoke(this, ex);
                }
                catch (TaskCanceledException) { /* Ignore Task Cancelled */  }
                catch (HttpRequestException ex)
                {
                    ConnectionError?.Invoke(this, ex);
                }
                catch (Exception ex)
                {
                    InternalError?.Invoke(this, ex);
                }
                finally
                {
                    responseTimer.Dispose();
                }
            }

            Stopped?.Invoke(this, new EventArgs());
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
                var i = 0;
                var size = length;
                var isHeader = true;

                // Create a buffer to contain body of the response
                // based on the size of the content-length received in the Http Headers
                var body = new MemoryStream(size);

                while (i < size)
                {
                    // Create a 512 byte buffer
                    var chunk = new byte[512];

                    // Read from the Network stream and store in the chunk buffer
                    var j = stream.Read(chunk, 0, chunk.Length);

                    // Remove blank lines before header (can cause XML deserialization error if Xml Declaration is not the first line)
                    if (isHeader) chunk = ObjectExtensions.TrimStartBytes(chunk, _trimBytes);

                    // Verify bytes read doesn't exceed destination array
                    // (could be blank lines after document that gets read)
                    if (j > size - i) j = size - i;

                    // Add the chunk bytes to the body buffer
                    body.Write(chunk, 0, Math.Min(j, chunk.Length));

                    // Increment the index of the body buffer based on the number of bytes read in this chunk
                    i += j;

                    isHeader = false;
                }

                return body;
            }

            return null;
        }

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
                        DocumentReceived?.Invoke(this, document);
                    }
                    else
                    {
                        // Process MTConnectError Document (if MTConnectStreams fails)
                        var errorFormatResult = ResponseDocumentFormatter.CreateErrorResponseDocument(_documentFormat, stream);
                        if (errorFormatResult.Success)
                        {
                            var errorDocument = errorFormatResult.Content;
                            if (errorDocument != null) ErrorReceived?.Invoke(this, errorDocument);
                        }
                        else
                        {
                            // Raise Format Error
                            if (FormatError != null) FormatError.Invoke(this, errorFormatResult);
                        }
                    }
                }
                else
                {
                    // Raise Format Error
                    if (FormatError != null) FormatError.Invoke(this, formatResult);
                }
            }
        }
    }
}