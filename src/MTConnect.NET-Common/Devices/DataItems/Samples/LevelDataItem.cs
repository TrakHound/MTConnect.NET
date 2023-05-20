// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// Level of a resource.
    /// </summary>
    public class LevelDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "LEVEL";
        public const string NameId = "level";
        public const string DefaultUnits = Devices.Units.PERCENT;
        public const int DefaultSignificantDigits = 0;
        public new const string DescriptionText = "Level of a resource.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MaximumVersion => MTConnectVersions.Version11;
        public override System.Version MinimumVersion => MTConnectVersions.Version11;


        public LevelDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public LevelDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
            SignificantDigits = DefaultSignificantDigits;
        }
    }
}