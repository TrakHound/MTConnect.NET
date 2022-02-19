// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Events
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
