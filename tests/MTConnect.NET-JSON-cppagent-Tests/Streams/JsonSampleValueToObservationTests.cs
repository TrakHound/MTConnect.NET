// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations;
using MTConnect.Streams.Json;
using NUnit.Framework;

namespace MTConnect.NET_JSON_cppagent_Tests.Streams
{
    /// <summary>
    /// Pins the round-trip behavior of <see cref="JsonSampleValue.ToObservation(string)"/>
    /// against null <c>ResetTriggered</c> and <c>Statistic</c> JSON
    /// properties (the JSON-cppagent wire format omits these when not
    /// supplied). The carrier must not write a stray default-enum value to
    /// the observation when the JSON did not supply one — mirror the
    /// null-guard already present on the sibling
    /// Devices/JsonDataItem.ToDataItem path.
    /// </summary>
    [Category("NumericSampleAsJsonNumber")]
    [TestFixture]
    public class JsonSampleValueToObservationTests
    {
        [Test]
        public void ToObservation_with_null_reset_triggered_does_not_store_value()
        {
            var sample = new JsonSampleValue
            {
                DataItemId = "temp",
                Value = "42.5",
                ResetTriggered = null,
                Statistic = null,
            };

            ISampleValueObservation observation = null;
            Assert.That(
                () => observation = sample.ToObservation("Temperature"),
                Throws.Nothing,
                "ToObservation must null-guard ResetTriggered/Statistic " +
                "rather than calling ConvertEnum on a null surface.");

            Assert.That(observation, Is.Not.Null);

            // The carrier must not stamp the observation with a stray
            // default-enum value when the JSON wire format omitted the
            // property. Inspect the underlying ValueKeys so the contract
            // is independent of the property's getter shape.
            var resetTriggeredStored = ((Observation)observation!).GetValue(ValueKeys.ResetTriggered);
            var statisticStored = ((Observation)observation).GetValue(ValueKeys.Statistic);

            Assert.That(resetTriggeredStored, Is.Null,
                "Null ResetTriggered must not get stored on the observation " +
                "as a stray default-enum value.");
            Assert.That(statisticStored, Is.Null,
                "Null Statistic must not get stored on the observation as a " +
                "stray default-enum value.");
        }

        [Test]
        public void ToObservation_with_supplied_reset_triggered_sets_enum()
        {
            // Confirm the happy path still flows through ConvertEnum when
            // a value is supplied.
            var sample = new JsonSampleValue
            {
                DataItemId = "temp",
                Value = "42.5",
                ResetTriggered = "MANUAL",
                Statistic = "AVERAGE",
            };

            var observation = sample.ToObservation("Temperature");

            Assert.That(observation.ResetTriggered, Is.EqualTo(ResetTriggered.MANUAL));
            Assert.That(observation.Statistic, Is.EqualTo(DataItemStatistic.AVERAGE));
        }
    }
}
