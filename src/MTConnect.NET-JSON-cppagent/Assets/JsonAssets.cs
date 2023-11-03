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
    public class JsonAssets
    {
        //[JsonPropertyName("ComponentConfigurationParameters")]
        //public List<JsonComponentConfigurationParametersAsset> ComponentConfigurationParameters { get; set; }

        [JsonPropertyName("CuttingTool")]
        public List<JsonCuttingToolAsset> CuttingTools { get; set; }

        [JsonPropertyName("File")]
        public List<JsonFileAsset> Files { get; set; }

        //[JsonPropertyName("QIF")]
        //public List<JsonCuttingToolAsset> QIF { get; set; }

        [JsonPropertyName("RawMaterial")]
        public List<JsonRawMaterialAsset> RawMaterials { get; set; }


        public JsonAssets() { }

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