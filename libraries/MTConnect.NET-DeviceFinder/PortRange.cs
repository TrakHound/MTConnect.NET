// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.DeviceFinder
{
    /// <summary>
    /// Range of TCP Ports
    /// </summary>
    public class PortRange
    {
        public int Minimum { get; set; }

        public int Maximum { get; set; }

        public int[] AllowedPorts { get; set; }

        public int[] DeniedPorts { get; set; }


        public PortRange() { }

        public PortRange(int minimum, int maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }
    }
}