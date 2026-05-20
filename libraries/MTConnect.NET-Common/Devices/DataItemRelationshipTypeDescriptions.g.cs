// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Description text for each <see cref="DataItemRelationshipType"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class DataItemRelationshipTypeDescriptions
    {
        /// <summary>
        /// Reference to a DataItem that associates the values with an external entity.
        /// </summary>
        public const string ATTACHMENT = "Reference to a DataItem that associates the values with an external entity.";
        
        /// <summary>
        /// Referenced DataItem provides the `id` of the effective Coordinate System.
        /// </summary>
        public const string COORDINATE_SYSTEM = "Referenced DataItem provides the `id` of the effective Coordinate System.";
        
        /// <summary>
        /// Referenced DataItem provides process limits.
        /// </summary>
        public const string LIMIT = "Referenced DataItem provides process limits.";
        
        /// <summary>
        /// Referenced DataItem provides the observed values.
        /// </summary>
        public const string OBSERVATION = "Referenced DataItem provides the observed values.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="DataItemRelationshipType"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(DataItemRelationshipType value)
        {
            switch (value)
            {
                case DataItemRelationshipType.ATTACHMENT: return "Reference to a DataItem that associates the values with an external entity.";
                case DataItemRelationshipType.COORDINATE_SYSTEM: return "Referenced DataItem provides the `id` of the effective Coordinate System.";
                case DataItemRelationshipType.LIMIT: return "Referenced DataItem provides process limits.";
                case DataItemRelationshipType.OBSERVATION: return "Referenced DataItem provides the observed values.";
            }

            return null;
        }
    }
}