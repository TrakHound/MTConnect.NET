// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// An indication that a piece of equipment, or a sub-part of a piece of equipment, is performing specific types of activities.
    /// </summary>
    public enum EquipmentMode
    {
        /// <summary>
        /// The equipment is not functioning in the mode designated by the subType.
        /// </summary>
        OFF,

        /// <summary>
        /// The equipment is functioning in the mode designated by the subType.
        /// </summary>
        ON
    }
}