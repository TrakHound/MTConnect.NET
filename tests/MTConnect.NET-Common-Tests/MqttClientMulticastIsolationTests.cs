// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MTConnect.Tests.Common
{
    /// <summary>
    /// Pins the multicast-isolation contract for the delegate shapes used by
    /// <c>MTConnectMqttClient</c> and <c>MTConnectMqttExpandedClient</c>. Both
    /// MQTT client classes declare their events as <see cref="EventHandler{T}"/>
    /// or <see cref="EventHandler"/>; after migration all raise sites use
    /// <see cref="MulticastIsolation.Raise{T}"/> / <see cref="MulticastIsolation.Raise"/>.
    /// These tests verify the isolation guarantee holds for every generic type
    /// argument surfaced by those events (no MQTT broker is required because the
    /// helper contract is independent of the originating class).
    /// </summary>
    [TestFixture]
    public class MqttClientMulticastIsolationTests
    {
        // -----------------------------------------------------------------------
        // Non-generic EventHandler raise sites
        // (ClientStarting, ClientStarted, ClientStopping, ClientStopped,
        //  ResponseReceived on MTConnectMqttClient)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second non-generic subscriber fires even when the first throws, covering the EventHandler raise sites on both MQTT client classes.</summary>
        [Test]
        public void MqttClient_NonGeneric_EventHandler_FiresAllSubscribersWhenOneThrows()
        {
            var fired = new List<int>();
            EventHandler? handler = null;
            handler += (_, _) => fired.Add(1);
            handler += (_, _) => throw new InvalidOperationException("subscriber-2 throws");
            handler += (_, _) => fired.Add(3);

            EventHandler<Exception>? internalError = (_, _) => { };

            MulticastIsolation.Raise(handler!, this, EventArgs.Empty, internalError);

            Assert.That(fired, Is.EqualTo(new[] { 1, 3 }));
        }

        /// <summary>Pins the behavior expressed by the test name: a throwing non-generic subscriber faults are routed through InternalError without interrupting subsequent non-generic subscribers.</summary>
        [Test]
        public void MqttClient_NonGeneric_EventHandler_RoutesFaultToInternalError()
        {
            var routed = new List<Exception>();
            EventHandler handler = (_, _) => throw new InvalidOperationException("mqtt-non-generic-fault");
            EventHandler<Exception> internalError = (_, ex) => routed.Add(ex);

            MulticastIsolation.Raise(handler, this, EventArgs.Empty, internalError);

            Assert.That(routed.Count, Is.EqualTo(1));
            Assert.That(routed[0], Is.InstanceOf<InvalidOperationException>());
            Assert.That(routed[0].Message, Is.EqualTo("mqtt-non-generic-fault"));
        }

        /// <summary>Pins the behavior expressed by the test name: when InternalError itself throws for a non-generic event, later InternalError subscribers still fire and remaining event subscribers are not starved.</summary>
        [Test]
        public void MqttClient_NonGeneric_EventHandler_ThrowingInternalErrorDoesNotStarveRemainingSubscribers()
        {
            var eventFired = new List<int>();
            var errorFired = new List<int>();

            EventHandler? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first subscriber throws");
            handler += (_, _) => eventFired.Add(2);

            EventHandler<Exception>? internalError = null;
            internalError += (_, _) => throw new InvalidOperationException("InternalError handler-1 throws");
            internalError += (_, ex) => errorFired.Add(2);

            MulticastIsolation.Raise(handler!, this, EventArgs.Empty, internalError!);

            Assert.That(eventFired, Is.EqualTo(new[] { 2 }));
            Assert.That(errorFired, Is.EqualTo(new[] { 2 }));
        }

        // -----------------------------------------------------------------------
        // Generic EventHandler<T> raise sites — string payload
        // (AgentConnected / AgentDisconnected on ShdrAdapter; Connected /
        //  Disconnected on ShdrClient — also applies to MQTT's generic events)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second generic subscriber fires even when the first throws, covering EventHandler{string} raise sites.</summary>
        [Test]
        public void MqttClient_Generic_EventHandlerOfString_FiresAllSubscribersWhenOneThrows()
        {
            var received = new List<string>();
            EventHandler<string>? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first throws");
            handler += (_, v) => received.Add(v!);

            MulticastIsolation.Raise(handler!, this, "payload", null);

            Assert.That(received, Is.EqualTo(new[] { "payload" }));
        }

        /// <summary>Pins the behavior expressed by the test name: fault from a generic EventHandler{string} subscriber is routed through InternalError.</summary>
        [Test]
        public void MqttClient_Generic_EventHandlerOfString_RoutesFaultToInternalError()
        {
            var routed = new List<string>();
            EventHandler<string> handler = (_, _) => throw new InvalidOperationException("string-event-fault");
            EventHandler<Exception> internalError = (_, ex) => routed.Add(ex.Message);

            MulticastIsolation.Raise(handler, this, "value", internalError);

            Assert.That(routed, Is.EqualTo(new[] { "string-event-fault" }));
        }

        /// <summary>Pins the behavior expressed by the test name: when InternalError throws for a generic EventHandler{string} event, later InternalError subscribers still fire.</summary>
        [Test]
        public void MqttClient_Generic_EventHandlerOfString_ThrowingInternalErrorIteratesPerSubscriber()
        {
            var seen = new List<int>();
            EventHandler<string> handler = (_, _) => throw new InvalidOperationException("origin");

            EventHandler<Exception>? internalError = null;
            internalError += (_, _) => throw new InvalidOperationException("internalError-1 throws");
            internalError += (_, _) => seen.Add(2);
            internalError += (_, _) => seen.Add(3);

            MulticastIsolation.Raise(handler, this, "x", internalError!);

            Assert.That(seen, Is.EqualTo(new[] { 2, 3 }));
        }

        // -----------------------------------------------------------------------
        // Generic EventHandler<T> raise sites — Exception payload
        // (ConnectionError, InternalError on MTConnectMqttClient /
        //  MTConnectMqttExpandedClient — the error-sink events themselves are
        //  also EventHandler<Exception> multicast, so a throwing ConnectionError
        //  subscriber must not starve later ConnectionError subscribers)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second ConnectionError subscriber fires even when the first throws, covering the EventHandler{Exception} raise pattern.</summary>
        [Test]
        public void MqttClient_Generic_EventHandlerOfException_FiresAllSubscribersWhenOneThrows()
        {
            var received = new List<string>();
            var payload = new InvalidOperationException("upstream-error");

            EventHandler<Exception>? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first ConnectionError subscriber throws");
            handler += (_, ex) => received.Add(ex.Message);

            MulticastIsolation.Raise(handler!, this, payload, null);

            Assert.That(received, Is.EqualTo(new[] { "upstream-error" }));
        }

        // -----------------------------------------------------------------------
        // Null-handler guard
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: Raise with a null handler is a safe no-op covering the no-subscriber case that every MQTT event can hit at runtime.</summary>
        [Test]
        public void MqttClient_NullHandler_DoesNotThrow()
        {
            EventHandler<string>? handler = null;
            EventHandler<Exception>? internalError = null;

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, this, "x", internalError));
        }

        /// <summary>Pins the behavior expressed by the test name: Raise non-generic with a null handler is a safe no-op.</summary>
        [Test]
        public void MqttClient_NullHandlerNonGeneric_DoesNotThrow()
        {
            EventHandler? handler = null;
            EventHandler<Exception>? internalError = null;

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, this, EventArgs.Empty, internalError));
        }
    }
}
