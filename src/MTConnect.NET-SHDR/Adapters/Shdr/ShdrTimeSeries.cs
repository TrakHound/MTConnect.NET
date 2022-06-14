// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations;
using MTConnect.Observations.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Adapters.Shdr
{
    /// <summary>
    /// An Observation representing an MTConnect Sample with a Representation of TIME_SERIES
    /// </summary>
    public class ShdrTimeSeries : TimeSeriesObservationInput
    {
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
                if (Timestamp > 0)
                {
                    if (!IsUnavailable)
                    {
                        return $"{Timestamp.ToDateTime().ToString("o")}|{DataItemKey}|{Samples.Count()}|{SampleRate}|{PrintSamples(Samples)}";
                    }
                    else
                    {
                        return $"{Timestamp.ToDateTime().ToString("o")}|{DataItemKey}|{Observation.Unavailable}";
                    }
                }
                else
                {
                    if (!IsUnavailable)
                    {
                        return $"{DataItemKey}|{Samples.Count()}|{SampleRate}|{PrintSamples(Samples)}";
                    }
                    else
                    {
                        return $"{DataItemKey}|{Observation.Unavailable}";
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
                    return FromLine(input);
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
                    timeSeries.DataItemKey = x;

                    // Samples Count
                    x = ShdrLine.GetNextValue(y);
                    y = ShdrLine.GetNextSegment(y);
                    var samplesCount = x;

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

                            return timeSeries;
                        }
                    }
                }
                catch { }
            }

            return null;
        }
    }
}
