// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Tests.Agents;
using NUnit.Framework;
using System.Linq;

namespace MTConnect.Tests.Http.Clients
{
    // Shared base for the HTTP client fixtures. Each derived fixture gets its
    // own AgentRunner bound to a distinct OS-assigned loopback port, started
    // and stopped through NUnit's one-time lifecycle. NUnit instantiates every
    // [TestFixture] up front, so a hard-coded shared port would have several
    // embedded HTTP servers contend for the same socket; an OS-assigned port
    // per fixture removes that contention. AgentRunner.Start() blocks until the
    // server actually answers a Probe, so the first client request never races
    // the fire-and-forget socket bind.
    public abstract class HttpClientFixture
    {
        protected const string Hostname = "127.0.0.1";
        protected const string DeviceName = "OKUMA-Lathe";
        protected const string DeviceUuid = "OKUMA.Lathe.123456";

        protected AgentRunner AgentRunner { get; private set; } = null!;

        protected int Port { get; private set; }

        // Streams/devices include the implicit Agent device alongside the two
        // device files AgentRunner loads, so a full document carries one more
        // entry than AgentRunner.Devices.
        protected int ExpectedDocumentEntryCount => AgentRunner.Devices.Count() + 1;


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Port = AgentRunner.GetFreePort();
            AgentRunner = new AgentRunner(Port);
            AgentRunner.Start();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            AgentRunner.Stop();
            AgentRunner.Dispose();
        }
    }
}
