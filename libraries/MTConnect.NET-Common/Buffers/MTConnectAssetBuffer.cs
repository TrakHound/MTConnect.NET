// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Buffers
{
    /// <summary>
    /// Buffer used to store Assets
    /// </summary>
    public class MTConnectAssetBuffer : IMTConnectAssetBuffer
    {       
        private readonly string _id = Guid.NewGuid().ToString();
        private readonly IAsset[] _storedAssets;
        private readonly Dictionary<string, uint> _assetIds = new Dictionary<string, uint>();
        private readonly object _lock = new object();
        private uint _bufferIndex = 0;


        /// <summary>
        /// Get a unique identifier for the Buffer
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// Get the configured size of the Buffer in the number of maximum number of Assets the buffer can hold at one time.
        /// </summary>
        public ulong BufferSize { get; set; } = 1024;

        /// <summary>
        /// Get the total number of Assets that are currently in the Buffer
        /// </summary>
        public ulong AssetCount
        {
            get { return (ulong)_assetIds.Keys.Count(); }
        }

        /// <summary>
        /// Get a list of AssetId's that are currently in the Buffer
        /// </summary>
        public IEnumerable<string> AssetIds
        {
            get { return _assetIds.Keys.ToList(); }
        }

        public event EventHandler<IAsset> AssetRemoved;


        public MTConnectAssetBuffer() 
        {
            _storedAssets = new IAsset[BufferSize];
        }

        public MTConnectAssetBuffer(IAgentConfiguration configuration)
        {
            if (configuration != null)
            {
                BufferSize = configuration.AssetBufferSize;
            }

            _storedAssets = new IAsset[BufferSize];
        }


        protected virtual void OnAssetAdd(uint bufferIndex, IAsset asset, uint originalIndex) { }

        protected virtual void OnAssetRemoved(IAsset asset) { }


        /// <summary>
        /// Get whether the Asset exists in the Buffer or not
        /// </summary>
        /// <param name="assetId">The ID of the Asset to return</param>
        /// <returns>True if the Asset exists</returns>
        public bool AssetExists(string assetId)
        {
            lock (_lock)
            {
                return _assetIds.ContainsKey(assetId);
            }
        }


        /// <summary>
        /// Get a list of all Assets from the Buffer
        /// </summary>
        public IEnumerable<IAsset> GetAssets(string deviceUuid = null, string type = null, bool removed = false, uint count = 100)
        {
            IEnumerable<IAsset> assets;
            lock (_lock) assets = _storedAssets.ToList();

            // Filter by Device
            if (!string.IsNullOrEmpty(deviceUuid))
            {
                assets = assets.Where(o => o != null && o.DeviceUuid == deviceUuid).ToList();
            }

            // Filter by Type
            if (!string.IsNullOrEmpty(type))
            {
                assets = assets.Where(o => o != null && o.Type == type).ToList();
            }

            // Filter Removed Assets
            if (!removed)
            {
                assets = assets.Where(o => o != null && !o.Removed).ToList();
            }

            return assets?.Take((int)count);
        }

        /// <summary>
        /// Get the specified Assets from the Buffer
        /// </summary>
        /// <param name="assetIds">The IDs of the Assets to return</param>
        public IEnumerable<IAsset> GetAssets(IEnumerable<string> assetIds)
        {
            if (!assetIds.IsNullOrEmpty())
            {
                var assets = new List<IAsset>();

                foreach (var assetId in assetIds)
                {
                    lock (_lock)
                    {
                        if (_assetIds.TryGetValue(assetId, out uint index))
                        {
                            assets.Add(_storedAssets[index]);
                        }
                    }
                }

                return assets;
            }        

            return null;
        }

        /// <summary>
        /// Get the specified Asset from the Buffer
        /// </summary>
        /// <param name="assetId">The ID of the Asset to return</param>
        public IAsset GetAsset(string assetId)
        {
            lock (_lock)
            {
                if (_assetIds.TryGetValue(assetId, out uint index))
                {
                    return _storedAssets[index];
                }
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
                if (!string.IsNullOrEmpty(asset.AssetId))
                {
                    uint addIndex = 0;
                    uint originalIndex = 0;
                    uint index;

                    lock (_lock)
                    {
                        if (_assetIds.TryGetValue(asset.AssetId, out index))
                        {
                            // Remove from Dictionary
                            _assetIds.Remove(asset.AssetId);

                            originalIndex = index;

                            // Shift array over at the Index of the Asset ID
                            var a = _storedAssets;
                            Array.Copy(a, index + 1, _storedAssets, index, a.Length - index - 1);

                            // Add existing Asset to the end of the array
                            _storedAssets[_bufferIndex - 1] = asset;
                            addIndex = _bufferIndex - 1;
                        }
                        else
                        {
                            if (_bufferIndex >= BufferSize)
                            {
                                // Raise event to notify that first Asset is being removed
                                if (AssetRemoved != null) AssetRemoved(this, _storedAssets[0]);

                                // Add to end of Buffer (push first item out)
                                var a = _storedAssets;
                                Array.Copy(a, 1, _storedAssets, 0, a.Length - 1);
                                _storedAssets[_storedAssets.Length - 1] = asset;
                                addIndex = (uint)_storedAssets.Length - 1;
                            }
                            else
                            {
                                // Add at current _bufferIndex
                                _storedAssets[_bufferIndex] = asset;
                                addIndex = _bufferIndex;
                                _bufferIndex++;
                            }
                        }

                        // Reset AssetId References
                        _assetIds.Clear();
                        for (uint i = 0; i <= _bufferIndex - 1; i++)
                        {
                            var a = _storedAssets[i];
                            _assetIds.Add(a.AssetId, i);
                        }
                    }

                    OnAssetAdd(addIndex, asset, originalIndex);
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Remove the Asset with the specified Asset ID
        /// </summary>
        /// <param name="assetId">The ID of the Asset to remove</param>
        public bool RemoveAsset(string assetId)
        {
            if (!string.IsNullOrEmpty(assetId))
            {
                var asset = GetAsset(assetId);
                if (asset != null)
                {
                    ((Asset)asset).Removed = true;
                    return AddAsset(asset);
                }
            }

            return false;
        }

        /// <summary>
        /// Remove all Assets with the specified Type
        /// </summary>
        /// <param name="assetType">The Type of the Asset(s) to remove</param>
        public bool RemoveAllAssets(string assetType)
        {
            if (!string.IsNullOrEmpty(assetType))
            {
                var assets = GetAssets(assetType);
                if (!assets.IsNullOrEmpty())
                {
                    foreach (var asset in assets)
                    {
                        ((Asset)asset).Removed = true;
                        return AddAsset(asset);
                    }
                }
            }

            return false;
        }
    }
}