// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.DeviceFinder
{
    /// <summary>
    /// Range of TCP Ports
    /// </summary>
    public class PortRange
    {
        /// <summary>The lowest TCP port (inclusive) the finder is allowed to probe.</summary>
        public int Minimum { get; set; }

        /// <summary>The highest TCP port (inclusive) the finder is allowed to probe.</summary>
        public int Maximum { get; set; }

        /// <summary>Optional explicit allow-list; if set, only these ports inside the [<see cref="Minimum"/>, <see cref="Maximum"/>] range are probed.</summary>
        public int[] AllowedPorts { get; set; }

        /// <summary>Optional explicit deny-list; ports inside the range that appear here are skipped during probing.</summary>
        public int[] DeniedPorts { get; set; }


        /// <summary>Creates an empty range; <see cref="Minimum"/>/<see cref="Maximum"/> must be set before the finder uses the range.</summary>
        public PortRange() { }

        /// <summary>Creates a range bounded by the supplied minimum and maximum port numbers.</summary>
        /// <param name="minimum">The lowest TCP port (inclusive).</param>
        /// <param name="maximum">The highest TCP port (inclusive).</param>
        public PortRange(int minimum, int maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }
    }
}