// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// DirectionLinear controlled vocabulary as defined by the MTConnect Standard.
    /// </summary>
    public enum DirectionLinear
    {
        /// <summary>
        /// Linear position is decreasing.
        /// </summary>
        NEGATIVE,
        
        /// <summary>
        /// No direction.
        /// </summary>
        NONE,
        
        /// <summary>
        /// Linear position is increasing.
        /// </summary>
        POSITIVE
    }
}