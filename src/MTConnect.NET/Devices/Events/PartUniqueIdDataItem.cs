// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// Identifier given to a distinguishable, individual part.
    /// </summary>
    public class PartUniqueIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PART_UNIQUE_ID";
        public const string NameId = "partUniqueId";

        public enum SubTypes
        {
            /// <summary>
            /// Material that is used to produce parts.
            /// </summary>
            RAW_MATERIAL,

            /// <summary>
            /// A serial number that uniquely identifies a specific part.
            /// </summary>
            SERIAL_NUMBER,

            /// <summary>
            /// The globally unique identifier as specified in ISO 11578 or RFC 4122.
            /// </summary>
            UUID
        }


        public PartUniqueIdDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public PartUniqueIdDataItem(
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
                case SubTypes.RAW_MATERIAL: return "rawMaterial";
                case SubTypes.SERIAL_NUMBER: return "serialNumber";
                case SubTypes.UUID: return "uuid";
            }

            return null;
        }
    }
}
