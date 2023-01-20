// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// The line of code or command being executed by a Controller Structural Element.
    /// </summary>
    public class BlockDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "BLOCK";
        public const string NameId = "block";
        public new const string DescriptionText = "The line of code or command being executed by a Controller Structural Element.";

        public override string TypeDescription => DescriptionText;


        public BlockDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public BlockDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}