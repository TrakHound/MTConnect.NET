// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// A comment or non-executable statement in the control program.
    /// </summary>
    public class ProgramCommentDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROGRAM_COMMENT";
        public const string NameId = "pgmComment";

        public enum SubTypes
        {
            /// <summary>
            /// The identity of the logic or motion program currently executing.
            /// </summary>
            ACTIVE,

            /// <summary>
            /// The identity of the primary logic or motion program currently being executed.It is the starting nest level in a call structure and may contain calls to sub programs.
            /// </summary>
            MAIN,

            /// <summary>
            /// The identity of a control program that is used to specify the order of execution of other programs.
            /// </summary>
            SCHEDULE
        }


        public ProgramCommentDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public ProgramCommentDataItem(
            string parentId,
            SubTypes subType = SubTypes.MAIN
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
        }


        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ACTIVE: return "act";
                case SubTypes.MAIN: return "main";
                case SubTypes.SCHEDULE: return "sch";
            }

            return null;
        }
    }
}
