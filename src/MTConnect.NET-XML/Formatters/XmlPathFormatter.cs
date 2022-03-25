// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Xml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;

namespace MTConnect.Formatters
{
    public class XmlPathFormatter : IPathFormatter
    {
        public string Id => "XML";


        public IEnumerable<string> GetDataItemIds(IDevicesResponseDocument devicesDocument, string path)
        {
            var dataItemIds = new List<string>();

            if (devicesDocument != null)
            {
                // Get List of all Components
                var components = devicesDocument.GetComponents();

                // Get List of all DataItems
                var dataItems = devicesDocument.GetDataItems();

                if (!dataItems.IsNullOrEmpty())
                {
                    // Convert Document to XML
                    var xml = XmlDevicesResponseDocument.ToXml(devicesDocument, null, false);
                    if (!string.IsNullOrEmpty(xml))
                    {
                        // Clear the namespaces from the document (this may be needed but a better implementation of namespaces will be needed)
                        xml = Namespaces.Clear(xml);

                        try
                        {
                            var bytes = System.Text.Encoding.UTF8.GetBytes(xml);
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

            return dataItemIds;
        }
    }
}
