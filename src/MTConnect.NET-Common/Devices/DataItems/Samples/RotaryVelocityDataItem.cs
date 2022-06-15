// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MTConnect.Devices.DataItems.Samples
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
        public new const string DescriptionText = "The measurement of the rotational speed of a rotary axis.";

        public override string TypeDescription => DescriptionText;

        public override Version MinimumVersion => MTConnectVersions.Version12;

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
        }


        protected override IDataItem OnProcess(IDataItem dataItem, Version mtconnectVersion)
        {
            if (SubType == SubTypes.PROGRAMMED.ToString() && mtconnectVersion < MTConnectVersions.Version13) return null;

            return dataItem;
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
