// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using MTConnect.Adapters;
using NUnit.Framework;

namespace MTConnect.Tests.Shdr
{
    /// <summary>
    /// Pins the multicast-isolation contract for the delegate shapes used by
    /// <c>ShdrAdapter</c> and <c>ShdrClient</c>. Both SHDR classes declare
    /// their events as <see cref="EventHandler{T}"/>; after migration all raise
    /// sites use <see cref="MulticastIsolation.Raise{T}"/> passing <c>null</c>
    /// as the <c>internalError</c> sink (neither class declares an InternalError
    /// event). These tests verify the isolation guarantee holds for every generic
    /// type argument surfaced by those events without requiring a live TCP
    /// connection.
    /// </summary>
    [TestFixture]
    public class ShdrMulticastIsolationTests
    {
        // -----------------------------------------------------------------------
        // EventHandler<string> raise sites
        // (AgentConnected, AgentDisconnected, AgentConnectionError, PingReceived,
        //  PongSent on ShdrAdapter; Connected, Disconnected, Listening,
        //  PingReceived, PingSent, PongReceived, ProtocolReceived, CommandReceived
        //  on ShdrClient)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second EventHandler{string} subscriber fires even when the first throws, covering all string-payload SHDR events.</summary>
        [Test]
        public void Shdr_Generic_EventHandlerOfString_FiresAllSubscribersWhenOneThrows()
        {
            var received = new List<string>();

            EventHandler<string> handler = null;
            handler += (_, _) => throw new InvalidOperationException("first shdr subscriber throws");
            handler += (_, v) => received.Add(v);

            MulticastIsolation.Raise(handler, this, "shdr-payload", null);

            Assert.That(received, Is.EqualTo(new[] { "shdr-payload" }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a string-payload SHDR event is silently swallowed and does not escape to the caller.</summary>
        [Test]
        public void Shdr_Generic_EventHandlerOfString_NullInternalErrorSwallowsFault()
        {
            EventHandler<string> handler = (_, _) => throw new InvalidOperationException("shdr-fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, this, "x", null));
        }

        /// <summary>Pins the behavior expressed by the test name: multiple string-payload subscribers all fire when no subscriber throws, covering the happy path for AgentConnected and related events.</summary>
        [Test]
        public void Shdr_Generic_EventHandlerOfString_AllSubscribersFireOnHappyPath()
        {
            var received = new List<string>();

            EventHandler<string> handler = null;
            handler += (_, v) => received.Add("sub1:" + v);
            handler += (_, v) => received.Add("sub2:" + v);

            MulticastIsolation.Raise(handler, this, "hello", null);

            Assert.That(received, Is.EqualTo(new[] { "sub1:hello", "sub2:hello" }));
        }

        // -----------------------------------------------------------------------
        // EventHandler<AdapterEventArgs<string>> raise sites
        // (LineSent, DataSent, SendError on ShdrAdapter)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second EventHandler{AdapterEventArgs{string}} subscriber fires even when the first throws, covering LineSent, DataSent, and SendError on ShdrAdapter.</summary>
        [Test]
        public void Shdr_Generic_EventHandlerOfAdapterEventArgsString_FiresAllSubscribersWhenOneThrows()
        {
            var received = new List<AdapterEventArgs<string>>();
            var payload = new AdapterEventArgs<string>("client-1", "line");

            EventHandler<AdapterEventArgs<string>> handler = null;
            handler += (_, _) => throw new InvalidOperationException("first LineSent subscriber throws");
            handler += (_, e) => received.Add(e);

            MulticastIsolation.Raise(handler, this, payload, null);

            Assert.That(received.Count, Is.EqualTo(1));
            Assert.That(received[0].ClientId, Is.EqualTo("client-1"));
            Assert.That(received[0].Data, Is.EqualTo("line"));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from an AdapterEventArgs{string} subscriber is swallowed and does not escape.</summary>
        [Test]
        public void Shdr_Generic_EventHandlerOfAdapterEventArgsString_NullInternalErrorSwallowsFault()
        {
            var payload = new AdapterEventArgs<string>("c1", "data");
            EventHandler<AdapterEventArgs<string>> handler = (_, _) => throw new InvalidOperationException("fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, this, payload, null));
        }

        // -----------------------------------------------------------------------
        // EventHandler<AdapterEventArgs<Exception>> raise sites
        // (ConnectionError on ShdrAdapter)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second EventHandler{AdapterEventArgs{Exception}} subscriber fires even when the first throws, covering ConnectionError on ShdrAdapter.</summary>
        [Test]
        public void Shdr_Generic_EventHandlerOfAdapterEventArgsException_FiresAllSubscribersWhenOneThrows()
        {
            var received = new List<AdapterEventArgs<Exception>>();
            var inner = new InvalidOperationException("tcp-error");
            var payload = new AdapterEventArgs<Exception>("client-2", inner);

            EventHandler<AdapterEventArgs<Exception>> handler = null;
            handler += (_, _) => throw new InvalidOperationException("first ConnectionError subscriber throws");
            handler += (_, e) => received.Add(e);

            MulticastIsolation.Raise(handler, this, payload, null);

            Assert.That(received.Count, Is.EqualTo(1));
            Assert.That(received[0].ClientId, Is.EqualTo("client-2"));
            Assert.That(received[0].Data.Message, Is.EqualTo("tcp-error"));
        }

        // -----------------------------------------------------------------------
        // EventHandler<Exception> raise sites
        // (ConnectionError on ShdrClient)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second EventHandler{Exception} subscriber fires even when the first throws, covering ConnectionError on ShdrClient.</summary>
        [Test]
        public void Shdr_Generic_EventHandlerOfException_FiresAllSubscribersWhenOneThrows()
        {
            var received = new List<string>();
            var payload = new InvalidOperationException("connection-refused");

            EventHandler<Exception> handler = null;
            handler += (_, _) => throw new InvalidOperationException("first ConnectionError subscriber throws");
            handler += (_, ex) => received.Add(ex.Message);

            MulticastIsolation.Raise(handler, this, payload, null);

            Assert.That(received, Is.EqualTo(new[] { "connection-refused" }));
        }

        // -----------------------------------------------------------------------
        // Null-handler guard
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: Raise with a null handler is a safe no-op, covering the no-subscriber case for every SHDR event at runtime.</summary>
        [Test]
        public void Shdr_NullGenericHandler_DoesNotThrow()
        {
            EventHandler<string> handler = null;

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, this, "x", null));
        }
    }
}
