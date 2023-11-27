// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations
{
    /// <summary>
    /// Level of the Condition (Normal, Warning, Fault, or Unavailable)
    /// </summary>
    public enum ConditionLevel
    {
        /// <summary>
        /// The value of the item is in an indeterminate state since the data source is no longer providing data. This will also be the initial state of the Condition before a connection is established with the data source. The Condition MUST be Unavailable when the value is unknown.
        /// </summary>
        UNAVAILABLE,

        /// <summary>
        /// The item being monitored is operating normally and no action is required. Normal also indicates a Fault or Warning condition has been cleared if the item was previously identified with Fault or Warning.
        /// </summary>
        NORMAL,

        /// <summary>
        /// The item being monitored is moving into an abnormal range and should be observed. No action is required at this time. Transition to a Normal condition indicates that the Warning condition has been cleared.
        /// </summary>
        WARNING,

        /// <summary>
        /// The item has failed and intervention is required to return to a Normal condition. Transition to a Normal condition indicates that the Fault condition has been cleared. A Fault condition is something that always needs to be acknowledged before operation can continue.
        /// </summary>
        FAULT
    }
}