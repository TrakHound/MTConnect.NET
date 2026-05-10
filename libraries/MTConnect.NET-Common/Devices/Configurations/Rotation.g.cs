// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_3_3870182_1764951373391_145162_327

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Rotations about X, Y, and Z axes are expressed in A, B, and C respectively within a 3-dimensional vector.
    /// </summary>
    public class Rotation : AbstractRotation, IRotation
    {
        public new const string DescriptionText = "Rotations about X, Y, and Z axes are expressed in A, B, and C respectively within a 3-dimensional vector.";


        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
    }
}