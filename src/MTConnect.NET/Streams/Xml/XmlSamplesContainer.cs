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
    /// Samples is a XML container type element. 
    /// Samples organizes the Data Entities returned in the MTConnectStreams XML document for those DataItem elements defined with a category attribute of SAMPLE in the MTConnectDevices document.
    /// </summary>
    public class XmlSamplesContainer : IXmlSerializable
    {
        private const string NodePrefixPattern = "(.*):(.*)";
        private const string TimeSeriesSuffix = "TimeSeries";
        private const string DataSetSuffix = "DataSet";

        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(Sample));

        private readonly XmlDocument _document;
        private readonly XmlNode _copy;

        /// <summary>
        /// An XML container type element that organizes the data reported in the MTConnectStreams document for DataItem elements defined in the MTConnectDevices document with a category attribute of SAMPLE.
        /// </summary>
        [XmlIgnore]
        public List<Sample> Samples { get; set; }


        public XmlSamplesContainer()
        {
            // Initialize the Samples List
            Samples = new List<Sample>();

            // Create a dummy XmlDocument to use create dummy nodes
            _document = new XmlDocument();

            // Create a new Node with the name of "DataItem"
            _copy = _document.CreateNode(XmlNodeType.Element, Devices.DataItemCategory.SAMPLE.ToString().ToPascalCase(), null);
        }


        #region "Xml Serialization"

        public void WriteXml(XmlWriter writer)
        {
            if (!Samples.IsNullOrEmpty())
            {
                foreach (var dataItem in Samples)
                {
                    if (dataItem.IsTimeSeries)
                    {
                        WriteTimeSeriesXml(writer, dataItem);
                    }
                    else if (dataItem.IsDataSet)
                    {
                        WriteDataSetXml(writer, dataItem);
                    }
                    else
                    {
                        WriteValueXml(writer, dataItem);
                    }
                }
            }
        }

        private static string GetElementName(Sample dataItem)
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
                if (dataItem.IsTimeSeries) name += TimeSeriesSuffix;
                else if (dataItem.IsDataSet) name += DataSetSuffix;

                return name;
            }

            return null;
        }

        private void AddCommonAttributes(Sample dataItem, XmlNode node)
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

                // Statistic
                if (dataItem.Statistic != Devices.DataItemStatistic.NONE)
                {
                    attribute = _document.CreateAttribute("statistic");
                    attribute.Value = dataItem.Statistic.ToString();
                    node.Attributes.Append(attribute);
                }

                // Duration
                if (dataItem.Duration > 0)
                {
                    attribute = _document.CreateAttribute("duration");
                    attribute.Value = dataItem.Duration.ToString();
                    node.Attributes.Append(attribute);
                }
            }
        }


        private void WriteValueXml(XmlWriter writer, Sample dataItem)
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

        private void WriteTimeSeriesXml(XmlWriter writer, Sample dataItem)
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
                        XmlAttribute attribute;

                        // Add Common Attributes to Node
                        AddCommonAttributes(dataItem, node);

                        // If TimeSeries, set InnerText to space delimited values
                        var timeSeries = new TimeSeries(dataItem);
                        node.InnerText = string.Join(" ", timeSeries.Values);

                        // SampleRate
                        if (timeSeries.SampleRate > 0)
                        {
                            attribute = _document.CreateAttribute("sampleRate");
                            attribute.Value = timeSeries.SampleRate.ToString();
                            node.Attributes.Append(attribute);
                        }

                        // SampleCount
                        if (!timeSeries.Values.IsNullOrEmpty())
                        {
                            attribute = _document.CreateAttribute("sampleCount");
                            attribute.Value = timeSeries.Values.Count().ToString();
                            node.Attributes.Append(attribute);
                        }

                        // Write Node to XmlWriter
                        writer.WriteNode(new XmlNodeReader(node), false);
                    }
                }
            }
            catch { }
        }

        private void WriteDataSetXml(XmlWriter writer, Sample dataItem)
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

                            foreach (var entry in dataItem.Entries)
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
                                    attribute.Value = entry.Removed.ToString().ToLower();
                                    entryNode.Attributes.Append(attribute);
                                }

                                if (entry.CDATA != null) entryNode.InnerText = entry.CDATA;

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

                                    var dataItem = (Sample)_serializer.Deserialize(new XmlNodeReader(_copy));
                                    dataItem.Type = GetDataItemType(child.Name);
                                    Samples.Add(dataItem);
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

        private static bool IsTimeSeries(string type)
        {
            return !string.IsNullOrEmpty(type) && type.EndsWith(TimeSeriesSuffix);
        }

        private static bool IsDataSet(string type)
        {
            return !string.IsNullOrEmpty(type) && type.EndsWith(DataSetSuffix);
        }

        private static string GetDataItemType(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                if (IsTimeSeries(type) && type.Length > TimeSeriesSuffix.Length)
                {
                    // Remove the "TimeSeries" suffix from the Type
                    return type.Substring(0, type.Length - TimeSeriesSuffix.Length);
                }
                else if (IsDataSet(type) && type.Length > DataSetSuffix.Length)
                {
                    // Remove the "DataSet" suffix from the Type
                    return type.Substring(0, type.Length - DataSetSuffix.Length);
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
