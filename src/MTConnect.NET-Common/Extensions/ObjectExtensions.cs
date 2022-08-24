// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Reflection;

namespace MTConnect
{
    public static class ObjectExtensions
    {
        public static bool IsNumeric(this object value)
        {
            if (value != null) return double.TryParse(value.ToString(), out _);
            return false;
        }

        public static bool IsOdd(this int value)
        {
            return value % 2 != 0;
        }

        public static bool ToBoolean(this object o)
        {
            if (o is bool) return (bool)o;

            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && bool.TryParse(s, out var x)) return x;
            else return false;
        }

        public static int ToInt(this object o)
        {
            if (o is int) return (int)o;

            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && int.TryParse(s, out var x)) return x;
            else return -1;
        }

        public static long ToLong(this object o)
        {
            if (o is long) return (long)o;

            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && long.TryParse(s, out var x)) return x;
            else return -1;
        }

        public static double ToDouble(this object o)
        {
            if (o is double) return (double)o;

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

        public static string[] GetChangeIdPropertyList(object obj)
        {
            if (obj != null)
            {
                var properties = obj.GetType().GetProperties();
                if (properties != null && properties.Length > 0)
                {
                    var items = new string[properties.Length];

                    for (int i = 0; i < properties.Length; i++)
                    {
                        var name = properties[i].Name;

                        if (name != "ChangeId" && properties[i].MemberType == MemberTypes.Property)
                        {
                            var type = properties[i].PropertyType;
                            if (type.IsValueType || type == typeof(string) || type == typeof(DateTime))
                            {
                                var value = properties[i].GetValue(obj);

                                items[i] = $"{name}:{value}";
                            }
                        }
                    }

                    return items;
                }
            }

            return null;
        }

        public static string GetChangeIdPropertyString(object obj)
        {
            var s = "";

            var items = GetChangeIdPropertyList(obj);
            if (!items.IsNullOrEmpty())
            {
                foreach (var item in items)
                {
                    if (item != null) s += item + "|";
                }
            }

            return s.TrimEnd('|');
        }

        public static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }
    }
}
