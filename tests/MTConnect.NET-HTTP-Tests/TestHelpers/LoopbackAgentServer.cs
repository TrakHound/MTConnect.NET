// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MTConnect.Agents;
using MTConnect.Clients;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Servers.Http;

namespace MTConnect.Tests.Http.TestHelpers
{
    /// <summary>
    /// An in-process Agent (broker) served by <see cref="MTConnectHttpServer"/> on a free loopback port,
    /// with devices "d1" (DataItems p1, p2) and "d2" (DataItem q1) that already have observations.
    /// </summary>
    internal sealed class LoopbackAgentServer : IDisposable
    {
        public MTConnectAgentBroker Agent { get; }
        public MTConnectHttpServer Server { get; }
        public int Port { get; }
        public string BaseUrl => $"http://127.0.0.1:{Port}";


        public LoopbackAgentServer(Action<HttpServerConfiguration>? configure = null)
        {
            Agent = new MTConnectAgentBroker();
            Agent.AddDevice(CreateDevice("d1", "p1", "p2"));
            Agent.AddDevice(CreateDevice("d2", "q1"));
            Agent.AddObservation("d1", "p1", "program-1");
            Agent.AddObservation("d1", "p2", "program-2");
            Agent.AddObservation("d2", "q1", "program-3");

            Port = GetFreePort();

            var configuration = new HttpServerConfiguration
            {
                Port = Port,
                Server = IPAddress.Loopback.ToString()
            };
            configure?.Invoke(configuration);

            Server = new MTConnectHttpServer(configuration, Agent);
            Server.Start();

            WaitForReady();
        }

        public void Dispose()
        {
            Server.Stop();
            Agent.Stop();
        }


        private static Device CreateDevice(string uuid, params string[] dataItemIds)
        {
            return new Device
            {
                Id = uuid,
                Name = uuid,
                Uuid = uuid,
                DataItems = dataItemIds.Select(id => (IDataItem)new ProgramDataItem { Id = id }).ToList()
            };
        }

        public static int GetFreePort()
        {
            using var probe = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            probe.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            return ((IPEndPoint)probe.LocalEndPoint!).Port;
        }

        private void WaitForReady()
        {
            var deadline = DateTime.UtcNow.AddSeconds(15);
            while (DateTime.UtcNow < deadline)
            {
                try
                {
                    var probe = new MTConnectHttpProbeClient($"127.0.0.1:{Port}") { Timeout = 1000 }.Get();
                    if (probe != null && !probe.Devices.IsNullOrEmpty()) return;
                }
                catch { }

                Thread.Sleep(100);
            }

            throw new TimeoutException("Loopback MTConnect HTTP server did not become ready");
        }
    }
}
