// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Samples
{
    /// <summary>
    /// The positive rate of change of velocity.
    /// </summary>
    public class FlowDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "FLOW";
        public const string NameId = "flow";
        public const string DefaultUnits = Devices.Units.LITER_PER_SECOND;
        public new const string DescriptionText = "The positive rate of change of velocity.";

        public override string TypeDescription => DescriptionText;


        public FlowDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public FlowDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
            SignificantDigits = 1;
        }
    }
}
