// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.Specifications;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("SpecificationLimits")]
    public class XmlSpecificationLimits
    {
        [XmlElement("UpperLimit")]
        public double? UpperLimit { get; set; }

        [XmlElement("Nominal")]
        public double? Nominal { get; set; }

        [XmlElement("LowerLimit")]
        public double? LowerLimit { get; set; }


        public ISpecificationLimits ToSpecificationLimits()
        {
            var specificationLimits = new SpecificationLimits();
            specificationLimits.UpperLimit = UpperLimit;
            specificationLimits.Nominal = Nominal;
            specificationLimits.LowerLimit = LowerLimit;
            return specificationLimits;
        }

        public static void WriteXml(XmlWriter writer, ISpecificationLimits specificationLimits)
        {
            if (specificationLimits != null)
            {
                writer.WriteStartElement("ControlLimits");

                // Write Upper Limit
                if (specificationLimits.UpperLimit != null)
                {
                    writer.WriteStartElement("UpperLimit");
                    writer.WriteString(specificationLimits.UpperLimit.ToString());
                    writer.WriteEndElement();
                }

                // Write Nominal
                if (specificationLimits.Nominal != null)
                {
                    writer.WriteStartElement("Nominal");
                    writer.WriteString(specificationLimits.Nominal.ToString());
                    writer.WriteEndElement();
                }

                // Write Lower Limit
                if (specificationLimits.LowerLimit != null)
                {
                    writer.WriteStartElement("LowerLimit");
                    writer.WriteString(specificationLimits.LowerLimit.ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}
