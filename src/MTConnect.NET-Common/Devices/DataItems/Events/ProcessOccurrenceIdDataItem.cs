// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// An identifier of a process being executed by the device.
    /// </summary>
    public class ProcessOccurrenceIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROCESS_OCCURRENCE_ID";
        public const string NameId = "processOccurrenceId";
        public new const string DescriptionText = "An identifier of a process being executed by the device.";

        public override string TypeDescription => DescriptionText;

        public override Version MinimumVersion => MTConnectVersions.Version17;


        public enum SubTypes
        {
            /// <summary>
            /// Phase or segment of a recipe or program.
            /// </summary>
            ACTIVITY,

            /// <summary>
            /// Step of a discrete manufacturing process.
            /// </summary>
            OPERATION,

            /// <summary>
            /// Process as part of product production; can be a subprocess of a larger process.
            /// </summary>
            RECIPE,

            /// <summary>
            /// Phase of a recipe process.
            /// </summary>
            SEGMENT
        }


        public ProcessOccurrenceIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public ProcessOccurrenceIdDataItem(string parentId, SubTypes subType = SubTypes.ACTIVITY)
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
        }


        protected override IDataItem OnProcess(IDataItem dataItem, Version mtconnectVersion)
        {
            if (!string.IsNullOrEmpty(SubType) && mtconnectVersion < MTConnectVersions.Version20) return null;

            return dataItem;
        }

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.ACTIVITY: return "Phase or segment of a recipe or program.";
                case SubTypes.OPERATION: return "Step of a discrete manufacturing process.";
                case SubTypes.RECIPE: return "Process as part of product production; can be a subprocess of a larger process.";
                case SubTypes.SEGMENT: return "Phase of a recipe process.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ACTIVITY: return "acty";
                case SubTypes.OPERATION: return "op";
                case SubTypes.RECIPE: return "rpe";
                case SubTypes.SEGMENT: return "seg";
            }

            return null;
        }
    }
}