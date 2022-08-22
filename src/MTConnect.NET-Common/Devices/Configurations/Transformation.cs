// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// The process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.
    /// </summary>
    public class Transformation : ITransformation
    {
        /// <summary>
        /// Translations along X, Y, and Z axes are expressed as x,y, and z respectively within a 3-dimensional vector.      
        /// </summary>
        public string Translation { get; set; }

        /// <summary>
        /// Rotations about X, Y, and Z axes are expressed in A, B, and C respectively within a 3-dimensional vector.
        /// </summary>
        public string Rotation { get; set; }
    }
}
