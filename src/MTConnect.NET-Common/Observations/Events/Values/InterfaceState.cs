// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The current functional or operational state of an Interface type element indicating whether the Interface is active or not currently functioning.
    /// </summary>
    public enum InterfaceState
    {
        /// <summary>
        /// The Interface is currently not operational.
        /// </summary>
        DISABLED,

        /// <summary>
        /// The Interface is currently operational and performing as expected.
        /// </summary>
        ENABLED
    }
}
