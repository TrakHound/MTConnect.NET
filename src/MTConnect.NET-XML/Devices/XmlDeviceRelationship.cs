// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations.Relationships;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("DeviceRelationship")]
    public class XmlDeviceRelationship : XmlRelationship
    {
        [XmlAttribute("type")]
        public DeviceRelationshipType Type { get; set; }

        [XmlAttribute("deviceUuidRef")]
        public string DeviceUuidRef { get; set; }

        [XmlAttribute("role")]
        public Role Role { get; set; }

        [XmlAttribute("href")]
        public string Href { get; set; }

        [XmlAttribute("type", Namespace = "http://www.w3.org/1999/xlink")]
        public string XLinkType { get; set; }


        public override IRelationship ToRelationship()
        {
            var relationship = new DeviceRelationship();
            relationship.Id = Id;
            relationship.Name = Name;
            relationship.Criticality = Criticality;
            relationship.IdRef = IdRef;
            relationship.Type = Type;
            relationship.DeviceUuidRef = DeviceUuidRef;
            relationship.Role = Role;
            relationship.Href = Href;
            relationship.XLinkType = XLinkType;
            return relationship;
        }
    }
}