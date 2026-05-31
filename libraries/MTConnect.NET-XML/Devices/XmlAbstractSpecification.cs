// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// Base XML serialization surrogate for MTConnect specifications. Carries
    /// the identification attributes shared by every specification kind and
    /// provides the default conversion to the strongly-typed
    /// <see cref="Specification"/> model.
    /// </summary>
    public class XmlAbstractSpecification
    {
        /// <summary>
        /// The unique identifier of the specification within the device.
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>
        /// The optional human-readable name of the specification.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of the measured value the specification constrains.
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// The optional subtype that further qualifies <see cref="Type"/>.
        /// </summary>
        [XmlAttribute("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// The <c>id</c> of the data item the specification applies to.
        /// </summary>
        [XmlAttribute("dataItemIdRef")]
        public string DataItemIdRef { get; set; }

        /// <summary>
        /// The engineering units the limits are expressed in.
        /// </summary>
        [XmlAttribute("units")]
        public string Units { get; set; }

        /// <summary>
        /// The <c>id</c> of the composition the specification applies to.
        /// </summary>
        [XmlAttribute("compositionIdRef")]
        public string CompositionIdRef { get; set; }

        /// <summary>
        /// The <c>id</c> of the coordinate system the limits are expressed in.
        /// </summary>
        [XmlAttribute("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// Who defined the specification, such as <c>MANUFACTURER</c> or
        /// <c>USER</c>, as the raw attribute text.
        /// </summary>
        [XmlAttribute("originator")]
        public string Originator { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="Specification"/> using the identification attributes
        /// common to every specification kind.
        /// </summary>
        public virtual ISpecification ToSpecification()
        {
            var specification = new Specification();
            specification.Id = Id;
            specification.Name = Name;
            specification.Type = Type;
            specification.SubType = SubType;
            specification.DataItemIdRef = DataItemIdRef;
            specification.Units = Units;
            specification.CompositionIdRef = CompositionIdRef;
            specification.CoordinateSystemIdRef = CoordinateSystemIdRef;
            if (!string.IsNullOrEmpty(Originator)) specification.Originator = Originator.ConvertEnum<Originator>();
            return specification;
        }
    }
}