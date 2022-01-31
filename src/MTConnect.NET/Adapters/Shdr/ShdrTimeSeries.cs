// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using MTConnect.Observations;

namespace MTConnect.Adapters.Shdr
{
    public class ShdrTimeSeries : TimeSeriesObservation
    {
        public bool IsUnavailable { get; set; }

        public bool IsSent { get; set; }


        public ShdrTimeSeries() { }

        public ShdrTimeSeries(string key, IEnumerable<double> samples, double sampleRate)
        {
            Key = key;
            Samples = samples;
            SampleRate = sampleRate;
        }

        public ShdrTimeSeries(string key, IEnumerable<double> samples, double sampleRate, long timestamp)
        {
            Key = key;
            Samples = samples;
            SampleRate = sampleRate;
            Timestamp = timestamp;
        }

        public ShdrTimeSeries(string key, IEnumerable<double> samples, double sampleRate, DateTime timestamp)
        {
            Key = key;
            Samples = samples;
            SampleRate = sampleRate;
            Timestamp = timestamp.ToUnixTime();
        }

        public ShdrTimeSeries(TimeSeriesObservation timeSeriesObservation)
        {
            if (timeSeriesObservation != null)
            {
                DeviceName = timeSeriesObservation.DeviceName;
                Key = timeSeriesObservation.Key;
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
            if (!string.IsNullOrEmpty(Key) && !Samples.IsNullOrEmpty())
            {
                if (Timestamp > 0)
                {
                    if (!IsUnavailable)
                    {
                        return $"{Timestamp.ToDateTime().ToString("o")}|{Key}|{Samples.Count()}|{SampleRate}|{PrintSamples(Samples)}";
                    }
                    else
                    {
                        return $"{Timestamp.ToDateTime().ToString("o")}|{Key}|{Streams.DataItem.Unavailable}";
                    }
                }
                else
                {
                    if (!IsUnavailable)
                    {
                        return $"{Key}|{Samples.Count()}|{SampleRate}|{PrintSamples(Samples)}";
                    }
                    else
                    {
                        return $"{Key}|{Streams.DataItem.Unavailable}";
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

                    // Set DataItemId
                    var x = ShdrLine.GetNextValue(input);
                    var y = ShdrLine.GetNextSegment(input);
                    timeSeries.Key = x;

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
