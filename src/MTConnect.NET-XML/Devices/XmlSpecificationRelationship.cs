// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.Relationships;
using MTConnect.Devices.DataItems;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("DataItemRelationship")]
    public class XmlSpecificationRelationship : XmlRelationship
    {
        [XmlAttribute("type")]
        public SpecificationRelationshipType Type { get; set; }


        public override IRelationship ToRelationship()
        {
            var relationship = new SpecificationRelationship();
            relationship.Id = Id;
            relationship.Name = Name;
            relationship.Criticality = Criticality;
            relationship.IdRef = IdRef;
            relationship.Type = Type;
            return relationship;
        }
    }
}
