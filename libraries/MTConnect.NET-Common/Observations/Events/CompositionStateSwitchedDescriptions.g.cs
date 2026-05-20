// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="CompositionStateSwitched"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class CompositionStateSwitchedDescriptions
    {
        /// <summary>
        /// Activation state of the Composition is in an `OFF` condition, it is not operating, or it is not powered.
        /// </summary>
        public const string OFF = "Activation state of the Composition is in an `OFF` condition, it is not operating, or it is not powered.";
        
        /// <summary>
        /// Activation state of the Composition is in an `ON` condition, it is operating, or it is powered.
        /// </summary>
        public const string ON = "Activation state of the Composition is in an `ON` condition, it is operating, or it is powered.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="CompositionStateSwitched"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(CompositionStateSwitched value)
        {
            switch (value)
            {
                case CompositionStateSwitched.OFF: return "Activation state of the Composition is in an `OFF` condition, it is not operating, or it is not powered.";
                case CompositionStateSwitched.ON: return "Activation state of the Composition is in an `ON` condition, it is operating, or it is powered.";
            }

            return null;
        }
    }
}