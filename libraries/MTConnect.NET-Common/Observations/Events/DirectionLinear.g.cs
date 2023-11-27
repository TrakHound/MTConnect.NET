// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// 
    /// </summary>
    public enum DirectionLinear
    {
        /// <summary>
        /// Linear position is increasing.
        /// </summary>
        POSITIVE,
        
        /// <summary>
        /// Linear position is decreasing.
        /// </summary>
        NEGATIVE,
        
        /// <summary>
        /// No direction.
        /// </summary>
        NONE
    }
}