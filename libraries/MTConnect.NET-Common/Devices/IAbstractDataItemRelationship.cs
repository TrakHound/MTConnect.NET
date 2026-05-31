// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public partial interface IAbstractDataItemRelationship
    {
        /// <summary>
        /// Condensed message digest from a secure one-way hash function. FIPS PUB 180-4
        /// </summary>
        string Hash { get; }


        /// <summary>
        /// Computes a content hash for this relationship that changes whenever its hashed properties change.
        /// </summary>
        /// <returns>A SHA-1 hash string identifying the current state of this relationship.</returns>
        string GenerateHash();
    }
}
