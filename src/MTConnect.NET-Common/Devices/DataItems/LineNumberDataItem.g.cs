// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Position of a block of program code within a control program.
    /// </summary>
    public class LineNumberDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "LINE_NUMBER";
        public const string NameId = "";
             
        public new const string DescriptionText = "Position of a block of program code within a control program.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14;       


        public enum SubTypes
        {
            /// <summary>
            /// Position of a block of program code relative to the beginning of the control program.
            /// </summary>
            ABSOLUTE,
            
            /// <summary>
            /// Position of a block of program code relative to the occurrence of the last LineLabel encountered in the control program.
            /// </summary>
            INCREMENTAL
        }


        public LineNumberDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public LineNumberDataItem(
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
                case SubTypes.ABSOLUTE: return "Position of a block of program code relative to the beginning of the control program.";
                case SubTypes.INCREMENTAL: return "Position of a block of program code relative to the occurrence of the last LineLabel encountered in the control program.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ABSOLUTE: return "ABSOLUTE";
                case SubTypes.INCREMENTAL: return "INCREMENTAL";
            }

            return null;
        }

    }
}