// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// CompositionStateAction controlled vocabulary as defined by the MTConnect Standard.
    /// </summary>
    public enum CompositionStateAction
    {
        /// <summary>
        /// Composition is operating.
        /// </summary>
        ACTIVE,
        
        /// <summary>
        /// Composition is not operating.
        /// </summary>
        INACTIVE
    }
}