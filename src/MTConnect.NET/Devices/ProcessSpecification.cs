// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// ProcessSpecification provides information used to assess the conformance of a variable to process requirements.
    /// </summary>
    public class ProcessSpecification : Specification
    {
        /// <summary>
        /// A set of limits used to indicate whether a process variable is stable and in control.
        /// </summary>
        [XmlElement("ControlLimits")]
        [JsonPropertyName("controlLimits")]
        public ControlLimits ControlLimits { get; set; }

        /// <summary>
        /// A set of limits defining a range of values designating acceptable performance for a variable.
        /// </summary>
        [XmlElement("SpecificationLimits")]
        [JsonPropertyName("specificationLimits")]
        public SpecificationLimits SpecificationLimits { get; set; }

        /// <summary>
        /// A set of limits used to trigger warning or alarm indicators.
        /// </summary>
        [XmlElement("AlarmLimits")]
        [JsonPropertyName("alarmLimits")]
        public AlarmLimits AlarmLimits { get; set; }
    }
}
