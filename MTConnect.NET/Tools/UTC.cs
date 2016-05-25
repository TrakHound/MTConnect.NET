// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MTConnect
{
    internal static partial class Tools
    {
        public class UTC
        {

            /// <summary>
            /// Convert string to UTC DateTime (DateTime.TryParse seems to always convert to local time even with DateTimeStyle.AssumeUniveral)
            /// </summary>
            /// <param name="s"></param>
            /// <returns></returns>
            public static DateTime FromString(string s)
            {
                DateTime result = DateTime.MinValue;

                try
                {
                    string sYear = s.Substring(0, 4);
                    string sMonth = s.Substring(5, 2);
                    string sDay = s.Substring(8, 2);

                    string sHour = s.Substring(11, 2);
                    string sMinute = s.Substring(14, 2);
                    string sSecond = s.Substring(17, 2);

                    int year = Convert.ToInt16(sYear);
                    int month = Convert.ToInt16(sMonth);
                    int day = Convert.ToInt16(sDay);

                    int hour = Convert.ToInt16(sHour);
                    int minute = Convert.ToInt16(sMinute);
                    int second = Convert.ToInt16(sSecond);

                    result = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

                    // Get number of fraction characters
                    int start = 20;
                    int end = 20;
                    int n = 20;

                    if (s.Length > 20)
                    {
                        char c = s[n];

                        while (Char.IsNumber(c))
                        {
                            n += 1;
                            if (n > s.Length - 1) break;
                            else c = s[n];
                        }

                        end = n;

                        string sFraction = s.Substring(start, end - start);
                        double fraction = Convert.ToDouble("." + sFraction);
                        int millisecond = System.Math.Min(999, Convert.ToInt32(System.Math.Round(fraction, 3) * 1000));
                        result = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc);
                    }
                }
                catch (Exception ex)
                {
                    Log.Write("ConvertStringToUTC() : Input = " + s + " : Exception : " + ex.Message);
                }

                return result;
            }

            public static DateTime FromDateTime(DateTime dt)
            {
                int year = dt.Year;
                int month = dt.Month;
                int day = dt.Day;

                int hour = dt.Hour;
                int minute = dt.Minute;
                int second = dt.Second;
                int millisecond = dt.Millisecond;

                return new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc);
            }

        }
    }
}
