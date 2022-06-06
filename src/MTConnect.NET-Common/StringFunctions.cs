// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MTConnect
{
    public static class StringFunctions
    {
        private static readonly Random _random = new Random();

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
                var parts = s.SplitOnWord();
                if (!parts.IsNullOrEmpty())
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i <= parts.Count() - 1; i++)
                    {
                        sb.Append(parts[i].UppercaseFirstCharacter());
                    }
                    return sb.ToString();
                }
            }

            return null;
        }

        public static string ToTitleCase(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var parts = s.SplitOnWord();
                if (!parts.IsNullOrEmpty())
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i <= parts.Count() - 1; i++)
                    {
                        sb.Append(parts[i].UppercaseFirstCharacter());
                    }
                    return sb.ToString();
                }
            }

            return null;
        }

        public static string ToCamelCase(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var parts = s.SplitOnWord();
                if (!parts.IsNullOrEmpty())
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i <= parts.Count() - 1; i++)
                    {
                        if (i > 0) sb.Append(parts[i].UppercaseFirstCharacter());
                        else sb.Append(parts[i].ToLower());
                    }
                    return sb.ToString();
                }
            }

            return null;
        }

        public static string[] SplitOnWord(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string[] parts;

                if (s.Contains(' '))
                {
                    // Split string by empty space
                    parts = s.Split(' ');
                }
                else if (s.Contains('_'))
                {
                    // Split string by underscore
                    parts = s.Split('_');
                }
                else
                {
                    // Split string by Uppercase characters
                    parts = SplitOnUppercase(s);
                }

                return parts;
            }

            return new string[] { s };
        }

        public static string[] SplitOnUppercase(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (s != s.ToUpper())
                {
                    var p = "";
                    var x = 0;
                    for (var i = 0; i < s.Length; i++)
                    {
                        if (i > 0 && char.IsUpper(s[i]))
                        {
                            p += s.Substring(x, i - x) + " ";
                            x = i;
                        }

                        if (i == s.Length - 1)
                        {
                            p += s.Substring(x);
                        }
                    }
                    return p.Split(' ');
                }
                else return new string[] { s };
            }

            return null;
        }

        public static string UppercaseFirstCharacter(this string s)
        {
            if (s == null) return null;

            if (s.Length > 1)
            {
                var sb = new StringBuilder(s.Length);
                for (var i = 0; i <= s.Length - 1; i++)
                {
                    if (i == 0) sb.Append(char.ToUpper(s[i]));
                    else sb.Append(char.ToLower(s[i]));
                }
                return sb.ToString();
            }

            return s.ToUpper();
        }

        public static string LowercaseFirstCharacter(this string s)
        {
            if (s == null) return null;

            if (s.Length > 1)
            {
                var sb = new StringBuilder(s.Length);
                for (var i = 0; i <= s.Length - 1; i++)
                {
                    if (i == 0) sb.Append(char.ToLower(s[i]));
                    else sb.Append(s[i]);
                }
                return sb.ToString();
            }

            return s.ToUpper();
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

        public static string ToUnderscoreUpper(this string s, bool splitOnUppercase = true)
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
                        return x.ToUpper();
                    }
                }
                else return s.ToUpper();
            }

            return null;
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
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

        public static string ToMD5Hash(string[] lines)
        {
            if (lines != null && lines.Length > 0)
            {
                var x1 = lines[0];
                var h = x1.ToMD5Hash();

                for (int i = 1; i < lines.Length; i++)
                {
                    x1 = lines[i].ToMD5Hash();
                    x1 = h + x1;
                    h = x1.ToMD5Hash();
                }

                return h;
            }

            return null;
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
