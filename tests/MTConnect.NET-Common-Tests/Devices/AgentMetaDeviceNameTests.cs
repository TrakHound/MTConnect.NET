// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Devices;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Devices
{
    /// <summary>
    /// Pins the `<c>name</c>` attribute the Agent meta-device exposes when
    /// MTConnect.NET hosts the agent itself (the
    /// <see cref="Agent(MTConnectAgent)"/> constructor path, reached via
    /// <see cref="MTConnectAgent.InitializeAgentDevice"/> when
    /// <c>EnableAgentDevice == true</c>).
    ///
    /// The MTConnect v2.7 DevicesType XSD (lines 5228-5300 of
    /// <c>MTConnectDevices_2.7.xsd</c>) requires every Device/Agent to carry
    /// a `name` attribute of simpleType `NameType` — an unrestricted
    /// `xs:string` with no case-normalisation facet. The cppagent reference
    /// JSON v2 serialiser emits this attribute verbatim from the source XML.
    /// MTConnect.NET previously hard-coded the Agent meta-device's Name to
    /// the lowercase placeholder "agent" inside the
    /// <see cref="Agent(MTConnectAgent)"/> constructor; this diverged from
    /// the cppagent convention of using the Pascal-case "Agent" matching the
    /// element name.
    ///
    /// Reference (cppagent fixtures): mixed-case `name` values such as
    /// `name="LinuxCNC"` survive round-trip without case normalisation.
    /// MTConnect.NET-as-agent does not expose an operator-configurable
    /// agent-device name, so this fixture pins the canonical default value
    /// that matches the cppagent JSON v2 keyed-object envelope.
    /// </summary>
    [TestFixture]
    [Category("AgentMetaDeviceNaming")]
    public class AgentMetaDeviceNameTests
    {
        /// <summary>Pins the behaviour expressed by the test name: agent meta device default name matches cppagent pascal case convention.</summary>
        [Test]
        public void Agent_meta_device_default_Name_matches_cppagent_pascal_case_convention()
        {
            using var mtAgent = new MTConnectAgent(uuid: "test-agent");

            Assert.That(mtAgent.Agent, Is.Not.Null,
                "InitializeAgentDevice must construct the Agent meta-device when EnableAgentDevice is true (default).");
            Assert.That(mtAgent.Agent.Name, Is.EqualTo("Agent"),
                "Agent meta-device Name must default to the Pascal-case 'Agent' "
                + "matching the cppagent JSON v2 element-key convention "
                + "(was previously hard-coded to the lowercase placeholder 'agent', "
                + "diverging from cppagent's verbatim-attribute behaviour).");
        }

        /// <summary>Pins the behaviour expressed by the test name: agent meta device type remains agent type id.</summary>
        [Test]
        public void Agent_meta_device_Type_remains_Agent_TypeId()
        {
            using var mtAgent = new MTConnectAgent(uuid: "test-agent");

            Assert.That(mtAgent.Agent.Type, Is.EqualTo(Agent.TypeId),
                "Agent meta-device Type must remain Agent.TypeId regardless of Name reformatting.");
        }
    }
}
