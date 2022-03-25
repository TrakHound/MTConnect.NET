// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Agents.Configuration;
using MTConnect.Headers;
using MTConnect.Interfaces;
//using MTConnect.Writers;
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
    public class DevicesResponseDocument : IDevicesResponseDocument
    {
        /// <summary>
        /// An XML container in an MTConnect Response Document that provides information from an Agent
        /// defining version information, storage capacity, and parameters associated with the data management within the Agent.
        /// </summary>
        [XmlElement("Header")]
        [JsonPropertyName("header")]
        public IMTConnectDevicesHeader Header { get; set; }

        /// <summary>
        /// The first, or highest level, Structural Element in a MTConnectDevices document.Devices is a container type XML element.
        /// </summary>
        [XmlIgnore]
        [JsonPropertyName("devices")]
        public virtual IEnumerable<IDevice> Devices { get; set; }


        [XmlArray("Interfaces")]
        [JsonPropertyName("interfaces")]
        public IEnumerable<IInterface> Interfaces { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Version Version { get; set; }


        public IEnumerable<IComponent> GetComponents()
        {
            var components = new List<IComponent>();

            if (!Devices.IsNullOrEmpty())
            {
                foreach (var device in Devices)
                {
                    components.AddRange(device.GetComponents());
                }
            }

            return components;
        }

        public IEnumerable<IDataItem> GetDataItems()
        {
            var dataItems = new List<IDataItem>();

            if (!Devices.IsNullOrEmpty())
            {
                foreach (var device in Devices)
                {
                    dataItems.AddRange(device.GetDataItems());
                }
            }

            return dataItems;
        }

        //public static DevicesResponseDocument FromXml(string xml)
        //{
        //    if (!string.IsNullOrEmpty(xml))
        //    {
        //        try
        //        {
        //            xml = xml.Trim();

        //            var version = MTConnectVersion.Get(xml);

        //            using (var textReader = new StringReader(Namespaces.Clear(xml)))
        //            {
        //                using (var xmlReader = XmlReader.Create(textReader))
        //                {
        //                    var xmlDocument = (Xml.XmlDevicesResponseDocument)_serializer.Deserialize(xmlReader);
        //                    if (xmlDocument != null)
        //                    {
        //                        var document = xmlDocument.ToDocument();
        //                        document.Version = version;
        //                        return document;
        //                    }
        //                }
        //            }  
        //        }
        //        catch { }
        //    }

        //    return null;
        //}

        //public static DevicesResponseDocument FromJson(string json)
        //{
        //    return JsonFunctions.FromJson<DevicesResponseDocument>(json);
        //}


        //public string ToXml(bool indent = false)
        //{
        //    return ToXml(this, null, indent);
        //}

        //public string ToXml(IEnumerable<NamespaceConfiguration> extendedSchemas, bool indent = false)
        //{
        //    return ToXml(this, null, indent);
        //}

        //public static string ToXml(IDevicesResponseDocument document, IEnumerable<NamespaceConfiguration> extendedSchemas, bool indent = false)
        //{
        //    if (document != null && document.Header != null)
        //    {
        //        try
        //        {
        //            var ns = Namespaces.GetDevices(document.Version.Major, document.Version.Minor);
        //            var schemaLocation = Schemas.GetDevices(document.Version.Major, document.Version.Minor);
        //            var extendedNamespaces = "";

        //            using (var writer = new Utf8Writer())
        //            {
        //                _serializer.Serialize(writer, new Xml.XmlDevicesResponseDocument(document));

        //                var xml = writer.ToString();

        //                // Remove the XSD namespace
        //                string regex = @"\s{1}xmlns:xsi=\""http:\/\/www\.w3\.org\/2001\/XMLSchema-instance\""\s{1}xmlns:xsd=\""http:\/\/www\.w3\.org\/2001\/XMLSchema\""";
        //                xml = Regex.Replace(xml, regex, "");
        //                regex = @"\s{1}xmlns:xsd=\""http:\/\/www\.w3\.org\/2001\/XMLSchema\""\s{1}xmlns:xsi=\""http:\/\/www\.w3\.org\/2001\/XMLSchema-instance\""";
        //                xml = Regex.Replace(xml, regex, "");

        //                // Add the default namespace, "m" namespace, xsi, and schemaLocation
        //                regex = @"<MTConnectDevices";

        //                // Add Extended Schemas
        //                if (!extendedSchemas.IsNullOrEmpty())
        //                {
        //                    schemaLocation = "";

        //                    foreach (var extendedSchema in extendedSchemas)
        //                    {
        //                        extendedNamespaces += $@" xmlns:{extendedSchema.Alias}=""{extendedSchema.Location}""";
        //                        schemaLocation += $"{extendedSchema.Location} {extendedSchema.Path}";
        //                    }
        //                }

        //                string replace = "<MTConnectDevices xmlns:m=\"" + ns + "\" xmlns=\"" + ns + "\" xmlns:xsi=\"" + Namespaces.DefaultXmlSchemaInstance + "\"" + extendedNamespaces + " xsi:schemaLocation=\"" + schemaLocation + "\"";
        //                xml = Regex.Replace(xml, regex, replace);

        //                if (indent) return XmlFunctions.IndentXml(xml);
        //                else return xml;
        //            }
        //        }
        //        catch { }
        //    }

        //    return null;
        //}

        //public string ToJson(bool indent = false)
        //{
        //    return ToJson(this, indent);
        //}

        //public static string ToJson(IDevicesResponseDocument document, bool indent = false)
        //{
        //    return JsonFunctions.ToJson(new Json.JsonDevicesDocument(document), indent);
        //}
    }
}
