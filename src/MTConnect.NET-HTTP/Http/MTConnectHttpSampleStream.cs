// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Streams.Output;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Http
{
    public class MTConnectHttpSampleStream
    {
        private const int _minInterval = 1; // 1 millisecond minimum interval


        private readonly string _id = StringFunctions.RandomString(10);
        private readonly string _boundary = UnixDateTime.Now.ToString().ToMD5Hash();
        private readonly IMTConnectAgent _mtconnectAgent;
        private readonly string _deviceKey;
        private readonly string _path;
        private readonly long _from;
        private readonly int _count;
        private readonly int _interval;
        private readonly int _heartbeat;
        private readonly string _documentFormat;
        private readonly IEnumerable<KeyValuePair<string, string>> _formatOptions;
        private readonly StringBuilder _multipartBuilder = new StringBuilder();

        private CancellationTokenSource _stop;
        private bool _isConnected;


        public string Id => _id;

        public string Boundary => _boundary;

        public bool IsConnected => _isConnected;

        public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }

        public EventHandler<string> StreamStarted { get; set; }

        public EventHandler<string> StreamStopped { get; set; }

        public EventHandler<Exception> StreamException { get; set; }

        public EventHandler<MTConnectHttpStreamArgs> DocumentReceived { get; set; }

        public EventHandler<MTConnectHttpStreamArgs> HeartbeatReceived { get; set; }



        public MTConnectHttpSampleStream(
            IMTConnectAgent mtconnectAgent,
            string deviceKey,
            string path = null,
            long from = -1,
            int count = 0,
            int interval = 500,
            int heartbeat = 10000,
            string documentFormat = DocumentFormat.XML,
            IEnumerable<KeyValuePair<string, string>> formatOptions = null
            )
        {
            _mtconnectAgent = mtconnectAgent;
            _deviceKey = deviceKey;
            _path = path;
            _from = from;
            _count = count;
            _interval = Math.Max(_minInterval, interval);
            _heartbeat = heartbeat * 10000; // Convert from ms to Ticks
            _documentFormat = documentFormat;
            _formatOptions = formatOptions;
        }


        public void Start(CancellationToken cancellationToken)
        {
            _stop = new CancellationTokenSource();
            cancellationToken.Register(() => { Stop(); });

            _= Task.Run(async () => await Worker(_stop.Token));

            _isConnected = true;
        }

        public void Stop()
        {
            if (_stop != null) _stop.Cancel();
            _isConnected = false;
        }

        public async Task Run()
        {
            _stop = new CancellationTokenSource();

            await Worker(_stop.Token);

            _isConnected = true;
        }


        private async Task Worker(CancellationToken cancellationToken)
        {
            if (_mtconnectAgent != null)
            {
                StreamStarted?.Invoke(this, _id);

                // Set Mime Type based documentFormat specified
                var mimeType = MimeTypes.XML;
                if (_documentFormat == DocumentFormat.JSON) mimeType = MimeTypes.JSON;

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

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        stpw.Restart();

                        // Read the MTConnectStreams document from the IMTConnectAgent
                        if (!string.IsNullOrEmpty(_deviceKey))
                        {
                            document = _mtconnectAgent.GetDeviceStream(_deviceKey, nextSequence, long.MaxValue, _count);
                        }
                        else
                        {
                            document = _mtconnectAgent.GetDeviceStreams(nextSequence, long.MaxValue, _count);
                        }

                        if (document != null)
                        {
                            // Get List of all DataItems in MTConnectStreams Document
                            //dataItems = document.GetObservations();

                            now = UnixDateTime.Now;
                            chunk = CreateChunk(document, _documentFormat, _formatOptions);

                            // Create the HTTP Multipart Chunk (with boundary)
                            multipartChunk = CreateMultipartChunk(chunk, mimeType, _boundary);

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
                        await Task.Delay(_interval, cancellationToken);
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


        private static byte[] CreateChunk(IStreamsResponseOutputDocument document, string documentFormat, IEnumerable<KeyValuePair<string, string>> formatOptions)
        {
            if (document != null)
            {
                // Format Response Document using the specified DocumentFormatter
                var result = Formatters.ResponseDocumentFormatter.Format(documentFormat, ref document, formatOptions);
                if (result.Success)
                {
                    // If successful then return the bytes
                    return result.Content;
                }
            }

            return null;
        }

        private byte[] CreateMultipartChunk(byte[] body, string mimeType, string boundary)
        {
            _multipartBuilder.Clear();

            // Add Boundary Line
            _multipartBuilder.Append("--");
            _multipartBuilder.Append(boundary);
            _multipartBuilder.Append("\r\n");

            // Add Content Type Line
            _multipartBuilder.Append("Content-type: ");
            _multipartBuilder.Append(mimeType);
            _multipartBuilder.Append("\r\n");

            // Add Content Length Line
            _multipartBuilder.Append("Content-length: ");
            _multipartBuilder.Append(body.Length);
            _multipartBuilder.Append("\r\n\r\n");

            // Get Bytes from StringBuilder
            char[] a = new char[_multipartBuilder.Length];
            _multipartBuilder.CopyTo(0, a, 0, _multipartBuilder.Length);

            // Convert StringBuilder result to UTF8 Bytes
            var headerBytes = Encoding.UTF8.GetBytes(a);

            // Create Array to return that is the size of the Header + body
            byte[] bytes = new byte[headerBytes.Length + body.Length];

            // Add Header Bytes
            Array.Copy(headerBytes, 0, bytes, 0, headerBytes.Length);

            // Add Body bytes
            Array.Copy(body, 0, bytes, headerBytes.Length, body.Length);

            return bytes;
        }
    }
}
