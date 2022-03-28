// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// A measured or calculated position of a Component element as reported by a piece of equipment.
    /// </summary>
    public class PositionDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "POSITION";
        public const string NameId = "pos";
        public const string DefaultUnits = Devices.Units.MILLIMETER;
        public new const string DescriptionText = "A measured or calculated position of a Component element as reported by a piece of equipment.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version12;

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
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public PositionDataItem(
            string parentId,
            SubTypes subType,
            DataItemCoordinateSystem coordinateSystem = DataItemCoordinateSystem.MACHINE
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType, coordinateSystem));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
            CoordinateSystem = coordinateSystem;
            Units = DefaultUnits;
            SignificantDigits = 4;
        }

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.ACTUAL: return "The measured or reported value of an observation.";
                case SubTypes.COMMANDED: return "Directive value including adjustments such as an offset or overrides.";
                case SubTypes.PROGRAMMED: return "Directive value without offsets and adjustments.";
                case SubTypes.TARGET: return "The goal of the operation or process.";
            }

            return null;
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
