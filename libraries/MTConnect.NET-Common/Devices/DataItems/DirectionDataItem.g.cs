// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218278_641962_1755

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Direction of motion.
    /// </summary>
    public class DirectionDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "DIRECTION";
        public const string NameId = "direction";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;     
             
        public new const string DescriptionText = "Direction of motion.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version10;       


        public enum SubTypes
        {
            /// <summary>
            /// Rotational direction of a rotary motion using the right hand rule convention.
            /// </summary>
            ROTARY,
            
            /// <summary>
            /// Direction of motion of a linear motion.
            /// </summary>
            LINEAR
        }


        public DirectionDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        public DirectionDataItem(
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
                case SubTypes.ROTARY: return "Rotational direction of a rotary motion using the right hand rule convention.";
                case SubTypes.LINEAR: return "Direction of motion of a linear motion.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ROTARY: return "ROTARY";
                case SubTypes.LINEAR: return "LINEAR";
            }

            return null;
        }

    }
}