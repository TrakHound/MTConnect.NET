// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    /// <summary>
    /// A SolidModel is a Configuration that references a file with the three-dimensional geometry of the Component or Composition.
    /// The geometry MAY have a transformation and a scale to position the Component with respect to the other Components.
    /// A geometry file can contain a set of assembled items, in this case, the SolidModel reference the id of the assembly model file and the specific item within that file.
    /// </summary>
    public interface ISolidModel
    {
        /// <summary>
        /// The unique identifier for this entity within the MTConnectDevices document.     
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The associated model file if an item reference is used.    
        /// </summary>
        string SolidModelIdRef { get; }
        
        /// <summary>
        /// The URL giving the location of the Solid Model.If not present, the model referenced in the solidModelIdRef is used.       
        /// </summary>
        string Href { get; }

        /// <summary>
        /// The reference to the item within the model within the related geometry.A solidModelIdRef MUST be given.
        /// </summary>
        string ItemRef { get; }

        /// <summary>
        /// The format of the referenced document.
        /// </summary>
        SolidModelMediaType MediaType { get; }

        /// <summary>
        /// A reference to the coordinate system for this SolidModel.
        /// </summary>
        string CoordinateSystemIdRef { get; }

        /// <summary>
        /// The translation of the origin to the position and orientation.
        /// </summary>
        Transformation Transformation { get; }

        /// <summary>
        /// The SolidModel Scale is either a single multiplier applied to all three dimensions or a three space multiplier given in the X, Y, and Z dimensions in the coordinate system used for the SolidModel.
        /// </summary>
        string Scale { get; }
    }
}
