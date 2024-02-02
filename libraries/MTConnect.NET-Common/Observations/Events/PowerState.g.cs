// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Indication of the status of the source of energy for an entity to allow it to perform its intended function or the state of an enabling signal providing permission for the entity to perform its functions.
    /// </summary>
    public enum PowerState
    {
        /// <summary>
        /// Source of energy for an entity or the enabling signal providing permission for the entity to perform its function(s) is present and active.
        /// </summary>
        ON,
        
        /// <summary>
        /// Source of energy for an entity or the enabling signal providing permission for the entity to perform its function(s) is not present or is disconnected.
        /// </summary>
        OFF
    }
}