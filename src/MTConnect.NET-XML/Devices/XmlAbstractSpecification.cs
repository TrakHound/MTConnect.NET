// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations.Specifications;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    public class XmlAbstractSpecification
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("subType")]
        public string SubType { get; set; }

        [XmlAttribute("dataItemIdRef")]
        public string DataItemIdRef { get; set; }

        [XmlAttribute("units")]
        public string Units { get; set; }

        [XmlAttribute("compositionIdRef")]
        public string CompositionIdRef { get; set; }

        [XmlAttribute("coordinateIdRef")]
        public string CoordinateIdRef { get; set; }

        [XmlAttribute("originator")]
        public Originator Originator { get; set; }


        public virtual IAbstractSpecification ToSpecification()
        {
            var specification = new Specification();
            specification.Id = Id;
            specification.Name = Name;
            specification.Type = Type;
            specification.SubType = SubType;
            specification.DataItemIdRef = DataItemIdRef;
            specification.Units = Units;
            specification.CompositionIdRef = CompositionIdRef;
            specification.CoordinateIdRef = CoordinateIdRef;
            specification.Originator = Originator;
            return specification;
        }
    }
}