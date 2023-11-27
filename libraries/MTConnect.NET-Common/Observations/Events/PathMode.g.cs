// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Describes the operational relationship between a Path entity and another Path entity for pieces of equipment comprised of multiple logical groupings of controlled axes or other logical operations.
    /// </summary>
    public enum PathMode
    {
        /// <summary>
        /// Path is operating independently and without the influence of another path.
        /// </summary>
        INDEPENDENT,
        
        /// <summary>
        /// Path provides information or state values that influences the operation of other DataItem of similar type.
        /// </summary>
        MASTER,
        
        /// <summary>
        /// Physical or logical parts which are not physically connected to each other but are operating together.
        /// </summary>
        SYNCHRONOUS,
        
        /// <summary>
        /// Axes associated with the path are mirroring the motion of the `MASTER` path.
        /// </summary>
        MIRROR
    }
}