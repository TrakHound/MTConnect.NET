// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for the <c>AlarmLimits</c> element of a
    /// Specification, carrying the upper/lower alarm and warning bounds as
    /// optional child elements.
    /// </summary>
    [XmlRoot("AlarmLimits")]
    public class XmlAlarmLimits
    {
        /// <summary>
        /// The upper alarm bound, serialized as the optional <c>UpperLimit</c>
        /// element.
        /// </summary>
        [XmlElement("UpperLimit")]
        public double? UpperLimit { get; set; }

        /// <summary>
        /// The upper warning bound, serialized as the optional
        /// <c>UpperWarning</c> element.
        /// </summary>
        [XmlElement("UpperWarning")]
        public double? UpperWarning { get; set; }

        /// <summary>
        /// The lower alarm bound, serialized as the optional <c>LowerLimit</c>
        /// element.
        /// </summary>
        [XmlElement("LowerLimit")]
        public double? LowerLimit { get; set; }

        /// <summary>
        /// The lower warning bound, serialized as the optional
        /// <c>LowerWarning</c> element.
        /// </summary>
        [XmlElement("LowerWarning")]
        public double? LowerWarning { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="AlarmLimits"/>,
        /// copying each present bound.
        /// </summary>
        public IAlarmLimits ToAlarmLimits()
        {
            var alarmLimits = new AlarmLimits();
            alarmLimits.UpperLimit = UpperLimit;
            alarmLimits.UpperWarning = UpperWarning;
            alarmLimits.LowerLimit = LowerLimit;
            alarmLimits.LowerWarning = LowerWarning;
            return alarmLimits;
        }

        /// <summary>
        /// Writes the alarm limits element, emitting only the bounds that are
        /// present.
        /// </summary>
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