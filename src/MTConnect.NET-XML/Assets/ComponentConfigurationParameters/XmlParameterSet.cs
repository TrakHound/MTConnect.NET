// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.ComponentConfigurationParameters
{
    public class XmlParameterSet
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlArray("Parameters")]
        [XmlArrayItem("Parameter")]
        public List<XmlParameter> Parameters { get; set; }


        public IParameterSet ToParameterSet()
        {
            var parameterSet = new ParameterSet();

            if (!string.IsNullOrEmpty(Name)) parameterSet.Name = Name;

            if (!Parameters.IsNullOrEmpty())
            {
                var parameters = new List<IParameter>();

                foreach (var parameter in Parameters)
                {
                    parameters.Add(parameter.ToParameter());
                }

                parameterSet.Parameters = parameters;
            }

            return parameterSet;
        }

        public static void WriteXml(XmlWriter writer, IEnumerable<IParameterSet> parameterSets)
        {
            if (!parameterSets.IsNullOrEmpty())
            {
                writer.WriteStartElement("ParameterSets");

                foreach (var parameterSet in parameterSets)
                {
                    writer.WriteStartElement("ParameterSet");
                    writer.WriteAttributeString("name", parameterSet.Name);

                    XmlParameter.WriteXml(writer, parameterSet.Parameters);

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}
