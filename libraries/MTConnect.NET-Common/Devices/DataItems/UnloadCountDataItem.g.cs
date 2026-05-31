// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1622197599372_551036_627

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Accumulation of the number of times an operation has attempted to, or is planned to attempt to, unload materials, parts, or other items.
    /// </summary>
    public class UnloadCountDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "UNLOAD_COUNT";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "unloadCount";

        /// <summary>
        /// The default <c>representation</c> for this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Accumulation of the number of times an operation has attempted to, or is planned to attempt to, unload materials, parts, or other items.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this DataItem.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version18;


        /// <summary>
        /// The set of <c>subType</c> values defined for this DataItem by the MTConnect Standard.
        /// </summary>
        public enum SubTypes
        {
            /// <summary>
            /// Accumulation of actions or activities that were attempted, but terminated before they could be completed.
            /// </summary>
            ABORTED,
            
            /// <summary>
            /// Accumulation of actions, items, or activities being counted that do not conform to specification or expectation.
            /// </summary>
            BAD,
            
            /// <summary>
            /// Accumulation of actions, items, or activities being counted that do not conform to specification or expectation.
            /// </summary>
            FAILED,
            
            /// <summary>
            /// Accumulation of actions, items, or activities being counted that conform to specification or expectation.
            /// </summary>
            GOOD,
            
            /// <summary>
            /// Accumulation of actions, items, or activities that have been completed, independent of the outcome.
            /// </summary>
            COMPLETE,
            
            /// <summary>
            /// Accumulation of all actions, items, or activities being counted independent of the outcome.
            /// </summary>
            ALL,
            
            /// <summary>
            /// Goal of the operation or process.
            /// </summary>
            TARGET,
            
            /// <summary>
            /// Accumulation of actions, items, or activities yet to be counted.
            /// </summary>
            REMAINING
        }


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public UnloadCountDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        /// <summary>
        /// Initializes a new instance for the given parent with the specified <paramref name="subType"/>.
        /// </summary>
        /// <param name="parentId">The Id of the parent element this DataItem belongs to.</param>
        /// <param name="subType">The subType to assign to this DataItem.</param>
        public UnloadCountDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
            Representation = DefaultRepresentation; 
            
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
                case SubTypes.ABORTED: return "Accumulation of actions or activities that were attempted, but terminated before they could be completed.";
                case SubTypes.BAD: return "Accumulation of actions, items, or activities being counted that do not conform to specification or expectation.";
                case SubTypes.FAILED: return "Accumulation of actions, items, or activities being counted that do not conform to specification or expectation.";
                case SubTypes.GOOD: return "Accumulation of actions, items, or activities being counted that conform to specification or expectation.";
                case SubTypes.COMPLETE: return "Accumulation of actions, items, or activities that have been completed, independent of the outcome.";
                case SubTypes.ALL: return "Accumulation of all actions, items, or activities being counted independent of the outcome.";
                case SubTypes.TARGET: return "Goal of the operation or process.";
                case SubTypes.REMAINING: return "Accumulation of actions, items, or activities yet to be counted.";
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
                case SubTypes.ABORTED: return "ABORTED";
                case SubTypes.BAD: return "BAD";
                case SubTypes.FAILED: return "FAILED";
                case SubTypes.GOOD: return "GOOD";
                case SubTypes.COMPLETE: return "COMPLETE";
                case SubTypes.ALL: return "ALL";
                case SubTypes.TARGET: return "TARGET";
                case SubTypes.REMAINING: return "REMAINING";
            }

            return null;
        }

    }
}