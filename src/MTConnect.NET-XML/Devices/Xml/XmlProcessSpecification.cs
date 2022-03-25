// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// ProcessSpecification provides information used to assess the conformance of a variable to process requirements.
    /// </summary>
    [XmlRoot("ProcessSpecification")]
    public class XmlProcessSpecification : XmlAbstractSpecification
    {
        /// <summary>
        /// A set of limits used to indicate whether a process variable is stable and in control.
        /// </summary>
        [XmlElement("ControlLimits")]
        public XmlControlLimits ControlLimits { get; set; }

        /// <summary>
        /// A set of limits defining a range of values designating acceptable performance for a variable.
        /// </summary>
        [XmlElement("SpecificationLimits")]
        public XmlSpecificationLimits SpecificationLimits { get; set; }

        /// <summary>
        /// A set of limits used to trigger warning or alarm indicators.
        /// </summary>
        [XmlElement("AlarmLimits")]
        public XmlAlarmLimits AlarmLimits { get; set; }


        public XmlProcessSpecification() { }

        public XmlProcessSpecification(ProcessSpecification specification)
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

                if (specification.ControlLimits != null) ControlLimits = new XmlControlLimits(specification.ControlLimits);
                if (specification.SpecificationLimits != null) SpecificationLimits = new XmlSpecificationLimits(specification.SpecificationLimits);
                if (specification.AlarmLimits != null) AlarmLimits = new XmlAlarmLimits(specification.AlarmLimits);
            }
        }

        public override AbstractSpecification ToSpecification()
        {
            var specification = new ProcessSpecification();
            specification.Id = Id;
            specification.Name = Name;
            specification.Type = Type;
            specification.SubType = SubType;
            specification.DataItemIdRef = DataItemIdRef;
            specification.Units = Units;
            specification.CompositionIdRef = CompositionIdRef;
            specification.CoordinateIdRef = CoordinateIdRef;
            specification.Originator = Originator;

            if (ControlLimits != null) specification.ControlLimits = ControlLimits.ToControlLimits();
            if (SpecificationLimits != null) specification.SpecificationLimits = SpecificationLimits.ToSpecificationLimits();
            if (AlarmLimits != null) specification.AlarmLimits = AlarmLimits.ToAlarmLimits();

            return specification;
        }
    }
}
