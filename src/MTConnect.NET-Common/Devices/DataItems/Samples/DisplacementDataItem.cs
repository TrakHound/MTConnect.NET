// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// The measurement of the change in position of an object.
    /// </summary>
    public class DisplacementDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "DISPLACEMENT";
        public const string NameId = "displacement";
        public const string DefaultUnits = Devices.Units.MILLIMETER;
        public new const string DescriptionText = "The measurement of the change in position of an object.";

        public override string TypeDescription => DescriptionText;


        public DisplacementDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public DisplacementDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
        }
    }
}
