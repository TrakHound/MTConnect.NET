// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// 
    /// </summary>
    public enum CompositionStateLateral
    {
        /// <summary>
        /// Position of the Composition is oriented to the right to the point of a positive confirmation.
        /// </summary>
        RIGHT,
        
        /// <summary>
        /// Position of the Composition is oriented to the left to the point of a positive confirmation.
        /// </summary>
        LEFT,
        
        /// <summary>
        /// Position of the Composition is not oriented to the right to the point of a positive confirmation and is not oriented to the left to the point of a positive confirmation. It is in an intermediate position.
        /// </summary>
        TRANSITIONING
    }
}