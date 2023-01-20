// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// An indication of the state of an interlock function or control logic state intended to prevent the associated CHUCK component from being operated.
    /// </summary>
    public static class ChuckInterlockDescriptions
    {
        /// <summary>
        /// The chuck can be unclamped
        /// </summary>
        public const string INACTIVE = "The chuck can be unclamped";

        /// <summary>
        /// The chuck cannot be unclamped
        /// </summary>
        public const string ACTIVE = "The chuck cannot be unclamped";


        public static string Get(ChuckInterlock value)
        {
            switch (value)
            {
                case ChuckInterlock.INACTIVE: return INACTIVE;
                case ChuckInterlock.ACTIVE: return ACTIVE;
            }

            return null;
        }
    }
}