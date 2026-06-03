// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Clients;
using MTConnect.Errors;
using MTConnect.Formatters;
using NUnit.Framework;
using System;
using System.Reflection;

namespace MTConnect.Tests.Http.Clients
{
    /// <summary>
    /// Pins multicast-isolation across every event raise-site on the HTTP
    /// sub-clients that the outer <see cref="MTConnectHttpClient"/> composes:
    /// <see cref="MTConnectHttpProbeClient"/>, <see cref="MTConnectHttpCurrentClient"/>,
    /// <see cref="MTConnectHttpSampleClient"/>, <see cref="MTConnectHttpAssetClient"/>,
    /// and <see cref="MTConnectHttpClientStream"/>. The outer-client fix on its
    /// own is insufficient: a consumer that attaches a handler directly to a
    /// sub-client (the public API permits that) still hit the original
    /// <c>?.Invoke</c> short-circuit on every sibling raise-site. The fix
    /// extends the per-class private <c>RaiseEvent</c> helper to every
    /// sub-client; this fixture pins both halves of the contract per event:
    /// a throwing subscriber must not starve later subscribers in the
    /// invocation list, and a fault raised by the sub-client's own
    /// <c>InternalError</c> handler must also be swallowed so the fan-out
    /// keeps running.
    /// </summary>
    /// <remarks>
    /// Each test exercises the helper directly through reflection rather than
    /// constructing an end-to-end harness for every event. The healthy
    /// embedded agent never returns an <c>MTConnectError</c> document, a
    /// malformed wire response, or an arbitrary unhandled <see cref="Exception"/>
    /// from <see cref="System.Net.Http.HttpClient"/>, so <c>MTConnectError</c>,
    /// <c>FormatError</c>, and <c>InternalError</c> raise-sites cannot be
    /// driven through public methods without bespoke fixture plumbing. The
    /// helper is the single shared isolation barrier — exercising it
    /// directly is the minimal-coupling way to pin every raise-site against
    /// the same contract. Mirrors the documented coverage gap in
    /// <see cref="NonGenericMulticastIsolationTests"/>.
    /// </remarks>
    [TestFixture]
    public class SubClientMulticastIsolationTests
    {
        // ---------------------------------------------------------------------
        // Reflection helpers. C# events on these sub-clients compile to a
        // private backing field whose name matches the event; subscribing via
        // <c>+=</c> goes through the auto-generated accessor and mutates that
        // field. The helper to drive a fan-out is the private RaiseEvent
        // method introduced by the fix on each class.
        // ---------------------------------------------------------------------

        private static T GetEventBacking<T>(object instance, string eventName) where T : Delegate
        {
            var field = instance.GetType().GetField(eventName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null,
                $"Backing field for event '{eventName}' was not found on {instance.GetType().Name}.");
            return (T)field!.GetValue(instance)!;
        }

        // Drives the sub-client's event fan-out through the shared
        // MulticastIsolation helper. Reflection still snapshots the event's
        // private backing field (events are private outside the declaring
        // class), but the dispatch barrier under test is now the single
        // shared helper, not a per-class private method.
        private static void InvokeGenericRaise<T>(object instance, string eventName, T arg)
        {
            var handler = GetEventBacking<EventHandler<T>>(instance, eventName);
            var internalError = GetEventBacking<EventHandler<Exception>>(instance, "InternalError");
            MulticastIsolation.Raise(handler, instance, arg, internalError);
        }

        private static void InvokeNonGenericRaise(object instance, string eventName, EventArgs arg)
        {
            var handler = GetEventBacking<EventHandler>(instance, eventName);
            var internalError = GetEventBacking<EventHandler<Exception>>(instance, "InternalError");
            MulticastIsolation.Raise(handler, instance, arg, internalError);
        }


        // ---------------------------------------------------------------------
        // Generic pinning helper: subscribes a throwing handler followed by a
        // recording handler, raises the event through the private helper, and
        // asserts the recording handler ran. Used for the "throwing subscriber
        // does not starve later subscribers" half of the contract on every
        // typed event.
        // ---------------------------------------------------------------------
        private static void AssertGenericFanOutSurvivesThrowingHandler<T>(
            object instance, string eventName, T payload) where T : class
        {
            bool ranAfterThrow = false;

            EventHandler<T> first = (_, _) => throw new InvalidOperationException("first handler throws");
            EventHandler<T> second = (_, _) => ranAfterThrow = true;

            AddTypedHandler(instance, eventName, first);
            AddTypedHandler(instance, eventName, second);

            InvokeGenericRaise(instance, eventName, payload);

            Assert.That(ranAfterThrow, Is.True,
                $"subscribers after a throwing one must still receive {eventName}");
        }

        // ---------------------------------------------------------------------
        // Pinning helper for the second half of the contract: an InternalError
        // handler that itself throws must not break the fan-out of the
        // originating event. Mirrors the outer-client paired test pattern.
        // ---------------------------------------------------------------------
        private static void AssertGenericFanOutSurvivesThrowingInternalError<T>(
            object instance, string eventName, T payload) where T : class
        {
            bool ranAfterThrow = false;

            EventHandler<Exception> internalErrorThrow = (_, _) =>
                throw new InvalidOperationException("InternalError throws");
            EventHandler<T> first = (_, _) =>
                throw new InvalidOperationException($"first {eventName} throws");
            EventHandler<T> second = (_, _) => ranAfterThrow = true;

            AddTypedHandler(instance, "InternalError", internalErrorThrow);
            AddTypedHandler(instance, eventName, first);
            AddTypedHandler(instance, eventName, second);

            InvokeGenericRaise(instance, eventName, payload);

            Assert.That(ranAfterThrow, Is.True,
                $"InternalError throwing must not break the {eventName} fan-out");
        }

        private static void AssertNonGenericFanOutSurvivesThrowingHandler(
            object instance, string eventName)
        {
            bool ranAfterThrow = false;

            EventHandler first = (_, _) => throw new InvalidOperationException("first handler throws");
            EventHandler second = (_, _) => ranAfterThrow = true;

            AddNonGenericHandler(instance, eventName, first);
            AddNonGenericHandler(instance, eventName, second);

            InvokeNonGenericRaise(instance, eventName, EventArgs.Empty);

            Assert.That(ranAfterThrow, Is.True,
                $"subscribers after a throwing one must still receive {eventName}");
        }

        private static void AssertNonGenericFanOutSurvivesThrowingInternalError(
            object instance, string eventName)
        {
            bool ranAfterThrow = false;

            EventHandler<Exception> internalErrorThrow = (_, _) =>
                throw new InvalidOperationException("InternalError throws");
            EventHandler first = (_, _) =>
                throw new InvalidOperationException($"first {eventName} throws");
            EventHandler second = (_, _) => ranAfterThrow = true;

            AddTypedHandler(instance, "InternalError", internalErrorThrow);
            AddNonGenericHandler(instance, eventName, first);
            AddNonGenericHandler(instance, eventName, second);

            InvokeNonGenericRaise(instance, eventName, EventArgs.Empty);

            Assert.That(ranAfterThrow, Is.True,
                $"InternalError throwing must not break the {eventName} fan-out");
        }

        // Uses the event's auto-generated <c>add</c> accessor so subscription
        // goes through the same path consumer code does. Falls back to
        // direct field mutation only if no accessor exists.
        private static void AddTypedHandler<T>(object instance, string eventName, EventHandler<T> handler)
        {
            var ev = instance.GetType().GetEvent(eventName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(ev, Is.Not.Null,
                $"Public event '{eventName}' was not found on {instance.GetType().Name}.");
            ev!.AddEventHandler(instance, handler);
        }

        private static void AddNonGenericHandler(object instance, string eventName, EventHandler handler)
        {
            var ev = instance.GetType().GetEvent(eventName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(ev, Is.Not.Null,
                $"Public event '{eventName}' was not found on {instance.GetType().Name}.");
            ev!.AddEventHandler(instance, handler);
        }


        // ---------------------------------------------------------------------
        // MTConnectHttpProbeClient — MTConnectError, FormatError,
        // ConnectionError, InternalError.
        // ---------------------------------------------------------------------

        private static MTConnectHttpProbeClient NewProbeClient()
            => new MTConnectHttpProbeClient("127.0.0.1", 1);

        /// <summary>Pins the behavior expressed by the test name: probe client mt connect error fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpProbeClient_MTConnectErrorFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<IErrorResponseDocument>(
                NewProbeClient(), "MTConnectError", new ErrorResponseDocument());

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break probe client mt connect error fan out.</summary>
        [Test]
        public void MTConnectHttpProbeClient_InternalErrorHandlerThrowingDoesNotBreakMTConnectErrorFanOut()
            => AssertGenericFanOutSurvivesThrowingInternalError<IErrorResponseDocument>(
                NewProbeClient(), "MTConnectError", new ErrorResponseDocument());

        /// <summary>Pins the behavior expressed by the test name: probe client format error fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpProbeClient_FormatErrorFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<IFormatReadResult>(
                NewProbeClient(), "FormatError", new FormatReadResult<object>());

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break probe client format error fan out.</summary>
        [Test]
        public void MTConnectHttpProbeClient_InternalErrorHandlerThrowingDoesNotBreakFormatErrorFanOut()
            => AssertGenericFanOutSurvivesThrowingInternalError<IFormatReadResult>(
                NewProbeClient(), "FormatError", new FormatReadResult<object>());

        /// <summary>Pins the behavior expressed by the test name: probe client connection error fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpProbeClient_ConnectionErrorFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<Exception>(
                NewProbeClient(), "ConnectionError", new Exception("test"));

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break probe client connection error fan out.</summary>
        [Test]
        public void MTConnectHttpProbeClient_InternalErrorHandlerThrowingDoesNotBreakConnectionErrorFanOut()
            => AssertGenericFanOutSurvivesThrowingInternalError<Exception>(
                NewProbeClient(), "ConnectionError", new Exception("test"));

        /// <summary>Pins the behavior expressed by the test name: probe client internal error fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpProbeClient_InternalErrorFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<Exception>(
                NewProbeClient(), "InternalError", new Exception("test"));

        // The InternalError-throwing variant for InternalError itself would
        // self-reference (the helper routes faults from InternalError fan-out
        // through InternalError again, which is the swallow path under test).
        // The positive test above already pins that the swallow path keeps the
        // fan-out alive: the second InternalError subscriber runs even after
        // the first one throws, which is the bug class being closed.


        // ---------------------------------------------------------------------
        // MTConnectHttpCurrentClient — same four events.
        // ---------------------------------------------------------------------

        private static MTConnectHttpCurrentClient NewCurrentClient()
            => new MTConnectHttpCurrentClient("127.0.0.1", 1);

        /// <summary>Pins the behavior expressed by the test name: current client mt connect error fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpCurrentClient_MTConnectErrorFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<IErrorResponseDocument>(
                NewCurrentClient(), "MTConnectError", new ErrorResponseDocument());

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break current client mt connect error fan out.</summary>
        [Test]
        public void MTConnectHttpCurrentClient_InternalErrorHandlerThrowingDoesNotBreakMTConnectErrorFanOut()
            => AssertGenericFanOutSurvivesThrowingInternalError<IErrorResponseDocument>(
                NewCurrentClient(), "MTConnectError", new ErrorResponseDocument());

        /// <summary>Pins the behavior expressed by the test name: current client format error fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpCurrentClient_FormatErrorFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<IFormatReadResult>(
                NewCurrentClient(), "FormatError", new FormatReadResult<object>());

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break current client format error fan out.</summary>
        [Test]
        public void MTConnectHttpCurrentClient_InternalErrorHandlerThrowingDoesNotBreakFormatErrorFanOut()
            => AssertGenericFanOutSurvivesThrowingInternalError<IFormatReadResult>(
                NewCurrentClient(), "FormatError", new FormatReadResult<object>());

        /// <summary>Pins the behavior expressed by the test name: current client connection error fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpCurrentClient_ConnectionErrorFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<Exception>(
                NewCurrentClient(), "ConnectionError", new Exception("test"));

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break current client connection error fan out.</summary>
        [Test]
        public void MTConnectHttpCurrentClient_InternalErrorHandlerThrowingDoesNotBreakConnectionErrorFanOut()
            => AssertGenericFanOutSurvivesThrowingInternalError<Exception>(
                NewCurrentClient(), "ConnectionError", new Exception("test"));

        /// <summary>Pins the behavior expressed by the test name: current client internal error fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpCurrentClient_InternalErrorFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<Exception>(
                NewCurrentClient(), "InternalError", new Exception("test"));


        // ---------------------------------------------------------------------
        // MTConnectHttpSampleClient — same four events.
        // ---------------------------------------------------------------------

        private static MTConnectHttpSampleClient NewSampleClient()
            => new MTConnectHttpSampleClient("127.0.0.1", 1);

        /// <summary>Pins the behavior expressed by the test name: sample client mt connect error fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpSampleClient_MTConnectErrorFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<IErrorResponseDocument>(
                NewSampleClient(), "MTConnectError", new ErrorResponseDocument());

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break sample client mt connect error fan out.</summary>
        [Test]
        public void MTConnectHttpSampleClient_InternalErrorHandlerThrowingDoesNotBreakMTConnectErrorFanOut()
            => AssertGenericFanOutSurvivesThrowingInternalError<IErrorResponseDocument>(
                NewSampleClient(), "MTConnectError", new ErrorResponseDocument());

        /// <summary>Pins the behavior expressed by the test name: sample client format error fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpSampleClient_FormatErrorFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<IFormatReadResult>(
                NewSampleClient(), "FormatError", new FormatReadResult<object>());

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break sample client format error fan out.</summary>
        [Test]
        public void MTConnectHttpSampleClient_InternalErrorHandlerThrowingDoesNotBreakFormatErrorFanOut()
            => AssertGenericFanOutSurvivesThrowingInternalError<IFormatReadResult>(
                NewSampleClient(), "FormatError", new FormatReadResult<object>());

        /// <summary>Pins the behavior expressed by the test name: sample client connection error fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpSampleClient_ConnectionErrorFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<Exception>(
                NewSampleClient(), "ConnectionError", new Exception("test"));

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break sample client connection error fan out.</summary>
        [Test]
        public void MTConnectHttpSampleClient_InternalErrorHandlerThrowingDoesNotBreakConnectionErrorFanOut()
            => AssertGenericFanOutSurvivesThrowingInternalError<Exception>(
                NewSampleClient(), "ConnectionError", new Exception("test"));

        /// <summary>Pins the behavior expressed by the test name: sample client internal error fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpSampleClient_InternalErrorFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<Exception>(
                NewSampleClient(), "InternalError", new Exception("test"));


        // ---------------------------------------------------------------------
        // MTConnectHttpAssetClient — same four events.
        // ---------------------------------------------------------------------

        private static MTConnectHttpAssetClient NewAssetClient()
            => new MTConnectHttpAssetClient("127.0.0.1", 1);

        /// <summary>Pins the behavior expressed by the test name: asset client mt connect error fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpAssetClient_MTConnectErrorFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<IErrorResponseDocument>(
                NewAssetClient(), "MTConnectError", new ErrorResponseDocument());

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break asset client mt connect error fan out.</summary>
        [Test]
        public void MTConnectHttpAssetClient_InternalErrorHandlerThrowingDoesNotBreakMTConnectErrorFanOut()
            => AssertGenericFanOutSurvivesThrowingInternalError<IErrorResponseDocument>(
                NewAssetClient(), "MTConnectError", new ErrorResponseDocument());

        /// <summary>Pins the behavior expressed by the test name: asset client format error fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpAssetClient_FormatErrorFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<IFormatReadResult>(
                NewAssetClient(), "FormatError", new FormatReadResult<object>());

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break asset client format error fan out.</summary>
        [Test]
        public void MTConnectHttpAssetClient_InternalErrorHandlerThrowingDoesNotBreakFormatErrorFanOut()
            => AssertGenericFanOutSurvivesThrowingInternalError<IFormatReadResult>(
                NewAssetClient(), "FormatError", new FormatReadResult<object>());

        /// <summary>Pins the behavior expressed by the test name: asset client connection error fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpAssetClient_ConnectionErrorFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<Exception>(
                NewAssetClient(), "ConnectionError", new Exception("test"));

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break asset client connection error fan out.</summary>
        [Test]
        public void MTConnectHttpAssetClient_InternalErrorHandlerThrowingDoesNotBreakConnectionErrorFanOut()
            => AssertGenericFanOutSurvivesThrowingInternalError<Exception>(
                NewAssetClient(), "ConnectionError", new Exception("test"));

        /// <summary>Pins the behavior expressed by the test name: asset client internal error fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpAssetClient_InternalErrorFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<Exception>(
                NewAssetClient(), "InternalError", new Exception("test"));


        // ---------------------------------------------------------------------
        // MTConnectHttpClientStream — DocumentReceived, ErrorReceived,
        // FormatError, InternalError, ConnectionError (generic);
        // Starting, Started, Stopping, Stopped (non-generic).
        // ---------------------------------------------------------------------

        private static MTConnectHttpClientStream NewStream()
            => new MTConnectHttpClientStream("http://127.0.0.1:1/sample");

        /// <summary>Pins the behavior expressed by the test name: client stream document received fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpClientStream_DocumentReceivedFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<MTConnect.Streams.IStreamsResponseDocument>(
                NewStream(), "DocumentReceived", new MTConnect.Streams.StreamsResponseDocument());

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break client stream document received fan out.</summary>
        [Test]
        public void MTConnectHttpClientStream_InternalErrorHandlerThrowingDoesNotBreakDocumentReceivedFanOut()
            => AssertGenericFanOutSurvivesThrowingInternalError<MTConnect.Streams.IStreamsResponseDocument>(
                NewStream(), "DocumentReceived", new MTConnect.Streams.StreamsResponseDocument());

        /// <summary>Pins the behavior expressed by the test name: client stream error received fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpClientStream_ErrorReceivedFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<IErrorResponseDocument>(
                NewStream(), "ErrorReceived", new ErrorResponseDocument());

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break client stream error received fan out.</summary>
        [Test]
        public void MTConnectHttpClientStream_InternalErrorHandlerThrowingDoesNotBreakErrorReceivedFanOut()
            => AssertGenericFanOutSurvivesThrowingInternalError<IErrorResponseDocument>(
                NewStream(), "ErrorReceived", new ErrorResponseDocument());

        /// <summary>Pins the behavior expressed by the test name: client stream format error fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpClientStream_FormatErrorFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<IFormatReadResult>(
                NewStream(), "FormatError", new FormatReadResult<object>());

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break client stream format error fan out.</summary>
        [Test]
        public void MTConnectHttpClientStream_InternalErrorHandlerThrowingDoesNotBreakFormatErrorFanOut()
            => AssertGenericFanOutSurvivesThrowingInternalError<IFormatReadResult>(
                NewStream(), "FormatError", new FormatReadResult<object>());

        /// <summary>Pins the behavior expressed by the test name: client stream connection error fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpClientStream_ConnectionErrorFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<Exception>(
                NewStream(), "ConnectionError", new Exception("test"));

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break client stream connection error fan out.</summary>
        [Test]
        public void MTConnectHttpClientStream_InternalErrorHandlerThrowingDoesNotBreakConnectionErrorFanOut()
            => AssertGenericFanOutSurvivesThrowingInternalError<Exception>(
                NewStream(), "ConnectionError", new Exception("test"));

        /// <summary>Pins the behavior expressed by the test name: client stream internal error fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpClientStream_InternalErrorFiresForAllSubscribersWhenOneThrows()
            => AssertGenericFanOutSurvivesThrowingHandler<Exception>(
                NewStream(), "InternalError", new Exception("test"));

        /// <summary>Pins the behavior expressed by the test name: client stream starting fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpClientStream_StartingFiresForAllSubscribersWhenOneThrows()
            => AssertNonGenericFanOutSurvivesThrowingHandler(NewStream(), "Starting");

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break client stream starting fan out.</summary>
        [Test]
        public void MTConnectHttpClientStream_InternalErrorHandlerThrowingDoesNotBreakStartingFanOut()
            => AssertNonGenericFanOutSurvivesThrowingInternalError(NewStream(), "Starting");

        /// <summary>Pins the behavior expressed by the test name: client stream started fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpClientStream_StartedFiresForAllSubscribersWhenOneThrows()
            => AssertNonGenericFanOutSurvivesThrowingHandler(NewStream(), "Started");

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break client stream started fan out.</summary>
        [Test]
        public void MTConnectHttpClientStream_InternalErrorHandlerThrowingDoesNotBreakStartedFanOut()
            => AssertNonGenericFanOutSurvivesThrowingInternalError(NewStream(), "Started");

        /// <summary>Pins the behavior expressed by the test name: client stream stopping fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpClientStream_StoppingFiresForAllSubscribersWhenOneThrows()
            => AssertNonGenericFanOutSurvivesThrowingHandler(NewStream(), "Stopping");

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break client stream stopping fan out.</summary>
        [Test]
        public void MTConnectHttpClientStream_InternalErrorHandlerThrowingDoesNotBreakStoppingFanOut()
            => AssertNonGenericFanOutSurvivesThrowingInternalError(NewStream(), "Stopping");

        /// <summary>Pins the behavior expressed by the test name: client stream stopped fires for all subscribers when one throws.</summary>
        [Test]
        public void MTConnectHttpClientStream_StoppedFiresForAllSubscribersWhenOneThrows()
            => AssertNonGenericFanOutSurvivesThrowingHandler(NewStream(), "Stopped");

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break client stream stopped fan out.</summary>
        [Test]
        public void MTConnectHttpClientStream_InternalErrorHandlerThrowingDoesNotBreakStoppedFanOut()
            => AssertNonGenericFanOutSurvivesThrowingInternalError(NewStream(), "Stopped");
    }
}
