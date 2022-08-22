// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
