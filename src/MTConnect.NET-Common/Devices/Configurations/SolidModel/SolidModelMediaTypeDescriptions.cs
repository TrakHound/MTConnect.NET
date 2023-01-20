// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Configurations.SolidModel
{
    public static class SolidModelMediaTypeDescriptions
    {
        /// <summary>
        /// ISO 10303 STEP AP203 or AP242 format.
        /// </summary>
        public const string STEP = "ISO 10303 STEP AP203 or AP242 format.";

        /// <summary>
        /// Stereolithography file format.
        /// </summary>
        public const string STL = "Stereolithography file format.";

        /// <summary>
        /// Geometry Description Markup Language.
        /// </summary>
        public const string GDML = "Geometry Description Markup Language.";

        /// <summary>
        /// Wavefront OBJ file format.
        /// </summary>
        public const string OBJ = "Wavefront OBJ file format.";

        /// <summary>
        /// ISO 17506.
        /// </summary>
        public const string COLLADA = "ISO 17506.";

        /// <summary>
        /// Initial Graphics Exchange Specification.
        /// </summary>
        public const string IGES = "Initial Graphics Exchange Specification.";

        /// <summary>
        /// Autodesk file format.
        /// </summary>
        public const string THREE_DS = "Autodesk file format.";

        /// <summary>
        /// Dassault file format.
        /// </summary>
        public const string ACIS = "Dassault file format.";

        /// <summary>
        /// Parasolid XT Siemens data interchange format.
        /// </summary>
        public const string X_T = "Parasolid XT Siemens data interchange format.";


        public static string Get(SolidModelMediaType solidModelMediaType)
        {
            switch (solidModelMediaType)
            {
                case SolidModelMediaType.STEP: return STEP;
                case SolidModelMediaType.STL: return STL;
                case SolidModelMediaType.GDML: return GDML;
                case SolidModelMediaType.OBJ: return OBJ;
                case SolidModelMediaType.COLLADA: return COLLADA;
                case SolidModelMediaType.IGES: return IGES;
                case SolidModelMediaType.THREE_DS: return THREE_DS;
                case SolidModelMediaType.ACIS: return ACIS;
                case SolidModelMediaType.X_T: return X_T;
            }

            return "";
        }
    }
}
