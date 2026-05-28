// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Clients;
using MTConnect.Observations;
using MTConnect.Observations.Events;
using MTConnect.Streams;
using MTConnect.Tests.Agents;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MTConnect.Tests.Http.Clients
{
    // Drives the streaming MTConnectHttpClient against the real embedded
    // MTConnectHttpServer started by AgentRunner: it opens a Current + Sample
    // stream, pushes a new observation into the live MTConnectAgentBroker, and
    // asserts the streamed Sample delivers that observation back. This
    // exercises the broker ingest/sequence/buffer path and the HTTP server
    // stream + HTTP streaming-client per-chunk loop end to end.
    [TestFixture]
    public class SampleClient
    {
        private const string Hostname = "127.0.0.1";
        private const string DeviceUuid = "OKUMA.Lathe.123456";
        private const string DataItemId = "L2avail";

        // Generous CI-safe bounds for the streamed delivery round-trip.
        private const int DeliveryTimeoutMs = 30000;
        private const int PollIntervalMs = 100;

        private AgentRunner _agentRunner = null!;
        private MTConnectHttpClient _client = null!;
        private readonly List<IObservation> _receivedObservations = new List<IObservation>();
        private readonly object _lock = new object();


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var port = AgentRunner.GetFreePort();
            _agentRunner = new AgentRunner(port);
            _agentRunner.Start();

            _client = new MTConnectHttpClient(Hostname, port, documentFormat: DocumentFormat.XML);
            _client.CurrentReceived += SampleReceived;
            _client.SampleReceived += SampleReceived;
            _client.Start();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _client.Stop();
            _agentRunner.Stop();
            _agentRunner.Dispose();
        }


        private void SampleReceived(object? sender, IStreamsResponseDocument response)
        {
            lock (_lock)
            {
                _receivedObservations.AddRange(response.GetObservations());
            }
        }

        private IObservation? WaitFor(Func<IObservation, bool> predicate)
        {
            var deadline = DateTime.UtcNow.AddMilliseconds(DeliveryTimeoutMs);
            while (DateTime.UtcNow < deadline)
            {
                lock (_lock)
                {
                    var match = _receivedObservations
                        .Where(predicate)
                        .OrderByDescending(o => o.Timestamp)
                        .FirstOrDefault();
                    if (match != null) return match;
                }

                Thread.Sleep(PollIntervalMs);
            }

            return null;
        }


        [Test]
        public void Run()
        {
            // The initial Current pushes the data item's UNAVAILABLE seed
            // observation over the stream.
            var unavailable = WaitFor(o => o.DeviceUuid == DeviceUuid && o.DataItemId == DataItemId);
            Assert.That(unavailable, Is.Not.Null, "Streamed Current did not deliver the seed observation");

            // Push a new value into the live broker; the open Sample stream
            // must deliver it back to the client.
            var added = _agentRunner.Agent.AddObservation(DeviceUuid, DataItemId, Availability.AVAILABLE);
            Assert.That(added, Is.True, "Broker rejected the AVAILABLE observation");

            var observation = WaitFor(o =>
                o.DeviceUuid == DeviceUuid &&
                o.DataItemId == DataItemId &&
                o.GetValue(ValueKeys.Result) == Availability.AVAILABLE.ToString());

            Assert.That(observation, Is.Not.Null, "Streamed Sample did not deliver the AVAILABLE observation");
            Assert.That(observation!.GetValue(ValueKeys.Result), Is.EqualTo(Availability.AVAILABLE.ToString()));
        }
    }
}
