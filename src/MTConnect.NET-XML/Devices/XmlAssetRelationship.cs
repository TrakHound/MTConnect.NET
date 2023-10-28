// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("AssetRelationship")]
    public class XmlAssetRelationship : XmlRelationship
    {
        [XmlAttribute("assetIdRef")]
        public string AssetIdRef { get; set; }

        [XmlAttribute("assetType")]
        public string AssetType { get; set; }

        [XmlAttribute("href")]
        public string Href { get; set; }


        public override IRelationship ToRelationship()
        {
            var relationship = new AssetRelationship();
            relationship.Id = Id;
            relationship.Name = Name;
            relationship.Criticality = Criticality;
            //relationship.IdRef = IdRef;
            relationship.AssetIdRef = AssetIdRef;
            relationship.AssetType = AssetType;
            relationship.Href = Href;
            return relationship;
        }
    }
}