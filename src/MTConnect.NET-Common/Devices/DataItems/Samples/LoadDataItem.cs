// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// The measurement of the actual versus the standard rating of a piece of equipment.
    /// </summary>
    public class LoadDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "LOAD";
        public const string NameId = "load";
        public const string DefaultUnits = Devices.Units.PERCENT;
        public new const string DescriptionText = "The measurement of the actual versus the standard rating of a piece of equipment.";

        public override string TypeDescription => DescriptionText;


        public LoadDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public LoadDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
            SignificantDigits = 0;
        }
    }
}