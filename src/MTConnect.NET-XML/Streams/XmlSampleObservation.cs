// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace MTConnect.Streams
{
    public class XmlSampleObservation
    {
        private const string NodePrefixPattern = "(.*):(.*)";
        private static readonly XmlDocument _document = new XmlDocument();


        public static void WriteXml(XmlWriter writer, IObservation observation)
        {
            if (observation != null)
            {
                switch (observation.Representation)
                {
                    case DataItemRepresentation.VALUE: WriteValueXml(writer, (SampleValueObservation)observation); break;
                    case DataItemRepresentation.DATA_SET: WriteDataSetXml(writer, (SampleDataSetObservation)observation); break;
                    case DataItemRepresentation.TABLE: WriteTableXml(writer, (SampleTableObservation)observation); break;
                    case DataItemRepresentation.TIME_SERIES: WriteTimeSeriesXml(writer, (SampleTimeSeriesObservation)observation); break;
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

        private static void WriteValueXml(XmlWriter writer, SampleValueObservation observation)
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

        private static void WriteDataSetXml(XmlWriter writer, SampleDataSetObservation observation)
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

        private static void WriteTableXml(XmlWriter writer, SampleTableObservation observation)
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

        private static void WriteTimeSeriesXml(XmlWriter writer, SampleTimeSeriesObservation observation)
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

                        // Count Attribute
                        var attribute = _document.CreateAttribute("sampleCount");
                        attribute.Value = !observation.Samples.IsNullOrEmpty() ? observation.Samples.Count().ToString() : "0";
                        node.Attributes.Append(attribute);

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
    }
}
