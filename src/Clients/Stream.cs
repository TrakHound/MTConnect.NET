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
        private ManualResetEvent stop;
        private HttpWebResponse response;
        private StreamReader reader;

        public string Url { get; set; }

        public string BodyNode { get; set; }

        public int ConnectionTimeout { get; set; }

        public int IOTimeout { get; set; }

        public event ConnectionErrorHandler ConnectionError;
        public event XmlHandler XmlError;
        public event XmlHandler XmlReceived;
        public event StreamStatusHandler Started;
        public event StreamStatusHandler Stopped;


        public Stream(string url, string bodyNode)
        {
            Url = url;
            BodyNode = bodyNode;
            ConnectionTimeout = 5000;
            IOTimeout = Convert.ToInt32(TimeSpan.FromMinutes(1).TotalMilliseconds);
        }

        public void Run()
        {
            Start();

            stop.WaitOne();

            Stopped?.Invoke();
        }

        public void Start()
        {
            if (!string.IsNullOrEmpty(Url))
            {
                stop = new ManualResetEvent(false);

                // Raise Started Event
                Started?.Invoke();

                ThreadPool.QueueUserWorkItem(new WaitCallback(Worker));
            }
        }

        public void Stop()
        {
            if (stop != null) stop.Set();
            if (reader != null) reader.Close();
            if (response != null) response.Close();
        }

        private void Worker(object obj)
        {
            try
            {
                if (!stop.WaitOne(0, true))
                {
                    var request = (HttpWebRequest)WebRequest.Create(Url);
                    request.Timeout = ConnectionTimeout;
                    request.ReadWriteTimeout = IOTimeout;
                    using (response = (HttpWebResponse)request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    using (reader = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
                    {
                        // Set Read Buffer
                        var buffer = new char[1048576]; // 1 MB
                        int i = reader.Read(buffer, 0, buffer.Length);

                        string xml = "";

                        while (i > 0 && !stop.WaitOne(0, true))
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
            }
            catch (Exception ex)
            {
                ConnectionError?.Invoke(ex);
            }

            Stop();
        }

        private int FindClosingTag(string xml)
        {
            if (!string.IsNullOrEmpty(xml) && !string.IsNullOrEmpty(BodyNode))
            {
                string closingTag = "</" + BodyNode + ">";
                if (xml.Contains(closingTag))
                {
                    int i = xml.IndexOf(closingTag);
                    if (i >= 0) return i + closingTag.Length;
                }

                closingTag = "</MTConnectError>";
                if (xml.Contains(closingTag))
                {
                    int i = xml.IndexOf(closingTag);
                    if (i >= 0) return i + closingTag.Length;
                }
            }

            return -1;
        }    
    }
}
