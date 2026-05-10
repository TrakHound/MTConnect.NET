// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1727808122960_76782_23993

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Coordinates of the origin position of a coordinate system represented as a dataset.
    /// </summary>
    public class OriginDataSet : AbstractOrigin, IOriginDataSet, IDataSet
    {
        public new const string DescriptionText = "Coordinates of the origin position of a coordinate system represented as a dataset.";


        /// <summary>
        /// X-coordinate.
        /// </summary>
        public string X { get; set; }
        
        /// <summary>
        /// Y-coordinate.
        /// </summary>
        public string Y { get; set; }
        
        /// <summary>
        /// X-coordinate.
        /// </summary>
        public string Z { get; set; }
    }
}