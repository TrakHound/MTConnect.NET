// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using NUnit.Framework;

namespace MTConnect.Tests.Common
{
    /// <summary>
    /// Pins the contract of <see cref="MulticastIsolation"/>: per-delegate
    /// fault isolation on both the outer event and the
    /// <c>internalError</c> sink, with a terminal swallow when the
    /// fault-reporter itself throws.
    /// </summary>
    [TestFixture]
    public class MulticastIsolationTests
    {
        // --- Generic overload --------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: every subscriber on the generic event fires even when an earlier one throws.</summary>
        [Test]
        public void MulticastIsolation_Generic_FiresAllSubscribersWhenOneThrows()
        {
            var fired = new System.Collections.Generic.List<int>();
            EventHandler<int>? handler = null;
            handler += (s, e) => fired.Add(1);
            handler += (s, e) => throw new InvalidOperationException("subscriber-2");
            handler += (s, e) => fired.Add(3);

            EventHandler<Exception> internalError = (s, ex) => { };

            MulticastIsolation.Raise(handler!, this, 42, internalError);

            Assert.That(fired, Is.EqualTo(new[] { 1, 3 }));
        }

        /// <summary>Pins the behavior expressed by the test name: a fault thrown by a subscriber is routed through internalError.</summary>
        [Test]
        public void MulticastIsolation_Generic_RoutesFaultToInternalError()
        {
            var routed = new System.Collections.Generic.List<Exception>();
            EventHandler<int> handler = (s, e) => throw new InvalidOperationException("boom");
            EventHandler<Exception> internalError = (s, ex) => routed.Add(ex);

            MulticastIsolation.Raise(handler, this, 0, internalError);

            Assert.That(routed.Count, Is.EqualTo(1));
            Assert.That(routed[0], Is.InstanceOf<InvalidOperationException>());
            Assert.That(routed[0].Message, Is.EqualTo("boom"));
        }

        /// <summary>Pins the behavior expressed by the test name: the internalError multicast is iterated per subscriber, so a throwing internalError handler does not starve later internalError handlers.</summary>
        [Test]
        public void MulticastIsolation_Generic_InternalErrorIteratesPerSubscriber()
        {
            var seen = new System.Collections.Generic.List<int>();
            EventHandler<int> handler = (s, e) => throw new InvalidOperationException("origin");

            EventHandler<Exception>? internalError = null;
            internalError += (s, ex) => throw new InvalidOperationException("internal-1");
            internalError += (s, ex) => seen.Add(2);
            internalError += (s, ex) => seen.Add(3);

            MulticastIsolation.Raise(handler, this, 0, internalError!);

            Assert.That(seen, Is.EqualTo(new[] { 2, 3 }));
        }

        /// <summary>Pins the behavior expressed by the test name: when every internalError subscriber throws, the helper still returns without escaping an exception.</summary>
        [Test]
        public void MulticastIsolation_Generic_TerminalSwallowsInternalErrorOwnThrow()
        {
            EventHandler<int> handler = (s, e) => throw new InvalidOperationException("origin");
            EventHandler<Exception>? internalError = null;
            internalError += (s, ex) => throw new InvalidOperationException("internal-1");
            internalError += (s, ex) => throw new InvalidOperationException("internal-2");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, this, 0, internalError!));
        }

        // --- Non-generic overload ---------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: every subscriber on the non-generic event fires even when an earlier one throws.</summary>
        [Test]
        public void MulticastIsolation_NonGeneric_FiresAllSubscribersWhenOneThrows()
        {
            var fired = new System.Collections.Generic.List<int>();
            EventHandler? handler = null;
            handler += (s, e) => fired.Add(1);
            handler += (s, e) => throw new InvalidOperationException("subscriber-2");
            handler += (s, e) => fired.Add(3);

            EventHandler<Exception> internalError = (s, ex) => { };

            MulticastIsolation.Raise(handler!, this, EventArgs.Empty, internalError);

            Assert.That(fired, Is.EqualTo(new[] { 1, 3 }));
        }

        /// <summary>Pins the behavior expressed by the test name: a fault thrown by a non-generic subscriber is routed through internalError.</summary>
        [Test]
        public void MulticastIsolation_NonGeneric_RoutesFaultToInternalError()
        {
            var routed = new System.Collections.Generic.List<Exception>();
            EventHandler handler = (s, e) => throw new InvalidOperationException("boom");
            EventHandler<Exception> internalError = (s, ex) => routed.Add(ex);

            MulticastIsolation.Raise(handler, this, EventArgs.Empty, internalError);

            Assert.That(routed.Count, Is.EqualTo(1));
            Assert.That(routed[0], Is.InstanceOf<InvalidOperationException>());
            Assert.That(routed[0].Message, Is.EqualTo("boom"));
        }

        /// <summary>Pins the behavior expressed by the test name: under the non-generic overload, a throwing internalError handler does not starve later internalError handlers.</summary>
        [Test]
        public void MulticastIsolation_NonGeneric_InternalErrorIteratesPerSubscriber()
        {
            var seen = new System.Collections.Generic.List<int>();
            EventHandler handler = (s, e) => throw new InvalidOperationException("origin");

            EventHandler<Exception>? internalError = null;
            internalError += (s, ex) => throw new InvalidOperationException("internal-1");
            internalError += (s, ex) => seen.Add(2);
            internalError += (s, ex) => seen.Add(3);

            MulticastIsolation.Raise(handler, this, EventArgs.Empty, internalError!);

            Assert.That(seen, Is.EqualTo(new[] { 2, 3 }));
        }

        /// <summary>Pins the behavior expressed by the test name: under the non-generic overload, when every internalError subscriber throws, the helper still returns without escaping an exception.</summary>
        [Test]
        public void MulticastIsolation_NonGeneric_TerminalSwallowsInternalErrorOwnThrow()
        {
            EventHandler handler = (s, e) => throw new InvalidOperationException("origin");
            EventHandler<Exception>? internalError = null;
            internalError += (s, ex) => throw new InvalidOperationException("internal-1");
            internalError += (s, ex) => throw new InvalidOperationException("internal-2");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, this, EventArgs.Empty, internalError!));
        }
    }
}
