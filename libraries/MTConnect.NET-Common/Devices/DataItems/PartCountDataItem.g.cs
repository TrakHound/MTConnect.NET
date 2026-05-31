// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218363_437912_1953

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Aggregate count of parts.
    /// </summary>
    public class PartCountDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "PART_COUNT";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "partCount";

        /// <summary>
        /// The default <c>representation</c> for this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Aggregate count of parts.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

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
            /// Accumulation of all actions, items, or activities being counted independent of the outcome.
            /// </summary>
            ALL,
            
            /// <summary>
            /// Accumulation of actions, items, or activities being counted that conform to specification or expectation.
            /// </summary>
            GOOD,
            
            /// <summary>
            /// Accumulation of actions, items, or activities being counted that do not conform to specification or expectation.
            /// </summary>
            BAD,
            
            /// <summary>
            /// Goal of the operation or process.
            /// </summary>
            TARGET,
            
            /// <summary>
            /// Accumulation of actions, items, or activities yet to be counted.
            /// </summary>
            REMAINING,
            
            /// <summary>
            /// Accumulation of actions, items, or activities that have been completed, independent of the outcome.
            /// </summary>
            COMPLETE,
            
            /// <summary>
            /// Accumulation of actions or activities that were attempted, but terminated before they could be completed.
            /// </summary>
            ABORTED,
            
            /// <summary>
            /// Accumulation of actions or activities that were attempted, but failed to complete or resulted in an unexpected or unacceptable outcome.
            /// </summary>
            FAILED
        }


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public PartCountDataItem()
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
        public PartCountDataItem(
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
                case SubTypes.ALL: return "Accumulation of all actions, items, or activities being counted independent of the outcome.";
                case SubTypes.GOOD: return "Accumulation of actions, items, or activities being counted that conform to specification or expectation.";
                case SubTypes.BAD: return "Accumulation of actions, items, or activities being counted that do not conform to specification or expectation.";
                case SubTypes.TARGET: return "Goal of the operation or process.";
                case SubTypes.REMAINING: return "Accumulation of actions, items, or activities yet to be counted.";
                case SubTypes.COMPLETE: return "Accumulation of actions, items, or activities that have been completed, independent of the outcome.";
                case SubTypes.ABORTED: return "Accumulation of actions or activities that were attempted, but terminated before they could be completed.";
                case SubTypes.FAILED: return "Accumulation of actions or activities that were attempted, but failed to complete or resulted in an unexpected or unacceptable outcome.";
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
                case SubTypes.ALL: return "ALL";
                case SubTypes.GOOD: return "GOOD";
                case SubTypes.BAD: return "BAD";
                case SubTypes.TARGET: return "TARGET";
                case SubTypes.REMAINING: return "REMAINING";
                case SubTypes.COMPLETE: return "COMPLETE";
                case SubTypes.ABORTED: return "ABORTED";
                case SubTypes.FAILED: return "FAILED";
            }

            return null;
        }

    }
}