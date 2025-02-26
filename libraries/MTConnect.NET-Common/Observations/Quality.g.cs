// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations
{
    public enum Quality
    {
        /// <summary>
        /// Observation is not valid against the MTConnect Standard according to the validation capabilities of the MTConnect Agent.
        /// </summary>
        INVALID,
        
        /// <summary>
        /// Observation cannot be validated.
        /// </summary>
        UNVERIFIABLE,
        
        /// <summary>
        /// Observation is valid against the MTConnect Standard.
        /// </summary>
        VALID
    }
}