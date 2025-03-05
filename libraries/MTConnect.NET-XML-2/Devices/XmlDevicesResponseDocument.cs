// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using MTConnect.Observations;
using MTConnect.Streams.Xml;
using MTConnect.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("MTConnectDevices")]
    public class XmlDevicesResponseDocument
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlDevicesResponseDocument));


        class CustomXmlUrlResolver : XmlUrlResolver
        {
            public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
            {
                // Ignore namespace resolution
                return base.GetEntity(absoluteUri, "", ofObjectToReturn);
            }
        }


        [XmlElement("Header")]
        public XmlDevicesHeader Header { get; set; }

        [XmlArray("Devices")]
        [XmlArrayItem("Device", typeof(XmlDevice))]
        [XmlArrayItem("Agent", typeof(XmlAgent))]
        public List<XmlDevice> Devices { get; set; }

        //[XmlArray("Interfaces")]
        //public List<Interface> Interfaces { get; set; }

        [XmlIgnore]
        public Version Version { get; set; }


        public IDevicesResponseDocument ToDocument()
        {
            var document = new DevicesResponseDocument();
            if (Header != null) document.Header = Header.ToDevicesHeader();
            document.Version = Version;

            if (!Devices.IsNullOrEmpty())
            {
                var devices = new List<IDevice>();

                foreach (var xmlDevice in Devices)
                {
                    var device = xmlDevice.ToDevice();
                    ((Device)device).MTConnectVersion = Version;
                    devices.Add(device);
                }

                document.Devices = devices;
            }

            //document.Interfaces = Interfaces;

            return document;
        }

        //public static IDevicesResponseDocument FromXml(byte[] xmlBytes)
        //{
        //    if (xmlBytes != null && xmlBytes.Length > 0)
        //    {
        //        try
        //        {
        //            // Clean whitespace and Encoding Marks (BOM)
        //            //var bytes = XmlFunctions.SanitizeBytes(xmlBytes);

        //            //var xml = Encoding.UTF8.GetString(bytes);
        //            //var version = MTConnectVersion.Get(xml);

        //            //xml = xml.Trim();
        //            //xml = Namespaces.Clear(xml);

        //            //using (var textReader = new StringReader(xml))
        //            using (var textReader = new TextReader(xmlBytes))
        //            {
        //                using (var xmlReader = XmlReader.Create(xmlBytes))
        //                {
        //                    return ReadXml(xmlReader);

        //                    //var xmlDocument = (XmlDevicesResponseDocument)_serializer.Deserialize(xmlReader);
        //                    //if (xmlDocument != null)
        //                    //{
        //                    //    xmlDocument.Version = version;
        //                    //    var document = xmlDocument.ToDocument();
        //                    //    return document;
        //                    //}
        //                }
        //            }
        //        }
        //        catch { }
        //    }

        //    return null;
        //}

        public static byte[] ToXmlBytes(
            IDevicesResponseDocument document,
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
                    using (var stream = new MemoryStream())
                    {
                        // Set the XmlWriterSettings to use
                        var xmlWriterSettings = indent ? XmlFunctions.XmlWriterSettingsIndent : XmlFunctions.XmlWriterSettings;

                        // Use XmlWriter to write XML to stream
                        using (var xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                        {
                            WriteXml(xmlWriter, document, indent, outputComments, styleSheet, extendedSchemas);
                            return stream.ToArray();
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static Stream ToXmlStream(
            IDevicesResponseDocument document,
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
                    var outputStream = new MemoryStream();

                    // Set the XmlWriterSettings to use
                    var xmlWriterSettings = indent ? XmlFunctions.XmlWriterSettingsIndent : XmlFunctions.XmlWriterSettings;

                    // Use XmlWriter to write XML to stream
                    using (var xmlWriter = XmlWriter.Create(outputStream, xmlWriterSettings))
                    {
                        WriteXml(xmlWriter, document, indent, outputComments, styleSheet, extendedSchemas);
                        return outputStream;
                    }
                }
                catch { }
            }

            return null;
        }

        public static IDevicesResponseDocument ReadXml(XmlReader reader)
        {
            var document = new DevicesResponseDocument();

            reader.ReadStartElement("MTConnectDevices");
            reader.MoveToContent();
            document.Version = MTConnectVersion.GetByNamespace(reader.NamespaceURI);

            // Read Header
            document.Header = XmlDevicesHeader.ReadXml(reader);

            // Read to Devices Node
            reader.ReadToNextSibling("Devices");
            if (!reader.IsEmptyElement)
            {
                // Read to Device node
                reader.ReadToDescendant("Device");

                var devices = new List<IDevice>();
                do
                {
                    var deviceReader = reader.ReadSubtree();
                    devices.Add(ReadDeviceXml(deviceReader));
                }
                while (reader.ReadToNextSibling("Device"));
                document.Devices = devices;
            }

            return document;
        }

        public static IDevice ReadDeviceXml(XmlReader reader)
        {
            reader.ReadToDescendant("Device");

            var device = new Device();
            device.Uuid = reader.GetAttribute("uuid");
            device.Id = reader.GetAttribute("id");
            device.Name = reader.GetAttribute("name");

            //var dummyComponent = new Component();
            //dummyComponent.Type = "Controller";
            //dummyComponent.Id = StringFunctions.RandomString(10);
            //device.AddComponent(dummyComponent);

            //reader.MoveToContent();
            //reader.MoveToElement();

            // Read to Component node
            //reader.ReadStartElement("Components");
            if (reader.ReadToDescendant("Components"))
            {
                var componentsReader = reader.ReadSubtree();
                if (!componentsReader.IsEmptyElement)
                {
                    componentsReader.MoveToContent();
                    var components = ReadComponentsXml(componentsReader);
                    if (!components.IsNullOrEmpty())
                    {
                        device.Components = components;
                    }
                }






                //reader.MoveToContent();
                //reader.MoveToElement();

                //var components = new List<IComponent>();
                //do
                //{
                //    var componentReader = reader.ReadSubtree();

                //    //var component = ReadComponentXml(componentReader);
                //    //if (component != null)
                //    //{
                //    //    components.Add(component);
                //    //}
                //}
                //while (reader.ReadToNextSibling("Component"));
                //device.Components = components;
            }

            return device;
        }

        public static IEnumerable<IComponent> ReadComponentsXml(XmlReader reader)
        {
            while (reader.NodeType == XmlNodeType.Whitespace) reader.Read();
            //reader.Read();
            //reader.ReadStartElement();
            //reader.MoveToContent();

            var components = new List<IComponent>();

            do
            {
                reader.MoveToContent();
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var componentReader = reader.ReadSubtree();
                    var component = ReadComponentXml(componentReader);
                    if (component != null)
                    {
                        components.Add(component);
                    }
                    reader.Skip();
                }
                else
                {
                    break;
                }

            } while (reader.NodeType != XmlNodeType.EndElement);

            return components;
        }

        public static IComponent ReadComponentXml(XmlReader reader)
        {
            reader.MoveToContent();

            var elementName = reader.LocalName;

            var component = new Component();
            component.Type = RemoveNamespacePrefix(elementName);
            component.Id = reader.GetAttribute("id");
            component.Name = reader.GetAttribute("name");
            component.Uuid = reader.GetAttribute("uuid");

            reader.MoveToContent();
            reader.Read();

            do
            {
                reader.MoveToContent();

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "DataItems":

                            //var dataItemsReader = reader.ReadSubtree();
                            //if (!dataItemsReader.IsEmptyElement)
                            //{
                            //    dataItemsReader.MoveToContent();
                            //    var dataItems = ReadDataItemsXml(dataItemsReader);
                            //    if (!dataItems.IsNullOrEmpty())
                            //    {
                            //        component.DataItems = dataItems;
                            //    }
                            //}
                            break;
                        case "Compositions": break;
                        case "Components":

                            //var componentsReader = reader.ReadSubtree();
                            //if (!componentsReader.IsEmptyElement)
                            //{
                            //    componentsReader.MoveToContent();
                            //    var components = ReadComponentsXml(componentsReader);
                            //    if (!components.IsNullOrEmpty())
                            //    {
                            //        component.Components = components;
                            //    }
                            //}
                            break;
                    }

                    reader.Skip();
                }
                else
                {
                    break;
                }

            } while (reader.NodeType != XmlNodeType.EndElement);

            return component;
        }

        public static IEnumerable<IDataItem> ReadDataItemsXml(XmlReader reader)
        {
            reader.ReadStartElement();
            //reader.MoveToContent();

            var dataItems = new List<IDataItem>();

            do
            {
                reader.MoveToContent();
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var dataItemReader = reader.ReadSubtree();
                    var dataItem = ReadDataItemXml(dataItemReader);
                    if (dataItem != null)
                    {
                        dataItems.Add(dataItem);
                    }
                    reader.Skip();
                }
                else
                {
                    break;
                }

            } while (reader.NodeType != XmlNodeType.EndElement);

            return dataItems;
        }

        public static IDataItem ReadDataItemXml(XmlReader reader)
        {
            reader.MoveToContent();

            var dataItem = new DataItem();
            dataItem.Id = reader.GetAttribute("id");
            dataItem.Category = reader.GetAttribute("category").ConvertEnum<DataItemCategory>();
            dataItem.Type = reader.GetAttribute("type");
            dataItem.SubType = reader.GetAttribute("subType");
            dataItem.Name = reader.GetAttribute("name");
            dataItem.Units = reader.GetAttribute("units");
            dataItem.NativeUnits = reader.GetAttribute("nativeUnits");

            var representation = reader.GetAttribute("representation");
            if (representation != null) dataItem.Representation = representation.ConvertEnum<DataItemRepresentation>();


            reader.MoveToContent();
            reader.Read();

            do
            {
                reader.MoveToContent();

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "DataItems": break;
                        case "Compositions": break;
                    }

                    reader.Skip();
                }
                else
                {
                    break;
                }

            } while (reader.NodeType != XmlNodeType.EndElement);

            return dataItem;
        }

        //public static IComponent ReadComponentXml(XmlReader reader)
        //{
        //    //while (reader.Read())
        //    //{
        //    //    if (reader.NodeType == XmlNodeType.Element)
        //    //    {
        //    //        Console.WriteLine(reader.LocalName);
        //    //        //if (reader.Name == elementName)
        //    //        //{
        //    //        //    XElement el = XNode.ReadFrom(reader) as XElement;
        //    //        //    if (el != null)
        //    //        //    {
        //    //        //        //yield return el;
        //    //        //    }
        //    //        //}
        //    //    }
        //    //}
        //    reader.ReadStartElement();
        //    reader.MoveToContent();
        //    //reader.Read();
        //    //while (reader.NodeType == XmlNodeType.Whitespace) reader.Read();

        //    var elementName = reader.LocalName;

        //    var component = new Component();
        //    component.Type = RemoveNamespacePrefix(elementName);
        //    component.Id = reader.GetAttribute("id");
        //    component.Name = reader.GetAttribute("name");
        //    component.Uuid = reader.GetAttribute("uuid");

        //    //reader.MoveToContent();
        //    //reader.Read();
        //    //while (reader.NodeType == XmlNodeType.Whitespace) reader.Read();

        //    //var observations = new List<IObservation>();

        //    reader.MoveToContent();
        //    //reader.MoveToElement();

        //    // Read to DataItem nodes
        //    if (reader.ReadToDescendant("DataItems"))
        //    {
        //        reader.MoveToContent();
        //        reader.ReadStartElement();
        //        reader.MoveToContent();

        //        //var components = new List<IComponent>();
        //        do
        //        {
        //            var dataItemReader = reader.ReadSubtree();

        //            //var subcomponent = ReadComponentXml(componentReader);
        //            //if (subcomponent != null)
        //            //{
        //            //    components.Add(subcomponent);
        //            //}
        //        }
        //        while (reader.ReadToNextSibling("DataItem"));
        //        //component.Components = components;

        //        reader.ReadEndElement();
        //        //reader.Read();
        //    }

        //    reader.MoveToContent();

        //    //// Read to Component nodes
        //    //reader.ReadStartElement("Components");
        //    //if (reader.NodeType != XmlNodeType.None)
        //    //{
        //    //    var components = new List<IComponent>();
        //    //    do
        //    //    {
        //    //        var componentReader = reader.ReadSubtree();

        //    //        var subcomponent = ReadComponentXml(componentReader);
        //    //        if (subcomponent != null)
        //    //        {
        //    //            components.Add(subcomponent);
        //    //        }
        //    //    }
        //    //    while (reader.Read());
        //    //    //while (reader.ReadToNextSibling("Component"));
        //    //    component.Components = components;
        //    //}

        //    // Read to Component nodes
        //    if (reader.Name == "Components")
        //    {
        //        reader.MoveToContent();
        //        reader.ReadStartElement();

        //        var components = new List<IComponent>();
        //        do
        //        {
        //            reader.MoveToContent();
        //            if (reader.NodeType == XmlNodeType.Element)
        //            {
        //                var componentReader = reader.ReadSubtree();

        //                var subcomponent = ReadComponentXml(componentReader);
        //                if (subcomponent != null)
        //                {
        //                    components.Add(subcomponent);
        //                }
        //            }
        //        }
        //        while (reader.Read());
        //        component.Components = components;

        //        if (reader.NodeType != XmlNodeType.None) reader.ReadEndElement();
        //    }
        //    //if (reader.ReadToDescendant("Components"))
        //    //{
        //    //    var components = new List<IComponent>();
        //    //    do
        //    //    {
        //    //        var componentReader = reader.ReadSubtree();

        //    //        var subcomponent = ReadComponentXml(componentReader);
        //    //        if (subcomponent != null)
        //    //        {
        //    //            components.Add(subcomponent);
        //    //        }
        //    //    }
        //    //    while (reader.Read());
        //    //    //while (reader.ReadToNextSibling("Component"));
        //    //    component.Components = components;
        //    //}

        //    //// Read to Component node
        //    //if (reader.ReadToDescendant("Components"))
        //    //{
        //    //    var components = new List<IComponent>();
        //    //    do
        //    //    {
        //    //        var componentReader = reader.ReadSubtree();

        //    //        var subcomponent = ReadComponentXml(componentReader);
        //    //        if (subcomponent != null)
        //    //        {
        //    //            components.Add(subcomponent);
        //    //        }
        //    //    }
        //    //    while (reader.ReadToNextSibling("Component"));
        //    //    component.Components = components;
        //    //}

        //    //do
        //    //{
        //    //switch (reader.LocalName)
        //    //{
        //    //    case "Samples":
        //    //        var samplesReader = reader.ReadSubtree();
        //    //        observations.AddRange(ReadObservationsXml(samplesReader, DataItemCategory.SAMPLE));
        //    //        break;

        //    //    case "Events":
        //    //        var eventsReader = reader.ReadSubtree();
        //    //        observations.AddRange(ReadObservationsXml(eventsReader, DataItemCategory.EVENT));
        //    //        break;

        //    //    case "Condition":
        //    //        var conditionReader = reader.ReadSubtree();
        //    //        observations.AddRange(ReadObservationsXml(conditionReader, DataItemCategory.CONDITION));
        //    //        break;
        //    //}
        //    //}
        //    //while (reader.Read());

        //    //componentStream.Observations = observations;

        //    return component;



        //    //reader.Read();
        //    //reader.MoveToContent();
        //    //reader.Read();
        //    ////var elementName = reader.LocalName;
        //    //if (reader.ReadToDescendant("Component"))
        //    //{




        //    //    var component = new Component();
        //    //    component.Type = RemoveNamespacePrefix(elementName);
        //    //    component.Id = reader.GetAttribute("id");
        //    //    component.Name = reader.GetAttribute("name");
        //    //    component.Uuid = reader.GetAttribute("uuid");

        //    //    reader.MoveToContent();
        //    //    reader.Read();
        //    //    while (reader.NodeType == XmlNodeType.Whitespace) reader.Read();

        //    //    //var observations = new List<IObservation>();

        //    //    do
        //    //    {
        //    //        //switch (reader.LocalName)
        //    //        //{
        //    //        //    case "Samples":
        //    //        //        var samplesReader = reader.ReadSubtree();
        //    //        //        observations.AddRange(ReadObservationsXml(samplesReader, DataItemCategory.SAMPLE));
        //    //        //        break;

        //    //        //    case "Events":
        //    //        //        var eventsReader = reader.ReadSubtree();
        //    //        //        observations.AddRange(ReadObservationsXml(eventsReader, DataItemCategory.EVENT));
        //    //        //        break;

        //    //        //    case "Condition":
        //    //        //        var conditionReader = reader.ReadSubtree();
        //    //        //        observations.AddRange(ReadObservationsXml(conditionReader, DataItemCategory.CONDITION));
        //    //        //        break;
        //    //        //}
        //    //    }
        //    //    while (reader.Read());

        //    //    //componentStream.Observations = observations;

        //    //    return component;
        //    //}

        //    return null;
        //}

        public static void WriteXml(
            XmlWriter writer,
            IDevicesResponseDocument document,
            bool indentOutput = true,
            bool outputComments = false,
            string styleSheet = null,
            IEnumerable<NamespaceConfiguration> extendedSchemas = null
            )
        {
            if (document != null && !document.Devices.IsNullOrEmpty())
            {
                var ns = Namespaces.GetDevices(document.Version.Major, document.Version.Minor);

                writer.WriteStartDocument();
                if (indentOutput) writer.WriteWhitespace(XmlFunctions.NewLine);

                // Add Stylesheet
                if (!string.IsNullOrEmpty(styleSheet))
                {
                    writer.WriteRaw($"<?xml-stylesheet type=\"text/xsl\" href=\"{styleSheet}?version={document.Version}\"?>");
                    if (indentOutput) writer.WriteWhitespace(XmlFunctions.NewLine);
                }

                // Add Header Comment
                if (outputComments) XmlFunctions.WriteHeaderComment(writer, indentOutput);

                // Write Root Document Element
                writer.WriteStartElement("MTConnectDevices", ns);

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
                XmlDevicesHeader.WriteXml(writer, document.Header);

                // Write Devices
                writer.WriteStartElement("Devices");
                foreach (var device in document.Devices)
                {
                    XmlDevice.WriteXml(writer, device, outputComments);
                }
                writer.WriteEndElement(); // Devices

                writer.WriteEndElement(); // MTConnectDevices
                writer.WriteEndDocument();
                writer.Flush();
            }
        }

        private static string RemoveNamespacePrefix(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                if (type.Contains(':'))
                {
                    return type.Substring(type.IndexOf(':') + 1);
                }

                return type;
            }

            return null;
        }
    }
}