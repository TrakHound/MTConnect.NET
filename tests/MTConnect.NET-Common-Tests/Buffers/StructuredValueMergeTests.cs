// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Linq;
using MTConnect.Buffers;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Observations;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Buffers
{
    /// <summary>
    /// Pins how the current value of a DATA_SET or TABLE DataItem is merged with a new observation:
    /// new keys/entries replace existing ones, untouched ones are kept, the Count is recomputed and
    /// the values stay sorted by key.
    /// </summary>
    [TestFixture]
    [Category("StructuredValueMerge")]
    public class StructuredValueMergeTests
    {
        private const int Key = 10;

        private static void Add(MTConnectObservationBuffer buffer, DataItemRepresentation representation, params ObservationValue[] values)
        {
            var observation = new BufferObservation(Key, DataItemCategory.EVENT, representation, values, 0, UnixDateTime.Now, Quality.VALID, false, false);
            buffer.AddObservation(ref observation);
        }

        private static ObservationValue[] Current(MTConnectObservationBuffer buffer)
        {
            return buffer.CurrentObservations[Key].Values;
        }

        /// <summary>Pins the behaviour expressed by the test name: data set entries merge and the count is recomputed.</summary>
        [Test]
        public void DataSet_new_entries_replace_existing_and_count_is_recomputed()
        {
            var buffer = new MTConnectObservationBuffer(new AgentConfiguration { ObservationBufferSize = 16 });

            Add(buffer, DataItemRepresentation.DATA_SET,
                new ObservationValue(ValueKeys.CreateDataSetValueKey("a"), "1"),
                new ObservationValue(ValueKeys.CreateDataSetValueKey("b"), "2"));

            Add(buffer, DataItemRepresentation.DATA_SET,
                new ObservationValue(ValueKeys.CreateDataSetValueKey("b"), "20"),
                new ObservationValue(ValueKeys.CreateDataSetValueKey("c"), "3"));

            var values = Current(buffer);
            string Get(string key) => values.FirstOrDefault(o => o.Key == key).Value;

            Assert.That(Get(ValueKeys.CreateDataSetValueKey("a")), Is.EqualTo("1"));
            Assert.That(Get(ValueKeys.CreateDataSetValueKey("b")), Is.EqualTo("20"));
            Assert.That(Get(ValueKeys.CreateDataSetValueKey("c")), Is.EqualTo("3"));
            Assert.That(Get(ValueKeys.Count), Is.EqualTo("3"));
            Assert.That(values.Count(o => o.Key == ValueKeys.CreateDataSetValueKey("b")), Is.EqualTo(1));
            Assert.That(values.Select(o => o.Key), Is.Ordered);
        }

        /// <summary>Pins the behaviour expressed by the test name: a table entry in the new values replaces all of that entry's cells.</summary>
        [Test]
        public void Table_new_entry_replaces_all_cells_of_that_entry()
        {
            var buffer = new MTConnectObservationBuffer(new AgentConfiguration { ObservationBufferSize = 16 });

            Add(buffer, DataItemRepresentation.TABLE,
                new ObservationValue(ValueKeys.CreateTableValueKey("r1", "x"), "1"),
                new ObservationValue(ValueKeys.CreateTableValueKey("r1", "y"), "2"),
                new ObservationValue(ValueKeys.CreateTableValueKey("r2", "x"), "3"));

            Add(buffer, DataItemRepresentation.TABLE,
                new ObservationValue(ValueKeys.CreateTableValueKey("r1", "z"), "9"),
                new ObservationValue(ValueKeys.CreateTableValueKey("r3", "x"), "4"));

            var keys = Current(buffer).Select(o => o.Key).ToList();

            Assert.That(keys, Does.Not.Contain(ValueKeys.CreateTableValueKey("r1", "x")));
            Assert.That(keys, Does.Not.Contain(ValueKeys.CreateTableValueKey("r1", "y")));
            Assert.That(keys, Does.Contain(ValueKeys.CreateTableValueKey("r1", "z")));
            Assert.That(keys, Does.Contain(ValueKeys.CreateTableValueKey("r2", "x")));
            Assert.That(keys, Does.Contain(ValueKeys.CreateTableValueKey("r3", "x")));
            Assert.That(Current(buffer).First(o => o.Key == ValueKeys.Count).Value, Is.EqualTo("3"));
            Assert.That(keys, Is.Ordered);
        }
    }
}
