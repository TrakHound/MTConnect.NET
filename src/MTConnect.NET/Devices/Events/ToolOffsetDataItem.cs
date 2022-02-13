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
        public new const string DescriptionText = "A reference to the tool offset variables applied to the active cutting tool.";

        public override string TypeDescription => DescriptionText;

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
            Category = CategoryId;
            Type = TypeId;
        }

        public ToolOffsetDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
        }

        public override string GetSubTypeDescription() => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.LENGTH: return "A reference to a length type tool offset.";
                case SubTypes.RADIAL: return "A reference to a radial type tool offset.";
            }

            return null;
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
