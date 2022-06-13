// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Assets;
using MTConnect.Agents.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTConnect.Buffers
{
    /// <summary>
    /// Buffer used to store Assets
    /// </summary>
    public class MTConnectAssetBuffer : IMTConnectAssetBuffer
    {       
        private readonly string _id = Guid.NewGuid().ToString();
        private readonly ConcurrentDictionary<string, IAsset> _storedAssets = new ConcurrentDictionary<string, IAsset>();

        /// <summary>
        /// Get a unique identifier for the Buffer
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// Get the configured size of the Buffer in the number of maximum number of Assets the buffer can hold at one time.
        /// </summary>
        public long BufferSize { get; set; } = 1024;

        /// <summary>
        /// Get the total number of Assets that are currently in the Buffer
        /// </summary>
        public long AssetCount
        {
            get { return _storedAssets.Count; }
        }


        public MTConnectAssetBuffer() { }

        public MTConnectAssetBuffer(MTConnectAgentConfiguration configuration)
        {
            if (configuration != null)
            {
                BufferSize = configuration.MaxAssets;
            }
        }

        protected virtual void OnAssetAdd(IAsset asset) { }


        /// <summary>
        /// Get a list of all Assets from the Buffer
        /// </summary>
        public IEnumerable<IAsset> GetAssets(string type = null, bool removed = false, int count = 100)
        {
            var assets = _storedAssets.Values.ToList();

            // Filter by Type
            if (!string.IsNullOrEmpty(type))
            {
                assets = assets.Where(o => o.Type == type).ToList();
            }

            // Filter Removed Assets
            if (!removed)
            {
                assets = assets.Where(o => !o.Removed).ToList();
            }

            return assets?.Take(100);
        }

        /// <summary>
        /// Get a list of all Assets from the Buffer
        /// </summary>
        public async Task<IEnumerable<IAsset>> GetAssetsAsync(string type = null, bool removed = false, int count = 100)
        {
            var assets = _storedAssets.Values.ToList();

            // Filter by Type
            if (!string.IsNullOrEmpty(type))
            {
                assets = assets.Where(o => o.Type == type).ToList();
            }

            // Filter Removed Assets
            if (!removed)
            {
                assets = assets.Where(o => !o.Removed).ToList();
            }

            return assets?.Take(100);
        }

        /// <summary>
        /// Get the specified Asset from the Buffer
        /// </summary>
        /// <param name="assetId">The ID of the Asset to return</param>
        public IAsset GetAsset(string assetId)
        {
            if (_storedAssets.TryGetValue(assetId, out IAsset asset))
            {
                return asset;
            }

            return null;
        }

        /// <summary>
        /// Get the specified Asset from the Buffer
        /// </summary>
        /// <param name="assetId">The ID of the Asset to return</param>
        public async Task<IAsset> GetAssetAsync(string assetId)
        {
            if (_storedAssets.TryGetValue(assetId, out IAsset asset))
            {
                return asset;
            }

            return null;
        }


        /// <summary>
        /// Add an Asset to the Buffer
        /// </summary>
        /// <param name="asset">The Asset to add to the Buffer</param>
        public bool AddAsset(IAsset asset)
        {
            if (asset != null)
            {
                if (_storedAssets.Count >= BufferSize - 1)
                {
                    _storedAssets.ToList();
                    var firstId = _storedAssets.ToList().FirstOrDefault();
                    _storedAssets.TryRemove(firstId.Key, out var _);
                }

                _storedAssets.TryRemove(asset.AssetId, out var _);
                if (_storedAssets.TryAdd(asset.AssetId, asset))
                {
                    OnAssetAdd(asset);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Add an Asset to the Buffer
        /// </summary>
        /// <param name="asset">The Asset to add to the Buffer</param>
        public async Task<bool> AddAssetAsync(IAsset asset)
        {
            return AddAsset(asset);
        }
    }
}
