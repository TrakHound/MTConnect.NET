// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MTConnect.Streams.Xml
{
    /// <summary>
    /// Events is a XML container type element. 
    /// Events organizes the Data Entities returned in the MTConnectStreams XML document for those DataItem elements defined with a category attribute of EVENT in the MTConnectDevices document.
    /// </summary>
    public class XmlEventsContainer : IXmlSerializable
    {
        private const string NodePrefixPattern = "(.*):(.*)";
        private const string DataSetSuffix = "DataSet";
        private const string TableSuffix = "Table";

        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(Event));

        private readonly XmlDocument _document;
        private readonly XmlNode _copy;

        /// <summary>
        /// An XML container type element that organizes the data reported in the MTConnectStreams document for DataItem elements defined in the MTConnectDevices document with a category attribute of EVENT.
        /// </summary>
        [XmlIgnore]
        public List<Event> Events { get; set; }


        public XmlEventsContainer()
        {
            // Initialize the Events List
            Events = new List<Event>();

            // Create a dummy XmlDocument to use create dummy nodes
            _document = new XmlDocument();

            // Create a new Node with the name of "DataItem"
            _copy = _document.CreateNode(XmlNodeType.Element, Devices.DataItemCategory.EVENT.ToString().ToPascalCase(), null);
        }


        #region "Xml Serialization"

        public void WriteXml(XmlWriter writer)
        {
            if (!Events.IsNullOrEmpty())
            {
                foreach (var dataItem in Events)
                {
                    if (dataItem.IsDataSet)
                    {
                        WriteDataSetXml(writer, dataItem);
                    }
                    else if (dataItem.IsTable)
                    {
                        WriteTableXml(writer, dataItem);
                    }
                    else
                    {
                        WriteValueXml(writer, dataItem);
                    }
                }
            }
        }

        private static string GetElementName(Event dataItem)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.Type))
            {
                string name;

                var match = Regex.Match(dataItem.Type, NodePrefixPattern);
                if (match.Success)
                {
                    name = match.Groups[2].Value.ToPascalCase();
                }
                else
                {
                    name = dataItem.Type.ToPascalCase();
                }

                // If TimeSeries, then add 'TimeSeries' suffix to Type name
                if (dataItem.IsDataSet) name += DataSetSuffix;
                else if (dataItem.IsTable) name += TableSuffix;

                return name;
            }

            return null;
        }

        private void AddCommonAttributes(Event dataItem, XmlNode node)
        {
            if (node != null)
            {
                XmlAttribute attribute;

                // DataItemId
                if (!string.IsNullOrEmpty(dataItem.DataItemId))
                {
                    attribute = _document.CreateAttribute("dataItemId");
                    attribute.Value = dataItem.DataItemId;
                    node.Attributes.Append(attribute);
                }

                // Timestamp
                attribute = _document.CreateAttribute("timestamp");
                attribute.Value = dataItem.Timestamp.ToString("o");
                node.Attributes.Append(attribute);

                // Name
                if (!string.IsNullOrEmpty(dataItem.Name))
                {
                    attribute = _document.CreateAttribute("name");
                    attribute.Value = dataItem.Name;
                    node.Attributes.Append(attribute);
                }

                // Sequence
                attribute = _document.CreateAttribute("sequence");
                attribute.Value = dataItem.Sequence.ToString();
                node.Attributes.Append(attribute);

                // SubType
                if (!string.IsNullOrEmpty(dataItem.SubType))
                {
                    attribute = _document.CreateAttribute("subType");
                    attribute.Value = dataItem.SubType;
                    node.Attributes.Append(attribute);
                }

                // CompositionId
                if (!string.IsNullOrEmpty(dataItem.CompositionId))
                {
                    attribute = _document.CreateAttribute("compositionId");
                    attribute.Value = dataItem.CompositionId;
                    node.Attributes.Append(attribute);
                }

                // ResetTriggered
                if (dataItem.ResetTriggered != Devices.DataItemResetTrigger.NONE)
                {
                    attribute = _document.CreateAttribute("resetTriggered");
                    attribute.Value = dataItem.ResetTriggered.ToString();
                    node.Attributes.Append(attribute);
                }
            }
        }


        private void WriteValueXml(XmlWriter writer, Event dataItem)
        {
            try
            {
                // Get Element name
                var name = GetElementName(dataItem);
                if (!string.IsNullOrEmpty(name))
                {
                    var node = _document.CreateNode(XmlNodeType.Element, null, name, null);
                    if (node != null)
                    {
                        // Add Common Attributes to Node
                        AddCommonAttributes(dataItem, node);

                        // Set InnerText to the CDATA
                        node.InnerText = dataItem.CDATA?.Trim();

                        // Write Node to XmlWriter
                        writer.WriteNode(new XmlNodeReader(node), false);
                    }
                }
            }
            catch { }
        }

        private void WriteDataSetXml(XmlWriter writer, Event dataItem)
        {
            try
            {
                // Get Element name
                var name = GetElementName(dataItem);
                if (!string.IsNullOrEmpty(name))
                {
                    var node = _document.CreateNode(XmlNodeType.Element, null, name, null);
                    if (node != null)
                    {
                        XmlNode entryNode;
                        XmlAttribute attribute;

                        // Add Common Attributes to Node
                        AddCommonAttributes(dataItem, node);

                        if (!dataItem.Entries.IsNullOrEmpty())
                        {
                            // Count Attribute
                            attribute = _document.CreateAttribute("count");
                            attribute.Value = dataItem.Entries.Count().ToString();
                            node.Attributes.Append(attribute);

                            foreach (var entry in dataItem.Entries.OrderBy(o => o.Key))
                            {
                                // Create the Entry XML Element
                                entryNode = _document.CreateNode(XmlNodeType.Element, null, "Entry", null);

                                // Key Attribute
                                attribute = _document.CreateAttribute("key");
                                attribute.Value = entry.Key;
                                entryNode.Attributes.Append(attribute);

                                // Removed Attribute
                                if (entry.Removed)
                                {
                                    attribute = _document.CreateAttribute("removed");
                                    attribute.Value = entry.Removed.ToString();
                                    entryNode.Attributes.Append(attribute);
                                }

                                entryNode.InnerText = entry.CDATA;

                                node.AppendChild(entryNode);
                            }
                        }

                        // Write Node to XmlWriter
                        writer.WriteNode(new XmlNodeReader(node), false);
                    }
                }
            }
            catch { }
        }

        private void WriteTableXml(XmlWriter writer, Event dataItem)
        {
            try
            {
                // Get Element name
                var name = GetElementName(dataItem);
                if (!string.IsNullOrEmpty(name))
                {
                    var node = _document.CreateNode(XmlNodeType.Element, null, name, null);
                    if (node != null)
                    {
                        XmlNode entryNode;
                        XmlNode cellNode;
                        XmlAttribute attribute;

                        // Add Common Attributes to Node
                        AddCommonAttributes(dataItem, node);

                        if (!dataItem.Entries.IsNullOrEmpty())
                        {
                            // Count Attribute
                            attribute = _document.CreateAttribute("count");
                            attribute.Value = dataItem.Entries.Count().ToString();
                            node.Attributes.Append(attribute);

                            foreach (var entry in dataItem.Entries.OrderBy(o => o.Key))
                            {
                                // Create the Entry XML Element
                                entryNode = _document.CreateNode(XmlNodeType.Element, null, "Entry", null);

                                // Key Attribute
                                attribute = _document.CreateAttribute("key");
                                attribute.Value = entry.Key;
                                entryNode.Attributes.Append(attribute);

                                // Removed Attribute
                                if (entry.Removed)
                                {
                                    attribute = _document.CreateAttribute("removed");
                                    attribute.Value = entry.Removed.ToString();
                                    entryNode.Attributes.Append(attribute);
                                }
                                
                                // Create the Cell XML Child Elements
                                if (!entry.Cells.IsNullOrEmpty())
                                {
                                    foreach (var cell in entry.Cells)
                                    {
                                        // Create the Cell XML Element
                                        cellNode = _document.CreateNode(XmlNodeType.Element, null, "Cell", null);

                                        // Key Attribute
                                        attribute = _document.CreateAttribute("key");
                                        attribute.Value = cell.Key;
                                        cellNode.Attributes.Append(attribute);

                                        cellNode.InnerText = cell.CDATA;

                                        entryNode.AppendChild(cellNode);
                                    }
                                }
                                
                                node.AppendChild(entryNode);
                            }
                        }

                        // Write Node to XmlWriter
                        writer.WriteNode(new XmlNodeReader(node), false);
                    }
                }
            }
            catch { }
        }


        public void ReadXml(XmlReader reader)
        {
            try
            {
                // Read Child Elements
                using (var inner = reader.ReadSubtree())
                {
                    while (inner.Read())
                    {
                        if (inner.NodeType == XmlNodeType.Element)
                        {
                            // Create a copy of each Child Node so we can change the name to "DataItem" and deserialize it
                            // (Seems like a dirty way to do this but until an XmlAttribute can be found to ignore the Node's name/type
                            // and to always deserialize as a DataItem)
                            // Condition uses the node name as the condition value
                            var node = _document.ReadNode(inner);
                            foreach (XmlNode child in node.ChildNodes)
                            {
                                if (child.NodeType == XmlNodeType.Element)
                                {
                                    // Clear any previous Attributes
                                    _copy.Attributes.RemoveAll();

                                    // Copy Attributes
                                    foreach (XmlAttribute attribute in child.Attributes)
                                    {
                                        var attr = _document.CreateAttribute(attribute.Name);
                                        attr.Value = attribute.Value;
                                        _copy.Attributes.Append(attr);
                                    }

                                    // Copy Text
                                    _copy.InnerText = child.InnerText;
                                    _copy.InnerXml = child.InnerXml;

                                    var dataItem = (Event)_serializer.Deserialize(new XmlNodeReader(_copy));
                                    dataItem.Type = GetDataItemType(child.Name);
                                    Events.Add(dataItem);
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            // Advance Reader
            reader.Skip();
        }

        public XmlSchema GetSchema()
        {
            return (null);
        }


        private static bool IsDataSet(string type)
        {
            return !string.IsNullOrEmpty(type) && type.EndsWith(DataSetSuffix);
        }

        private static bool IsTable(string type)
        {
            return !string.IsNullOrEmpty(type) && type.EndsWith(TableSuffix);
        }

        private static string GetDataItemType(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                if (IsDataSet(type) && type.Length > DataSetSuffix.Length)
                {
                    // Remove the "DataSet" suffix from the Type
                    return type.Substring(0, type.Length - DataSetSuffix.Length);
                }
                else if (IsTable(type) && type.Length > TableSuffix.Length)
                {
                    // Remove the "Table" suffix from the Type
                    return type.Substring(0, type.Length - TableSuffix.Length);
                }
                else
                {
                    return type;
                }
            }

            return null;
        }

        #endregion
    }
}
