// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MTConnect.Shdr
{
    /// <summary>
    /// Tools for analyzing and extracting data from an SHDR text line
    /// </summary>
    public static class ShdrLine
    {
        /// <summary>The SHDR field separator (pipe) used between fields within an SHDR line.</summary>
        public const char PipeDelimiter = '|';

        /// <summary>The SHDR line terminator (CR LF) appended to every line written to the adapter socket.</summary>
        public const string LineTerminator = "\r\n";

        private const char Quote = '"';

        private static readonly Regex _timestampRegex = new Regex(@"(.*)\@([0-9\.]+)", RegexOptions.Compiled);

        // Reset triggers: ":DAY 100" (prefix, as data sets and messages are written) or "100:DAY" (suffix, as values are written).
        // Only a known ResetTriggered name counts, so text such as "(MSG:CHECK TOOL)" is left alone.
        private static readonly Regex _resetPrefixRegex = new Regex(@"^:([A-Z_]+)(?:\s+(.*))?$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex _resetSuffixRegex = new Regex(@"^(.*):([A-Z_]+)$", RegexOptions.Compiled | RegexOptions.Singleline);


        /// <summary>
        /// Determine if the SHDR Line represents an MTConnect Asset
        /// </summary>
        /// <param name="line">SHDR Line</param>
        /// <returns>Returns 'true' if the line represents an MTConnect Asset</returns>
        internal static bool IsAsset(string line)
        {
            return !string.IsNullOrEmpty(line) && line.StartsWith("@ASSET@");
        }


        #region "Timestamps"

        internal static DateTime? GetTimestamp(string s)
        {
            if (string.IsNullOrEmpty(s)) return null;

            string x = s;

            // Check for duration (faster than running Regex every time)
            if (x.Contains("@"))
            {
                // Expected Format
                // Without Duration : 2014-09-29T23:59:33.460470Z
                // With Duration : 2014-09-29T23:59:33.460470Z@100.0
                var match = _timestampRegex.Match(s);
                if (match.Success && match.Groups != null && match.Groups.Count > 2)
                {
                    // First Group contains Timestamp
                    x = match.Groups[1].Value;
                }
            }

            // 16 is the minimum number of chars in a ISO 8601 Timestamp (ex. 1900-01-01T00:00)
            if (x.Length > 16 && x[4] == '-' && x[7] == '-')
            {
                if (DateTime.TryParse(x, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var y))
                {
                    return y;
                }
            }

            return null;
        }

        internal static double? GetDuration(string s)
        {
            if (s != null && s.Contains("@"))
            {
                // Expected Format
                // Without Duration : 2014-09-29T23:59:33.460470Z
                // With Duration : 2014-09-29T23:59:33.460470Z@100.0
                var match = _timestampRegex.Match(s);
                if (match.Success && match.Groups != null && match.Groups.Count > 2)
                {
                    // Second group contains Duration
                    var x = match.Groups[2].Value;

                    if (double.TryParse(x, NumberStyles.Float, CultureInfo.InvariantCulture, out var y))
                    {
                        return y;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Reads the timestamp field of a line ("2014-09-29T23:59:33.460470Z", optionally followed by "@duration", or empty).
        /// <paramref name="timestamp"/> is Unix ticks (UTC), or 0 when the field is empty or not a timestamp (the Agent then uses its own time).
        /// </summary>
        internal static void ParseTimestampField(string field, out long timestamp, out double duration)
        {
            var parsed = GetTimestamp(field);
            timestamp = parsed.HasValue ? parsed.Value.ToUnixUtcTime() : 0;

            var parsedDuration = GetDuration(field);
            duration = parsedDuration.HasValue ? parsedDuration.Value : 0;
        }

        /// <summary>
        /// Splits a line into its timestamp field and the remainder. Lines that start with '@' (a designator such as
        /// @ASSET@) or have no field separator have no timestamp field; otherwise the first field is the timestamp,
        /// which may be empty.
        /// </summary>
        internal static string GetBody(string input, out string timestampField)
        {
            timestampField = null;
            if (string.IsNullOrEmpty(input) || input[0] == '@') return input;

            var i = input.IndexOf(PipeDelimiter);
            if (i < 0) return input;

            timestampField = input.Substring(0, i);
            return input.Substring(i + 1);
        }

        /// <summary>Returns true when the line (after any timestamp field) starts with <paramref name="designator"/>.</summary>
        internal static bool HasDesignator(string input, string designator)
        {
            if (string.IsNullOrEmpty(input)) return false;

            // Compared in place: this runs for every line received, so avoid allocating the remainder
            var start = 0;
            if (input[0] != '@')
            {
                var i = input.IndexOf(PipeDelimiter);
                if (i < 0) return false;
                start = i + 1;
            }

            return input.Length - start >= designator.Length && string.CompareOrdinal(input, start, designator, 0, designator.Length) == 0;
        }

        /// <summary>
        /// Splits a line into its fields in one pass (quoted values as described for <see cref="GetNextValue"/>).
        /// A trailing '|' produces a final empty field.
        /// </summary>
        internal static List<string> SplitFields(string s)
        {
            var fields = new List<string>();
            if (s == null) return fields;

            var start = 0;
            while (true)
            {
                var end = FindFieldEnd(s, start, out var quoted);
                if (end < 0) end = s.Length;

                var field = s.Substring(start, end - start);
                fields.Add(quoted ? Unquote(field) : field);

                if (end >= s.Length) break;
                start = end + 1; // A trailing '|' yields a final empty field
            }

            return fields;
        }

        /// <summary>Formats a duration for the "@duration" timestamp suffix (always with '.' decimals).</summary>
        internal static string FormatDuration(double duration)
        {
            return duration.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region "Fields"

        internal static string GetNextValue(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var i = FindFieldEnd(s, out var quoted);
                var field = i >= 0 ? s.Substring(0, i) : s;
                return quoted ? Unquote(field) : field;
            }

            return null;
        }

        internal static string GetNextSegment(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var i = FindFieldEnd(s, out _);
                if (i >= 0 && i < s.Length - 1)
                {
                    return s.Substring(i + 1);
                }
            }

            return null;
        }

        // Index of the '|' that ends the first field of s, or -1 when it is the last field.
        // Quoting follows the MTConnect reference agent (cppagent ShdrTokenizer): a field that starts with '"' and whose
        // closing quote is followed (after optional spaces) by '|' or the end of the line is a quoted value. Inside it a
        // backslash escapes the next character, so "a\|b" is the value a|b; an unescaped '|' still ends the field.
        // Anything else (e.g. "Tool broken" detected) is read as plain text up to the next '|'.
        private static int FindFieldEnd(string s, out bool quoted)
        {
            return FindFieldEnd(s, 0, out quoted);
        }

        // As above, for the field starting at index start; the result is an index into s
        private static int FindFieldEnd(string s, int start, out bool quoted)
        {
            quoted = false;

            if (s.Length - start > 1 && s[start] == Quote)
            {
                for (var i = start + 1; i < s.Length; i++)
                {
                    var c = s[i];
                    if (c == '\\' && i + 1 < s.Length)
                    {
                        i++; // Escaped character
                        continue;
                    }

                    if (c == PipeDelimiter) break; // Not a complete quoted field

                    if (c == Quote)
                    {
                        var next = i + 1;
                        while (next < s.Length && s[next] == ' ') next++;

                        if (next >= s.Length)
                        {
                            quoted = true;
                            return -1;
                        }

                        if (s[next] == PipeDelimiter)
                        {
                            quoted = true;
                            return next;
                        }

                        break; // Not a quoted field
                    }
                }
            }

            return s.IndexOf(PipeDelimiter, start);
        }

        private static string Unquote(string field)
        {
            var end = field.LastIndexOf(Quote);
            var sb = new StringBuilder(field.Length);
            for (var i = 1; i < end; i++)
            {
                var c = field[i];
                if (c == '\\' && i + 1 < end)
                {
                    i++;
                    c = field[i];
                }
                sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Formats a field value for an SHDR line: a value containing '|' (or starting with '"') is written as a quoted
        /// value with '\', '"' and '|' backslash-escaped ("Program Author \| Patrick"), the form the MTConnect reference
        /// agent reads back as a single value.
        /// </summary>
        internal static string QuoteValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return value ?? string.Empty;

            if (value.IndexOf(PipeDelimiter) >= 0 || value[0] == Quote)
            {
                return Quote + value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("|", "\\|") + Quote;
            }

            return value;
        }

        #endregion

        #region "Reset Triggered"

        /// <summary>
        /// Removes a reset trigger from <paramref name="value"/> (":DAY 100", or "100:DAY" when <paramref name="allowSuffix"/>)
        /// and returns the remaining value.
        /// </summary>
        internal static string ParseResetTriggered(string value, out ResetTriggered resetTriggered, bool allowSuffix = true)
        {
            resetTriggered = ResetTriggered.NOT_SPECIFIED;
            if (string.IsNullOrEmpty(value)) return value;

            var match = _resetPrefixRegex.Match(value);
            if (match.Success && TryParseResetTriggered(match.Groups[1].Value, out resetTriggered))
            {
                return match.Groups[2].Success ? match.Groups[2].Value : string.Empty;
            }

            match = allowSuffix ? _resetSuffixRegex.Match(value) : Match.Empty;
            if (match.Success && TryParseResetTriggered(match.Groups[2].Value, out resetTriggered))
            {
                return match.Groups[1].Value;
            }

            resetTriggered = ResetTriggered.NOT_SPECIFIED;
            return value;
        }

        private static bool TryParseResetTriggered(string s, out ResetTriggered resetTriggered)
        {
            if (Enum.IsDefined(typeof(ResetTriggered), s) && Enum.TryParse(s, false, out resetTriggered) && resetTriggered != ResetTriggered.NOT_SPECIFIED)
            {
                return true;
            }

            resetTriggered = ResetTriggered.NOT_SPECIFIED;
            return false;
        }

        #endregion

        #region "Entries"

        /// <summary>
        /// Reads space separated DataSet / Table entries: "key=value", "key='a b'", "key=\"a b\"", "key={...}" (kept
        /// with its braces, which may nest and contain quoted text), and "key" or "key=" for a removed entry.
        /// Quoted values are returned without their quotes, with \' \" and \\ unescaped.
        /// </summary>
        internal static IEnumerable<ShdrEntry> GetEntries(string s)
        {
            var entries = new List<ShdrEntry>();
            if (string.IsNullOrEmpty(s)) return entries;

            var i = 0;
            var n = s.Length;

            while (i < n)
            {
                while (i < n && char.IsWhiteSpace(s[i])) i++;
                if (i >= n) break;

                var keyStart = i;
                while (i < n && s[i] != '=' && !char.IsWhiteSpace(s[i])) i++;
                var key = s.Substring(keyStart, i - keyStart);

                if (i >= n || s[i] != '=')
                {
                    // "key" on its own
                    entries.Add(new ShdrEntry(key, null, true));
                    continue;
                }

                i++; // '='

                if (i >= n || char.IsWhiteSpace(s[i]))
                {
                    // "key="
                    entries.Add(new ShdrEntry(key, null, true));
                    continue;
                }

                string value;
                var c = s[i];
                if (c == '"' || c == '\'')
                {
                    value = ReadQuoted(s, ref i);
                }
                else if (c == '{')
                {
                    value = ReadBraces(s, ref i);
                }
                else
                {
                    var valueStart = i;
                    while (i < n && !char.IsWhiteSpace(s[i])) i++;
                    value = s.Substring(valueStart, i - valueStart);
                }

                if (key.Length > 0) entries.Add(new ShdrEntry(key, value, false));
            }

            return entries;
        }

        private static string ReadQuoted(string s, ref int i)
        {
            var quote = s[i];
            i++;

            var sb = new StringBuilder();
            while (i < s.Length)
            {
                var c = s[i];
                if (c == '\\' && i + 1 < s.Length)
                {
                    sb.Append(s[i + 1]);
                    i += 2;
                    continue;
                }

                i++;
                if (c == quote) break;
                sb.Append(c);
            }

            return sb.ToString();
        }

        private static string ReadBraces(string s, ref int i)
        {
            var start = i;
            var depth = 0;

            while (i < s.Length)
            {
                var c = s[i];
                if (c == '"' || c == '\'')
                {
                    ReadQuoted(s, ref i);
                    continue;
                }

                i++;
                if (c == '{') depth++;
                else if (c == '}' && --depth == 0) break;
            }

            return s.Substring(start, i - start);
        }

        /// <summary>
        /// Formats a DataSet / Table cell value so <see cref="GetEntries"/> reads it back unchanged: values that are
        /// empty or contain whitespace, quotes or braces are double quoted (with '"' and '\' escaped).
        /// </summary>
        internal static string FormatEntryValue(string value)
        {
            if (value == null) value = string.Empty;

            var needsQuotes = value.Length == 0;
            foreach (var c in value)
            {
                if (char.IsWhiteSpace(c) || c == '"' || c == '\'' || c == '{' || c == '}')
                {
                    needsQuotes = true;
                    break;
                }
            }

            if (!needsQuotes) return value;

            return Quote + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + Quote;
        }

        #endregion
    }
}
