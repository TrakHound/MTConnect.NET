// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.Xml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;

namespace MTConnect.Formatters.Xml
{
    /// <summary>
    /// <see cref="IPathFormatter"/> that resolves an MTConnect <c>path</c>
    /// request parameter against the XML representation of a device, returning
    /// the ids of the data items selected by the supplied XPath expression.
    /// </summary>
    public class XmlPathFormatter : IPathFormatter
    {
        private readonly Dictionary<string, byte[]> _documents = new Dictionary<string, byte[]>();
        private readonly object _lock = new object();

        /// <summary>
        /// The formatter identifier (<c>XML</c>) used to select this path
        /// formatter for XML requests.
        /// </summary>
        public string Id => "XML";


        private byte[] GetDocumentBytes(IDevice device)
        {
            if (device != null)
            {
                byte[] bytes;
                lock (_lock) _documents.TryGetValue(device.Id, out bytes);
                if (bytes == null)
                {
                    try
                    {
                        using (var stream = new MemoryStream())
                        {
                            // Use XmlWriter to write XML to stream
                            var xmlWriter = XmlWriter.Create(stream, XmlFunctions.XmlWriterSettings);

                            xmlWriter.WriteStartDocument();
                            XmlDevice.WriteXml(xmlWriter, device);
                            xmlWriter.WriteEndDocument();
                            xmlWriter.Flush();

                            // Return byte[]
                            bytes = stream.ToArray();
                            if (bytes != null && bytes.Length > 0)
                            {
                                lock (_lock) _documents.Add(device.Id, bytes);
                            }
                        }
                    }
                    catch { }
                }

                return bytes;
            }

            return null;
        }

        /// <summary>
        /// Resolves the given XPath <paramref name="path"/> against the XML
        /// rendering of each device in the document and returns the ids of the
        /// matching data items. When a matched node is a component, the ids of
        /// all of its data items are included. Unmatched or malformed
        /// expressions yield an empty result.
        /// </summary>
        public IEnumerable<string> GetDataItemIds(IDevicesResponseDocument devicesDocument, string path)
        {
            var dataItemIds = new List<string>();

            if (devicesDocument != null && !devicesDocument.Devices.IsNullOrEmpty())
            {
                foreach (var device in devicesDocument.Devices)
                {
                    // Get List of all Components
                    var components = device.GetComponents();

                    // Get List of all DataItems
                    var dataItems = device.GetDataItems();

                    if (!dataItems.IsNullOrEmpty())
                    {
                        var bytes = GetDocumentBytes(device);
                        if (bytes != null)
                        {
                            try
                            {
                                using (var stream = new MemoryStream(bytes))
                                {
                                    using (var xmlReader = new XmlTextReader(stream))
                                    {
                                        // Create an XPathDocument using the DevicesDocument XML
                                        var xPathDocument = new XPathDocument(xmlReader);

                                        // Create an XPathNavigator
                                        var xPathNavigator = xPathDocument.CreateNavigator();

                                        // Select the nodes matching the XPath string
                                        var nodes = xPathNavigator.Select(path);
                                        if (nodes != null)
                                        {
                                            foreach (XPathNavigator node in nodes)
                                            {
                                                // Get the ID attribute from the Node
                                                var idAttribute = node.GetAttribute("id", "");

                                                if (node.Name == "DataItem")
                                                {
                                                    // Get ID attribute
                                                    if (idAttribute != null)
                                                    {
                                                        // Get DataItem based on ID
                                                        var dataItem = dataItems.FirstOrDefault(o => o.Id == idAttribute);
                                                        if (dataItem != null)
                                                        {
                                                            dataItemIds.Add(dataItem.Id);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    // Get ID attribute
                                                    if (idAttribute != null)
                                                    {
                                                        // Get DataItem based on ID
                                                        var component = components.FirstOrDefault(o => o.Id == idAttribute);
                                                        if (component != null)
                                                        {
                                                            var componentDataItems = component.GetDataItems();
                                                            if (!componentDataItems.IsNullOrEmpty())
                                                            {
                                                                foreach (var dataItem in componentDataItems)
                                                                {
                                                                    dataItemIds.Add(dataItem.Id);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
            }

            return dataItemIds;
        }
    }
}