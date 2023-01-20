// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    public static class SpecificationRelationshipTypeDescriptions
    {
        /// <summary>
        /// The referenced Specification provides process limits.
        /// </summary>
        public const string LIMIT = "The referenced Specification provides process limits.";


        public static string Get(SpecificationRelationshipType specificationRelationshipType)
        {
            switch (specificationRelationshipType)
            {
                case SpecificationRelationshipType.LIMIT: return LIMIT;
            }

            return "";
        }
    }
}