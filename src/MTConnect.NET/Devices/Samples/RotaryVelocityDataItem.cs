// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Samples
{
    /// <summary>
    /// The measurement of the rotational speed of a rotary axis.
    /// </summary>
    public class RotaryVelocityDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "ROTARY_VELOCITY";
        public const string NameId = "speed";
        public const string DefaultUnits = Devices.Units.REVOLUTION_PER_MINUTE;

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
            PROGRAMMED
        }


        public RotaryVelocityDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
            MinimumVersion = MTConnectVersions.Version14;
        }

        public RotaryVelocityDataItem(string parentId, SubTypes subType = SubTypes.ACTUAL)
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
            Units = DefaultUnits;
            SignificantDigits = 1;
            MinimumVersion = MTConnectVersions.Version14;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            { 
                case SubTypes.ACTUAL: return "act";
                case SubTypes.COMMANDED: return "cmd";
                case SubTypes.PROGRAMMED: return "prg";
            }

            return null;
        }
    }
}
