// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("DataItemRelationship")]
    public class XmlSpecificationRelationship : XmlAbstractDataItemRelationship
    {
        [XmlAttribute("type")]
        public SpecificationRelationshipType Type { get; set; }


        public override IAbstractDataItemRelationship ToRelationship()
        {
            var relationship = new SpecificationRelationship();
            //relationship.Id = Id;
            relationship.Name = Name;
            //relationship.Criticality = Criticality;
            relationship.IdRef = IdRef;
            relationship.Type = Type;
            return relationship;
        }
    }
}