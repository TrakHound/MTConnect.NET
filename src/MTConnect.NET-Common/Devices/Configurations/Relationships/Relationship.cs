// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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

        /// <summary>
        /// A MD5 Hash of the Relationship that can be used to compare Relationship objects
        /// </summary>
        public string ChangeId => CreateChangeId();


        public string CreateChangeId()
        {
            return CreateChangeId(this);
        }

        public static string CreateChangeId(IRelationship relationship)
        {
            if (relationship != null)
            {
                return ObjectExtensions.GetChangeIdPropertyString(relationship).ToMD5Hash();
            }

            return null;
        }
    }
}
