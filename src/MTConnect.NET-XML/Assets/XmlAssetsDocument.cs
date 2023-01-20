// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml
{
    [XmlRoot("MTConnectAssets")]
    public class XmlAssetsResponseDocument
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(XmlAssetsResponseDocument));


        [XmlElement("Header")]
        public XmlAssetsHeader Header { get; set; }

        [XmlElement("Assets")]
        public XmlAssetCollection AssetCollection { get; set; }

        [XmlIgnore]
        public Version Version { get; set; }


        public AssetsResponseDocument ToAssetsDocument()
        {
            var assetsDocument = new AssetsResponseDocument();
            assetsDocument.Header = Header.ToErrorHeader();
            assetsDocument.Version = Version;

            // Add Assets
            if (AssetCollection != null && !AssetCollection.Assets.IsNullOrEmpty())
            {
                assetsDocument.Assets = AssetCollection.Assets.ToList();
            }
            else assetsDocument.Assets = new List<IAsset>();

            return assetsDocument;
        }


        public static AssetsResponseDocument FromXml(byte[] xmlBytes)
        {
            if (xmlBytes != null && xmlBytes.Length > 0)
            {
                try
                {
                    // Clean whitespace and Encoding Marks (BOM)
                    var bytes = XmlFunctions.SanitizeBytes(xmlBytes);

                    var xml = Encoding.UTF8.GetString(bytes);
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

        public static byte[] ToXmlBytes(
            IAssetsResponseDocument document,
            bool indentOutput = false,
            bool outputComments = false,
            string stylesheet = null
            )
        {
            if (document != null && document.Header != null)
            {
                try
                {
                    var mtconnectStreamsNamespace = Namespaces.GetStreams(document.Version.Major, document.Version.Minor);

                    using (var stream = new MemoryStream())
                    {
                        // Set the XmlWriterSettings to use
                        var xmlWriterSettings = indentOutput ? XmlFunctions.XmlWriterSettingsIndent : XmlFunctions.XmlWriterSettings;

                        // Use XmlWriter to write XML to stream
                        using (var xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                        {
                            WriteXml(xmlWriter, document, indentOutput, outputComments, stylesheet);
                            return stream.ToArray();
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static void WriteXml(
            XmlWriter writer,
            IAssetsResponseDocument document,
            bool indentOutput = true,
            bool outputComments = false,
            string stylesheet = null
            )
        {
            if (document != null)
            {
                var ns = Namespaces.GetAssets(document.Version.Major, document.Version.Minor);

                writer.WriteStartDocument();
                if (indentOutput) writer.WriteWhitespace(XmlFunctions.NewLine);

                // Add Stylesheet
                if (!string.IsNullOrEmpty(stylesheet))
                {
                    writer.WriteRaw($"<?xml-stylesheet type=\"text/xsl\" href=\"{stylesheet}?version={document.Version}\"?>");
                    if (indentOutput) writer.WriteWhitespace(XmlFunctions.NewLine);
                }

                // Add Header Comment
                if (outputComments) XmlFunctions.WriteHeaderComment(writer, indentOutput);

                // Write Root Document Element
                writer.WriteStartElement("MTConnectAssets", ns);

                // Write Namespace Declarations
                writer.WriteAttributeString("xmlns", null, null, ns);
                writer.WriteAttributeString("xmlns", "m", null, ns);
                writer.WriteAttributeString("xmlns", "xsi", null, Namespaces.DefaultXmlSchemaInstance);

                // Write Schema Location
                writer.WriteAttributeString("xsi", "schemaLocation", null, Schemas.GetStreams(document.Version.Major, document.Version.Minor));

                // Write Header
                XmlAssetsHeader.WriteXml(writer, document.Header);

                writer.WriteStartElement("Assets");

                if (!document.Assets.IsNullOrEmpty())
                {
                    // Write Assets
                    new XmlAssetCollection(document.Assets, indentOutput).WriteXml(writer);
                }

                writer.WriteEndElement(); // Assets
                writer.WriteEndElement(); // MTConnectAssets
                writer.WriteEndDocument();
                writer.Flush();
            }
        }
    }
}
