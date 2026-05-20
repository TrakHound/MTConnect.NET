// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Classifies the kind of physical or logical capability a Collaborator advertises when negotiating an Interface handshake.
    /// </summary>
    public enum CapabilityType
    {
        /// <summary>
        /// The maximum distance the equipment can extend to access or deliver material.
        /// </summary>
        REACH,

        /// <summary>
        /// The maximum mass the equipment can bear or transfer.
        /// </summary>
        LOAD,

        /// <summary>
        /// The maximum number of discrete items the equipment can hold or stage.
        /// </summary>
        CAPACITY,

        /// <summary>
        /// The maximum bulk volume the equipment can accommodate.
        /// </summary>
        VOLUME,

        /// <summary>
        /// The dimensional or positional tolerance the equipment can guarantee.
        /// </summary>
        TOLERANCE
    }
}
