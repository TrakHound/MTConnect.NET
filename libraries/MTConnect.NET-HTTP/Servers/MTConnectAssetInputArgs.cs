// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Servers
{
    /// <summary>
    /// Carries the inputs of a <c>PUT</c>-style request that adds or updates a single asset on
    /// the MTConnect agent. The structure is filled in by the HTTP transport from the request
    /// route, query string, and body, then handed to the agent's asset-handling pipeline.
    /// </summary>
    public struct MTConnectAssetInputArgs
    {
        /// <summary>The MTConnect <c>assetId</c> targeted by the request (typically the last path segment of the asset URL).</summary>
        public string AssetId { get; set; }

        /// <summary>The MTConnect asset <c>type</c> (e.g. <c>CuttingTool</c>, <c>Pallet</c>) supplied with the request and used to dispatch to the matching deserialiser.</summary>
        public string AssetType { get; set; }

        /// <summary>The device key (UUID or name) the asset is associated with; empty if the asset is unattached.</summary>
        public string DeviceKey { get; set; }

        /// <summary>The MTConnect document format of the request body (e.g. <c>xml</c>, <c>json</c>), used to pick the asset deserialiser.</summary>
        public string DocumentFormat { get; set; }

        /// <summary>The raw HTTP request body containing the serialised asset document.</summary>
        public byte[] RequestBody { get; set; }
    }
}
