// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605549230052_910367_748

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier given to link the individual occurrence to a class of parts, typically distinguished by a particular part design.
    /// </summary>
    public class PartKindIdDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "PART_KIND_ID";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "partKindId";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Identifier given to link the individual occurrence to a class of parts, typically distinguished by a particular part design.";

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


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public PartKindIdDataItem()
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
                case SubTypes.UUID: return "Universally unique identifier as specified in ISO 11578 or RFC 4122.";
                case SubTypes.PART_FAMILY: return "Identifier given to a group of parts having similarities in geometry, manufacturing process, and/or functions.";
                case SubTypes.PART_NAME: return "Word or set of words by which a part is known, addressed, or referred to.";
                case SubTypes.PART_NUMBER: return "Identifier of a particular part design or model.";
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
                case SubTypes.UUID: return "UUID";
                case SubTypes.PART_FAMILY: return "PART_FAMILY";
                case SubTypes.PART_NAME: return "PART_NAME";
                case SubTypes.PART_NUMBER: return "PART_NUMBER";
            }

            return null;
        }

    }
}