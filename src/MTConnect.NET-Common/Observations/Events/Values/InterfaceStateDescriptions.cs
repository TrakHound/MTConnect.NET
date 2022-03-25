// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The current functional or operational state of an Interface type element indicating whether the Interface is active or not currently functioning.
    /// </summary>
    public static class InterfaceStateDescriptions
    {
        /// <summary>
        /// The Interface is currently not operational.
        /// </summary>
        public const string DISABLED = "The Interface is currently not operational.";

        /// <summary>
        /// The Interface is currently operational and performing as expected.
        /// </summary>
        public const string ENABLED = "The Interface is currently operational and performing as expected.";


        public static string Get(InterfaceState value)
        {
            switch (value)
            {
                case InterfaceState.DISABLED: return DISABLED;
                case InterfaceState.ENABLED: return ENABLED;
            }

            return null;
        }
    }
}
