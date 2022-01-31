// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MTConnect
{
    public static class StringFunctions
    {
        private static Random random = new Random();


        public static bool ToBoolean(this string s)
        {
            if (!string.IsNullOrEmpty(s) && bool.TryParse(s, out var x)) return x;
            else return false;
        }

        public static int ToInt(this string s)
        {
            if (!string.IsNullOrEmpty(s) && int.TryParse(s, out var x)) return x;
            else return 0;
        }

        public static long ToLong(this string s)
        {
            if (!string.IsNullOrEmpty(s) && long.TryParse(s, out var x)) return x;
            else return 0;
        }

        public static double ToDouble(this string s)
        {
            if (!string.IsNullOrEmpty(s) && double.TryParse(s, out var x)) return x;
            else return 0;
        }

        public static double ToDouble(this string s, int decimalPlaces = int.MaxValue)
        {
            if (!string.IsNullOrEmpty(s) && double.TryParse(s, out var x))
            {
                return Math.Round(x, decimalPlaces);
            }
            else return 0;
        }

        public static string ToPascalCase(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var parts = s.Split('_');
                if (!parts.IsNullOrEmpty())
                {
                    var l = new List<string>();

                    foreach (var part in parts)
                    {
                        var textInfo = new CultureInfo("en-US", false).TextInfo;
                        l.Add(textInfo.ToTitleCase(part.ToLower()));
                    }

                    return string.Concat(l);
                }
            }

            return null;
        }

        public static string ToTitleCase(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var parts = s.Split('_');
                if (!parts.IsNullOrEmpty())
                {
                    var l = new List<string>();

                    foreach (var part in parts)
                    {
                        var textInfo = new CultureInfo("en-US", false).TextInfo;
                        l.Add(textInfo.ToTitleCase(part.ToLower()));
                    }

                    return string.Join(" ", l);
                }
            }

            return null;
        }

        public static string ToCamelCase(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var parts = s.Split('_');
                if (!parts.IsNullOrEmpty())
                {
                    var l = new List<string>();

                    for (var i = 0; i <= parts.Count() - 1; i++)
                    {
                        var textInfo = new CultureInfo("en-US", false).TextInfo;
                        if (i > 0) l.Add(textInfo.ToTitleCase(parts[i].ToLower()));
                        else l.Add(parts[i].ToLower());
                    }

                    return string.Concat(l);
                }
            }

            return null;
        }

        public static string ToUnderscore(this string s, bool splitOnUppercase = true)
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (s != s.ToUpper())
                {
                    string[] parts = null;

                    if (s.Contains(' '))
                    {
                        parts = s.Split(' ');
                    }
                    else if (splitOnUppercase)
                    {
                        // Split string by Uppercase characters
                        parts = Regex.Split(s, @"(?<!^)(?=[A-Z])");
                    }

                    var a = new List<string>();
                    if (!parts.IsNullOrEmpty()) foreach (var part in parts) a.Add(part.Trim());
                    if (!a.IsNullOrEmpty())
                    {
                        string x = string.Join("_", a);
                        return x.ToLower();
                    }
                }
                else return s.ToLower();
            }

            return null;
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static DateTime ToDateTime(this string s)
        {
            if (DateTime.TryParse(s, out var dateTime))
            {
                return dateTime;
            }

            return DateTime.MinValue;
        }

        public static string ToMD5Hash(this string s)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }

        public static string ToFileSize(this long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        public static string ToFileSize(this decimal byteCount)
        {
            var x = (long)byteCount;
            return x.ToFileSize();
        }


        public static bool IsHtml(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var regex = new Regex(@"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");
                return regex.IsMatch(s);
            }

            return false;
        }

        public static T ConvertEnum<T>(this string value) where T : struct
        {
            if (value != null && typeof(T).IsEnum)
            {
                if (Enum.TryParse<T>(value.ToString(), true, out var result))
                {
                    return (T)result;
                }
            }

            return default;
        }
    }
}
