// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Linq;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Input;
using MTConnect.Observations;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Agents
{
    /// <summary>
    /// Pins that adding a batch of observations attempts every item: one observation for an unknown
    /// DataItem no longer drops the observations after it, and the result reports the failure.
    /// </summary>
    [TestFixture]
    [Category("AddObservationsBatch")]
    public class AddObservationsBatchTests
    {
        private const string DeviceKey = "U-BATCH";

        /// <summary>Pins the behaviour expressed by the test name: observations after an invalid one are still added.</summary>
        [Test]
        public void AddObservations_continues_past_an_unknown_data_item()
        {
            using var agent = new MTConnectAgentBroker(new AgentConfiguration());
            agent.Start();

            var device = new Device { Id = "d-batch", Name = "d-batch", Uuid = DeviceKey };
            device.AddDataItem(new DataItem { Id = "msg1", Name = "msg1", Category = DataItemCategory.EVENT, Type = "MESSAGE", Representation = DataItemRepresentation.VALUE });
            device.AddDataItem(new DataItem { Id = "msg2", Name = "msg2", Category = DataItemCategory.EVENT, Type = "MESSAGE", Representation = DataItemRepresentation.VALUE });
            Assert.That(agent.AddDevice(device), Is.Not.Null);

            var inputs = new IObservationInput[]
            {
                new ObservationInput("msg1", "first"),
                new ObservationInput("unknown", "dropped"),
                new ObservationInput("msg2", "second"),
            };

            var result = agent.AddObservations(DeviceKey, inputs);

            Assert.That(result, Is.False, "The batch reports that one observation could not be added.");
            Assert.That(agent.GetCurrentObservations(DeviceKey, "msg1").Single().GetValue(ValueKeys.Result), Is.EqualTo("first"));
            Assert.That(agent.GetCurrentObservations(DeviceKey, "msg2").Single().GetValue(ValueKeys.Result), Is.EqualTo("second"));
        }
    }
}
