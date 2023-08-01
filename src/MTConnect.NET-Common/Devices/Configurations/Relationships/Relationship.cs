// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations.Relationships
{
    /// <summary>
    /// Relationship is an XML element that describes the association between two pieces of equipment that function independently but together perform a manufacturing operation. 
    /// Relationship may also be used to define the association between two components within a piece of equipment.
    /// </summary>
    public class Relationship : IRelationship
    {
        /// <summary>
        /// The unique identifier for this Relationship.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// A descriptive name associated with this Relationship.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Defines whether the services or functions provided by the associated piece of equipment is required for the operation of this piece of equipment.
        /// </summary>
        public Criticality Criticality { get; set; }

        /// <summary>
        /// A reference to the associated component element.
        /// </summary>
        public string IdRef { get; set; }


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

        public static string GenerateHash(IRelationship relationship)
        {
            if (relationship != null)
            {
                return ObjectExtensions.GetHashPropertyString(relationship).ToSHA1Hash();
            }

            return null;
        }
    }
}