// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579103900791_417826_16362

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.
    /// </summary>
    public class Transformation : ITransformation
    {
        public const string DescriptionText = "Process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.";


        /// <summary>
        /// Rotations about X, Y, and Z axes are expressed in A, B, and C respectively within a 3-dimensional vector.
        /// </summary>
        public MTConnect.Degree3D Rotation { get; set; }
        
        /// <summary>
        /// Translations along X, Y, and Z axes are expressed as x,y, and z respectively within a 3-dimensional vector.
        /// </summary>
        public MTConnect.UnitVector3D Translation { get; set; }
    }
}