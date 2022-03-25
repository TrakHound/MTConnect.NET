// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Samples
{
    /// <summary>
    /// The measurement of accumulated time for an activity or event.
    /// </summary>
    public class AccumulatedTimeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "ACCUMULATED_TIME";
        public const string NameId = "time";
        public const string DefaultUnits = Devices.Units.SECOND;
        public new const string DescriptionText = "The measurement of accumulated time for an activity or event.";

        public override string TypeDescription => DescriptionText;


        public AccumulatedTimeDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public AccumulatedTimeDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
        }
    }
}
