// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// Identifier given to a distinguishable, individual part.
    /// </summary>
    public class PartUniqueIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PART_UNIQUE_ID";
        public const string NameId = "partUniqueId";
        public new const string DescriptionText = "Identifier given to a distinguishable, individual part.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;

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
            Category = CategoryId;
            Type = TypeId;
        }

        public PartUniqueIdDataItem(
            string parentId,
            SubTypes subType = SubTypes.UUID
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
                case SubTypes.RAW_MATERIAL: return "Material that is used to produce parts.";
                case SubTypes.SERIAL_NUMBER: return "A serial number that uniquely identifies a specific part.";
                case SubTypes.UUID: return "The globally unique identifier as specified in ISO 11578 or RFC 4122.";
            }

            return null;
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