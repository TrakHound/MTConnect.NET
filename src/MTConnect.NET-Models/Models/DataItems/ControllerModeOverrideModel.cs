// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Events.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// A setting or operator selection that changes the behavior of a piece of equipment.
    /// </summary>
    public class ControllerModeOverrideModel
    {
        /// <summary>
        /// A setting or operator selection used to execute a test mode to confirm the execution of machine functions.
        /// </summary>
        public ControllerModeOverrideValue DryRun { get; set; }
        public IDataItemModel DryRunDataItem { get; set; }

        /// <summary>
        /// A setting or operator selection that changes the behavior of the controller on a piece of equipment.
        /// </summary>
        public ControllerModeOverrideValue SingleBlock { get; set; }
        public IDataItemModel SingleBlockDataItem { get; set; }

        /// <summary>
        /// A setting or operator selection that changes the behavior of the controller on a piece of equipment.
        /// </summary>
        public ControllerModeOverrideValue MachineAxisLock { get; set; }
        public IDataItemModel MachineAxisLockDataItem { get; set; }

        /// <summary>
        /// A setting or operator selection that changes the behavior of the controller on a piece of equipment.
        /// </summary>
        public ControllerModeOverrideValue OptionalStop { get; set; }
        public IDataItemModel OptionalStopDataItem { get; set; }

        /// <summary>
        /// A setting or operator selection that changes the behavior of the controller on a piece of equipment.
        /// </summary>
        public ControllerModeOverrideValue ToolChangeStop { get; set; }
        public IDataItemModel ToolChangeStopDataItem { get; set; }
    }
}