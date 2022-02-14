// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// Channel represents each sensing element connected to a sensor unit.
    /// </summary>
    [XmlRoot("Channel")]
    public class XmlChannel
    {
        /// <summary>
        /// A unique identifier that will only refer to a specific sensing element.      
        /// </summary>
        [XmlAttribute("number")]
        public string Number { get; set; }

        /// <summary>
        /// The name of the sensing element.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Description MAY include any additional descriptive information the implementer chooses to include regarding a sensor element.
        /// </summary>
        [XmlElement("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Date upon which the sensor unit was last calibrated.
        /// </summary>
        [XmlElement("CalibrationDate")]
        public DateTime CalibrationDate { get; set; }

        [XmlIgnore]
        public bool CalibrationDateSpecified => CalibrationDate > DateTime.MinValue;

        /// <summary>
        /// Date upon which the sensor unit is next scheduled to be calibrated.
        /// </summary>
        [XmlElement("NextCalibrationDate")]
        public DateTime NextCalibrationDate { get; set; }

        [XmlIgnore]
        public bool NextCalibrationDateSpecified => NextCalibrationDate > DateTime.MinValue;

        /// <summary>
        /// The initials of the person verifying the validity of the calibration data
        /// </summary>
        [XmlElement("CalibrationInitials")]
        public string CalibrationInitials { get; set; }


        public XmlChannel() { }

        public XmlChannel(Channel channel)
        {
            if (channel != null)
            {
                Number = channel.Number;
                Name = channel.Name;
                Description = channel.Description;
                CalibrationDate = channel.CalibrationDate;
                NextCalibrationDate = channel.NextCalibrationDate;
                CalibrationInitials = channel.CalibrationInitials;
            }
        }

        public Channel ToChannel()
        {
            var channel = new Channel();
            channel.Number = Number;
            channel.Name = Name;
            channel.Description = Description;
            channel.CalibrationDate = CalibrationDate;
            channel.NextCalibrationDate = NextCalibrationDate;
            channel.CalibrationInitials = CalibrationInitials;
            return channel;
        }
    }
}
