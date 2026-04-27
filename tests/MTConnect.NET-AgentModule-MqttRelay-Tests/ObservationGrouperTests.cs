// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using MTConnect.Devices;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using NUnit.Framework;

namespace MTConnect.AgentModule.MqttRelay.Tests
{
    /// <summary>
    /// Pins the MqttRelay PublishObservations grouping policy. The
    /// previous implementation iterated the input enumerable up to
    /// three times per distinct DataItemId:
    ///
    ///   var dataItemIds = observations.Select(o => o.DataItemId).Distinct();
    ///   foreach (var dataItemId in dataItemIds)
    ///   {
    ///       var dataItemObservations = observations.Where(o => o.DataItemId == dataItemId);
    ///       var dataItemObservation  = dataItemObservations.FirstOrDefault();
    ///       ...
    ///   }
    ///
    /// That is O(n*k) where n is the observation count and k is the
    /// distinct-DataItemId count, plus repeated enumeration of an
    /// IEnumerable that may be a deferred/expensive query. On large
    /// agents (thousands of observations across hundreds of data
    /// items) that materially slowed the relay catch-up after a
    /// reconnect.
    ///
    /// ObservationGrouper.GroupByDataItem produces a single-pass
    /// grouping: an IEnumerable of (DataItemId, observations) groups,
    /// each iteration of the source happening exactly once.
    /// </summary>
    [TestFixture]
    public class ObservationGrouperTests
    {
        [Test]
        public void GroupByDataItem_returns_empty_when_input_null()
        {
            Assert.That(ObservationGrouper.GroupByDataItem(null), Is.Empty);
        }

        [Test]
        public void GroupByDataItem_returns_empty_when_input_empty()
        {
            Assert.That(
                ObservationGrouper.GroupByDataItem(new List<IObservationOutput>()),
                Is.Empty);
        }

        [Test]
        public void GroupByDataItem_groups_observations_by_data_item_id()
        {
            var input = new List<IObservationOutput>
            {
                Stub("A", DataItemCategory.SAMPLE, sequence: 1),
                Stub("B", DataItemCategory.EVENT, sequence: 2),
                Stub("A", DataItemCategory.SAMPLE, sequence: 3),
                Stub("C", DataItemCategory.CONDITION, sequence: 4),
                Stub("B", DataItemCategory.EVENT, sequence: 5),
            };

            var groups = ObservationGrouper.GroupByDataItem(input).ToList();

            Assert.That(groups.Select(g => g.Key), Is.EquivalentTo(new[] { "A", "B", "C" }));

            var groupA = groups.Single(g => g.Key == "A").ToList();
            Assert.That(groupA.Select(o => o.Sequence), Is.EquivalentTo(new ulong[] { 1, 3 }));

            var groupB = groups.Single(g => g.Key == "B").ToList();
            Assert.That(groupB.Select(o => o.Sequence), Is.EquivalentTo(new ulong[] { 2, 5 }));

            var groupC = groups.Single(g => g.Key == "C").ToList();
            Assert.That(groupC.Select(o => o.Sequence), Is.EquivalentTo(new ulong[] { 4 }));
        }

        [Test]
        public void GroupByDataItem_iterates_source_exactly_once()
        {
            // Pins the perf-fix contract. The previous Module.cs path
            // iterated the source up to three times (Distinct, Where,
            // FirstOrDefault) per group. The grouping helper must take
            // a single pass over the source.
            var iterationCount = 0;
            IEnumerable<IObservationOutput> Source()
            {
                iterationCount++;
                yield return Stub("A", DataItemCategory.SAMPLE, sequence: 1);
                yield return Stub("B", DataItemCategory.EVENT, sequence: 2);
                yield return Stub("A", DataItemCategory.SAMPLE, sequence: 3);
            }

            var groups = ObservationGrouper.GroupByDataItem(Source()).ToList();
            // Force eager enumeration of every group.
            foreach (var g in groups) _ = g.ToList();

            Assert.That(iterationCount, Is.EqualTo(1),
                "Grouping must iterate the source exactly once.");
            Assert.That(groups, Has.Count.EqualTo(2));
        }

        [Test]
        public void GroupByDataItem_preserves_first_seen_order_per_group()
        {
            // Useful for callers that rely on encounter order (the
            // condition path) for sequence-monotonic publishing.
            var input = new List<IObservationOutput>
            {
                Stub("A", DataItemCategory.SAMPLE, sequence: 10),
                Stub("A", DataItemCategory.SAMPLE, sequence: 11),
                Stub("A", DataItemCategory.SAMPLE, sequence: 12),
            };

            var groupA = ObservationGrouper.GroupByDataItem(input).Single();
            Assert.That(
                groupA.Select(o => o.Sequence).ToList(),
                Is.EqualTo(new ulong[] { 10, 11, 12 }));
        }

        [Test]
        public void GroupByDataItem_handles_null_data_item_id_as_distinct_group()
        {
            // Defensive: an IObservationOutput stub with a null
            // DataItemId should not crash the grouping; it should
            // appear as its own (null-keyed) group so the caller can
            // decide what to do with it.
            var input = new List<IObservationOutput>
            {
                Stub(null, DataItemCategory.SAMPLE, sequence: 1),
                Stub("A", DataItemCategory.SAMPLE, sequence: 2),
                Stub(null, DataItemCategory.SAMPLE, sequence: 3),
            };

            var groups = ObservationGrouper.GroupByDataItem(input).ToList();

            Assert.That(groups, Has.Count.EqualTo(2));
            var nullGroup = groups.Single(g => g.Key == null).ToList();
            Assert.That(nullGroup.Select(o => o.Sequence), Is.EquivalentTo(new ulong[] { 1, 3 }));
        }

        private static IObservationOutput Stub(string dataItemId, DataItemCategory category, ulong sequence)
        {
            return new ObservationOutputStub(dataItemId, category, sequence);
        }

        private sealed class ObservationOutputStub : IObservationOutput
        {
            public ObservationOutputStub(string dataItemId, DataItemCategory category, ulong sequence)
            {
                DataItemId = dataItemId;
                Category = category;
                Sequence = sequence;
            }

            public string DeviceUuid => "device-1";
            public IDataItem DataItem => null;
            public string DataItemId { get; }
            public DataItemCategory Category { get; }
            public string Type => "TYPE";
            public string SubType => null;
            public string Name => null;
            public ulong InstanceId => 0UL;
            public ulong Sequence { get; }
            public DateTime Timestamp => DateTime.UtcNow;
            public DateTimeOffset TimeZoneTimestamp => DateTimeOffset.UtcNow;
            public string CompositionId => null;
            public DataItemRepresentation Representation => DataItemRepresentation.VALUE;
            public Quality Quality => Quality.VALID;
            public bool Deprecated => false;
            public bool Extended => false;
            public ObservationValue[] Values => Array.Empty<ObservationValue>();
            public string GetValue(string valueKey) => null;
        }
    }
}
