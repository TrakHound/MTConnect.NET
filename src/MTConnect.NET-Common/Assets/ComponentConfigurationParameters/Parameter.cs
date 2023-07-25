// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.ComponentConfigurationParameters
{
	/// <summary>
	/// Property defining a configuration of a Component
	/// </summary>
	public class Parameter
    {
		/// <summary>
		/// Internal identifier, register, or address.
		/// </summary>
		[XmlAttribute("identifier")]
		[JsonPropertyName("identifier")]
		public string Identifier { get; set; }

		/// <summary>
		/// Descriptive name.
		/// </summary>
		[XmlAttribute("name")]
		[JsonPropertyName("name")]
		public string Name { get; set; }

		/// <summary>
		/// Minimal allowed value.
		/// </summary>
		[XmlAttribute("minimum")]
		[JsonPropertyName("minimum")]
		public string Minimum { get; set; }

		/// <summary>
		/// Maximum allowed value.
		/// </summary>
		[XmlAttribute("maximum")]
		[JsonPropertyName("maximum")]
		public string Maximum { get; set; }

		/// <summary>
		/// Nominal value.
		/// </summary>
		[XmlAttribute("nominal")]
		[JsonPropertyName("nominal")]
		public string Nominal { get; set; }

		/// <summary>
		/// Configured value.
		/// </summary>
		[XmlAttribute("value")]
		[JsonPropertyName("value")]
		public string Value { get; set; }

		/// <summary>
		/// Engineering units. Units SHOULD be SI or MTConnect Units(See UnitEnum).
		/// </summary>
		[XmlAttribute("units")]
		[JsonPropertyName("units")]
		public string Units { get; set; }
	}
}