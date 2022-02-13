// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Samples
{
    /// <summary>
    /// A Force applied to a mass in one direction only
    /// </summary>
    public class LinearForceDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "LINEAR_FORCE";
        public const string NameId = "linForce";
        public const string DefaultUnits = Devices.Units.NEWTON;
        public new const string DescriptionText = "A Force applied to a mass in one direction only";

        public override string TypeDescription => DescriptionText;


        public LinearForceDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public LinearForceDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
        }
    }
}
