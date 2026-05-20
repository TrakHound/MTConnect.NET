// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Operating state of the service to remove or retract material or product.
    /// </summary>
    public class MaterialRetractDataItem : InterfaceDataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this Interface DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this Interface DataItem.
        /// </summary>
        public const string TypeId = "MATERIAL_RETRACT";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this Interface DataItem.
        /// </summary>
        public const string NameId = "materialRetract";

        /// <summary>
        /// The description of this Interface DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Operating state of the service to remove or retract material or product.";

        /// <summary>
        /// The description of this Interface DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this Interface DataItem.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        /// <summary>
        /// The set of <c>subType</c> values defined for this Interface DataItem by the MTConnect Standard.
        /// </summary>
        public new enum SubTypes
        {
            /// <summary>
            /// Operating state of the request to remove or retract material or product.
            /// </summary>
            REQUEST,
            
            /// <summary>
            /// Operating state of the response to a request to remove or retract material or product.
            /// </summary>
            RESPONSE
        }


        /// <summary>
        /// Initializes a new instance with its category and type set to the defaults for this Interface DataItem.
        /// </summary>
        public MaterialRetractDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        /// <summary>
        /// Initializes a new instance for the given parent with the specified <paramref name="subType"/>.
        /// </summary>
        /// <param name="parentId">The Id of the parent element this Interface DataItem belongs to.</param>
        /// <param name="subType">The subType to assign to this Interface DataItem.</param>
        public MaterialRetractDataItem(
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
        /// The MTConnect Standard description of this Interface DataItem's current <c>subType</c>.
        /// </summary>
        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        /// <summary>
        /// Returns the MTConnect Standard description for the specified <paramref name="subType"/>, or <c>null</c> when it is unknown.
        /// </summary>
        public new static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.REQUEST: return "Operating state of the request to remove or retract material or product.";
                case SubTypes.RESPONSE: return "Operating state of the response to a request to remove or retract material or product.";
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
                case SubTypes.REQUEST: return "REQUEST";
                case SubTypes.RESPONSE: return "RESPONSE";
            }

            return null;
        }

    }
}
