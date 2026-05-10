// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1579107743274_159386_163610

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Coordinates of the origin position of a coordinate system.
    /// </summary>
    public class Origin : AbstractOrigin, IOrigin
    {
        public new const string DescriptionText = "Coordinates of the origin position of a coordinate system.";


        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
    }
}