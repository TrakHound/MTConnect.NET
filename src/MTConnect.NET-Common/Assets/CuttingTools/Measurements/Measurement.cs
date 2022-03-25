// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using MTConnect.Devices;

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// A Measurement MUST be a scalar floating-point value that MAY be constrained to a maximum and minimum value.
    /// </summary>
    public abstract class Measurement
    {
        [XmlIgnore]
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [XmlText]
        [JsonPropertyName("cdata")]
        public double CDATA { get; set; }

        /// <summary>
        /// The number of significant digits in the reported value. 
        /// This is used by applications to determine accuracy of values. This MAY be specified for all numeric values.
        /// </summary>
        [XmlAttribute("significantDigits")]
        [JsonPropertyName("significantDigits")]
        public int SignificantDigits { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool SignificantDigitsSpecified => SignificantDigits > 0;

        /// <summary>
        /// The units for the measurements.
        /// </summary>
        [XmlAttribute("units")]
        [JsonPropertyName("units")]
        public string Units { get; set; }
        //public Units Units { get; set; }

        //[XmlIgnore]
        //[JsonIgnore]
        //public bool UnitsSpecified => Units != Units.NOT_SPECIFIED;

        /// <summary>
        /// The units the measurement was originally recorded in.
        /// </summary>
        [XmlAttribute("nativeUnits")]
        [JsonPropertyName("nativeUnits")]
        public string NativeUnits { get; set; }
        //public NativeUnits NativeUnits { get; set; }

        //[XmlIgnore]
        //[JsonIgnore]
        //public bool NativeUnitsSpecified => NativeUnits != NativeUnits.NOT_SPECIFIED;

        /// <summary>
        /// A shop specific code for this measurement. ISO 13399 codes MAY be used to for these codes as well.
        /// </summary>
        [XmlAttribute("code")]
        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// The maximum value for this measurement. Exceeding this value would indicate the tool is not usable.
        /// </summary>
        [XmlAttribute("maximum")]
        [JsonPropertyName("maximum")]
        public double Maximum { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool MaximumSpecified => Maximum > 0;

        /// <summary>
        /// The minimum value for this measurement. Exceeding this value would indicate the tool is not usable.
        /// </summary>
        [XmlAttribute("minimum")]
        [JsonPropertyName("minimum")]
        public double Minimum { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool MinimumSpecified => Minimum > 0;

        /// <summary>
        /// The as advertised value for this measurement.
        /// </summary>
        [XmlAttribute("nominal")]
        [JsonPropertyName("nominal")]
        public double Nominal { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool NominalSpecified => Nominal > 0;
    }
}
