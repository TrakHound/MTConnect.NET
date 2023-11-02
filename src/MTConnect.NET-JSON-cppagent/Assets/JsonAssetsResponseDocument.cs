// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json
{
    public class JsonAssetsResponseDocument
    {
        [JsonPropertyName("MTConnectAssets")]
        public JsonMTConnectAssets MTConnectAssets { get; set; }


        public JsonAssetsResponseDocument() { }

        public JsonAssetsResponseDocument(IAssetsResponseDocument assetsDocument)
        {
            MTConnectAssets = new JsonMTConnectAssets(assetsDocument);
        }


        public IAssetsResponseDocument ToAssetsDocument()
        {
            if (MTConnectAssets != null)
            {
                return MTConnectAssets.ToAssetsDocument();
            }

            return null;
        }
    }
}