// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTConnect.Clients.Rest
{
    class MTConnectSampleJsonStream
    {
        private const string BodyNode = "MTConnectStreams";
        private const int DefaultTimeout = 20000;

        private static readonly HttpClient _httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromMilliseconds(DefaultTimeout)
        };

        private CancellationTokenSource _stop;


        public MTConnectSampleJsonStream(string url)
        {
            Id = Guid.NewGuid().ToString();
            Url = url;
            Timeout = DefaultTimeout;

            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
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

        //class JsonStreamString
        //{
        //    public int Start { get; set; }

        //    public int End { get; set; }

        //    public string Value { get; set; }

        //    public XmlStreamString(int start, int end, string value)
        //    {
        //        Start = start;
        //        End = end;
        //        Value = value;
        //    }
        //}


        public async Task Start(CancellationToken cancellationToken)
        {
            _stop = new CancellationTokenSource();
            cancellationToken.Register(() => Stop());

            // Raise Starting Event
            Starting?.Invoke(this, new EventArgs());

            _ = Task.Run(() => Run(_stop.Token));
        }

        public async Task Stop()
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
                    using (var stream = await _httpClient.GetStreamAsync(Url))
                    {
                        // Create Reader
                        using (var reader = new StreamReader(stream))
                        {
                            // Set Read Buffer
                            var memory = new Memory<char>(new char[1048576]); // 1 MB

                            // Read First Chunk from the Stream
                            int i = await reader.ReadAsync(memory, 0, memory.Length);

                            responseTimer.Stop();
                            responseTimer.Start();

                            string json = "";

                            while (i > 0 && !stop.Token.IsCancellationRequested)
                            {
                                if (memory.Length > 0)
                                {
                                    // Get string from buffer
                                    string s = memory.Slice(0, i).ToString();

                                    // Add buffer to JSON
                                    json += s;

                                    if (ProcessDocument(json))
                                    {
                                        json = "";
                                    }

                                    // Continue to read JSON documents in the string until no more are found
                                    //var xmlString = GetJson(json);
                                    //while (xmlString != null)
                                    //{
                                    //    // Return the XML document (if found)
                                    //    ProcessDocument(xmlString.Value);

                                    //    // Remove the read XML document from the string and reprocess to look for more
                                    //    json = json.Substring(xmlString.End);
                                    //    xmlString = GetJson(json);
                                    //}
                                }

                                // Read Next Chunk
                                i = await reader.ReadAsync(memory, stop.Token);

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

        //private string GetJson(string s)
        //{
        //    if (!string.IsNullOrEmpty(s))
        //    {
        //        var document = JsonSerializer.Deserialize<Streams.Document>(s);
        //        if (document != null)
        //        {
        //            DocumentReceived.Invoke(this, document);
        //        }
        //        else
        //        {
        //            var error = JsonSerializer.Deserialize<Errors.Document>(s);
        //            if (error != null) ErrorReceived.Invoke(this, error);
        //        }
        //    }

        //    return null;
        //}

        private bool ProcessDocument(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                var document = JsonSerializer.Deserialize<Streams.StreamsDocument>(json);
                if (document != null)
                {
                    DocumentReceived.Invoke(this, document);
                    return true;
                }
                else
                {
                    var error = JsonSerializer.Deserialize<Errors.ErrorDocument>(json);
                    if (error != null)
                    {
                        ErrorReceived.Invoke(this, error);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
