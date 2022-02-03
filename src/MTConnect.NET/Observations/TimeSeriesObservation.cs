// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Observations
{
    /// <summary>
    /// An Information Model that describes Streaming Data reported by a piece of equipment
    /// where multiple values are reported at fixed intervals in a single observation
    /// </summary>
    public class TimeSeriesObservation : Observation
    {
        /// <summary>
        /// The frequency at which the values were observed at
        /// </summary>
        public double SampleRate
        {
            get => GetValue(ValueTypes.SampleRate).ToDouble();
            set => AddValue(new ObservationValue(ValueTypes.SampleRate, value));
        }

        /// <summary>
        /// The values that were reported during the Observation
        /// </summary>
        public IEnumerable<double> Samples
        {
            get
            {
                var values = new List<double>();

                if (!Values.IsNullOrEmpty())
                {
                    var sampleValues = Values.Where(o => o.ValueType != null && o.ValueType.StartsWith(ValueTypes.TimeSeriesPrefix));
                    if (!sampleValues.IsNullOrEmpty())
                    {
                        var oValues = sampleValues.OrderBy(o => ValueTypes.GetTimeSeriesIndex(o.ValueType));
                        foreach (var value in oValues)
                        {
                            values.Add(value.Value.ToDouble());
                        }
                    }
                }

                return values;
            }
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    var x = value.ToList();
                    for (var i = 0; i < x.Count(); i++)
                    {
                        AddValue(new ObservationValue(ValueTypes.CreateTimeSeriesValueType(i), x[i]));
                    }
                }
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
