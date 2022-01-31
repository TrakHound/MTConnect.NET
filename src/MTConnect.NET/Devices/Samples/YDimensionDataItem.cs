// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Samples
{
    /// <summary>
    /// Measured dimension of an entity relative to the Y direction of the referenced coordinate system.
    /// </summary>
    public class YDimensionDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "Y_DIMENSION";
        public const string NameId = "yDim";


        public YDimensionDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
            Units = Devices.Units.MILLIMETER;
        }

        public YDimensionDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = Devices.Units.MILLIMETER;
        }
    }
}
