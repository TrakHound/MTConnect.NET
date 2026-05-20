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
        /// <summary>
        /// The current UTC instant expressed in Unix ticks (tenths of a microsecond since the Unix epoch).
        /// </summary>
        public static long Now
        {
            get
            {
                return DateTime.UtcNow.ToUnixTime();
            }
        }
    }


    /// <summary>
    /// Conversions between <see cref="DateTime"/> and the Unix-tick representation MTConnect uses for observation timestamps.
    /// </summary>
    public static class UnixTimeExtensions
    {
        /// <summary>
        /// The Unix epoch (1970-01-01T00:00:00Z) used as the reference point for tick conversions.
        /// </summary>
        public static readonly DateTime EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// The number of .NET ticks between DateTime.MinValue and the Unix epoch.
        /// </summary>
        public const long EpochTicks = 621355968000000000;


        /// <summary>
        /// Converts a DateTime to Unix ticks since the epoch, converting a Local value to UTC first; an Unspecified value is treated as UTC.
        /// </summary>
        /// <param name="d">The DateTime to convert.</param>
        public static long ToUnixTime(this DateTime d)
        {
            var x = d;
            if (d.Kind == DateTimeKind.Local) x = d.ToUniversalTime();
            var duration = x - EpochTime;
            return duration.Ticks;
        }


        /// <summary>
        /// Convert a DateTime to Unix ticks (1/10,000 of a millisecond) ensuring the value is in UTC.
        /// If the DateTime.Kind is Local, it will be converted to UTC. If Unspecified, the value will be
        /// treated as UTC by default (for backwards compatibility) or as the specified kind, then converted to UTC.
        /// </summary>
        /// <param name="d">The DateTime to convert.</param>
        /// <param name="unspecifiedAssume">The kind to assume when DateTime.Kind is Unspecified. Defaults to Utc.</param>
        /// <returns>Unix ticks since epoch in UTC.</returns>
        public static long ToUnixUtcTime(this DateTime d, DateTimeKind unspecifiedAssume = DateTimeKind.Utc)
        {
            var x = d;
            if (x.Kind == DateTimeKind.Local)
            {
                x = x.ToUniversalTime();
            }
            else if (x.Kind == DateTimeKind.Unspecified)
            {
                // Specify the assumed kind, then convert to UTC if necessary
                x = DateTime.SpecifyKind(x, unspecifiedAssume);
                if (x.Kind == DateTimeKind.Local) x = x.ToUniversalTime();
            }

            var duration = x - EpochTime;
            return duration.Ticks;
        }

        /// <summary>
        /// Alias to <see cref="ToUnixUtcTime"/> to match requested API name.
        /// </summary>
        public static long ToUnixUTCTime(this DateTime d, DateTimeKind unspecifiedAssume = DateTimeKind.Utc)
            => ToUnixUtcTime(d, unspecifiedAssume);


        /// <summary>
        /// Converts Unix ticks since the epoch to a UTC <see cref="DateTime"/>.
        /// </summary>
        /// <param name="unixTicks">The Unix ticks to convert.</param>
        public static DateTime ToDateTime(this long unixTicks)
        {
            return FromUnixTime(unixTicks);
        }

        /// <summary>
        /// Converts Unix ticks since the epoch to a <see cref="DateTime"/> in the local time zone.
        /// </summary>
        /// <param name="unixTicks">The Unix ticks to convert.</param>
        public static DateTime ToLocalDateTime(this long unixTicks)
        {
            return FromUnixTime(unixTicks).ToLocalTime();
        }

        /// <summary>
        /// Converts Unix ticks since the epoch to a UTC <see cref="DateTime"/> by adding them to <see cref="EpochTime"/>.
        /// </summary>
        /// <param name="unixTicks">The Unix ticks to convert.</param>
        public static DateTime FromUnixTime(long unixTicks)
        {
            return EpochTime.AddTicks(unixTicks);
        }
    }
}