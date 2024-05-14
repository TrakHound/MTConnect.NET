// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// State or condition of a part.
    /// </summary>
    public enum PartStatus
    {
        /// <summary>
        /// Part conforms to given requirements.
        /// </summary>
        PASS,
        
        /// <summary>
        /// Part does not conform to some given requirements.
        /// </summary>
        FAIL
    }
}