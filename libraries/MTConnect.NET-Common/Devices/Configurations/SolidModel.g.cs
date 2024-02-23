// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1587596157073_106480_480

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// References to a file with the three-dimensional geometry of the Component or Composition.
    /// </summary>
    public class SolidModel : ISolidModel
    {
        public const string DescriptionText = "References to a file with the three-dimensional geometry of the Component or Composition.";


        /// <summary>
        /// Reference to the coordinate system for this SolidModel.
        /// </summary>
        public string CoordinateSystemIdRef { get; set; }
        
        /// <summary>
        /// URL giving the location of the SolidModel. solidModelIdRef is used.href is of type `xlink:href` from the W3C XLink specification.
        /// </summary>
        public string Href { get; set; }
        
        /// <summary>
        /// Unique identifier for this element.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// SolidModelIdRef **MUST** be given. > Note: `Item` defined in ASME Y14.100 - A nonspecific term used to denote any unit or product, including materials, parts, assemblies, equipment, accessories, and computer software.
        /// </summary>
        public string ItemRef { get; set; }
        
        /// <summary>
        /// Format of the referenced document.
        /// </summary>
        public MTConnect.Devices.Configurations.MediaType MediaType { get; set; }
        
        /// <summary>
        /// NativeUnits. See DataItem.
        /// </summary>
        public string NativeUnits { get; set; }
        
        /// <summary>
        /// Either a single multiplier applied to all three dimensions or a three space multiplier given in the X, Y, and Z dimensions in the coordinate system used for the SolidModel.
        /// </summary>
        public MTConnect.UnitVector3D Scale { get; set; }
        
        /// <summary>
        /// Associated model file if an item reference is used.
        /// </summary>
        public string SolidModelIdRef { get; set; }
        
        /// <summary>
        /// Process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.
        /// </summary>
        public MTConnect.Devices.Configurations.ITransformation Transformation { get; set; }
        
        /// <summary>
        /// Units. See DataItem.
        /// </summary>
        public string Units { get; set; }
    }
}