// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for the <c>ControlLimits</c> of a process
    /// specification, the statistical-control boundaries of a measured value.
    /// Mirrors the on-the-wire element and converts to and from the
    /// strongly-typed <see cref="ControlLimits"/> model.
    /// </summary>
    [XmlRoot("ControlLimits")]
    public class XmlControlLimits
    {
        /// <summary>
        /// The upper control limit.
        /// </summary>
        [XmlElement("UpperLimit")]
        public double? UpperLimit { get; set; }

        /// <summary>
        /// The upper boundary indicating a warning condition.
        /// </summary>
        [XmlElement("UpperWarning")]
        public double? UpperWarning { get; set; }

        /// <summary>
        /// The centre line (target) value.
        /// </summary>
        [XmlElement("Nominal")]
        public double? Nominal { get; set; }

        /// <summary>
        /// The lower control limit.
        /// </summary>
        [XmlElement("LowerLimit")]
        public double? LowerLimit { get; set; }

        /// <summary>
        /// The lower boundary indicating a warning condition.
        /// </summary>
        [XmlElement("LowerWarning")]
        public double? LowerWarning { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="ControlLimits"/>.
        /// </summary>
        public IControlLimits ToControlLimits()
        {
            var controlLimits = new ControlLimits();
            controlLimits.UpperLimit = UpperLimit;
            controlLimits.UpperWarning = UpperWarning;
            controlLimits.Nominal = Nominal;
            controlLimits.LowerLimit = LowerLimit;
            controlLimits.LowerWarning = LowerWarning;
            return controlLimits;
        }

        /// <summary>
        /// Writes the given <see cref="IControlLimits"/> to
        /// <paramref name="writer"/> as a <c>ControlLimits</c> element,
        /// emitting only the limit elements that are set.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IControlLimits controlLimits)
        {
            if (controlLimits != null)
            {
                writer.WriteStartElement("ControlLimits");

                // Write Upper Limit
                if (controlLimits.UpperLimit != null)
                {
                    writer.WriteStartElement("UpperLimit");
                    writer.WriteString(controlLimits.UpperLimit.ToString());
                    writer.WriteEndElement();
                }

                // Write Upper Warning
                if (controlLimits.UpperWarning != null)
                {
                    writer.WriteStartElement("UpperWarning");
                    writer.WriteString(controlLimits.UpperWarning.ToString());
                    writer.WriteEndElement();
                }

                // Write Nominal
                if (controlLimits.Nominal != null)
                {
                    writer.WriteStartElement("Nominal");
                    writer.WriteString(controlLimits.Nominal.ToString());
                    writer.WriteEndElement();
                }

                // Write Lower Limit
                if (controlLimits.LowerLimit != null)
                {
                    writer.WriteStartElement("LowerLimit");
                    writer.WriteString(controlLimits.LowerLimit.ToString());
                    writer.WriteEndElement();
                }

                // Write Lower Warning
                if (controlLimits.LowerWarning != null)
                {
                    writer.WriteStartElement("LowerWarning");
                    writer.WriteString(controlLimits.LowerWarning.ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}