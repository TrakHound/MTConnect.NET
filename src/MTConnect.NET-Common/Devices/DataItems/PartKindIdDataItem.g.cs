// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier given to link the individual occurrence to a class of parts, typically distinguished by a particular part design.
    /// </summary>
    public class PartKindIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PART_KIND_ID";
        public const string NameId = "";
             
        public new const string DescriptionText = "Identifier given to link the individual occurrence to a class of parts, typically distinguished by a particular part design.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17;       


        public enum SubTypes
        {
            /// <summary>
            /// Universally unique identifier as specified in ISO 11578 or RFC 4122.
            /// </summary>
            UUID,
            
            /// <summary>
            /// Identifier given to a group of parts having similarities in geometry, manufacturing process, and/or functions.
            /// </summary>
            PART_FAMILY,
            
            /// <summary>
            /// Word or set of words by which a part is known, addressed, or referred to.
            /// </summary>
            PART_NAME,
            
            /// <summary>
            /// Identifier of a particular part design or model.
            /// </summary>
            PART_NUMBER
        }


        public PartKindIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public PartKindIdDataItem(
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
                case SubTypes.UUID: return "Universally unique identifier as specified in ISO 11578 or RFC 4122.";
                case SubTypes.PART_FAMILY: return "Identifier given to a group of parts having similarities in geometry, manufacturing process, and/or functions.";
                case SubTypes.PART_NAME: return "Word or set of words by which a part is known, addressed, or referred to.";
                case SubTypes.PART_NUMBER: return "Identifier of a particular part design or model.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.UUID: return "UUID";
                case SubTypes.PART_FAMILY: return "PART_FAMILY";
                case SubTypes.PART_NAME: return "PART_NAME";
                case SubTypes.PART_NUMBER: return "PART_NUMBER";
            }

            return null;
        }

    }
}