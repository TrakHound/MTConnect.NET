// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Samples
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
        public const int DefaultSignificantDigits = 1;
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
            SignificantDigits = DefaultSignificantDigits;
        }
    }
}