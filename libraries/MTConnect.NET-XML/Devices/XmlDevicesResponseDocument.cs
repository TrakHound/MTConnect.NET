// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
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
    /// <summary>
    /// XML serialization surrogate for the root <c>MTConnectDevices</c>
    /// response document returned by the agent <c>probe</c> request. Mirrors
    /// the on-the-wire shape so it can be read and written, then converts to
    /// and from the strongly-typed <see cref="DevicesResponseDocument"/> model.
    /// </summary>
    [XmlRoot("MTConnectDevices")]
    public class XmlDevicesResponseDocument
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlDevicesResponseDocument));


        /// <summary>
        /// The document <c>Header</c> describing the agent instance and version.
        /// </summary>
        [XmlElement("Header")]
        public XmlDevicesHeader Header { get; set; }

        /// <summary>
        /// The devices in the document, serialized as <c>Device</c> elements
        /// (or <c>Agent</c> for the self-describing agent device).
        /// </summary>
        [XmlArray("Devices")]
        [XmlArrayItem("Device", typeof(XmlDevice))]
        [XmlArrayItem("Agent", typeof(XmlAgent))]
        public List<XmlDevice> Devices { get; set; }

        //[XmlArray("Interfaces")]
        //public List<Interface> Interfaces { get; set; }

        /// <summary>
        /// The MTConnect schema version detected from the source XML; carried
        /// out of band so the correct namespace can be emitted on write.
        /// </summary>
        [XmlIgnore]
        public Version Version { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="DevicesResponseDocument"/>, converting the header and each
        /// device and stamping every device with the detected schema version.
        /// </summary>
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

        /// <summary>
        /// Deserializes an <c>MTConnectDevices</c> document from raw XML bytes,
        /// sanitizing the input, detecting the schema version, and clearing
        /// namespace prefixes before deserialization.
        /// </summary>
        /// <exception cref="XmlException">XML Exception thrown during Serialization</exception>
        public static IDevicesResponseDocument FromXml(byte[] xmlBytes)
        {
            if (xmlBytes != null && xmlBytes.Length > 0)
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

            return null;
        }

        /// <summary>
        /// Serializes the devices response document to a UTF-8 XML byte array,
        /// choosing indented or compact writer settings and optionally emitting
        /// a stylesheet, header comment, and extended schema namespaces.
        /// </summary>
        /// <exception cref="XmlException">XML Exception thrown during Serialization</exception>
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

            return null;
        }

        /// <summary>
        /// Serializes the devices response document to a new XML stream,
        /// choosing indented or compact writer settings and optionally emitting
        /// a stylesheet, header comment, and extended schema namespaces.
        /// </summary>
        /// <exception cref="XmlException">XML Exception thrown during Serialization</exception>
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

            return null;
        }

        /// <summary>
        /// Writes the complete <c>MTConnectDevices</c> document, emitting the
        /// version-specific namespace, optional extended schema namespaces and
        /// schema location, the header, and each device. No document is written
        /// when the response has no devices.
        /// </summary>
        /// <exception cref="XmlException">XML Exception thrown during Serialization</exception>
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