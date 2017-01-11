// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace MTConnect.Clients
{
    public class Stream
    {
        public Stream(string url)
        {
            Url = url;
        }

        public string Url { get; set; }

        public event ConnectionErrorHandler ConnectionError;
        public event XmlHandler XmlError;
        public event XmlHandler XmlReceived;
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

                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(Url);
                    using (var response = (HttpWebResponse)request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
                    {
                        // Set Read Buffer
                        var buffer = new char[1048576]; // 1 MB
                        int i = reader.Read(buffer, 0, buffer.Length);

                        string xml = "";

                        while (i > 0 && !stop.WaitOne(10, true))
                        {
                            // Get string from buffer
                            var s = new string(buffer, 0, i);

                            // Find Beginning of XML (exclude HTTP header)
                            int b = s.IndexOf("<?xml ");
                            if (b >= 0) s = s.Substring(b);

                            // Add buffer to XML
                            xml += s;

                            // Find Closing Tag
                            int c = FindClosingTag(xml);

                            // Test if End of XML
                            if (c >= 0) 
                            {
                                b = xml.LastIndexOf("<?xml ");
                                if (c > b)
                                {
                                    // Raise XML Received Event and pass XML
                                    XmlReceived?.Invoke(xml.Substring(b, c - b));

                                    // Remove MTConnect document from xml
                                    xml = xml.Remove(b, c - b).Trim();
                                }

                                if (!string.IsNullOrEmpty(xml))
                                {
                                    // Raise XML Error and break from while
                                    XmlError?.Invoke(xml);
                                    break;
                                }

                                // Reset XML
                                xml = "";
                            }

                            // Clear Buffer
                            Array.Clear(buffer, 0, buffer.Length);

                            // Read Next Chunk
                            i = reader.Read(buffer, 0, buffer.Length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ConnectionError?.Invoke(ex);
                }

                Stopped?.Invoke();
            }
        }

        public void Stop()
        {
            if (stop != null) stop.Set();
        }

        private static int FindClosingTag(string xml)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                string tag = "</MTConnectDevices>";
                int i = xml.IndexOf(tag);
                if (i >= 0) return i + tag.Length;

                tag = "</MTConnectStreams>";
                i = xml.IndexOf(tag);
                if (i >= 0) return i + tag.Length;

                tag = "</MTConnectAssets>";
                i = xml.IndexOf(tag);
                if (i >= 0) return i + tag.Length;

                tag = "</MTConnectError>";
                i = xml.IndexOf(tag);
                if (i >= 0) return i + tag.Length;
            }

            return -1;
        }       
    }
}
