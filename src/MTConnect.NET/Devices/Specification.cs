// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// Specification elements define information describing the design characteristics for a piece of equipment.
    /// </summary>
    public class Specification
    {
        /// <summary>
        /// The unique identifier for this Specification.The id attribute MUST be unique within the MTConnectDevices document.
        /// </summary>
        [XmlAttribute("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name provides additional meaning and differentiates between Specifications.
        /// </summary>
        [XmlAttribute("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of data being measured.
        /// Examples of types are POSITION, VELOCITY, ANGLE, BLOCK, ROTARY_VELOCITY, etc.
        /// </summary>
        [XmlAttribute("type")]
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// A sub-categorization of the data item type.
        /// For example, the Sub-types of POSITION can be ACTUAL or COMMANDED.
        /// Not all types have subTypes and they can be optional.
        /// </summary>
        [XmlAttribute("subType")]
        [JsonPropertyName("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// Specification elements define information describing the design characteristics for a piece of equipment.
        /// </summary>
        [XmlAttribute("dataItemIdRef")]
        [JsonPropertyName("dataItemIdRef")]
        public string DataItemIdRef { get; set; }

        /// <summary>
        /// Specification elements define information describing the design characteristics for a piece of equipment.
        /// </summary>
        [XmlAttribute("units")]
        [JsonPropertyName("units")]
        public string Units { get; set; }

        /// <summary>
        /// Specification elements define information describing the design characteristics for a piece of equipment.
        /// </summary>
        [XmlAttribute("compositionIdRef")]
        [JsonPropertyName("compositionIdRef")]
        public string CompositionIdRef { get; set; }

        /// <summary>
        /// References the CoordinateSystem for geometric Specification elements.
        /// </summary>
        [XmlAttribute("coordinateIdRef")]
        [JsonPropertyName("coordinateIdRef")]
        public string CoordinateIdRef { get; set; }

        /// <summary>
        /// References the CoordinateSystem for geometric Specification elements.
        /// </summary>
        [XmlAttribute("originator")]
        [JsonPropertyName("originator")]
        public Originator Originator { get; set; }

        /// <summary>
        /// A numeric upper constraint. 
        /// </summary>
        [XmlElement("Maximum")]
        [JsonPropertyName("maximum")]
        public double Maximum { get; set; }

        /// <summary>
        /// The upper conformance boundary for a variable.
        /// </summary>
        [XmlElement("UpperLimit")]
        [JsonPropertyName("upperLimit")]
        public double UpperLimit { get; set; }

        /// <summary>
        /// The upper boundary indicating increased concern and supervision may be required.
        /// </summary>
        [XmlElement("UpperWarning")]
        [JsonPropertyName("upperWarning")]
        public double UpperWarning { get; set; }

        /// <summary>
        /// The ideal or desired value for a variable.
        /// </summary>
        [XmlElement("Nominal")]
        [JsonPropertyName("nominal")]
        public double Nominal { get; set; }

        /// <summary>
        /// The lower conformance boundary for a variable.
        /// </summary>
        [XmlElement("LowerLimit")]
        [JsonPropertyName("lowerLimit")]
        public double LowerLimit { get; set; }

        /// <summary>
        /// The lower boundary indicating increased concern and supervision may be required.
        /// </summary>
        [XmlElement("LowerWarning")]
        [JsonPropertyName("lowerWarning")]
        public double LowerWarning { get; set; }

        /// <summary>
        /// A numeric lower constraint. 
        /// </summary>
        [XmlElement("Minimum")]
        [JsonPropertyName("minimum")]
        public double Minimum { get; set; }

    }
}
