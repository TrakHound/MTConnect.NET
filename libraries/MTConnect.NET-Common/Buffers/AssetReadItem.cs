// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;

namespace MTConnect.Buffers
{
    struct AssetReadItem
    {
        public int Index { get; set; }

        public IAsset Asset { get; set; }


        public AssetReadItem(int index, IAsset asset)
        {
            Index = index;
            Asset = asset;
        }
    }
}