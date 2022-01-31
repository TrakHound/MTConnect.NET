// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Samples
{
    /// <summary>
    /// The average rate of change of values for assets in the MTConnect streams. 
    /// The average is computed over a rolling window defined by the implementation.
    /// </summary>
    public class AssetUpdateRateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "ASSET_UPDATE_RATE";
        public const string NameId = "assetUpdateRate";


        public AssetUpdateRateDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
            Units = Devices.Units.COUNT_PER_SECOND;
        }

        public AssetUpdateRateDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = Devices.Units.COUNT_PER_SECOND;
        }
    }
}
