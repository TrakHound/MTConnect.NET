// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.ComponentConfigurationParameters;
using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.Files;
using MTConnect.Assets.QIF;
using MTConnect.Assets.RawMaterials;
using MTConnect.Assets.Xml.CuttingTools;
using MTConnect.Assets.Xml.Files;
using MTConnect.Assets.Xml.RawMaterials;
using System;
using System.Collections.Generic;
using System.IO;
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


        [XmlArray("Assets")]
        [XmlArrayItem(typeof(XmlComponentConfigurationParametersAsset), ElementName = ComponentConfigurationParametersAsset.TypeId)]
        [XmlArrayItem(typeof(XmlCuttingToolAsset), ElementName = CuttingToolAsset.TypeId)]
        [XmlArrayItem(typeof(XmlFileAsset), ElementName = FileAsset.TypeId)]
        [XmlArrayItem(typeof(XmlQIFDocumentWrapperAsset), ElementName = QIFDocumentWrapperAsset.TypeId)]
        [XmlArrayItem(typeof(XmlRawMaterialAsset), ElementName = RawMaterialAsset.TypeId)]
        public List<object> Assets { get; set; }

        //[XmlElement("Assets")]
        //public XmlAssetCollection AssetCollection { get; set; }

        [XmlIgnore]
        public Version Version { get; set; }


        public AssetsResponseDocument ToAssetsDocument()
        {
            var assetsDocument = new AssetsResponseDocument();
            assetsDocument.Header = Header.ToErrorHeader();
            assetsDocument.Version = Version;
            //assetsDocument.Assets = Assets;

            if (!Assets.IsNullOrEmpty())
            {
                var assets = new List<IAsset>();

                foreach (var asset in Assets)
                {
                    // ComponentConfigurationParameters
                    if (asset.GetType() ==  typeof(XmlComponentConfigurationParametersAsset))
                    {
                        var componentConfigurationParametersAsset = ((XmlComponentConfigurationParametersAsset)asset).ToAsset();
                        if (componentConfigurationParametersAsset != null) assets.Add(componentConfigurationParametersAsset);
                    }

                    // CuttingTool
                    if (asset.GetType() == typeof(XmlCuttingToolAsset))
                    {
                        var cuttingToolAsset = ((XmlCuttingToolAsset)asset).ToAsset();
                        if (cuttingToolAsset != null) assets.Add(cuttingToolAsset);
                    }

                    // File
                    if (asset.GetType() == typeof(XmlFileAsset))
                    {
                        var fileAsset = ((XmlFileAsset)asset).ToAsset();
                        if (fileAsset != null) assets.Add(fileAsset);
                    }

                    // QIF
                    if (asset.GetType() == typeof(XmlQIFDocumentWrapperAsset))
                    {
                        var qifAsset = ((XmlQIFDocumentWrapperAsset)asset).ToAsset();
                        if (qifAsset != null) assets.Add(qifAsset);
                    }

                    // RawMaterial
                    if (asset.GetType() == typeof(XmlRawMaterialAsset))
                    {
                        var rawMaterialAsset = ((XmlRawMaterialAsset)asset).ToAsset();
                        if (rawMaterialAsset != null) assets.Add(rawMaterialAsset);
                    }
                }

                assetsDocument.Assets = assets;
            }

            //// Add Assets
            //if (AssetCollection != null && !AssetCollection.Assets.IsNullOrEmpty())
            //{
            //    assetsDocument.Assets = AssetCollection.Assets.ToList();
            //}
            //else assetsDocument.Assets = new List<IAsset>();

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
                    var version = MTConnectVersion.Get(xml);

                    xml = xml.Trim();
                    xml = Namespaces.Clear(xml);

                    using (var textReader = new StringReader(xml))
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