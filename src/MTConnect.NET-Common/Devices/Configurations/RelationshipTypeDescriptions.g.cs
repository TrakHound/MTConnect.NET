// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
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
    }
}