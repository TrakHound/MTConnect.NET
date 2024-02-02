// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// Reference to the target Device for this File.
    /// </summary>
    public interface IDestination
    {
        /// <summary>
        /// `uuid` of the target device or application.
        /// </summary>
        string DeviceUuid { get; }
    }
}