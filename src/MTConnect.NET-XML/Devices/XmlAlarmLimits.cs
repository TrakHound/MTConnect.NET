// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.Specifications;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// A set of limits used to trigger warning or alarm indicators.
    /// </summary>
    [XmlRoot("AlarmLimits")]
    public class XmlAlarmLimits
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


        public XmlAlarmLimits() { }

        public XmlAlarmLimits(AlarmLimits alarmLimits)
        {
            if (alarmLimits != null)
            {
                UpperLimit = alarmLimits.UpperLimit;
                UpperWarning = alarmLimits.UpperWarning;
                LowerLimit = alarmLimits.LowerLimit;
                LowerWarning = alarmLimits.LowerWarning;
            }
        }

        public AlarmLimits ToAlarmLimits()
        {
            var alarmLimits = new AlarmLimits();
            alarmLimits.UpperLimit = UpperLimit;
            alarmLimits.UpperWarning = UpperWarning;
            alarmLimits.LowerLimit = LowerLimit;
            alarmLimits.LowerWarning = LowerWarning;
            return alarmLimits;
        }
    }
}
