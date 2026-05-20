// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.Files;
using MTConnect.Assets.Json.CuttingTools;
using MTConnect.Assets.Json.Files;
using MTConnect.Assets.Json.RawMaterials;
using MTConnect.Assets.RawMaterials;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json
{
    /// <summary>
    /// JSON serialization surrogate that holds every asset in an Assets
    /// response document partitioned by asset type into typed lists.
    /// The cppagent shape keys each list by the asset element name
    /// (<c>CuttingTool</c>, <c>File</c>, <c>RawMaterial</c>), so this
    /// container exposes one list per supported asset type and only the
    /// lists with content are populated. Converts to and from a uniform
    /// <see cref="IAsset"/> collection.
    /// </summary>
    public class JsonAssets
    {
        //[JsonPropertyName("ComponentConfigurationParameters")]
        //public List<JsonComponentConfigurationParametersAsset> ComponentConfigurationParameters { get; set; }

        /// <summary>
        /// CuttingTool assets in the document.
        /// </summary>
        [JsonPropertyName("CuttingTool")]
        public List<JsonCuttingToolAsset> CuttingTools { get; set; }

        /// <summary>
        /// File assets in the document.
        /// </summary>
        [JsonPropertyName("File")]
        public List<JsonFileAsset> Files { get; set; }

        //[JsonPropertyName("QIF")]
        //public List<JsonCuttingToolAsset> QIF { get; set; }

        /// <summary>
        /// RawMaterial assets in the document.
        /// </summary>
        [JsonPropertyName("RawMaterial")]
        public List<JsonRawMaterialAsset> RawMaterials { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonAssets() { }

        /// <summary>
        /// Initializes the container from a uniform asset collection,
        /// partitioning each asset into the appropriate typed list by
        /// its <c>TypeId</c> discriminator.
        /// </summary>
        public JsonAssets(IEnumerable<IAsset> assets)
        {
            if (!assets.IsNullOrEmpty())
            {
                foreach (var asset in assets)
                {
                    switch (asset.Type)
                    {
                        case CuttingToolAsset.TypeId:
                            if (CuttingTools == null) CuttingTools = new List<JsonCuttingToolAsset>();
                            CuttingTools.Add(new JsonCuttingToolAsset((ICuttingToolAsset)asset));
                            break;

                        case FileAsset.TypeId:
                            if (Files == null) Files = new List<JsonFileAsset>();
                            Files.Add(new JsonFileAsset((IFileAsset)asset));
                            break;

                        case RawMaterialAsset.TypeId:
                            if (RawMaterials == null) RawMaterials = new List<JsonRawMaterialAsset>();
                            RawMaterials.Add(new JsonRawMaterialAsset((IRawMaterialAsset)asset));
                            break;
                    }
                }
            }
        }


        /// <summary>
        /// Flattens the typed asset lists back into a uniform
        /// <see cref="IAsset"/> collection in CuttingTool, File,
        /// RawMaterial order.
        /// </summary>
        public IEnumerable<IAsset> ToAssets()
        {
            var assets = new List<IAsset>();

            if (!CuttingTools.IsNullOrEmpty())
            {
                foreach (var asset in CuttingTools) assets.Add(asset.ToCuttingToolAsset());
            }

            if (!Files.IsNullOrEmpty())
            {
                foreach (var asset in Files) assets.Add(asset.ToFileAsset());
            }

            if (!RawMaterials.IsNullOrEmpty())
            {
                foreach (var asset in RawMaterials) assets.Add(asset.ToRawMaterialAsset());
            }

            return assets;
        }
    }
}