// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Agents.Configuration;
using MTConnect.Headers;
using MTConnect.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Streams.Xml
{
    /// <summary>
    /// The Streams Information Model provides a representation of the data reported by a piece of equipment used for a manufacturing process, or used for any other purpose.
    /// </summary>
    [XmlRoot("MTConnectStreams")]
    public class XmlStreamsResponseDocument
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlStreamsResponseDocument));


        /// <summary>
        /// Contains the Header information in an MTConnect Streams XML document
        /// </summary>
        [XmlElement("Header")]
        public MTConnectStreamsHeader Header { get; set; }

        /// <summary>
        /// Streams is a container type XML element used to group the data reported from one or more pieces of equipment into a single XML document.
        /// </summary>
        [XmlElement("Streams")]
        public XmlStreamsContainer Streams { get; set; }

        [XmlIgnore]
        public Version Version { get; set; }


        public XmlStreamsResponseDocument() { }

        public XmlStreamsResponseDocument(IStreamsResponseDocument streamsDocument)
        {
            if (streamsDocument != null)
            {
                Header = streamsDocument.Header as MTConnectStreamsHeader;
                Version = streamsDocument.Version;

                var xmlStreams = new List<XmlDeviceStream>();
                if (!streamsDocument.Streams.IsNullOrEmpty())
                {
                    foreach (var stream in streamsDocument.Streams)
                    {
                        var xmlStream = new XmlDeviceStream(stream);
                        if (xmlStream != null) xmlStreams.Add(xmlStream);
                    }
                }

                Streams = new XmlStreamsContainer { DeviceStreams = xmlStreams };
            }
        }


        public IStreamsResponseDocument ToStreamsResponseDocument()
        {
            var streamsDocument = new StreamsResponseDocument();

            streamsDocument.Header = Header;
            streamsDocument.Version = Version;
            
            if (Streams != null && !Streams.DeviceStreams.IsNullOrEmpty())
            {
                var deviceStreams = new List<IDeviceStream>();

                foreach (var xmlStream in Streams.DeviceStreams)
                {
                    deviceStreams.Add(xmlStream.ToDeviceStream());
                }

                streamsDocument.Streams = deviceStreams;
            }

            return streamsDocument;
        }


        public static IStreamsResponseDocument FromXml(string xml)
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
                            var doc = (XmlStreamsResponseDocument)_serializer.Deserialize(xmlReader);
                            if (doc != null)
                            {
                                doc.Version = version;
                                return doc.ToStreamsResponseDocument();
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static string ToXml(IStreamsResponseDocument streamsDocument, bool indent = false)
        {
            return ToXml(streamsDocument, null, indent);
        }

        public static string ToXml(IStreamsResponseDocument streamsDocument, IEnumerable<NamespaceConfiguration> extendedSchemas, bool indent = true, bool outputComments = false)
        {
            if (streamsDocument != null && streamsDocument.Header != null)
            {
                try
                {
                    var ns = Namespaces.GetStreams(streamsDocument.Version.Major, streamsDocument.Version.Minor);
                    var schemaLocation = Schemas.GetStreams(streamsDocument.Version.Major, streamsDocument.Version.Minor);
                    var extendedNamespaces = "";

                    using (var textWriter = new Utf8Writer())
                    {
                        textWriter.NewLine = "\r\n";

                        _serializer.Serialize(textWriter, new XmlStreamsResponseDocument(streamsDocument));

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

                        return XmlFunctions.FormatXml(xml, indent, outputComments);
                    }
                }
                catch { }
            }

            return null;
        }
    }
}
