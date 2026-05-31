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
    /// <summary>
    /// String casing, word-splitting, and hashing helpers used to normalize identifiers between document formats and to derive stable content hashes.
    /// The MD5/SHA1/Random instances are cached per thread to avoid repeated allocation under concurrent serialization.
    /// </summary>
    public static class StringFunctions
    {
        private static readonly Encoding _utf8 = Encoding.UTF8;

        [ThreadStatic]
        private static MD5 _md5;

		[ThreadStatic]
		private static SHA1 _sha1;

		[ThreadStatic]
        private static Random _random;

        private static MD5 MD5Algorithm
        {
            get
            {
                if (_md5 == null)
                {
                    _md5 = MD5.Create();
                }
                return _md5;
            }
        }

		private static SHA1 SHA1Algorithm
		{
			get
			{
				if (_sha1 == null)
				{
					_sha1 = SHA1.Create();
				}
				return _sha1;
			}
		}

		private static Random Random
        {
            get
            {
                if (_random == null)
                {
                    _random = new Random();
                }

                return _random;
            }
        }

        /// <summary>
        /// Splits the string into words (see <see cref="SplitOnWord"/>) and concatenates them with each word's first character uppercased and no separator, producing PascalCase; returns null when the input is null or empty.
        /// </summary>
        /// <param name="s">The string to convert.</param>
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

        /// <summary>
        /// Splits the string into words and concatenates them with each word's first character uppercased and no separator; behaves identically to <see cref="ToPascalCase"/>. Returns null when the input is null or empty.
        /// </summary>
        /// <param name="s">The string to convert.</param>
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

        /// <summary>
        /// Splits the string into words and concatenates them with the first word lowercased and every subsequent word's first character uppercased, producing camelCase; returns null when the input is null or empty.
        /// </summary>
        /// <param name="s">The string to convert.</param>
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

        /// <summary>
        /// Splits the string into its constituent words, preferring spaces, then underscores, then uppercase boundaries as the delimiter; returns a single-element array when none apply.
        /// </summary>
        /// <param name="s">The string to split.</param>
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

        /// <summary>
        /// Splits the string at each uppercase letter (except the first character) into separate words; returns the string unsplit when it is entirely uppercase, or null when null or empty.
        /// </summary>
        /// <param name="s">The string to split on uppercase boundaries.</param>
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

        /// <summary>
        /// Returns the string with its first character uppercased and the remainder lowercased; returns null for null input and the fully-uppercased single character for one-character input.
        /// </summary>
        /// <param name="s">The string to recapitalize.</param>
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

        /// <summary>
        /// Returns the string with only its first character lowercased and the remainder unchanged; returns null for null input and the fully-lowercased single character for one-character input.
        /// </summary>
        /// <param name="s">The string to recapitalize.</param>
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


        /// <summary>
        /// Converts the string to lowercase snake_case, splitting on spaces or (when <paramref name="splitOnUppercase"/> is true) on uppercase boundaries; an all-uppercase input is simply lowercased, and null is returned when no split applies or the input is null or empty.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <param name="splitOnUppercase">When true and the string has no spaces, word boundaries are inferred from uppercase letters.</param>
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

        /// <summary>
        /// Converts the string to uppercase SNAKE_CASE, splitting on spaces or (when <paramref name="splitOnUppercase"/> is true) on uppercase boundaries; an all-uppercase input is simply uppercased, and null is returned when no split applies or the input is null or empty.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <param name="splitOnUppercase">When true and the string has no spaces, word boundaries are inferred from uppercase letters.</param>
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

        /// <summary>
        /// Generates a random string of the given length drawn from uppercase letters and digits, using the per-thread cached Random (not cryptographically secure).
        /// </summary>
        /// <param name="length">The number of characters to generate.</param>
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Parses the string as a DateTime, returning DateTime.MinValue when it cannot be parsed.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        public static DateTime ToDateTime(this string s)
        {
            if (DateTime.TryParse(s, out var dateTime))
            {
                return dateTime;
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Computes the MD5 digest of the UTF-8 bytes of the string and returns it as a lowercase hex string; returns null if hashing throws.
        /// </summary>
        /// <param name="s">The string to hash.</param>
        public static string ToMD5Hash(this string s)
        {
            try
            {
                var hash = MD5Algorithm.ComputeHash(_utf8.GetBytes(s));
                return string.Concat(hash.Select(b => b.ToString("x2")));
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Computes the MD5 digest of the byte buffer and returns it as a lowercase hex string; returns null when the buffer is null or hashing throws.
        /// </summary>
        /// <param name="bytes">The buffer to hash.</param>
        public static string ToMD5Hash(this byte[] bytes)
        {
            if (bytes != null)
            {
                try
                {
                    var hash = MD5Algorithm.ComputeHash(bytes);
                    return string.Concat(hash.Select(b => b.ToString("x2")));
                }
                catch { }
            }

            return null;
        }

        /// <summary>
        /// Formats an already-computed hash digest as a lowercase hex string without re-hashing; returns null when the buffer is null or formatting throws.
        /// </summary>
        /// <param name="hashBytes">The raw digest bytes to render as hex.</param>
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

        /// <summary>
        /// Computes the raw MD5 digest of the UTF-8 bytes of the string; returns null if hashing throws.
        /// </summary>
        /// <param name="s">The string to hash.</param>
        public static byte[] ToMD5HashBytes(this string s)
        {
            try
            {
                return MD5Algorithm.ComputeHash(_utf8.GetBytes(s));
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Computes the raw MD5 digest of the byte buffer; returns null when the buffer is null or hashing throws.
        /// </summary>
        /// <param name="bytes">The buffer to hash.</param>
        public static byte[] ToMD5HashBytes(this byte[] bytes)
        {
            if (bytes != null)
            {
                try
                {
                    return MD5Algorithm.ComputeHash(bytes);
                }
                catch { }
            }
            return null;
        }

        /// <summary>
        /// Computes an order-sensitive rolling MD5 over the lines: each line is hashed, concatenated after the running hash, and re-hashed, so the result depends on both the content and sequence of lines. Returns null for a null or empty array.
        /// </summary>
        /// <param name="lines">The ordered lines to fold into a single hash.</param>
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

        /// <summary>
        /// Folds an array of digest buffers into a single MD5 digest by repeatedly concatenating the running result with the next buffer and re-hashing; a single-element array is returned unchanged, and null/empty input returns null.
        /// </summary>
        /// <param name="hashBytes">The ordered digest buffers to combine.</param>
        public static byte[] ToMD5HashBytes(byte[][] hashBytes)
        {
            if (hashBytes != null && hashBytes.Length > 0)
            {
                var x1 = hashBytes[0];
                byte[] x2;
                byte[] a1 = x1;

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

                return a1;
            }

            return null;
        }


		/// <summary>
		/// Computes the SHA-1 digest of the UTF-8 bytes of the string and returns it as a lowercase hex string; returns null if hashing throws.
		/// </summary>
		/// <param name="s">The string to hash.</param>
		public static string ToSHA1Hash(this string s)
		{
			try
			{
				var hash = SHA1Algorithm.ComputeHash(_utf8.GetBytes(s));
				return string.Concat(hash.Select(b => b.ToString("x2")));
			}
			catch { }

			return null;
		}

		/// <summary>
		/// Computes the SHA-1 digest of the byte buffer and returns it as a lowercase hex string; returns null when the buffer is null or hashing throws.
		/// </summary>
		/// <param name="bytes">The buffer to hash.</param>
		public static string ToSHA1Hash(this byte[] bytes)
		{
			if (bytes != null)
			{
				try
				{
					var hash = SHA1Algorithm.ComputeHash(bytes);
					return string.Concat(hash.Select(b => b.ToString("x2")));
				}
				catch { }
			}

			return null;
		}

		/// <summary>
		/// Formats an already-computed SHA-1 digest as a lowercase hex string without re-hashing; returns null when the buffer is null or formatting throws.
		/// </summary>
		/// <param name="hashBytes">The raw digest bytes to render as hex.</param>
		public static string ToSHA1HashString(this byte[] hashBytes)
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

		/// <summary>
		/// Computes the raw SHA-1 digest of the UTF-8 bytes of the string; returns null if hashing throws.
		/// </summary>
		/// <param name="s">The string to hash.</param>
		public static byte[] ToSHA1HashBytes(this string s)
		{
			try
			{
				return SHA1Algorithm.ComputeHash(_utf8.GetBytes(s));
			}
			catch { }

			return null;
		}

		/// <summary>
		/// Computes the raw SHA-1 digest of the byte buffer; returns null when the buffer is null or hashing throws.
		/// </summary>
		/// <param name="bytes">The buffer to hash.</param>
		public static byte[] ToSHA1HashBytes(this byte[] bytes)
		{
			if (bytes != null)
			{
				try
				{
					return SHA1Algorithm.ComputeHash(bytes);
				}
				catch { }
			}
			return null;
		}

		/// <summary>
		/// Computes an order-sensitive rolling SHA-1 over the lines: each line is hashed, concatenated after the running hash, and re-hashed, so the result depends on both content and sequence. Returns null for a null or empty array.
		/// </summary>
		/// <param name="lines">The ordered lines to fold into a single hash.</param>
		public static string ToSHA1Hash(string[] lines)
		{
			if (lines != null && lines.Length > 0)
			{
				var x1 = lines[0];
				var h = x1.ToSHA1Hash();

				for (int i = 1; i < lines.Length; i++)
				{
					x1 = lines[i].ToSHA1Hash();
					x1 = h + x1;
					h = x1.ToSHA1Hash();
				}

				return h;
			}

			return null;
		}

		/// <summary>
		/// Iterates the digest buffers, concatenating the running result with each next buffer and re-hashing it with SHA-1; for a single-element array the element is returned unchanged, and null/empty input returns null.
		/// Note: for multi-element input the value returned is the last source buffer rather than the accumulated hash.
		/// </summary>
		/// <param name="hashBytes">The ordered digest buffers to combine.</param>
		public static byte[] ToSHA1HashBytes(byte[][] hashBytes)
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

						x1 = a1.ToSHA1HashBytes();
					}
				}

				return x2;
			}

			return null;
		}


		/// <summary>
		/// Formats a byte count as a human-readable size using binary (1024) steps and one decimal place, ranging from "B" to "EB" and preserving the sign of a negative count.
		/// </summary>
		/// <param name="byteCount">The number of bytes to format.</param>
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

        /// <summary>
        /// Formats a decimal byte count as a human-readable size by truncating to a long and delegating to <see cref="ToFileSize(long)"/>.
        /// </summary>
        /// <param name="byteCount">The number of bytes to format.</param>
        public static string ToFileSize(this decimal byteCount)
        {
            var x = (long)byteCount;
            return x.ToFileSize();
        }


        /// <summary>
        /// Heuristically determines whether the string contains HTML by testing for a matched open/close tag pair; returns false for null or empty input.
        /// </summary>
        /// <param name="s">The string to inspect.</param>
        public static bool IsHtml(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var regex = new Regex(@"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");
                return regex.IsMatch(s);
            }

            return false;
        }

        /// <summary>
        /// Parses the string into the enum value of type <typeparamref name="T"/> case-insensitively, returning default(T) when <typeparamref name="T"/> is not an enum or the value does not match a member.
        /// </summary>
        /// <typeparam name="T">The target enum type.</typeparam>
        /// <param name="value">The string to parse into an enum member.</param>
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

        /// <summary>
        /// Derives a 64-bit hash from the string by MD5-hashing its default-encoded bytes and XOR-folding the digest into eight-byte words, yielding a compact non-cryptographic key.
        /// </summary>
        /// <param name="text">The string to reduce to a 64-bit hash.</param>
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