// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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
        private static readonly Encoding _utf8 = Encoding.UTF8;
        private static MD5 _md5 = MD5.Create();


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
                var l = s.ToLower().ToCharArray();
                var a = new char[l.Length];

                a[0] = char.ToUpper(l[0]);
                Array.Copy(l, 1, a, 1, a.Length - 1);

                return new string(a);
            }

            return s.ToUpper();
        }

        public static string LowercaseFirstCharacter(this string s)
        {
            if (s == null) return null;

            if (s.Length > 1)
            {
                var l = s.ToCharArray();
                var a = new char[l.Length];

                a[0] = char.ToLower(l[0]);
                Array.Copy(l, 1, a, 1, a.Length - 1);

                return new string(a);
            }

            return s.ToLower();
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
            try
            {
                if (_md5 == null) _md5 = MD5.Create();
                var hash = _md5.ComputeHash(_utf8.GetBytes(s));
                return string.Concat(hash.Select(b => b.ToString("x2")));
            }
            catch { }

            return null;
        }

        public static string ToMD5Hash(this byte[] bytes)
        {
            if (bytes != null)
            {
                try
                {
                    var hash = _md5.ComputeHash(bytes);
                    return string.Concat(hash.Select(b => b.ToString("x2")));
                }
                catch { }
            }

            return null;
        }

        public static string ToMD5HashString(this byte[] hashBytes)
        {
            if (hashBytes != null)
            {
                try
                {
                    return string.Concat(hashBytes.Select(b => b.ToString("x2")));
                }
                catch { }
            }

            return null;
        }

        public static byte[] ToMD5HashBytes(this string s)
        {
            try
            {
                return _md5.ComputeHash(_utf8.GetBytes(s));
            }
            catch { }

            return null;
        }

        public static byte[] ToMD5HashBytes(this byte[] bytes)
        {
            if (bytes != null)
            {
                try
                {
                    return _md5.ComputeHash(bytes);
                }
                catch { }
            }
            return null;
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

        public static byte[] ToMD5HashBytes(byte[][] hashBytes)
        {
            if (hashBytes != null && hashBytes.Length > 0)
            {
                var x1 = hashBytes[0];
                var x2 = x1;
                byte[] a1;

                for (int i = 1; i < hashBytes.Length; i++)
                {
                    x2 = hashBytes[i];
                    if (x2 != null)
                    {
                        a1 = new byte[x1.Length + x2.Length];
                        Array.Copy(x1, 0, a1, 0, x1.Length);
                        Array.Copy(x2, 0, a1, x1.Length, x2.Length);

                        x1 = a1.ToMD5HashBytes();
                    }
                }

                return x2;
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

        public static ulong GetUInt64Hash(this string text)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(Encoding.Default.GetBytes(text));
                Array.Resize(ref bytes, bytes.Length + bytes.Length % 8); //make multiple of 8 if hash is not, for exampel SHA1 creates 20 bytes. 
                return Enumerable.Range(0, bytes.Length / 8) // create a counter for de number of 8 bytes in the bytearray
                    .Select(i => BitConverter.ToUInt64(bytes, i * 8)) // combine 8 bytes at a time into a integer
                    .Aggregate((x, y) => x ^ y); //xor the bytes together so you end up with a ulong (64-bit int)
            }
        }
    }
}