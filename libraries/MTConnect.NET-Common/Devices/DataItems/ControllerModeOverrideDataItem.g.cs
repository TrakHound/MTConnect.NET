// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218236_117661_1659

using System;
using MTConnect.Observations;
using MTConnect.Input;
using MTConnect.Observations.Events;

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Setting or operator selection that changes the behavior of a piece of equipment.
    /// </summary>
    public class ControllerModeOverrideDataItem : DataItem
    {
        /// <summary>
        /// The MTConnect <c>category</c> (SAMPLE, EVENT, or CONDITION) of this DataItem.
        /// </summary>
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;

        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this DataItem.
        /// </summary>
        public const string TypeId = "CONTROLLER_MODE_OVERRIDE";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this DataItem.
        /// </summary>
        public const string NameId = "controllerModeOverride";

        /// <summary>
        /// The default <c>representation</c> for this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Setting or operator selection that changes the behavior of a piece of equipment.";

        /// <summary>
        /// The description of this DataItem as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this DataItem.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version14;


        /// <summary>
        /// The set of <c>subType</c> values defined for this DataItem by the MTConnect Standard.
        /// </summary>
        public enum SubTypes
        {
            /// <summary>
            /// Setting or operator selection used to execute a test mode to confirm the execution of machine functions. When `DRY_RUN` is `ON`, the equipment performs all of its normal functions, except no part or product is produced.  If the equipment has a spindle, spindle operation is suspended.
            /// </summary>
            DRY_RUN,
            
            /// <summary>
            /// Setting or operator selection that changes the behavior of the controller on a piece of equipment. Program execution is paused after each block of code is executed when `SINGLE_BLOCK` is `ON`.   When `SINGLE_BLOCK` is `ON`, Execution **MUST** change to `INTERRUPTED` after completion of each block of code.
            /// </summary>
            SINGLE_BLOCK,
            
            /// <summary>
            /// Setting or operator selection that changes the behavior of the controller on a piece of equipment.  When `MACHINE_AXIS_LOCK` is `ON`, program execution continues normally, but no equipment motion occurs.
            /// </summary>
            MACHINE_AXIS_LOCK,
            
            /// <summary>
            /// Setting or operator selection that changes the behavior of the controller on a piece of equipment. The program execution is stopped after a specific program block is executed when `OPTIONAL_STOP` is `ON`.    In the case of a G-Code program, a program block containing a M01 code designates the command for an `OPTIONAL_STOP`. Execution **MUST** change to `OPTIONAL_STOP` after a program block specifying an optional stop is executed and the ControllerModeOverride `OPTIONAL_STOP` selection is `ON`.
            /// </summary>
            OPTIONAL_STOP,
            
            /// <summary>
            /// Setting or operator selection that changes the behavior of the controller on a piece of equipment.  Program execution is paused when a command is executed requesting a cutting tool to be changed. Execution **MUST** change to `INTERRUPTED` after completion of the command requesting a cutting tool to be changed and `TOOL_CHANGE_STOP` is `ON`.
            /// </summary>
            TOOL_CHANGE_STOP
        }


        /// <summary>
        /// Initializes a new instance with its category, type, and name set to the defaults for this DataItem.
        /// </summary>
        public ControllerModeOverrideDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        /// <summary>
        /// Initializes a new instance for the given parent with the specified <paramref name="subType"/>.
        /// </summary>
        /// <param name="parentId">The Id of the parent element this DataItem belongs to.</param>
        /// <param name="subType">The subType to assign to this DataItem.</param>
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
            Representation = DefaultRepresentation; 
            
        }

        /// <summary>
        /// The MTConnect Standard description of this DataItem's current <c>subType</c>.
        /// </summary>
        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        /// <summary>
        /// Returns the MTConnect Standard description for the specified <paramref name="subType"/>, or <c>null</c> when it is unknown.
        /// </summary>
        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.DRY_RUN: return "Setting or operator selection used to execute a test mode to confirm the execution of machine functions. When `DRY_RUN` is `ON`, the equipment performs all of its normal functions, except no part or product is produced.  If the equipment has a spindle, spindle operation is suspended.";
                case SubTypes.SINGLE_BLOCK: return "Setting or operator selection that changes the behavior of the controller on a piece of equipment. Program execution is paused after each block of code is executed when `SINGLE_BLOCK` is `ON`.   When `SINGLE_BLOCK` is `ON`, Execution **MUST** change to `INTERRUPTED` after completion of each block of code.";
                case SubTypes.MACHINE_AXIS_LOCK: return "Setting or operator selection that changes the behavior of the controller on a piece of equipment.  When `MACHINE_AXIS_LOCK` is `ON`, program execution continues normally, but no equipment motion occurs.";
                case SubTypes.OPTIONAL_STOP: return "Setting or operator selection that changes the behavior of the controller on a piece of equipment. The program execution is stopped after a specific program block is executed when `OPTIONAL_STOP` is `ON`.    In the case of a G-Code program, a program block containing a M01 code designates the command for an `OPTIONAL_STOP`. Execution **MUST** change to `OPTIONAL_STOP` after a program block specifying an optional stop is executed and the ControllerModeOverride `OPTIONAL_STOP` selection is `ON`.";
                case SubTypes.TOOL_CHANGE_STOP: return "Setting or operator selection that changes the behavior of the controller on a piece of equipment.  Program execution is paused when a command is executed requesting a cutting tool to be changed. Execution **MUST** change to `INTERRUPTED` after completion of the command requesting a cutting tool to be changed and `TOOL_CHANGE_STOP` is `ON`.";
            }

            return null;
        }

        /// <summary>
        /// Returns the string identifier for the specified <paramref name="subType"/>, or <c>null</c> when it is unknown.
        /// </summary>
        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.DRY_RUN: return "DRY_RUN";
                case SubTypes.SINGLE_BLOCK: return "SINGLE_BLOCK";
                case SubTypes.MACHINE_AXIS_LOCK: return "MACHINE_AXIS_LOCK";
                case SubTypes.OPTIONAL_STOP: return "OPTIONAL_STOP";
                case SubTypes.TOOL_CHANGE_STOP: return "TOOL_CHANGE_STOP";
            }

            return null;
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
                    var validValues = Enum.GetValues(typeof(ControllerModeOverride));
                    foreach (var validValue in validValues)
                    {
                        if (result == validValue.ToString())
                        {
                            return new ValidationResult(true);
                        }
                    }

                    return new ValidationResult(false, "'" + result + "' is not a valid value for CONTROLLER_MODE_OVERRIDE");
                }
                else
                {
                    return new ValidationResult(false, "No Result is specified for the Observation");
                }
            }

            return new ValidationResult(false, "No Observation is Specified");
        }

        /// <summary>
        /// Determine if the DataItem with the specified Observation is valid in the specified MTConnectVersion
        /// </summary>
        /// <param name="mtconnectVersion">The Version of the MTConnect Standard</param>
        /// <param name="observation">The Observation to validate</param>
        /// <returns>A DataItemValidationResult indicating if Validation was successful and a Message</returns>
        protected override ValidationResult OnValidation(Version mtconnectVersion, IObservation observation)
        {
            if (observation != null && !observation.Values.IsNullOrEmpty())
            {
                // Get the Result Value for the Observation
                var result = observation.GetValue(ValueKeys.Result);
                if (result != null)
                {
                    // Check Valid values in Enum
                    var validValues = Enum.GetValues(typeof(ControllerModeOverride));
                    foreach (var validValue in validValues)
                    {
                        if (result == validValue.ToString())
                        {
                            return new ValidationResult(true);
                        }
                    }

                    return new ValidationResult(false, "'" + result + "' is not a valid value for CONTROLLER_MODE_OVERRIDE");
                }
                else
                {
                    return new ValidationResult(false, "No Result is specified for the Observation");
                }
            }

            return new ValidationResult(false, "No Observation is Specified");
        }
    }
}