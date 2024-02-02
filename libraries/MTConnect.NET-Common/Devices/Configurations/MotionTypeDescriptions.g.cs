// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    public static class MotionTypeDescriptions
    {
        /// <summary>
        /// Revolves around an axis with a continuous range of motion.
        /// </summary>
        public const string CONTINUOUS = "Revolves around an axis with a continuous range of motion.";
        
        /// <summary>
        /// Axis does not move.
        /// </summary>
        public const string FIXED = "Axis does not move.";
        
        /// <summary>
        /// Sliding linear motion along an axis with a fixed range of motion.
        /// </summary>
        public const string PRISMATIC = "Sliding linear motion along an axis with a fixed range of motion.";
        
        /// <summary>
        /// Rotates around an axis with a fixed range of motion.
        /// </summary>
        public const string REVOLUTE = "Rotates around an axis with a fixed range of motion.";


        public static string Get(MotionType value)
        {
            switch (value)
            {
                case MotionType.CONTINUOUS: return "Revolves around an axis with a continuous range of motion.";
                case MotionType.FIXED: return "Axis does not move.";
                case MotionType.PRISMATIC: return "Sliding linear motion along an axis with a fixed range of motion.";
                case MotionType.REVOLUTE: return "Rotates around an axis with a fixed range of motion.";
            }

            return null;
        }
    }
}