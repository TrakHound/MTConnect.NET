// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// The value of the CDATA for the event MUST be the assetId of the asset that has been removed. 
    /// The asset will still be visible if requested with the includeRemoved parameter as described in the protocol section. 
    /// When assets are removed they are not moved to the beginning of the most recently modified list.
    /// </summary>
    public class AssetRemovedDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ASSET_REMOVED";
        public const string NameId = "assetRemoved";


        public AssetRemovedDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
            MinimumVersion = MTConnectVersions.Version14;
        }

        public AssetRemovedDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
            Name = NameId;
            MinimumVersion = MTConnectVersions.Version14;
        }
    }
}
