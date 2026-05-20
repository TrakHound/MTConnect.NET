// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Observations
{
    /// <summary>
    /// A Time Series observation reports an ordered sequence of sampled values captured over a time interval.
    /// </summary>
    public static class TimeSeriesObservation
    {
        /// <summary>
        /// Extracts the ordered numeric samples from a flat collection of Observation values.
        /// </summary>
        /// <param name="values">The Observation values to decode.</param>
        /// <returns>The samples in index order.</returns>
        public static IEnumerable<double> GetSamples(IEnumerable<ObservationValue> values)
        {
            var samples = new List<double>();

            if (!values.IsNullOrEmpty())
            {
                var sampleValues = values.Where(o => o.Key != null && o.Key.StartsWith(ValueKeys.TimeSeriesPrefix));
                if (!sampleValues.IsNullOrEmpty())
                {
                    var oValues = sampleValues.OrderBy(o => ValueKeys.GetTimeSeriesIndex(o.Key));
                    foreach (var value in oValues)
                    {
                        samples.Add(value.Value.ToDouble());
                    }
                }
            }

            return samples;
        }
    }
}