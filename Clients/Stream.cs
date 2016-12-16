// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
                    //int charSize = ASCIIEncoding.UTF8.GetByteCount("a");
                    //int bufferSize = 1000000;

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

                        while (i > 0 && !stop.WaitOne(0, true))
                        {
                            // Get string from buffer
                            var s = new string(buffer, 0, i);

                            // Find Beginning of XML
                            int b = s.IndexOf("<?xml");
                            if (b >= 0) s = s.Substring(b);

                            if (b >= 0 && s.IndexOf("<?xml", b) >= 0)
                            {
                                Console.WriteLine("Two open tags");
                            }

                            // Add buffer to XML
                            xml += s;

                            // Test if End of XML
                            if (HasClosingTag(xml))
                            {
                                // Raise XML Received Event and pass XML
                                XmlReceived?.Invoke(xml);

                                // Reset XML
                                xml = "";
                            }

                            int bytes = Encoding.UTF8.GetByteCount(xml);
                            if (bytes >= bufferSize)
                            {
                                Console.WriteLine("Xml bigger than BufferSize");
                            }

                            

                            // Clear Buffer
                            buffer = new char[bufferSize];

                            // Read Next Chunk
                            i = reader.Read(buffer, 0, bufferSize);
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
