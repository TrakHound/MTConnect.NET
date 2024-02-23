// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    public static class SolidModelDescriptions
    {
        /// <summary>
        /// Reference to the coordinate system for this SolidModel.
        /// </summary>
        public const string CoordinateSystemIdRef = "Reference to the coordinate system for this SolidModel.";
        
        /// <summary>
        /// URL giving the location of the SolidModel. solidModelIdRef is used.href is of type `xlink:href` from the W3C XLink specification.
        /// </summary>
        public const string Href = "URL giving the location of the SolidModel. solidModelIdRef is used.href is of type `xlink:href` from the W3C XLink specification.";
        
        /// <summary>
        /// Unique identifier for this element.
        /// </summary>
        public const string Id = "Unique identifier for this element.";
        
        /// <summary>
        /// SolidModelIdRef **MUST** be given. > Note: `Item` defined in ASME Y14.100 - A nonspecific term used to denote any unit or product, including materials, parts, assemblies, equipment, accessories, and computer software.
        /// </summary>
        public const string ItemRef = "SolidModelIdRef **MUST** be given. > Note: `Item` defined in ASME Y14.100 - A nonspecific term used to denote any unit or product, including materials, parts, assemblies, equipment, accessories, and computer software.";
        
        /// <summary>
        /// Format of the referenced document.
        /// </summary>
        public const string MediaType = "Format of the referenced document.";
        
        /// <summary>
        /// NativeUnits. See DataItem.
        /// </summary>
        public const string NativeUnits = "NativeUnits. See DataItem.";
        
        /// <summary>
        /// Either a single multiplier applied to all three dimensions or a three space multiplier given in the X, Y, and Z dimensions in the coordinate system used for the SolidModel.
        /// </summary>
        public const string Scale = "Either a single multiplier applied to all three dimensions or a three space multiplier given in the X, Y, and Z dimensions in the coordinate system used for the SolidModel.";
        
        /// <summary>
        /// Associated model file if an item reference is used.
        /// </summary>
        public const string SolidModelIdRef = "Associated model file if an item reference is used.";
        
        /// <summary>
        /// Process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.
        /// </summary>
        public const string Transformation = "Process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.";
        
        /// <summary>
        /// Units. See DataItem.
        /// </summary>
        public const string Units = "Units. See DataItem.";
    }
}