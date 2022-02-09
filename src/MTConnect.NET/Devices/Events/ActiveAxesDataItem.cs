// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// The set of axes currently associated with a Path or Controller Structural Element.
    /// </summary>
    public class ActiveAxesDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ACTIVE_AXES";
        public const string NameId = "actAxes";


        public ActiveAxesDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public ActiveAxesDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
