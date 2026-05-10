// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Translations along X, Y, and Z axes are expressed as x,y, and z respectively within a 3-dimensional vector represented as a dataset.
    /// </summary>
    public interface ITranslationDataSet : IAbstractTranslation, IDataSet
    {
        /// <summary>
        /// Translation along X axis.
        /// </summary>
        string X { get; }
        
        /// <summary>
        /// Translation along Y axis.
        /// </summary>
        string Y { get; }
        
        /// <summary>
        /// Translation along Z axis.
        /// </summary>
        string Z { get; }
    }
}