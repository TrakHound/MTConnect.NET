// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Samples
{
    /// <summary>
    /// The measurement of the amount of time a piece of equipment has performed different types of activities associated with the process being performed at that piece of equipment.
    /// </summary>
    public class ProcessTimerDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "PROCESS_TIMER";
        public const string NameId = "procTimer";
        public const string DefaultUnits = Devices.Units.SECOND;

        public enum SubTypes
        {
            /// <summary>
            /// The measurement of the time from the beginning of production of a part or product on a piece of equipment until the time that production is complete for that part or product on that piece of equipment.
            /// This includes the time that the piece of equipment is running, producing parts or products, or in the process of producing parts.
            /// </summary>
            PROCESS,

            /// <summary>
            /// The elapsed time of a temporary halt of action.
            /// </summary>
            DELAY
        }


        public ProcessTimerDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

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


        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.PROCESS: return "process";
                case SubTypes.DELAY: return "delay";
            }

            return null;
        }
    }
}
