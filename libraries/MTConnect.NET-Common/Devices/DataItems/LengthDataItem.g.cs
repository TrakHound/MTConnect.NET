// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218326_31395_1863

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Length of an object.
    /// </summary>
    public class LengthDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "LENGTH";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "length";

        /// <summary>
        /// The default <c>units</c> for this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public const string DefaultUnits = Devices.Units.MILLIMETER;

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Length of an object.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this DataItem.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        /// <summary>
        /// The set of <c>subType</c> values defined for this DataItem by the MTConnect Standard.
        /// </summary>
        public enum SubTypes
        {
            /// <summary>
            /// Standard or original length of an object.
            /// </summary>
            STANDARD,
            
            /// <summary>
            /// Remaining total length of an object.
            /// </summary>
            REMAINING,
            
            /// <summary>
            /// Remaining usable length of an object.
            /// </summary>
            USEABLE
        }


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public LengthDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        /// <summary>
        /// Initializes a new instance for the given parent with the specified <paramref name="subType"/>.
        /// </summary>
        /// <param name="parentId">The Id of the parent element this DataItem belongs to.</param>
        /// <param name="subType">The subType to assign to this DataItem.</param>
        public LengthDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
             
            Units = DefaultUnits;
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
                case SubTypes.STANDARD: return "Standard or original length of an object.";
                case SubTypes.REMAINING: return "Remaining total length of an object.";
                case SubTypes.USEABLE: return "Remaining usable length of an object.";
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
                case SubTypes.STANDARD: return "STANDARD";
                case SubTypes.REMAINING: return "REMAINING";
                case SubTypes.USEABLE: return "USEABLE";
            }

            return null;
        }

    }
}