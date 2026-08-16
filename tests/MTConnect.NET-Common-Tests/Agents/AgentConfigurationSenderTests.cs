// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Net;
using MTConnect.Agents;
using MTConnect.Configurations;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Agents
{
    /// <summary>
    /// Pins the contract that <c>IAgentConfiguration.Sender</c>, when set,
    /// flows through to <see cref="MTConnectAgent.Sender"/> — which populates
    /// the <c>Header/@sender</c> attribute on every emitted MTConnect response
    /// document (see MTConnect Part 1 §7). When the config value is absent,
    /// the pre-existing <see cref="Dns.GetHostName"/> fallback is preserved
    /// bit-for-bit, so hosts that do not opt in see no behavioural change.
    /// </summary>
    [TestFixture]
    public class AgentConfigurationSenderTests
    {
        /// <summary>
        /// When <c>AgentConfiguration.Sender</c> is set, the constructed
        /// <see cref="MTConnectAgent"/> exposes that exact value on its
        /// <see cref="MTConnectAgent.Sender"/> property.
        /// </summary>
        [Test]
        public void Sender_set_in_config_flows_through_to_Agent_Sender()
        {
            const string PinnedSender = "foo-plant-a";

            var configuration = new AgentConfiguration
            {
                Sender = PinnedSender,
            };

            var agent = new MTConnectAgent(
                configuration,
                uuid: "sender-fixture-uuid",
                initializeAgentDevice: false);

            Assert.That(agent.Sender, Is.EqualTo(PinnedSender));
        }

        /// <summary>
        /// When <c>AgentConfiguration.Sender</c> is null or empty, the agent
        /// falls back to <see cref="Dns.GetHostName"/> — matching the
        /// pre-existing behaviour before the config surface was added.
        /// </summary>
        [Test]
        public void Sender_absent_from_config_falls_back_to_Dns_GetHostName()
        {
            var configuration = new AgentConfiguration();

            var agent = new MTConnectAgent(
                configuration,
                uuid: "sender-fallback-fixture-uuid",
                initializeAgentDevice: false);

            Assert.That(agent.Sender, Is.EqualTo(Dns.GetHostName()));
        }
    }
}
