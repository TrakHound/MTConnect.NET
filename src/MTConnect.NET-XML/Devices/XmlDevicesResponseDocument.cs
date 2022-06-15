// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Configurations;
using MTConnect.Writers;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices
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


        public XmlDevicesResponseDocument() { }

        public XmlDevicesResponseDocument(IDevicesResponseDocument document)
        {
            if (document != null)
            {
                Header = new XmlDevicesHeader(document.Header);

                if (!document.Devices.IsNullOrEmpty())
                {
                    var devices = new List<XmlDevice>();

                    foreach (var device in document.Devices)
                    {
                        if (device.Type == Device.TypeId) devices.Add(new XmlDevice(device));
                        else if (device.Type == Agent.TypeId) devices.Add(new XmlAgent(device));
                    }

                    Devices = devices;
                }

                //Interfaces = document.Interfaces;
            }
        }


        public DevicesResponseDocument ToDocument()
        {
            var document = new DevicesResponseDocument();

            if (Header != null) document.Header = Header.ToDevicesHeader();

            if (!Devices.IsNullOrEmpty())
            {
                var devices = new List<IDevice>();

                foreach (var device in Devices) devices.Add(device.ToDevice());

                document.Devices = devices;
            }

            //document.Interfaces = Interfaces;

            return document;
        }


        public static IDevicesResponseDocument FromXml(string xml)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                try
                {
                    xml = xml.Trim();

                    var version = MTConnectVersion.Get(xml);

                    using (var textReader = new StringReader(Namespaces.Clear(xml)))
                    {
                        using (var xmlReader = XmlReader.Create(textReader))
                        {
                            var xmlDocument = (XmlDevicesResponseDocument)_serializer.Deserialize(xmlReader);
                            if (xmlDocument != null)
                            {
                                var document = xmlDocument.ToDocument();
                                document.Version = version;
                                return document;
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static string ToXml(
            IDevicesResponseDocument document, 
            IEnumerable<NamespaceConfiguration> extendedSchemas = null,
            string styleSheet = null,
            bool indent = false,
            bool outputComments = false
            )
        {
            if (document != null && document.Header != null)
            {
                try
                {
                    var ns = Namespaces.GetDevices(document.Version.Major, document.Version.Minor);
                    var schemaLocation = Schemas.GetDevices(document.Version.Major, document.Version.Minor);
                    var extendedNamespaces = "";

                    using (var writer = new Utf8Writer())
                    {
                        _serializer.Serialize(writer, new XmlDevicesResponseDocument(document));

                        var xml = writer.ToString();

                        // Remove the XSD namespace
                        string regex = @"\s{1}xmlns:xsi=\""http:\/\/www\.w3\.org\/2001\/XMLSchema-instance\""\s{1}xmlns:xsd=\""http:\/\/www\.w3\.org\/2001\/XMLSchema\""";
                        xml = Regex.Replace(xml, regex, "");
                        regex = @"\s{1}xmlns:xsd=\""http:\/\/www\.w3\.org\/2001\/XMLSchema\""\s{1}xmlns:xsi=\""http:\/\/www\.w3\.org\/2001\/XMLSchema-instance\""";
                        xml = Regex.Replace(xml, regex, "");

                        // Add the default namespace, "m" namespace, xsi, and schemaLocation
                        regex = @"<MTConnectDevices";

                        // Add Extended Schemas
                        if (!extendedSchemas.IsNullOrEmpty())
                        {
                            schemaLocation = "";

                            foreach (var extendedSchema in extendedSchemas)
                            {
                                extendedNamespaces += $@" xmlns:{extendedSchema.Alias}=""{extendedSchema.Location}""";
                                schemaLocation += $"{extendedSchema.Location} {extendedSchema.Path}";
                            }
                        }

                        string replace = "<MTConnectDevices xmlns:m=\"" + ns + "\" xmlns=\"" + ns + "\" xmlns:xsi=\"" + Namespaces.DefaultXmlSchemaInstance + "\"" + extendedNamespaces + " xsi:schemaLocation=\"" + schemaLocation + "\"";
                        xml = Regex.Replace(xml, regex, replace);

                        // Specify Xml Delcaration
                        var xmlDeclaration = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";

                        // Remove Xml Declaration (in order to add Header Comment)
                        xml = xml.Replace(xmlDeclaration, "");

                        // Add Header Comments
                        if (outputComments)
                        {
                            xml = XmlFunctions.CreateHeaderComment() + xml;
                        }

                        // Add Stylesheet
                        if (!string.IsNullOrEmpty(styleSheet))
                        {
                            xml = $"<?xml-stylesheet type=\"text/xsl\" href=\"{styleSheet}?version={document.Version}\"?>" + xml;
                        }

                        xml = xmlDeclaration + xml;

                        return XmlFunctions.FormatXml(xml, indent, outputComments);
                    }
                }
                catch { }
            }

            return null;
        }
    }
}
