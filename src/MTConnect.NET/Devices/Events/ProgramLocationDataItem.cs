// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// The Uniform Resource Identifier(URI) for the source file associated with PROGRAM.
    /// </summary>
    public class ProgramLocationDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROGRAM_LOCATION";
        public const string NameId = "pgmLocation";
        public new const string DescriptionText = "The Uniform Resource Identifier(URI) for the source file associated with PROGRAM.";

        public override string TypeDescription => DescriptionText;

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


        public ProgramLocationDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public ProgramLocationDataItem(
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

        public override string GetSubTypeDescription() => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.ACTIVE: return "The identity of the logic or motion program currently executing.";
                case SubTypes.MAIN: return "The identity of the primary logic or motion program currently being executed.It is the starting nest level in a call structure and may contain calls to sub programs.";
                case SubTypes.SCHEDULE: return "The identity of a control program that is used to specify the order of execution of other programs.";
            }

            return null;
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
