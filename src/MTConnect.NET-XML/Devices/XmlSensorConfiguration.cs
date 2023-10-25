// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations.Sensor;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("SensorConfiguration")]
    public class XmlSensorConfiguration
    {
        [XmlElement("FirmwareVersion")]
        public string FirmwareVersion { get; set; }

        [XmlElement("CalibrationDate")]
        public DateTime CalibrationDate { get; set; }

        [XmlElement("NextCalibrationDate")]
        public DateTime NextCalibrationDate { get; set; }

        [XmlElement("CalibrationInitials")]
        public string CalibrationInitials { get; set; }

        [XmlArray("Channels")]
        [XmlArrayItem("Channel", typeof(XmlChannel))]
        public List<XmlChannel> Channels { get; set; }


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
                    writer.WriteString(sensorConfiguration.CalibrationDate.ToString("o"));
                    writer.WriteEndElement();
                }

                // Write NextCalibrationDate
                if (sensorConfiguration.NextCalibrationDate > DateTime.MinValue)
                {
                    writer.WriteStartElement("NextCalibrationDate");
                    writer.WriteString(sensorConfiguration.NextCalibrationDate.ToString("o"));
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