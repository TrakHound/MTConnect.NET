// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
	/// <summary>
	/// Reference to the tool offset variables applied to the active cutting tool.
	/// </summary>
	public class ToolOffsetDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "TOOL_OFFSET";
        public const string NameId = "toolOffset";
        public new const string DescriptionText = "Reference to the tool offset variables applied to the active cutting tool.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version14;

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

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

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