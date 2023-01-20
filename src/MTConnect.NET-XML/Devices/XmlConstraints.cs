// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    public class XmlConstraints
    {
        [XmlElement("Maximum")]
        public string Maximum { get; set; }

        [XmlElement("Minimum")]
        public string Minimum { get; set; }

        [XmlElement("Nominal")]
        public string Nominal { get; set; }

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
                if (!string.IsNullOrEmpty(constraints.Maximum))
                {
                    writer.WriteStartElement("Maximum");
                    writer.WriteString(constraints.Maximum);
                    writer.WriteEndElement();
                }

                // Minimum
                if (!string.IsNullOrEmpty(constraints.Minimum))
                {
                    writer.WriteStartElement("Minimum");
                    writer.WriteString(constraints.Minimum);
                    writer.WriteEndElement();
                }

                // Nominal
                if (!string.IsNullOrEmpty(constraints.Nominal))
                {
                    writer.WriteStartElement("Nominal");
                    writer.WriteString(constraints.Nominal);
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