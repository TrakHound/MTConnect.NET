// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1727807698383_716499_23819

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Either a single multiplier applied to all three dimensions or a three space multiplier given in the X, Y, and Z dimensions in the coordinate system used for the SolidModel represented as a dataset.
    /// </summary>
    public class ScaleDataSet : DataSet, IScaleDataSet
    {
        public new const string DescriptionText = "Either a single multiplier applied to all three dimensions or a three space multiplier given in the X, Y, and Z dimensions in the coordinate system used for the SolidModel represented as a dataset.";


        /// <summary>
        /// Multiplier for X axis.
        /// </summary>
        public double X { get; set; }
        
        /// <summary>
        /// Multiplier for Y axis.
        /// </summary>
        public double Y { get; set; }
        
        /// <summary>
        /// Multiplier for Z axis.
        /// </summary>
        public double Z { get; set; }
    }
}