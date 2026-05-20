// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Diagnostics;

namespace MTConnect
{
    /// <summary>
    /// Extensions for measuring elapsed time at sub-millisecond resolution from a high-frequency Stopwatch.
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Returns the elapsed time of the stopwatch in fractional milliseconds, computed from its tick count and frequency for resolution finer than ElapsedMilliseconds; returns -1 when the stopwatch is null.
        /// </summary>
        /// <param name="stpw">The stopwatch to read; may be null.</param>
        public static double GetElapsedMilliseconds(this Stopwatch stpw)
        {
            if (stpw != null)
            {
                return 1000.0 * stpw.ElapsedTicks / Stopwatch.Frequency;
            }

            return -1;
        }
    }
}