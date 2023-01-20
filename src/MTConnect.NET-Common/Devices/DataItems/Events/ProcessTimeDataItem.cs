// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// The time and date associated with an activity or event.
    /// </summary>
    public class ProcessTimeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROCESS_TIME";
        public const string NameId = "processTime";
        public new const string DescriptionText = "The time and date associated with an activity or event.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version15;

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
            Category = CategoryId;
            Type = TypeId;
        }

        public ProcessTimeDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
            Units = Devices.Units.COUNT;
        }

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.COMPLETE: return "The time and date associated with the completion of an activity or event.";
                case SubTypes.START: return "Boundary when an activity or an event commences.";
                case SubTypes.TARGET_COMPLETION: return "The projected time and date associated with the end or completion of an activity or event.";
            }

            return null;
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