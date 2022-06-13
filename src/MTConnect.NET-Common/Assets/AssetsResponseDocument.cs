// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Headers;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets
{
    /// <summary>
    /// The Asset Information Model associates each electronic MTConnectAssets document with a unique
    /// identifier and allows for some predefined mechanisms to find, create, request, updated, and delete these
    /// electronic documents in a way that provides for consistency across multiple pieces of equipment.
    /// </summary>
    public class AssetsResponseDocument : IAssetsResponseDocument
    {
        /// <summary>
        /// Contains the Header information in an MTConnect Assets XML document
        /// </summary>
        [JsonPropertyName("header")]
        public IMTConnectAssetsHeader Header { get; set; }

        /// <summary>
        /// An XML container that consists of one or more types of Asset XML elements.
        /// </summary>
        [JsonPropertyName("assets")]
        public IEnumerable<IAsset> Assets { get; set; }

        [JsonIgnore]
        public Version Version { get; set; }

        //[JsonIgnore]
        //public string Xml { get; set; }
    }
}
