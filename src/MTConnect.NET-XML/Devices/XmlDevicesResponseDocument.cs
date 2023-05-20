// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("MTConnectDevices")]
    public class XmlDevicesResponseDocument
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlDevicesResponseDocument));


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

        public static IDevicesResponseDocument FromXml(byte[] xmlBytes)
        {
            if (xmlBytes != null && xmlBytes.Length > 0)
            {
                try
                {
                    // Clean whitespace and Encoding Marks (BOM)
                    var bytes = XmlFunctions.SanitizeBytes(xmlBytes);

                    var xml = Encoding.UTF8.GetString(bytes);
                    var version = MTConnectVersion.Get(xml);

                    xml = xml.Trim();
                    xml = Namespaces.Clear(xml);

                    using (var textReader = new StringReader(xml))
                    {
                        using (var xmlReader = XmlReader.Create(textReader))
                        {
                            var xmlDocument = (XmlDevicesResponseDocument)_serializer.Deserialize(xmlReader);
                            if (xmlDocument != null)
                            {
                                xmlDocument.Version = version;
                                var document = xmlDocument.ToDocument();
                                return document;
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

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
    }
}