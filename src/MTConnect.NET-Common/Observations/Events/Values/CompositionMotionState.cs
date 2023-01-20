// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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