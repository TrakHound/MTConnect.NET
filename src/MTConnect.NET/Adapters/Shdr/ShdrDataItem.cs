// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MTConnect.Observations;

namespace MTConnect.Adapters.Shdr
{
    public class ShdrDataItem : Observation
    {
        private static readonly Regex _deviceNameRegex = new Regex("(.*):(.*)");
        private static readonly Regex _resetTriggeredRegex = new Regex(@":([A-Z_]+)\s+(.*)");

        public bool IsSent { get; set; }


        public ShdrDataItem() { }

        public ShdrDataItem(string key)
        {
            Key = key;
            Timestamp = 0;
        }

        public ShdrDataItem(string key, object value)
        {
            Key = key;
            Values = new List<ObservationValue>
            {
                new ObservationValue(ValueTypes.CDATA, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = 0;
        }

        public ShdrDataItem(string key, object value, long timestamp)
        {
            Key = key;
            Values = new List<ObservationValue>
            {
                new ObservationValue(ValueTypes.CDATA, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = timestamp;
        }

        public ShdrDataItem(string key, object value, DateTime timestamp)
        {
            Key = key;
            Values = new List<ObservationValue>
            {
                new ObservationValue(ValueTypes.CDATA, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = timestamp.ToUnixTime();
        }

        public ShdrDataItem(string key, string valueType, object value, long timestamp)
        {
            Key = key;
            Values = new List<ObservationValue>
            {
                new ObservationValue(ValueTypes.CDATA, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = timestamp;
        }

        public ShdrDataItem(string key, string valueType, object value, DateTime timestamp)
        {
            Key = key;
            Values = new List<ObservationValue>
            {
                new ObservationValue(ValueTypes.CDATA, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = timestamp.ToUnixTime();
        }

        public ShdrDataItem(Observation observation)
        {
            if (observation != null)
            {
                DeviceName = observation.DeviceName;
                Key = observation.Key;
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
            if (!string.IsNullOrEmpty(Key))
            {
                var valueString = GetValue(ValueTypes.CDATA);
                if (valueString != null)
                {
                    var value = valueString.Replace("|", @"\|");
                    var resetTriggered = ResetTriggered != Streams.ResetTriggered.NOT_SPECIFIED ? $":{ResetTriggered} " : "";

                    if (Timestamp > 0 && Duration > 0)
                    {
                        return $"{Timestamp.ToDateTime().ToString("o")}@{Duration}|{Key}|{resetTriggered}{value}";
                    }
                    else if (Timestamp > 0)
                    {
                        return $"{Timestamp.ToDateTime().ToString("o")}|{Key}|{resetTriggered}{value}";
                    }
                    else
                    {
                        return $"{Key}|{resetTriggered}{value}";
                    }
                }     
            }

            return null;
        }

        private static string ToString(ShdrDataItem dataItem, bool ignoreTimestamp = false)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.Key))
            {
                var valueString = dataItem.GetValue(ValueTypes.CDATA);
                if (valueString != null)
                {
                    var value = valueString.Replace("|", @"\|");
                    var resetTriggered = dataItem.ResetTriggered != Streams.ResetTriggered.NOT_SPECIFIED ? $":{dataItem.ResetTriggered} " : "";

                    if (dataItem.Timestamp > 0 && dataItem.Duration > 0)
                    {
                        return $"{dataItem.Timestamp.ToDateTime().ToString("o")}@{dataItem.Duration}|{dataItem.Key}|{resetTriggered}{value}";
                    }
                    else if (dataItem.Timestamp > 0 && !ignoreTimestamp)
                    {
                        return $"{GetTimestampString(dataItem.Timestamp, dataItem.Duration)}|{dataItem.Key}|{resetTriggered}{value}";
                    }
                    else
                    {
                        return $"{dataItem.Key}|{resetTriggered}{value}";
                    }
                }
            }

            return "";
        }

        private static string GetTimestampString(long timestamp, double duration = 0)
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


        public static string ToString(IEnumerable<Observation> observations)
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
                    // Get list of unique Timestamps in list of ShdrDataItems
                    var timestamps = dataItems.Select(o => o.Timestamp).Distinct();
                    if (!timestamps.IsNullOrEmpty())
                    {
                        foreach (var timestamp in timestamps)
                        {
                            string line = null;

                            // Get list of ShdrDataItems at this Timestamp (No Duration)
                            var timestampDataItems = dataItems.Where(o => o.Timestamp == timestamp && o.Duration <= 0)?.ToList();
                            if (!timestampDataItems.IsNullOrEmpty())
                            {
                                // Add Timestamp to beginning of line
                                line = GetTimestampString(timestamp) + "|";

                                // Add each DataItem to line
                                for (var i = 0; i < timestampDataItems.Count; i++)
                                {
                                    var x = ToString(timestampDataItems[i], true);
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
                            timestampDataItems = dataItems.Where(o => o.Timestamp == timestamp && o.Duration > 0)?.ToList();
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

                        // Get Device Name (if specified). Example : Device01:avail
                        var match = _deviceNameRegex.Match(x);
                        if (match.Success && match.Groups.Count > 2)
                        {
                            dataItem.DeviceName = match.Groups[1].Value;
                            dataItem.Key = match.Groups[2].Value;
                        }
                        else
                        {
                            dataItem.Key = x;
                        }

                        if (y != null)
                        {
                            // Set Value
                            x = ShdrLine.GetNextValue(y);

                            dataItem.ResetTriggered = Streams.ResetTriggered.NOT_SPECIFIED;
                            var valueString = x;

                            if (x != null)
                            {
                                // Parse the ResetTriggered (if exists)
                                var resetMatch = _resetTriggeredRegex.Match(x);
                                if (resetMatch.Success && resetMatch.Groups.Count > 2)
                                {
                                    dataItem.ResetTriggered = resetMatch.Groups[1].Value.ConvertEnum<Streams.ResetTriggered>();
                                    valueString = resetMatch.Groups[2].Value;
                                }
                            }


                            dataItem.AddValue(new ObservationValue(ValueTypes.CDATA, valueString != null ? valueString.ToString() : string.Empty));

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
