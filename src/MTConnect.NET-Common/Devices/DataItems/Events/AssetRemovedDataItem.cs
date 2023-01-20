// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// The value of the Result for the event MUST be the assetId of the asset that has been removed. 
    /// The asset will still be visible if requested with the includeRemoved parameter as described in the protocol section. 
    /// When assets are removed they are not moved to the beginning of the most recently modified list.
    /// </summary>
    public class AssetRemovedDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ASSET_REMOVED";
        public const string NameId = "assetRemoved";
        public new const string DescriptionText = "The value of the Result for the event MUST be the assetId of the asset that has been removed. The asset will still be visible if requested with the includeRemoved parameter as described in the protocol section. When assets are removed they are not moved to the beginning of the most recently modified list.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        public AssetRemovedDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public AssetRemovedDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
