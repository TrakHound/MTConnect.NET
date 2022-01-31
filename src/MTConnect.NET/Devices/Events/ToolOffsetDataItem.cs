// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// A reference to the tool offset variables applied to the active cutting tool.
    /// </summary>
    public class ToolOffsetDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "TOOL_OFFSET";
        public const string NameId = "toolOffset";

        public enum SubTypes
        {
            /// <summary>
            /// A reference to a length type tool offset.
            /// </summary>
            LENGTH,

            /// <summary>
            /// A reference to a radial type tool offset.
            /// </summary>
            RADIAL
        }


        public ToolOffsetDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public ToolOffsetDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            DataItemCategory = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
        }


        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.LENGTH: return "length";
                case SubTypes.RADIAL: return "radial";
            }

            return null;
        }
    }
}
