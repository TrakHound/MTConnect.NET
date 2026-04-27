// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using NUnit.Framework;

namespace MTConnect.AgentModule.MqttRelay.Tests
{
    /// <summary>
    /// Pins the MqttRelay module shutdown policy. Prior to this guard
    /// the module's <c>OnStop()</c> unconditionally invoked
    /// <c>_documentServer.Stop()</c>. When the module was configured
    /// with <c>TopicStructure=Entity</c> only <c>_entityServer</c> was
    /// constructed, so <c>_documentServer</c> was <c>null</c> and the
    /// invocation threw a <see cref="System.NullReferenceException"/>
    /// during agent shutdown. The lifecycle helper centralises the
    /// null-guard so the policy is unit-testable without standing up an
    /// MTConnect agent broker.
    ///
    /// Background: the bug was first observed when an Entity-mode relay
    /// was stopped as part of a graceful agent shutdown and the NRE
    /// surfaced in the host service event log, masking the real shutdown
    /// reason.
    /// </summary>
    [TestFixture]
    public class MqttRelayLifecycleStopTests
    {
        [Test]
        public void StopServers_does_not_throw_when_both_servers_null()
        {
            // Entity-mode plus a constructor that did not initialise
            // either server is the worst case; the helper must be a
            // total function over (null, null).
            Assert.DoesNotThrow(
                () => MqttRelayLifecycle.StopServers(documentStop: null, entityStop: null));
        }

        [Test]
        public void StopServers_invokes_document_stop_when_provided()
        {
            var documentStopped = false;
            MqttRelayLifecycle.StopServers(
                documentStop: () => documentStopped = true,
                entityStop: null);

            Assert.That(documentStopped, Is.True,
                "Document-mode shutdown must invoke the document-server stop action.");
        }

        [Test]
        public void StopServers_invokes_entity_stop_when_provided()
        {
            var entityStopped = false;
            MqttRelayLifecycle.StopServers(
                documentStop: null,
                entityStop: () => entityStopped = true);

            Assert.That(entityStopped, Is.True,
                "Entity-mode shutdown must invoke the entity-server stop action.");
        }

        [Test]
        public void StopServers_invokes_both_when_both_provided()
        {
            var documentStopped = false;
            var entityStopped = false;

            MqttRelayLifecycle.StopServers(
                documentStop: () => documentStopped = true,
                entityStop: () => entityStopped = true);

            Assert.That(documentStopped, Is.True);
            Assert.That(entityStopped, Is.True);
        }

        [Test]
        public void StopServers_swallows_document_stop_exception_and_runs_entity_stop()
        {
            // A throwing document-server stop must not prevent the
            // entity-server stop from running; otherwise a partial
            // shutdown leaks live handlers.
            var entityStopped = false;

            Assert.DoesNotThrow(() => MqttRelayLifecycle.StopServers(
                documentStop: () => throw new System.InvalidOperationException("doc"),
                entityStop: () => entityStopped = true));

            Assert.That(entityStopped, Is.True);
        }
    }
}
