// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    public static class DeviceRelationshipTypeDescriptions
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


        public static string Get(DeviceRelationshipType deviceRelationshipType)
        {
            switch (deviceRelationshipType)
            {
                case DeviceRelationshipType.PARENT: return PARENT;
                case DeviceRelationshipType.CHILD: return CHILD;
                case DeviceRelationshipType.PEER: return PEER;
            }

            return "";
        }
    }
}
