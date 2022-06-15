// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.Configurations.CoordinateSystems;
using MTConnect.Devices.Configurations.Motion;
using MTConnect.Devices.Configurations.Relationships;
using MTConnect.Devices.Configurations.Sensor;
using MTConnect.Devices.Configurations.SolidModel;
using MTConnect.Devices.Configurations.Specifications;
using MTConnect.Devices.DataItems;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// A Relationship providing a semantic reference to another DataItem described by the type property.
    /// </summary>
    [XmlRoot("DataItemRelationship")]
    public class XmlDataItemRelationship : XmlRelationship
    {
        /// <summary>
        /// Specifies how the DataItem is related.
        /// </summary>
        [XmlAttribute("type")]
        public DataItemRelationshipType Type { get; set; }


        public XmlDataItemRelationship() { }

        public XmlDataItemRelationship(DataItemRelationship relationship)
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
            var relationship = new DataItemRelationship();
            relationship.Id = Id;
            relationship.Name = Name;
            relationship.Criticality = Criticality;
            relationship.IdRef = IdRef;
            relationship.Type = Type;
            return relationship;
        }
    }
}
