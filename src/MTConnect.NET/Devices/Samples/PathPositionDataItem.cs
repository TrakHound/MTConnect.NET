// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Samples
{
    /// <summary>
    /// A measured or calculated position of a control point reported by a piece of equipment expressed in WORK coordinates. 
    /// The coordinate system will revert to MACHINE coordinates if WORK coordinates are not available.
    /// </summary>
    public class PathPositionDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "PATH_POSITION";
        public const string NameId = "pathPos";

        public enum SubTypes
        {
            /// <summary>
            /// The measured or reported value of an observation.
            /// </summary>
            ACTUAL,

            /// <summary>
            /// Directive value including adjustments such as an offset or overrides.
            /// </summary>
            COMMANDED,

            /// <summary>
            /// Directive value without offsets and adjustments.
            /// </summary>
            PROGRAMMED,

            /// <summary>
            /// The goal of the operation or process.
            /// </summary>
            TARGET,

            /// <summary>
            /// The position provided by a measurement probe.
            /// </summary>
            PROBE
        }


        public PathPositionDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
            Units = Devices.Units.MILLIMETER;
        }

        public PathPositionDataItem(
            string parentId,
            SubTypes subType = SubTypes.ACTUAL,
            DataItemCoordinateSystem coordinateSystem = DataItemCoordinateSystem.MACHINE
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType, coordinateSystem));
            DataItemCategory = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
            CoordinateSystem = coordinateSystem;
            Units = Devices.Units.MILLIMETER;
        }


        public static string GetSubTypeId(SubTypes subType, DataItemCoordinateSystem coordinateSystem)
        {
            var suffix = "";
            switch (coordinateSystem)
            {
                case DataItemCoordinateSystem.MACHINE: suffix = "m"; break;
                case DataItemCoordinateSystem.WORK: suffix = "w"; break;
            }

            switch (subType)
            { 
                case SubTypes.ACTUAL: return $"act{suffix}";
                case SubTypes.COMMANDED: return $"cmd{suffix}";
                case SubTypes.PROGRAMMED: return $"prg{suffix}";
                case SubTypes.TARGET: return $"tgt{suffix}";
                case SubTypes.PROBE: return $"probe{suffix}";
            }

            return null;
        }
    }
}
