// Copyright (c) 2020 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.DeviceFinder
{
    /// <summary>
    /// Range of IP Addresses
    /// </summary>
    public class AddressRange
    {
        public string Minimum { get; set; }

        public string Maximum { get; set; }

        public string[] AllowedAddresses { get; set; }

        public string[] DeniedAddresses { get; set; }


        public AddressRange() { }

        public AddressRange(string minimum, string maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }
    }
}
