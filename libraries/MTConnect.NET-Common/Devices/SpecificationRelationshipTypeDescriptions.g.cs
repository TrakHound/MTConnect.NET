// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public static class SpecificationRelationshipTypeDescriptions
    {
        /// <summary>
        /// Referenced Specification provides process limits.
        /// </summary>
        public const string LIMIT = "Referenced Specification provides process limits.";


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