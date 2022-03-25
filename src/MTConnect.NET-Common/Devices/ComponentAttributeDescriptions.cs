// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    public static class ComponentAttributeDescriptions
    {
        /// <summary>
        /// The unique identifier for this Component in the document.
        /// </summary>
        public const string Id = "The unique identifier for this Component in the document.";

        /// <summary>
        /// The type of component
        /// </summary>
        public const string Type = "The type of component";

        /// <summary>
        /// The name of the Component.
        /// </summary>
        public const string Name = "The name of the Component.";

        /// <summary>
        /// A unique identifier that will only refer ot this Component.
        /// For example, this may be the manufacturer's code and the serial number.
        /// </summary>
        public const string Uuid = "A unique identifier that will only refer ot this Component. For example, this may be the manufacturer's code and the serial number.";

        /// <summary>
        /// The name the device manufacturer assigned to the Component.
        /// If the native name is not provided, it MUST be the name.
        /// </summary>
        public const string NativeName = "The name the device manufacturer assigned to the Component. If the native name is not provided, it MUST be the name.";

        /// <summary>
        /// The interval in milliseconds between the completion of the reading of one sample of data from a component until the beginning of the next sampling of that data.
        /// This is the number of milliseconds between data captures.
        /// </summary>
        public const string SampleInterval = "The interval in milliseconds between the completion of the reading of one sample of data from a component until the beginning of the next sampling of that data. This is the number of milliseconds between data captures.";

        /// <summary>
        /// DEPRECATED IN REL. 1.2 (REPLACED BY SampleInterval)
        /// </summary>
        public const string SampleRate = "DEPRECATED IN REL. 1.2 (REPLACED BY SampleInterval)";

        /// <summary>
        /// Specifies the CoordinateSystem for this Component and its children.
        /// </summary>
        public const string CoordinateSystemIdRef = "Specifies the CoordinateSystem for this Component and its children.";
    }
}
