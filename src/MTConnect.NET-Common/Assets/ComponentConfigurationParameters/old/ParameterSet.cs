// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.ComponentConfigurationParameters
{
	/// <summary>
	/// Set of parameters defining the configuration of a Component
	/// </summary>
	public class ParameterSet
    {
		/// <summary>
		/// Set of parameters defining the configuration of a Component
		/// </summary>
		[XmlAttribute("name")]
		[JsonPropertyName("name")]
		public string Name { get; set; }

		/// <summary>
		/// Destinations organizes one or more Destination elements.
		/// </summary>
		[XmlArray("Parameters")]
		[XmlArrayItem("Parameter", typeof(Parameter))]
		[JsonPropertyName("parameters")]
		public List<ParameterSet> Parameters { get; set; }

		[XmlIgnore]
		public bool ParametersSpecified => !Parameters.IsNullOrEmpty();
	}
}