// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_3_3870182_1764951924679_898861_876

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Either a single multiplier applied to all three dimensions or a three space multiplier given in the X, Y, and Z dimensions in the coordinate system used for the SolidModel.
    /// </summary>
    public class Scale : AbstractScale, IScale
    {
        public new const string DescriptionText = "Either a single multiplier applied to all three dimensions or a three space multiplier given in the X, Y, and Z dimensions in the coordinate system used for the SolidModel.";


        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
    }
}