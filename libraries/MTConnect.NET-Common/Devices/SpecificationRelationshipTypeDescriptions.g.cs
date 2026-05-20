// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Description text for each <see cref="SpecificationRelationshipType"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class SpecificationRelationshipTypeDescriptions
    {
        /// <summary>
        /// Referenced Specification provides process limits.
        /// </summary>
        public const string LIMIT = "Referenced Specification provides process limits.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="SpecificationRelationshipType"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(SpecificationRelationshipType value)
        {
            switch (value)
            {
                case SpecificationRelationshipType.LIMIT: return "Referenced Specification provides process limits.";
            }

            return null;
        }
    }
}