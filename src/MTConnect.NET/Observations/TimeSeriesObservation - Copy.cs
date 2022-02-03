// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

namespace MTConnect.Observations
{
    /// <summary>
    /// An Information Model that describes Streaming Data reported by a piece of equipment
    /// where multiple values are reported at fixed intervals in a single observation
    /// </summary>
    public class TimeSeriesObservation : ITimeSeriesObservation
    {
        /// <summary>
        /// The name of the Device that the Observation is associated with
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// The (ID, Name, or Source) of the DataItem that the Observation is associated with
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The frequency at which the values were observed at
        /// </summary>
        public double SampleRate { get; set; }

        /// <summary>
        /// The values that were reported during the Observation
        /// </summary>
        public IEnumerable<double> Samples { get; set; }
        /// <summary>
        /// The timestamp (UnixTime in Milliseconds) that the observation was recorded at
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// A MD5 Hash of the Observation that can be used for comparison
        /// </summary>
        public string ChangeId
        {
            get
            {
                if (!Samples.IsNullOrEmpty())
                {
                    var x = "";
                    foreach (var sample in Samples) x += $"{sample}|";
                    return x;
                }

                return null;
            }
        }


        public TimeSeriesObservation() { }

        public TimeSeriesObservation(string key, IEnumerable<double> samples, double sampleRate)
        {
            Key = key;
            Samples = samples;
            SampleRate = sampleRate;
        }

        public TimeSeriesObservation(string key, IEnumerable<double> samples, double sampleRate, long timestamp)
        {
            Key = key;
            Samples = samples;
            SampleRate = sampleRate;
            Timestamp = timestamp;
        }

        public TimeSeriesObservation(string key, IEnumerable<double> samples, double sampleRate, DateTime timestamp)
        {
            Key = key;
            Samples = samples;
            SampleRate = sampleRate;
            Timestamp = timestamp.ToUnixTime();
        }
    }
}
