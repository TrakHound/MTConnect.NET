// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// The identity of the logic or motion program being executed.
    /// </summary>
    public class ProgramDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROGRAM";
        public const string NameId = "pgm";
        public new const string DescriptionText = "The identity of the logic or motion program being executed.";

        public override string TypeDescription => DescriptionText;

        public enum SubTypes
        {
            /// <summary>
            /// The identity of the logic or motion program currently executing.
            /// </summary>
            ACTIVE,

            /// <summary>
            /// Phase or segment of a recipe or program.
            /// </summary>
            ACTIVITY,

            /// <summary>
            /// The identity of the primary logic or motion program currently being executed.It is the starting nest level in a call structure and may contain calls to sub programs.
            /// </summary>
            MAIN,

            /// <summary>
            /// Step of a discrete manufacturing process.
            /// </summary>
            OPERATION,

            /// <summary>
            /// Process as part of product production; can be a subprocess of a larger process.
            /// </summary>
            RECIPE,

            /// <summary>
            /// The identity of a control program that is used to specify the order of execution of other programs.
            /// </summary>
            SCHEDULE,

            /// <summary>
            /// Phase of a recipe process.
            /// </summary>
            SEGMENT
        }


        public ProgramDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public ProgramDataItem(
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

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.ACTIVE: return "The identity of the logic or motion program currently executing.";
                case SubTypes.ACTIVITY: return "Phase or segment of a recipe or program.";
                case SubTypes.MAIN: return "The identity of the primary logic or motion program currently being executed.It is the starting nest level in a call structure and may contain calls to sub programs.";
                case SubTypes.OPERATION: return "Step of a discrete manufacturing process.";
                case SubTypes.RECIPE: return "Process as part of product production; can be a subprocess of a larger process.";
                case SubTypes.SCHEDULE: return "The identity of a control program that is used to specify the order of execution of other programs.";
                case SubTypes.SEGMENT: return "Phase of a recipe process.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ACTIVE: return "actv";
                case SubTypes.ACTIVITY: return "acty";
                case SubTypes.MAIN: return "main";
                case SubTypes.OPERATION: return "op";
                case SubTypes.RECIPE: return "rpe";
                case SubTypes.SCHEDULE: return "sch";
                case SubTypes.SEGMENT: return "seg";
            }

            return null;
        }
    }
}
