// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Description text for each <see cref="RelationshipType"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class RelationshipTypeDescriptions
    {
        /// <summary>
        /// Functions as a child in the relationship with the associated element.
        /// </summary>
        public const string CHILD = "Functions as a child in the relationship with the associated element.";
        
        /// <summary>
        /// Functions as a parent in the relationship with the associated element.
        /// </summary>
        public const string PARENT = "Functions as a parent in the relationship with the associated element.";
        
        /// <summary>
        /// Functions as a peer which provides equal functionality and capabilities in the relationship with the associated element.
        /// </summary>
        public const string PEER = "Functions as a peer which provides equal functionality and capabilities in the relationship with the associated element.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="RelationshipType"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(RelationshipType value)
        {
            switch (value)
            {
                case RelationshipType.CHILD: return "Functions as a child in the relationship with the associated element.";
                case RelationshipType.PARENT: return "Functions as a parent in the relationship with the associated element.";
                case RelationshipType.PEER: return "Functions as a peer which provides equal functionality and capabilities in the relationship with the associated element.";
            }

            return null;
        }
    }
}