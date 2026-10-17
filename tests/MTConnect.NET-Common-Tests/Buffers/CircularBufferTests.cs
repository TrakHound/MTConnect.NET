// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Linq;
using MTConnect.Buffers;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Observations;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Buffers
{
    /// <summary>
    /// Pins the behaviour of the internal <c>CircularBuffer</c> archive through the public
    /// <see cref="MTConnectObservationBuffer"/> surface: ordering across wrap-around,
    /// overwrite-when-full, the empty-buffer edge cases and capacity validation.
    /// The archive is internal and the library does not use <c>[InternalsVisibleTo]</c>,
    /// so every case drives it through <c>AddObservation</c> / <c>GetObservations</c> /
    /// <c>GetCurrentObservations</c>.
    /// </summary>
    [TestFixture]
    [Category("CircularBuffer")]
    public class CircularBufferTests
    {
        private const int KeyA = 10;
        private const int KeyB = 20;

        private static MTConnectObservationBuffer CreateBuffer(uint capacity)
        {
            return new MTConnectObservationBuffer(new AgentConfiguration { ObservationBufferSize = capacity });
        }

        private static ulong Add(MTConnectObservationBuffer buffer, int key, string value = "v")
        {
            var observation = new BufferObservation(
                key,
                DataItemCategory.EVENT,
                DataItemRepresentation.VALUE,
                new[] { new ObservationValue(ValueKeys.Result, value) },
                0,
                UnixDateTime.Now,
                Quality.VALID,
                false,
                false);

            return buffer.AddObservation(ref observation);
        }

        private static ulong[] Sequences(IObservationBufferResults results)
        {
            return results.Observations == null ? Array.Empty<ulong>() : results.Observations.Select(o => o.Sequence).ToArray();
        }


        /// <summary>
        /// Pins the behaviour expressed by the test name: constructor zero capacity throws.
        /// </summary>
        [Test]
        public void Constructor_ZeroCapacity_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateBuffer(0));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: first sequence empty buffer returns one.
        /// </summary>
        [Test]
        public void FirstSequence_EmptyBuffer_ReturnsOne()
        {
            var buffer = CreateBuffer(4);

            Assert.That(buffer.FirstSequence, Is.EqualTo(1UL));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: first sequence after wrap is oldest retained.
        /// </summary>
        [Test]
        public void FirstSequence_AfterWrap_IsOldestRetained()
        {
            var buffer = CreateBuffer(3);
            for (var i = 0; i < 5; i++) Add(buffer, KeyA);

            Assert.That(buffer.FirstSequence, Is.EqualTo(3UL));
            Assert.That(buffer.LastSequence, Is.EqualTo(5UL));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: get observations not full returns all in order.
        /// </summary>
        [Test]
        public void GetObservations_NotFull_ReturnsAllInOrder()
        {
            var buffer = CreateBuffer(10);
            for (var i = 0; i < 4; i++) Add(buffer, KeyA);

            var results = buffer.GetObservations(new[] { KeyA }, count: 0);

            Assert.That(Sequences(results), Is.EqualTo(new ulong[] { 1, 2, 3, 4 }));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: get observations after wrap returns oldest to newest.
        /// </summary>
        [Test]
        public void GetObservations_AfterWrap_ReturnsOldestToNewest()
        {
            var buffer = CreateBuffer(3);
            for (var i = 0; i < 5; i++) Add(buffer, KeyA);

            var results = buffer.GetObservations(new[] { KeyA }, count: 0);

            Assert.That(Sequences(results), Is.EqualTo(new ulong[] { 3, 4, 5 }));
            Assert.That(results.FirstObservationSequence, Is.EqualTo(3UL));
            Assert.That(results.LastObservationSequence, Is.EqualTo(5UL));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: get observations many wraps keeps last capacity observations.
        /// </summary>
        [Test]
        public void GetObservations_ManyWraps_KeepsLastCapacityObservations()
        {
            var buffer = CreateBuffer(4);
            for (var i = 0; i < 4 * 7 + 2; i++) Add(buffer, KeyA); // Wrap several times, ending mid-ring

            var results = buffer.GetObservations(new[] { KeyA }, count: 0);

            Assert.That(Sequences(results), Is.EqualTo(new ulong[] { 27, 28, 29, 30 }));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: get observations capacity one keeps only newest.
        /// </summary>
        [Test]
        public void GetObservations_CapacityOne_KeepsOnlyNewest()
        {
            var buffer = CreateBuffer(1);
            for (var i = 0; i < 3; i++) Add(buffer, KeyA);

            var results = buffer.GetObservations(new[] { KeyA }, count: 0);

            Assert.That(Sequences(results), Is.EqualTo(new ulong[] { 3 }));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: get observations from to window across wrap returns window.
        /// </summary>
        [Test]
        public void GetObservations_FromToWindowAcrossWrap_ReturnsWindow()
        {
            var buffer = CreateBuffer(4);
            for (var i = 0; i < 10; i++) Add(buffer, KeyA); // Retains 7..10

            var results = buffer.GetObservations(new[] { KeyA }, from: 8, to: 9, count: 0);

            Assert.That(Sequences(results), Is.EqualTo(new ulong[] { 8, 9 }));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: get observations count limits results.
        /// </summary>
        [Test]
        public void GetObservations_Count_LimitsResults()
        {
            var buffer = CreateBuffer(8);
            for (var i = 0; i < 6; i++) Add(buffer, KeyA);

            var results = buffer.GetObservations(new[] { KeyA }, count: 2);

            Assert.That(Sequences(results), Is.EqualTo(new ulong[] { 1, 2 }));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: get observations multiple keys filters and groups by key.
        /// </summary>
        [Test]
        public void GetObservations_MultipleKeys_FiltersAndGroupsByKey()
        {
            var buffer = CreateBuffer(4);
            for (var i = 0; i < 6; i++) Add(buffer, i % 2 == 0 ? KeyA : KeyB); // Retains 3(A) 4(B) 5(A) 6(B)

            var results = buffer.GetObservations(new[] { KeyB, KeyA }, count: 0);

            // Results are grouped by ascending buffer key, oldest first within each key
            Assert.That(Sequences(results), Is.EqualTo(new ulong[] { 3, 5, 4, 6 }));
            Assert.That(buffer.GetObservations(new[] { KeyB }, count: 0).Observations.All(o => o.Key == KeyB), Is.True);
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: get observations empty buffer returns no observations.
        /// </summary>
        [Test]
        public void GetObservations_EmptyBuffer_ReturnsNoObservations()
        {
            var buffer = CreateBuffer(4);

            var results = buffer.GetObservations(new[] { KeyA }, count: 0);

            Assert.That(results.ObservationCount, Is.EqualTo(0U));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: get current observations at within archive after wrap returns latest at or before.
        /// </summary>
        [Test]
        public void GetCurrentObservationsAt_WithinArchiveAfterWrap_ReturnsLatestAtOrBefore()
        {
            var buffer = CreateBuffer(4);
            for (var i = 0; i < 10; i++) Add(buffer, KeyA); // Retains 7..10

            var results = buffer.GetCurrentObservations(new[] { KeyA }, 8);

            Assert.That(Sequences(results), Is.EqualTo(new ulong[] { 8 }));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: get current observations at beyond last while not full does not throw.
        /// </summary>
        [Test]
        public void GetCurrentObservationsAt_BeyondLastWhileNotFull_DoesNotThrow()
        {
            // Regression: the scan was bounded by Capacity, so reading past Size threw
            var buffer = CreateBuffer(10);
            for (var i = 0; i < 4; i++) Add(buffer, KeyA);

            IObservationBufferResults? results = null;
            Assert.DoesNotThrow(() => results = buffer.GetCurrentObservations(new[] { KeyA }, 8));

            Assert.That(Sequences(results!), Is.EqualTo(new ulong[] { 4 }));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: get current observations at older than archive does not return newer observation.
        /// </summary>
        [Test]
        public void GetCurrentObservationsAt_OlderThanArchive_DoesNotReturnNewerObservation()
        {
            var buffer = CreateBuffer(4);
            for (var i = 0; i < 10; i++) Add(buffer, KeyA); // Retains 7..10

            var results = buffer.GetCurrentObservations(new[] { KeyA }, 5);

            Assert.That(results.Observations.Any(o => o.Sequence > 5), Is.False);
        }
    }
}
