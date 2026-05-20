// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    /// <summary>
    /// The operational state of an Interface, controlling whether it participates in request/response handshakes.
    /// </summary>
    public enum InterfaceState
    {
        /// <summary>
        ///  The Interface is currently not operational.
        /// </summary>
        DISABLED,

        /// <summary>
        /// The Interface is currently operational and performing as expected.
        /// </summary>
        ENABLED
    }
}