// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218416_705945_2091

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Amount of time a piece of equipment has performed different types of activities associated with the process being performed at that piece of equipment.
    /// </summary>
    public class ProcessTimerDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "PROCESS_TIMER";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "processTimer";

        /// <summary>
        /// The default <c>units</c> for this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public const string DefaultUnits = Devices.Units.SECOND;

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Amount of time a piece of equipment has performed different types of activities associated with the process being performed at that piece of equipment.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this DataItem.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version14;


        /// <summary>
        /// The set of <c>subType</c> values defined for this DataItem by the MTConnect Standard.
        /// </summary>
        public enum SubTypes
        {
            /// <summary>
            /// Time from the beginning of production of a part or product on a piece of equipment until the time that production is complete for that part or product onthat piece of equipment.This includes the time that the piece of equipment is running, producing parts or products, or in the process of producing parts.
            /// </summary>
            PROCESS,
            
            /// <summary>
            /// Elapsed time of a temporary halt of action.
            /// </summary>
            DELAY
        }


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public ProcessTimerDataItem()
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
        public ProcessTimerDataItem(
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
                case SubTypes.PROCESS: return "Time from the beginning of production of a part or product on a piece of equipment until the time that production is complete for that part or product onthat piece of equipment.This includes the time that the piece of equipment is running, producing parts or products, or in the process of producing parts.";
                case SubTypes.DELAY: return "Elapsed time of a temporary halt of action.";
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
                case SubTypes.PROCESS: return "PROCESS";
                case SubTypes.DELAY: return "DELAY";
            }

            return null;
        }

    }
}