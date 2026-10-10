// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

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
        /// The axes that can be read, in the order of their position macro variables (#5021 - #5026).
        /// </summary>
        public static readonly string[] SupportedAxes = { "X", "Y", "Z", "A", "B", "C" };


        /// <summary>
        /// Maximum time (in milliseconds) to wait for a reply to a command. Default is 2000.
        /// </summary>
        public int ResponseTimeout { get; set; } = 2000;

        /// <summary>
        /// The interval (in milliseconds) between reads of values that change slowly (timers, counters,
        /// software version, single block / optional stop). Default is 10000.
        /// </summary>
        public int SlowReadInterval { get; set; } = 10000;

        /// <summary>
        /// The units the control reports linear axis positions in (<c>mm</c> or <c>inch</c>). Positions
        /// reported in inches are converted to millimeters, as MTConnect requires. Default is <c>mm</c>.
        /// </summary>
        public string Units { get; set; } = MillimeterUnits;

        /// <summary>
        /// The axes to read positions for: any of <c>X</c>, <c>Y</c>, <c>Z</c> (linear) and <c>A</c>, <c>B</c>, <c>C</c> (rotary). Default is X, Y, Z.
        /// </summary>
        public List<string> Axes { get; set; } = new List<string> { "X", "Y", "Z" };

        /// <summary>
        /// When <c>true</c>, work coordinate positions (#5041 - #5046) are read in addition to machine positions. Default is <c>false</c>.
        /// </summary>
        public bool WorkPositions { get; set; }

        /// <summary>
        /// When <c>true</c>, the spindle load (#1098, machines with a Haas vector drive) is read. Default is <c>true</c>.
        /// </summary>
        public bool SpindleLoad { get; set; } = true;

        /// <summary>
        /// When <c>true</c>, the coolant level (#13013, machines with a coolant level sensor) is read. Default is <c>false</c>.
        /// </summary>
        public bool CoolantLevel { get; set; }


        /// <summary>
        /// Gets whether the control reports axis positions in inches.
        /// </summary>
        public bool IsInch => string.Equals(Units, InchUnits, StringComparison.OrdinalIgnoreCase)
            || string.Equals(Units, "in", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Returns the configured <see cref="Axes"/> that are supported, upper case, without duplicates, in the order of <see cref="SupportedAxes"/>.
        /// </summary>
        /// <returns>The axis names.</returns>
        public IEnumerable<string> GetAxes()
        {
            var configured = (Axes ?? new List<string>()).Where(o => o != null).Select(o => o.Trim().ToUpperInvariant());
            return SupportedAxes.Where(o => configured.Contains(o));
        }

        /// <summary>
        /// Returns whether the specified axis is rotary (<c>A</c>, <c>B</c> or <c>C</c>).
        /// </summary>
        /// <param name="axis">The axis name.</param>
        /// <returns><c>true</c> for a rotary axis.</returns>
        public static bool IsRotaryAxis(string axis) => axis == "A" || axis == "B" || axis == "C";

        /// <summary>
        /// Copies the shared Haas settings to another configuration.
        /// </summary>
        /// <param name="target">The configuration to copy to.</param>
        public void CopyTo(HaasConfiguration target)
        {
            if (target == null) return;

            target.ResponseTimeout = ResponseTimeout;
            target.SlowReadInterval = SlowReadInterval;
            target.Units = Units;
            target.Axes = Axes != null ? new List<string>(Axes) : null;
            target.WorkPositions = WorkPositions;
            target.SpindleLoad = SpindleLoad;
            target.CoolantLevel = CoolantLevel;
        }
    }
}
