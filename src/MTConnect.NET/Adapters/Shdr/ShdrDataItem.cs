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
            //if (!string.IsNullOrEmpty(Key) && Value != null)
            if (!string.IsNullOrEmpty(Key))
            {
                var valueString = GetValue(ValueTypes.CDATA);
                //if (!string.IsNullOrEmpty(valueString))
                if (valueString != null)
                {
                    var value = valueString.Replace("|", @"\|");

                    if (Timestamp > 0)
                    {
                        return $"{Timestamp.ToDateTime().ToString("o")}|{Key}|{value}";
                    }
                    else
                    {
                        return $"{Key}|{value}";
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
                //if (!string.IsNullOrEmpty(valueString))
                if (valueString != null)
                {
                    var value = valueString.Replace("|", @"\|");

                    if (dataItem.Timestamp > 0 && !ignoreTimestamp)
                    {
                        return $"{GetTimestampString(dataItem.Timestamp)}|{dataItem.Key}|{value}";
                    }
                    else
                    {
                        return $"{dataItem.Key}|{value}";
                    }
                }
            }

            return "";
        }

        private static string GetTimestampString(long timestamp)
        {
            return timestamp.ToDateTime().ToString("o");
        }

        private static string GetTimestampString(DateTime timestamp)
        {
            return timestamp.ToString("o");
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
                            // Get list of ShdrDataItems at this Timestamp
                            var timestampDataItems = dataItems.Where(o => o.Timestamp == timestamp)?.ToList();
                            if (!timestampDataItems.IsNullOrEmpty())
                            {
                                // Add Timestamp to beginning of line
                                var line = GetTimestampString(timestamp) + "|";

                                // Add each DataItem to line
                                for (var i = 0; i < timestampDataItems.Count; i++)
                                {
                                    line += ToString(timestampDataItems[i], true);

                                    if (i < timestampDataItems.Count - 1) line += "|";
                                }

                                // Add line to list of lines
                                lines.Add(line);
                            }
                        }
                    }
                }

                // Convert list of lines to single string with new line terminator
                return string.Join("\r\n", lines);
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

                if (DateTime.TryParse(x, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out var timestamp))
                {
                    var y = ShdrLine.GetNextSegment(input);
                    return FromKeyValuePairs(y, timestamp.ToUnixTime(), uppercaseValue);
                }
                else
                {
                    return FromKeyValuePairs(input, 0, uppercaseValue);
                }
            }

            return dataItems;
        }

        private static IEnumerable<ShdrDataItem> FromKeyValuePairs(string input, long timestamp = 0, bool uppercaseValue = true)
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

                        // Set DataItemId
                        var x = ShdrLine.GetNextValue(y);
                        y = ShdrLine.GetNextSegment(y);

                        // Get Device Name (if specified). Example : Device01:avail
                        var match = new Regex("(.*):(.*)").Match(x);
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
                            y = ShdrLine.GetNextSegment(y);

                            dataItem.Values = new List<ObservationValue>
                            {
                                new ObservationValue(ValueTypes.CDATA, x != null ? x.ToString() : string.Empty)
                            };

                            //dataItem.Value = !string.IsNullOrEmpty(x) ? x.ToUpper() : "";

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
