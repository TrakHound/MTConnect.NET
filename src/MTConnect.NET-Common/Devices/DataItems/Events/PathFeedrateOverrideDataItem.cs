// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// The value of a signal or calculation issued to adjust the feedrate for the axes associated
    /// with a Path component that may represent a single axis or the coordinated movement of multiple axes.
    /// </summary>
    public class PathFeedrateOverrideDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PATH_FEEDATE_OVERRIDE";
        public const string NameId = "feedOvr";
        public new const string DescriptionText = "The value of a signal or calculation issued to adjust the feedrate for the axes associated with a Path component that may represent a single axis or the coordinated movement of multiple axes.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version13;

        public enum SubTypes
        {
            /// <summary>
            /// Directive value without offsets and adjustments.
            /// </summary>
            PROGRAMMED,

            /// <summary>
            /// Performing an operation faster or in less time than nominal rate.
            /// </summary>
            RAPID,

            /// <summary>
            /// The value of a signal or calculation issued to adjust the feedrate of an individual linear type axis when that axis is being operated in a manual state or method(jogging).
            /// </summary>
            JOG
        }


        public PathFeedrateOverrideDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public PathFeedrateOverrideDataItem(
            string parentId,
            SubTypes subType = SubTypes.PROGRAMMED
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
            Units = Devices.Units.PERCENT;
        }

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.PROGRAMMED: return "Directive value without offsets and adjustments.";
                case SubTypes.RAPID: return "Performing an operation faster or in less time than nominal rate.";
                case SubTypes.JOG: return "The value of a signal or calculation issued to adjust the feedrate of an individual linear type axis when that axis is being operated in a manual state or method(jogging).";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.PROGRAMMED: return "";
                case SubTypes.RAPID: return "rapid";
                case SubTypes.JOG: return "jog";
            }

            return null;
        }
    }
}