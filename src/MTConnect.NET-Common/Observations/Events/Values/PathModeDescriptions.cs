// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// Represents the operational state of an apparatus for moving or controlling a mechanism or system.
    /// </summary>
    public static class PathModeDescriptions
    {
        /// <summary>
        /// The path is operating independently and without the influence of another path.
        /// </summary>
        public const string INDEPENDENT = "The path is operating independently and without the influence of another path.";

        /// <summary>
        /// The path provides the reference motion for a SYNCHRONOUS or MIRROR type path to follow. For non-motion type paths, the MASTER provides information or state values that influences the operation of other paths
        /// </summary>
        public const string MASTER = "The path provides the reference motion for a SYNCHRONOUS or MIRROR type path to follow. For non-motion type paths, the MASTER provides information or state values that influences the operation of other paths";

        /// <summary>
        /// The axes associated with the path are following the motion of the MASTER type path.
        /// </summary>
        public const string SYNCHRONOUS = "The axes associated with the path are following the motion of the MASTER type path.";

        /// <summary>
        /// The axes associated with the path are mirroring the motion of the MASTER path.
        /// </summary>
        public const string MIRROR = "The axes associated with the path are mirroring the motion of the MASTER path.";


        public static string Get(PathMode value)
        {
            switch (value)
            {
                case PathMode.INDEPENDENT: return INDEPENDENT;
                case PathMode.MASTER: return MASTER;
                case PathMode.SYNCHRONOUS: return SYNCHRONOUS;
                case PathMode.MIRROR: return MIRROR;
            }

            return null;
        }
    }
}
