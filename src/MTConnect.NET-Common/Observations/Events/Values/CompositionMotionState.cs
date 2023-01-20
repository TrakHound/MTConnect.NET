// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    public enum CompositionMotionState
    {
        /// <summary>
        /// The position of the Composition element is open to the point of a positive confirmation
        /// </summary>
        OPEN,

        /// <summary>
        /// The position of the Composition element is not open to the point of a positive confirmation and is not closed to the point of a positive confirmation. 
        /// It is in an intermediate position.
        /// </summary>
        UNLATCHED,

        /// <summary>
        /// The position of the Composition element is closed to the point of a positive confirmation
        /// </summary>
        CLOSED
    }
}
