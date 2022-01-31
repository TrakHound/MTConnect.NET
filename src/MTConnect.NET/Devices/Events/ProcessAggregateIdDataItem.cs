// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// Identifier given to link the individual occurrence to a group of related occurrences, such as a process step in a process plan.
    /// </summary>
    public class ProcessAggregateIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROCESS_AGGREGATE_ID";
        public const string NameId = "processAggregateId";

        public enum SubTypes
        {
            /// <summary>
            /// Identifier of the authorization of the process occurrence. Synonyms include "job id", "work order".
            /// </summary>
            ORDER_NUMBER,

            /// <summary>
            /// Identifier of the process plan that this occurrence belongs to. Synonyms include "routing id", "job id".
            /// </summary>
            PROCESS_PLAN,

            /// <summary>
            /// Identifier of the step in the process plan that this occurrence corresponds to. Synonyms include "operation id".
            /// </summary>
            PROCESS_STEP
        }


        public ProcessAggregateIdDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public ProcessAggregateIdDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            DataItemCategory = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
        }


        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ORDER_NUMBER: return "orderNumber";
                case SubTypes.PROCESS_PLAN: return "processPlan";
                case SubTypes.PROCESS_STEP: return "processStep";
            }

            return null;
        }
    }
}
