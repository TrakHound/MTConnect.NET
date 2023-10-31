// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Xml;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.QIF
{
    [XmlRoot("QIFDocumentWrapper")]
    public class XmlQIFDocumentWrapperAsset : XmlAsset
    {
        [XmlAttribute("qifDocumentType")]
        public string QIFDocumentType { get; set; }
  
        public XmlQIFDocument QIFDocument { get; set; }


        public override IQIFDocumentWrapperAsset ToAsset()
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

        public static void WriteXml(XmlWriter writer, IAsset asset)
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
