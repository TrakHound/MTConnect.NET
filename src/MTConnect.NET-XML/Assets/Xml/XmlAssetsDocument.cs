// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Headers;
using MTConnect.Writers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml
{
    /// <summary>
    /// The Asset Information Model associates each electronic MTConnectAssets document with a unique
    /// identifier and allows for some predefined mechanisms to find, create, request, updated, and delete these
    /// electronic documents in a way that provides for consistency across multiple pieces of equipment.
    /// </summary>
    [XmlRoot("MTConnectAssets")]
    public class XmlAssetsResponseDocument
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlAssetsResponseDocument));


        /// <summary>
        /// Contains the Header information in an MTConnect Assets XML document
        /// </summary>
        [XmlElement("Header")]
        public MTConnectAssetsHeader Header { get; set; }

        /// <summary>
        /// An XML container that consists of one or more types of Asset XML elements.
        /// </summary>
        [XmlElement("Assets")]
        public XmlAssetCollection AssetCollection { get; set; }


        public XmlAssetsResponseDocument() { }

        public XmlAssetsResponseDocument(IAssetsResponseDocument document)
        {
            if (document != null)
            {
                if (document.Header != null)
                {
                    var header = new MTConnectAssetsHeader();
                    header.AssetBufferSize = document.Header.AssetBufferSize;
                    header.Sender = document.Header.Sender;
                    header.Version = document.Header.Version;
                    header.AssetCount = document.Header.AssetCount;
                    header.CreationTime = document.Header.CreationTime;
                    header.DeviceModelChangeTime = document.Header.DeviceModelChangeTime;
                    header.InstanceId = document.Header.InstanceId;
                    header.TestIndicator = document.Header.TestIndicator;
                    Header = header;
                }

                // Add Assets
                if (!document.Assets.IsNullOrEmpty())
                {
                    AssetCollection = new XmlAssetCollection
                    {
                        Assets = document.Assets.ToList()
                    };
                }
                else AssetCollection = new XmlAssetCollection();
            }
        }


        public AssetsResponseDocument ToAssetsDocument()
        {
            var assetsDocument = new AssetsResponseDocument();

            assetsDocument.Header = Header;

            // Add Assets
            if (AssetCollection != null && !AssetCollection.Assets.IsNullOrEmpty())
            {
                assetsDocument.Assets = AssetCollection.Assets.ToList();
            }
            else assetsDocument.Assets = new List<IAsset>();

            return assetsDocument;
        }


        public static AssetsResponseDocument FromXml(string xml)
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
                            var doc = (XmlAssetsResponseDocument)_serializer.Deserialize(xmlReader);
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

        public static string ToXml(IAssetsResponseDocument document, bool indent = false, bool outputComments = false)
        {
            if (document != null && document.Header != null)
            {
                try
                {
                    var ns = Namespaces.GetAssets(document.Version.Major, document.Version.Minor);
                    var schemaLocation = Schemas.GetAssets(document.Version.Major, document.Version.Minor);

                    using (var writer = new Utf8Writer())
                    {
                        _serializer.Serialize(writer, new XmlAssetsResponseDocument(document));

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

                        if (outputComments)
                        {
                            // Specify Xml Delcaration
                            var xmlDeclaration = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";

                            // Remove Xml Declaration (in order to add Header Comment)
                            xml = xml.Replace(xmlDeclaration, "");

                            // Add Header Comments
                            xml = xmlDeclaration + XmlFunctions.CreateHeaderComment() + xml;
                        }

                        return XmlFunctions.FormatXml(xml, indent, outputComments);
                    }
                }
                catch { }
            }

            return null;
        }
    }
}
