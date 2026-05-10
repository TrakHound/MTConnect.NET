// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_3_3870182_1764951167326_754957_161

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Translations along X, Y, and Z axes are expressed as x,y, and z respectively within a 3-dimensional vector.
    /// </summary>
    public class Translation : AbstractTranslation, ITranslation
    {
        public new const string DescriptionText = "Translations along X, Y, and Z axes are expressed as x,y, and z respectively within a 3-dimensional vector.";


        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
    }
}