// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations.SolidModel
{
    /// <summary>
    /// The format of the referenced document.
    /// </summary>
    public enum SolidModelMediaType
    {
        /// <summary>
        /// ISO 10303 STEP AP203 or AP242 format.
        /// </summary>
        STEP,

        /// <summary>
        /// Stereolithography file format.
        /// </summary>
        STL,

        /// <summary>
        /// Geometry Description Markup Language.
        /// </summary>
        GDML,

        /// <summary>
        /// Wavefront OBJ file format.
        /// </summary>
        OBJ,

        /// <summary>
        /// ISO 17506.
        /// </summary>
        COLLADA,

        /// <summary>
        /// Initial Graphics Exchange Specification.
        /// </summary>
        IGES,

        /// <summary>
        /// Autodesk file format.
        /// </summary>
        THREE_DS,

        /// <summary>
        /// Dassault file format.
        /// </summary>
        ACIS,

        /// <summary>
        /// Parasolid XT Siemens data interchange format.
        /// </summary>
        X_T
    }
}