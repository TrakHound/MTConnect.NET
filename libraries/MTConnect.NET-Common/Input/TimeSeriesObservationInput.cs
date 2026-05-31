// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Input
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
        /// Number of values given for the Observation
        /// </summary>
        public int SampleCount
        {
            get => GetValue(ValueKeys.SampleCount).ToInt();
            set => AddValue(new ObservationValue(ValueKeys.SampleCount, value));
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
                else
                {
                    AddValue(ValueKeys.SampleCount, 0);
                }
            }
        }


        /// <summary>
        /// Initializes a new, empty Time Series Observation with no DataItem key or samples.
        /// </summary>
        public TimeSeriesObservationInput() { }

        /// <summary>
        /// Initializes a new Time Series Observation for the specified DataItem with the given samples and sample rate.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="samples">The values reported during the Observation, in time order.</param>
        /// <param name="sampleRate">The frequency at which the samples were observed.</param>
        public TimeSeriesObservationInput(string dataItemKey, IEnumerable<double> samples, double sampleRate)
        {
            DataItemKey = dataItemKey;
            Samples = samples;
            SampleRate = sampleRate;
        }

        /// <summary>
        /// Initializes a new Time Series Observation for the specified DataItem with the given samples, sample rate, and timestamp.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="samples">The values reported during the Observation, in time order.</param>
        /// <param name="sampleRate">The frequency at which the samples were observed.</param>
        /// <param name="timestamp">The observation timestamp as UnixTime in milliseconds.</param>
        public TimeSeriesObservationInput(string dataItemKey, IEnumerable<double> samples, double sampleRate, long timestamp)
        {
            DataItemKey = dataItemKey;
            Samples = samples;
            SampleRate = sampleRate;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Initializes a new Time Series Observation for the specified DataItem with the given samples, sample rate, and timestamp.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="samples">The values reported during the Observation, in time order.</param>
        /// <param name="sampleRate">The frequency at which the samples were observed.</param>
        /// <param name="timestamp">The observation timestamp, converted to UnixTime in milliseconds.</param>
        public TimeSeriesObservationInput(string dataItemKey, IEnumerable<double> samples, double sampleRate, DateTime timestamp)
        {
            DataItemKey = dataItemKey;
            Samples = samples;
            SampleRate = sampleRate;
            Timestamp = timestamp.ToUnixTime();
        }

        /// <summary>
        /// Initializes a new Time Series Observation by copying the Device key, DataItem key, timestamp, and values from an existing Observation.
        /// </summary>
        /// <param name="observation">The source Observation to copy; a <c>null</c> argument leaves the new instance empty.</param>
        public TimeSeriesObservationInput(IObservationInput observation)
        {
            if (observation != null)
            {
                DeviceKey = observation.DeviceKey;
                DataItemKey = observation.DataItemKey;
                Timestamp = observation.Timestamp;
                Values = observation.Values;
            }
        }
    }
}