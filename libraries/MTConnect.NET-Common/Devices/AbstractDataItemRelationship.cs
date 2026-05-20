// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public partial class AbstractDataItemRelationship
    {
        private string _hash;
        /// <summary>
        /// Condensed message digest from a secure one-way hash function. FIPS PUB 180-4
        /// </summary>
        public string Hash
        {
            get
            {
                if (_hash == null) _hash = GenerateHash();
                return _hash;
            }
        }


        /// <summary>
        /// Computes a content hash for this relationship that changes whenever its hashed properties change.
        /// </summary>
        /// <returns>A SHA-1 hash string identifying the current state of this relationship.</returns>
        public string GenerateHash()
        {
            return GenerateHash(this);
        }

        /// <summary>
        /// Computes a content hash for the specified DataItem relationship.
        /// </summary>
        /// <param name="relationship">The relationship to hash.</param>
        /// <returns>A SHA-1 hash string, or null when <paramref name="relationship"/> is null.</returns>
        public static string GenerateHash(IAbstractDataItemRelationship relationship)
        {
            if (relationship != null)
            {
                return ObjectExtensions.GetHashPropertyString(relationship).ToSHA1Hash();
            }

            return null;
        }
    }
}
