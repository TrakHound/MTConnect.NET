// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations.Motion
{
    public static class MotionTypeDescriptions
    {
        /// <summary>
        /// Rotates around an axis with a fixed range of motion.
        /// </summary>
        public const string REVOLUTE = "Rotates around an axis with a fixed range of motion.";

        /// <summary>
        /// Revolves around an axis with a continuous range of motion.
        /// </summary>
        public const string CONTINUOUS = "Revolves around an axis with a continuous range of motion.";

        /// <summary>
        /// Sliding linear motion along an axis with a fixed range of motion.
        /// </summary>
        public const string PRISMATIC = "Sliding linear motion along an axis with a fixed range of motion.";

        /// <summary>
        /// The axis does not move.
        /// </summary>
        public const string FIXED = "The axis does not move.";


        public static string Get(MotionType motionType)
        {
            switch (motionType)
            {
                case MotionType.REVOLUTE: return REVOLUTE;
                case MotionType.CONTINUOUS: return CONTINUOUS;
                case MotionType.PRISMATIC: return PRISMATIC;
                case MotionType.FIXED: return FIXED;
            }

            return "";
        }
    }
}