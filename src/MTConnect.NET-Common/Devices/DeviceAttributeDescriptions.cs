// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    public static class DeviceAttributeDescriptions
    {
        /// <summary>
        /// The unique identifier for this Device in the document.
        /// </summary>
        public const string Id = "The unique identifier for this Device in the document.";

        /// <summary>
        /// The type of Device
        /// </summary>
        public const string Type = "The type of Device";

        /// <summary>
        /// The name of the Device.
        /// </summary>
        public const string Name = "The name of the Device.";

        /// <summary>
        /// A unique identifier that will only refer ot this Device.
        /// For example, this may be the manufacturer's code and the serial number.
        /// </summary>
        public const string Uuid = "A unique identifier that will only refer ot this Device. For example, this may be the manufacturer's code and the serial number.";

        /// <summary>
        /// DEPRECATED IN REL. 1.1
        /// </summary>
        public const string Iso841Class = "DEPRECATED IN REL. 1.1";

        /// <summary>
        /// The name the device manufacturer assigned to this Device.
        /// If the native name is not provided, it MUST be the name.
        /// </summary>
        public const string NativeName = "The name the device manufacturer assigned to this Device. If the native name is not provided, it MUST be the name.";

        /// <summary>
        /// The interval in milliseconds between the completion of the reading of one sample of data from a device until the beginning of the next sampling of that data.
        /// This is the number of milliseconds between data captures.
        /// </summary>
        public const string SampleInterval = "The interval in milliseconds between the completion of the reading of one sample of data from a device until the beginning of the next sampling of that data. This is the number of milliseconds between data captures.";

        /// <summary>
        /// DEPRECATED IN REL. 1.2 (REPLACED BY SampleInterval)
        /// </summary>
        public const string SampleRate = "DEPRECATED IN REL. 1.2 (REPLACED BY SampleInterval)";

        /// <summary>
        /// Specifies the CoordinateSystem for this Component and its children.
        /// </summary>
        public const string CoordinateSystemIdRef = "Specifies the CoordinateSystem for this Component and its children.";

        /// <summary>
        /// The MTConnect version of the Devices Information Model used to configure
        /// the information to be published for a piece of equipment in an MTConnect Response Document.
        /// </summary>
        public const string MTConnectVersion = "The MTConnect version of the Devices Information Model used to configure the information to be published for a piece of equipment in an MTConnect Response Document.";
    }
}
