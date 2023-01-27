// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Servers.Http;

namespace MTConnect.Tests.Agents
{
    public class AgentRunner : IDisposable
    {
        private MTConnectAgentBroker _agent;
        private MTConnectHttpServer _httpServer;
        private IEnumerable<IDevice> _devices;

        public MTConnectAgentBroker Agent => _agent;

        public MTConnectHttpServer HttpServer => _httpServer;

        public IEnumerable<IDevice> Devices => _devices;


        public AgentRunner(string hostname, int port = 5000, Version mtconnectVersion = null)
        {
            // Read Agent Configuration File
            var configuration = new HttpAgentConfiguration();
            configuration.Server = hostname;
            configuration.Port = port;
            configuration.DefaultVersion= mtconnectVersion;

            // Create MTConnect Agent
            _agent = new MTConnectAgentBroker(configuration);

            var devices = new List<IDevice>();

            // Read Device Files
            var devicesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "devices");
            var deviceFiles = Directory.GetFiles(devicesDir);
            foreach (var deviceFile in deviceFiles)
            {
                var fileDevices = DeviceConfiguration.FromFile(deviceFile, DocumentFormat.XML);
                if (!fileDevices.IsNullOrEmpty())
                {
                    foreach (var device in fileDevices)
                    {
                        devices.Add(device);
                        _agent.AddDevice(device);
                    }
                }
            }
            _devices = devices;

            // Start the Http Server
            _httpServer = new MTConnectHttpServer(configuration, _agent);
        }

        public void Start()
        {
            _agent.Start();
            _httpServer.Start();
        }

        public void Stop()
        {
            _agent.Stop();
            _httpServer.Stop();
        }

        public void Dispose()
        {
            _httpServer.Dispose();
        }
    }
}