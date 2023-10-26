// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Setting or operator selection that changes the behavior of a piece of equipment.
    /// </summary>
    public enum ControllerModeOverride
    {
        /// <summary>
        /// Controllermodeoverride is in the `ON` state and the mode override is active.
        /// </summary>
        ON,
        
        /// <summary>
        /// Controllermodeoverride is in the `OFF` state and the mode override is inactive.
        /// </summary>
        OFF
    }
}