// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MTConnect
{
    /// <summary>
    /// Loosely-typed conversion and byte-buffer helpers used when coercing observation values and message payloads whose runtime type is not known in advance.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Returns true when the value's string form parses as a double, treating null as non-numeric.
        /// </summary>
        /// <param name="value">The value to test.</param>
        public static bool IsNumeric(this object value)
        {
            if (value != null) return double.TryParse(value.ToString(), out _);
            return false;
        }

        /// <summary>
        /// Returns true when the integer is odd.
        /// </summary>
        /// <param name="value">The integer to test.</param>
        public static bool IsOdd(this int value)
        {
            return value % 2 != 0;
        }

        /// <summary>
        /// Coerces the value to a bool, passing through an actual bool and otherwise parsing its string form; returns false when null, empty, or unparseable.
        /// </summary>
        /// <param name="o">The value to convert.</param>
        public static bool ToBoolean(this object o)
        {
            if (o is bool) return (bool)o;

            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && bool.TryParse(s, out var x)) return x;
            else return false;
        }

        /// <summary>
        /// Coerces the value to an int, passing through an actual int and otherwise parsing its string form; returns -1 when null, empty, or unparseable.
        /// </summary>
        /// <param name="o">The value to convert.</param>
        public static int ToInt(this object o)
        {
            if (o is int) return (int)o;

            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && int.TryParse(s, out var x)) return x;
            else return -1;
        }

        /// <summary>
        /// Coerces the value to a uint, passing through an actual uint and otherwise parsing its string form; returns 0 when null, empty, or unparseable.
        /// </summary>
        /// <param name="o">The value to convert.</param>
        public static uint ToUInt(this object o)
        {
            if (o is uint) return (uint)o;

            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && uint.TryParse(s, out var x)) return x;
            else return 0;
        }

        /// <summary>
        /// Coerces the value to a long, passing through an actual long and otherwise parsing its string form; returns -1 when null, empty, or unparseable.
        /// </summary>
        /// <param name="o">The value to convert.</param>
        public static long ToLong(this object o)
        {
            if (o is long) return (long)o;

            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && long.TryParse(s, out var x)) return x;
            else return -1;
        }

        /// <summary>
        /// Coerces the value to a ulong, passing through an actual ulong and otherwise parsing its string form; returns 0 when null, empty, or unparseable.
        /// </summary>
        /// <param name="o">The value to convert.</param>
        public static ulong ToULong(this object o)
        {
            if (o is ulong) return (ulong)o;

            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && ulong.TryParse(s, out var x)) return x;
            else return 0;
        }

        /// <summary>
        /// Coerces the value to a double, passing through an actual double and otherwise parsing its string form; returns -1 when null, empty, or unparseable.
        /// </summary>
        /// <param name="o">The value to convert.</param>
        public static double ToDouble(this object o)
        {
            if (o is double) return (double)o;

            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && double.TryParse(s, out var x)) return x;
            else return -1;
        }

        /// <summary>
        /// Parses the value's string form to a double rounded to the given number of decimal places; returns -1 when null, empty, or unparseable.
        /// </summary>
        /// <param name="o">The value to convert.</param>
        /// <param name="decimalPlaces">The number of fractional digits to round the result to.</param>
        public static double ToDouble(this object o, int decimalPlaces = int.MaxValue)
        {
            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && double.TryParse(s, out var x))
            {
                return Math.Round(x, decimalPlaces);
            }
            else return -1;
        }

        /// <summary>
        /// Counts the digits in the integer part of the number by repeated division by ten (fractional digits are ignored; values below 1 yield 0).
        /// </summary>
        /// <param name="num">The number whose integer-part digit count is wanted.</param>
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

        /// <summary>
        /// Counts the digits after the decimal point in the number's default string representation; returns 0 when there is no decimal point.
        /// </summary>
        /// <param name="value">The number whose fractional digit count is wanted.</param>
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

        /// <summary>
        /// Returns the digit positioned <paramref name="index"/> characters to the left of the decimal point in the number's string form; returns 0 when that position falls outside the string.
        /// </summary>
        /// <param name="value">The number to inspect.</param>
        /// <param name="index">The offset to the left of the decimal point (1 selects the units digit).</param>
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

        /// <summary>
        /// Returns the digit positioned <paramref name="index"/> characters to the right of the decimal point in the number's string form; returns 0 when that position falls outside the string.
        /// </summary>
        /// <param name="value">The number to inspect.</param>
        /// <param name="index">The offset to the right of the decimal point (1 selects the first fractional digit).</param>
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

        /// <summary>
        /// Builds a per-property "name:value" string array over the object's public scalar properties (value types, string, and DateTime), used as the basis for content hashing.
        /// The "Hash" property and any names in <paramref name="ignorePropertyNames"/> are excluded; returns null when the object is null or has no properties.
        /// </summary>
        /// <param name="obj">The object whose properties contribute to the hash.</param>
        /// <param name="ignorePropertyNames">Property names to exclude so they do not affect the hash.</param>
        public static string[] GetHashPropertyList(object obj, IEnumerable<string> ignorePropertyNames = null)
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

                        if (ignorePropertyNames == null || !ignorePropertyNames.Contains(name))
                        {
                            if (name != "Hash" && properties[i].MemberType == MemberTypes.Property)
                            {
                                var type = properties[i].PropertyType;
                                if (type.IsValueType || type == typeof(string) || type == typeof(DateTime))
                                {
                                    var value = properties[i].GetValue(obj);

                                    items[i] = $"{name}:{value}";
                                }
                            }
                        }
                    }

                    return items;
                }
            }

            return null;
        }

        /// <summary>
        /// Joins the object's hashable "name:value" pairs (see <see cref="GetHashPropertyList"/>) into a single pipe-delimited string suitable for hashing.
        /// </summary>
        /// <param name="obj">The object whose properties contribute to the hash.</param>
        /// <param name="ignorePropertyNames">Property names to exclude so they do not affect the hash.</param>
        public static string GetHashPropertyString(object obj, IEnumerable<string> ignorePropertyNames = null)
        {
            var s = "";

            var items = GetHashPropertyList(obj, ignorePropertyNames);
            if (!items.IsNullOrEmpty())
            {
                foreach (var item in items)
                {
                    if (item != null) s += item + "|";
                }
            }

            return s.TrimEnd('|');
        }

        /// <summary>
        /// Performs an element-by-element equality comparison of two byte arrays, treating reference-equal (including both null) as equal and any null/length mismatch as unequal.
        /// </summary>
        /// <param name="b1">The first byte array.</param>
        /// <param name="b2">The second byte array.</param>
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

        /// <summary>
        /// Returns a copy of <paramref name="inputBytes"/> with any leading and trailing bytes that appear in <paramref name="trimBytes"/> removed; returns null when either argument is null or empty.
        /// </summary>
        /// <param name="inputBytes">The buffer to trim.</param>
        /// <param name="trimBytes">The set of byte values to strip from both ends.</param>
        public static byte[] TrimBytes(byte[] inputBytes, byte[] trimBytes)
        {
            if (inputBytes != null && inputBytes.Length > 0 && trimBytes != null && trimBytes.Length > 0)
            {
                var bytes = new byte[inputBytes.Length];
                inputBytes.CopyTo(bytes, 0);

                bytes = TrimStartBytes(bytes, trimBytes);

                bytes = TrimEndBytes(bytes, trimBytes);

                return bytes;
            }

            return null;
        }

        /// <summary>
        /// Shifts <paramref name="inputBytes"/> in place to drop any leading bytes that appear in <paramref name="trimBytes"/> and returns the number of bytes removed.
        /// The array length is left unchanged; only the returned count delimits the meaningful prefix.
        /// </summary>
        /// <param name="inputBytes">The buffer to trim in place.</param>
        /// <param name="trimBytes">The set of byte values to strip from the start.</param>
        public static int TrimStartBytes(ref byte[] inputBytes, byte[] trimBytes)
        {
            if (inputBytes != null && inputBytes.Length > 0 && trimBytes != null && trimBytes.Length > 0)
            {
                // Look for Trim bytes
                int i = 0;
                bool found;

                while (i < inputBytes.Length)
                {
                    found = false;
                    for (var k = 0; k < trimBytes.Length; k++)
                    {
                        if (inputBytes[i] == trimBytes[k])
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found) break;
                    i++;
                }

                if (i > 0)
                {
                    // Shift Array over past the initial Whitespace bytes
                    Array.Copy(inputBytes, i, inputBytes, 0, inputBytes.Length - i);
                }

                return i; // Return number of bytes shifted in array
            }

            return 0;
        }

        /// <summary>
        /// Returns a new, length-adjusted copy of <paramref name="inputBytes"/> with leading bytes that appear in <paramref name="trimBytes"/> removed; returns null when either argument is null or empty.
        /// </summary>
        /// <param name="inputBytes">The buffer to trim.</param>
        /// <param name="trimBytes">The set of byte values to strip from the start.</param>
        public static byte[] TrimStartBytes(byte[] inputBytes, byte[] trimBytes)
        {
            if (inputBytes != null && inputBytes.Length > 0 && trimBytes != null && trimBytes.Length > 0)
            {
                var bytes = new byte[inputBytes.Length];
                inputBytes.CopyTo(bytes, 0);

                // Look for Trim bytes
                var i = 0;
                var j = 0;
                while (i < bytes.Length)
                {
                    var found = false;
                    for (var k = 0; k < trimBytes.Length; k++)
                    {
                        if (bytes[i] == trimBytes[k]) found = true;
                    }
                    if (!found) break;
                    i++;
                    j++;
                }

                if (j > 0)
                {
                    // Shift Array over past the initial Whitespace bytes
                    var s = bytes.Length - j;
                    Array.Copy(bytes, j, bytes, 0, bytes.Length - j);
                    Array.Resize(ref bytes, s);
                }

                return bytes;
            }

            return null;
        }

        /// <summary>
        /// Returns a new, length-adjusted copy of <paramref name="inputBytes"/> with trailing bytes that appear in <paramref name="trimBytes"/> removed; returns null when either argument is null or empty.
        /// </summary>
        /// <param name="inputBytes">The buffer to trim.</param>
        /// <param name="trimBytes">The set of byte values to strip from the end.</param>
        public static byte[] TrimEndBytes(byte[] inputBytes, byte[] trimBytes)
        {
            if (inputBytes != null && inputBytes.Length > 0 && trimBytes != null && trimBytes.Length > 0)
            {
                var bytes = new byte[inputBytes.Length];
                inputBytes.CopyTo(bytes, 0);

                // Look for trailing Trim bytes
                var i = bytes.Length - 1;
                var j = 0;
                while (i > 0)
                {
                    var found = false;
                    for (var k = 0; k < trimBytes.Length; k++)
                    {
                        if (bytes[i] == trimBytes[k]) found = true;
                    }
                    if (!found) break;
                    i--;
                    j++;
                }

                if (j > 0)
                {
                    // Shift Array over past the trailing Whitespace bytes
                    Array.Resize(ref bytes, bytes.Length - j);
                }

                return bytes;
            }

            return null;
        }
    }
}