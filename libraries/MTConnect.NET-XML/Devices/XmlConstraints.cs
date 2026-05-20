// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for the <c>Constraints</c> on a data item's
    /// reported value. Mirrors the on-the-wire element and converts to and from
    /// the strongly-typed <see cref="Constraints"/> model.
    /// </summary>
    public class XmlConstraints
    {
        /// <summary>
        /// The maximum value the data item may report.
        /// </summary>
        [XmlElement("Maximum")]
        public double? Maximum { get; set; }

        /// <summary>
        /// The minimum value the data item may report.
        /// </summary>
        [XmlElement("Minimum")]
        public double? Minimum { get; set; }

        /// <summary>
        /// The nominal (target) value of the data item.
        /// </summary>
        [XmlElement("Nominal")]
        public double? Nominal { get; set; }

        /// <summary>
        /// The discrete set of values the data item is constrained to.
        /// </summary>
        [XmlElement("Value")]
        public List<string> Values { get; set; }

        /// <summary>
        /// The filter applied to the data item's reported values.
        /// </summary>
        [XmlElement("Filter")]
        public XmlFilter Filter { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="Constraints"/>.
        /// </summary>
        public IConstraints ToConstraints()
        {
            var constraints = new Constraints();
            constraints.Maximum = Maximum;
            constraints.Minimum = Minimum;
            constraints.Nominal = Nominal;
            constraints.Values = Values;
            return constraints;
        }

        /// <summary>
        /// Writes the given <see cref="IConstraints"/> to
        /// <paramref name="writer"/> as a <c>Constraints</c> element, emitting
        /// only the bounds, enumerated values, and filter that are set.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IConstraints constraints)
        {
            if (constraints != null)
            {
                writer.WriteStartElement("Constraints");

                // Maximum
                if (constraints.Maximum != null)
                {
                    writer.WriteStartElement("Maximum");
                    writer.WriteString(constraints.Maximum.ToString());
                    writer.WriteEndElement();
                }

                // Minimum
                if (constraints.Minimum != null)
                {
                    writer.WriteStartElement("Minimum");
                    writer.WriteString(constraints.Minimum.ToString());
                    writer.WriteEndElement();
                }

                // Nominal
                if (constraints.Nominal != null)
                {
                    writer.WriteStartElement("Nominal");
                    writer.WriteString(constraints.Nominal.ToString());
                    writer.WriteEndElement();
                }

                // Write Values
                if (!constraints.Values.IsNullOrEmpty())
                {
                    foreach (var value in constraints.Values)
                    {
                        writer.WriteStartElement("Value");
                        writer.WriteString(value);
                        writer.WriteEndElement();
                    }
                }

                // Filter
                XmlFilter.WriteXml(writer, constraints.Filter);

                writer.WriteEndElement();
            }
        }
    }
}