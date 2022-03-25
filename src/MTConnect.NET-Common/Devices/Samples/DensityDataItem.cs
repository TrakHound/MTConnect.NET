// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Samples
{
    /// <summary>
    /// The volumetric mass of a material per unit volume of that material.
    /// </summary>
    public class DensityDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "DENSITY";
        public const string NameId = "density";
        public const string DefaultUnits = Devices.Units.MILLIGRAM_PER_CUBIC_MILLIMETER;
        public new const string DescriptionText = "The volumetric mass of a material per unit volume of that material.";

        public override string TypeDescription => DescriptionText;


        public DensityDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public DensityDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
        }
    }
}
