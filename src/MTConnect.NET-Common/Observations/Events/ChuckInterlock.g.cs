// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// State of an interlock function or control logic state intended to prevent the associated Chuck component from being operated.
    /// </summary>
    public enum ChuckInterlock
    {
        /// <summary>
        /// Chuck cannot be unclamped.
        /// </summary>
        ACTIVE,
        
        /// <summary>
        /// Chuck can be unclamped.
        /// </summary>
        INACTIVE
    }
}