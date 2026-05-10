// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1727807350445_154414_23573

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Translations along X, Y, and Z axes are expressed as x,y, and z respectively within a 3-dimensional vector represented as a dataset.
    /// </summary>
    public class TranslationDataSet : AbstractTranslation, ITranslationDataSet, IDataSet
    {
        public new const string DescriptionText = "Translations along X, Y, and Z axes are expressed as x,y, and z respectively within a 3-dimensional vector represented as a dataset.";


        /// <summary>
        /// Translation along X axis.
        /// </summary>
        public string X { get; set; }
        
        /// <summary>
        /// Translation along Y axis.
        /// </summary>
        public string Y { get; set; }
        
        /// <summary>
        /// Translation along Z axis.
        /// </summary>
        public string Z { get; set; }
    }
}