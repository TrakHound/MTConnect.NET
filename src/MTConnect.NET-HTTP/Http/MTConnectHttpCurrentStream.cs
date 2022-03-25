// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Observations;
using MTConnect.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Http
{
    public class MTConnectHttpCurrentStream
    {
        private readonly string _id = StringFunctions.RandomString(10);
        private readonly string _boundary = UnixDateTime.Now.ToString().ToMD5Hash();
        private readonly IMTConnectAgent _mtconnectAgent;
        private readonly string _deviceName;
        private readonly string _path;
        private readonly int _interval;
        private readonly int _heartbeat;
        private readonly string _documentFormat;
        private readonly IEnumerable<KeyValuePair<string, string>> _formatOptions;

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
            string deviceName,
            string path = null,
            int interval = 500,
            int heartbeat = 1000,
            string documentFormat = DocumentFormat.XML,
            IEnumerable<KeyValuePair<string, string>> formatOptions = null
            )
        {
            _mtconnectAgent = mtconnectAgent;
            _deviceName = deviceName;
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
                    IStreamsResponseDocument document = null;
                    IEnumerable<IObservation> dataItems = null;

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        stpw.Restart();

                        // Read the MTConnectStreams document from the IMTConnectAgent
                        if (!string.IsNullOrEmpty(_deviceName))
                        {
                            document = await _mtconnectAgent.GetDeviceStreamAsync(_deviceName);
                        }
                        else
                        {
                            document = await _mtconnectAgent.GetDeviceStreamsAsync();
                        }

                        if (document != null)
                        {
                            // Get List of all DataItems in MTConnectStreams Document
                            dataItems = document.GetObservations();

                            now = UnixDateTime.Now;
                            var chunk = CreateChunk(document, _documentFormat, _formatOptions);

                            // Create the HTTP Multipart Chunk (with boundary)
                            var multipartChunk = CreateMultipartChunk(chunk, mimeType, _boundary);

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

                        // Pause the stream for the specified Interval
                        await Task.Delay(_interval);

                        document = null;
                    }
                }
                catch (Exception ex)
                {
                    StreamException?.Invoke(this, ex);
                }

                StreamStopped?.Invoke(this, _id);
            }
        }


        private static string CreateMultipartChunk(string body, string mimeType, string boundary)
        {
            var chunk = $"--{boundary}\r\n";
            chunk += $"Content-type: {mimeType}\r\n";
            chunk += $"Content-length: {body.Length}\r\n\r\n";

            chunk += body;
            chunk += "\r\n\r\n";

            return chunk;
        }


        private static string CreateChunk(IStreamsResponseDocument document, string documentFormat, IEnumerable<KeyValuePair<string, string>> formatOptions)
        {
            if (document != null)
            {
                var result = Formatters.ResponseDocumentFormatter.Format(documentFormat, document, formatOptions);
                if (result.Success)
                {
                    return result.Content;
                }
            }

            return "";
        }
    }
}
