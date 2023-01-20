// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations.Sensor;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("Channel")]
    public class XmlChannel
    {
        [XmlAttribute("number")]
        public string Number { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlElement("CalibrationDate")]
        public DateTime CalibrationDate { get; set; }

        [XmlElement("NextCalibrationDate")]
        public DateTime NextCalibrationDate { get; set; }

        [XmlElement("CalibrationInitials")]
        public string CalibrationInitials { get; set; }


        public IChannel ToChannel()
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

        public static void WriteXml(XmlWriter writer, IChannel channel)
        {
            if (channel != null)
            {
                writer.WriteStartElement("Channel");

                // Write Properties
                if (!string.IsNullOrEmpty(channel.Number)) writer.WriteAttributeString("number", channel.Number);
                if (!string.IsNullOrEmpty(channel.Name)) writer.WriteAttributeString("name", channel.Name);

                // Write Description
                if (!string.IsNullOrEmpty(channel.Description))
                {
                    writer.WriteStartElement("Description");
                    writer.WriteString(channel.Description);
                    writer.WriteEndElement();
                }

                // Write CalibrationDate
                if (channel.CalibrationDate > DateTime.MinValue)
                {
                    writer.WriteStartElement("CalibrationDate");
                    writer.WriteString(channel.CalibrationDate.ToString("o"));
                    writer.WriteEndElement();
                }

                // Write NextCalibrationDate
                if (channel.NextCalibrationDate > DateTime.MinValue)
                {
                    writer.WriteStartElement("NextCalibrationDate");
                    writer.WriteString(channel.NextCalibrationDate.ToString("o"));
                    writer.WriteEndElement();
                }

                // Write Calibration Initials
                if (!string.IsNullOrEmpty(channel.CalibrationInitials))
                {
                    writer.WriteStartElement("CalibrationInitials");
                    writer.WriteString(channel.CalibrationInitials);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}