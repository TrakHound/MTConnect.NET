// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.DataItems
{
    public static class DataItemRelationshipTypeDescriptions
    {
        /// <summary>
        /// A reference to a DataItem that associates the values with an external entity.
        /// </summary>
        public const string ATTACHMENT = "A reference to a DataItem that associates the values with an external entity.";

        /// <summary>
        /// The referenced DataItem provides the id of the effective Coordinate System.
        /// </summary>
        public const string COORDINATE_SYSTEM = "The referenced DataItem provides the id of the effective Coordinate System.";

        /// <summary>
        /// The referenced DataItem provides process limits.
        /// </summary>
        public const string LIMIT = "The referenced DataItem provides process limits.";

        /// <summary>
        /// The referenced DataItem provides the observed values.
        /// </summary>
        public const string OBSERVATION = "The referenced DataItem provides the observed values.";


        public static string Get(DataItemRelationshipType dataItemCoordinateSystem)
        {
            switch (dataItemCoordinateSystem)
            {
                case DataItemRelationshipType.ATTACHMENT: return ATTACHMENT;
                case DataItemRelationshipType.COORDINATE_SYSTEM: return COORDINATE_SYSTEM;
                case DataItemRelationshipType.LIMIT: return LIMIT;
                case DataItemRelationshipType.OBSERVATION: return OBSERVATION;
            }

            return "";
        }
    }
}
