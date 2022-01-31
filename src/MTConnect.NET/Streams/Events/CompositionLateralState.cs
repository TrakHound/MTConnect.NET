// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Events
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
