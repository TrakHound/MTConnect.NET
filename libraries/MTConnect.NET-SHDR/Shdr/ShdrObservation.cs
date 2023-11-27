// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Input;
using MTConnect.Observations;
using System.Linq;

namespace MTConnect.Shdr
{
    public static class ShdrObservation
    {
        public static ShdrObservationType GetObservationType(IObservationInput observation)
        {
            if (observation != null && observation.Values != null)
            {
                if (observation.Values.Any(o => o.Key == ValueKeys.Level)) return ShdrObservationType.Condition;
                if (observation.Values.Any(o => o.Key == ValueKeys.NativeCode)) return ShdrObservationType.Message;
                if (observation.Values.Any(o => ValueKeys.IsDataSetKey(o.Key))) return ShdrObservationType.DataSet;
                if (observation.Values.Any(o => ValueKeys.IsTableKey(o.Key))) return ShdrObservationType.Table;
                if (observation.Values.Any(o => ValueKeys.IsTimeSeriesKey(o.Key))) return ShdrObservationType.TimeSeries;          
            }

            return ShdrObservationType.DataItem;
        }
    }
}
