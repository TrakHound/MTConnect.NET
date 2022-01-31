// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json
{
    public class JsonAssetsDocument
    {
        /// <summary>
        /// Contains the Header information in an MTConnect Assets XML document
        /// </summary>
        [JsonPropertyName("header")]
        public Headers.MTConnectAssetsHeader Header { get; set; }

        /// <summary>
        /// An XML container that consists of one or more types of Asset XML elements.
        /// </summary>
        [JsonPropertyName("assets")]
        public List<object> Assets { get; set; }


        public JsonAssetsDocument(AssetsDocument assetsDocument)
        {
            if (assetsDocument != null)
            {
                Header = assetsDocument.Header;

                // Add Assets
                if (!assetsDocument.Assets.IsNullOrEmpty())
                {
                    var assets = new List<object>();
                    foreach (var asset in assetsDocument.Assets)
                    {
                        assets.Add(asset);
                    }
                    Assets = assets;
                }
            }
        }


        public AssetsDocument ToAssetsDocument()
        {
            var assetsDocument = new AssetsDocument();
            assetsDocument.Header = Header;

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
