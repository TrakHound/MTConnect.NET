// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Direction of motion.
    /// </summary>
    public enum Direction
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
        /// 
        /// </summary>
        POSITIVE,
        
        /// <summary>
        /// 
        /// </summary>
        NEGATIVE
    }
}