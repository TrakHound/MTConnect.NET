// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Setting or operator selection that changes the behavior of a piece of equipment.
    /// </summary>
    public class ControllerModeOverrideDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "CONTROLLER_MODE_OVERRIDE";
        public const string NameId = "controllerModeOverride";
             
        public new const string DescriptionText = "Setting or operator selection that changes the behavior of a piece of equipment.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14;       


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

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

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

    }
}