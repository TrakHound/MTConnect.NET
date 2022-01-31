// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Agents.Configuration;
using MTConnect.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// A document that contains information describing both the physical and logical structure of the piece of equipment
    /// and a detailed description of each Data Entity that can be reported by the Agent associated with the piece of equipment.
    /// </summary>
    [XmlRoot("MTConnectDevices")]
    public class DevicesDocument
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(Xml.XmlDevicesDocument));


        /// <summary>
        /// An XML container in an MTConnect Response Document that provides information from an Agent
        /// defining version information, storage capacity, and parameters associated with the data management within the Agent.
        /// </summary>
        [XmlElement("Header")]
        [JsonPropertyName("header")]
        public Headers.MTConnectDevicesHeader Header { get; set; }

        /// <summary>
        /// The first, or highest level, Structural Element in a MTConnectDevices document.Devices is a container type XML element.
        /// </summary>
        [XmlIgnore]
        [JsonPropertyName("devices")]
        public virtual List<Device> Devices { get; set; }


        [XmlArray("Interfaces")]
        [JsonPropertyName("interfaces")]
        public List<Interfaces.Interface> Interfaces { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Version Version { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public object UserObject { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public string Url { get; set; }


        public static DevicesDocument FromXml(string xml)
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
                            var document = (Xml.XmlDevicesDocument)_serializer.Deserialize(xmlReader);
                            if (document != null)
                            {
                                return document.ToDocument();
                            }
                        }
                    }  
                }
                catch { }
            }

            return null;
        }

        public static DevicesDocument FromJson(string json)
        {
            return JsonFunctions.FromJson<DevicesDocument>(json);
        }


        public string ToXml(bool indent = false)
        {
            return ToXml(null, indent);
        }

        public string ToXml(IEnumerable<NamespaceConfiguration> extendedSchemas, bool indent = false)
        {
            if (Header != null)
            {
                try
                {
                    var ns = Namespaces.GetDevices(Version.Major, Version.Minor);
                    var schemaLocation = Schemas.GetDevices(Version.Major, Version.Minor);
                    var extendedNamespaces = "";

                    using (var writer = new Utf8Writer())
                    {
                        _serializer.Serialize(writer, new Xml.XmlDevicesDocument(this));

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

                        if (indent) return XmlFunctions.IndentXml(xml);
                        else return xml;
                    }
                }
                catch { }
            }

            return null;
        }

        public string ToJson(bool indent = false)
        {
            return JsonFunctions.ToJson(new Json.JsonDevicesDocument(this), indent);
        }
    }
}
