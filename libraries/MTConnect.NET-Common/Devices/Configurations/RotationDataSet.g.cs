// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1727807509860_595526_23700

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Rotations about X, Y, and Z axes are expressed in A, B, and C respectively within a 3-dimensional vector represented as a dataset.
    /// </summary>
    public class RotationDataSet : AbstractRotation, IRotationDataSet, IDataSet
    {
        public new const string DescriptionText = "Rotations about X, Y, and Z axes are expressed in A, B, and C respectively within a 3-dimensional vector represented as a dataset.";


        /// <summary>
        /// Rotation about X axis.
        /// </summary>
        public string A { get; set; }
        
        /// <summary>
        /// Rotation about Y axis.
        /// </summary>
        public string B { get; set; }
        
        /// <summary>
        /// Rotation about Z axis.
        /// </summary>
        public string C { get; set; }
    }
}