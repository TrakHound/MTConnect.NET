// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// URI for the source file associated with Program.
    /// </summary>
    public class ProgramLocationDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROGRAM_LOCATION";
        public const string NameId = "programLocation";
             
             
        public new const string DescriptionText = "URI for the source file associated with Program.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15;       


        public enum SubTypes
        {
            /// <summary>
            /// Identity of a control program that is used to specify the order of execution of other programs.
            /// </summary>
            SCHEDULE,
            
            /// <summary>
            /// Identity of the primary logic or motion program currently being executed. It is the starting nest level in a call structure and may contain calls to sub programs.
            /// </summary>
            MAIN,
            
            /// <summary>
            /// Identity of the logic or motion program currently executing.
            /// </summary>
            ACTIVE
        }


        public ProgramLocationDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public ProgramLocationDataItem(
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
                case SubTypes.SCHEDULE: return "Identity of a control program that is used to specify the order of execution of other programs.";
                case SubTypes.MAIN: return "Identity of the primary logic or motion program currently being executed. It is the starting nest level in a call structure and may contain calls to sub programs.";
                case SubTypes.ACTIVE: return "Identity of the logic or motion program currently executing.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.SCHEDULE: return "SCHEDULE";
                case SubTypes.MAIN: return "MAIN";
                case SubTypes.ACTIVE: return "ACTIVE";
            }

            return null;
        }

    }
}