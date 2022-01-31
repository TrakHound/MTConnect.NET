// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// The name of the program being edited.
    /// </summary>
    public class ProgramEditNameDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROGRAM_EDIT_NAME";
        public const string NameId = "programEditName";


        public ProgramEditNameDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public ProgramEditNameDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
