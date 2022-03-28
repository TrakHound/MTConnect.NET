// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// Electrical potential between two points.
    /// </summary>
    public class VoltageDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "VOLTAGE";
        public const string NameId = "volt";
        public const string DefaultUnits = Devices.Units.VOLT;
        public new const string DescriptionText = "Electrical potential between two points.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MaximumVersion => MTConnectVersions.Version15;


        public VoltageDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public VoltageDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
        }
    }
}
