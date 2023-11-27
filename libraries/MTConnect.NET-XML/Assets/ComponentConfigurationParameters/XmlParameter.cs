// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.ComponentConfigurationParameters
{
    public class XmlParameter
    {
        [XmlAttribute("identifier")]
        public string Identifier { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("minimum")]
        public string Minimum { get; set; }

        [XmlAttribute("maximum")]
        public string Maximum { get; set; }

        [XmlAttribute("nominal")]
        public string Nominal { get; set; }

        [XmlText]
        public string Value { get; set; }

        [XmlAttribute("units")]
        public string Units { get; set; }



        public IParameter ToParameter()
        {
            var parameter = new Parameter();

            parameter.Identifier = Identifier;
            parameter.Name = Name;

            if (!string.IsNullOrEmpty(Minimum)) parameter.Minimum = Minimum.ToDouble();
            if (!string.IsNullOrEmpty(Maximum)) parameter.Maximum = Maximum.ToDouble();
            if (!string.IsNullOrEmpty(Nominal)) parameter.Nominal = Nominal.ToDouble();
            if (!string.IsNullOrEmpty(Value)) parameter.Value = Value;
            if (!string.IsNullOrEmpty(Units)) parameter.Units = Units;

            return parameter;
        }

        public static void WriteXml(XmlWriter writer, IEnumerable<IParameter> parameters)
        {
            if (!parameters.IsNullOrEmpty())
            {
                writer.WriteStartElement("Parameters");

                foreach (var parameter in parameters)
                {
                    writer.WriteStartElement("Parameter");
                    writer.WriteAttributeString("identifier", parameter.Identifier);
                    writer.WriteAttributeString("name", parameter.Name);
                    if (parameter.Minimum != null) writer.WriteAttributeString("minimum", parameter.Minimum.ToString());
                    if (parameter.Maximum != null) writer.WriteAttributeString("maximum", parameter.Maximum.ToString());
                    if (parameter.Nominal != null) writer.WriteAttributeString("nominal", parameter.Nominal.ToString());
                    if (!string.IsNullOrEmpty(parameter.Units)) writer.WriteAttributeString("units", parameter.Units);
                    writer.WriteString(parameter.Value);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}
