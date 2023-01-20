// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public static class CompositionAttributeDescriptions
    {
        /// <summary>
        /// The unique identifier for this Composition in the document.
        /// </summary>
        public const string Id = "The unique identifier for this Composition in the document.";

        /// <summary>
        /// The type of Composition
        /// </summary>
        public const string Type = "The type of Composition";

        /// <summary>
        /// The name of the Composition.
        /// </summary>
        public const string Name = "The name of the Composition.";

        /// <summary>
        /// A unique identifier that will only refer ot this Composition.
        /// For example, this may be the manufacturer's code and the serial number.
        /// </summary>
        public const string Uuid = "A unique identifier that will only refer ot this Composition. For example, this may be the manufacturer's code and the serial number.";

        /// <summary>
        /// The name the device manufacturer assigned to the Composition.
        /// If the native name is not provided, it MUST be the name.
        /// </summary>
        public const string NativeName = "The name the device manufacturer assigned to the Composition. If the native name is not provided, it MUST be the name.";

        /// <summary>
        /// The interval in milliseconds between the completion of the reading of one sample of data from a Composition until the beginning of the next sampling of that data.
        /// This is the number of milliseconds between data captures.
        /// </summary>
        public const string SampleInterval = "The interval in milliseconds between the completion of the reading of one sample of data from a Composition until the beginning of the next sampling of that data. This is the number of milliseconds between data captures.";

        /// <summary>
        /// DEPRECATED IN REL. 1.2 (REPLACED BY SampleInterval)
        /// </summary>
        public const string SampleRate = "DEPRECATED IN REL. 1.2 (REPLACED BY SampleInterval)";
    }
}