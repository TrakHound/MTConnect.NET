// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations.CoordinateSystems
{
    public static class CoordinateSystemAttributeDescriptions
    {
		/// <summary>
		/// UUID for the coordinate system.
		/// </summary>
		public const string Uuid = "UUID for the coordinate system.";

		/// <summary>
		/// The unique identifier for this element.
		/// </summary>
		public const string Id = "The unique identifier for this element.";

        /// <summary>
        /// The name of the coordinate system.
        /// </summary>
        public const string Name = "The name of the coordinate system.";

        /// <summary>
        /// The manufacturer�s name or users name for the coordinate system.
        /// </summary>
        public const string NativeName = "The manufacturer�s name or users name for the coordinate system.";

        /// <summary>
        /// A pointer to the id attribute of the parent CoordinateSystem.
        /// </summary>
        public const string ParentIdRef = "A pointer to the id attribute of the parent CoordinateSystem.";

        /// <summary>
        /// The type of coordinate system.
        /// </summary>
        public const string Type = "The type of coordinate system.";

        /// <summary>
        /// The coordinates of the origin position of a coordinate system.
        /// </summary>
        public const string Origin = "The coordinates of the origin position of a coordinate system.";

        /// <summary>
        /// The natural language description of the CoordinateSystem.
        /// </summary>
        public const string Description = "The natural language description of the CoordinateSystem.";

    }
}