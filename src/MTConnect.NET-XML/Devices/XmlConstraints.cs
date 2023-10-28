// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    public class XmlConstraints
    {
        [XmlElement("Maximum")]
        public double? Maximum { get; set; }

        [XmlElement("Minimum")]
        public double? Minimum { get; set; }

        [XmlElement("Nominal")]
        public double? Nominal { get; set; }

        [XmlElement("Value")]
        public List<string> Values { get; set; }

        [XmlElement("Filter")]
        public XmlFilter Filter { get; set; }


        public IConstraints ToConstraints()
        {
            var constraints = new Constraints();
            constraints.Maximum = Maximum;
            constraints.Minimum = Minimum;
            constraints.Nominal = Nominal;
            constraints.Values = Values;
            return constraints;
        }

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