// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using MTConnect.Observations.Output;

namespace MTConnect
{
    /// <summary>
    /// Single-pass grouping helper for MqttRelay observation
    /// publishing. Groups by <see cref="IObservationOutput.DataItemId"/>
    /// in one pass over the source so the publish hot path stays O(n)
    /// in the observation count, which keeps the post-reconnect
    /// catch-up bounded on large agents.
    /// </summary>
    internal static class ObservationGrouper
    {
        /// <summary>
        /// Groups <paramref name="observations"/> by
        /// <see cref="IObservationOutput.DataItemId"/> in a single
        /// pass. Encounter order is preserved within each group so a
        /// caller that relies on sequence-monotonic ordering is not
        /// broken. A null source returns an empty result.
        /// </summary>
        public static IEnumerable<IGrouping<string, IObservationOutput>> GroupByDataItem(
            IEnumerable<IObservationOutput> observations)
        {
            if (observations == null) return Enumerable.Empty<IGrouping<string, IObservationOutput>>();

            // Materialise into a List then GroupBy, so a deferred
            // upstream query is iterated exactly once. (LINQ GroupBy
            // is itself single-pass over its source, but ToList
            // makes the contract explicit and lets the caller iterate
            // the resulting groups multiple times safely.)
            return observations.ToList().GroupBy(o => o.DataItemId);
        }
    }
}
