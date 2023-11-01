// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.ComponentConfigurationParameters;
using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.Files;
using MTConnect.Assets.Json.ComponentConfigurationParameters;
using MTConnect.Assets.Json.CuttingTools;
using MTConnect.Assets.Json.Files;
using MTConnect.Assets.Json.QIF;
using MTConnect.Assets.Json.RawMaterials;
using MTConnect.Assets.QIF;
using MTConnect.Assets.RawMaterials;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json
{
    public class JsonAssetsDocument
    {
        [JsonPropertyName("header")]
        public JsonAssetsHeader Header { get; set; }

        [JsonPropertyName("assets")]
        public List<object> Assets { get; set; }


        public JsonAssetsDocument(IAssetsResponseDocument assetsDocument)
        {
            if (assetsDocument != null)
            {
                Header = new JsonAssetsHeader(assetsDocument.Header);

                // Add Assets
                if (!assetsDocument.Assets.IsNullOrEmpty())
                {
                    var assets = new List<object>();
                    foreach (var asset in assetsDocument.Assets)
                    {
                        object jsonAsset = null;

                        switch (asset.Type)
                        {
                            case "ComponentConfigurationParameters": jsonAsset = new JsonComponentConfigurationParametersAsset(asset as ComponentConfigurationParametersAsset); break;
                            case "CuttingTool": jsonAsset = new JsonCuttingToolAsset(asset as CuttingToolAsset); break;
                            case "File": jsonAsset = new JsonFileAsset(asset as FileAsset); break;
                            case "QIFDocumentWrapper": jsonAsset = new JsonQIFDocumentWrapperAsset(asset as QIFDocumentWrapperAsset); break;
                            case "RawMaterial": jsonAsset = new JsonRawMaterialAsset(asset as RawMaterialAsset); break;
                        }

                        if (jsonAsset != null) assets.Add(jsonAsset);
                    }
                    Assets = assets;
                }
            }
        }


        public AssetsResponseDocument ToAssetsDocument()
        {
            var assetsDocument = new AssetsResponseDocument();

            assetsDocument.Header = Header.ToAssetsHeader();

            // Add Assets
            if (!Assets.IsNullOrEmpty())
            {
                var assets = new List<IAsset>();
                foreach (var asset in Assets)
                {
                    //assets.Add(asset);
                }
                assetsDocument.Assets = assets;
            }
            else assetsDocument.Assets = new List<IAsset>();

            return assetsDocument;
        }
    }
}