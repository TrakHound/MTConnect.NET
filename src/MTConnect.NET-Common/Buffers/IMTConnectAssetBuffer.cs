// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Assets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTConnect.Buffers
{
    /// <summary>
    /// Buffer interface used to store Assets
    /// </summary>
    public interface IMTConnectAssetBuffer
    {
        /// <summary>
        /// Get a unique identifier for the Buffer
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Get the configured size of the Asset Buffer in the number of maximum number of Assets the buffer can hold at one time.
        /// </summary>
        long BufferSize { get; }

        /// <summary>
        /// Get the total number of Assets that are currently in the Buffer
        /// </summary>
        long AssetCount { get; }

        /// <summary>
        /// Get a list of AssetId's that are currently in the Buffer
        /// </summary>
        IEnumerable<string> AssetIds { get; }


        /// <summary>
        /// Get a list of all Assets from the Buffer
        /// </summary>
        IEnumerable<IAsset> GetAssets(string type = null, bool removed = false, int count = 0);

        /// <summary>
        /// Get a list of all Assets from the Buffer
        /// </summary>
        Task<IEnumerable<IAsset>> GetAssetsAsync(string type = null, bool removed = false, int count = 0);

        /// <summary>
        /// Get the specified Asset from the Buffer
        /// </summary>
        /// <param name="assetId">The ID of the Asset to return</param>
        IAsset GetAsset(string assetId);

        /// <summary>
        /// Get the specified Asset from the Buffer
        /// </summary>
        /// <param name="assetId">The ID of the Asset to return</param>
        Task<IAsset> GetAssetAsync(string assetId);


        /// <summary>
        /// Add an Asset to the Buffer
        /// </summary>
        /// <param name="asset">The Asset to add to the Buffer</param>
        bool AddAsset(IAsset asset);

        /// <summary>
        /// Add an Asset to the Buffer
        /// </summary>
        /// <param name="asset">The Asset to add to the Buffer</param>
        Task<bool> AddAssetAsync(IAsset asset);


        /// <summary>
        /// Remove the Asset with the specified Asset ID
        /// </summary>
        /// <param name="assetId">The ID of the Asset to remove</param>
        bool RemoveAsset(string assetId);

        /// <summary>
        /// Remove the Asset with the specified Asset ID
        /// </summary>
        /// <param name="assetId">The ID of the Asset to remove</param>
        Task<bool> RemoveAssetAsync(string assetId);


        /// <summary>
        /// Remove all Assets with the specified Type
        /// </summary>
        /// <param name="assetType">The Type of the Asset(s) to remove</param>
        bool RemoveAllAssets(string assetType);

        /// <summary>
        /// Remove all Assets with the specified Type
        /// </summary>
        /// <param name="assetType">The Type of the Asset(s) to remove</param>
        Task<bool> RemoveAllAssetsAsync(string assetType);
    }
}
