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


        public string GenerateHash()
        {
            return GenerateHash(this);
        }

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
