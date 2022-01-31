// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// A set of limits used to indicate whether a process variable is stable and in control.
    /// </summary>
    public class ControlLimitDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "CONTROL_LIMIT";
        public const string NameId = "controlLimit";


        public ControlLimitDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
            Representation = DataItemRepresentation.DATA_SET;
        }

        public ControlLimitDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DataItemRepresentation.DATA_SET;
        }
    }
}
