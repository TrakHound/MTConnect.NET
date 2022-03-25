// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// A set of limits defining a range of values designating acceptable performance for a variable.
    /// </summary>
    [XmlRoot("SpecificationLimits")]
    public class XmlSpecificationLimits
    {
        /// <summary>
        /// The upper conformance boundary for a variable.
        /// </summary>
        [XmlElement("UpperLimit")]
        public double? UpperLimit { get; set; }

        [XmlIgnore]
        public bool UpperLimitSpecified => UpperLimit.HasValue;

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


        public XmlSpecificationLimits() { }

        public XmlSpecificationLimits(SpecificationLimits specificationLimits)
        {
            if (specificationLimits != null)
            {
                UpperLimit = specificationLimits.UpperLimit;
                Nominal = specificationLimits.Nominal;
                LowerLimit = specificationLimits.LowerLimit;
            }
        }

        public SpecificationLimits ToSpecificationLimits()
        {
            var specificationLimits = new SpecificationLimits();
            specificationLimits.UpperLimit = UpperLimit;
            specificationLimits.Nominal = Nominal;
            specificationLimits.LowerLimit = LowerLimit;
            return specificationLimits;
        }
    }
}
