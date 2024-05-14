// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect
{
    /// <summary>
    /// DateTime represented in Unix Ticks. The time in Ticks (1 / 10,000 of a Millisecond) since the Unix Epoch
    /// </summary>
    public static class UnixDateTime
    {
        public static long Now
        {
            get
            {
                return DateTime.UtcNow.ToUnixTime();
            }
        }
    }


    public static class UnixTimeExtensions
    {
        public static readonly DateTime EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public const long EpochTicks = 621355968000000000;


        public static long ToUnixTime(this DateTime d)
        {
            var x = d;
            if (d.Kind == DateTimeKind.Local) x = d.ToUniversalTime();
            var duration = x - EpochTime;
            return duration.Ticks;
        }


        public static DateTime ToDateTime(this long unixTicks)
        {
            return FromUnixTime(unixTicks);
        }

        public static DateTime ToLocalDateTime(this long unixTicks)
        {
            return FromUnixTime(unixTicks).ToLocalTime();
        }

        public static DateTime FromUnixTime(long unixTicks)
        {
            return EpochTime.AddTicks(unixTicks);
        }
    }
}