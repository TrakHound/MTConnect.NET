// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using System;
using System.Text.RegularExpressions;

namespace MTConnect.Shdr
{
    /// <summary>
    /// An Device representing an MTConnect Device to be sent using the SHDR protocol
    /// </summary>
    public class ShdrDevice
    {
        public const string DeviceDesignator = "@DEVICE@";
        public const string DeviceRemoveDesignator = "@REMOVE_DEVICE@";
        public const string DeviceRemoveAllDesignator = "@REMOVE_ALL_DEVICE@";

        private static string DeviceUuidPattern = $"{DeviceDesignator}\\|(.*)\\|.*\\|--multiline--";
        private static string DeviceTypePattern = $"{DeviceDesignator}\\|.*\\|(.*)\\|--multiline--";
        private static string DeviceMutlilineBeginPattern = $"{DeviceDesignator}.*--multiline--(.*)";
        private static string DeviceMutlilineEndPattern = "--multiline--(.*)";
        private static string DeviceRemovePattern = $"{DeviceRemoveDesignator}\\|(.*)";
        private static string DeviceRemoveAllPattern = $"{DeviceRemoveAllDesignator}\\|(.*)";

        private static readonly Regex _deviceUuidRegex = new Regex(DeviceUuidPattern);
        private static readonly Regex _deviceTypeRegex = new Regex(DeviceTypePattern);
        private static readonly Regex _multilineBeginRegex = new Regex(DeviceMutlilineBeginPattern);
        private static readonly Regex _multilineEndRegex = new Regex(DeviceMutlilineEndPattern);
        private static readonly Regex _deviceRemoveRegex = new Regex(DeviceRemovePattern);
        private static readonly Regex _deviceRemoveAllRegex = new Regex(DeviceRemoveAllPattern);

        /// <summary>
        /// Flag to set whether the Device has been sent by the adapter or not
        /// </summary>
        internal bool IsSent { get; set; }

        /// <summary>
        /// The unique idenifier of the Device
        /// </summary>
        public string DeviceUuid { get; set; }

        /// <summary>
        /// The Device object that represents the MTConnect Device to output in the SHDR protocol
        /// </summary>
        public IDevice Device { get; }

        /// <summary>
        /// The XML representation of the MTConnect Device
        /// </summary>
        public string Xml { get; }

        /// <summary>
        /// A MD5 Hash of the Device that can be used for comparison
        /// </summary>
        public string ChangeId
        {
            get
            {
                // Normalize Timestamp in order to compare
                return ToString(new ShdrDevice(DeviceUuid, Xml));
            }
        }


        public ShdrDevice() { }

        public ShdrDevice(string deviceUuid, string xml)
        {
            DeviceUuid = deviceUuid;
            Device = XmlDevice.FromXml(xml);
            Xml = xml;
        }

        public ShdrDevice(IDevice device)
        {
            if (device != null)
            {
                DeviceUuid = device.Uuid;
                Device = device;
                //Xml = XmlDevice.ToXml(device);
            }
        }


        public override string ToString() => ToString(true);

        public string ToString(bool multiline = false)
        {
            if (!string.IsNullOrEmpty(DeviceUuid) && !string.IsNullOrEmpty(Xml))
            {
                if (multiline)
                {
                    var multilineId = StringFunctions.RandomString(10);

                    var header = $"{DeviceDesignator}|{DeviceUuid}|--multiline--{multilineId}";

                    var xml = XmlFunctions.FormatXml(Xml, true, false, true);

                    var result = header;
                    result += "\n";
                    result += xml;
                    result += $"\n--multiline--{multilineId}\n";

                    return result;
                }
                else
                {
                    return $"{DeviceDesignator}|{DeviceUuid}|{Xml}";
                }
            }

            return null;
        }

        private static string ToString(ShdrDevice device, bool ignoreTimestamp = false)
        {
            if (device != null && !string.IsNullOrEmpty(device.DeviceUuid) && !string.IsNullOrEmpty(device.Xml))
            {
                return $"{DeviceDesignator}|{device.DeviceUuid}|{device.Xml}";
            }

            return "";
        }


        #region "Commands"

        /// <summary>
        /// Create an SHDR string to Remove the specified Device ID
        /// </summary>
        /// <param name="deviceUuid">The Device ID of the Device to remove</param>
        /// <param name="timestamp">The timestamp to output in the SHDR string</param>
        public static string Remove(string deviceUuid, long timestamp = 0)
        {
            if (!string.IsNullOrEmpty(deviceUuid))
            {
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

                return $"{ts.ToDateTime().ToString("o")}|{DeviceRemoveDesignator}|{deviceUuid}";
            }

            return null;
        }

        /// <summary>
        /// Create an SHDR string to Remove All Devices of the specified Type
        /// </summary>
        /// <param name="deviceType">The Device Type of the Device(s) to remove</param>
        /// <param name="timestamp">The timestamp to output in the SHDR string</param>
        public static string RemoveAll(string deviceType, long timestamp = 0)
        {
            if (!string.IsNullOrEmpty(deviceType))
            {
                var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

                return $"{ts.ToDateTime().ToString("o")}|{DeviceRemoveAllDesignator}|{deviceType}";
            }

            return null;
        }

        #endregion

        #region "Detect"

        public static bool IsDeviceLine(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format (Single) : <timestamp>|@ASSET@|<deviceUuid>|<deviceType>|<xml>
                // Expected format (Single) : 2012-02-21T23:59:33.460470Z|@ASSET@|KSSP300R.1|CuttingTool|<CuttingTool>...

                // Start reading input and read Timestamp first (if specified)
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, out _))
                {
                    var y = ShdrLine.GetNextSegment(input);
                    x = ShdrLine.GetNextValue(y);
                    return x == DeviceDesignator;
                }
                else
                {
                    //return FromLine(input);
                }
            }

            return false;
        }

        public static bool IsDeviceMultilineBegin(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format (Multiline) : <timestamp>|@ASSET@|<deviceUuid>|<deviceType>|--multiline--0FED07ACED

                // Start reading input and read Timestamp first (if specified)
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, out _))
                {
                    var y = ShdrLine.GetNextSegment(input);

                    return _multilineBeginRegex.IsMatch(y);
                }
                else
                {
                    //return FromLine(input);
                }
            }

            return false;
        }

        public static bool IsDeviceMultilineEnd(string multilineId, string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format (Multiline) : --multiline--0FED07ACED
                var match = _multilineEndRegex.Match(input);
                if (match.Success && match.Groups.Count > 1)
                {
                    var id = match.Groups[1].Value;
                    return id == multilineId;
                }
            }

            return false;
        }

        public static bool IsDeviceRemove(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format : @REMOVE_ASSET@|<DeviceUuid>
                var match = _deviceRemoveRegex.Match(input);
                return match.Success && match.Groups.Count > 1;
            }

            return false;
        }

        public static bool IsDeviceRemoveAll(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format : @REMOVE_ASSET_ALL@|<Type>
                var match = _deviceRemoveAllRegex.Match(input);
                return match.Success && match.Groups.Count > 1;
            }

            return false;
        }

        #endregion

        #region "Read"

        public static long ReadTimestamp(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, out var timestamp))
                {
                    return timestamp.ToUnixTime();
                }
            }

            return 0;
        }

        public static string ReadDeviceUuid(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format (Multiline) : <timestamp>|@ASSET@|<deviceUuid>|<deviceType>|--multiline--0FED07ACED

                // Start reading input and read Timestamp first (if specified)
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, out _))
                {
                    var y = ShdrLine.GetNextSegment(input);

                    var match = _deviceUuidRegex.Match(y);
                    if (match.Success && match.Groups.Count > 1)
                    {
                        return match.Groups[1].Value;
                    }
                }
                else
                {
                    //return FromLine(input);
                }
            }

            return null;
        }

        public static string ReadDeviceType(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format (Multiline) : <timestamp>|@ASSET@|<deviceUuid>|<deviceType>|--multiline--0FED07ACED

                // Start reading input and read Timestamp first (if specified)
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, out _))
                {
                    var y = ShdrLine.GetNextSegment(input);

                    var match = _deviceTypeRegex.Match(y);
                    if (match.Success && match.Groups.Count > 1)
                    {
                        return match.Groups[1].Value;
                    }
                }
                else
                {
                    //return FromLine(input);
                }
            }

            return null;
        }

        public static string ReadDeviceMultilineId(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format (Multiline) : <timestamp>|@ASSET@|<deviceUuid>|<deviceType>|--multiline--0FED07ACED

                // Start reading input and read Timestamp first (if specified)
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, out _))
                {
                    var y = ShdrLine.GetNextSegment(input);

                    var match = _multilineBeginRegex.Match(y);
                    if (match.Success && match.Groups.Count > 1)
                    {
                        return match.Groups[1].Value;
                    }
                }
                else
                {
                    //return FromLine(input);
                }
            }

            return null;
        }

        public static string ReadRemoveDeviceUuid(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Start reading input and read Timestamp first (if specified)
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, out _))
                {
                    var y = ShdrLine.GetNextSegment(input);

                    var match = _deviceRemoveRegex.Match(y);
                    if (match.Success && match.Groups.Count > 1)
                    {
                        return match.Groups[1].Value;
                    }
                }
                else
                {
                    //return FromLine(input);
                }
            }

            return null;
        }

        public static string ReadRemoveAllDeviceType(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Start reading input and read Timestamp first (if specified)
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, out _))
                {
                    var y = ShdrLine.GetNextSegment(input);

                    var match = _deviceRemoveAllRegex.Match(y);
                    if (match.Success && match.Groups.Count > 1)
                    {
                        return match.Groups[1].Value;
                    }
                }
                else
                {
                    //return FromLine(input);
                }
            }

            return null;
        }


        public static ShdrDevice FromString(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format (Single) : <timestamp>|@ASSET@|<deviceUuid>|<deviceType>|<xml>
                // Expected format (Single) : 2012-02-21T23:59:33.460470Z|@ASSET@|KSSP300R.1|CuttingTool|<CuttingTool>...

                return FromLine(input);
            }

            return null;
        }

        private static ShdrDevice FromLine(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                try
                {
                    // Skip @ASSET@. We already know if it is an Device or not by this point.
                    var y = ShdrLine.GetNextSegment(input);
                    if (y != null)
                    {
                        // Set Device ID
                        var x = ShdrLine.GetNextValue(y);
                        y = ShdrLine.GetNextSegment(y);
                        var deviceUuid = x;

                        if (y != null)
                        {
                            // Set Device Type
                            x = ShdrLine.GetNextValue(y);
                            y = ShdrLine.GetNextSegment(y);
                            var deviceType = x;

                            if (y != null)
                            {
                                // Set Device XML
                                x = ShdrLine.GetNextValue(y);
                                if (x != null)
                                {
                                    return new ShdrDevice(deviceUuid, x);
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        #endregion
    }
}
