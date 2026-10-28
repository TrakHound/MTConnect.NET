// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using MTConnect.Input;
using MTConnect.Observations;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Input
{
    /// <summary>
    /// Pins <c>MTConnectDataSource.SuppressUnchangedObservations</c>: when enabled, a value equal to the last value
    /// sent for the same DataItem is not raised again, while changes (including back to a previous value) are.
    /// </summary>
    [TestFixture]
    [Category("DataSourceSuppression")]
    public class DataSourceSuppressionTests
    {
        private sealed class TestDataSource : MTConnectDataSource
        {
            public TestDataSource(bool suppress) { SuppressUnchangedObservations = suppress; }
        }

        private static List<string> Send(bool suppress, params (string key, string value)[] observations)
        {
            var source = new TestDataSource(suppress);
            var raised = new List<string>();
            source.ObservationAdded += (s, o) => raised.Add(o.DataItemKey + "=" + o.GetValue(ValueKeys.Result));

            foreach (var (key, value) in observations) source.AddObservation(key, value);
            return raised;
        }

        /// <summary>Pins the behaviour expressed by the test name: repeated values are suppressed and changes are raised.</summary>
        [Test]
        public void Enabled_suppresses_repeats_but_raises_changes()
        {
            var raised = Send(true, ("a", "1"), ("a", "1"), ("b", "1"), ("a", "2"), ("a", "2"), ("a", "1"));

            Assert.That(raised, Is.EqualTo(new[] { "a=1", "b=1", "a=2", "a=1" }));
        }

        /// <summary>Pins the behaviour expressed by the test name: by default every value is raised.</summary>
        [Test]
        public void Disabled_by_default_raises_every_value()
        {
            var raised = Send(false, ("a", "1"), ("a", "1"));

            Assert.That(raised, Is.EqualTo(new[] { "a=1", "a=1" }));
        }
    }
}
