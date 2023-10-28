// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Observations;
using MTConnect.Streams.Output;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace MTConnect.Streams.Xml
{
    /// <summary>
    /// The Streams Information Model provides a representation of the data reported by a piece of equipment used for a manufacturing process, or used for any other purpose.
    /// </summary>
    public static class XmlStreamsResponseDocument
    {

        #region "Read"

        public static IStreamsResponseDocument FromXml(byte[] xmlBytes)
        {
            if (xmlBytes != null && xmlBytes.Length > 0)
            {
                try
                {
                    // Clean whitespace and Encoding Marks (BOM)
                    var bytes = XmlFunctions.SanitizeBytes(xmlBytes);

                    using (var memoryReader = new MemoryStream(bytes))
                    {
                        using (var xmlReader = XmlReader.Create(memoryReader))
                        {
                            return ReadXml(xmlReader);
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static IStreamsResponseDocument ReadXml(XmlReader reader)
        {
            var document = new StreamsResponseDocument();

            reader.ReadStartElement("MTConnectStreams");
            reader.MoveToContent();
            document.Version = MTConnectVersion.GetByNamespace(reader.NamespaceURI);

            // Read Header
            document.Header = XmlStreamsHeader.ReadXml(reader);

            // Read to Streams Node
            reader.ReadToNextSibling("Streams");
            if (!reader.IsEmptyElement)
            {
                // Read to DeviceStream node
                reader.ReadToDescendant("DeviceStream");

                var deviceStreams = new List<IDeviceStream>();
                do
                {
                    var deviceStreamReader = reader.ReadSubtree();
                    deviceStreams.Add(ReadDeviceStreamXml(deviceStreamReader));
                }
                while (reader.ReadToNextSibling("DeviceStream"));
                document.Streams = deviceStreams;
            }

            return document;
        }

        public static IDeviceStream ReadDeviceStreamXml(XmlReader reader)
        {
            reader.ReadToDescendant("DeviceStream");

            var deviceStream = new DeviceStream();
            deviceStream.Name = reader.GetAttribute("name");
            deviceStream.Uuid = reader.GetAttribute("uuid");

            // Read to DeviceStream node
            reader.ReadToDescendant("ComponentStream");

            var componentStreams = new List<IComponentStream>();
            do
            {
                var componentStreamReader = reader.ReadSubtree();
                componentStreams.Add(ReadComponentStreamXml(componentStreamReader));
            }
            while (reader.ReadToNextSibling("ComponentStream"));
            deviceStream.ComponentStreams = componentStreams;

            return deviceStream;
        }

        public static IComponentStream ReadComponentStreamXml(XmlReader reader)
        {
            reader.ReadToDescendant("ComponentStream");

            var componentStream = new ComponentStream();
            componentStream.ComponentType = reader.GetAttribute("component");
            componentStream.ComponentId = reader.GetAttribute("componentId");
            componentStream.Name = reader.GetAttribute("name");
            componentStream.Uuid = reader.GetAttribute("uuid");

            reader.MoveToContent();
            reader.Read();
            while (reader.NodeType == XmlNodeType.Whitespace) reader.Read();

            var observations = new List<IObservation>();

            do
            {
                switch (reader.LocalName)
                {
                    case "Samples":
                        var samplesReader = reader.ReadSubtree();
                        observations.AddRange(ReadObservationsXml(samplesReader, DataItemCategory.SAMPLE));
                        break;

                    case "Events":
                        var eventsReader = reader.ReadSubtree();
                        observations.AddRange(ReadObservationsXml(eventsReader, DataItemCategory.EVENT));
                        break;

                    case "Condition":
                        var conditionReader = reader.ReadSubtree();
                        observations.AddRange(ReadObservationsXml(conditionReader, DataItemCategory.CONDITION));
                        break;
                }
            }
            while (reader.Read());

            componentStream.Observations = observations;

            return componentStream;
        }

        public static IEnumerable<IObservation> ReadObservationsXml(XmlReader reader, DataItemCategory category)
        {
            var observations = new List<IObservation>();

            // Set Container Name
            string container;
            switch (category)
            {
                case DataItemCategory.SAMPLE: container = "Samples"; break;
                case DataItemCategory.EVENT: container = "Events"; break;
                default: container = "Condition"; break;
            }

            reader.ReadToDescendant(container);
            reader.MoveToContent();
            reader.Read();

            do
            {
                while (reader.NodeType == XmlNodeType.Whitespace) reader.Read();

                var elementName = reader.LocalName;
                var dataItemType = XmlObservation.GetDataItemType(elementName);
                var representation = XmlObservation.GetRepresentation(elementName);

                if (reader.LocalName != container && reader.NodeType == XmlNodeType.Element)
                {
                    // Check if node is empty before moving position
                    bool isEmptyElement = reader.IsEmptyElement;
                    // Read Observation Properties
                    var observation = ReadObservationProperties(reader, category, dataItemType, representation);

                    // Set Condition Level
                    if (category == DataItemCategory.CONDITION)
                    {
                        observation.AddValue(ValueKeys.Level, elementName.ToUpper());
                    }

                    // Read Content
                    if (!isEmptyElement && reader.NodeType != XmlNodeType.None)
                    {
                        while (reader.NodeType != XmlNodeType.EndElement &&
                            ((category == DataItemCategory.CONDITION && reader.NodeType == XmlNodeType.Whitespace) ||
                            (category == DataItemCategory.CONDITION && reader.NodeType == XmlNodeType.Attribute) ||
                            (category != DataItemCategory.CONDITION && reader.NodeType != XmlNodeType.Element)))
                        {
                            reader.Read();
                            if (reader.NodeType == XmlNodeType.Text)
                            {
                                var text = reader.ReadContentAsString();
                                if (!string.IsNullOrEmpty(text))
                                {
                                    if (representation == DataItemRepresentation.TIME_SERIES)
                                    {
                                        observation.AddValues(ReadTimeSeries(text));
                                    }
                                    else if (category == DataItemCategory.CONDITION)
                                    {
                                        observation.AddValue(ValueKeys.Message, text);
                                    }
                                    else
                                    {
                                        observation.AddValue(ValueKeys.Result, text);
                                    }
                                }
                            }
                            else if (reader.NodeType == XmlNodeType.Element)
                            {
                                switch (representation)
                                {
                                    case DataItemRepresentation.DATA_SET:
                                        observation.AddValues(ReadDataSetEntries(reader));
                                        break;

                                    case DataItemRepresentation.TABLE:
                                        observation.AddValues(ReadTableEntries(reader));
                                        break;
                                }
                            }
                        }
                    }

                    switch (observation.Category)
                    {
                        case DataItemCategory.CONDITION: observations.Add(ConditionObservation.Create(observation)); break;

                        case DataItemCategory.EVENT: observations.Add(EventObservation.Create(observation)); break;

                        case DataItemCategory.SAMPLE: observations.Add(SampleObservation.Create(observation)); break;
                    }
                }
            }
            while (reader.Read());

            return observations;
        }

        private static Observation ReadObservationProperties(
            XmlReader reader,
            DataItemCategory category,
            string type,
            DataItemRepresentation representation
            )
        {
            Observation observation;

            switch (category)
            {
                case DataItemCategory.SAMPLE:
                    observation = SampleObservation.Create(type, representation);
                    observation.Type = type;
                    break;

                case DataItemCategory.EVENT:
                    observation = EventObservation.Create(type, representation);
                    observation.Type = type;
                    break;

                default:
                    type = reader.GetAttribute("type");
                    observation = ConditionObservation.Create(type, representation);
                    break;
            }

            if (reader.HasAttributes)
            {
                for (var i = 0; i < reader.AttributeCount; i++)
                {
                    reader.MoveToAttribute(i);

                    switch (reader.Name)
                    {
                        case "dataItemId":
                            observation.DataItemId = reader.Value;
                            break;

                        case "name":
                            observation.Name = reader.Value;
                            break;

                        case "type":
                            observation.Type = reader.Value;
                            break;

                        case "subType":
                            observation.SubType = reader.Value;
                            break;

                        case "sequence":
                            observation.Sequence = reader.Value.ToLong();
                            break;

                        case "timestamp":
                            observation.Timestamp = reader.Value.ToDateTime().ToUniversalTime();
                            break;

                        case "compositionId":
                            observation.CompositionId = reader.Value;
                            break;

                        default:
                            // Add as Value
                            var valueKey = ValueKeys.GetPascalCaseKey(reader.Name);
                            observation.AddValue(new ObservationValue(valueKey, reader.Value));
                            break;
                    }
                }
            }

            return observation;
        }


        private static IEnumerable<ObservationValue> ReadDataSetEntries(XmlReader reader)
        {
            var values = new List<ObservationValue>();

            do
            {
                var entryKey = reader.GetAttribute("key");
                var valueKey = ValueKeys.CreateDataSetValueKey(entryKey);
                var removed = reader.GetAttribute("removed").ToBoolean();
                if (!removed)
                {
                    string value = null;

                    // Read Content
                    if (reader.NodeType != XmlNodeType.None)
                    {
                        while (reader.NodeType != XmlNodeType.EndElement)
                        {
                            reader.Read();
                            if (reader.NodeType == XmlNodeType.Text)
                            {
                                value = reader.ReadContentAsString();
                            }
                        }
                    }

                    values.Add(new ObservationValue(valueKey, value));
                }
                else
                {
                    values.Add(new ObservationValue(valueKey, DataSetObservation.EntryRemovedValue));
                }
            }
            while (reader.ReadToNextSibling("Entry"));

            return values;
        }

        private static IEnumerable<ObservationValue> ReadTableEntries(XmlReader reader)
        {
            var values = new List<ObservationValue>();

            do
            {
                var entryKey = reader.GetAttribute("key");
                var valueKey = ValueKeys.CreateTableValueKey(entryKey);
                var removed = reader.GetAttribute("removed").ToBoolean();
                if (!removed)
                {
                    var cellReader = reader.ReadSubtree();
                    values.AddRange(ReadTableCells(cellReader, entryKey));
                    reader.Skip();
                }
                else
                {
                    values.Add(new ObservationValue(valueKey, TableObservation.EntryRemovedValue));
                }
            }
            while (reader.ReadToNextSibling("Entry"));

            return values;
        }

        private static IEnumerable<ObservationValue> ReadTableCells(XmlReader reader, string entryKey)
        {
            var values = new List<ObservationValue>();

            reader.ReadToDescendant("Cell");

            do
            {
                var cellKey = reader.GetAttribute("key");
                var valueKey = ValueKeys.CreateTableValueKey(entryKey, cellKey);
                string value = null;

                // Read Content
                if (reader.NodeType != XmlNodeType.None)
                {
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        reader.Read();
                        if (reader.NodeType == XmlNodeType.Text)
                        {
                            value = reader.ReadContentAsString();
                        }
                    }
                }

                values.Add(new ObservationValue(valueKey, value));
            }
            while (reader.ReadToNextSibling("Cell"));

            return values;
        }

        private static IEnumerable<ObservationValue> ReadTimeSeries(string text)
        {
            var values = new List<ObservationValue>();

            if (!string.IsNullOrEmpty(text))
            {
                var samples = text.Split(' ');
                if (samples != null && samples.Length > 0)
                {
                    for (var i = 0; i < samples.Length; i++)
                    {
                        values.Add(new ObservationValue(ValueKeys.CreateTimeSeriesValueKey(i), samples[i]));
                    }
                }
            }

            return values;
        }

        #endregion

        #region "Write"

        public static byte[] ToXmlBytes(
            ref IStreamsResponseOutputDocument document,
            IEnumerable<NamespaceConfiguration> extendedSchemas = null,
            string styleSheet = null,
            bool indent = true,
            bool outputComments = false
            )
        {
            if (document != null && document.Header != null)
            {
                try
                {
                    var mtconnectStreamsNamespace = Namespaces.GetStreams(document.Version.Major, document.Version.Minor);

                    using (var stream = new MemoryStream())
                    {
                        // Set the XmlWriterSettings to use
                        var xmlWriterSettings = indent ? XmlFunctions.XmlWriterSettingsIndent : XmlFunctions.XmlWriterSettings;

                        // Use XmlWriter to write XML to stream
                        using (var xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                        {
                            WriteXml(xmlWriter, ref document, extendedSchemas, styleSheet, indent, outputComments);
                            return stream.ToArray();
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static void WriteXml(
            XmlWriter writer,
            ref IStreamsResponseOutputDocument document,
            IEnumerable<NamespaceConfiguration> extendedSchemas = null,
            string styleSheet = null,
            bool indent = true,
            bool outputComments = false
            )
        {
            if (document != null && document.Streams != null && !document.Streams.IsNullOrEmpty())
            {
                var ns = Namespaces.GetStreams(document.Version.Major, document.Version.Minor);

                writer.WriteStartDocument();
                if (indent) writer.WriteWhitespace(XmlFunctions.NewLine);

                // Add Stylesheet
                if (!string.IsNullOrEmpty(styleSheet))
                {
                    writer.WriteRaw($"<?xml-stylesheet type=\"text/xsl\" href=\"{styleSheet}?version={document.Version}\"?>");
                    if (indent) writer.WriteWhitespace(XmlFunctions.NewLine);
                }

                // Add Header Comment
                if (outputComments) XmlFunctions.WriteHeaderComment(writer, indent);

                // Write Root Document Element
                writer.WriteStartElement("MTConnectStreams", ns);

                // Write Namespace Declarations
                writer.WriteAttributeString("xmlns", null, null, ns);
                writer.WriteAttributeString("xmlns", "m", null, ns);
                writer.WriteAttributeString("xmlns", "xsi", null, Namespaces.DefaultXmlSchemaInstance);

                if (!extendedSchemas.IsNullOrEmpty())
                {
                    foreach (var schema in extendedSchemas)
                    {
                        writer.WriteAttributeString("xmlns", schema.Alias, null, schema.Urn);
                    }
                }

                // Write Schema Location
                writer.WriteAttributeString("xsi", "schemaLocation", null, Schemas.GetStreams(document.Version.Major, document.Version.Minor));

                // Write Header
                XmlStreamsHeader.WriteXml(writer, document.Header);

                writer.WriteStartElement("Streams");

                // Write Device Streams
                for (var i = 0; i < document.Streams.Length; i++)
                {
                    XmlDeviceStream.WriteXml(writer, ref document.Streams[i], extendedSchemas, outputComments);
                }

                writer.WriteEndElement(); // Streams
                writer.WriteEndElement(); // MTConnectStreams
                writer.WriteEndDocument();
                writer.Flush();
            }
        }

        #endregion

    }
}