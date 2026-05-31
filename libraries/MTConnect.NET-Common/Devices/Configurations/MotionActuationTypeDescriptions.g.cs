// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Description text for each <see cref="MotionActuationType"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class MotionActuationTypeDescriptions
    {
        /// <summary>
        /// Movement is initiated by the component.
        /// </summary>
        public const string DIRECT = "Movement is initiated by the component.";
        
        /// <summary>
        /// No actuation of this axis.> Note: Actuation of `NONE` can be either a derived `REVOLUTE` or `PRISMATIC` motion or static `FIXED` relationship.
        /// </summary>
        public const string NONE = "No actuation of this axis.> Note: Actuation of `NONE` can be either a derived `REVOLUTE` or `PRISMATIC` motion or static `FIXED` relationship.";
        
        /// <summary>
        /// Motion is computed and is used for expressing an imaginary movement.
        /// </summary>
        public const string VIRTUAL = "Motion is computed and is used for expressing an imaginary movement.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="MotionActuationType"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(MotionActuationType value)
        {
            switch (value)
            {
                case MotionActuationType.DIRECT: return "Movement is initiated by the component.";
                case MotionActuationType.NONE: return "No actuation of this axis.> Note: Actuation of `NONE` can be either a derived `REVOLUTE` or `PRISMATIC` motion or static `FIXED` relationship.";
                case MotionActuationType.VIRTUAL: return "Motion is computed and is used for expressing an imaginary movement.";
            }

            return null;
        }
    }
}