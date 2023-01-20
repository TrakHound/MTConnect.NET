// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using MTConnect.Observations.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MTConnect.Shdr
{
    /// <summary>
    /// An Observation representing either an MTConnect Sample or Event DataItem with a Representation of VALUE
    /// </summary>
    public class ShdrDataItem : ObservationInput
    {
        private static readonly Regex _deviceKeyRegex = new Regex("(.*):(.*)");
        private static readonly Regex _resetTriggeredRegex = new Regex(@":([A-Z_]+)\s+(.*)");


        /// <summary>
        /// Flag to set whether the Observation has been sent by the adapter or not
        /// </summary>
        internal bool IsSent { get; set; }

        public string CDATA
        {
            get => GetValue(ValueKeys.Result);
            set => AddValue(new ObservationValue(ValueKeys.Result, value));
        }


        public ShdrDataItem() { }

        public ShdrDataItem(string dataItemKey)
        {
            DataItemKey = dataItemKey;
            Timestamp = 0;
        }

        public ShdrDataItem(string dataItemKey, object value)
        {
            DataItemKey = dataItemKey;
            Values = new List<ObservationValue>
            {
                new ObservationValue(ValueKeys.Result, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = 0;
        }

        public ShdrDataItem(string dataItemKey, object value, long timestamp)
        {
            DataItemKey = dataItemKey;
            Values = new List<ObservationValue>
            {
                new ObservationValue(ValueKeys.Result, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = timestamp;
        }

        public ShdrDataItem(string dataItemKey, object value, DateTime timestamp)
        {
            DataItemKey = dataItemKey;
            Values = new List<ObservationValue>
            {
                new ObservationValue(ValueKeys.Result, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = timestamp.ToUnixTime();
        }


        public ShdrDataItem(ObservationInput observation)
        {
            if (observation != null)
            {
                DeviceKey = observation.DeviceKey;
                DataItemKey = observation.DataItemKey;
                Values = observation.Values;
                Timestamp = observation.Timestamp;
            }
        }

        /// <summary>
        /// Convert ShdrDataItem to an SHDR string
        /// </summary>
        /// <returns>SHDR string</returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(DataItemKey))
            {
                var valueString = GetValue(ValueKeys.Result);
                if (valueString != null)
                {
                    var target = DataItemKey;
                    if (!string.IsNullOrEmpty(DeviceKey)) target = $"{DeviceKey}:{target}";

                    var value = valueString.Replace("|", @"\|").Trim();
                    var resetTriggered = ResetTriggered != ResetTriggered.NOT_SPECIFIED ? $":{ResetTriggered}" : "";

                    if (Timestamp > 0 && Duration > 0)
                    {
                        return $"{Timestamp.ToDateTime().ToString("o")}@{Duration}|{target}|{value}{resetTriggered}";
                    }
                    else if (Timestamp > 0)
                    {
                        return $"{Timestamp.ToDateTime().ToString("o")}|{target}|{value}{resetTriggered}";
                    }
                    else if (Duration > 0)
                    {
                        return $"@{Duration}|{target}|{value}{resetTriggered}";
                    }
                    else
                    {
                        return $"{target}|{value}{resetTriggered}";
                    }
                }     
            }

            return null;
        }

        private static string ToString(ShdrDataItem dataItem, bool ignoreTimestamp = false, string deviceKey = null)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.DataItemKey))
            {
                var valueString = dataItem.GetValue(ValueKeys.Result);
                if (valueString != null)
                {
                    var target = dataItem.DataItemKey;
                    if (!string.IsNullOrEmpty(dataItem.DeviceKey)) target = $"{dataItem.DeviceKey}:{target}";

                    var value = valueString.Replace("|", @"\|").Trim();
                    var resetTriggered = dataItem.ResetTriggered != ResetTriggered.NOT_SPECIFIED ? $":{dataItem.ResetTriggered}" : "";

                    if (dataItem.Timestamp > 0 && dataItem.Duration > 0)
                    {
                        return $"{dataItem.Timestamp.ToDateTime().ToString("o")}@{dataItem.Duration}|{target}|{value}{resetTriggered}";
                    }
                    else if (dataItem.Timestamp > 0 && !ignoreTimestamp)
                    {
                        return $"{GetTimestampString(dataItem.Timestamp, dataItem.Duration)}|{target}|{value}{resetTriggered}";
                    }
                    else if (dataItem.Duration > 0)
                    {
                        return $"@{dataItem.Duration}|{target}|{value}{resetTriggered}";
                    }
                    else
                    {
                        return $"{target}|{value}{resetTriggered}";
                    }
                }
            }

            return "";
        }

        private static string GetTimestampString(long timestamp, double duration = 0)
        {
            if (timestamp > 0)
            {
                if (duration > 0)
                {
                    return $"{timestamp.ToDateTime().ToString("o")}@{duration}";
                }
                else
                {
                    return timestamp.ToDateTime().ToString("o");
                }
            }
            else if (duration > 0)
            {
                return $"@{duration}";
            }

            return null;
        }

        private static string GetTimestampString(DateTime timestamp, double duration = 0)
        {
            if (duration > 0)
            {
                return $"{timestamp.ToString("o")}@{duration}";
            }
            else
            {
                return timestamp.ToString("o");
            }
        }


        public static string ToString(IEnumerable<ObservationInput> observations)
        {
            if (!observations.IsNullOrEmpty())
            {
                var objs = new List<ShdrDataItem>();
                foreach (var observation in observations) objs.Add(new ShdrDataItem(observation));
                return ToString(objs);
            }

            return null;
        }

        /// <summary>
        /// Convert to a Multi-Line SHDR string. This breaks the string into lines with unique Timestamps
        /// </summary>
        /// <param name="dataItems">List of ShdrDataItems</param>
        /// <returns>A single Multi-Line SHDR valid string</returns>
        public static string ToString(IEnumerable<ShdrDataItem> dataItems)
        {
            var lines = new List<string>();

            try
            {
                if (!dataItems.IsNullOrEmpty())
                {
                    // Get list of unique DeviceKeys in list of ShdrDataItems
                    var deviceKeys = dataItems.Select(o => o.DeviceKey).Distinct();
                    if (!deviceKeys.IsNullOrEmpty())
                    {
                        foreach (var deviceKey in deviceKeys)
                        {
                            var deviceDataItems = dataItems.Where(o => o.DeviceKey == deviceKey);

                            // Get list of unique Timestamps in list of ShdrDataItems
                            var timestamps = deviceDataItems.Select(o => o.Timestamp).Distinct();
                            if (!timestamps.IsNullOrEmpty())
                            {
                                var oTimestamps = timestamps.OrderBy(o => o);

                                foreach (var timestamp in oTimestamps)
                                {
                                    string line = null;

                                    // Get list of ShdrDataItems at this Timestamp (No Duration)
                                    var timestampDataItems = deviceDataItems.Where(o => o.Timestamp == timestamp && o.Duration <= 0)?.ToList();
                                    if (!timestampDataItems.IsNullOrEmpty())
                                    {
                                        // Add Timestamp to beginning of line
                                        var timestampPrefix = GetTimestampString(timestamp);
                                        if (!string.IsNullOrEmpty(timestampPrefix)) line = timestampPrefix + "|";

                                        // Add each DataItem to line
                                        for (var i = 0; i < timestampDataItems.Count; i++)
                                        {
                                            var x = ToString(timestampDataItems[i], true, timestampDataItems[i].DeviceKey);
                                            if (!string.IsNullOrEmpty(x))
                                            {
                                                line += x;

                                                if (i < timestampDataItems.Count - 1) line += "|";
                                            }
                                        }

                                        // Add line to list of lines
                                        lines.Add(line);
                                    }

                                    // Get list of ShdrDataItems at this Timestamp (With Duration)
                                    timestampDataItems = deviceDataItems.Where(o => o.Timestamp == timestamp && o.Duration > 0)?.ToList();
                                    if (!timestampDataItems.IsNullOrEmpty())
                                    {
                                        foreach (var dataItem in timestampDataItems)
                                        {
                                            line = dataItem.ToString();
                                            if (!string.IsNullOrEmpty(line))
                                            {
                                                // Add line to list of lines
                                                lines.Add(line);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }  
                }

                // Convert list of lines to single string with new line terminator
                return string.Join(ShdrLine.LineTerminator, lines);
            }
            catch { }

            return "";
        }

        /// <summary>
        /// Read list of ShdrDataItem objects from an SHDR line
        /// </summary>
        /// <param name="input">SHDR Input String</param>
        /// <returns>List of ShdrDataItems</returns>
        public static IEnumerable<ShdrDataItem> FromString(string input, bool uppercaseValue = true)
        {
            var dataItems = new List<ShdrDataItem>();

            if (!string.IsNullOrEmpty(input))
            {
                // Expected format (Single) : <timestamp>|<key>|<value>
                // Expected format (Single) : 2009-06-15T00:00:00.000000|power|ON
                // Expected format (Multiple) : <timestamp>|<key>|<value>|<key>|<value>|<key>|<value>|<key>|<value>|<key>|<value>
                // Expected format (Multiple) : 2009-06-15T00:00:00.000000|power|ON|execution|ACTIVE|line|412|Xact|-1.1761875153|Yact|1766618937

                // Start reading input and read Timestamp first (if specified)
                var x = ShdrLine.GetNextValue(input);
                var timestamp = ShdrLine.GetTimestamp(x);
                var duration = ShdrLine.GetDuration(x);

                if (timestamp.HasValue)
                {
                    var y = ShdrLine.GetNextSegment(input);
                    return FromKeyValuePairs(y, timestamp.Value.ToUnixTime(), duration.HasValue ? duration.Value : 0, uppercaseValue);
                }
                else
                {
                    return FromKeyValuePairs(input, 0, duration.HasValue ? duration.Value : 0, uppercaseValue);
                }
            }

            return dataItems;
        }

        private static IEnumerable<ShdrDataItem> FromKeyValuePairs(string input, long timestamp = 0, double duration = 0, bool uppercaseValue = true)
        {
            var dataItems = new List<ShdrDataItem>();

            if (!string.IsNullOrEmpty(input))
            {
                var y = input;

                try
                {
                    while (y != null)
                    {
                        // Create new ShdrDataItem
                        var dataItem = new ShdrDataItem();
                        dataItem.Timestamp = timestamp;
                        dataItem.Duration = duration;

                        // Set DataItemId
                        var x = ShdrLine.GetNextValue(y);
                        y = ShdrLine.GetNextSegment(y);

                        // Get Device Key (if specified). Example : Device01:avail
                        var match = _deviceKeyRegex.Match(x);
                        if (match.Success && match.Groups.Count > 2)
                        {
                            dataItem.DeviceKey = match.Groups[1].Value;
                            dataItem.DataItemKey = match.Groups[2].Value;
                        }
                        else
                        {
                            dataItem.DataItemKey = x;
                        }

                        if (y != null)
                        {
                            // Set Value
                            x = ShdrLine.GetNextValue(y);
                            y = ShdrLine.GetNextSegment(y);

                            dataItem.ResetTriggered = ResetTriggered.NOT_SPECIFIED;
                            var valueString = x;

                            if (x != null)
                            {
                                // Parse the ResetTriggered (if exists)
                                var resetMatch = _resetTriggeredRegex.Match(x);
                                if (resetMatch.Success && resetMatch.Groups.Count > 2)
                                {
                                    dataItem.ResetTriggered = resetMatch.Groups[1].Value.ConvertEnum<ResetTriggered>();
                                    valueString = resetMatch.Groups[2].Value;
                                }
                            }


                            dataItem.AddValue(new ObservationValue(ValueKeys.Result, valueString != null ? valueString.ToString() : string.Empty));

                            dataItems.Add(dataItem);
                        }
                    }
                }
                catch { }
            }

            return dataItems;
        }
    }
}