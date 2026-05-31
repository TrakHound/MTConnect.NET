// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.ComponentConfigurationParameters
{
    /// <summary>
    /// XML serialization surrogate for a <c>ParameterSet</c>, a named group of
    /// configuration parameters within a ComponentConfigurationParameters
    /// asset.
    /// </summary>
    public class XmlParameterSet
    {
        /// <summary>
        /// The name identifying the parameter set, carried by the <c>name</c>
        /// attribute.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The parameters in this set, serialized as <c>Parameter</c> elements
        /// within <c>Parameters</c>.
        /// </summary>
        [XmlArray("Parameters")]
        [XmlArrayItem("Parameter")]
        public List<XmlParameter> Parameters { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="ParameterSet"/>,
        /// copying the optional name and converting each parameter.
        /// </summary>
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

        /// <summary>
        /// Writes the <c>ParameterSets</c> container with one
        /// <c>ParameterSet</c> element per set; nothing is written when the
        /// collection is empty.
        /// </summary>
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
