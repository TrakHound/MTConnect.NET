// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets
{
    /// <summary>
    /// The Asset Information Model associates each electronic MTConnectAssets document with a unique
    /// identifier and allows for some predefined mechanisms to find, create, request, updated, and delete these
    /// electronic documents in a way that provides for consistency across multiple pieces of equipment.
    /// </summary>
    public class AssetsDocument
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(Xml.XmlAssetsDocument));


        /// <summary>
        /// Contains the Header information in an MTConnect Assets XML document
        /// </summary>
        [JsonPropertyName("header")]
        public Headers.MTConnectAssetsHeader Header { get; set; }

        /// <summary>
        /// An XML container that consists of one or more types of Asset XML elements.
        /// </summary>
        [JsonPropertyName("assets")]
        public List<IAsset> Assets { get; set; }

        [JsonIgnore]
        public Version Version { get; set; }

        [JsonIgnore]
        public object UserObject { get; set; }

        [JsonIgnore]
        public string Url { get; set; }

        [JsonIgnore]
        public string Xml { get; set; }


        public static AssetsDocument Create(string xml)
        {
            try
            {
                xml = xml.Trim();

                var version = MTConnectVersion.Get(xml);

                using (var textReader = new StringReader(Namespaces.Clear(xml)))
                {
                    using (var xmlReader = XmlReader.Create(textReader))
                    {
                        var doc = (AssetsDocument)_serializer.Deserialize(xmlReader);
                        if (doc != null)
                        {
                            doc.Version = version;
                            doc.Xml = xml;
                            return doc;
                        }
                    }
                }
            }
            catch { }

            return null;
        }


        public static AssetsDocument FromXml(string xml)
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
                            var doc = (Xml.XmlAssetsDocument)_serializer.Deserialize(xmlReader);
                            if (doc != null)
                            {
                                return doc.ToAssetsDocument();
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static AssetsDocument FromJson(string json)
        {
            var jsonDocument = JsonFunctions.FromJson<Json.JsonAssetsDocument>(json);
            if (jsonDocument != null)
            {
                return jsonDocument.ToAssetsDocument();
            }

            return null;
        }


        public string ToXml(bool indent = false)
        {
            if (Header != null)
            {
                try
                {
                    var ns = Namespaces.GetAssets(Version.Major, Version.Minor);
                    var schemaLocation = Schemas.GetAssets(Version.Major, Version.Minor);

                    using (var writer = new StringWriter())
                    {
                        _serializer.Serialize(writer, new Xml.XmlAssetsDocument(this));

                        var xml = writer.ToString();

                        // Remove the XSD namespace
                        string regex = @"\s{1}xmlns:xsi=\""http:\/\/www\.w3\.org\/2001\/XMLSchema-instance\""\s{1}xmlns:xsd=\""http:\/\/www\.w3\.org\/2001\/XMLSchema\""";
                        xml = Regex.Replace(xml, regex, "");
                        regex = @"\s{1}xmlns:xsd=\""http:\/\/www\.w3\.org\/2001\/XMLSchema\""\s{1}xmlns:xsi=\""http:\/\/www\.w3\.org\/2001\/XMLSchema-instance\""";
                        xml = Regex.Replace(xml, regex, "");

                        // Add the default namespace, "m" namespace, xsi, and schemaLocation
                        regex = @"<MTConnectAssets";
                        string replace = "<MTConnectAssets xmlns:m=\"" + ns + "\" xmlns=\"" + ns + "\" xmlns:xsi=\"" + Namespaces.DefaultXmlSchemaInstance + "\" xsi:schemaLocation=\"" + schemaLocation + "\"";
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
            return JsonFunctions.ToJson(new Json.JsonAssetsDocument(this), indent);
        }
    }
}
