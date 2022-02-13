// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Samples
{
    /// <summary>
    /// The measurement of the electrical potential between two points in an electrical circuit in which the current periodically reverses direction.
    /// </summary>
    public class VoltageACDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "VOLTAGE_AC";
        public const string NameId = "voltAC";
        public const string DefaultUnits = Devices.Units.VOLT;
        public new const string DescriptionText = "The measurement of the electrical potential between two points in an electrical circuit in which the current periodically reverses direction.";

        public override string TypeDescription => DescriptionText;

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


        public VoltageACDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public VoltageACDataItem(
            string parentId,
            SubTypes subType = SubTypes.ACTUAL
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
            Units = DefaultUnits;
        }

        public override string GetSubTypeDescription() => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.ACTUAL: return "The measured or reported value of an observation.";
                case SubTypes.COMMANDED: return "Directive value including adjustments such as an offset or overrides.";
                case SubTypes.PROGRAMMED: return "Directive value without offsets and adjustments.";
            }

            return null;
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
