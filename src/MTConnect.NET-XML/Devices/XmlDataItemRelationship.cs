// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("DataItemRelationship")]
    public class XmlDataItemRelationship
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("idRef")]
        public string IdRef { get; set; }

        [XmlAttribute("type")]
        public DataItemRelationshipType Type { get; set; }


        public IDataItemRelationship ToRelationship()
        {
            var relationship = new DataItemRelationship();
            //relationship.Id = Id;
            relationship.Name = Name;
            //relationship.Criticality = Criticality;
            relationship.IdRef = IdRef;
            relationship.Type = Type;
            return relationship;
        }
    }
}