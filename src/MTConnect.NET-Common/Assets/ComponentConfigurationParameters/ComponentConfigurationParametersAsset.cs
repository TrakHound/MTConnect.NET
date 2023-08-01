// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MTConnect.Assets.ComponentConfigurationParameters
{
	/// <summary>
	/// Set of parameters that govern the functionality of the related Component
	/// </summary>
	[XmlRoot("ComponentConfigurationParameters")]
    public class ComponentConfigurationParametersAsset : Asset
	{
        public const string TypeId = "ComponentConfigurationParameters";


        /// <summary>
        /// Destinations organizes one or more Destination elements.
        /// </summary>
        [XmlArray("ParameterSets")]
        [XmlArrayItem("ParameterSet", typeof(ParameterSet))]
        [JsonPropertyName("parameterSets")]
        public List<ParameterSet> ParameterSets { get; set; }

        [XmlIgnore]
        public bool ParameterSetsSpecified => !ParameterSets.IsNullOrEmpty();


        public ComponentConfigurationParametersAsset()
        {
            Type = TypeId;
        }


        protected override IAsset OnProcess(Version mtconnectVersion)
        {
			if (mtconnectVersion != null && mtconnectVersion >= MTConnectVersions.Version22)
			{
				return this;
			}

			return null;
		}

        public override AssetValidationResult IsValid(Version mtconnectVersion)
        {
			var message = "";
			var result = true;

			if (ParameterSets.IsNullOrEmpty())
			{
				message = "At least one ParameterSet is Required";
				result = false;
			}

			return new AssetValidationResult(result, message);
		}
    }
}