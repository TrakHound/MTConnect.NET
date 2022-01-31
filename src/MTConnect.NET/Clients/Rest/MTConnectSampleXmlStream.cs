// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients.Rest
{
    class MTConnectSampleXmlStream
    {
        private const string BodyNode = "MTConnectStreams";
        private const int DefaultTimeout = 20000;

        private static readonly HttpClient _httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromMilliseconds(DefaultTimeout)
        };

        private CancellationTokenSource _stop;


        public MTConnectSampleXmlStream(string url)
        {
            Id = Guid.NewGuid().ToString();
            Url = url;
            Timeout = DefaultTimeout;

            _httpClient.DefaultRequestHeaders.Add("Accept", "application/xml");
        }


        public string Id { get; set; }

        public string Url { get; set; }

        public int Timeout { get; set; }


        public EventHandler<Exception> OnInternalError { get; set; }

        public EventHandler<Exception> OnConnectionError { get; set; }

        public EventHandler Starting { get; set; }
        public EventHandler Started { get; set; }

        public EventHandler Stopping { get; set; }
        public EventHandler Stopped { get; set; }

        public EventHandler<Streams.StreamsDocument> DocumentReceived { get; set; }

        public EventHandler<Errors.ErrorDocument> ErrorReceived { get; set; }

        class XmlStreamString
        {
            public int Start { get; set; }

            public int End { get; set; }

            public string Value { get; set; }

            public XmlStreamString(int start, int end, string value)
            {
                Start = start;
                End = end;
                Value = value;
            }
        }


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
                responseTimer.AutoReset = false;
                responseTimer.Interval = Timeout;
                responseTimer.Elapsed += (o, e) =>
                {
                    stop.Cancel();
                    OnConnectionError?.Invoke(this, new TimeoutException($"HTTP Stream Timeout Exceeded ({Timeout})"));
                };
                responseTimer.Start();

                try
                {
                    stop.Token.ThrowIfCancellationRequested();

                    // Raise Started Event
                    Started?.Invoke(this, new EventArgs());

                    // Get the HTTP Stream 
#if NET5_0_OR_GREATER
                    using (var stream = await _httpClient.GetStreamAsync(Url, stop.Token))
#else
                    using (var stream = await _httpClient.GetStreamAsync(Url))
#endif
                    {
                        // Create Reader
                        using (var reader = new StreamReader(stream))
                        {
                            // Set Read Buffer
                            //var memory = new Memory<char>(new char[1048576]); // 1 MB
                            var memory = new char[1048576]; // 1 MB

                            // Read First Chunk from the Stream
                            int i = await reader.ReadAsync(memory, 0, memory.Length);

                            responseTimer.Stop();
                            responseTimer.Start();

                            string xml = "";

                            while (i > 0 && !stop.Token.IsCancellationRequested)
                            {
                                if (memory.Length > 0)
                                {
                                    // Get string from buffer
                                    string s = new string(memory, 0, i);

                                    Console.WriteLine(s);

                                    // Add buffer to XML
                                    xml += s;

                                    // Continue to read XML documents in the string until no more are found
                                    var xmlString = GetXml(xml);
                                    while (xmlString != null)
                                    {
                                        // Return the XML document (if found)
                                        //XmlReceived?.Invoke(this, xmlString.Value);
                                        ProcessDocument(xmlString.Value);

                                        // Remove the read XML document from the string and reprocess to look for more
                                        xml = xml.Substring(xmlString.End);
                                        xmlString = GetXml(xml);
                                    }
                                }

                                // Read Next Chunk
#if NET5_0_OR_GREATER
                                i = await reader.ReadAsync(memory, stop.Token);
#else
                                i = await reader.ReadAsync(memory, 0, memory.Length);
#endif

                                //i = await reader.ReadAsync(memory, stop.Token);

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

        private XmlStreamString GetXml(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var startNode = "<?xml ";
                var endNode = $"</{BodyNode}>";

                var i = s.IndexOf(startNode, StringComparison.Ordinal);
                if (i >= 0)
                {
                    var j = s.IndexOf(endNode, i, StringComparison.Ordinal);
                    if (j > 0)
                    {
                        j += endNode.Length;
                        var v = s.Substring(i, j - i);
                        return new XmlStreamString(i, j, v);
                    }
                }

                endNode = $"</MTConnectError>";

                i = s.IndexOf(startNode, StringComparison.Ordinal);
                if (i >= 0)
                {
                    var j = s.IndexOf(endNode, i, StringComparison.Ordinal);
                    if (j > 0)
                    {
                        j += endNode.Length;
                        var v = s.Substring(i, j - i);
                        return new XmlStreamString(i, j, v);
                    }
                }
            }

            return null;
        }

        private void ProcessDocument(string xml)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                // Process MTConnectStreams Document
                var doc = Streams.StreamsDocument.FromXml(xml);
                if (doc != null)
                {
                    DocumentReceived?.Invoke(this, doc);
                }
                else
                {
                    // Process MTConnectError Document (if MTConnectStreams fails)
                    var errorDoc = Errors.ErrorDocument.Create(xml);
                    if (errorDoc != null) ErrorReceived?.Invoke(this, errorDoc);
                }
            }
        }
    }
}
