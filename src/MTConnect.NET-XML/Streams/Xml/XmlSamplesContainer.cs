// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.DataItems;
using MTConnect.Observations;
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
    /// Samples organizes the Data Entities returned in the MTConnectStreams XML document for those DataItem elements defined with a category attribute of EVENT in the MTConnectDevices document.
    /// </summary>
    public class XmlSamplesContainer : IXmlSerializable
    {
        private const string NodePrefixPattern = "(.*):(.*)";
        private readonly XmlDocument _document;

        /// <summary>
        /// An XML container type element that organizes the data reported in the MTConnectStreams document for DataItem elements defined in the MTConnectDevices document with a category attribute of EVENT.
        /// </summary>
        [XmlIgnore]
        public List<SampleObservation> Samples { get; set; }


        public XmlSamplesContainer()
        {
            // Initialize the Samples List
            Samples = new List<SampleObservation>();

            // Create a dummy XmlDocument to use create dummy nodes
            _document = new XmlDocument();
        }


        #region "Xml Serialization"

        public void WriteXml(XmlWriter writer)
        {
            if (!Samples.IsNullOrEmpty())
            {
                foreach (var dataItem in Samples)
                {
                    switch (dataItem.Representation)
                    {
                        case DataItemRepresentation.VALUE: WriteValueXml(writer, (SampleValueObservation)dataItem); break;
                        case DataItemRepresentation.DATA_SET: WriteDataSetXml(writer, (SampleDataSetObservation)dataItem); break;
                        case DataItemRepresentation.TABLE: WriteTableXml(writer, (SampleTableObservation)dataItem); break;
                        case DataItemRepresentation.TIME_SERIES: WriteTimeSeriesXml(writer, (SampleTimeSeriesObservation)dataItem); break;
                    }
                }
            }
        }

        private static string GetElementName(SampleObservation observation)
        {
            if (observation != null && !string.IsNullOrEmpty(observation.Type))
            {
                string name;

                var match = Regex.Match(observation.Type, NodePrefixPattern);
                if (match.Success)
                {
                    name = Devices.DataItem.GetPascalCaseType(match.Groups[2].Value);
                }
                else
                {
                    name = Devices.DataItem.GetPascalCaseType(observation.Type);
                }

                // Add Suffix based on Representation
                switch (observation.Representation)
                {
                    case DataItemRepresentation.DATA_SET: name += XmlObservation.DataSetSuffix; break;
                    case DataItemRepresentation.TABLE: name += XmlObservation.TableSuffix; break;
                    case DataItemRepresentation.TIME_SERIES: name += XmlObservation.TimeSeriesSuffix; break;
                }

                return name;
            }

            return null;
        }

        private void WriteValueXml(XmlWriter writer, SampleValueObservation observation)
        {
            try
            {
                // Get Element name
                var name = GetElementName(observation);
                if (!string.IsNullOrEmpty(name))
                {
                    var node = _document.CreateNode(XmlNodeType.Element, null, name, null);
                    if (node != null)
                    {
                        // Add Common Attributes to Node
                        XmlObservation.AddAttributes(observation, node);

                        // Set InnerText to the CDATA
                        node.InnerText = observation.CDATA?.Trim();

                        // Add Comment
                        if (observation.DataItem != null)
                        {
                            // Write DataItem Type Description as Comment
                            writer.WriteComment($"Type = {observation.DataItem.Type} : {observation.DataItem.TypeDescription}");
                            writer.WriteWhitespace("\r\n");

                            // Write DataItem SubType Description as Comment
                            if (!string.IsNullOrEmpty(observation.DataItem.SubType))
                            {
                                writer.WriteComment($"SubType = {observation.DataItem.SubType} : {observation.DataItem.SubTypeDescription}");
                                writer.WriteWhitespace("\r\n");
                            }
                        }

                        // Write Node to XmlWriter
                        writer.WriteNode(new XmlNodeReader(node), false);
                    }
                }
            }
            catch { }
        }

        private void WriteDataSetXml(XmlWriter writer, SampleDataSetObservation observation)
        {
            try
            {
                // Get Element name
                var name = GetElementName(observation);
                if (!string.IsNullOrEmpty(name))
                {
                    var node = _document.CreateNode(XmlNodeType.Element, null, name, null);
                    if (node != null)
                    {
                        XmlNode entryNode;
                        XmlAttribute attribute;

                        // Add Common Attributes to Node
                        XmlObservation.AddAttributes(observation, node);

                        if (!observation.Entries.IsNullOrEmpty())
                        {
                            // Count Attribute
                            attribute = _document.CreateAttribute("count");
                            attribute.Value = observation.Entries.Count().ToString();
                            node.Attributes.Append(attribute);

                            foreach (var entry in observation.Entries.OrderBy(o => o.Key))
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

                                if (entry.Value != null && !entry.Removed) entryNode.InnerText = entry.Value;

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

        private void WriteTableXml(XmlWriter writer, SampleTableObservation observation)
        {
            try
            {
                // Get Element name
                var name = GetElementName(observation);
                if (!string.IsNullOrEmpty(name))
                {
                    var node = _document.CreateNode(XmlNodeType.Element, null, name, null);
                    if (node != null)
                    {
                        XmlNode entryNode;
                        XmlNode cellNode;
                        XmlAttribute attribute;

                        // Add Common Attributes to Node
                        XmlObservation.AddAttributes(observation, node);

                        if (!observation.Entries.IsNullOrEmpty())
                        {
                            // Count Attribute
                            attribute = _document.CreateAttribute("count");
                            attribute.Value = observation.Entries.Count().ToString();
                            node.Attributes.Append(attribute);

                            foreach (var entry in observation.Entries.OrderBy(o => o.Key))
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

                                        cellNode.InnerText = cell.Value;

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

        private void WriteTimeSeriesXml(XmlWriter writer, SampleTimeSeriesObservation observation)
        {
            try
            {
                // Get Element name
                var name = GetElementName(observation);
                if (!string.IsNullOrEmpty(name))
                {
                    var node = _document.CreateNode(XmlNodeType.Element, null, name, null);
                    if (node != null)
                    {
                        // Add Common Attributes to Node
                        XmlObservation.AddAttributes(observation, node);

                        // Set InnerText to the CDATA
                        if (!observation.Samples.IsNullOrEmpty())
                        {
                            node.InnerText = string.Join(" ", observation.Samples);
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
                            var node = _document.ReadNode(inner);
                            foreach (XmlNode child in node.ChildNodes)
                            {
                                if (child.NodeType == XmlNodeType.Element)
                                {
                                    var elementName = child.Name;
                                    var type = XmlObservation.GetDataItemType(elementName);
                                    var representation = XmlObservation.GetDataItemRepresentation(elementName);

                                    // Create a new Observation based on Type and Representation
                                    var observation = SampleObservation.Create(type, representation);
                                    if (observation != null)
                                    {
                                        observation.SetProperty(nameof(Observation.Type), type);
                                        observation.SetProperty(nameof(Observation.Representation), representation);

                                        // Read the XML Attributes and assign the corresponding Properties
                                        XmlObservation.ReadAttributes(observation, child);

                                        switch (observation.Representation)
                                        {
                                            case DataItemRepresentation.VALUE: XmlObservation.SetValue(observation, child); break;
                                            case DataItemRepresentation.DATA_SET: XmlObservation.SetDataSetEntries(observation, child); break;
                                            case DataItemRepresentation.TABLE: XmlObservation.SetTableEntries(observation, child); break;
                                            case DataItemRepresentation.TIME_SERIES: XmlObservation.SetTimeSeriesEntries(observation, child); break;
                                        }

                                        Samples.Add(observation);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        public XmlSchema GetSchema()
        {
            return (null);
        }

        #endregion
    }
}
