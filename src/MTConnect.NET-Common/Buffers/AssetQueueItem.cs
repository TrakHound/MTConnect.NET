// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;

namespace MTConnect.Buffers
{
    struct AssetQueueItem
    {
        public int Index { get; set; }

        public int OriginalIndex { get; set; }

        public IAsset Asset { get; set; }


        public AssetQueueItem(int index, IAsset asset, int originalIndex = -1)
        {
            Index = index;
            Asset = asset;
            OriginalIndex = originalIndex;
        }
    }
}