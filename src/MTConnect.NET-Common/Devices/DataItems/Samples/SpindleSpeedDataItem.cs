// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// Rotational speed of the rotary axis.
    /// </summary>
    public class SpindleSpeedDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "SPINDLE_SPEED";
        public const string NameId = "spindleSpeed";
        public const string DefaultUnits = Devices.Units.REVOLUTION_PER_MINUTE;
        public new const string DescriptionText = "Rotational speed of the rotary axis.";

        public override string TypeDescription => DescriptionText;


        public SpindleSpeedDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public SpindleSpeedDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
        }
    }
}
