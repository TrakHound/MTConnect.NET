// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Application.Streams;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace MTConnect.Client
{
    public class xStream
    {
        public delegate void StreamHandler();
        public delegate void DocumentHandler(string xml);
        public delegate void ConnectionErrorHandler(Exception ex);
        //public delegate void MTConnectErrorHandler(MTConnect.v13.Error error);

        public event DocumentHandler DocumentRecieved;
        public event StreamHandler Started;
        public event StreamHandler Stopped;


        private ManualResetEvent stop;

        public void Start(string baseUrl)
        {
            Start(baseUrl, 1000);
        }

        public void Start(string baseUrl, int interval)
        {
            stop = new ManualResetEvent(false);

            // Raise Started Event
            Started?.Invoke();

            // Get MTConnect Current to find first available sequence
            var current = Requests.GetCurrent(baseUrl);
            if (current != null)
            {
                string url = CreateSampleUrl(baseUrl, interval, current);

                var request = (HttpWebRequest)WebRequest.Create(url);
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
                {
                    // Set Read Buffer
                    int bufferSize = 1048576;
                    var buffer = new char[bufferSize];
                    int i = reader.Read(buffer, 0, bufferSize);

                    string xml = "";
                    long lastSequence = current.Header.LastSequence;

                    while (i > 0 && !stop.WaitOne(0, true))
                    {
                        // Get string from buffer
                        var s = new string(buffer, 0, i);

                        // Find Beginning of XML
                        int b = s.IndexOf("<?xml");
                        if (b >= 0) s = s.Substring(b);

                        // Add buffer to XML
                        xml += s;

                        // Test if End of XML
                        if (HasClosingTag(xml))
                        {
                            // Process the XML Document
                            ProcessDocument(xml);

                            // Reset XML
                            xml = "";
                        }

                        // Read Next Chunk
                        i = reader.Read(buffer, 0, bufferSize);
                    }
                }
            }

            Stop();
        }

        public void Stop()
        {
            if (stop != null) stop.Set();

            Stopped?.Invoke();
        }

        private static string CreateSampleUrl(string baseUrl, int interval, Response current)
        {
            var uri = new Uri(baseUrl);
            uri = new Uri(uri, "sample");
            var from = current.Header.LastSequence - 200;
            var to = current.Header.LastSequence;
            var format = "{0}?from={1}&to={2}&interval={3}";

            return string.Format(format, uri, from, to, interval);
        }

        private static bool HasClosingTag(string xml)
        {
            int e = xml.IndexOf("</MTConnectStreams>");
            if (e < 0) e = xml.IndexOf("</MTConnectAssets>");
            if (e < 0) e = xml.IndexOf("</MTConnectError>");
            return e >= 0;
        }

        private void ProcessDocument(string xml)
        {



            // Raise Document Recieved Event and pass XML
            DocumentRecieved?.Invoke(xml);
        }
        
    }
}
