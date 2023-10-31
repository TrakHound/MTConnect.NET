// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Xml;
using MTConnect.Devices.Xml;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MTConnect.Assets.QIF
{
    [XmlRoot("QIFDocumentWrapper")]
    public class XmlQIFDocumentWrapperAsset : XmlAsset
    {
        [XmlAttribute("qifDocumentType")]
        public string QIFDocumentType { get; set; }

        //[XmlText]      
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


        #region "Xml Serialization"

        //public void WriteXml(XmlWriter writer)
        //{
        //    // Use static WriteXml()
        //}

        //public void ReadXml(XmlReader reader)
        //{
        //    try
        //    {
        //        // Read Child Elements
        //        using (var inner = reader.ReadSubtree())
        //        {
        //            while (inner.Read())
        //            {
        //                var qifDocument = inner.ReadInnerXml();
        //                if (qifDocument != null)
        //                {

        //                }


        //                //if (inner.NodeType == XmlNodeType.Element)
        //                //{
        //                //    // Create a copy of each Child Node so we can change the name to "Component" and deserialize it
        //                //    // (Seems like a dirty way to do this but until an XmlAttribute can be found to ignore the Node's name/type
        //                //    // and to always deserialize as a Component)
        //                //    var doc = new XmlDocument();
        //                //    var node = doc.ReadNode(inner);
        //                //    foreach (XmlNode child in node.ChildNodes)
        //                //    {
        //                //        if (child.NodeType == XmlNodeType.Element)
        //                //        {
        //                //            // Create a new Node with the name of "Component"
        //                //            var copy = doc.CreateNode(XmlNodeType.Element, "Component", null);

        //                //            // Copy Attributes
        //                //            foreach (XmlAttribute attribute in child.Attributes)
        //                //            {
        //                //                var attr = doc.CreateAttribute(attribute.Name);
        //                //                attr.Value = attribute.Value;
        //                //                copy.Attributes.Append(attr);
        //                //            }

        //                //            // Copy Text
        //                //            copy.InnerText = child.InnerText;
        //                //            copy.InnerXml = child.InnerXml;

        //                //            // Deserialize the copied Node to the Component base class
        //                //            var component = (XmlComponent)_serializer.Deserialize(new XmlNodeReader(copy));
        //                //            component.Type = child.Name;
        //                //            Components.Add(component);
        //                //        }
        //                //    }
        //                //}
        //            }
        //        }
        //    }
        //    catch { }

        //    // Advance Reader
        //    reader.Skip();
        //}

        //public XmlSchema GetSchema()
        //{
        //    return (null);
        //}

        #endregion

    }
}
