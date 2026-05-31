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
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "PART_GROUP_ID";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "partGroupId";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Identifier given to a collection of individual parts.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this DataItem.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        /// <summary>
        /// The set of <c>subType</c> values defined for this DataItem by the MTConnect Standard.
        /// </summary>
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


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public PartGroupIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        /// <summary>
        /// Initializes a new instance for the given parent with the specified <paramref name="subType"/>.
        /// </summary>
        /// <param name="parentId">The Id of the parent element this DataItem belongs to.</param>
        /// <param name="subType">The subType to assign to this DataItem.</param>
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

        /// <summary>
        /// The MTConnect Standard description of this DataItem's current <c>subType</c>.
        /// </summary>
        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        /// <summary>
        /// Returns the MTConnect Standard description for the specified <paramref name="subType"/>, or <c>null</c> when it is unknown.
        /// </summary>
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

        /// <summary>
        /// Returns the string identifier for the specified <paramref name="subType"/>, or <c>null</c> when it is unknown.
        /// </summary>
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