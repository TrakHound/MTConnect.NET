// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.Json.CuttingTools;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json
{
    public class JsonAssets
    {
        [JsonPropertyName("CuttingTool")]
        public List<JsonCuttingToolAsset> CuttingTools { get; set; }


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
                    }
                }
            }
        }


        public IEnumerable<IAsset> ToAssets()
        {
            var assets = new List<IAsset>();

            if (!CuttingTools.IsNullOrEmpty())
            {
                foreach (var cuttingTool in CuttingTools) assets.Add(cuttingTool.ToCuttingToolAsset());
            }

            return assets;
        }
    }
}