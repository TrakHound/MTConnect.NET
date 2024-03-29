// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// The name of the program being edited.
    /// </summary>
    public class ProgramEditNameDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROGRAM_EDIT_NAME";
        public const string NameId = "programEditName";
        public new const string DescriptionText = "The name of the program being edited.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        public ProgramEditNameDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public ProgramEditNameDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}