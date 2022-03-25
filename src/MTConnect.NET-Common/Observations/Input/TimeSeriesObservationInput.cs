// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Observations.Input
{
    /// <summary>
    /// An Information Model that describes Streaming Data reported by a piece of equipment
    /// where multiple values are reported at fixed intervals in a single observation
    /// </summary>
    public class TimeSeriesObservationInput : ObservationInput
    {
        /// <summary>
        /// The frequency at which the values were observed at
        /// </summary>
        public double SampleRate
        {
            get => GetValue(ValueKeys.SampleRate).ToDouble();
            set => AddValue(new ObservationValue(ValueKeys.SampleRate, value));
        }

        /// <summary>
        /// The values that were reported during the Observation
        /// </summary>
        public IEnumerable<double> Samples
        {
            get => TimeSeriesObservation.GetSamples(Values);
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    var x = value.ToList();
                    for (var i = 0; i < x.Count(); i++)
                    {
                        AddValue(new ObservationValue(ValueKeys.CreateTimeSeriesValueKey(i), x[i]));
                    }
                }
            }
        }


        public TimeSeriesObservationInput() { }

        public TimeSeriesObservationInput(string dataItemKey, IEnumerable<double> samples, double sampleRate)
        {
            DataItemKey = dataItemKey;
            Samples = samples;
            SampleRate = sampleRate;
        }

        public TimeSeriesObservationInput(string dataItemKey, IEnumerable<double> samples, double sampleRate, long timestamp)
        {
            DataItemKey = dataItemKey;
            Samples = samples;
            SampleRate = sampleRate;
            Timestamp = timestamp;
        }

        public TimeSeriesObservationInput(string dataItemKey, IEnumerable<double> samples, double sampleRate, DateTime timestamp)
        {
            DataItemKey = dataItemKey;
            Samples = samples;
            SampleRate = sampleRate;
            Timestamp = timestamp.ToUnixTime();
        }
    }
}
