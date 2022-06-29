// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Headers;
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
        public MTConnectAssetsHeader Header { get; set; }

        /// <summary>
        /// An XML container that consists of one or more types of Asset XML elements.
        /// </summary>
        [JsonPropertyName("assets")]
        public List<object> Assets { get; set; }


        public JsonAssetsDocument(IAssetsResponseDocument assetsDocument)
        {
            if (assetsDocument != null)
            {
                var header = new MTConnectAssetsHeader();
                header.InstanceId = assetsDocument.Header.InstanceId;
                header.Version = assetsDocument.Header.Version;
                header.Sender = assetsDocument.Header.Sender;
                header.AssetBufferSize = assetsDocument.Header.AssetBufferSize;
                header.AssetCount = assetsDocument.Header.AssetCount;
                header.DeviceModelChangeTime = assetsDocument.Header.DeviceModelChangeTime;
                header.TestIndicator = assetsDocument.Header.TestIndicator;
                header.CreationTime = assetsDocument.Header.CreationTime;
                Header = header;

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


        public AssetsResponseDocument ToAssetsDocument()
        {
            var assetsDocument = new AssetsResponseDocument();
            assetsDocument.Header = Header;

            // Add Assets
            if (!Assets.IsNullOrEmpty())
            {
                //var assets = new List<object>();
                //foreach (var asset in Assets)
                //{
                //    assets.Add(asset);
                //}
                //assetsDocument.Assets = assets;
            }
            else assetsDocument.Assets = new List<IAsset>();

            return assetsDocument;
        }
    }
}
