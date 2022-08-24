// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.Specifications;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("AlarmLimits")]
    public class XmlAlarmLimits
    {
        [XmlElement("UpperLimit")]
        public double? UpperLimit { get; set; }

        [XmlElement("UpperWarning")]
        public double? UpperWarning { get; set; }

        [XmlElement("LowerLimit")]
        public double? LowerLimit { get; set; }

        [XmlElement("LowerWarning")]
        public double? LowerWarning { get; set; }


        public IAlarmLimits ToAlarmLimits()
        {
            var alarmLimits = new AlarmLimits();
            alarmLimits.UpperLimit = UpperLimit;
            alarmLimits.UpperWarning = UpperWarning;
            alarmLimits.LowerLimit = LowerLimit;
            alarmLimits.LowerWarning = LowerWarning;
            return alarmLimits;
        }

        public static void WriteXml(XmlWriter writer, IAlarmLimits alarmLimits)
        {
            if (alarmLimits != null)
            {
                writer.WriteStartElement("ControlLimits");

                // Write Upper Limit
                if (alarmLimits.UpperLimit != null)
                {
                    writer.WriteStartElement("UpperLimit");
                    writer.WriteString(alarmLimits.UpperLimit.ToString());
                    writer.WriteEndElement();
                }

                // Write Upper Warning
                if (alarmLimits.UpperWarning != null)
                {
                    writer.WriteStartElement("UpperWarning");
                    writer.WriteString(alarmLimits.UpperWarning.ToString());
                    writer.WriteEndElement();
                }

                // Write Lower Limit
                if (alarmLimits.LowerLimit != null)
                {
                    writer.WriteStartElement("LowerLimit");
                    writer.WriteString(alarmLimits.LowerLimit.ToString());
                    writer.WriteEndElement();
                }

                // Write Lower Warning
                if (alarmLimits.LowerWarning != null)
                {
                    writer.WriteStartElement("LowerWarning");
                    writer.WriteString(alarmLimits.LowerWarning.ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}
