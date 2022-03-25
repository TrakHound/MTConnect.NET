// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    public static class SolidModelAttributeDescriptions
    {
        /// <summary>
        /// The unique identifier for this entity within the MTConnectDevices document.
        /// </summary>
        public const string Id = "The unique identifier for this entity within the MTConnectDevices document.";

        /// <summary>
        /// The associated model file if an item reference is used.    
        /// </summary>
        public const string SolidModelIdRef = "The associated model file if an item reference is used.";

        /// <summary>
        /// The URL giving the location of the Solid Model.If not present, the model referenced in the solidModelIdRef is used.  
        /// </summary>
        public const string Href = "The URL giving the location of the Solid Model.If not present, the model referenced in the solidModelIdRef is used.";

        /// <summary>
        /// The reference to the item within the model within the related geometry.A solidModelIdRef MUST be given.
        /// </summary>
        public const string ItemRef = "The reference to the item within the model within the related geometry.A solidModelIdRef MUST be given.";

        /// <summary>
        /// The format of the referenced document.
        /// </summary>
        public const string MediaType = "The format of the referenced document.";

        /// <summary>
        /// A reference to the coordinate system for this SolidModel.
        /// </summary>
        public const string CoordinateSystemIdRef = "A reference to the coordinate system for this SolidModel.";

        /// <summary>
        /// The SolidModel Scale is either a single multiplier applied to all three dimensions or a three space multiplier given in the X, Y, and Z dimensions in the coordinate system used for the SolidModel.
        /// </summary>
        public const string Scale = "The SolidModel Scale is either a single multiplier applied to all three dimensions or a three space multiplier given in the X, Y, and Z dimensions in the coordinate system used for the SolidModel.";
    }
}
