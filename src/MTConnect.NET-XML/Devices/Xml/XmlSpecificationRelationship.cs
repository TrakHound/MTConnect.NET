// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// A Relationship providing a semantic reference to another DataItem described by the type property.
    /// </summary>
    [XmlRoot("DataItemRelationship")]
    public class XmlSpecificationRelationship : XmlRelationship
    {
        /// <summary>
        /// Specifies how the Specification is related.
        /// </summary>
        [XmlAttribute("type")]
        public SpecificationRelationshipType Type { get; set; }


        public XmlSpecificationRelationship() { }

        public XmlSpecificationRelationship(SpecificationRelationship relationship)
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
