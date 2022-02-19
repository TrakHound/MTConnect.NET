// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Events
{
    /// <summary>
    /// Describes the way the axes will be associated to each other.
    /// </summary>
    public static class AxisCouplingDescriptions
    {
        /// <summary>
        /// The axes are physically connected to each other and operate as a single unit.
        /// </summary>
        public const string TANDEM = "The axes are physically connected to each other and operate as a single unit.";

        /// <summary>
        /// The axes are not physically connected to each other but are operating together in lockstep.
        /// </summary>
        public const string SYNCHRONOUS = "The axes are not physically connected to each other but are operating together in lockstep.";

        /// <summary>
        /// The axis is the master of the CoupledAxes
        /// </summary>
        public const string MASTER = "The axis is the master of the CoupledAxes";

        /// <summary>
        /// The axis is a slave to the CoupledAxes
        /// </summary>
        public const string SLAVE = "The axis is a slave to the CoupledAxes";


        public static string Get(AxisCoupling value)
        {
            switch (value)
            {
                case AxisCoupling.TANDEM: return TANDEM;
                case AxisCoupling.SYNCHRONOUS: return SYNCHRONOUS;
                case AxisCoupling.MASTER: return MASTER;
                case AxisCoupling.SLAVE: return SLAVE;
            }

            return null;
        }
    }
}
