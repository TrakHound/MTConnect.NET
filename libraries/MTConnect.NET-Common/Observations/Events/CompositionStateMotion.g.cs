// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// CompositionStateMotion controlled vocabulary as defined by the MTConnect Standard.
    /// </summary>
    public enum CompositionStateMotion
    {
        /// <summary>
        /// Position of the Composition is closed to the point of a positive confirmation.
        /// </summary>
        CLOSED,
        
        /// <summary>
        /// Position of the Composition is open to the point of a positive confirmation.
        /// </summary>
        OPEN,
        
        /// <summary>
        /// Position of the Composition is not open to thepoint of a positive confirmation and is not closed to the point of a positive confirmation. It is in an intermediate position.
        /// </summary>
        UNLATCHED
    }
}