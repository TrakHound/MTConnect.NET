// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_3_3870182_1764951682685_285104_645

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Axis along or around which the Component moves relative to a coordinate system.
    /// </summary>
    public class Axis : AbstractAxis, IAxis
    {
        public new const string DescriptionText = "Axis along or around which the Component moves relative to a coordinate system.";


        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
    }
}