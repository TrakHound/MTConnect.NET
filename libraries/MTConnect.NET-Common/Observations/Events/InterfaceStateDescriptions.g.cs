// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class InterfaceStateDescriptions
    {
        /// <summary>
        /// Interface is currently operational and performing as expected.
        /// </summary>
        public const string ENABLED = "Interface is currently operational and performing as expected.";
        
        /// <summary>
        /// Interface is currently not operational.
        /// </summary>
        public const string DISABLED = "Interface is currently not operational.";


        public static string Get(InterfaceState value)
        {
            switch (value)
            {
                case InterfaceState.ENABLED: return "Interface is currently operational and performing as expected.";
                case InterfaceState.DISABLED: return "Interface is currently not operational.";
            }

            return null;
        }
    }
}