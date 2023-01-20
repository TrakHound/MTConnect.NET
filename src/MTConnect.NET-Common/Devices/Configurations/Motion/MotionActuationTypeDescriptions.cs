// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Configurations.Motion
{
    public static class MotionActuationTypeDescriptions
    {
        /// <summary>
        /// The movement is initiated by the Component.
        /// </summary>
        public const string DIRECT = "The movement is initiated by the Component.";

        /// <summary>
        /// The motion is computed and is used for expressing an imaginary movement.
        /// </summary>
        public const string VIRTUAL = "The motion is computed and is used for expressing an imaginary movement.";

        /// <summary>
        /// There is no actuation of this Axis.
        /// </summary>
        public const string NONE = "There is no actuation of this Axis.";


        public static string Get(MotionActuationType motionActuationType)
        {
            switch (motionActuationType)
            {
                case MotionActuationType.DIRECT: return DIRECT;
                case MotionActuationType.VIRTUAL: return VIRTUAL;
                case MotionActuationType.NONE: return NONE;
            }

            return "";
        }
    }
}
