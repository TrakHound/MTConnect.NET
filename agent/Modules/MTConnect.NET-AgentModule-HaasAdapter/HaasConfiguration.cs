// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Haas
{
    /// <summary>
    /// Settings shared by every Haas connection type.
    /// </summary>
    public class HaasConfiguration
    {
        /// <summary>
        /// Value of <see cref="Units"/> when the control reports positions in inches (Setting 9 = INCH).
        /// </summary>
        public const string InchUnits = "inch";

        /// <summary>
        /// Value of <see cref="Units"/> when the control reports positions in millimeters (Setting 9 = MM).
        /// </summary>
        public const string MillimeterUnits = "mm";


        /// <summary>
        /// Maximum time (in milliseconds) to wait for a reply to a command. Default is 2000.
        /// </summary>
        public int ResponseTimeout { get; set; } = 2000;

        /// <summary>
        /// The units the control reports axis positions in (<c>mm</c> or <c>inch</c>). Positions
        /// reported in inches are converted to millimeters, as MTConnect requires. Default is <c>mm</c>.
        /// </summary>
        public string Units { get; set; } = MillimeterUnits;


        /// <summary>
        /// Gets whether the control reports axis positions in inches.
        /// </summary>
        public bool IsInch => string.Equals(Units, InchUnits, StringComparison.OrdinalIgnoreCase)
            || string.Equals(Units, "in", StringComparison.OrdinalIgnoreCase);
    }
}
