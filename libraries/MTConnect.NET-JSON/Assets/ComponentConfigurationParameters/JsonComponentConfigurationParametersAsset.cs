// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.ComponentConfigurationParameters;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.ComponentConfigurationParameters
{
    /// <summary>
    /// JSON serialization surrogate for an MTConnect
    /// <c>ComponentConfigurationParameters</c> asset, which carries named
    /// parameter sets for a component. Mirrors the on-the-wire shape so the
    /// JSON serializer can read and write it, then converts to and from the
    /// strongly-typed <see cref="ComponentConfigurationParametersAsset"/>
    /// model.
    /// </summary>
    public class JsonComponentConfigurationParametersAsset
    {
        /// <summary>
        /// The unique identifier of the asset.
        /// </summary>
        [JsonPropertyName("assetId")]
        public string AssetId { get; set; }

        /// <summary>
        /// The asset type identifier, <c>ComponentConfigurationParameters</c>.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The timestamp at which the asset was last reported.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The UUID of the device the asset is associated with.
        /// </summary>
        [JsonPropertyName("deviceUuid")]
        public string DeviceUuid { get; set; }

        /// <summary>
        /// Whether the asset has been removed from the agent.
        /// </summary>
        [JsonPropertyName("removed")]
        public bool Removed { get; set; }

        /// <summary>
        /// The free-form description of the asset.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// The named parameter sets defined for the component.
        /// </summary>
        [JsonPropertyName("parameterSets")]
        public IEnumerable<JsonParameterSet> ParameterSets { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization, defaulting
        /// <see cref="Type"/> to the ComponentConfigurationParameters type
        /// identifier.
        /// </summary>
        public JsonComponentConfigurationParametersAsset()
        {
            Type = ComponentConfigurationParametersAsset.TypeId;
        }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IComponentConfigurationParametersAsset"/>.
        /// </summary>
        public JsonComponentConfigurationParametersAsset(IComponentConfigurationParametersAsset asset)
        {
            if (asset != null)
            {
                AssetId = asset.AssetId;
                Type = asset.Type;
                Timestamp = asset.Timestamp;
                DeviceUuid = asset.DeviceUuid;
                Removed = asset.Removed;

                //if (asset.Description != null) Description = new JsonDescription(asset.Description);

                if (!asset.ParameterSets.IsNullOrEmpty())
                {
                    var jsonParameterSets = new List<JsonParameterSet>();
                }
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IComponentConfigurationParametersAsset"/>, converting
        /// each parameter set.
        /// </summary>
        public IComponentConfigurationParametersAsset ToComponentConfigurationParametersAsset()
        {
            var asset = new ComponentConfigurationParametersAsset();

            asset.AssetId = AssetId;
            asset.Timestamp = Timestamp;
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;

            //if (Description != null) asset.Description = Description.ToDescription();

            if (!ParameterSets.IsNullOrEmpty())
            {
                var parameterSets = new List<IParameterSet>();
                foreach (var parameterSet in ParameterSets)
                {
                    parameterSets.Add(parameterSet.ToParameterSet());
                }
                asset.ParameterSets = parameterSets;
            }

            return asset;
        }
    }
}