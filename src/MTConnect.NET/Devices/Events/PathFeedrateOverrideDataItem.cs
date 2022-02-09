// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
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
