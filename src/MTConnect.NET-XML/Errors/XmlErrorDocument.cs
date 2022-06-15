// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Headers;
using MTConnect.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Errors
{
    /// <summary>
    /// The Error Information Model establishes the rules and terminology that describes the Response Document
    /// returned by an Agent when it encounters an error while interpreting a Request for information from a client
    /// software application or when an Agent experiences an error while publishing the Response to a Request for information.
    /// </summary>
    [XmlRoot("MTConnectError")]
    public class XmlErrorResponseDocument
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlErrorResponseDocument));


        /// <summary>
        /// The Header element for an MTConnectErrors Response Document defines information regarding
        /// the creation of the document and the data storage capability of the Agent that generated the document.
        /// </summary>
        [XmlElement("Header")]
        [JsonPropertyName("header")]
        public MTConnectErrorHeader Header { get; set; }

        /// <summary>
        /// An XML container element in an MTConnectErrors Response Document provided by an Agent when an error
        /// is encountered associated with a Request for information from a client software application.
        /// </summary>
        [XmlArray("Errors")]
        [JsonPropertyName("errors")]
        public List<Error> Errors { get; set; }

        /// <summary>
        /// The MTConnect Version of the Response document
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public Version Version { get; set; }


        public XmlErrorResponseDocument() { }

        public XmlErrorResponseDocument(IErrorResponseDocument document)
        {
            if (document != null)
            {
                if (document.Header != null)
                {
                    var header = new MTConnectErrorHeader();
                    header.AssetBufferSize = document.Header.AssetBufferSize;
                    header.Sender = document.Header.Sender;
                    header.Version = document.Header.Version;
                    header.AssetCount = document.Header.AssetCount;
                    header.CreationTime = document.Header.CreationTime;
                    header.DeviceModelChangeTime = document.Header.DeviceModelChangeTime;
                    header.InstanceId = document.Header.InstanceId;
                    header.TestIndicator = document.Header.TestIndicator;
                    header.BufferSize = document.Header.BufferSize;
                    Header = header;
                }

                if (!document.Errors.IsNullOrEmpty())
                {
                    var errors = new List<Error>();

                    foreach (var error in document.Errors)
                    {
                        errors.Add(new Error(error.ErrorCode, error.CDATA));
                    }

                    Errors = errors;
                }

                Version = document.Version;
            }
        }


        public static ErrorResponseDocument Create(string xml)
        {
            try
            {
                xml = xml.Trim();

                var version = MTConnectVersion.Get(xml);

                using (var textReader = new StringReader(Namespaces.Clear(xml)))
                {
                    using (var xmlReader = XmlReader.Create(textReader))
                    {
                        var doc = (ErrorResponseDocument)_serializer.Deserialize(xmlReader);
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


        public static ErrorResponseDocument FromXml(string xml)
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
                            var doc = (ErrorResponseDocument)_serializer.Deserialize(xmlReader);
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


        public static string ToXml(
            IErrorResponseDocument document,
            string styleSheet = null,
            bool indent = false,
            bool outputComments = false
            )
        {
            if (document != null && document.Header != null)
            {
                try
                {
                    var ns = Namespaces.GetError(document.Version.Major, document.Version.Minor);
                    var schemaLocation = Schemas.GetError(document.Version.Major, document.Version.Minor);

                    using (var writer = new Utf8Writer())
                    {
                        _serializer.Serialize(writer, new XmlErrorResponseDocument(document));

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
