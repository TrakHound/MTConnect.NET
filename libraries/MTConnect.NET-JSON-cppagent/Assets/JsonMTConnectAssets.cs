// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json
{
    /// <summary>
    /// JSON serialization surrogate for the top-level
    /// <c>MTConnectAssets</c> document in the cppagent-compatible
    /// shape. Sits inside a <see cref="JsonAssetsResponseDocument"/>
    /// envelope and carries the agent header plus the typed
    /// asset container. Converts to and from the strongly-typed
    /// <see cref="AssetsResponseDocument"/> model.
    /// </summary>
    public class JsonMTConnectAssets
    {
        /// <summary>
        /// The MTConnect Agent header.
        /// </summary>
        [JsonPropertyName("Header")]
        public JsonAssetsHeader Header { get; set; }

        /// <summary>
        /// The typed asset container holding every asset in the
        /// document.
        /// </summary>
        [JsonPropertyName("Assets")]
        public JsonAssets Assets { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonMTConnectAssets() { }

        /// <summary>
        /// Initializes the container from a strongly-typed
        /// <see cref="IAssetsResponseDocument"/>.
        /// </summary>
        public JsonMTConnectAssets(IAssetsResponseDocument assetsDocument)
        {
            if (assetsDocument != null)
            {
                Header = new JsonAssetsHeader(assetsDocument.Header);

                Assets = new JsonAssets(assetsDocument.Assets);
            }
        }


        /// <summary>
        /// Converts the container to a strongly-typed
        /// <see cref="AssetsResponseDocument"/>.
        /// </summary>
        public IAssetsResponseDocument ToAssetsDocument()
        {
            var assetsDocument = new AssetsResponseDocument();

            assetsDocument.Header = Header.ToAssetsHeader();

            assetsDocument.Assets = Assets?.ToAssets();

            return assetsDocument;
        }
    }
}