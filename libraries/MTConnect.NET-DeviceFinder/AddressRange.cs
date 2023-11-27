// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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