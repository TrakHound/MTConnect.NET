// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// DirectionRotary controlled vocabulary as defined by the MTConnect Standard.
    /// </summary>
    public enum DirectionRotary
    {
        /// <summary>
        /// Clockwise rotation using the right-hand rule.
        /// </summary>
        CLOCKWISE,
        
        /// <summary>
        /// Counter-clockwise rotation using the right-hand rule.
        /// </summary>
        COUNTER_CLOCKWISE,
        
        /// <summary>
        /// No direction.
        /// </summary>
        NONE
    }
}