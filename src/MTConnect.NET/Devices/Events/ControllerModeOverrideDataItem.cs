// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations;
using System;

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// A setting or operator selection that changes the behavior of a piece of equipment.
    /// </summary>
    public class ControllerModeOverrideDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "CONTROLLER_MODE_OVERRIDE";
        public const string NameId = "contModeOvr";
        public new const string DescriptionText = "A setting or operator selection that changes the behavior of a piece of equipment.";

        public override string TypeDescription => DescriptionText;

        public enum SubTypes
        {
            /// <summary>
            /// When DRY_RUN is ON, the equipment performs all of its normal functions, except no part or product is produced. If the equipment has a spindle, spindle operation is suspended.
            /// </summary>
            DRY_RUN,

            /// <summary>
            /// Program execution is paused after each BLOCK of code is executed when SINGLE_BLOCK is ON
            /// </summary>
            SINGLE_BLOCK,

            /// <summary>
            /// When MACHINE_AXIS_LOCK is ON, program execution continues normally, but no equipment motion occurs
            /// </summary>
            MACHINE_AXIS_LOCK,

            /// <summary>
            /// The program execution is stopped after a specific program block is executed when OPTIONAL_STOP is ON.
            /// </summary>
            OPTIONAL_STOP,

            /// <summary>
            /// Program execution is paused when a command is executed requesting a cutting tool to be changed.
            /// </summary>
            TOOL_CHANGE_STOP
        }


        public ControllerModeOverrideDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public ControllerModeOverrideDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
        }


        /// <summary>
        /// Determine if the DataItem with the specified Observation is valid in the specified MTConnectVersion
        /// </summary>
        /// <param name="mtconnectVersion">The Version of the MTConnect Standard</param>
        /// <param name="observation">The Observation to validate</param>
        /// <returns>A DataItemValidationResult indicating if Validation was successful and a Message</returns>
        protected override DataItemValidationResult OnValidation(Version mtconnectVersion, IObservation observation)
        {
            if (observation != null && !observation.Values.IsNullOrEmpty())
            {
                // Get the CDATA Value for the Observation
                var cdata = observation.GetValue(ValueTypes.CDATA);
                if (cdata != null)
                {
                    // Check Valid values in Enum
                    var validValues = Enum.GetValues(typeof(Streams.Events.ControllerModeOverrideValue));
                    foreach (var validValue in validValues)
                    {
                        if (cdata == validValue.ToString())
                        {
                            return new DataItemValidationResult(true);
                        }
                    }

                    return new DataItemValidationResult(false, "'" + cdata + "' is not a valid value");
                }
                else
                {
                    return new DataItemValidationResult(false, "No CDATA is specified for the Observation");
                }
            }

            return new DataItemValidationResult(false, "No Observation is Specified");
        }

        public override string GetSubTypeDescription() => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.DRY_RUN: return "When DRY_RUN is ON, the equipment performs all of its normal functions, except no part or product is produced.If the equipment has a spindle, spindle operation is suspended.";
                case SubTypes.SINGLE_BLOCK: return "Program execution is paused after each BLOCK of code is executed when SINGLE_BLOCK is ON";
                case SubTypes.MACHINE_AXIS_LOCK: return "When MACHINE_AXIS_LOCK is ON, program execution continues normally, but no equipment motion occurs";
                case SubTypes.OPTIONAL_STOP: return "The program execution is stopped after a specific program block is executed when OPTIONAL_STOP is ON.";
                case SubTypes.TOOL_CHANGE_STOP: return "Program execution is paused when a command is executed requesting a cutting tool to be changed.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.DRY_RUN: return "dryRun";
                case SubTypes.SINGLE_BLOCK: return "singleBlock";
                case SubTypes.MACHINE_AXIS_LOCK: return "axisLock";
                case SubTypes.OPTIONAL_STOP: return "opStop";
                case SubTypes.TOOL_CHANGE_STOP: return "tcStop";
            }

            return null;
        }
    }
}
