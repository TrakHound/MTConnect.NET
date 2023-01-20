// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
