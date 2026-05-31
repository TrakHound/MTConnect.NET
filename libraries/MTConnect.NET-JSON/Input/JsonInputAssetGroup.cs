// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Assets.ComponentConfigurationParameters;
using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.Files;
using MTConnect.Assets.Json.ComponentConfigurationParameters;
using MTConnect.Assets.Json.CuttingTools;
using MTConnect.Assets.Json.Files;
using MTConnect.Assets.Json.RawMaterials;
using MTConnect.Assets.RawMaterials;
using MTConnect.Input;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Mqtt
{
    /// <summary>
    /// JSON serialization surrogate for a group of asset inputs partitioned by
    /// asset type. Used to carry multiple assets of different types in a single
    /// JSON payload over transports such as MQTT.
    /// </summary>
    public class JsonInputAssetGroup
    {
        /// <summary>
        /// The ComponentConfigurationParameters assets in the group.
        /// </summary>
        [JsonPropertyName("componentConfigurationParameters")]
        public List<JsonComponentConfigurationParametersAsset> ComponentConfigurationParameters { get; set; }

        /// <summary>
        /// The CuttingTool assets in the group.
        /// </summary>
        [JsonPropertyName("cuttingTool")]
        public List<JsonCuttingToolAsset> CuttingTools { get; set; }

        /// <summary>
        /// The File assets in the group.
        /// </summary>
        [JsonPropertyName("file")]
        public List<JsonFileAsset> Files { get; set; }

        /// <summary>
        /// The RawMaterial assets in the group.
        /// </summary>
        [JsonPropertyName("rawMaterials")]
        public List<JsonRawMaterialAsset> RawMaterials { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonInputAssetGroup() { }

        /// <summary>
        /// Initializes the surrogate from a collection of asset inputs,
        /// dispatching each by its asset type to the matching surrogate list.
        /// </summary>
        public JsonInputAssetGroup(IEnumerable<IAssetInput> assets)
        {
            if (!assets.IsNullOrEmpty())
            {
                foreach (var asset in assets)
                {
                    if (asset != null && asset.Asset != null)
                    {
                        switch (asset.Type)
                        {
                            case ComponentConfigurationParametersAsset.TypeId:
                                var componentConfigurationParametersAsset = new JsonComponentConfigurationParametersAsset(asset.Asset as IComponentConfigurationParametersAsset);
                                if (componentConfigurationParametersAsset != null)
                                {
                                    if (ComponentConfigurationParameters == null) ComponentConfigurationParameters = new List<JsonComponentConfigurationParametersAsset>();
                                    ComponentConfigurationParameters.Add(componentConfigurationParametersAsset);
                                }
                                break;

                            case CuttingToolAsset.TypeId:
                                var cuttingToolAsset = new JsonCuttingToolAsset(asset.Asset as ICuttingToolAsset);
                                if (cuttingToolAsset != null)
                                {
                                    if (CuttingTools == null) CuttingTools = new List<JsonCuttingToolAsset>();
                                    CuttingTools.Add(cuttingToolAsset);
                                }
                                break;

                            case FileAsset.TypeId:
                                var fileAsset = new JsonFileAsset(asset.Asset as IFileAsset);
                                if (fileAsset != null)
                                {
                                    if (Files == null) Files = new List<JsonFileAsset>();
                                    Files.Add(fileAsset);
                                }
                                break;

                            case RawMaterialAsset.TypeId:
                                var rawMaterialAsset = new JsonRawMaterialAsset(asset.Asset as IRawMaterialAsset);
                                if (rawMaterialAsset != null)
                                {
                                    if (RawMaterials == null) RawMaterials = new List<JsonRawMaterialAsset>();
                                    RawMaterials.Add(rawMaterialAsset);
                                }
                                break;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Flattens the type-partitioned surrogates of
        /// <paramref name="inputAssetGroup"/> into a single asset collection by
        /// converting each surrogate to its strongly-typed asset.
        /// </summary>
        public static IEnumerable<IAsset> ToAssets(JsonInputAssetGroup inputAssetGroup)
        {
            var assets = new List<IAsset>();

            if (inputAssetGroup != null)
            {
                // ComponentConfigurationParameters
                if (!inputAssetGroup.ComponentConfigurationParameters.IsNullOrEmpty())
                {
                    foreach (var x in inputAssetGroup.ComponentConfigurationParameters)
                    {
                        var asset = x.ToComponentConfigurationParametersAsset();
                        if (asset != null) assets.Add(asset);
                    }
                }

                // CuttingTools
                if (!inputAssetGroup.CuttingTools.IsNullOrEmpty())
                {
                    foreach (var x in inputAssetGroup.CuttingTools)
                    {
                        var asset = x.ToCuttingToolAsset();
                        if (asset != null) assets.Add(asset);
                    }
                }

                // Files
                if (!inputAssetGroup.Files.IsNullOrEmpty())
                {
                    foreach (var x in inputAssetGroup.Files)
                    {
                        var asset = x.ToFileAsset();
                        if (asset != null) assets.Add(asset);
                    }
                }

                // RawMaterials
                if (!inputAssetGroup.RawMaterials.IsNullOrEmpty())
                {
                    foreach (var x in inputAssetGroup.RawMaterials)
                    {
                        var asset = x.ToRawMaterialAsset();
                        if (asset != null) assets.Add(asset);
                    }
                }
            }

            return assets;
        }
    }
}
