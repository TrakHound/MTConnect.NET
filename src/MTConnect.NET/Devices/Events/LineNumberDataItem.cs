// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// A reference to the position of a block of program code within a control program.
    /// </summary>
    public class LineNumberDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "LINE_NUMBER";
        public const string NameId = "lineNumber";

        public enum SubTypes
        {
            /// <summary>
            /// The position of a block of program code relative to the beginning of the control program.
            /// </summary>
            ABSOLUTE,

            /// <summary>
            /// The position of a block of program code relative to the occurrence of the last LINE_LABEL encountered in the control program.
            /// </summary>
            INCREMENTAL
        }


        public LineNumberDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public LineNumberDataItem(
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
                case SubTypes.ABSOLUTE: return "abs";
                case SubTypes.INCREMENTAL: return "inc";
            }

            return null;
        }
    }
}
