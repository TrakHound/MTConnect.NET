// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Linq;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Agents
{
    /// <summary>
    /// Regression pin for the empty-Result wire-level spec violation in
    /// <see cref="MTConnectAgent.AddObservation(string, MTConnect.Input.IObservationInput, bool?, bool?, bool?, bool)"/>:
    /// when the inbound observation carries a null, empty, or whitespace-only
    /// Result value, the agent currently publishes that value verbatim on the
    /// wire. The MTConnect Part 1 Observation Information Model mandates
    /// "UNAVAILABLE" as the sole valid representation of a missing value, so
    /// the SDK must coerce null / empty / whitespace to
    /// <see cref="Observation.Unavailable"/> before the observation reaches
    /// the buffer.
    ///
    /// Spec: MTConnect Standard, Part 1 - Devices Information Model,
    /// Observation Information Model - Representation - Observation Values
    /// ("If an Agent cannot determine a Valid Data Value for a DataItem, the
    /// value returned for the Result for the Data Entity MUST be reported as
    /// UNAVAILABLE.").
    ///
    /// This fixture pins the coerce on the canonical
    /// <c>AddObservation(string, IObservationInput, ...)</c> overload that
    /// every other AddObservation overload routes through. The convenience
    /// overload <c>AddObservation(string deviceKey, string dataItemKey, object value, DateTime timestamp)</c>
    /// at MTConnectAgent.cs:2007 is used as the exercise vector because it
    /// builds a fresh ObservationInput and routes through the canonical path
    /// at MTConnectAgent.cs:2123.
    /// </summary>
    [TestFixture]
    [Category("AddObservationEmptyResultCoerce")]
    public class AddObservationEmptyResultCoerceTests
    {
        private const string DeviceKey = "U-COERCE";
        private const string DataItemKey = "availability";

        private static readonly InputValidationLevel[] _nonStrictLevels =
        {
            InputValidationLevel.Ignore,
            InputValidationLevel.Warning,
            InputValidationLevel.Remove,
        };

        private static readonly object?[] _nullEmptyWhitespaceValues =
        {
            new object?[] { null },
            new object?[] { string.Empty },
            new object?[] { "   " },
            new object?[] { "\t" },
            new object?[] { "\n" },
        };

        /// <summary>Pins the positive contract: under every non-Strict input-validation level, an empty-string Result is coerced to <see cref="Observation.Unavailable"/> on the wire.</summary>
        /// <param name="level">The input-validation level the agent operates under.</param>
        [Test]
        [TestCaseSource(nameof(_nonStrictLevels))]
        public void AddObservation_EmptyStringResult_Coerced_To_Unavailable_Under_NonStrict_Levels(InputValidationLevel level)
        {
            using var agent = NewAgentWithAvailabilityDataItem(level);

            var added = agent.AddObservation(DeviceKey, DataItemKey, (object)string.Empty, DateTime.UtcNow);

            Assert.That(added, Is.True, "empty-Result observation must reach the buffer post-coerce");
            Assert.That(CurrentResult(agent), Is.EqualTo(Observation.Unavailable),
                "Part 1 mandates UNAVAILABLE for any Result that is null, empty, or whitespace");
        }

        /// <summary>Pins the positive contract across the null / empty / whitespace family: every such Result is coerced to <see cref="Observation.Unavailable"/>.</summary>
        /// <param name="badValue">The non-Valid-Data-Value Result the caller forwards in.</param>
        [Test]
        [TestCaseSource(nameof(_nullEmptyWhitespaceValues))]
        public void AddObservation_NullEmptyOrWhitespaceResult_Coerced_To_Unavailable(object? badValue)
        {
            using var agent = NewAgentWithAvailabilityDataItem(InputValidationLevel.Warning);

            var added = agent.AddObservation(DeviceKey, DataItemKey, badValue!, DateTime.UtcNow);

            Assert.That(added, Is.True);
            Assert.That(CurrentResult(agent), Is.EqualTo(Observation.Unavailable));
        }

        /// <summary>Pins the secondary defect's positive contract: under <see cref="InputValidationLevel.Strict"/>, an empty-Result observation is coerced and lands in the buffer rather than being silently dropped.</summary>
        [Test]
        public void AddObservation_EmptyResult_Under_Strict_Coerced_And_Lands()
        {
            using var agent = NewAgentWithAvailabilityDataItem(InputValidationLevel.Strict);

            var added = agent.AddObservation(DeviceKey, DataItemKey, (object)string.Empty, DateTime.UtcNow);

            Assert.That(added, Is.True, "Strict must coerce to UNAVAILABLE — never silently drop an empty Result");
            Assert.That(CurrentResult(agent), Is.EqualTo(Observation.Unavailable));
        }

        /// <summary>Pins the negative contract: a concrete non-empty Result is preserved verbatim — the coerce only fires on the null / empty / whitespace family.</summary>
        [Test]
        public void AddObservation_ConcreteResult_Is_Preserved_Verbatim()
        {
            using var agent = NewAgentWithAvailabilityDataItem(InputValidationLevel.Warning);

            var added = agent.AddObservation(DeviceKey, DataItemKey, (object)"AVAILABLE", DateTime.UtcNow);

            Assert.That(added, Is.True);
            Assert.That(CurrentResult(agent), Is.EqualTo("AVAILABLE"),
                "the coerce must not substitute a sentinel for a Valid Data Value");
        }

        private static MTConnectAgentBroker NewAgentWithAvailabilityDataItem(InputValidationLevel level)
        {
            var config = new AgentConfiguration { InputValidationLevel = level };
            var agent = new MTConnectAgentBroker(config);
            agent.Start();

            var device = new Device
            {
                Id = "d-coerce",
                Name = "d-coerce",
                Uuid = DeviceKey,
            };
            device.AddDataItem(new AvailabilityDataItem(device.Id));

            var added = agent.AddDevice(device);
            Assert.That(added, Is.Not.Null, "AddDevice must succeed for test pre-condition");
            return agent;
        }

        private static object? CurrentResult(IMTConnectAgentBroker agent)
        {
            var current = agent.GetCurrentObservations(DeviceKey, DataItemKey).SingleOrDefault();
            return current?.GetValue(ValueKeys.Result);
        }
    }
}
