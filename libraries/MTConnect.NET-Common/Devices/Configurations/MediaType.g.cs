// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    public enum MediaType
    {
        /// <summary>
        /// Autodesk file format.
        /// </summary>
        THREE_D_S,
        
        /// <summary>
        /// Dassault file format.
        /// </summary>
        ACIS,
        
        /// <summary>
        /// ISO 17506.
        /// </summary>
        COLLADA,
        
        /// <summary>
        /// Geometry Description Markup Language.
        /// </summary>
        GDML,
        
        /// <summary>
        /// Initial Graphics Exchange Specification.
        /// </summary>
        IGES,
        
        /// <summary>
        /// Wavefront OBJ file format.
        /// </summary>
        OBJ,
        
        /// <summary>
        /// ISO 10303 STEP AP203 or AP242 format.
        /// </summary>
        STEP,
        
        /// <summary>
        /// STereoLithography file format.
        /// </summary>
        STL,
        
        /// <summary>
        /// Parasolid XT Siemens data interchange format.
        /// </summary>
        X_T
    }
}