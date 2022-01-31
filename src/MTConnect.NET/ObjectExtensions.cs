// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MTConnect
{
    public static class ObjectExtensions
    {
        public static bool IsNumeric(this object value)
        {
            if (value != null) return double.TryParse(value.ToString(), out _);
            return false;
        }

        public static int ToInt(this object o)
        {
            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && int.TryParse(s, out var x)) return x;
            else return -1;
        }

        public static long ToLong(this object o)
        {
            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && long.TryParse(s, out var x)) return x;
            else return -1;
        }

        public static double ToDouble(this object o)
        {
            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && double.TryParse(s, out var x)) return x;
            else return -1;
        }

        public static double ToDouble(this object o, int decimalPlaces = int.MaxValue)
        {
            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && double.TryParse(s, out var x))
            {
                return Math.Round(x, decimalPlaces);
            }
            else return -1;
        }

        public static int GetDigitCount(this double num)
        {
            int digits = 0;
            while (num >= 1)
            {
                digits++;
                num /= 10;
            }
            return digits;
        }

        public static int GetDigitCountAfterDecimal(this double value)
        {
            bool start = false;
            int count = 0;
            foreach (var s in value.ToString())
            {
                if (s == '.')
                {
                    start = true;
                }
                else if (start)
                {
                    count++;
                }
            }

            return count;
        }

        public static int GetDigitBeforeDecimal(this double value, int index)
        {
            var s = value.ToString();
            var i = s.IndexOf('.');
            var x = i - index;
            if (x >= 0 && x < s.Length - 1)
            {
                return s[x].ToInt();
            }

            return 0;
        }

        public static int GetDigitAfterDecimal(this double value, int index)
        {
            var s = value.ToString();
            var i = s.IndexOf('.');
            var x = i + index;
            if (x >= 0 && x < s.Length - 1)
            {
                return s[x].ToInt();
            }

            return 0;
        }

        //public static T ConvertEnum<T>(object value)
        //{
        //    if (value != null)
        //    {
        //        if (Enum.TryParse(typeof(T), value.ToString(), true, out var result))
        //        {
        //            return (T)result;
        //        }
        //    }

        //    return default;
        //}
    }
}
