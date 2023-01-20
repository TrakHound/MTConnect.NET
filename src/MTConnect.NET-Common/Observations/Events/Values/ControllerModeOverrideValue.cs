// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// A setting or operator selection that changes the behavior of a piece of equipment.
    /// </summary>
    public enum ControllerModeOverrideValue
    {
        /// <summary>
        /// The indicator of the ControllerModeOverride is in the OFF state and the mode override is inactive.
        /// </summary>
        OFF,

        /// <summary>
        /// The indicator of the ControllerModeOverride is in the ON state and the mode override is active.
        /// </summary>
        ON
    }
}