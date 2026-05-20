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
    /// JSON serialization surrogate that wraps a single asset of any
    /// supported type in the cppagent-compatible shape. The cppagent
    /// asset envelope keys each asset by its element name
    /// (<c>CuttingTool</c>, <c>File</c>, <c>RawMaterial</c>), so this
    /// container exposes one optional property per asset type and only
    /// the property matching the wrapped asset is populated. Converts
    /// to and from the strongly-typed <see cref="IAsset"/> model.
    /// </summary>
    public class JsonAssetContainer
    {
        //[JsonPropertyName("ComponentConfigurationParameters")]
        //public List<JsonComponentConfigurationParametersAsset> ComponentConfigurationParameters { get; set; }

        /// <summary>
        /// The wrapped <c>CuttingTool</c> asset, when the container
        /// holds one.
        /// </summary>
        [JsonPropertyName("CuttingTool")]
        public JsonCuttingToolAsset CuttingTool { get; set; }

        /// <summary>
        /// The wrapped <c>File</c> asset, when the container holds one.
        /// </summary>
        [JsonPropertyName("File")]
        public JsonFileAsset File { get; set; }

        //[JsonPropertyName("QIF")]
        //public List<JsonCuttingToolAsset> QIF { get; set; }

        /// <summary>
        /// The wrapped <c>RawMaterial</c> asset, when the container
        /// holds one.
        /// </summary>
        [JsonPropertyName("RawMaterial")]
        public JsonRawMaterialAsset RawMaterial { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonAssetContainer() { }

        /// <summary>
        /// Initializes the container from a strongly-typed
        /// <see cref="IAsset"/>, dispatching to the appropriate typed
        /// property by the asset's <c>TypeId</c> discriminator.
        /// </summary>
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


        /// <summary>
        /// Wraps the contained asset in a single-element
        /// <see cref="AssetsResponseDocument"/>, returning <c>null</c>
        /// when the container is empty.
        /// </summary>
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

        /// <summary>
        /// Returns the contained asset by inspecting the populated
        /// property, returning <c>null</c> when none is populated.
        /// </summary>
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