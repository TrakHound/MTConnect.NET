// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605548793581_13403_454

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier given to a collection of individual parts.
    /// </summary>
    public class PartGroupIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PART_GROUP_ID";
        public const string NameId = "partGroupId";
             
             
        public new const string DescriptionText = "Identifier given to a collection of individual parts.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public enum SubTypes
        {
            /// <summary>
            /// Identifier that references a group of parts tracked as a lot.
            /// </summary>
            LOT,
            
            /// <summary>
            /// Material that is used to produce parts.
            /// </summary>
            RAW_MATERIAL,
            
            /// <summary>
            /// Identifier that references a group of parts produced in a batch.
            /// </summary>
            BATCH,
            
            /// <summary>
            /// Universally unique identifier as specified in ISO 11578 or RFC 4122.
            /// </summary>
            UUID,
            
            /// <summary>
            /// Identifier used to reference a material heat number.
            /// </summary>
            HEAT_TREAT
        }


        public PartGroupIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public PartGroupIdDataItem(
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
                case SubTypes.LOT: return "Identifier that references a group of parts tracked as a lot.";
                case SubTypes.RAW_MATERIAL: return "Material that is used to produce parts.";
                case SubTypes.BATCH: return "Identifier that references a group of parts produced in a batch.";
                case SubTypes.UUID: return "Universally unique identifier as specified in ISO 11578 or RFC 4122.";
                case SubTypes.HEAT_TREAT: return "Identifier used to reference a material heat number.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.LOT: return "LOT";
                case SubTypes.RAW_MATERIAL: return "RAW_MATERIAL";
                case SubTypes.BATCH: return "BATCH";
                case SubTypes.UUID: return "UUID";
                case SubTypes.HEAT_TREAT: return "HEAT_TREAT";
            }

            return null;
        }

    }
}