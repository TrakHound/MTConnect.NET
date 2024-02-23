// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// References to a file with the three-dimensional geometry of the Component or Composition.
    /// </summary>
    public interface ISolidModel
    {
        /// <summary>
        /// Reference to the coordinate system for this SolidModel.
        /// </summary>
        string CoordinateSystemIdRef { get; }
        
        /// <summary>
        /// URL giving the location of the SolidModel. solidModelIdRef is used.href is of type `xlink:href` from the W3C XLink specification.
        /// </summary>
        string Href { get; }
        
        /// <summary>
        /// Unique identifier for this element.
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// SolidModelIdRef **MUST** be given. > Note: `Item` defined in ASME Y14.100 - A nonspecific term used to denote any unit or product, including materials, parts, assemblies, equipment, accessories, and computer software.
        /// </summary>
        string ItemRef { get; }
        
        /// <summary>
        /// Format of the referenced document.
        /// </summary>
        MTConnect.Devices.Configurations.MediaType MediaType { get; }
        
        /// <summary>
        /// NativeUnits. See DataItem.
        /// </summary>
        string NativeUnits { get; }
        
        /// <summary>
        /// Either a single multiplier applied to all three dimensions or a three space multiplier given in the X, Y, and Z dimensions in the coordinate system used for the SolidModel.
        /// </summary>
        MTConnect.UnitVector3D Scale { get; }
        
        /// <summary>
        /// Associated model file if an item reference is used.
        /// </summary>
        string SolidModelIdRef { get; }
        
        /// <summary>
        /// Process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.
        /// </summary>
        MTConnect.Devices.Configurations.ITransformation Transformation { get; }
        
        /// <summary>
        /// Units. See DataItem.
        /// </summary>
        string Units { get; }
    }
}