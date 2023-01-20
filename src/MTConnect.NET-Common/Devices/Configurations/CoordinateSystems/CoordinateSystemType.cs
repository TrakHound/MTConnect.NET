// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations.CoordinateSystems
{
    /// <summary>
    /// The type of coordinate system.
    /// </summary>
    public enum CoordinateSystemType
    {
        /// <summary>
        /// Stationary coordinate system referenced to earth, which is independent of the robot motion.
        /// </summary>
        WORLD,

        /// <summary>
        /// Coordinate system referenced to the base mounting surface.
        /// </summary>
        BASE,

        /// <summary>
        /// Coordinate system referenced to the object.
        /// </summary>
        OBJECT,

        /// <summary>
        /// Coordinate system referenced to the site of the task.
        /// </summary>
        TASK,

        /// <summary>
        /// Coordinate system referenced to the mechanical interface.
        /// </summary>
        MECHANICAL_INTERFACE,

        /// <summary>
        /// Coordinate system referenced to the tool or to the end effector attached to the mechanical interface. 
        /// </summary>
        TOOL,

        /// <summary>
        /// Coordinate system referenced to one of the components of a mobile platform.
        /// </summary>
        MOBILE_PLATFORM,

        /// <summary>
        /// Coordinate system referenced to the home position and orientation of the primary axes of a piece of equipment.
        /// </summary>
        MACHINE,

        /// <summary>
        /// Coordinate system referenced to the sensor which monitors the site of the task
        /// </summary>
        CAMERA
    }
}