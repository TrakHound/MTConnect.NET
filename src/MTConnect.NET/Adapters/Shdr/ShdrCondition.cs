// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MTConnect.Streams;
using MTConnect.Observations;

namespace MTConnect.Adapters.Shdr
{
    public class ShdrCondition : ConditionObservation
    {
        public bool IsSent { get; set; }


        private ShdrCondition() { }

        public ShdrCondition(string key, ConditionLevel level)
        {
            Key = key;
            Level = level;
        }

        public ShdrCondition(string key, ConditionLevel level, long timestamp)
        {
            Key = key;
            Level = level;
            Timestamp = timestamp;
        }

        public ShdrCondition(string key, ConditionLevel level, DateTime timestamp)
        {
            Key = key;
            Level = level;
            Timestamp = timestamp.ToUnixTime();
        }

        public ShdrCondition(string deviceName, string key, ConditionLevel level)
        {
            DeviceName = deviceName;
            Key = key;
            Level = level;
        }

        public ShdrCondition(string deviceName, string key, ConditionLevel level, long timestamp)
        {
            DeviceName = deviceName;
            Key = key;
            Level = level;
            Timestamp = timestamp;
        }

        public ShdrCondition(string deviceName, string key, ConditionLevel level, DateTime timestamp)
        {
            DeviceName = deviceName;
            Key = key;
            Level = level;
            Timestamp = timestamp.ToUnixTime();
        }

        public ShdrCondition(ConditionObservation conditionObservation)
        {
            if (conditionObservation != null)
            {
                DeviceName = conditionObservation.DeviceName;
                Key = conditionObservation.Key;
                Level = conditionObservation.Level;
                NativeCode = conditionObservation.NativeCode;
                NativeSeverity = conditionObservation.NativeSeverity;
                Qualifier = conditionObservation.Qualifier;
                Message = conditionObservation.Message;
                Timestamp = conditionObservation.Timestamp;
            }
        }


        /// <summary>
        /// Convert ShdrCondition to an SHDR string
        /// </summary>
        /// <returns>SHDR string</returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Key))
            {
                var message = !string.IsNullOrEmpty(Message) ? Message.Replace("|", @"\|") : "";

                if (Timestamp > 0)
                {
                    if (Level != ConditionLevel.UNAVAILABLE)
                    {
                        return $"{Timestamp.ToDateTime().ToString("o")}|{Key}|{Level}|{NativeCode}|{NativeSeverity}|{Qualifier}|{message}";
                    }
                    else
                    {
                        return $"{Timestamp.ToDateTime().ToString("o")}|{Key}|{Level}||||";
                    }
                }
                else
                {
                    if (Level != ConditionLevel.UNAVAILABLE)
                    {
                        return $"{Key}|{Level}|{NativeCode}|{NativeSeverity}|{Qualifier}|{message}";
                    }
                    else
                    {
                        return $"{Key}|{Level}||||";
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// Read a ShdrCondition object from an SHDR line
        /// </summary>
        /// <param name="input">SHDR Input String</param>
        public static ShdrCondition FromString(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Expected format : <timestamp>|<data_item_name>|<level>|<native_code>|<native_severity>|<qualifier>|<message>
                // Expected format : 2014-09-29T23:59:33.460470Z|htemp|WARNING|HTEMP|1|HIGH|Oil Temperature High

                // Start reading input and read Timestamp first (if specified)
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out var timestamp))
                {
                    var y = ShdrLine.GetNextSegment(input);
                    return FromLine(y, timestamp.ToUnixTime());
                }
                else
                {
                    return FromLine(input);
                }
            }

            return null;
        }

        private static ShdrCondition FromLine(string input, long timestamp = 0)
        {
            if (!string.IsNullOrEmpty(input))
            {
                try
                {
                    var condition = new ShdrCondition();
                    condition.Timestamp = timestamp;

                    // Set DataItemId
                    var x = ShdrLine.GetNextValue(input);
                    var y = ShdrLine.GetNextSegment(input);
                    condition.Key = x;

                    // Set Condition Level
                    x = ShdrLine.GetNextValue(y);
                    y = ShdrLine.GetNextSegment(y);
                    condition.Level = (ConditionLevel)Enum.Parse(typeof(ConditionLevel), x);

                    if (y != null)
                    {
                        // Set NativeCode
                        x = ShdrLine.GetNextValue(y);
                        y = ShdrLine.GetNextSegment(y);
                        condition.NativeCode = x;

                        if (y != null)
                        {
                            // Set NativeSeverity
                            x = ShdrLine.GetNextValue(y);
                            y = ShdrLine.GetNextSegment(y);
                            condition.NativeSeverity = x;

                            if (y != null)
                            {
                                // Set Qualifier
                                x = ShdrLine.GetNextValue(y);
                                y = ShdrLine.GetNextSegment(y);
                                condition.Qualifier = x;
                            }
                        }

                        // Set Message
                        x = ShdrLine.GetNextValue(y);
                        condition.Message = x;
                    }

                    return condition;
                }
                catch { }
            }

            return null;
        }
    }
}
