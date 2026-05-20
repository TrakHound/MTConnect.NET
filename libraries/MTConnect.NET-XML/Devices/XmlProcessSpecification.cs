// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for a <c>ProcessSpecification</c>, the
    /// specification variant that carries process control, specification, and
    /// alarm limit ranges in addition to the shared specification attributes.
    /// </summary>
    [XmlRoot("ProcessSpecification")]
    public class XmlProcessSpecification : XmlAbstractSpecification
    {
        /// <summary>
        /// The process control limits, serialized as the <c>ControlLimits</c>
        /// element.
        /// </summary>
        [XmlElement("ControlLimits")]
        public XmlControlLimits ControlLimits { get; set; }

        /// <summary>
        /// The specification (acceptance) limits, serialized as the
        /// <c>SpecificationLimits</c> element.
        /// </summary>
        [XmlElement("SpecificationLimits")]
        public XmlSpecificationLimits SpecificationLimits { get; set; }

        /// <summary>
        /// The alarm limits, serialized as the <c>AlarmLimits</c> element.
        /// </summary>
        [XmlElement("AlarmLimits")]
        public XmlAlarmLimits AlarmLimits { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ProcessSpecification"/>, copying the shared specification
        /// attributes and converting each present limit set.
        /// </summary>
        public override ISpecification ToSpecification()
        {
            var specification = new ProcessSpecification();
            specification.Id = Id;
            specification.Name = Name;
            specification.Type = Type;
            specification.SubType = SubType;
            specification.DataItemIdRef = DataItemIdRef;
            specification.Units = Units;
            specification.CompositionIdRef = CompositionIdRef;
            specification.CoordinateSystemIdRef = CoordinateSystemIdRef;
            specification.Originator = Originator.ConvertEnum<Originator>();

            if (ControlLimits != null) specification.ControlLimits = ControlLimits.ToControlLimits();
            if (SpecificationLimits != null) specification.SpecificationLimits = SpecificationLimits.ToSpecificationLimits();
            if (AlarmLimits != null) specification.AlarmLimits = AlarmLimits.ToAlarmLimits();

            return specification;
        }
    }
}