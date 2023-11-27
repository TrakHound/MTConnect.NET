// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class AxisCouplingDescriptions
    {
        /// <summary>
        /// Axes are physically connected to each other and operate as a single unit.
        /// </summary>
        public const string TANDEM = "Axes are physically connected to each other and operate as a single unit.";
        
        /// <summary>
        /// Axes are not physically connected to each other but are operating together in lockstep.
        /// </summary>
        public const string SYNCHRONOUS = "Axes are not physically connected to each other but are operating together in lockstep.";
        
        /// <summary>
        /// Axis is the master of the CoupledAxes.
        /// </summary>
        public const string MASTER = "Axis is the master of the CoupledAxes.";
        
        /// <summary>
        /// Axis is a slave to the CoupledAxes.
        /// </summary>
        public const string SLAVE = "Axis is a slave to the CoupledAxes.";


        public static string Get(AxisCoupling value)
        {
            switch (value)
            {
                case AxisCoupling.TANDEM: return "Axes are physically connected to each other and operate as a single unit.";
                case AxisCoupling.SYNCHRONOUS: return "Axes are not physically connected to each other but are operating together in lockstep.";
                case AxisCoupling.MASTER: return "Axis is the master of the CoupledAxes.";
                case AxisCoupling.SLAVE: return "Axis is a slave to the CoupledAxes.";
            }

            return null;
        }
    }
}