// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    public enum CompositionSwitchedState
    {
        /// <summary>
        /// The activation state of the Composition element is in an OFF condition, it is not operating, or it is not powered.
        /// </summary>
        OFF,

        /// <summary>
        /// The activation state of the Composition element is in an ON condition, it is operating, or it is powered.
        /// </summary>
        ON
    }
}