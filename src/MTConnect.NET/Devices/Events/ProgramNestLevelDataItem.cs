// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// An indication of the nesting level within a control program that is associated with the code or instructions that is currently being executed.
    /// </summary>
    public class ProgramNestLevelDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROGRAM_NEST_LEVEL";
        public const string NameId = "programNestLevel";


        public ProgramNestLevelDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public ProgramNestLevelDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
