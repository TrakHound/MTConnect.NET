// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Errors;
using MTConnect.Http;
using MTConnect.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients.Rest
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

        private static readonly HttpClient _httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromMilliseconds(DefaultTimeout)
        };

        private CancellationTokenSource _stop;
        private string _documentFormat = DocumentFormat.XML;


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


        public EventHandler<IStreamsResponseDocument> DocumentReceived { get; set; }

        public EventHandler<IErrorResponseDocument> ErrorReceived { get; set; }

        public EventHandler<Exception> OnInternalError { get; set; }

        public EventHandler<Exception> OnConnectionError { get; set; }

        public EventHandler Starting { get; set; }
        public EventHandler Started { get; set; }

        public EventHandler Stopping { get; set; }
        public EventHandler Stopped { get; set; }


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
                        OnConnectionError?.Invoke(this, new TimeoutException($"HTTP Stream Timeout Exceeded ({Timeout})"));
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


                    // Get the HTTP Stream 
#if NET5_0_OR_GREATER
                    using (var stream = await _httpClient.GetStreamAsync(Url, stop.Token))
#else
                    using (var stream = await _httpClient.GetStreamAsync(Url))
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
                        var trimBytes = new byte[] { 10, 13 };
                        string lineStr = null;

                        var readBuffer = new byte[1];
                        await stream.ReadAsync(readBuffer, 0, readBuffer.Length, stop.Token);
                        int b = readBuffer[0];

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
                                // Trim CR and LF bytes from beginning and end
                                //var lineBytes = ObjectExtensions.TrimBytes(line.ToArray(), trimBytes);

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
                            await stream.ReadAsync(readBuffer, 0, readBuffer.Length, stop.Token);
                            b = readBuffer[0];

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
                    OnConnectionError?.Invoke(this, ex);
                }
                catch (TaskCanceledException) { /* Ignore Task Cancelled */  }
                catch (HttpRequestException ex)
                {
                    OnConnectionError?.Invoke(this, ex);
                }
                catch (Exception ex)
                {
                    OnInternalError?.Invoke(this, ex);
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

        private static byte[] ReadBody(Stream stream, int length)
        {
            if (stream != null && length > 0)
            {
                var i = 0;
                var size = length;

                // Create a buffer to contain body of the response
                // based on the size of the content-length received in the Http Headers
                var body = new byte[size];

                // Read New Line characters
                // These always appear after the Http Header
                stream.ReadByte(); // 13
                stream.ReadByte(); // 10

                while (i < size)
                {
                    // Create a 512 byte buffer
                    var chunk = new byte[512];

                    // Read from the Network stream and store in the chunk buffer
                    var j = stream.Read(chunk, 0, chunk.Length);

                    // Verify bytes read doesn't exceed destination array
                    // (could be blank lines after document that gets read)
                    if (j > size - i) j = size - i;

                    // Add the chunk bytes to the body buffer
                    Array.Copy(chunk, 0, body, i, j);

                    // Increment the index of the body buffer based on the number of bytes read in this chunk
                    i += j;
                }

                return body;
            }

            return null;
        }

        protected virtual void ProcessResponseBody(byte[] responseBody, string contentEncoding = null)
        {
            if (responseBody != null && responseBody.Length > 0)
            {
                // Handle Compression Encoding
                var bytes = MTConnectHttpResponse.HandleContentEncoding(contentEncoding, responseBody);

                // Process MTConnectDevices Document
                var document = Formatters.ResponseDocumentFormatter.CreateStreamsResponseDocument(_documentFormat, bytes).Document;
                if (document != null)
                {
                    DocumentReceived?.Invoke(this, document);
                }
                else
                {
                    // Process MTConnectError Document (if MTConnectStreams fails)
                    var errorDocument = Formatters.ResponseDocumentFormatter.CreateErrorResponseDocument(_documentFormat, bytes).Document;
                    if (errorDocument != null) ErrorReceived?.Invoke(this, errorDocument);
                }
            }
        }

    }
}
