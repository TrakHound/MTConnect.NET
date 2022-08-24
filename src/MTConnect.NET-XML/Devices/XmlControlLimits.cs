// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.Specifications;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("ControlLimits")]
    public class XmlControlLimits
    {
        [XmlElement("UpperLimit")]
        public double? UpperLimit { get; set; }

        [XmlElement("UpperWarning")]
        public double? UpperWarning { get; set; }

        [XmlElement("Nominal")]
        public double? Nominal { get; set; }

        [XmlElement("LowerLimit")]
        public double? LowerLimit { get; set; }

        [XmlElement("LowerWarning")]
        public double? LowerWarning { get; set; }


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
