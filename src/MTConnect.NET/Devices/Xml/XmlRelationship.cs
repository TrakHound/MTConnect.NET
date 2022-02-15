// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// Relationship is an XML element that describes the association between two pieces of equipment that function independently but together perform a manufacturing operation. 
    /// Relationship may also be used to define the association between two components within a piece of equipment.
    /// </summary>
    public class XmlRelationship
    {
        /// <summary>
        /// The unique identifier for this Relationship.
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>
        /// A descriptive name associated with this Relationship.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// A reference to the related DataItem id.
        /// </summary>
        [XmlAttribute("criticality")]
        public Criticality Criticality { get; set; }

        [XmlIgnore]
        public bool CriticalitySpecified => Criticality != Criticality.NOT_SPECIFIED;

        /// <summary>
        /// A reference to the associated component element.
        /// </summary>
        [XmlAttribute("idRef")]
        public string IdRef { get; set; }


        public XmlRelationship() { }

        public XmlRelationship(Relationship relationship)
        {
            if (relationship != null)
            {
                Id = relationship.Id;
                Name = relationship.Name;
                Criticality = relationship.Criticality;
                IdRef = relationship.IdRef;
            }
        }

        public virtual Relationship ToRelationship()
        {
            var relationship = new Relationship();
            relationship.Id = Id;
            relationship.Name = Name;
            relationship.Criticality = Criticality;
            relationship.IdRef = IdRef;
            return relationship;
        }
    }
}
