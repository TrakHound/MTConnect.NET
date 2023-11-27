// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json
{
    public class JsonMTConnectAssets
    {
        [JsonPropertyName("Header")]
        public JsonAssetsHeader Header { get; set; }

        [JsonPropertyName("Assets")]
        public JsonAssets Assets { get; set; }


        public JsonMTConnectAssets() { }

        public JsonMTConnectAssets(IAssetsResponseDocument assetsDocument)
        {
            if (assetsDocument != null)
            {
                Header = new JsonAssetsHeader(assetsDocument.Header);

                Assets = new JsonAssets(assetsDocument.Assets);
            }
        }


        public IAssetsResponseDocument ToAssetsDocument()
        {
            var assetsDocument = new AssetsResponseDocument();

            assetsDocument.Header = Header.ToAssetsHeader();

            assetsDocument.Assets = Assets?.ToAssets();

            return assetsDocument;
        }
    }
}