// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Input;
using MTConnect.Observations;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Agents
{
    /// <summary>
    /// Pins that an unchanged value is filtered as a duplicate however the Device and DataItem are addressed
    /// (UUID / ID, or Name), for both regular observations and conditions.
    /// </summary>
    [TestFixture]
    [Category("DuplicateFilter")]
    public class DuplicateFilterKeyTests
    {
        private const string DeviceUuid = "U-DUP";
        private const string DeviceName = "dup-device";

        private static MTConnectAgentBroker CreateAgent()
        {
            var agent = new MTConnectAgentBroker(new AgentConfiguration());
            agent.Start();

            var device = new Device { Id = "d-dup", Name = DeviceName, Uuid = DeviceUuid };
            device.AddDataItem(new DataItem { Id = "msg_id", Name = "msg_name", Category = DataItemCategory.EVENT, Type = "MESSAGE", Representation = DataItemRepresentation.VALUE });
            device.AddDataItem(new DataItem { Id = "sys_id", Name = "sys_name", Category = DataItemCategory.CONDITION, Type = "SYSTEM", Representation = DataItemRepresentation.VALUE });
            Assert.That(agent.AddDevice(device), Is.Not.Null);
            return agent;
        }

        /// <summary>Pins the behaviour expressed by the test name: a repeated value is filtered for every key form.</summary>
        [TestCase(DeviceUuid, "msg_id")]
        [TestCase(DeviceName, "msg_name")]
        [TestCase(DeviceUuid, "msg_name")]
        [TestCase(DeviceName, "msg_id")]
        public void Repeated_value_is_filtered(string deviceKey, string dataItemKey)
        {
            using var agent = CreateAgent();

            agent.AddObservation(deviceKey, dataItemKey, (object)"HELLO", DateTime.UtcNow);
            var sequence = agent.LastSequence;

            agent.AddObservation(deviceKey, dataItemKey, (object)"HELLO", DateTime.UtcNow.AddSeconds(1));

            Assert.That(agent.LastSequence, Is.EqualTo(sequence), "An unchanged value must not be stored again.");

            agent.AddObservation(deviceKey, dataItemKey, (object)"WORLD", DateTime.UtcNow.AddSeconds(2));
            Assert.That(agent.LastSequence, Is.EqualTo(sequence + 1), "A changed value must be stored.");
        }

        /// <summary>Pins the behaviour expressed by the test name: a repeated condition is filtered for every key form.</summary>
        [TestCase(DeviceUuid, "sys_id")]
        [TestCase(DeviceName, "sys_name")]
        public void Repeated_condition_is_filtered(string deviceKey, string dataItemKey)
        {
            using var agent = CreateAgent();

            agent.AddObservation(deviceKey, new ConditionFaultStateObservationInput(dataItemKey, ConditionLevel.WARNING) { NativeCode = "A1" });
            var sequence = agent.LastSequence;

            agent.AddObservation(deviceKey, new ConditionFaultStateObservationInput(dataItemKey, ConditionLevel.WARNING) { NativeCode = "A1" });
            Assert.That(agent.LastSequence, Is.EqualTo(sequence), "An unchanged condition must not be stored again.");

            agent.AddObservation(deviceKey, new ConditionFaultStateObservationInput(dataItemKey, ConditionLevel.FAULT) { NativeCode = "A1" });
            Assert.That(agent.LastSequence, Is.EqualTo(sequence + 1), "A changed condition must be stored.");
        }
    }
}
