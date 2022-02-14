// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    /// <summary>
    /// The type of coordinate system.
    /// </summary>
    public static class CoordinateSystemTypeDescriptions
    {
        /// <summary>
        /// Stationary coordinate system referenced to earth, which is independent of the robot motion.
        /// </summary>
        public const string WORLD = "Stationary coordinate system referenced to earth, which is independent of the robot motion.";

        /// <summary>
        /// Coordinate system referenced to the base mounting surface.
        /// </summary>
        public const string BASE = "Coordinate system referenced to the base mounting surface.";

        /// <summary>
        /// Coordinate system referenced to the object.
        /// </summary>
        public const string OBJECT = "Coordinate system referenced to the object.";

        /// <summary>
        /// Coordinate system referenced to the site of the task.
        /// </summary>
        public const string TASK = "Coordinate system referenced to the site of the task.";

        /// <summary>
        /// Coordinate system referenced to the mechanical interface.
        /// </summary>
        public const string MECHANICAL_INTERFACE = "Coordinate system referenced to the mechanical interface.";

        /// <summary>
        /// Coordinate system referenced to the tool or to the end effector attached to the mechanical interface. 
        /// </summary>
        public const string TOOL = "Coordinate system referenced to the tool or to the end effector attached to the mechanical interface.";

        /// <summary>
        /// Coordinate system referenced to one of the components of a mobile platform.
        /// </summary>
        public const string MOBILE_PLATFORM = "Coordinate system referenced to one of the components of a mobile platform.";

        /// <summary>
        /// Coordinate system referenced to the home position and orientation of the primary axes of a piece of equipment.
        /// </summary>
        public const string MACHINE = "Coordinate system referenced to the home position and orientation of the primary axes of a piece of equipment.";

        /// <summary>
        /// Coordinate system referenced to the sensor which monitors the site of the task
        /// </summary>
        public const string CAMERA = "Coordinate system referenced to the sensor which monitors the site of the task";


        public static string Get(CoordinateSystemType coordinateSystemType)
        {
            switch (coordinateSystemType)
            {
                case CoordinateSystemType.WORLD: return WORLD;
                case CoordinateSystemType.BASE: return BASE;
                case CoordinateSystemType.OBJECT: return OBJECT;
                case CoordinateSystemType.TASK: return TASK;
                case CoordinateSystemType.MECHANICAL_INTERFACE: return MECHANICAL_INTERFACE;
                case CoordinateSystemType.TOOL: return TOOL;
                case CoordinateSystemType.MOBILE_PLATFORM: return MOBILE_PLATFORM;
                case CoordinateSystemType.MACHINE: return MACHINE;
                case CoordinateSystemType.CAMERA: return CAMERA;
            }

            return "";
        }
    }
}
