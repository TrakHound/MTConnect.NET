// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218458_345999_2226

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Reference to the tool offset variables applied to the active cutting tool.
    /// </summary>
    public class ToolOffsetDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "TOOL_OFFSET";
        public const string NameId = "toolOffset";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;     
             
        public new const string DescriptionText = "Reference to the tool offset variables applied to the active cutting tool.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14;       


        public enum SubTypes
        {
            /// <summary>
            /// Reference to a radial type tool offset variable.
            /// </summary>
            RADIAL,
            
            /// <summary>
            /// Reference to a length type tool offset variable.
            /// </summary>
            LENGTH
        }


        public ToolOffsetDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
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
            Representation = DefaultRepresentation; 
            
        }

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.RADIAL: return "Reference to a radial type tool offset variable.";
                case SubTypes.LENGTH: return "Reference to a length type tool offset variable.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.RADIAL: return "RADIAL";
                case SubTypes.LENGTH: return "LENGTH";
            }

            return null;
        }

    }
}