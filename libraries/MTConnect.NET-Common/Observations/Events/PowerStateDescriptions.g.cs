// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="PowerState"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class PowerStateDescriptions
    {
        /// <summary>
        /// Source of energy for an entity or the enabling signal providing permission for the entity to perform its function(s) is present and active.
        /// </summary>
        public const string ON = "Source of energy for an entity or the enabling signal providing permission for the entity to perform its function(s) is present and active.";
        
        /// <summary>
        /// Source of energy for an entity or the enabling signal providing permission for the entity to perform its function(s) is not present or is disconnected.
        /// </summary>
        public const string OFF = "Source of energy for an entity or the enabling signal providing permission for the entity to perform its function(s) is not present or is disconnected.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="PowerState"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(PowerState value)
        {
            switch (value)
            {
                case PowerState.ON: return "Source of energy for an entity or the enabling signal providing permission for the entity to perform its function(s) is present and active.";
                case PowerState.OFF: return "Source of energy for an entity or the enabling signal providing permission for the entity to perform its function(s) is not present or is disconnected.";
            }

            return null;
        }
    }
}