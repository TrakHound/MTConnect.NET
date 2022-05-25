// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// Data Set of the number of Assets of a given type for a Device.
    /// </summary>
    public class AssetCountDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ASSET_COUNT";
        public const string NameId = "assetCount";
        public new const string DescriptionText = "Data Set of the number of Assets of a given type for a Device.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version20;


        public AssetCountDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public AssetCountDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
