// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.Relationships;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// DeviceRelationship describes the association between two pieces of equipment that function independently but together perform a manufacturing operation.
    /// </summary>
    [XmlRoot("DeviceRelationship")]
    public class XmlDeviceRelationship : XmlRelationship
    {
        /// <summary>
        /// Defines the authority that this piece of equipment has relative to the associated piece of equipment.
        /// </summary>
        [XmlAttribute("type")]
        public DeviceRelationshipType Type { get; set; }

        /// <summary>
        /// A reference to the associated piece of equipment.
        /// </summary>
        [XmlAttribute("deviceUuidRef")]
        public string DeviceUuidRef { get; set; }

        /// <summary>
        /// Defines the services or capabilities that the referenced piece of equipment provides relative to this piece of equipment.    
        /// </summary>
        [XmlAttribute("role")]
        public Role Role { get; set; }

        /// <summary>
        /// A URI identifying the Agent that is publishing information for the associated piece of equipment. href MUST also include the UUID for that specific piece of equipment.    
        /// </summary>
        [XmlAttribute("href")]
        public string Href { get; set; }

        /// <summary>
        /// The XLink type attribute MUST have a fixed value of locator as defined in W3C XLink 1.1 https://www.w3.org/TR/xlink11/ section 5.4 Locator Attribute(href)
        /// </summary>
        [XmlAttribute("type", Namespace = "http://www.w3.org/1999/xlink")]
        public string XLinkType { get; set; }


        public XmlDeviceRelationship() { }

        public XmlDeviceRelationship(DeviceRelationship relationship)
        {
            if (relationship != null)
            {
                Id = relationship.Id;
                Name = relationship.Name;
                Criticality = relationship.Criticality;
                IdRef = relationship.IdRef;
                Type = relationship.Type;
                DeviceUuidRef = relationship.DeviceUuidRef;
                Role = relationship.Role;
                Href = relationship.Href;
                XLinkType = relationship.XLinkType;
            }
        }

        public override Relationship ToRelationship()
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
