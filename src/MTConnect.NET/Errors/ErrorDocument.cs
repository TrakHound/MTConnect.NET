// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Errors
{
    [XmlRoot("MTConnectError")]
    public class ErrorDocument
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(ErrorDocument));

        public ErrorDocument() { }


        [XmlElement("Header")]
        [JsonPropertyName("header")]
        public Headers.MTConnectErrorHeader Header { get; set; }

        [XmlArray("Errors")]
        [JsonPropertyName("errors")]
        public List<Error> Errors { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Version Version { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public object UserObject { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public string Url { get; set; }


        public static ErrorDocument Create(string xml)
        {
            try
            {
                xml = xml.Trim();

                var version = MTConnectVersion.Get(xml);

                using (var textReader = new StringReader(Namespaces.Clear(xml)))
                {
                    using (var xmlReader = XmlReader.Create(textReader))
                    {
                        var doc = (ErrorDocument)_serializer.Deserialize(xmlReader);
                        if (doc != null)
                        {
                            doc.Version = version;
                            return doc;
                        }
                    }
                }
            }
            catch { }

            return null;
        }


        public static ErrorDocument FromXml(string xml)
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
                            var doc = (ErrorDocument)_serializer.Deserialize(xmlReader);
                            if (doc != null)
                            {
                                doc.Version = version;
                                return doc;
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static ErrorDocument FromJson(string json)
        {
            return JsonFunctions.FromJson<ErrorDocument>(json);
        }


        public string ToXml(bool indent = false)
        {
            if (Header != null)
            {
                try
                {
                    var ns = Namespaces.GetError(Version.Major, Version.Minor);
                    var schemaLocation = Schemas.GetError(Version.Major, Version.Minor);

                    using (var writer = new StringWriter())
                    {
                        _serializer.Serialize(writer, this);

                        var xml = writer.ToString();

                        // Remove the XSD namespace
                        string regex = @"\s{1}xmlns:xsi=\""http:\/\/www\.w3\.org\/2001\/XMLSchema-instance\""\s{1}xmlns:xsd=\""http:\/\/www\.w3\.org\/2001\/XMLSchema\""";
                        xml = Regex.Replace(xml, regex, "");
                        regex = @"\s{1}xmlns:xsd=\""http:\/\/www\.w3\.org\/2001\/XMLSchema\""\s{1}xmlns:xsi=\""http:\/\/www\.w3\.org\/2001\/XMLSchema-instance\""";
                        xml = Regex.Replace(xml, regex, "");

                        // Add the default namespace, "m" namespace, xsi, and schemaLocation
                        regex = @"<MTConnectError";
                        string replace = "<MTConnectError xmlns:m=\"" + ns + "\" xmlns=\"" + ns + "\" xmlns:xsi=\"" + Namespaces.DefaultXmlSchemaInstance + "\" xsi:schemaLocation=\"" + schemaLocation + "\"";
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
            return JsonFunctions.ToJson(this, indent);
        }
    }
}
