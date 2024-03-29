// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// The coordinate system being used.
    /// The available values for coordinateSystem are WORK and MACHINE.
    /// </summary>
    public enum DataItemCoordinateSystem
    {
        /// <summary>
        /// An unchangeable coordinate system that has machine zero as its origin.
        /// </summary>
        MACHINE,

        /// <summary>
        /// The coordinate system that represents the working area for a particular workpiece whose origin is shifted within the MACHINE coordinate system. If the WORK coordinates are not currently defined in the piece of equipment, the MACHINE coordinates will be used.
        /// </summary>
        WORK
    }
}