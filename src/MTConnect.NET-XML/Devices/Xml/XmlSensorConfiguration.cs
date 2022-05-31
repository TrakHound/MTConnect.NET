// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.Sensor;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// Configuration contains technical information about a component describing its physical layout,
    /// functional characteristics, and relationships with other components within a piece of equipment.
    /// </summary>
    [XmlRoot("SensorConfiguration")]
    public class XmlSensorConfiguration
    {
        /// <summary>
        /// Version number for the sensor unit as specified by the manufacturer.
        /// </summary>
        [XmlElement("FirmwareVersion")]
        public string FirmwareVersion { get; set; }

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

        /// <summary>
        /// When Sensor represents multiple sensing elements, each sensing element is represented by a Channel for the Sensor.
        /// </summary>
        [XmlArray("Channels")]
        [XmlArrayItem("Channel", typeof(XmlChannel))]
        public List<XmlChannel> Channels { get; set; }

        [XmlIgnore]
        public bool ChannelsSpecified => !Channels.IsNullOrEmpty();


        public XmlSensorConfiguration() { }

        public XmlSensorConfiguration(SensorConfiguration sensorConfiguration)
        {
            if (sensorConfiguration != null)
            {
                FirmwareVersion = sensorConfiguration.FirmwareVersion;
                CalibrationDate = sensorConfiguration.CalibrationDate;
                NextCalibrationDate = sensorConfiguration.NextCalibrationDate;
                CalibrationInitials = sensorConfiguration.CalibrationInitials;

                // Channels
                if (!sensorConfiguration.Channels.IsNullOrEmpty())
                {
                    var channels = new List<XmlChannel>();
                    foreach (var channel in sensorConfiguration.Channels)
                    {
                        channels.Add(new XmlChannel(channel));
                    }
                    Channels = channels;
                }
            }
        }

        public SensorConfiguration ToSensorConfiguration()
        {
            var sensorConfiguration = new SensorConfiguration();
            sensorConfiguration.FirmwareVersion = FirmwareVersion;
            sensorConfiguration.CalibrationDate = CalibrationDate;
            sensorConfiguration.NextCalibrationDate = NextCalibrationDate;
            sensorConfiguration.CalibrationInitials = CalibrationInitials;

            // Channels
            if (!Channels.IsNullOrEmpty())
            {
                var channels = new List<Channel>();
                foreach (var channel in Channels)
                {
                    channels.Add(channel.ToChannel());
                }
                sensorConfiguration.Channels = channels;
            }

            return sensorConfiguration;
        }
    }
}
