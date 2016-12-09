// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;


using System;
using System.Collections.Generic;
using System.Xml;
using RestSharp;

using System.Web;
using System.Net;

using MTConnect.Application;
using MTConnect.Application.Streams;
using MTConnect;

namespace MTConnect.Application.Streams
{
    /// <summary>
    /// Object class to return all data associated with Current command results
    /// </summary>
    public class Response : IDisposable
    {
        // Device object with heirarchy of values and xml structure
        public List<DeviceStream> DeviceStreams { get; set; }

        // Header Information
        public Headers.Streams Header { get; set; }

        public void Dispose()
        {
            DeviceStreams.Clear();
            DeviceStreams = null;
        }

        public Response(string xml)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);

                if (doc.DocumentElement != null)
                {
                    // Get Root Element from Xml Document
                    XmlElement root = doc.DocumentElement;

                    // Get Header_Streams object from Root node
                    Header = GetHeader(root);

                    // Get DeviceStream object from Root node
                    DeviceStreams = GetDeviceStreams(root);
                }
            }
            catch (XmlException ex)
            {
                Console.WriteLine("XmlException :: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception :: " + ex.Message);
            }
        }

        private static Headers.Streams GetHeader(XmlElement root)
        {
            XmlNodeList nodes = root.GetElementsByTagName("Header");
            if (nodes != null && nodes.Count > 0)
            {
                return new Headers.Streams(nodes[0]);
            }

            return null;
        }

        private static List<DeviceStream> GetDeviceStreams(XmlElement root)
        {
            var nodes = root.GetElementsByTagName("DeviceStream");
            if (nodes != null)
            {
                if (nodes.Count > 0)
                {
                    var streams = new List<DeviceStream>();

                    foreach (XmlNode node in nodes)
                    {
                        streams.Add(new DeviceStream(node));
                    }

                    return streams;
                }
            }

            return null;
        }
    }
}
