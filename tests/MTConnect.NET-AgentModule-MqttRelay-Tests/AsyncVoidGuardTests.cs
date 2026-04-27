// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MTConnect.AgentModule.MqttRelay.Tests
{
    /// <summary>
    /// Pins the MqttRelay async-void event-handler safety policy. The
    /// module exposes seven event handlers that the agent broker
    /// invokes as <c>async void</c>:
    ///
    ///   AgentDeviceAdded, AgentObservationAdded, AgentAssetAdded,
    ///   ProbeReceived,   CurrentReceived,        SampleReceived,
    ///   AssetReceived
    ///
    /// An <c>async void</c> method that throws routes its exception to
    /// the synchronization context, which on the ThreadPool tears down
    /// the host process. Three of the seven handlers (AgentDeviceAdded,
    /// AgentObservationAdded, AgentAssetAdded) had no top-level
    /// try/catch; the other four guarded only the inner publish. A
    /// throw from the formatter or from a synchronous broker call
    /// (DataItem null-deref, broker shutting down) crashed the agent.
    ///
    /// AsyncVoidGuard.Run wraps the handler body and routes any
    /// exception to a logging callback so the policy is enforced
    /// uniformly and is unit-testable. Pinning the policy here also
    /// prevents a future contributor from re-introducing an unguarded
    /// async-void path.
    /// </summary>
    [TestFixture]
    public class AsyncVoidGuardTests
    {
        [Test]
        public async Task Run_completes_normally_when_body_succeeds()
        {
            string loggedFault = null;

            await AsyncVoidGuard.Run(
                async () => { await Task.Yield(); },
                ex => loggedFault = ex.Message);

            Assert.That(loggedFault, Is.Null,
                "Successful body must not invoke the fault logger.");
        }

        [Test]
        public async Task Run_routes_synchronous_throw_to_logger()
        {
            string loggedFault = null;

            await AsyncVoidGuard.Run(
                () => throw new InvalidOperationException("sync throw"),
                ex => loggedFault = ex.Message);

            Assert.That(loggedFault, Is.EqualTo("sync throw"));
        }

        [Test]
        public async Task Run_routes_async_throw_to_logger()
        {
            string loggedFault = null;

            await AsyncVoidGuard.Run(
                async () => { await Task.Yield(); throw new InvalidOperationException("async throw"); },
                ex => loggedFault = ex.Message);

            Assert.That(loggedFault, Is.EqualTo("async throw"));
        }

        [Test]
        public async Task Run_does_not_rethrow_when_logger_is_null()
        {
            // Even with a null logger, the guard must swallow the
            // exception so an async-void handler cannot crash the host.
            await AsyncVoidGuard.Run(
                () => throw new InvalidOperationException("no logger"),
                onFault: null);

            Assert.Pass();
        }

        [Test]
        public async Task Run_does_not_rethrow_when_logger_itself_throws()
        {
            // A misbehaving logger must not corrupt the guard
            // contract: the originating handler must still complete.
            await AsyncVoidGuard.Run(
                () => throw new InvalidOperationException("body"),
                ex => throw new InvalidOperationException("logger crashed"));

            Assert.Pass();
        }

        [Test]
        public async Task Run_no_ops_when_body_is_null()
        {
            // A misuse case (handler delegate not wired); the guard
            // must not throw NullReferenceException of its own.
            await AsyncVoidGuard.Run(body: null, onFault: _ => { });
            Assert.Pass();
        }
    }
}
