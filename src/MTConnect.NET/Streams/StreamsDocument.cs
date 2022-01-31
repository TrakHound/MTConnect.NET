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

namespace MTConnect.Streams
{
    /// <summary>
    /// The Streams Information Model provides a representation of the data reported by a piece of equipment used for a manufacturing process, or used for any other purpose.
    /// </summary>
    [XmlRoot("MTConnectStreams")]
    public class StreamsDocument
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(Xml.XmlStreamsDocument));


        /// <summary>
        /// Contains the Header information in an MTConnect Streams XML document
        /// </summary>
        [XmlElement("Header")]
        [JsonPropertyName("header")]
        public Headers.MTConnectStreamsHeader Header { get; set; }

        /// <summary>
        /// Streams is a container type XML element used to group the data reported from one or more pieces of equipment into a single XML document.
        /// </summary>
        [XmlArray("Streams")]
        [XmlArrayItem("DeviceStream")]
        [JsonPropertyName("streams")]
        public List<DeviceStream> Streams { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Version Version { get; set; }


        public IEnumerable<IDataItem> GetDataItems()
        {
            if (!Streams.IsNullOrEmpty())
            {
                var dataItems = new List<IDataItem>();

                foreach (var stream in Streams)
                {
                    dataItems.AddRange(stream.DataItems);
                }

                return dataItems;
            }

            return null;
        }


        public static StreamsDocument FromXml(string xml)
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
                            var doc = (Xml.XmlStreamsDocument)_serializer.Deserialize(xmlReader);
                            if (doc != null)
                            {
                                doc.Version = version;
                                return doc.ToStreamsDocument();
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static StreamsDocument FromJson(string json)
        {
            return JsonFunctions.FromJson<StreamsDocument>(json);
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
                    var ns = Namespaces.GetStreams(Version.Major, Version.Minor);
                    var schemaLocation = Schemas.GetStreams(Version.Major, Version.Minor);
                    var extendedNamespaces = "";

                    using (var textWriter = new Utf8Writer())
                    {
                        _serializer.Serialize(textWriter, new Xml.XmlStreamsDocument(this));

                        var xml = textWriter.ToString();

                        // Remove the XSD namespace
                        string regex = @"\s{1}xmlns:xsi=\""http:\/\/www\.w3\.org\/2001\/XMLSchema-instance\""\s{1}xmlns:xsd=\""http:\/\/www\.w3\.org\/2001\/XMLSchema\""";
                        xml = Regex.Replace(xml, regex, "");
                        regex = @"\s{1}xmlns:xsd=\""http:\/\/www\.w3\.org\/2001\/XMLSchema\""\s{1}xmlns:xsi=\""http:\/\/www\.w3\.org\/2001\/XMLSchema-instance\""";
                        xml = Regex.Replace(xml, regex, "");

                        // Add the default namespace, "m" namespace, xsi, and schemaLocation
                        regex = @"<MTConnectStreams";

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

                        string replace = "<MTConnectStreams xmlns:m=\"" + ns + "\" xmlns=\"" + ns + "\" xmlns:xsi=\"" + Namespaces.DefaultXmlSchemaInstance + "\"" + extendedNamespaces + " xsi:schemaLocation=\"" + schemaLocation + "\"";
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
            return JsonFunctions.ToJson(new Json.JsonStreamsDocument(this), indent);
        }
    }
}
