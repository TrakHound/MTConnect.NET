using MTConnect.Clients;
using MTConnect.Tests.Agents;
using NUnit.Framework;
using System.Linq;

namespace MTConnect.Tests.Http.Clients
{
    public class Current
    {
        private const string _hostname = "localhost";
        private const int _port = 5012;
        private const string _deviceName = "OKUMA-Lathe";

        private readonly AgentRunner _agentRunner;


        public Current()
        {
            _agentRunner = new AgentRunner(_hostname, _port);
            _agentRunner.Start();
        }


        [SetUp]
        public void Setup() { }

        [Test]
        public void RunXml()
        {
            var client = new MTConnectHttpCurrentClient(_hostname, _port, documentFormat: DocumentFormat.XML);
            var response = client.Get();
            if (response != null)
            {
                if (response.Streams.Count() == _agentRunner.Devices.Count() + 1)
                {
                    Assert.Pass($"XML Current Response Received Successfully");
                }
                else
                {
                    Assert.Fail($"XML All Devices Not Found");
                }
            }
            else
            {
                Assert.Fail($"XML Error during Current Request");
            }
        }

        [Test]
        public void RunJson()
        {
            var client = new MTConnectHttpCurrentClient(_hostname, _port, documentFormat: DocumentFormat.JSON);
            var response = client.Get();
            if (response != null)
            {
                if (response.Streams.Count() == _agentRunner.Devices.Count() + 1)
                {
                    Assert.Pass($"JSON Current Response Received Successfully");
                }
                else
                {
                    Assert.Fail($"JSON All Devices Not Found");
                }
            }
            else
            {
                Assert.Fail($"JSON Error during Current Request");
            }
        }


        [Test]
        public void RunDeviceXml()
        {
            var client = new MTConnectHttpCurrentClient(_hostname, _port, _deviceName, documentFormat: DocumentFormat.XML);
            var response = client.Get();
            if (response != null)
            {
                var device = response.Streams.FirstOrDefault(o => o.Name == _deviceName);
                if (device != null)
                {
                    Assert.Pass($"XML Current Response Received Successfully for ({_deviceName})");
                }
                else
                {
                    Assert.Fail($"XML Device Not Found for ({_deviceName})");
                }
            }
            else
            {
                Assert.Fail($"XML Error during Current Request");
            }
        }

        [Test]
        public void RunDeviceJson()
        {
            var client = new MTConnectHttpCurrentClient(_hostname, _port, _deviceName, documentFormat: DocumentFormat.JSON);
            var response = client.Get();
            if (response != null)
            {
                var device = response.Streams.FirstOrDefault(o => o.Name == _deviceName);
                if (device != null)
                {
                    Assert.Pass($"JSON Current Response Received Successfully for ({_deviceName})");
                }
                else
                {
                    Assert.Fail($"JSON Device Not Found for ({_deviceName})");
                }
            }
            else
            {
                Assert.Fail($"JSON Error during Current Request");
            }
        }


        [Test]
        public void RunDevicePathXml()
        {
            var client = new MTConnectHttpCurrentClient(_hostname, _port, _deviceName, path: "//DataItem[@type=\"AVAILABILITY\"]", documentFormat: DocumentFormat.XML);
            var response = client.Get();
            if (response != null)
            {
                var device = response.Streams.FirstOrDefault(o => o.Name == _deviceName);
                if (device != null)
                {
                    Assert.Pass($"XML Current Response Received Successfully for ({_deviceName})");
                }
                else
                {
                    Assert.Fail($"XML Device Not Found for ({_deviceName})");
                }
            }
            else
            {
                Assert.Fail($"XML Error during Current Request");
            }
        }

        //[Test]
        //public void RunDevicePathJson()
        //{
        //    var client = new MTConnectCurrentClient(_hostname, _port, _deviceName, path: "//DataItem[@type=\"AVAILABILITY\"]", documentFormat: DocumentFormat.JSON);
        //    var response = client.Get();
        //    if (response != null)
        //    {
        //        var device = response.Streams.FirstOrDefault(o => o.Name == _deviceName);
        //        if (device != null)
        //        {
        //            Assert.Pass($"JSON Current Response Received Successfully for ({_deviceName})");
        //        }
        //        else
        //        {
        //            Assert.Fail($"JSON Device Not Found for ({_deviceName})");
        //        }
        //    }
        //    else
        //    {
        //        Assert.Fail($"JSON Error during Current Request");
        //    }
        //}
    }
}