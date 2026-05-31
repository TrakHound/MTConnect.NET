// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Xml;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.QIF
{
    /// <summary>
    /// XML serialization surrogate for a <c>QIFDocumentWrapper</c> asset, which
    /// embeds a foreign QIF (Quality Information Framework) document inside an
    /// MTConnect asset envelope.
    /// </summary>
    [XmlRoot("QIFDocumentWrapper")]
    public class XmlQIFDocumentWrapperAsset : XmlAsset
    {
        /// <summary>
        /// The QIF document classification, carried by the
        /// <c>qifDocumentType</c> attribute.
        /// </summary>
        [XmlAttribute("qifDocumentType")]
        public string QIFDocumentType { get; set; }

        /// <summary>
        /// The embedded QIF document, captured verbatim as raw XML.
        /// </summary>
        public XmlQIFDocument QIFDocument { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="QIFDocumentWrapperAsset"/>, copying the shared asset
        /// fields, mapping the document type to its enumeration, and copying
        /// the embedded QIF markup.
        /// </summary>
        public override IAsset ToAsset()
        {
            var asset = new QIFDocumentWrapperAsset();

            asset.AssetId = AssetId;
            asset.Timestamp = Timestamp;
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;

            //if (Description != null) asset.Description = Description.ToDescription();

            asset.QifDocumentType = QIFDocumentType.ConvertEnum<QIFDocumentType>();

            if (QIFDocument != null) asset.QIFDocument = QIFDocument.QIFDocument;

            return asset;
        }

        /// <summary>
        /// Writes the <c>QIFDocumentWrapper</c> element, emitting the shared
        /// asset attributes, the <c>qifDocumentType</c> attribute, and the
        /// embedded QIF document as raw markup.
        /// </summary>
        public static new void WriteXml(XmlWriter writer, IAsset asset)
        {
            if (asset != null)
            {
                var qifDocumentWrapper = (IQIFDocumentWrapperAsset)asset;

                writer.WriteStartElement("QIFDocumentWrapper");

                WriteCommonXml(writer, asset);

                writer.WriteAttributeString("qifDocumentType", qifDocumentWrapper.QifDocumentType.ToString());
                writer.WriteRaw(qifDocumentWrapper.QIFDocument);

                writer.WriteEndElement();
            }
        }
    }
}
