// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Configurations.Relationships
{
    public static class ComponentRelationshipTypeDescriptions
    {
        /// <summary>
        /// This piece of equipment functions as a parent in the relationship with the associated piece of equipment.
        /// </summary>
        public const string PARENT = "This piece of equipment functions as a parent in the relationship with the associated piece of equipment.";

        /// <summary>
        /// This piece of equipment functions as a child in the relationship with the associated piece of equipment.
        /// </summary>
        public const string CHILD = "This piece of equipment functions as a child in the relationship with the associated piece of equipment.";

        /// <summary>
        /// This piece of equipment functions as a peer which provides equal functionality and capabilities in the relationship with the associated piece of equipment.
        /// </summary>
        public const string PEER = "This piece of equipment functions as a peer which provides equal functionality and capabilities in the relationship with the associated piece of equipment.";


        public static string Get(ComponentRelationshipType type)
        {
            switch (type)
            {
                case ComponentRelationshipType.PARENT: return PARENT;
                case ComponentRelationshipType.CHILD: return CHILD;
                case ComponentRelationshipType.PEER: return PEER;
            }

            return "";
        }
    }
}
