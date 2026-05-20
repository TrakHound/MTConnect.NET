// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218469_994902_2259

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Electrical potential between two points.**DEPRECATED** in *Version 1.6*. Replaced by `VOLTAGE_AC` and `VOLTAGE_DC`.
    /// </summary>
    public class VoltageDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "VOLTAGE";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "voltage";

        /// <summary>
        /// The default <c>units</c> for this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public const string DefaultUnits = Devices.Units.VOLT;

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Electrical potential between two points.**DEPRECATED** in *Version 1.6*. Replaced by `VOLTAGE_AC` and `VOLTAGE_DC`.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The maximum MTConnect Version that this DataItem is valid for; set when the type has been deprecated.
        /// </summary>
        public override System.Version MaximumVersion => MTConnectVersions.Version16;

        /// <summary>
        /// The minimum MTConnect Version that introduced this DataItem.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version10;


        /// <summary>
        /// The set of <c>subType</c> values defined for this DataItem by the MTConnect Standard.
        /// </summary>
        public enum SubTypes
        {
            /// <summary>
            /// Alternating voltage or current. If not specified further in statistic, defaults to RMS voltage. **DEPRECATED** in *Version 1.6*.
            /// </summary>
            ALTERNATING,
            
            /// <summary>
            /// DC current or voltage.**DEPRECATED** in *Version 1.6*.
            /// </summary>
            DIRECT,
            
            /// <summary>
            /// Measured or reported value of an observation.**DEPRECATED** in *Version 1.6*.
            /// </summary>
            ACTUAL,
            
            /// <summary>
            /// Goal of the operation or process.**DEPRECATED** in *Version 1.6*.
            /// </summary>
            TARGET
        }


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public VoltageDataItem()
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
        public VoltageDataItem(
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
                case SubTypes.ALTERNATING: return "Alternating voltage or current. If not specified further in statistic, defaults to RMS voltage. **DEPRECATED** in *Version 1.6*.";
                case SubTypes.DIRECT: return "DC current or voltage.**DEPRECATED** in *Version 1.6*.";
                case SubTypes.ACTUAL: return "Measured or reported value of an observation.**DEPRECATED** in *Version 1.6*.";
                case SubTypes.TARGET: return "Goal of the operation or process.**DEPRECATED** in *Version 1.6*.";
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
                case SubTypes.ALTERNATING: return "ALTERNATING";
                case SubTypes.DIRECT: return "DIRECT";
                case SubTypes.ACTUAL: return "ACTUAL";
                case SubTypes.TARGET: return "TARGET";
            }

            return null;
        }

    }
}