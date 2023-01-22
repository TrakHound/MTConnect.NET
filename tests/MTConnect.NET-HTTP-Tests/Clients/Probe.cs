using MTConnect.Clients;
using MTConnect.Tests.Agents;
using NUnit.Framework;
using System.Linq;

namespace MTConnect.Tests.Http.Clients
{
    public class Probe
    {
        private const string _hostname = "localhost";
        private const int _port = 5012;
        private const string _deviceName = "OKUMA-Lathe";

        private readonly AgentRunner _agentRunner;


        public Probe()
        {
            _agentRunner = new AgentRunner(_hostname, _port);
            _agentRunner.Start();
        }


        [SetUp]
        public void Setup() { }

        [Test]
        public void RunXml()
        {
            var client = new MTConnectHttpProbeClient(_hostname, _port, documentFormat: DocumentFormat.XML);
            var response = client.Get();
            if (response != null)
            {
                if (response.Devices.Count() == _agentRunner.Devices.Count() + 1)
                {
                    Assert.Pass($"XML Probe Response Received Successfully");
                }
                else
                {
                    Assert.Fail($"XML All Devices Not Found");
                }
            }
            else
            {
                Assert.Fail($"XML Error during Probe Request");
            }
        }

        [Test]
        public void RunJson()
        {
            var client = new MTConnectHttpProbeClient(_hostname, _port, documentFormat: DocumentFormat.JSON);
            var response = client.Get();
            if (response != null)
            {
                if (response.Devices.Count() == _agentRunner.Devices.Count() + 1)
                {
                    Assert.Pass($"JSON Probe Response Received Successfully");
                }
                else
                {
                    Assert.Fail($"JSON All Devices Not Found");
                }
            }
            else
            {
                Assert.Fail($"JSON Error during Probe Request");
            }
        }


        [Test]
        public void RunDeviceXml()
        {
            var client = new MTConnectHttpProbeClient(_hostname, _port, _deviceName, documentFormat: DocumentFormat.XML);
            var response = client.Get();
            if (response != null)
            {
                var device = response.Devices.FirstOrDefault(o => o.Name == _deviceName);
                if (device != null)
                {
                    Assert.Pass($"XML Probe Response Received Successfully for ({_deviceName})");
                }
                else
                {
                    Assert.Fail($"XML Device Not Found for ({_deviceName})");
                }
            }
            else
            {
                Assert.Fail($"XML Error during Probe Request");
            }
        }

        [Test]
        public void RunDeviceJson()
        {
            var client = new MTConnectHttpProbeClient(_hostname, _port, _deviceName, documentFormat: DocumentFormat.JSON);
            var response = client.Get();
            if (response != null)
            {
                var device = response.Devices.FirstOrDefault(o => o.Name == _deviceName);
                if (device != null)
                {
                    Assert.Pass($"JSON Probe Response Received Successfully for ({_deviceName})");
                }
                else
                {
                    Assert.Fail($"JSON Device Not Found for ({_deviceName})");
                }
            }
            else
            {
                Assert.Fail($"JSON Error during Probe Request");
            }
        }
    }
}