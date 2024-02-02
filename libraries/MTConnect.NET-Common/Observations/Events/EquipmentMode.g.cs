// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Indication that a piece of equipment, or a sub-part of a piece of equipment, is performing specific types of activities.
    /// </summary>
    public enum EquipmentMode
    {
        /// <summary>
        /// Equipment is functioning in the mode designated by the `subType`.
        /// </summary>
        ON,
        
        /// <summary>
        /// Equipment is not functioning in the mode designated by the `subType`.
        /// </summary>
        OFF
    }
}