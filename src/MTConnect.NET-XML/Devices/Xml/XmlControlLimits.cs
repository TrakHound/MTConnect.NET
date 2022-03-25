// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// A set of limits used to indicate whether a process variable is stable and in control.
    /// </summary>
    [XmlRoot("ControlLimits")]
    public class XmlControlLimits
    {
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


        public XmlControlLimits() { }

        public XmlControlLimits(ControlLimits controlLimits)
        {
            if (controlLimits != null)
            {
                UpperLimit = controlLimits.UpperLimit;
                UpperWarning = controlLimits.UpperWarning;
                Nominal = controlLimits.Nominal;
                LowerLimit = controlLimits.LowerLimit;
                LowerWarning = controlLimits.LowerWarning;
            }
        }

        public ControlLimits ToControlLimits()
        {
            var controlLimits = new ControlLimits();
            controlLimits.UpperLimit = UpperLimit;
            controlLimits.UpperWarning = UpperWarning;
            controlLimits.Nominal = Nominal;
            controlLimits.LowerLimit = LowerLimit;
            controlLimits.LowerWarning = LowerWarning;
            return controlLimits;
        }
    }
}
