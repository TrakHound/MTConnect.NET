// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.ComponentConfigurationParameters
{
    /// <summary>
    /// XML serialization surrogate for a component-configuration
    /// <c>Parameter</c>. Mirrors the on-the-wire element, where the value is
    /// carried as the element text and its metadata as attributes, and converts
    /// to the strongly-typed <see cref="Parameter"/> model.
    /// </summary>
    public class XmlParameter
    {
        /// <summary>
        /// The identifier that uniquely identifies the parameter.
        /// </summary>
        [XmlAttribute("identifier")]
        public string Identifier { get; set; }

        /// <summary>
        /// The human-readable name of the parameter.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The minimum permitted value, as the raw attribute text.
        /// </summary>
        [XmlAttribute("minimum")]
        public string Minimum { get; set; }

        /// <summary>
        /// The maximum permitted value, as the raw attribute text.
        /// </summary>
        [XmlAttribute("maximum")]
        public string Maximum { get; set; }

        /// <summary>
        /// The nominal (target) value, as the raw attribute text.
        /// </summary>
        [XmlAttribute("nominal")]
        public string Nominal { get; set; }

        /// <summary>
        /// The configured value, carried as the element text.
        /// </summary>
        [XmlText]
        public string Value { get; set; }

        /// <summary>
        /// The engineering units the value is expressed in.
        /// </summary>
        [XmlAttribute("units")]
        public string Units { get; set; }



        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="Parameter"/>, parsing the numeric metadata values.
        /// </summary>
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

        /// <summary>
        /// Writes the given parameters to <paramref name="writer"/> wrapped in
        /// a <c>Parameters</c> element, one <c>Parameter</c> element per item,
        /// omitting optional attributes that are not set.
        /// </summary>
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
