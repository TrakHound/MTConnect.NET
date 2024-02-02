// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Name of the logic or motion program being executed by the Controller component.
    /// </summary>
    public class ProgramDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROGRAM";
        public const string NameId = "program";
             
             
        public new const string DescriptionText = "Name of the logic or motion program being executed by the Controller component.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version10;       


        public enum SubTypes
        {
            /// <summary>
            /// Phase or segment of a recipe or program.
            /// </summary>
            ACTIVITY,
            
            /// <summary>
            /// Phase of a recipe process.
            /// </summary>
            SEGMENT,
            
            /// <summary>
            /// Process as part of product production; can be a subprocess of a larger process.
            /// </summary>
            RECIPE,
            
            /// <summary>
            /// Step of a discrete manufacturing process.
            /// </summary>
            OPERATION,
            
            /// <summary>
            /// Identity of the logic or motion program currently executing.
            /// </summary>
            ACTIVE,
            
            /// <summary>
            /// Identity of the primary logic or motion program currently being executed. It is the starting nest level in a call structure and may contain calls to sub programs.
            /// </summary>
            MAIN,
            
            /// <summary>
            /// Identity of a control program that is used to specify the order of execution of other programs.
            /// </summary>
            SCHEDULE
        }


        public ProgramDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public ProgramDataItem(
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
                case SubTypes.ACTIVITY: return "Phase or segment of a recipe or program.";
                case SubTypes.SEGMENT: return "Phase of a recipe process.";
                case SubTypes.RECIPE: return "Process as part of product production; can be a subprocess of a larger process.";
                case SubTypes.OPERATION: return "Step of a discrete manufacturing process.";
                case SubTypes.ACTIVE: return "Identity of the logic or motion program currently executing.";
                case SubTypes.MAIN: return "Identity of the primary logic or motion program currently being executed. It is the starting nest level in a call structure and may contain calls to sub programs.";
                case SubTypes.SCHEDULE: return "Identity of a control program that is used to specify the order of execution of other programs.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ACTIVITY: return "ACTIVITY";
                case SubTypes.SEGMENT: return "SEGMENT";
                case SubTypes.RECIPE: return "RECIPE";
                case SubTypes.OPERATION: return "OPERATION";
                case SubTypes.ACTIVE: return "ACTIVE";
                case SubTypes.MAIN: return "MAIN";
                case SubTypes.SCHEDULE: return "SCHEDULE";
            }

            return null;
        }

    }
}