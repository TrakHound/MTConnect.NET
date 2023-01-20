// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// State or condition of a part.
    /// </summary>
    public enum PartStatus
    {
        /// <summary>
        /// The part does not conform to some given requirements
        /// </summary>
        FAIL,

        /// <summary>
        /// The part does conform to given requirements.
        /// </summary>
        PASS
    }
}