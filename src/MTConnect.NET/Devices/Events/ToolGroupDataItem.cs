// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// An identifier for the tool group associated with a specific tool. Commonly used to designate spare tools.
    /// </summary>
    public class ToolGroupDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "TOOL_GROUP";
        public const string NameId = "toolGroup";


        public ToolGroupDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public ToolGroupDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
