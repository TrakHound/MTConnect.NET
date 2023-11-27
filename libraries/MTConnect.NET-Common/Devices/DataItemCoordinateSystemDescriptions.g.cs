// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public static class DataItemCoordinateSystemDescriptions
    {
        /// <summary>
        /// Unchangeable coordinate system that has machine zero as its origin.
        /// </summary>
        public const string MACHINE = "Unchangeable coordinate system that has machine zero as its origin.";
        
        /// <summary>
        /// Coordinate system that represents the working area for a particular workpiece whose origin is shifted within the `MACHINE` coordinate system.If the `WORK` coordinates are not currently defined in the piece of equipment, the `MACHINE` coordinates will be used.
        /// </summary>
        public const string WORK = "Coordinate system that represents the working area for a particular workpiece whose origin is shifted within the `MACHINE` coordinate system.If the `WORK` coordinates are not currently defined in the piece of equipment, the `MACHINE` coordinates will be used.";


        public static string Get(DataItemCoordinateSystem value)
        {
            switch (value)
            {
                case DataItemCoordinateSystem.MACHINE: return "Unchangeable coordinate system that has machine zero as its origin.";
                case DataItemCoordinateSystem.WORK: return "Coordinate system that represents the working area for a particular workpiece whose origin is shifted within the `MACHINE` coordinate system.If the `WORK` coordinates are not currently defined in the piece of equipment, the `MACHINE` coordinates will be used.";
            }

            return null;
        }
    }
}