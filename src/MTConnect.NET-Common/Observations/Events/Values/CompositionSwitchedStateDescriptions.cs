// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    public static class CompositionSwitchedStateDescriptions
    {
        /// <summary>
        /// The activation state of the Composition element is in an OFF condition, it is not operating, or it is not powered.
        /// </summary>
        public const string OFF = "The activation state of the Composition element is in an OFF condition, it is not operating, or it is not powered.";

        /// <summary>
        /// The activation state of the Composition element is in an ON condition, it is operating, or it is powered.
        /// </summary>
        public const string ON = "The activation state of the Composition element is in an ON condition, it is operating, or it is powered.";


        public static string Get(CompositionSwitchedState value)
        {
            switch (value)
            {
                case CompositionSwitchedState.OFF: return OFF;
                case CompositionSwitchedState.ON: return ON;
            }

            return null;
        }
    }
}