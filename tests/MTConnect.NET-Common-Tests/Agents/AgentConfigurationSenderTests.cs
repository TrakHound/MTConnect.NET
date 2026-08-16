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

        /// <summary>
        /// When <c>AgentConfiguration.Sender</c> is explicitly the empty
        /// string, the agent still falls back to <see cref="Dns.GetHostName"/>
        /// because the guard on the wire-through branch is
        /// <see cref="string.IsNullOrEmpty(string)"/>, which treats null and
        /// the empty string identically.
        /// </summary>
        [Test]
        public void Sender_empty_string_in_config_falls_back_to_Dns_GetHostName()
        {
            var configuration = new AgentConfiguration { Sender = string.Empty };

            var agent = new MTConnectAgent(
                configuration,
                uuid: "sender-empty-fixture-uuid",
                initializeAgentDevice: false);

            Assert.That(agent.Sender, Is.EqualTo(Dns.GetHostName()));
        }

        /// <summary>
        /// A whitespace-only <see cref="AgentConfiguration.Sender"/> is
        /// wire-through verbatim — the constructor guard is
        /// <see cref="string.IsNullOrEmpty(string)"/>, NOT
        /// <c>IsNullOrWhiteSpace</c>, so a whitespace value overrides the
        /// hostname fallback. This test pins the exact null-vs-empty-vs-
        /// whitespace boundary the setter contract carries.
        /// </summary>
        [Test]
        public void Sender_whitespace_only_in_config_is_carried_through_verbatim()
        {
            const string PinnedWhitespace = "   ";
            var configuration = new AgentConfiguration { Sender = PinnedWhitespace };

            var agent = new MTConnectAgent(
                configuration,
                uuid: "sender-whitespace-fixture-uuid",
                initializeAgentDevice: false);

            Assert.That(agent.Sender, Is.EqualTo(PinnedWhitespace));
        }

        /// <summary>
        /// The <see cref="IAgentConfiguration.Sender"/> interface surface
        /// reflects the value set on the concrete
        /// <see cref="AgentConfiguration"/> — the class's writable setter is
        /// the only way to author the value, and the interface's getter
        /// projects it. Pins the polymorphic access path that operator
        /// integrators reach through the interface abstraction.
        /// </summary>
        [Test]
        public void IAgentConfiguration_Sender_getter_reflects_concrete_setter()
        {
            const string PinnedSender = "interface-getter-fixture";
            IAgentConfiguration configuration = new AgentConfiguration
            {
                Sender = PinnedSender
            };

            Assert.That(configuration.Sender, Is.EqualTo(PinnedSender));
        }

        /// <summary>
        /// Constructing the agent with a null <see cref="AgentConfiguration"/>
        /// argument falls through the constructor's default-config branch
        /// and therefore falls back to <see cref="Dns.GetHostName"/> without
        /// throwing — the null-config path is the historically-supported
        /// zero-config bootstrap.
        /// </summary>
        [Test]
        public void Sender_null_configuration_falls_back_to_Dns_GetHostName()
        {
            var agent = new MTConnectAgent(
                (IAgentConfiguration)null,
                uuid: "sender-null-config-fixture-uuid",
                initializeAgentDevice: false);

            Assert.That(agent.Sender, Is.EqualTo(Dns.GetHostName()));
        }
    }
}
