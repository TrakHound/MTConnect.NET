// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.ComponentConfigurationParameters;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.ComponentConfigurationParameters
{
    public class JsonComponentConfigurationParametersAsset
    {
        [JsonPropertyName("assetId")]
        public string AssetId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("deviceUuid")]
        public string DeviceUuid { get; set; }

        [JsonPropertyName("removed")]
        public bool Removed { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("parameterSets")]
        public IEnumerable<JsonParameterSet> ParameterSets { get; set; }


        public JsonComponentConfigurationParametersAsset() { }

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


        public IComponentConfigurationParametersAsset ToComponentConfigurationParametersAsset()
        {
            var asset = new ComponentConfigurationParametersAsset();

            asset.AssetId = AssetId;
            asset.Type = Type;
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