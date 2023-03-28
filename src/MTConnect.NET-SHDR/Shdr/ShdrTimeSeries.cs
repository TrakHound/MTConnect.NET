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
    /// An Observation representing an MTConnect Sample with a Representation of TIME_SERIES
    /// </summary>
    public class ShdrTimeSeries : TimeSeriesObservationInput
    {
        private static readonly Regex _deviceKeyRegex = new Regex("(.*):(.*)");


        /// <summary>
        /// Flag to set whether the Observation has been sent by the adapter or not
        /// </summary>
        internal bool IsSent { get; set; }


        public ShdrTimeSeries() { }

        public ShdrTimeSeries(string dataItemKey, IEnumerable<double> samples, double sampleRate)
        {
            DataItemKey = dataItemKey;
            Samples = samples;
            SampleRate = sampleRate;
        }

        public ShdrTimeSeries(string dataItemKey, IEnumerable<double> samples, double sampleRate, long timestamp)
        {
            DataItemKey = dataItemKey;
            Samples = samples;
            SampleRate = sampleRate;
            Timestamp = timestamp;
        }

        public ShdrTimeSeries(string dataItemKey, IEnumerable<double> samples, double sampleRate, DateTime timestamp)
        {
            DataItemKey = dataItemKey;
            Samples = samples;
            SampleRate = sampleRate;
            Timestamp = timestamp.ToUnixTime();
        }

        public ShdrTimeSeries(TimeSeriesObservationInput timeSeriesObservation)
        {
            if (timeSeriesObservation != null)
            {
                DeviceKey = timeSeriesObservation.DeviceKey;
                DataItemKey = timeSeriesObservation.DataItemKey;
                SampleRate = timeSeriesObservation.SampleRate;
                Samples = timeSeriesObservation.Samples;
                Timestamp = timeSeriesObservation.Timestamp;
            }
        }


        /// <summary>
        /// Convert ShdrTimeSeries to an SHDR string
        /// </summary>
        /// <returns>SHDR string</returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(DataItemKey))
            {
                var target = DataItemKey;
                if (!string.IsNullOrEmpty(DeviceKey)) target = $"{DeviceKey}:{target}";

                if (Timestamp > 0)
                {
                    if (!IsUnavailable)
                    {
                        return $"{Timestamp.ToDateTime().ToString("o")}|{target}|{Samples.Count()}|{SampleRate}|{PrintSamples(Samples)}";
                    }
                    else
                    {
                        return $"{Timestamp.ToDateTime().ToString("o")}|{target}|{Observation.Unavailable}";
                    }
                }
                else
                {
                    if (!IsUnavailable)
                    {
                        return $"|{target}|{Samples.Count()}|{SampleRate}|{PrintSamples(Samples)}";
                    }
                    else
                    {
                        return $"|{target}|{Observation.Unavailable}";
                    }
                }
            }

            return null;
        }

        private static string PrintSamples(IEnumerable<double> samples)
        {
            if (!samples.IsNullOrEmpty())
            {
                var pairs = new List<string>();

                foreach (var sample in samples)
                {
                    pairs.Add(sample.ToString());
                }

                return string.Join(" ", pairs);
            }

            return "";
        }

        /// <summary>
        /// Read a ShdrTimeSeries object from an SHDR line
        /// </summary>
        /// <param name="input">SHDR Input String</param>
        public static ShdrTimeSeries FromString(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Start reading input and read Timestamp first (if specified)
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out var timestamp))
                {
                    var y = ShdrLine.GetNextSegment(input);
                    return FromLine(y, timestamp.ToUnixTime());
                }
                else
                {
                    var y = ShdrLine.GetNextSegment(input);
                    return FromLine(y);
                }
            }

            return null;
        }

        private static ShdrTimeSeries FromLine(string input, long timestamp = 0)
        {
            if (!string.IsNullOrEmpty(input))
            {
                try
                {
                    var timeSeries = new ShdrTimeSeries();
                    timeSeries.Timestamp = timestamp;

                    // Set DataItemKey
                    var x = ShdrLine.GetNextValue(input);
                    var y = ShdrLine.GetNextSegment(input);

                    // Get Device Key (if specified). Example : Device01:avail
                    var match = _deviceKeyRegex.Match(x);
                    if (match.Success && match.Groups.Count > 2)
                    {
                        timeSeries.DeviceKey = match.Groups[1].Value;
                        timeSeries.DataItemKey = match.Groups[2].Value;
                    }
                    else
                    {
                        timeSeries.DataItemKey = x;
                    }

                    x = ShdrLine.GetNextValue(y);
                    if (!string.IsNullOrEmpty(x) && x.ToLower() != Observation.Unavailable.ToLower())
                    {
                        // Samples Count
                        x = ShdrLine.GetNextValue(y);
                        y = ShdrLine.GetNextSegment(y);
                        timeSeries.SampleCount = x.ToInt();

                        // Set Sample Rate
                        x = ShdrLine.GetNextValue(y);
                        y = ShdrLine.GetNextSegment(y);
                        timeSeries.SampleRate = x.ToDouble();

                        if (y != null)
                        {
                            x = ShdrLine.GetNextValue(y);
                            if (!string.IsNullOrEmpty(x))
                            {
                                var samples = new List<double>();
                                var sampleSegments = x.Split(' ');

                                foreach (var sampleSegment in sampleSegments)
                                {
                                    samples.Add(sampleSegment.ToDouble());
                                }

                                timeSeries.Samples = samples;
                                timeSeries.SampleCount = samples.Count();
                            }
                        }
                    }
                    else
                    {
                        timeSeries.SampleCount = 0;
                        timeSeries.SampleRate = 0;
                        timeSeries.Unavailable();
                    }

                    return timeSeries;
                }
                catch { }
            }

            return null;
        }
    }
}