// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// The measurement of the rate of change of position of a Component
    /// </summary>
    public class VelocityDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "VELOCITY";
        public const string NameId = "vel";
        public const string DefaultUnits = Devices.Units.MILLIMETER_PER_SECOND;
        public new const string DescriptionText = "The measurement of the rate of change of position of a Component";

        public override string TypeDescription => DescriptionText;


        public VelocityDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public VelocityDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
        }
    }
}
