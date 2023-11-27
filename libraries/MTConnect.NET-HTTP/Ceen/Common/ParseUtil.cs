using System.Net.Sockets;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Ceen
{
    /// <summary>
    /// Helper methods used to parse durations and sizes in a human readable format
    /// </summary>
    public static class ParseUtil
    {
        /// <summary>
        /// Parses a duration value
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <returns>The parsed value</returns>
        public static TimeSpan ParseDuration(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Empty string is not a valid duration", nameof(value));

            if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var r))
                return TimeSpan.FromSeconds(r);

            if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var n))
                return n;

            var res = new TimeSpan(0);
            var len = 0;
            foreach (var m in new Regex("\\s*(?<number>[-|+]?[0-9]+)(?<suffix>[wdhms])\\s*", RegexOptions.IgnoreCase).Matches(value).Cast<Match>())
            {
                if (!m.Success)
                    break;
                len += m.Length;

                var number = int.Parse(m.Groups["number"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                switch (m.Groups["suffix"].Value.ToLowerInvariant()[0])
                {
                    case 'w':
                        res += TimeSpan.FromDays(number * 7);
                        break;
                    case 'd':
                        res += TimeSpan.FromDays(number);
                        break;
                    case 'h':
                        res += TimeSpan.FromHours(number);
                        break;
                    case 'm':
                        res += TimeSpan.FromMinutes(number);
                        break;
                    case 's':
                        res += TimeSpan.FromSeconds(number);
                        break;
                    default:
                        throw new ArgumentException($"Invalid suffix: \"{m.Groups["suffix"].Value}\"", value);
                }
            }

            if (len != value.Length)
                throw new ArgumentException($"String is not a valid duration: \"{value}\"", nameof(value));

            return res;
        }

        /// <summary>
        /// Parses a potential size string
        /// </summary>
        /// <param name="value">The string to parse</param>
        /// <returns>The size</returns>
        public static long ParseSize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Empty string is not a valid number", nameof(value));

            if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var r))
                return r;

            var m = new Regex("(?<number>[0-9,.]+)\\s*(?<suffix>[ptgmk]i?b)?", RegexOptions.IgnoreCase).Match(value);
            if (!m.Success || m.Length != value.Length)
                throw new ArgumentException($"String is not a valid number or size: \"{value}\"", nameof(value));

            var suffix = m.Groups["suffix"].Success ? m.Groups["suffix"].Value.ToLowerInvariant()[0] : 'b';
            var number = float.Parse(m.Groups["number"].Value, System.Globalization.CultureInfo.InvariantCulture);
            switch (suffix)
            {
                case 'p':
                    return (long)(number * Math.Pow(1024, 5));
                case 't':
                    return (long)(number * Math.Pow(1024, 4));
                case 'g':
                    return (long)(number * Math.Pow(1024, 3));
                case 'm':
                    return (long)(number * Math.Pow(1024, 2));
                case 'k':
                    return (long)(number * Math.Pow(1024, 1));
                case 'b':
                    // No suffix or 'b' must be a valid integer number
                    return long.Parse(m.Groups["number"].Value, System.Globalization.CultureInfo.InvariantCulture);
                default:
                    throw new ArgumentException($"Invalid suffix: \"{suffix}\"", nameof(value));
            }
        }

        /// <summary>
        /// Parses a boolean string
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <param name="@default">The default value to use, if the value does not match</param>
        /// <param name="requireValid">A flag indicating if an invalid value can return the <paramref name="default" /> value</param>
        /// <returns>The parsed boolean</returns>
        public static bool ParseBool(string value, bool @default = false, bool requireValid = false)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (string.Equals(value, "1", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(value, "on", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase))
                    return true;

                if (string.Equals(value, "0", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(value, "off", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(value, "false", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(value, "no", StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return requireValid ? throw new ArgumentException("Invalid boolean", nameof(value)) : @default;
        }

        /// <summary>
        /// Parse an IP Address, supporting &quot;any&quot;, &quot;*&quot;, &quot;local&quot;, and &quot;loopback&quot;
        /// </summary>
        /// <param name="address">The adress to parse.</param>
        /// <returns>The IP Address.</returns>
        public static IPAddress ParseIPAddress(string address)
        {
            var enabled = !string.IsNullOrWhiteSpace(address);
            var addr = IPAddress.Loopback;

            if (new string[] { "any", "*" }.Contains(address, StringComparer.OrdinalIgnoreCase))
                addr = IPAddress.Any;
            else if (new string[] { "local", "loopback" }.Contains(address, StringComparer.OrdinalIgnoreCase))
                addr = IPAddress.Loopback;
            else if (enabled)
                addr = IPAddress.Parse(address);

            return addr;
        }

        /// <summary>
        /// The prefix to a hostname that indicates a unix domain socket
        /// </summary>
        public const string UNIX_SOCKET_PREFIX = "unix:";
        
        /// <summary>
        /// Parse an EndPoint Address, supporting &quot;any&quot;, &quot;*&quot;, &quot;local&quot;, &quot;loopback&quot;, and &quot;unix:&quot;
        /// </summary>
        /// <param name="address">The adress to parse.</param>
        /// <param name="port">The port number to use, if not in the address.</param>
        /// <returns>The EndPoint Address.</returns>
        public static EndPoint ParseEndPoint(string address, int port)
        {

#if NET5_0_OR_GREATER
            if (!string.IsNullOrWhiteSpace(address) && address.StartsWith(UNIX_SOCKET_PREFIX, StringComparison.OrdinalIgnoreCase))
                return new UnixDomainSocketEndPoint(address.Substring(UNIX_SOCKET_PREFIX.Length));
#endif
            
            // Support for having "localhost:1234" as the listen address
            if (address.Contains(":"))
            {
                var parts = address.Split(new char[] { ':' }, 2);
                if (int.TryParse(parts[1], NumberStyles.Integer, null, out var aport) && aport > 0 && aport <= ushort.MaxValue)
                    return new IPEndPoint(ParseIPAddress(parts[0]), aport);
            }

            return new IPEndPoint(ParseIPAddress(address), port);
        }
    
    }
}
