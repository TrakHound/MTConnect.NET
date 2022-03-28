// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// The measurement of the rate of change of angular position.
    /// </summary>
    public class AngularVelocityDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "ANGULAR_VELOCITY";
        public const string NameId = "speed";
        public const string DefaultUnits = Devices.Units.DEGREE_PER_SECOND;
        public new const string DescriptionText = "The measurement of the rate of change of angular position.";

        public override string TypeDescription => DescriptionText;


        public AngularVelocityDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public AngularVelocityDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
        }
    }
}
