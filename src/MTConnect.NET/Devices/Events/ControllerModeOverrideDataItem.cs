// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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

        public enum SubTypes
        {
            /// <summary>
            /// A setting or operator selection used to execute a test mode to confirm the execution of machine functions.
            /// </summary>
            DRY_RUN,

            /// <summary>
            /// A setting or operator selection that changes the behavior of the controller on a piece of equipment.
            /// </summary>
            SINGLE_BLOCK,

            /// <summary>
            /// A setting or operator selection that changes the behavior of the controller on a piece of equipment.
            /// </summary>
            MACHINE_AXIS_LOCK,

            /// <summary>
            /// A setting or operator selection that changes the behavior of the controller on a piece of equipment.
            /// </summary>
            OPTIONAL_STOP,

            /// <summary>
            /// A setting or operator selection that changes the behavior of the controller on a piece of equipment.
            /// </summary>
            TOOL_CHANGE_STOP
        }


        public ControllerModeOverrideDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public ControllerModeOverrideDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            DataItemCategory = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
        }


        /// <summary>
        /// Determine if the DataItem with the specified Value is valid in the specified MTConnectVersion
        /// </summary>
        /// <param name="mtconnectVersion">The Version of the MTConnect Standard</param>
        /// <param name="value">The value of the DataItem</param>
        /// <returns>(true) if the value is valid. (false) if the value is invalid.</returns>
        public override bool IsValid(Version mtconnectVersion, object value)
        {
            if (value != null)
            {
                // Check if Unavailable
                if (value.ToString() == Streams.DataItem.Unavailable) return true;

                // Check Valid values in Enum
                var validValues = Enum.GetValues(typeof(Streams.Events.ControllerModeOverrideValue));
                foreach (var validValue in validValues)
                {
                    if (value.ToString() == validValue.ToString()) return true;
                }
            }

            return false;
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
