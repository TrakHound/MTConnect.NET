// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Threading;

namespace MTConnect.Client
{
    public class MTConnectClient
    {
        public MTConnectClient()
        {
            Init();
        }

        public MTConnectClient(string baseUrl)
        {
            Init();
            BaseUrl = baseUrl;
        }

        private void Init()
        {
            Interval = 1000;
            MaximumSampleCount = 2000;
        }

        public string BaseUrl { get; set; }

        public string DeviceName { get; set; }

        public int Interval { get; set; }

        public long MaximumSampleCount { get; set; }

        public event MTConnectDevicesHandler ProbeReceived;
        public event MTConnectStreamsHandler CurrentReceived;
        public event MTConnectStreamsHandler SampleReceived;
        public event MTConnectErrorHandler MTConnectError;

        public event StreamStatusHandler Started;
        public event StreamStatusHandler Stopped;

        private ManualResetEvent stop;

        public void Start()
        {
            Started?.Invoke();

            stop = new ManualResetEvent(false);

            var thread = new Thread(new ThreadStart(Worker));
            thread.Start();
        }

        public void Stop()
        {
            if (stop != null) stop.Set();
        }

        private void Worker()
        {
            long instanceId = -1;
            bool first = true;

            do
            {
                var probe = new Probe(BaseUrl, DeviceName);
                probe.MTConnectError += MTConnectErrorRecieved;
                var probeDoc = probe.Execute();
                if (probeDoc != null)
                {
                    // Raise ProbeReceived Event
                    ProbeReceived?.Invoke(probeDoc);

                    do
                    {
                        var current = new Current(BaseUrl, DeviceName);
                        current.MTConnectError += MTConnectErrorRecieved;
                        var currentDoc = current.Execute();
                        if (currentDoc != null)
                        {
                            // Raise CurrentReceived Event
                            CurrentReceived?.Invoke(currentDoc);

                            // Check if Agent InstanceID has changed (Agent has been reset)
                            if (!first && instanceId != currentDoc.Header.InstanceId)
                            {
                                instanceId = currentDoc.Header.InstanceId;
                                break;
                            }                         
                            first = false;

                            // Get the Start Sequence
                            long from = currentDoc.Header.NextSequence;

                            // Create the Url to use for the Sample Stream
                            string url = CreateSampleUrl(BaseUrl, DeviceName, Interval, from, MaximumSampleCount);

                            // Create and Start the Sample Stream
                            var stream = new Stream(url);
                            stream.DocumentRecieved += ProcessSampleResponse;
                            stream.Start();
                        }
                    } while (!stop.WaitOne(5000, true));
                }
            } while (!stop.WaitOne(5000, true));

            Stopped?.Invoke();
        }

        private void ProcessSampleResponse(string xml)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                // Process MTConnectStreams Document
                var doc = v13.MTConnectStreams.Document.Create(xml);
                if (doc != null)
                {
                    SampleReceived?.Invoke(doc);
                }
                else
                {
                    // Process MTConnectError Document (if MTConnectDevices fails)
                    var errorDoc = v13.MTConnectError.Document.Create(xml);
                    if (errorDoc != null) MTConnectError?.Invoke(errorDoc);
                }
            }
        }

        private void MTConnectErrorRecieved(v13.MTConnectError.Document errorDocument)
        {
            MTConnectError?.Invoke(errorDocument);
        }

        private static string CreateSampleUrl(string baseUrl, string deviceName, int interval, long from , long count)
        {
            var uri = new Uri(baseUrl);
            uri = new Uri(uri, "sample");
            if (!string.IsNullOrEmpty(deviceName)) uri = new Uri(uri, deviceName);
            var format = "{0}?from={1}&count={2}&interval={3}";

            return string.Format(format, uri, from, count, interval);
        }
    }
}
