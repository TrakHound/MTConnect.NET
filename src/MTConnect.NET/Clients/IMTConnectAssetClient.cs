// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Clients
{
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
        /// User settable object sent with request and returned in Document on response
        /// </summary>
        object UserObject { get; set; }

        /// <summary>
        /// Raised when an MTConnectError Document is received
        /// </summary>
        EventHandler<Errors.ErrorDocument> OnMTConnectError { get; set; }


        /// <summary>
        /// Execute the Asset Request
        /// </summary>
        Assets.AssetsDocument Get();

        /// <summary>
        /// Asynchronously execute the Asset Request
        /// </summary>
        Task<Assets.AssetsDocument> GetAsync(CancellationToken cancellationToken);
    }
}
