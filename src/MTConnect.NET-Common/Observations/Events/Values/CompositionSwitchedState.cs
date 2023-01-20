// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
