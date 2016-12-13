// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace MTConnect.Client
{
    public class Stream
    {
        public Stream(string url)
        {
            Url = url;
        }

        public string Url { get; set; }

        public event DocumentHandler DocumentRecieved;
        public event StreamStatusHandler Started;
        public event StreamStatusHandler Stopped;

        private ManualResetEvent stop;

        public void Start()
        {
            if (!string.IsNullOrEmpty(Url))
            {
                stop = new ManualResetEvent(false);

                // Raise Started Event
                Started?.Invoke();

                var request = (HttpWebRequest)WebRequest.Create(Url);
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
                {
                    // Set Read Buffer
                    int bufferSize = 1048576;
                    var buffer = new char[bufferSize];
                    int i = reader.Read(buffer, 0, bufferSize);

                    string xml = "";
                    //long lastSequence = current.Header.LastSequence;

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
                            // Raise Document Recieved Event and pass XML
                            DocumentRecieved?.Invoke(xml);

                            // Reset XML
                            xml = "";
                        }

                        // Read Next Chunk
                        i = reader.Read(buffer, 0, bufferSize);
                    }
                }

                Stopped?.Invoke();
            }
        }

        public void Stop()
        {
            if (stop != null) stop.Set();
        }

        private static bool HasClosingTag(string xml)
        {
            int e = xml.IndexOf("</MTConnectDevices>");
            if (e < 0) e = xml.IndexOf("</MTConnectStreams>");
            if (e < 0) e = xml.IndexOf("</MTConnectAssets>");
            if (e < 0) e = xml.IndexOf("</MTConnectError>");
            return e >= 0;
        }        
    }
}
