// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Enumerates the categories of equipment that can participate as a <see cref="Collaborator"/> in an Interface handshake.
    /// </summary>
    public enum CollaboratorType
    {
        /// <summary>
        /// An articulated or gantry robot that loads, unloads, or transfers material.
        /// </summary>
        ROBOT,

        /// <summary>
        /// A conveyor that transports material between stations.
        /// </summary>
        CONVEYOR,

        /// <summary>
        /// A computer numerical control machine tool.
        /// </summary>
        CNC,

        /// <summary>
        /// A buffer or staging area that temporarily holds material between operations.
        /// </summary>
        BUFFER
    }
}
