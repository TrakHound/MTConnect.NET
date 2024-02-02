// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    public static class CoordinateSystemTypeDescriptions
    {
        /// <summary>
        /// Coordinate system referenced to the base mounting surface. ISO 9787:2013A base mounting surface is a connection surface between the arm and its supporting structure.ISO 9787:2013For non-robotic devices, it is the connection surface between the device and its supporting structure.
        /// </summary>
        public const string BASE = "Coordinate system referenced to the base mounting surface. ISO 9787:2013A base mounting surface is a connection surface between the arm and its supporting structure.ISO 9787:2013For non-robotic devices, it is the connection surface between the device and its supporting structure.";
        
        /// <summary>
        /// Coordinate system referenced to the sensor which monitors the site of the task. ISO 9787:2013
        /// </summary>
        public const string CAMERA = "Coordinate system referenced to the sensor which monitors the site of the task. ISO 9787:2013";
        
        /// <summary>
        /// Coordinate system referenced to the home position and orientation of the primary axes of a piece of equipment.
        /// </summary>
        public const string MACHINE = "Coordinate system referenced to the home position and orientation of the primary axes of a piece of equipment.";
        
        /// <summary>
        /// Coordinate system referenced to the mechanical interface. ISO 9787:2013
        /// </summary>
        public const string MECHANICAL_INTERFACE = "Coordinate system referenced to the mechanical interface. ISO 9787:2013";
        
        /// <summary>
        /// Coordinate system referenced to one of the components of a mobile platform. ISO 8373:2012
        /// </summary>
        public const string MOBILE_PLATFORM = "Coordinate system referenced to one of the components of a mobile platform. ISO 8373:2012";
        
        /// <summary>
        /// Coordinate system referenced to the object. ISO 9787:2013
        /// </summary>
        public const string OBJECT = "Coordinate system referenced to the object. ISO 9787:2013";
        
        /// <summary>
        /// Coordinate system referenced to the site of the task. ISO 9787:2013
        /// </summary>
        public const string TASK = "Coordinate system referenced to the site of the task. ISO 9787:2013";
        
        /// <summary>
        /// Coordinate system referenced to the tool or to the end effector attached to the mechanical interface. ISO 9787:2013
        /// </summary>
        public const string TOOL = "Coordinate system referenced to the tool or to the end effector attached to the mechanical interface. ISO 9787:2013";
        
        /// <summary>
        /// Stationary coordinate system referenced to earth, which is independent of the robot motion. ISO 9787:2013For non-robotic devices, stationary coordinate system referenced to earth, which is independent of the motion of a piece of equipment.
        /// </summary>
        public const string WORLD = "Stationary coordinate system referenced to earth, which is independent of the robot motion. ISO 9787:2013For non-robotic devices, stationary coordinate system referenced to earth, which is independent of the motion of a piece of equipment.";


        public static string Get(CoordinateSystemType value)
        {
            switch (value)
            {
                case CoordinateSystemType.BASE: return "Coordinate system referenced to the base mounting surface. ISO 9787:2013A base mounting surface is a connection surface between the arm and its supporting structure.ISO 9787:2013For non-robotic devices, it is the connection surface between the device and its supporting structure.";
                case CoordinateSystemType.CAMERA: return "Coordinate system referenced to the sensor which monitors the site of the task. ISO 9787:2013";
                case CoordinateSystemType.MACHINE: return "Coordinate system referenced to the home position and orientation of the primary axes of a piece of equipment.";
                case CoordinateSystemType.MECHANICAL_INTERFACE: return "Coordinate system referenced to the mechanical interface. ISO 9787:2013";
                case CoordinateSystemType.MOBILE_PLATFORM: return "Coordinate system referenced to one of the components of a mobile platform. ISO 8373:2012";
                case CoordinateSystemType.OBJECT: return "Coordinate system referenced to the object. ISO 9787:2013";
                case CoordinateSystemType.TASK: return "Coordinate system referenced to the site of the task. ISO 9787:2013";
                case CoordinateSystemType.TOOL: return "Coordinate system referenced to the tool or to the end effector attached to the mechanical interface. ISO 9787:2013";
                case CoordinateSystemType.WORLD: return "Stationary coordinate system referenced to earth, which is independent of the robot motion. ISO 9787:2013For non-robotic devices, stationary coordinate system referenced to earth, which is independent of the motion of a piece of equipment.";
            }

            return null;
        }
    }
}