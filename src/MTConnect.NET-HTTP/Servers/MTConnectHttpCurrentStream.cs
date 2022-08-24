// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Http;
using MTConnect.Observations.Output;
using MTConnect.Streams.Output;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Servers.Http
{
    internal class MTConnectHttpCurrentStream
    {
        private readonly string _id = StringFunctions.RandomString(10);
        private readonly string _boundary = UnixDateTime.Now.ToString().ToMD5Hash();
        private readonly IMTConnectAgent _mtconnectAgent;
        private readonly string _deviceKey;
        private readonly string _path;
        private readonly int _interval;
        private readonly int _heartbeat;
        private readonly string _documentFormat;
        private readonly IEnumerable<KeyValuePair<string, string>> _formatOptions;
        private readonly StringBuilder _multipartBuilder = new StringBuilder();

        private CancellationTokenSource _stop;


        public string Id => _id;

        public string Boundary => _boundary;

        public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }

        public EventHandler<string> StreamStarted { get; set; }

        public EventHandler<string> StreamStopped { get; set; }

        public EventHandler<Exception> StreamException { get; set; }

        public EventHandler<MTConnectHttpStreamArgs> DocumentReceived { get; set; }

        public EventHandler<MTConnectHttpStreamArgs> HeartbeatReceived { get; set; }



        public MTConnectHttpCurrentStream(
            IMTConnectAgent mtconnectAgent,
            string deviceKey,
            string path = null,
            int interval = 500,
            int heartbeat = 1000,
            string documentFormat = DocumentFormat.XML,
            IEnumerable<KeyValuePair<string, string>> formatOptions = null
            )
        {
            _mtconnectAgent = mtconnectAgent;
            _deviceKey = deviceKey;
            _path = path;
            _interval = interval;
            _heartbeat = heartbeat;
            _documentFormat = documentFormat;
            _formatOptions = formatOptions;
        }

        public void Start(CancellationToken cancellationToken)
        {
            _stop = new CancellationTokenSource();
            cancellationToken.Register(() => { Stop(); });

            _ = Task.Run(async () => await Worker(_stop.Token));
        }

        public void Stop()
        {
            if (_stop != null) _stop.Cancel();
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

                    long lastDocumentSent = 0;
                    long lastHeartbeatSent = 0;
                    long now = UnixDateTime.Now;
                    IStreamsResponseOutputDocument document = null;
                    IEnumerable<IObservationOutput> dataItems = null;
                    byte[] chunk = null;
                    byte[] multipartChunk = null;

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        stpw.Restart();

                        // Read the MTConnectStreams document from the IMTConnectAgent
                        if (!string.IsNullOrEmpty(_deviceKey))
                        {
                            document = _mtconnectAgent.GetDeviceStream(_deviceKey);
                        }
                        else
                        {
                            document = _mtconnectAgent.GetDeviceStreams();
                        }

                        if (document != null)
                        {
                            // Get List of all DataItems in MTConnectStreams Document
                            dataItems = document.GetObservations();

                            now = UnixDateTime.Now;
                            chunk = CreateChunk(document, _documentFormat, _formatOptions);

                            // Create the HTTP Multipart Chunk (with boundary)
                            multipartChunk = CreateMultipartChunk(chunk, mimeType, _boundary);

                            if (document.Streams.IsNullOrEmpty() || dataItems.IsNullOrEmpty())
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
                            }
                        }

                        document = null;
                        dataItems = null;
                        chunk = null;
                        multipartChunk = null;

                        // Pause the stream for the specified Interval
                        await Task.Delay(_interval, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    StreamException?.Invoke(this, ex);
                }

                StreamStopped?.Invoke(this, _id);
            }
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

            // Add Body
            if (body != null && body.Length > 0) _multipartBuilder.Append(body);
            _multipartBuilder.Append("\r\n\r\n");

            char[] a = new char[_multipartBuilder.Length];
            _multipartBuilder.CopyTo(0, a, 0, _multipartBuilder.Length);

            _multipartBuilder.Clear();

            return Encoding.UTF8.GetBytes(a);
        }

        private static byte[] CreateChunk(IStreamsResponseOutputDocument document, string documentFormat, IEnumerable<KeyValuePair<string, string>> formatOptions)
        {
            if (document != null)
            {
                var result = Formatters.ResponseDocumentFormatter.Format(documentFormat, ref document, formatOptions);
                if (result.Success)
                {
                    return result.Content;
                }
            }

            return null;
        }
    }
}
