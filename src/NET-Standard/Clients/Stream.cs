// Copyright (c) 2020 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients
{
    class Stream
    {
        private ManualResetEvent stop;

        public string Url { get; set; }

        public string BodyNode { get; set; }

        public int Timeout { get; set; }


        public event ConnectionErrorHandler ConnectionError;
        public event XmlHandler XmlReceived;
        public event StreamStatusHandler Started;
        public event StreamStatusHandler Stopped;


        public Stream(string url, string bodyNode)
        {
            Url = url;
            BodyNode = bodyNode;
            Timeout = 5000;
        }

        public async Task Run()
        {
            stop = new ManualResetEvent(false);

            if (!string.IsNullOrEmpty(Url))
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromMilliseconds(Timeout);
                client.DefaultRequestHeaders.Add("Accept", "application/xml");

                try
                {
                    // Raise Started Event
                    Started?.Invoke();

                    var stream = await client.GetStreamAsync(Url);
                    using (var reader = new StreamReader(stream))
                    {
                        string prevLine = null;

                        while (!stop.WaitOne(0, true) && !reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (!string.IsNullOrEmpty(line))
                            {
                                if (IsXml(line) && IsXml(prevLine))
                                {
                                    var xml = $"{prevLine}{line}";
                                    XmlReceived?.Invoke(xml);
                                }

                                prevLine = line;
                            }
                        }
                    }
                }
                catch (HttpRequestException e)
                {
                    ConnectionError?.Invoke(e);
                }
            }

            Stopped?.Invoke();
        }

        public void Stop()
        {
            if (stop != null) stop.Set();
        }

        private bool IsXml(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string tag = "<?xml";
                return s.StartsWith(tag);
            }

            return false;
        }
    }
}
