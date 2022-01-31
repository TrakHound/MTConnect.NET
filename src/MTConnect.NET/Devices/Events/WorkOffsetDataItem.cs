// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// A reference to the offset variables for a work piece or part associated with a Path in a Controller type component.
    /// </summary>
    public class WorkOffsetDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "WORK_OFFSET";
        public const string NameId = "workOffset";


        public WorkOffsetDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public WorkOffsetDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
