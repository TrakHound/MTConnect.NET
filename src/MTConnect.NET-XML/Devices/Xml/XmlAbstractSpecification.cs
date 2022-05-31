// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.Specifications;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// Specification elements define information describing the design characteristics for a piece of equipment.
    /// </summary>
    public class XmlAbstractSpecification
    {
        /// <summary>
        /// The unique identifier for this Specification.The id attribute MUST be unique within the MTConnectDevices document.
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name provides additional meaning and differentiates between Specifications.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of data being measured.
        /// Examples of types are POSITION, VELOCITY, ANGLE, BLOCK, ROTARY_VELOCITY, etc.
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// A sub-categorization of the data item type.
        /// For example, the Sub-types of POSITION can be ACTUAL or COMMANDED.
        /// Not all types have subTypes and they can be optional.
        /// </summary>
        [XmlAttribute("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// Specification elements define information describing the design characteristics for a piece of equipment.
        /// </summary>
        [XmlAttribute("dataItemIdRef")]
        public string DataItemIdRef { get; set; }

        /// <summary>
        /// Specification elements define information describing the design characteristics for a piece of equipment.
        /// </summary>
        [XmlAttribute("units")]
        public string Units { get; set; }

        /// <summary>
        /// Specification elements define information describing the design characteristics for a piece of equipment.
        /// </summary>
        [XmlAttribute("compositionIdRef")]
        public string CompositionIdRef { get; set; }

        /// <summary>
        /// References the CoordinateSystem for geometric Specification elements.
        /// </summary>
        [XmlAttribute("coordinateIdRef")]
        public string CoordinateIdRef { get; set; }

        /// <summary>
        /// References the CoordinateSystem for geometric Specification elements.
        /// </summary>
        [XmlAttribute("originator")]
        public Originator Originator { get; set; }


        public XmlAbstractSpecification() { }

        public XmlAbstractSpecification(Specification specification)
        {
            if (specification != null)
            {
                Id = specification.Id;
                Name = specification.Name;
                Type = specification.Type;
                SubType = specification.SubType;
                DataItemIdRef = specification.DataItemIdRef;
                Units = specification.Units;
                CompositionIdRef = specification.CompositionIdRef;
                CoordinateIdRef = specification.CoordinateIdRef;
                Originator = specification.Originator;
            }
        }

        public virtual AbstractSpecification ToSpecification()
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
