// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// Identifier given to link the individual occurrence to a class of parts, typically distinguished by a particular part design.
    /// </summary>
    public class PartKindIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PART_KIND_ID";
        public const string NameId = "partKindId";

        public enum SubTypes
        {
            /// <summary>
            /// An identifier given to a group of parts having similarities in geometry, manufacturing process, and/or functions.
            /// </summary>
            PART_FAMILY,

            /// <summary>
            /// A word or set of words by which a part is known, addressed, or referred to.
            /// </summary>
            PART_NAME,

            /// <summary>
            /// Identifier of a particular part design or model.
            /// </summary>
            PART_NUMBER,

            /// <summary>
            /// The globally unique identifier as specified in ISO 11578 or RFC 4122.
            /// </summary>
            UUID
        }


        public PartKindIdDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public PartKindIdDataItem(
            string parentId,
            SubTypes subType = SubTypes.UUID
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            DataItemCategory = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
        }


        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.PART_FAMILY: return "partFamily";
                case SubTypes.PART_NAME: return "partName";
                case SubTypes.PART_NUMBER: return "partNumber";
                case SubTypes.UUID: return "uuid";
            }

            return null;
        }
    }
}
