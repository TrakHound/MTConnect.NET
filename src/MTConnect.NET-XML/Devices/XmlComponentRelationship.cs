// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.Relationships;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// ComponentRelationship describes the association between two components within a piece of equipment that function independently but together perform a capability or service within a piece of equipment.
    /// </summary>
    [XmlRoot("ComponentRelationship")]
    public class XmlComponentRelationship : XmlRelationship
    {
        /// <summary>
        /// Defines the authority that this component element has relative to the associated component element.
        /// </summary>
        [XmlAttribute("type")]
        public ComponentRelationshipType Type { get; set; }


        public XmlComponentRelationship() { }

        public XmlComponentRelationship(ComponentRelationship relationship)
        {
            if (relationship != null)
            {
                Id = relationship.Id;
                Name = relationship.Name;
                Criticality = relationship.Criticality;
                IdRef = relationship.IdRef;
                Type = relationship.Type;
            }
        }

        public override Relationship ToRelationship()
        {
            var relationship = new ComponentRelationship();
            relationship.Id = Id;
            relationship.Name = Name;
            relationship.Criticality = Criticality;
            relationship.IdRef = IdRef;
            relationship.Type = Type;
            return relationship;
        }
    }
}
