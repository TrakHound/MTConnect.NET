// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605549689754_638221_1396

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier given to link the individual occurrence to a group of related occurrences, such as a process step in a process plan.
    /// </summary>
    public class ProcessAggregateIdDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "PROCESS_AGGREGATE_ID";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "processAggregateId";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Identifier given to link the individual occurrence to a group of related occurrences, such as a process step in a process plan.";

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
            /// Identifier of the authorization of the process occurrence. Synonyms include 'job id', 'work order'.
            /// </summary>
            ORDER_NUMBER,
            
            /// <summary>
            /// Identifier of the step in the process plan that this occurrence corresponds to. Synonyms include 'operation id'.
            /// </summary>
            PROCESS_STEP,
            
            /// <summary>
            /// Identifier of the process plan that this occurrence belongs to. Synonyms include 'routing id', 'job id'.
            /// </summary>
            PROCESS_PLAN
        }


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public ProcessAggregateIdDataItem()
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
        public ProcessAggregateIdDataItem(
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
                case SubTypes.ORDER_NUMBER: return "Identifier of the authorization of the process occurrence. Synonyms include 'job id', 'work order'.";
                case SubTypes.PROCESS_STEP: return "Identifier of the step in the process plan that this occurrence corresponds to. Synonyms include 'operation id'.";
                case SubTypes.PROCESS_PLAN: return "Identifier of the process plan that this occurrence belongs to. Synonyms include 'routing id', 'job id'.";
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
                case SubTypes.ORDER_NUMBER: return "ORDER_NUMBER";
                case SubTypes.PROCESS_STEP: return "PROCESS_STEP";
                case SubTypes.PROCESS_PLAN: return "PROCESS_PLAN";
            }

            return null;
        }

    }
}