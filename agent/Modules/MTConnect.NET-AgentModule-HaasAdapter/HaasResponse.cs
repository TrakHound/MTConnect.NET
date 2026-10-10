// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Events;
using System;
using System.Globalization;

namespace MTConnect.Haas
{
    /// <summary>
    /// The values parsed from a <c>?Q500</c> (program status) reply.
    /// </summary>
    public class HaasStatus
    {
        /// <summary>
        /// The Execution state, or <c>null</c> if the reply did not indicate one.
        /// </summary>
        public Execution? Execution { get; set; }

        /// <summary>
        /// The active program name (empty in MDI), or <c>null</c> if not reported (ex. while BUSY).
        /// </summary>
        public string Program { get; set; }

        /// <summary>
        /// The M30 part counter, or <c>null</c> if not reported (ex. while BUSY).
        /// </summary>
        public int? PartCount { get; set; }

        /// <summary>
        /// Whether an alarm is active, or <c>null</c> if not reported (ex. while BUSY).
        /// </summary>
        public bool? Alarm { get; set; }
    }

    /// <summary>
    /// Parses replies to Haas Q-commands. Replies may be framed with STX / ETB control
    /// characters and the <c>&gt;</c> prompt depending on the control and connection type;
    /// these are removed before parsing (ex. <c>"\u0002PROGRAM, O01234, IDLE, PARTS, 42\u0017"</c>
    /// and <c>"&gt;PROGRAM, O01234, IDLE, PARTS, 42"</c> are equivalent).
    /// </summary>
    public static class HaasResponse
    {
        private static readonly char[] _trimChars = { '\u0002', '\u0017', '>', ' ', '\t', '\r', '\n' };


        /// <summary>
        /// Removes framing characters and the prompt from a raw reply.
        /// </summary>
        /// <param name="reply">The raw reply.</param>
        /// <returns>The reply content, or <c>null</c> if the reply is empty.</returns>
        public static string Clean(string reply)
        {
            if (reply == null) return null;

            var cleaned = reply.Replace("\u0002", "").Replace("\u0017", "").Trim(_trimChars);
            return cleaned.Length > 0 ? cleaned : null;
        }

        /// <summary>
        /// Splits a reply into its comma separated fields.
        /// </summary>
        /// <param name="reply">The raw reply.</param>
        /// <returns>The trimmed fields, or an empty array if the reply is empty.</returns>
        public static string[] Split(string reply)
        {
            var cleaned = Clean(reply);
            if (cleaned == null) return new string[0];

            var fields = cleaned.Split(',');
            for (var i = 0; i < fields.Length; i++) fields[i] = fields[i].Trim();
            return fields;
        }


        /// <summary>
        /// Parses a <c>?Q104</c> reply (ex. <c>MODE, (MEM)</c>).
        /// </summary>
        /// <param name="reply">The raw reply.</param>
        /// <returns>The ControllerMode, or <c>null</c> if the reply or mode is not recognized.</returns>
        public static ControllerMode? ParseMode(string reply)
        {
            var fields = Split(reply);
            if (fields.Length < 2 || !IsKey(fields[0], "MODE")) return null;

            var mode = fields[1].Trim('(', ')', ' ').ToUpperInvariant();
            switch (mode)
            {
                case "MEM":
                case "DNC":
                    return ControllerMode.AUTOMATIC;

                case "MDI":
                    return ControllerMode.MANUAL_DATA_INPUT;

                case "JOG":
                case "HAND JOG":
                case "HANDLE JOG":
                case "ZERO RET":
                    return ControllerMode.MANUAL;

                case "EDIT":
                case "LIST PROG":
                    return ControllerMode.EDIT;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Parses a <c>?Q500</c> reply (ex. <c>PROGRAM, O01234, IDLE, PARTS, 42</c> or <c>STATUS, BUSY</c>).
        /// </summary>
        /// <param name="reply">The raw reply.</param>
        /// <returns>The parsed status, or <c>null</c> if the reply is not recognized.</returns>
        public static HaasStatus ParseStatus(string reply)
        {
            var cleaned = Clean(reply);
            if (cleaned == null) return null;

            // Reported while a program is running. Program and part count are not available in this state.
            if (cleaned.StartsWith("STATUS", StringComparison.OrdinalIgnoreCase))
            {
                var value = cleaned.Substring("STATUS".Length).Trim(',', ' ').ToUpperInvariant();
                var status = new HaasStatus();
                if (value == "BUSY") status.Execution = Execution.ACTIVE;
                return status;
            }

            var fields = Split(reply);
            if (fields.Length >= 5 && IsKey(fields[0], "PROGRAM") && IsKey(fields[3], "PARTS"))
            {
                var status = new HaasStatus();

                status.Program = fields[1] == "MDI" ? "" : fields[1];

                switch (fields[2].ToUpperInvariant())
                {
                    case "IDLE": status.Execution = Execution.READY; break;
                    case "FEED HOLD": status.Execution = Execution.INTERRUPTED; break;
                    case "ALARM ON": status.Execution = Execution.STOPPED; break;
                }

                status.Alarm = string.Equals(fields[2], "ALARM ON", StringComparison.OrdinalIgnoreCase);

                int partCount;
                if (int.TryParse(fields[4], NumberStyles.Integer, CultureInfo.InvariantCulture, out partCount))
                {
                    status.PartCount = partCount;
                }

                return status;
            }

            return null;
        }

        /// <summary>
        /// Parses a <c>?Q600</c> macro variable reply (ex. <c>MACRO, 123.4500</c>).
        /// </summary>
        /// <param name="reply">The raw reply.</param>
        /// <returns>The variable value, or <c>null</c> if the reply is not recognized.</returns>
        public static double? ParseMacro(string reply)
        {
            var fields = Split(reply);
            if (fields.Length < 2 || !IsKey(fields[0], "MACRO")) return null;

            double value;
            if (double.TryParse(fields[1], NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            {
                return value;
            }

            return null;
        }


        private static bool IsKey(string field, string key) => string.Equals(field, key, StringComparison.OrdinalIgnoreCase);
    }
}
