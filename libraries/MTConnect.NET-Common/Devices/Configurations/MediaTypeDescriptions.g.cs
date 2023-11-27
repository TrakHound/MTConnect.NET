// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    public static class MediaTypeDescriptions
    {
        /// <summary>
        /// Autodesk file format.
        /// </summary>
        public const string THREE_D_S = "Autodesk file format.";
        
        /// <summary>
        /// Dassault file format.
        /// </summary>
        public const string ACIS = "Dassault file format.";
        
        /// <summary>
        /// ISO 17506.
        /// </summary>
        public const string COLLADA = "ISO 17506.";
        
        /// <summary>
        /// Geometry Description Markup Language.
        /// </summary>
        public const string GDML = "Geometry Description Markup Language.";
        
        /// <summary>
        /// Initial Graphics Exchange Specification.
        /// </summary>
        public const string IGES = "Initial Graphics Exchange Specification.";
        
        /// <summary>
        /// Wavefront OBJ file format.
        /// </summary>
        public const string OBJ = "Wavefront OBJ file format.";
        
        /// <summary>
        /// ISO 10303 STEP AP203 or AP242 format.
        /// </summary>
        public const string STEP = "ISO 10303 STEP AP203 or AP242 format.";
        
        /// <summary>
        /// STereoLithography file format.
        /// </summary>
        public const string STL = "STereoLithography file format.";
        
        /// <summary>
        /// Parasolid XT Siemens data interchange format.
        /// </summary>
        public const string X_T = "Parasolid XT Siemens data interchange format.";


        public static string Get(MediaType value)
        {
            switch (value)
            {
                case MediaType.THREE_D_S: return "Autodesk file format.";
                case MediaType.ACIS: return "Dassault file format.";
                case MediaType.COLLADA: return "ISO 17506.";
                case MediaType.GDML: return "Geometry Description Markup Language.";
                case MediaType.IGES: return "Initial Graphics Exchange Specification.";
                case MediaType.OBJ: return "Wavefront OBJ file format.";
                case MediaType.STEP: return "ISO 10303 STEP AP203 or AP242 format.";
                case MediaType.STL: return "STereoLithography file format.";
                case MediaType.X_T: return "Parasolid XT Siemens data interchange format.";
            }

            return null;
        }
    }
}