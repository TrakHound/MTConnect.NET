// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// Accumulation of the number of times a cyclic function has attempted to, or is planned to attempt to execute.
    /// </summary>
    public class CycleCountDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "CYCLE_COUNT";
        public const string NameId = "cycleCount";

        public enum SubTypes
        {
            /// <summary>
            /// An accumulation representing all actions, items, or activities being counted independent of the outcome.ALL is the default subType.
            /// </summary>
            ALL,

            /// <summary>
            /// An accumulation representing actions, items, or activities being counted that do not conform to specification or expectation.
            /// </summary>
            BAD,

            /// <summary>
            /// An accumulation representing actions, items, or activities being counted that conform to specification or expectation.
            /// </summary>
            GOOD,

            /// <summary>
            /// The goal of the operation or process.
            /// </summary>
            TARGET,

            /// <summary>
            /// An accumulation representing actions, items, or activities yet to be counted.
            /// </summary>
            REMAINING,

            /// <summary>
            /// An accumulation representing actions, items, or activities that have been completed, independent of the outcome.
            /// </summary>
            COMPLETE,

            /// <summary>
            /// An accumulation representing actions or activities that were attempted, but failed to complete or resulted in an unexpected or unacceptable outcome.
            /// </summary>
            FAILED,

            /// <summary>
            /// An accumulation representing actions or activities that were attempted, but terminated before they could be completed.
            /// </summary>
            ABORTED
        }


        public CycleCountDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
            Units = Devices.Units.COUNT;
        }

        public CycleCountDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            DataItemCategory = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
            Units = Devices.Units.COUNT;
        }


        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ALL: return "all";
                case SubTypes.BAD: return "bad";
                case SubTypes.GOOD: return "good";
                case SubTypes.TARGET: return "target";
                case SubTypes.REMAINING: return "remaining";
                case SubTypes.COMPLETE: return "complete";
                case SubTypes.FAILED: return "failed";
                case SubTypes.ABORTED: return "aborted";
            }

            return null;
        }
    }
}
