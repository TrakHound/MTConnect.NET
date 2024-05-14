// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public enum DataItemCoordinateSystem
    {
        /// <summary>
        /// Unchangeable coordinate system that has machine zero as its origin.
        /// </summary>
        MACHINE,
        
        /// <summary>
        /// Coordinate system that represents the working area for a particular workpiece whose origin is shifted within the `MACHINE` coordinate system.If the `WORK` coordinates are not currently defined in the piece of equipment, the `MACHINE` coordinates will be used.
        /// </summary>
        WORK
    }
}