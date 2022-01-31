// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// An indication of the status of the Controller components program editing mode.
    /// </summary>
    public class ProgramEditDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROGRAM_EDIT";
        public const string NameId = "programEdit";


        public ProgramEditDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public ProgramEditDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
