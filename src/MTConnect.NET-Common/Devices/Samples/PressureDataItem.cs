// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Samples
{
    /// <summary>
    /// The force per unit area measured relative to atmospheric pressure.
    /// </summary>
    public class PressureDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "PRESSURE";
        public const string NameId = "pres";
        public const string DefaultUnits = Devices.Units.PASCAL;
        public new const string DescriptionText = "The force per unit area measured relative to atmospheric pressure.";

        public override string TypeDescription => DescriptionText;


        public PressureDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public PressureDataItem(string parentId)
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
