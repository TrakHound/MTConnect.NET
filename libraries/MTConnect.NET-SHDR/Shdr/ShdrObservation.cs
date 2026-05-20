// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Input;
using MTConnect.Observations;
using System.Linq;

namespace MTConnect.Shdr
{
    /// <summary>
    /// Helper that classifies an inbound observation by inspecting its value-key shape so the
    /// SHDR adapter knows which line format to emit (or which parser to apply). The
    /// classification is driven by the well-known <see cref="ValueKeys"/> markers:
    /// <c>Level</c> implies a Condition, <c>NativeCode</c> implies a Message, and the
    /// data-set/table/time-series predicates identify those payload shapes.
    /// </summary>
    public static class ShdrObservation
    {
        /// <summary>
        /// Returns the <see cref="ShdrObservationType"/> that matches the value-key shape of
        /// <paramref name="observation"/>. Defaults to <see cref="ShdrObservationType.DataItem"/>
        /// when no specialised key is found.
        /// </summary>
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
