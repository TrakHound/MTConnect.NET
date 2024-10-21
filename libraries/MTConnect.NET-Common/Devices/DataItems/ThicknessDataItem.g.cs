// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1712321222573_635969_358

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Dimension between two surfaces of an object, usually the dimension of smallest measure, for example an additive layer, or a depth of cut.
    /// </summary>
    public class ThicknessDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "THICKNESS";
        public const string NameId = "thickness";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;     
             
        public new const string DescriptionText = "Dimension between two surfaces of an object, usually the dimension of smallest measure, for example an additive layer, or a depth of cut.";
        
        public override string TypeDescription => DescriptionText;
        
               


        public enum SubTypes
        {
            /// <summary>
            /// ACTUAL.
            /// </summary>
            ACTUAL,
            
            /// <summary>
            /// COMMANDED.
            /// </summary>
            COMMANDED,
            
            /// <summary>
            /// TARGET.
            /// </summary>
            TARGET,
            
            /// <summary>
            /// PROGRAMMED.
            /// </summary>
            PROGRAMMED
        }


        public ThicknessDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        public ThicknessDataItem(
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
                case SubTypes.ACTUAL: return "ACTUAL.";
                case SubTypes.COMMANDED: return "COMMANDED.";
                case SubTypes.TARGET: return "TARGET.";
                case SubTypes.PROGRAMMED: return "PROGRAMMED.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ACTUAL: return "ACTUAL";
                case SubTypes.COMMANDED: return "COMMANDED";
                case SubTypes.TARGET: return "TARGET";
                case SubTypes.PROGRAMMED: return "PROGRAMMED";
            }

            return null;
        }

    }
}