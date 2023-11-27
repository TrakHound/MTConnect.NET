// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Operational state of an Interface.
    /// </summary>
    public enum InterfaceState
    {
        /// <summary>
        /// Interface is currently operational and performing as expected.
        /// </summary>
        ENABLED,
        
        /// <summary>
        /// Interface is currently not operational.
        /// </summary>
        DISABLED
    }
}