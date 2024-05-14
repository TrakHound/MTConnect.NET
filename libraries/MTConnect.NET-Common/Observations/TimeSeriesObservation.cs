// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Observations
{
    public static class TimeSeriesObservation
    {
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