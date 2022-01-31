// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// Describes the way the axes will be associated to each other.
    /// </summary>
    public class AxisCouplingDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "AXIS_COUPLING";
        public const string NameId = "axisCoupling";


        public AxisCouplingDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public AxisCouplingDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
