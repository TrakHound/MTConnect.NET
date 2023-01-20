// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
