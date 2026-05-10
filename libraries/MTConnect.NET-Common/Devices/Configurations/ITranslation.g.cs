// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Translations along X, Y, and Z axes are expressed as x,y, and z respectively within a 3-dimensional vector.
    /// </summary>
    public interface ITranslation : IAbstractTranslation
    {
        /// <summary>
        /// 
        /// </summary>
        string Value { get; }
    }
}