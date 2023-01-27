using MTConnect.Clients;
using MTConnect.Observations;
using MTConnect.Observations.Events.Values;
using MTConnect.Streams;
using MTConnect.Tests.Agents;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;

namespace MTConnect.Tests.Http.Clients
{
    public class SampleClient : IDisposable
    {
        private const string _hostname = "localhost";
        private const int _port = 5012;
        private const string _deviceUuid = "OKUMA.Lathe.123456";
        private const string _dataItemId = "L2avail";

        private readonly AgentRunner _agentRunner;
        private readonly MTConnectHttpClient _client;
        private readonly System.Collections.Generic.List<IObservation> _receivedObservations = new System.Collections.Generic.List<IObservation>();


        public SampleClient()
        {
            _agentRunner = new AgentRunner(_hostname, _port);
            _agentRunner.Start();

            _client = new MTConnectHttpClient(_hostname, _port, documentFormat: DocumentFormat.XML);
            _client.OnCurrentReceived += SampleReceived;
            _client.OnSampleReceived += SampleReceived;
            _client.Start();
        }

        public void Dispose()
        {
            _client.Stop();

            _agentRunner.Stop();
            _agentRunner.Dispose();
        }


        private void SampleReceived(object sender, IStreamsResponseDocument response)
        {
            _receivedObservations.AddRange(response.GetObservations());
        }


        [SetUp]
        public void Setup() { }

        [Test]
        public void Run()
        {
            Thread.Sleep(2000); // Allow time for the Client to Start

            var unavailable = _receivedObservations.FirstOrDefault(o => o.DeviceUuid == _deviceUuid && o.DataItemId == _dataItemId);

            if (unavailable != null)
            {
                _agentRunner.Agent.AddObservation(_deviceUuid, _dataItemId, Availability.AVAILABLE);

                Thread.Sleep(500); // Allow time for the Client to Receive the Obsrvation

                var observation = _receivedObservations.Where(o => o.DeviceUuid == _deviceUuid && o.DataItemId == _dataItemId).OrderByDescending(o => o.Timestamp).FirstOrDefault();
                if (observation != null)
                {
                    var result = observation.GetValue(ValueKeys.Result);
                    if (result == Availability.AVAILABLE.ToString()) Assert.Pass($"XML Observation Received Successfully = {result}");
                    else Assert.Fail($"XML Observation Error = {result}");
                }
                else
                {
                    Assert.Fail($"XML Observation Error");
                }
            }
            else Assert.Fail($"XML Unavailable Observation Error");
        }
    }
}