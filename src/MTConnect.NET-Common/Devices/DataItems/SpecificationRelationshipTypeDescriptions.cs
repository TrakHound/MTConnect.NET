// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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
