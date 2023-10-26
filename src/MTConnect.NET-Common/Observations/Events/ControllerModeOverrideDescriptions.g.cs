// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class ControllerModeOverrideDescriptions
    {
        /// <summary>
        /// Controllermodeoverride is in the `ON` state and the mode override is active.
        /// </summary>
        public const string ON = "Controllermodeoverride is in the `ON` state and the mode override is active.";
        
        /// <summary>
        /// Controllermodeoverride is in the `OFF` state and the mode override is inactive.
        /// </summary>
        public const string OFF = "Controllermodeoverride is in the `OFF` state and the mode override is inactive.";


        public static string Get(ControllerModeOverride value)
        {
            switch (value)
            {
                case ControllerModeOverride.ON: return "Controllermodeoverride is in the `ON` state and the mode override is active.";
                case ControllerModeOverride.OFF: return "Controllermodeoverride is in the `OFF` state and the mode override is inactive.";
            }

            return null;
        }
    }
}