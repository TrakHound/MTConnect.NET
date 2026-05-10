// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1727807971743_962437_23839

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Axis along or around which the Component moves relative to a coordinate system represented as a dataset.
    /// </summary>
    public class AxisDataSet : AbstractAxis, IAxisDataSet, IDataSet
    {
        public new const string DescriptionText = "Axis along or around which the Component moves relative to a coordinate system represented as a dataset.";


        /// <summary>
        /// X-component of Axis.
        /// </summary>
        public double X { get; set; }
        
        /// <summary>
        /// Y-component of Axis.
        /// </summary>
        public double Y { get; set; }
        
        /// <summary>
        /// Z-component of Axis.
        /// </summary>
        public double Z { get; set; }
    }
}