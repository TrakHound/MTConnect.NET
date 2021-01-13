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
        private CancellationTokenSource stop;

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
            stop = new CancellationTokenSource();

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
                        string doc = null;
                        var xmlFound = false;

                        while (!stop.IsCancellationRequested && !reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (!string.IsNullOrEmpty(line))
                            {
                                // Test if Previous Line was beginning of XML Document
                                if (IsXml(prevLine))
                                {
                                    xmlFound = true;
                                    doc = prevLine;
                                }

                                // Test if Current Line is a HTML Header
                                if (IsHeader(line)) xmlFound = false;

                                // Build XML Document
                                if (xmlFound) doc += line;

                                // Test if End of XML Document
                                if (IsEnd(line) && !string.IsNullOrEmpty(doc))
                                {
                                    XmlReceived?.Invoke(doc);
                                    doc = null;
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
                catch (TaskCanceledException ex)
                {
                    ConnectionError?.Invoke(ex);
                }
            }

            Stopped?.Invoke();
        }

        public void Stop()
        {
            if (stop != null) stop.Cancel();
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

        private bool IsEnd(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string tag = "</" + BodyNode + ">";
                return s.EndsWith(tag);
            }

            return false;
        }

        private bool IsHeader(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string tag = "--";
                return s.StartsWith(tag);
            }

            return false;
        }
    }
}
