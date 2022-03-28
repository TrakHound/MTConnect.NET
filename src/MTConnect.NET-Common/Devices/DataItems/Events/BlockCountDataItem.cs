// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// The total count of the number of blocks of program code that have been executed since execution started.
    /// </summary>
    public class BlockCountDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "BLOCK_COUNT";
        public const string NameId = "blockCount";
        public new const string DescriptionText = "The total count of the number of blocks of program code that have been executed since execution started.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version14;


        public BlockCountDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = Devices.Units.COUNT;
        }

        public BlockCountDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = Devices.Units.COUNT;
        }
    }
}
