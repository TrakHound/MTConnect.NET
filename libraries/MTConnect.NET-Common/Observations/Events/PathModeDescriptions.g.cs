// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class PathModeDescriptions
    {
        /// <summary>
        /// Path is operating independently and without the influence of another path.
        /// </summary>
        public const string INDEPENDENT = "Path is operating independently and without the influence of another path.";
        
        /// <summary>
        /// Path provides information or state values that influences the operation of other DataItem of similar type.
        /// </summary>
        public const string MASTER = "Path provides information or state values that influences the operation of other DataItem of similar type.";
        
        /// <summary>
        /// Physical or logical parts which are not physically connected to each other but are operating together.
        /// </summary>
        public const string SYNCHRONOUS = "Physical or logical parts which are not physically connected to each other but are operating together.";
        
        /// <summary>
        /// Axes associated with the path are mirroring the motion of the `MASTER` path.
        /// </summary>
        public const string MIRROR = "Axes associated with the path are mirroring the motion of the `MASTER` path.";


        public static string Get(PathMode value)
        {
            switch (value)
            {
                case PathMode.INDEPENDENT: return "Path is operating independently and without the influence of another path.";
                case PathMode.MASTER: return "Path provides information or state values that influences the operation of other DataItem of similar type.";
                case PathMode.SYNCHRONOUS: return "Physical or logical parts which are not physically connected to each other but are operating together.";
                case PathMode.MIRROR: return "Axes associated with the path are mirroring the motion of the `MASTER` path.";
            }

            return null;
        }
    }
}