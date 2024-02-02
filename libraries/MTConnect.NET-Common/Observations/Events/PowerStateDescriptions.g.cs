// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
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