// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    public enum CompositionLateralState
    {
        /// <summary>
        /// The position of the Composition element is oriented to the left to the point of a positive confirmation
        /// </summary>
        LEFT,

        /// <summary>
        /// The position of the Composition element is oriented to the right to the point of a positive confirmation
        /// </summary>
        RIGHT,

        /// <summary>
        /// The position of the Composition element is not oriented to the right to the point of a positive confirmation and is not oriented to the left to the point of a positive confirmation. 
        /// It is in an intermediate position.
        /// </summary>
        TRANSITIONING
    }
}