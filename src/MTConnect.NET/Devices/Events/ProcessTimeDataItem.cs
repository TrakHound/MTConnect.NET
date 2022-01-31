// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// The time and date associated with an activity or event.
    /// </summary>
    public class ProcessTimeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROCESS_TIME";
        public const string NameId = "processTime";

        public enum SubTypes
        {
            /// <summary>
            /// The time and date associated with the completion of an activity or event.
            /// </summary>
            COMPLETE,

            /// <summary>
            /// Boundary when an activity or an event commences.
            /// </summary>
            START,

            /// <summary>
            /// The projected time and date associated with the end or completion of an activity or event.
            /// </summary>
            TARGET_COMPLETION
        }


        public ProcessTimeDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public ProcessTimeDataItem(
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
                case SubTypes.COMPLETE: return "complete";
                case SubTypes.START: return "start";
                case SubTypes.TARGET_COMPLETION: return "targetCompletion";
            }

            return null;
        }
    }
}
