// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    public enum RelationshipType
    {
        /// <summary>
        /// Functions as a child in the relationship with the associated element.
        /// </summary>
        CHILD,
        
        /// <summary>
        /// Functions as a parent in the relationship with the associated element.
        /// </summary>
        PARENT,
        
        /// <summary>
        /// Functions as a peer which provides equal functionality and capabilities in the relationship with the associated element.
        /// </summary>
        PEER
    }
}