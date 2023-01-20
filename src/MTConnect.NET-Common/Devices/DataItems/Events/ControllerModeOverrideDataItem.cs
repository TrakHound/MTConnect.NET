// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using MTConnect.Observations.Input;
using System;

namespace MTConnect.Devices.DataItems.Events
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

        public override Version MinimumVersion => MTConnectVersions.Version14;

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
        protected override ValidationResult OnValidation(Version mtconnectVersion, IObservationInput observation)
        {
            if (observation != null && !observation.Values.IsNullOrEmpty())
            {
                // Get the Result Value for the Observation
                var result = observation.GetValue(ValueKeys.Result);
                if (result != null)
                {
                    // Check Valid values in Enum
                    var validValues = Enum.GetValues(typeof(Observations.Events.Values.ControllerModeOverrideValue));
                    foreach (var validValue in validValues)
                    {
                        if (result == validValue.ToString())
                        {
                            return new ValidationResult(true);
                        }
                    }

                    return new ValidationResult(false, "'" + result + "' is not a valid value");
                }
                else
                {
                    return new ValidationResult(false, "No Result is specified for the Observation");
                }
            }

            return new ValidationResult(false, "No Observation is Specified");
        }

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

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