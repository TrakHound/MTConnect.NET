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

namespace MTConnect.Streams
{
    /// <summary>
    /// Events is a XML container type element. 
    /// Events organizes the Data Entities returned in the MTConnectStreams XML document for those DataItem elements defined with a category attribute of EVENT in the MTConnectDevices document.
    /// </summary>
    public class XmlEventsContainer : IXmlSerializable
    {
        private const string NodePrefixPattern = "(.*):(.*)";
        private readonly XmlDocument _document;

        /// <summary>
        /// An XML container type element that organizes the data reported in the MTConnectStreams document for DataItem elements defined in the MTConnectDevices document with a category attribute of EVENT.
        /// </summary>
        [XmlIgnore]
        public List<EventObservation> Events { get; set; }


        public XmlEventsContainer()
        {
            // Initialize the Events List
            Events = new List<EventObservation>();

            // Create a dummy XmlDocument to use create dummy nodes
            _document = new XmlDocument();
        }

        public XmlEventsContainer(IObservation observation)
        {
            // Initialize the Events List
            Events = new List<EventObservation>();
            if (observation != null)
            {
                var eventObservation = observation as EventObservation;
                if (eventObservation != null) Events.Add(eventObservation);
            }

            // Create a dummy XmlDocument to use create dummy nodes
            _document = new XmlDocument();
        }


        #region "Xml Serialization"

        public void WriteXml(XmlWriter writer)
        {
            if (!Events.IsNullOrEmpty())
            {
                foreach (var dataItem in Events)
                {
                    switch (dataItem.Representation)
                    {
                        case DataItemRepresentation.VALUE: WriteValueXml(writer, (EventValueObservation)dataItem); break;
                        case DataItemRepresentation.DATA_SET: WriteDataSetXml(writer, (EventDataSetObservation)dataItem); break;
                        case DataItemRepresentation.TABLE: WriteTableXml(writer, (EventTableObservation)dataItem); break;
                    }
                }
            }
        }

        private static string GetElementName(IEventObservation observation)
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
                }

                return name;
            }

            return null;
        }

        private void WriteValueXml(XmlWriter writer, EventValueObservation observation)
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

                        // Write Value Description as Comment
                        var valueDescriptionText = Observation.GetDescriptionText(observation.Category, observation.Type, observation.SubType, observation.CDATA);
                        if (!string.IsNullOrEmpty(valueDescriptionText))
                        {
                            writer.WriteComment($"CDATA = {observation.CDATA} : {valueDescriptionText}");
                            writer.WriteWhitespace("\r\n");
                        }

                        // Write Node to XmlWriter
                        writer.WriteNode(new XmlNodeReader(node), false);
                    }
                }
            }
            catch { }
        }

        private void WriteDataSetXml(XmlWriter writer, EventDataSetObservation observation)
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

                        // Count Attribute
                        attribute = _document.CreateAttribute("count");
                        attribute.Value = !observation.Entries.IsNullOrEmpty() ? observation.Entries.Count().ToString() : "0";
                        node.Attributes.Append(attribute);

                        if (!observation.Entries.IsNullOrEmpty())
                        {
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
                        else
                        {
                            node.InnerText = Observation.Unavailable;
                        }

                        // Write Node to XmlWriter
                        writer.WriteNode(new XmlNodeReader(node), false);
                    }
                }
            }
            catch { }
        }

        private void WriteTableXml(XmlWriter writer, EventTableObservation observation)
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

                        // Count Attribute
                        attribute = _document.CreateAttribute("count");
                        attribute.Value = !observation.Entries.IsNullOrEmpty() ? observation.Entries.Count().ToString() : "0";
                        node.Attributes.Append(attribute);

                        if (!observation.Entries.IsNullOrEmpty())
                        {
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
                        else
                        {
                            node.InnerText = Observation.Unavailable;
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
                                    //System.Console.WriteLine(child.Name);

                                    var elementName = child.Name;
                                    var type = XmlObservation.GetDataItemType(elementName);
                                    var representation = XmlObservation.GetDataItemRepresentation(elementName);

                                    // Create a new Observation based on Type and Representation
                                    var observation = EventObservation.Create(type, representation);
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
                                        }

                                        Events.Add(observation);
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
