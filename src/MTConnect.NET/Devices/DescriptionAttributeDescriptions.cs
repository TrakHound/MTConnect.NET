// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    public static class DescriptionAttributeDescriptions
    {
        /// <summary>
        /// The name of the manufacturer of the Component
        /// </summary>
        public const string Manufacturer = "The name of the manufacturer of the Component";

        /// <summary>
        /// The model description of the Component
        /// </summary>
        public const string Model = "The model description of the Component";

        /// <summary>
        /// The component's serial number
        /// </summary>
        public const string SerialNumber = "The component's serial number";

        /// <summary>
        /// The station where the Component is located when a component is part of a manufacturing unit or cell with multiple stations that share the same physical controller.
        /// </summary>
        public const string Station = "The station where the Component is located when a component is part of a manufacturing unit or cell with multiple stations that share the same physical controller.";

        /// <summary>
        /// Any additional descriptive information the implementer chooses to include regarding the Component.
        /// </summary>
        public const string CDATA = "Any additional descriptive information the implementer chooses to include regarding the Component.";
    }
}
