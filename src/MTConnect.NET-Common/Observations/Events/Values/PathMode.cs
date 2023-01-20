// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// Represents the operational state of an apparatus for moving or controlling a mechanism or system.
    /// </summary>
    public enum PathMode
    {
        /// <summary>
        /// The path is operating independently and without the influence of another path.
        /// </summary>
        INDEPENDENT,

        /// <summary>
        /// The path provides the reference motion for a SYNCHRONOUS or MIRROR type path to follow. For non-motion type paths, the MASTER provides information or state values that influences the operation of other paths
        /// </summary>
        MASTER,

        /// <summary>
        /// The axes associated with the path are following the motion of the MASTER type path.
        /// </summary>
        SYNCHRONOUS,

        /// <summary>
        /// The axes associated with the path are mirroring the motion of the MASTER path.
        /// </summary>
        MIRROR
    }
}
