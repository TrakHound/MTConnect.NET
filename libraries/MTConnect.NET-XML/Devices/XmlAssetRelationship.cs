// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for an <c>AssetRelationship</c>, associating
    /// a Component with an asset (for example a cutting tool) it depends on or
    /// otherwise relates to.
    /// </summary>
    [XmlRoot("AssetRelationship")]
    public class XmlAssetRelationship : XmlConfigurationRelationship
    {
        /// <summary>
        /// The <c>assetId</c> of the related asset, carried by the
        /// <c>assetIdRef</c> attribute.
        /// </summary>
        [XmlAttribute("assetIdRef")]
        public string AssetIdRef { get; set; }

        /// <summary>
        /// The type of the related asset, carried by the <c>assetType</c>
        /// attribute.
        /// </summary>
        [XmlAttribute("assetType")]
        public string AssetType { get; set; }

        /// <summary>
        /// An optional URI locating the related asset, carried by the
        /// <c>href</c> attribute.
        /// </summary>
        [XmlAttribute("href")]
        public string Href { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="AssetRelationship"/>, copying the shared relationship
        /// fields, mapping the optional criticality, and copying the asset
        /// reference attributes.
        /// </summary>
        public override IConfigurationRelationship ToRelationship()
        {
            var relationship = new AssetRelationship();
            relationship.Id = Id;
            relationship.Name = Name;
            relationship.Type = Type;
            if (!string.IsNullOrEmpty(Criticality)) relationship.Criticality = Criticality.ConvertEnum<CriticalityType>();
            relationship.AssetIdRef = AssetIdRef;
            relationship.AssetType = AssetType;
            relationship.Href = Href;
            return relationship;
        }

        /// <summary>
        /// Writes the relationship element, emitting the shared relationship
        /// attributes followed by the asset reference attributes that are
        /// populated.
        /// </summary>
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