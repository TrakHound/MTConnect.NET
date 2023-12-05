// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Diagnostics;

namespace MTConnect
{
    public static class TimeSpanExtensions
    {
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