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
    /// <summary>Represents the http client fixture.</summary>
    public abstract class HttpClientFixture
    {
        /// <summary>The hostname.</summary>
        protected const string Hostname = "127.0.0.1";
        /// <summary>The device name.</summary>
        protected const string DeviceName = "OKUMA-Lathe";
        /// <summary>The device uuid.</summary>
        protected const string DeviceUuid = "OKUMA.Lathe.123456";

        /// <summary>Gets or sets the agent runner.</summary>
        protected AgentRunner AgentRunner { get; private set; } = null!;

        /// <summary>Gets or sets the port.</summary>
        protected int Port { get; private set; }

        // Streams/devices include the implicit Agent device alongside the two
        // device files AgentRunner loads, so a full document carries one more
        // entry than AgentRunner.Devices.
        /// <summary>Gets or sets the expected document entry count.</summary>
        protected int ExpectedDocumentEntryCount => AgentRunner.Devices.Count() + 1;


        /// <summary>Sets up the fixture before each test.</summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Port = AgentRunner.GetFreePort();
            AgentRunner = new AgentRunner(Port);
            AgentRunner.Start();
        }

        /// <summary>Tears down the fixture after each test.</summary>
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            AgentRunner.Stop();
            AgentRunner.Dispose();
        }
    }
}
