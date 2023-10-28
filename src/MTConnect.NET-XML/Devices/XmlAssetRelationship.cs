// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("AssetRelationship")]
    public class XmlAssetRelationship : XmlConfigurationRelationship
    {
        [XmlAttribute("assetIdRef")]
        public string AssetIdRef { get; set; }

        [XmlAttribute("assetType")]
        public string AssetType { get; set; }

        [XmlAttribute("href")]
        public string Href { get; set; }


        public override IAssetRelationship ToRelationship()
        {
            var relationship = new AssetRelationship();
            relationship.Id = Id;
            relationship.Name = Name;
            relationship.Type = Type;
            relationship.Criticality = Criticality;
            relationship.AssetIdRef = AssetIdRef;
            relationship.AssetType = AssetType;
            relationship.Href = Href;
            return relationship;
        }

        public static void WriteXml(XmlWriter writer, IAssetRelationship relationship)
        {
            if (relationship != null)
            {
                writer.WriteStartElement(relationship.GetType().Name);
                WriteCommonXml(writer, relationship);
                if (!string.IsNullOrEmpty(relationship.AssetIdRef)) writer.WriteAttributeString("assetIdRef", relationship.AssetIdRef);
                if (!string.IsNullOrEmpty(relationship.AssetType)) writer.WriteAttributeString("assetType", relationship.AssetType);
                if (!string.IsNullOrEmpty(relationship.Href)) writer.WriteAttributeString("href", relationship.Href);
                writer.WriteEndElement();
            }
        }
    }
}