// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Agents
{
    /// <summary>
    /// Controls how the Agent reacts when Devices data fails validation against
    /// the MTConnect Standard.
    /// </summary>
    public enum DeviceValidationLevel
    {
        /// <summary>
        /// Accept invalid device information; perform no validation action.
        /// </summary>
        Ignore,

        /// <summary>
        /// Accept invalid device information but emit a validation warning.
        /// </summary>
        Warning,

        /// <summary>
        /// Drop the invalid device information and continue processing the remainder.
        /// </summary>
        Remove,

        /// <summary>
        /// Reject the entire device information on the first validation failure.
        /// </summary>
        Strict
    }
}