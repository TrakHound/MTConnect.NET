// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Description text for each <see cref="DataItemCoordinateSystem"/> value as defined by the MTConnect Standard.
    /// </summary>
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


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="DataItemCoordinateSystem"/> value, or <c>null</c> when none is defined.
        /// </summary>
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