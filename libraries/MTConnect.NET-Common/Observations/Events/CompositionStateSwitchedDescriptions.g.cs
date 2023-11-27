// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class CompositionStateSwitchedDescriptions
    {
        /// <summary>
        /// Activation state of the Composition is in an `ON` condition, it is operating, or it is powered.
        /// </summary>
        public const string ON = "Activation state of the Composition is in an `ON` condition, it is operating, or it is powered.";
        
        /// <summary>
        /// Activation state of the Composition is in an `OFF` condition, it is not operating, or it is not powered.
        /// </summary>
        public const string OFF = "Activation state of the Composition is in an `OFF` condition, it is not operating, or it is not powered.";


        public static string Get(CompositionStateSwitched value)
        {
            switch (value)
            {
                case CompositionStateSwitched.ON: return "Activation state of the Composition is in an `ON` condition, it is operating, or it is powered.";
                case CompositionStateSwitched.OFF: return "Activation state of the Composition is in an `OFF` condition, it is not operating, or it is not powered.";
            }

            return null;
        }
    }
}