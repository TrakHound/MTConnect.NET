// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for a <c>SensorConfiguration</c>, the
    /// calibration metadata and channels of a sensor component. Mirrors the
    /// on-the-wire element and converts to and from the strongly-typed
    /// <see cref="SensorConfiguration"/> model.
    /// </summary>
    [XmlRoot("SensorConfiguration")]
    public class XmlSensorConfiguration
    {
        /// <summary>
        /// The firmware version running on the sensor.
        /// </summary>
        [XmlElement("FirmwareVersion")]
        public string FirmwareVersion { get; set; }

        /// <summary>
        /// The date the sensor was last calibrated.
        /// </summary>
        [XmlElement("CalibrationDate")]
        public DateTime? CalibrationDate { get; set; }

        /// <summary>
        /// The date the sensor is next due for calibration.
        /// </summary>
        [XmlElement("NextCalibrationDate")]
        public DateTime? NextCalibrationDate { get; set; }

        /// <summary>
        /// The initials of the person who performed the calibration.
        /// </summary>
        [XmlElement("CalibrationInitials")]
        public string CalibrationInitials { get; set; }

        /// <summary>
        /// The individual calibrated input channels of the sensor.
        /// </summary>
        [XmlArray("Channels")]
        [XmlArrayItem("Channel", typeof(XmlChannel))]
        public List<XmlChannel> Channels { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="SensorConfiguration"/>, projecting the channels into
        /// their model representation.
        /// </summary>
        public ISensorConfiguration ToSensorConfiguration()
        {
            var sensorConfiguration = new SensorConfiguration();
            sensorConfiguration.FirmwareVersion = FirmwareVersion;
            sensorConfiguration.CalibrationDate = CalibrationDate;
            sensorConfiguration.NextCalibrationDate = NextCalibrationDate;
            sensorConfiguration.CalibrationInitials = CalibrationInitials;

            // Channels
            if (!Channels.IsNullOrEmpty())
            {
                var channels = new List<IChannel>();
                foreach (var channel in Channels)
                {
                    channels.Add(channel.ToChannel());
                }
                sensorConfiguration.Channels = channels;
            }

            return sensorConfiguration;
        }

        /// <summary>
        /// Writes the given <see cref="ISensorConfiguration"/> to
        /// <paramref name="writer"/> as a <c>SensorConfiguration</c> element,
        /// omitting calibration metadata and channels that are not set.
        /// </summary>
        public static void WriteXml(XmlWriter writer, ISensorConfiguration sensorConfiguration)
        {
            if (sensorConfiguration != null)
            {
                writer.WriteStartElement("SensorConfiguration");

                // Write Firmware Version
                if (!string.IsNullOrEmpty(sensorConfiguration.FirmwareVersion))
                {
                    writer.WriteStartElement("FirmwareVersion");
                    writer.WriteString(sensorConfiguration.FirmwareVersion);
                    writer.WriteEndElement();
                }

                // Write CalibrationDate
                if (sensorConfiguration.CalibrationDate > DateTime.MinValue)
                {
                    writer.WriteStartElement("CalibrationDate");
                    writer.WriteString(sensorConfiguration.CalibrationDate?.ToString("o"));
                    writer.WriteEndElement();
                }

                // Write NextCalibrationDate
                if (sensorConfiguration.NextCalibrationDate > DateTime.MinValue)
                {
                    writer.WriteStartElement("NextCalibrationDate");
                    writer.WriteString(sensorConfiguration.NextCalibrationDate?.ToString("o"));
                    writer.WriteEndElement();
                }

                // Write Calibration Initials
                if (!string.IsNullOrEmpty(sensorConfiguration.CalibrationInitials))
                {
                    writer.WriteStartElement("CalibrationInitials");
                    writer.WriteString(sensorConfiguration.CalibrationInitials);
                    writer.WriteEndElement();
                }

                // Write Channels
                if (!sensorConfiguration.Channels.IsNullOrEmpty())
                {
                    writer.WriteStartElement("Channels");

                    foreach (var channel in sensorConfiguration.Channels)
                    {
                        XmlChannel.WriteXml(writer, channel);
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}