// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public partial interface IDevice : IComponent, IContainer
    {
        /// <summary>
        /// DEPRECATED IN REL. 1.1
        /// </summary>
        string Iso841Class { get; }

        /// <summary>
        /// The Type of Device
        /// </summary>
        new string Type { get; }


        /// <summary>
        /// Computes a content hash for this Device that changes whenever its hashed members change.
        /// </summary>
        /// <returns>A SHA-1 hash string identifying the current state of this Device.</returns>
        string GenerateHash();
    }
}