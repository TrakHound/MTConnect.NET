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
    [XmlRoot("Specification")]
    public class XmlSpecification : XmlAbstractSpecification
    {
        /// <summary>
        /// A numeric upper constraint. 
        /// </summary>
        [XmlElement("Maximum")]
        public double? Maximum { get; set; }

        [XmlIgnore]
        public bool MaximumSpecified => Maximum.HasValue;

        /// <summary>
        /// The upper conformance boundary for a variable.
        /// </summary>
        [XmlElement("UpperLimit")]
        public double? UpperLimit { get; set; }

        [XmlIgnore]
        public bool UpperLimitSpecified => UpperLimit.HasValue;

        /// <summary>
        /// The upper boundary indicating increased concern and supervision may be required.
        /// </summary>
        [XmlElement("UpperWarning")]
        public double? UpperWarning { get; set; }

        [XmlIgnore]
        public bool UpperWarningSpecified => UpperWarning.HasValue;

        /// <summary>
        /// The ideal or desired value for a variable.
        /// </summary>
        [XmlElement("Nominal")]
        public double? Nominal { get; set; }

        [XmlIgnore]
        public bool NominalSpecified => Nominal.HasValue;

        /// <summary>
        /// The lower conformance boundary for a variable.
        /// </summary>
        [XmlElement("LowerLimit")]
        public double? LowerLimit { get; set; }

        [XmlIgnore]
        public bool LowerLimitSpecified => LowerLimit.HasValue;

        /// <summary>
        /// The lower boundary indicating increased concern and supervision may be required.
        /// </summary>
        [XmlElement("LowerWarning")]
        public double? LowerWarning { get; set; }

        [XmlIgnore]
        public bool LowerWarningSpecified => LowerWarning.HasValue;

        /// <summary>
        /// A numeric lower constraint. 
        /// </summary>
        [XmlElement("Minimum")]
         public double? Minimum { get; set; }

        [XmlIgnore]
        public bool MinimumSpecified => Minimum.HasValue;


        public XmlSpecification() { }

        public XmlSpecification(Specification specification)
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
                Maximum = specification.Maximum;
                UpperLimit = specification.UpperLimit;
                UpperWarning = specification.UpperWarning;
                Nominal = specification.Nominal;
                LowerLimit = specification.LowerLimit;
                LowerWarning = specification.LowerWarning;
                Minimum = specification.Minimum;
            }
        }

        public override AbstractSpecification ToSpecification()
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
            specification.Maximum = Maximum;
            specification.UpperLimit = UpperLimit;
            specification.UpperWarning = UpperWarning;
            specification.Nominal = Nominal;
            specification.LowerLimit = LowerLimit;
            specification.LowerWarning = LowerWarning;
            specification.Minimum = Minimum;
            return specification;
        }
    }
}
