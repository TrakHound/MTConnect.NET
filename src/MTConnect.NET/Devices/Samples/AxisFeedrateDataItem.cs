// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Samples
{
    /// <summary>
    /// The measurement of the feedrate of a linear axis.
    /// </summary>
    public class AxisFeedrateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "AXIS_FEEDRATE";
        public const string NameId = "feed";
        public const string DefaultUnits = Devices.Units.MILLIMETER_PER_SECOND;
        public new const string DescriptionText = "The measurement of the feedrate of a linear axis.";

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
            /// The feedrate specified by a logic or motion program, by a pre-set value, or set by a switch as the feedrate for a linear axis when operating in a manual state or method(jogging).
            /// </summary>
            JOG,

            /// <summary>
            /// Directive value without offsets and adjustments.
            /// </summary>
            PROGRAMMED,

            /// <summary>
            /// Performing an operation faster or in less time than nominal rate.
            /// </summary>
            RAPID
        }


        public AxisFeedrateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public AxisFeedrateDataItem(
            string parentId,
            SubTypes subType)
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
                case SubTypes.JOG: return "The feedrate specified by a logic or motion program, by a pre-set value, or set by a switch as the feedrate for a linear axis when operating in a manual state or method(jogging).";
                case SubTypes.PROGRAMMED: return "Directive value without offsets and adjustments.";
                case SubTypes.RAPID: return "Performing an operation faster or in less time than nominal rate.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            { 
                case SubTypes.ACTUAL: return "act";
                case SubTypes.COMMANDED: return "cmd";
                case SubTypes.JOG: return "jog";
                case SubTypes.PROGRAMMED: return "prg";
                case SubTypes.RAPID: return "rapid";
            }

            return null;
        }
    }
}
