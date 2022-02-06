// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations;
using MTConnect.Streams;
using System;

namespace MTConnect.Adapters.Shdr
{
    public class ShdrFaultState: ConditionObservation
    {
        public ShdrFaultState() { }

        public ShdrFaultState(
            ConditionLevel level,
            string text = null,
            string nativeCode = null,
            string nativeSeverity = null,
            ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED
            )
        {
            Level = level;
            NativeCode = nativeCode;
            NativeSeverity = nativeSeverity;
            Qualifier = qualifier;
            Text = text;
        }

        public ShdrFaultState(ConditionObservation conditionObservation)
        {
            if (conditionObservation != null)
            {
                DeviceName = conditionObservation.DeviceName;
                Key = conditionObservation.Key;
                Level = conditionObservation.Level;
                NativeCode = conditionObservation.NativeCode;
                NativeSeverity = conditionObservation.NativeSeverity;
                Qualifier = conditionObservation.Qualifier;
                Text = conditionObservation.Text;
                Timestamp = conditionObservation.Timestamp;
            }
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
                                var qualifier = x.ConvertEnum<ConditionQualifier>();
                                if (qualifier != ConditionQualifier.NOT_SPECIFIED)
                                {
                                    condition.Qualifier = qualifier;
                                }
                            }
                        }

                        // Set Text
                        x = ShdrLine.GetNextValue(y);
                        condition.Text = x;
                    }

                    return condition;
                }
                catch { }
            }

            return null;
        }
    }
}
