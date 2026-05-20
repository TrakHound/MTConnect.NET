// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json
{
    /// <summary>
    /// Outer JSON envelope for an Assets response document in the
    /// cppagent-compatible shape, with the actual content wrapped in a
    /// single <c>MTConnectAssets</c> property to mirror the XML root
    /// element name.
    /// </summary>
    public class JsonAssetsResponseDocument
    {
        /// <summary>
        /// The wrapped assets document.
        /// </summary>
        [JsonPropertyName("MTConnectAssets")]
        public JsonMTConnectAssets MTConnectAssets { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonAssetsResponseDocument() { }

        /// <summary>
        /// Initializes the envelope from a strongly-typed assets
        /// document.
        /// </summary>
        public JsonAssetsResponseDocument(IAssetsResponseDocument assetsDocument)
        {
            MTConnectAssets = new JsonMTConnectAssets(assetsDocument);
        }


        /// <summary>
        /// Unwraps the envelope and converts it to a strongly-typed
        /// assets document, returning <c>null</c> when the envelope is
        /// empty.
        /// </summary>
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