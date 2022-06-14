// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Assets;

namespace MTConnect.Buffers
{
    public struct AssetQueueItem
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
