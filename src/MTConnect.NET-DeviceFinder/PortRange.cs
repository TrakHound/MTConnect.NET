// Copyright (c) 2020 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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
