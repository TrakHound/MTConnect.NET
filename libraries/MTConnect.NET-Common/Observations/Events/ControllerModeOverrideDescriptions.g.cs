// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class ControllerModeOverrideDescriptions
    {
        /// <summary>
        /// ControllerModeOverride is in the `ON` state and the mode override is active.
        /// </summary>
        public const string ON = "ControllerModeOverride is in the `ON` state and the mode override is active.";
        
        /// <summary>
        /// ControllerModeOverride is in the `OFF` state and the mode override is inactive.
        /// </summary>
        public const string OFF = "ControllerModeOverride is in the `OFF` state and the mode override is inactive.";


        public static string Get(ControllerModeOverride value)
        {
            switch (value)
            {
                case ControllerModeOverride.ON: return "ControllerModeOverride is in the `ON` state and the mode override is active.";
                case ControllerModeOverride.OFF: return "ControllerModeOverride is in the `OFF` state and the mode override is inactive.";
            }

            return null;
        }
    }
}