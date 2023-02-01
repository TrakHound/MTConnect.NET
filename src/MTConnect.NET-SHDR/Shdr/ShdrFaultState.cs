// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations;
using MTConnect.Observations.Input;
using System;
using System.Text.RegularExpressions;

namespace MTConnect.Shdr
{
    /// <summary>
    /// A FaultState associated with an MTConnect Condition Observation
    /// </summary>
    public class ShdrFaultState: ConditionObservationInput
    {
        private static readonly Regex _deviceKeyRegex = new Regex("(.*):(.*)");


        public ShdrFaultState() { }

        public ShdrFaultState(
            ConditionLevel level,
            string message = null,
            string nativeCode = null,
            string nativeSeverity = null,
            ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED,
            long timestamp = 0
            )
        {
            Level = level;
            if (!string.IsNullOrEmpty(nativeCode)) NativeCode = nativeCode;
            if (!string.IsNullOrEmpty(nativeSeverity)) NativeSeverity = nativeSeverity;
            Qualifier = qualifier;
            if (!string.IsNullOrEmpty(message)) Message = message;
            Timestamp = timestamp;
        }

        public ShdrFaultState(ConditionObservationInput conditionObservation)
        {
            if (conditionObservation != null)
            {
                DeviceKey = conditionObservation.DeviceKey;
                DataItemKey = conditionObservation.DataItemKey;
                Level = conditionObservation.Level;
                if (!string.IsNullOrEmpty(conditionObservation.NativeCode)) NativeCode = conditionObservation.NativeCode;
                if (!string.IsNullOrEmpty(conditionObservation.NativeSeverity)) NativeSeverity = conditionObservation.NativeSeverity;
                Qualifier = conditionObservation.Qualifier;
                if (!string.IsNullOrEmpty(conditionObservation.Message)) Message = conditionObservation.Message;
                Timestamp = conditionObservation.Timestamp;
            }
        }


        /// <summary>
        /// Convert ShdrFaultState to an SHDR string
        /// </summary>
        /// <returns>SHDR string</returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(DataItemKey))
            {
                var target = DataItemKey;
                if (!string.IsNullOrEmpty(DeviceKey)) target = $"{DeviceKey}:{target}";

                var message = !string.IsNullOrEmpty(Message) ? Message.Replace("|", @"\|") : "";
                var qualifier = Qualifier != ConditionQualifier.NOT_SPECIFIED ? Qualifier.ToString() : "";

                string line;

                if (Timestamp > 0)
                {
                    if (Level != ConditionLevel.UNAVAILABLE)
                    {
                        line = $"{Timestamp.ToDateTime().ToString("o")}|{target}|{Level}|{NativeCode}|{NativeSeverity}|{qualifier}|{message}";
                    }
                    else
                    {
                        line = $"{Timestamp.ToDateTime().ToString("o")}|{target}|{Level}||||";
                    }
                }
                else
                {
                    if (Level != ConditionLevel.UNAVAILABLE)
                    {
                        line = $"{target}|{Level}|{NativeCode}|{NativeSeverity}|{qualifier}|{message}";
                    }
                    else
                    {
                        line = $"{target}|{Level}||||";
                    }
                }

                return line;
            }

            return "";
        }


        /// <summary>
        /// Read a ShdrCondition object from an SHDR line
        /// </summary>
        /// <param name="input">SHDR Input String</param>
        public static ShdrFaultState FromString(string input)
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

        private static ShdrFaultState FromLine(string input, long timestamp = 0)
        {
            if (!string.IsNullOrEmpty(input))
            {
                try
                {
                    var condition = new ShdrFaultState();
                    condition.Timestamp = timestamp;

                    // Set DataItemKey
                    var x = ShdrLine.GetNextValue(input);
                    var y = ShdrLine.GetNextSegment(input);

                    // Get Device Key (if specified). Example : Device01:avail
                    var match = _deviceKeyRegex.Match(x);
                    if (match.Success && match.Groups.Count > 2)
                    {
                        condition.DeviceKey = match.Groups[1].Value;
                        condition.DataItemKey = match.Groups[2].Value;
                    }
                    else
                    {
                        condition.DataItemKey = x;
                    }

                    // Set Condition Level
                    x = ShdrLine.GetNextValue(y);
                    y = ShdrLine.GetNextSegment(y);
                    condition.Level = (ConditionLevel)Enum.Parse(typeof(ConditionLevel), x);

                    if (y != null)
                    {
                        // Set NativeCode
                        x = ShdrLine.GetNextValue(y);
                        y = ShdrLine.GetNextSegment(y);
                        if (!string.IsNullOrEmpty(x)) condition.NativeCode = x;

                        if (y != null)
                        {
                            // Set NativeSeverity
                            x = ShdrLine.GetNextValue(y);
                            y = ShdrLine.GetNextSegment(y);
                            if (!string.IsNullOrEmpty(x)) condition.NativeSeverity = x;

                            if (y != null)
                            {
                                // Set Qualifier
                                x = ShdrLine.GetNextValue(y);
                                y = ShdrLine.GetNextSegment(y);
                                var qualifier = x.ConvertEnum<ConditionQualifier>();
                                if (qualifier != ConditionQualifier.NOT_SPECIFIED)
                                {
                                    condition.Qualifier = qualifier;
                                }
                            }
                        }

                        // Set Message
                        x = ShdrLine.GetNextValue(y);
                        if (!string.IsNullOrEmpty(x)) condition.Message = x;
                    }

                    return condition;
                }
                catch { }
            }

            return null;
        }
    }
}