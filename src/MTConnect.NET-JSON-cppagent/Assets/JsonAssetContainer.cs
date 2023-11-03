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
    public class JsonAssetContainer
    {
        //[JsonPropertyName("ComponentConfigurationParameters")]
        //public List<JsonComponentConfigurationParametersAsset> ComponentConfigurationParameters { get; set; }

        [JsonPropertyName("CuttingTool")]
        public JsonCuttingToolAsset CuttingTool { get; set; }

        [JsonPropertyName("File")]
        public JsonFileAsset File { get; set; }

        //[JsonPropertyName("QIF")]
        //public List<JsonCuttingToolAsset> QIF { get; set; }

        [JsonPropertyName("RawMaterial")]
        public JsonRawMaterialAsset RawMaterial { get; set; }


        public JsonAssetContainer() { }

        public JsonAssetContainer(IAsset asset)
        {
            if (asset != null)
            {
                switch (asset.Type)
                {
                    case CuttingToolAsset.TypeId:
                        CuttingTool = new JsonCuttingToolAsset((ICuttingToolAsset)asset);
                        break;

                    case FileAsset.TypeId:
                        File = new JsonFileAsset((IFileAsset)asset);
                        break;

                    case RawMaterialAsset.TypeId:
                        RawMaterial = new JsonRawMaterialAsset((IRawMaterialAsset)asset);
                        break;
                }
            }
        }


        public IAssetsResponseDocument ToAssetsDocument()
        {
            var asset = ToAsset();
            if (asset != null)
            {
                var document = new AssetsResponseDocument();

                // Needs a Header?

                var assets = new List<IAsset>();
                assets.Add(asset);
                document.Assets = assets;

                return document;
            }

            return null;
        }

        public IAsset ToAsset()
        {
            if (CuttingTool != null)
            {
                return CuttingTool.ToCuttingToolAsset();
            }

            if (File != null)
            {
                return File.ToFileAsset();
            }

            if (RawMaterial != null)
            {
                return RawMaterial.ToRawMaterialAsset();
            }

            return null;
        }
    }
}