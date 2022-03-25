// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// A setting or operator selection that changes the behavior of a piece of equipment.
    /// </summary>
    public static class ControllerModeOverrideValueDescriptions
    {
        /// <summary>
        /// The indicator of the ControllerModeOverride is in the OFF state and the mode override is inactive.
        /// </summary>
        public const string OFF = "The indicator of the ControllerModeOverride is in the OFF state and the mode override is inactive.";

        /// <summary>
        /// The indicator of the ControllerModeOverride is in the ON state and the mode override is active.
        /// </summary>
        public const string ON = "The indicator of the ControllerModeOverride is in the ON state and the mode override is active.";


        public static string Get(ControllerModeOverrideValue value)
        {
            switch (value)
            {
                case ControllerModeOverrideValue.OFF: return OFF;
                case ControllerModeOverrideValue.ON: return ON;
            }

            return null;
        }
    }
}
