// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Samples
{
    /// <summary>
    /// A measured or calculated position of a Component element as reported by a piece of equipment.
    /// </summary>
    public class PositionDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "POSITION";
        public const string NameId = "pos";

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
            TARGET
        }


        public PositionDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
            Units = Devices.Units.MILLIMETER;
        }

        public PositionDataItem(
            string parentId,
            SubTypes subType,
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
            SignificantDigits = 4;
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
            }

            return null;
        }
    }
}
