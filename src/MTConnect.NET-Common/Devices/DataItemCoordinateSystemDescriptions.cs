// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    public static class DataItemCoordinateSystemDescriptions
    {
        /// <summary>
        /// An unchangeable coordinate system that has machine zero as its origin.
        /// </summary>
        public const string MACHINE = "An unchangeable coordinate system that has machine zero as its origin.";

        /// <summary>
        /// The coordinate system that represents the working area for a particular workpiece whose origin is shifted within the MACHINE coordinate system. If the WORK coordinates are not currently defined in the piece of equipment, the MACHINE coordinates will be used.
        /// </summary>
        public const string WORK = "The coordinate system that represents the working area for a particular workpiece whose origin is shifted within the MACHINE coordinate system. If the WORK coordinates are not currently defined in the piece of equipment, the MACHINE coordinates will be used.";


        public static string Get(DataItemCoordinateSystem dataItemCoordinateSystem)
        {
            switch (dataItemCoordinateSystem)
            {
                case DataItemCoordinateSystem.MACHINE: return MACHINE;
                case DataItemCoordinateSystem.WORK: return WORK;
            }

            return "";
        }
    }
}
