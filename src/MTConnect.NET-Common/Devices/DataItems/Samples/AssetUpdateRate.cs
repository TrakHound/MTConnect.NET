// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Samples
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
        public const string DefaultUnits = Devices.Units.COUNT_PER_SECOND;
        public const DataItemStatistic DefaultStatistic = DataItemStatistic.AVERAGE;
        public new const string DescriptionText = "The average rate of change of values for assets in the MTConnect streams. The average is computed over a rolling window defined by the implementation.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public AssetUpdateRateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
            Statistic = DefaultStatistic;
            SignificantDigits = 1;
        }

        public AssetUpdateRateDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
            Statistic = DefaultStatistic;
            SignificantDigits = 1;
        }
    }
}