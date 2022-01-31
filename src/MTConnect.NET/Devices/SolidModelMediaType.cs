// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
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
