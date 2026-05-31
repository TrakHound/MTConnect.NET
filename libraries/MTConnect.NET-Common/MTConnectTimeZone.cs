// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MTConnect
{
    /// <summary>
    /// A named time zone identified by an abbreviation and a UTC offset, with helpers to resolve it to a system <see cref="TimeZoneInfo"/> and to express UTC timestamps in that zone.
    /// </summary>
    public class MTConnectTimeZone
    {
        private readonly static IEnumerable<MTConnectTimeZone> _timeZones = Init();
        private readonly static string _offsetParsePattern = "^(?:UTC)?\\s?([-+])([0-9]{1,2})(?::([0-9]{2}))?$";


        /// <summary>
        /// The short abbreviation for the zone (e.g. "CST").
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        /// The human-readable name of the zone (e.g. "Central Standard Time").
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The UTC offset in the textual "UTC+H:MM" form.
        /// </summary>
        public string Offset { get; set; }


        /// <summary>
        /// Initializes a time zone from its abbreviation, name, and UTC offset.
        /// </summary>
        /// <param name="abbreviation">The short abbreviation.</param>
        /// <param name="name">The human-readable name.</param>
        /// <param name="offset">The UTC offset in "UTC+H:MM" form.</param>
        public MTConnectTimeZone(string abbreviation, string name, string offset)
        {
            Abbreviation = abbreviation;
            Name = name;
            Offset = offset;
        }

        /// <summary>
        /// Converts a UTC timestamp into a <see cref="DateTimeOffset"/> expressed in this zone, resolving to the best-matching system time zone.
        /// </summary>
        /// <param name="utcTimestamp">The UTC instant to convert.</param>
        public DateTimeOffset ToTimestamp(DateTime utcTimestamp)
        {
            return GetTimestamp(utcTimestamp, ToTimeZoneInfo());
        }

        /// <summary>
        /// Resolves this zone to the system <see cref="TimeZoneInfo"/> whose base UTC offset matches, breaking ties by the closest standard-name match; returns null when no match is found.
        /// </summary>
        public TimeZoneInfo ToTimeZoneInfo()
        {
            try
            {
                var offsetTimeSpan = GetOffsetTimeSpan(Offset);
                if (offsetTimeSpan.HasValue)
                {
                    var matchedTimeZones = TimeZoneInfo.GetSystemTimeZones().Where(o => o.BaseUtcOffset == offsetTimeSpan.Value);
                    if (matchedTimeZones != null)
                    {
                        return matchedTimeZones.OrderBy(o => GetSimilarity(o.StandardName, Name)).FirstOrDefault();
                    }
                }
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Converts a UTC timestamp into a <see cref="DateTimeOffset"/> in the given time zone, normalizing the kind to Unspecified so the offset comes from the zone rather than local machine settings; falls back to the raw UTC value when no zone is supplied.
        /// </summary>
        /// <param name="utcTimestamp">The UTC instant to convert.</param>
        /// <param name="timeZoneInfo">The target time zone, or null to use UTC.</param>
        public static DateTimeOffset GetTimestamp(DateTime utcTimestamp, TimeZoneInfo timeZoneInfo)
        {
            if (timeZoneInfo != null)
            {
                // Convert Timestamp to Time Zone using Offset
                var output = TimeZoneInfo.ConvertTime(utcTimestamp, timeZoneInfo);

                // Convert to Unspecified (to not use Local based on PC settings)
                output = DateTime.SpecifyKind(output, DateTimeKind.Unspecified);

                //return new DateTimeOffset(output, timeZoneInfo.BaseUtcOffset);
                return new DateTimeOffset(output, timeZoneInfo.GetUtcOffset(output));
            }

            return new DateTimeOffset(utcTimestamp);
        }


        /// <summary>
        /// Looks up a known time zone by abbreviation first, then by an equivalent UTC offset; returns null when the input is empty or matches nothing.
        /// </summary>
        /// <param name="input">A zone abbreviation or a UTC offset string.</param>
        public static MTConnectTimeZone Get(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Try Abbreviation
                var output = _timeZones.FirstOrDefault(o => o.Abbreviation.ToLower() == input.ToLower());

                // Try Offset
                if (output == null)
                {
                    output = _timeZones.FirstOrDefault(o => MatchOffset(input, o.Offset));
                }

                return output;
            }

            return null;
        }

        private static bool MatchOffset(string input, string compare)
        {
            var offsetTimeSpan = GetOffsetTimeSpan(input);
            var compareTimeSpan = GetOffsetTimeSpan(compare);

            if (offsetTimeSpan.HasValue && compareTimeSpan.HasValue)
            {
                return offsetTimeSpan == compareTimeSpan;
            }

            return false;
        }

        private static TimeSpan? GetOffsetTimeSpan(string input)
        {
            var match = Regex.Match(input, _offsetParsePattern);
            if (match.Success)
            {
                var offsetDirection = match.Groups[1].Value;
                var offsetHour = match.Groups[2].Value;
                var offsetMinute = match.Groups[3].Value;
                if (string.IsNullOrEmpty(offsetMinute)) offsetMinute = "00";
                var offsetInput = $"{offsetDirection}{offsetHour}:{offsetMinute}";
                return TimeSpan.Parse(offsetInput);
            }

            return null;
        }

        private static IEnumerable<MTConnectTimeZone> Init()
        {
            var timeZones = new List<MTConnectTimeZone>();
            timeZones.Add(new MTConnectTimeZone("Z", "Zulu Time", "UTC+0"));

            // USA
            timeZones.Add(new MTConnectTimeZone("ET", "Eastern Time", "UTC-5:00"));
            timeZones.Add(new MTConnectTimeZone("EST", "Eastern Standard Time", "UTC-5:00"));
            timeZones.Add(new MTConnectTimeZone("CT", "Central Time", "UTC-6:00"));
            timeZones.Add(new MTConnectTimeZone("CST", "Central Standard Time", "UTC-6:00"));
            timeZones.Add(new MTConnectTimeZone("MT", "Mountain Time", "UTC-7:00"));
            timeZones.Add(new MTConnectTimeZone("MST", "Mountain Standard Time", "UTC-7:00"));
            timeZones.Add(new MTConnectTimeZone("PT", "Pacific Time", "UTC-8:00"));
            timeZones.Add(new MTConnectTimeZone("PST", "Pacific Standard Time", "UTC-8:00"));
            timeZones.Add(new MTConnectTimeZone("AKST", "Alaska Standard Time", "UTC-9:00"));
            timeZones.Add(new MTConnectTimeZone("HST", "Hawaii Standard Time", "UTC-10:00"));

            // Europe
            timeZones.Add(new MTConnectTimeZone("WET", "Western European Time", "UTC+0:00"));
            timeZones.Add(new MTConnectTimeZone("CET", "Central European Time", "UTC+1:00"));
            timeZones.Add(new MTConnectTimeZone("EET", "Eastern European Time", "UTC+2:00"));

            // Australia
            timeZones.Add(new MTConnectTimeZone("AWST", "Austrailian Western Standard Time", "UTC+8:00"));
            timeZones.Add(new MTConnectTimeZone("ACT", "Austrailian Central Time", "UTC+9:30"));
            timeZones.Add(new MTConnectTimeZone("ACST", "Austrailian Central Standard Time", "UTC+9:30"));
            timeZones.Add(new MTConnectTimeZone("AET", "Austrailian Eastern Time", "UTC+10:00"));
            timeZones.Add(new MTConnectTimeZone("AEST", "Austrailian Eastern Standard Time", "UTC+10:00"));

            return timeZones;
        }

        private static int GetSimilarity(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // initialize the top and right of the table to 0, 1, 2, ...
            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }
    }
}
