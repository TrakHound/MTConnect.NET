// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Comment or non-executable statement in the control program.
    /// </summary>
    public class ProgramCommentDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROGRAM_COMMENT";
        public const string NameId = "programComment";
             
        public new const string DescriptionText = "Comment or non-executable statement in the control program.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;       


        public enum SubTypes
        {
            /// <summary>
            /// Identity of the primary logic or motion program currently being executed. It is the starting nest level in a call structure and may contain calls to sub programs.
            /// </summary>
            MAIN,
            
            /// <summary>
            /// Identity of a control program that is used to specify the order of execution of other programs.
            /// </summary>
            SCHEDULE,
            
            /// <summary>
            /// Identity of the logic or motion program currently executing.
            /// </summary>
            ACTIVE
        }


        public ProgramCommentDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public ProgramCommentDataItem(
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
                case SubTypes.MAIN: return "Identity of the primary logic or motion program currently being executed. It is the starting nest level in a call structure and may contain calls to sub programs.";
                case SubTypes.SCHEDULE: return "Identity of a control program that is used to specify the order of execution of other programs.";
                case SubTypes.ACTIVE: return "Identity of the logic or motion program currently executing.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.MAIN: return "MAIN";
                case SubTypes.SCHEDULE: return "SCHEDULE";
                case SubTypes.ACTIVE: return "ACTIVE";
            }

            return null;
        }

    }
}