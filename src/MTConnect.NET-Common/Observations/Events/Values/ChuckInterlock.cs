// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// An indication of the state of an interlock function or control logic state intended to prevent the associated CHUCK component from being operated.
    /// </summary>
    public enum ChuckInterlock
    {
        /// <summary>
        /// The chuck can be unclamped
        /// </summary>
        INACTIVE,

        /// <summary>
        /// The chuck cannot be unclamped
        /// </summary>
        ACTIVE
    }
}