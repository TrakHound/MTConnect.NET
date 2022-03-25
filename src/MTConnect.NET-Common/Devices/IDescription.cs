// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    /// <summary>
    /// An element that can contain any description content.
    /// </summary>
    public interface IDescription
    {
        /// <summary>
        /// The name of the manufacturer of the Component
        /// </summary>
        string Manufacturer { get; }

        /// <summary>
        /// The model description of the Component
        /// </summary>
        string Model { get; }

        /// <summary>
        /// The component's serial number
        /// </summary>
        string SerialNumber { get; }

        /// <summary>
        /// The station where the Component is located when a component is part of a manufacturing unit or cell with multiple stations that share the same physical controller.
        /// </summary>
        string Station { get; }

        /// <summary>
        /// Any additional descriptive information the implementer chooses to include regarding the Component.
        /// </summary>
        string CDATA { get; }
    }
}
