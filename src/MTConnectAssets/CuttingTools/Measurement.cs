// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.MTConnectAssets.CuttingTools
{
    public class Measurement
    {
        [XmlText]
        public double CDATA { get; set; }

        /// <summary>
        /// The number of significant digits in the reported value. 
        /// This is used by applications to determine accuracy of values. This MAY be specified for all numeric values.
        /// </summary>
        [XmlAttribute("significantDigits")]
        public int SignificantDigits { get; set; }

        /// <summary>
        /// The units for the measurements.
        /// </summary>
        [XmlAttribute("units")]
        public string Units { get; set; }

        /// <summary>
        /// The units the measurement was originally recorded in.
        /// </summary>
        [XmlAttribute("nativeUnits")]
        public string NativeUnits { get; set; }

        /// <summary>
        /// A shop specific code for this measurement. ISO 13399 codes MAY be used to for these codes as well.
        /// </summary>
        [XmlAttribute("code")]
        public string Code { get; set; }

        /// <summary>
        /// The maximum value for this measurement. Exceeding this value would indicate the tool is not usable.
        /// </summary>
        [XmlAttribute("maximum")]
        public double Maximum { get; set; }

        /// <summary>
        /// The minimum value for this measurement. Exceeding this value would indicate the tool is not usable.
        /// </summary>
        [XmlAttribute("minimum")]
        public double Minimum { get; set; }

        /// <summary>
        /// The as advertised value for this measurement.
        /// </summary>
        [XmlAttribute("nominal")]
        public double Nominal { get; set; }
    }
}
