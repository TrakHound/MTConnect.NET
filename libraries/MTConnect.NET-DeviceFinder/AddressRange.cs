// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.DeviceFinder
{
    /// <summary>
    /// Range of IP Addresses
    /// </summary>
    public class AddressRange
    {
        /// <summary>The lowest IPv4 address (inclusive) in dotted-quad form that the finder is allowed to probe.</summary>
        public string Minimum { get; set; }

        /// <summary>The highest IPv4 address (inclusive) in dotted-quad form that the finder is allowed to probe.</summary>
        public string Maximum { get; set; }

        /// <summary>Optional explicit allow-list; if set, only these addresses inside the [<see cref="Minimum"/>, <see cref="Maximum"/>] range are probed.</summary>
        public string[] AllowedAddresses { get; set; }

        /// <summary>Optional explicit deny-list; addresses inside the range that match an entry here are skipped during probing.</summary>
        public string[] DeniedAddresses { get; set; }


        /// <summary>Creates an empty range; <see cref="Minimum"/>/<see cref="Maximum"/> must be set before the finder uses the range.</summary>
        public AddressRange() { }

        /// <summary>Creates a range bounded by the supplied minimum and maximum addresses.</summary>
        /// <param name="minimum">The lowest IPv4 address (inclusive) in dotted-quad form.</param>
        /// <param name="maximum">The highest IPv4 address (inclusive) in dotted-quad form.</param>
        public AddressRange(string minimum, string maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }
    }
}