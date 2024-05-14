// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Errors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients
{
    /// <summary>
    /// Client that is used to perform a Assets request from an MTConnect Agent
    /// </summary>
    public interface IMTConnectAssetClient
    {
        /// <summary>
        /// (Optional) The Id of the requested Asset
        /// </summary>
        string AssetId { get; set; }

        /// <summary>
        /// (Optional) The Type of Assets to retrieve
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// (Optional) The maximum Count of Assets to retrieve
        /// </summary>
        long Count { get; set; }

        /// <summary>
        /// Raised when an MTConnectError Document is received
        /// </summary>
        event EventHandler<IErrorResponseDocument> MTConnectError;


        /// <summary>
        /// Execute the Asset Request
        /// </summary>
        IAssetsResponseDocument Get();

        /// <summary>
        /// Asynchronously execute the Asset Request
        /// </summary>
        Task<IAssetsResponseDocument> GetAsync(CancellationToken cancellationToken);
    }
}