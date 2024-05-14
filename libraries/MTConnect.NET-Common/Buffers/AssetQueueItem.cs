// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;

namespace MTConnect.Buffers
{
    struct AssetQueueItem
    {
        public uint Index { get; set; }

        public uint OriginalIndex { get; set; }

        public IAsset Asset { get; set; }


        public AssetQueueItem(uint index, IAsset asset, uint originalIndex = 0)
        {
            Index = index;
            Asset = asset;
            OriginalIndex = originalIndex;
        }
    }
}