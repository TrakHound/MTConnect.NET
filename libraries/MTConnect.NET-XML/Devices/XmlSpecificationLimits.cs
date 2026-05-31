// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for the <c>SpecificationLimits</c> element
    /// of a Specification, carrying the upper/nominal/lower acceptance bounds
    /// as optional child elements.
    /// </summary>
    [XmlRoot("SpecificationLimits")]
    public class XmlSpecificationLimits
    {
        /// <summary>
        /// The upper acceptance bound, serialized as the optional
        /// <c>UpperLimit</c> element.
        /// </summary>
        [XmlElement("UpperLimit")]
        public double? UpperLimit { get; set; }

        /// <summary>
        /// The nominal (target) value, serialized as the optional
        /// <c>Nominal</c> element.
        /// </summary>
        [XmlElement("Nominal")]
        public double? Nominal { get; set; }

        /// <summary>
        /// The lower acceptance bound, serialized as the optional
        /// <c>LowerLimit</c> element.
        /// </summary>
        [XmlElement("LowerLimit")]
        public double? LowerLimit { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="SpecificationLimits"/>, copying each present bound.
        /// </summary>
        public ISpecificationLimits ToSpecificationLimits()
        {
            var specificationLimits = new SpecificationLimits();
            specificationLimits.UpperLimit = UpperLimit;
            specificationLimits.Nominal = Nominal;
            specificationLimits.LowerLimit = LowerLimit;
            return specificationLimits;
        }

        /// <summary>
        /// Writes the specification limits element, emitting only the bounds
        /// that are present.
        /// </summary>
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