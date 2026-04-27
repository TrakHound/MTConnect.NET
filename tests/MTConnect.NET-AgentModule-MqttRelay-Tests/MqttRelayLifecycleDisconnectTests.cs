// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MTConnect.AgentModule.MqttRelay.Tests
{
    /// <summary>
    /// Pins the MqttRelay module disconnect-on-shutdown policy. The
    /// previous <c>OnStop()</c> implementation called
    /// <c>_mqttClient.DisconnectAsync(...)</c> as a fire-and-forget
    /// task: the returned <see cref="Task"/> was never awaited and any
    /// fault on the disconnect path was silently lost. That hid broker
    /// errors at shutdown (so an admin diagnosing a hung shutdown could
    /// not see the real cause) and risked the host process exiting
    /// before the disconnect actually completed.
    ///
    /// The lifecycle helper now exposes a bounded await with a
    /// fault-logging continuation. The policy:
    ///
    /// * Awaits the disconnect, but bounds the wait so a misbehaving
    ///   broker cannot hang shutdown forever.
    /// * Surfaces a fault to a logging callback so the operator gets a
    ///   diagnostic instead of a silently-dropped exception.
    /// * Treats the timeout itself as success (best-effort shutdown).
    /// </summary>
    [TestFixture]
    public class MqttRelayLifecycleDisconnectTests
    {
        [Test]
        public void DisconnectWithTimeout_returns_when_disconnect_completes()
        {
            // The disconnect task completes promptly; the helper must
            // not block the shutdown thread or invoke the fault log.
            string loggedFault = null;

            MqttRelayLifecycle.DisconnectWithTimeout(
                disconnect: () => Task.CompletedTask,
                timeout: TimeSpan.FromSeconds(1),
                onFault: ex => loggedFault = ex.Message);

            Assert.That(loggedFault, Is.Null,
                "Successful disconnect must not invoke the fault logger.");
        }

        [Test]
        public void DisconnectWithTimeout_logs_fault_when_disconnect_throws()
        {
            string loggedFault = null;

            MqttRelayLifecycle.DisconnectWithTimeout(
                disconnect: () => Task.FromException(new InvalidOperationException("broker rejected")),
                timeout: TimeSpan.FromSeconds(1),
                onFault: ex => loggedFault = ex.Message);

            Assert.That(loggedFault, Is.EqualTo("broker rejected"),
                "A faulted disconnect Task must surface its exception to the logger.");
        }

        [Test]
        public void DisconnectWithTimeout_returns_after_timeout_when_disconnect_hangs()
        {
            // A misbehaving broker that never completes the disconnect
            // must not hang shutdown indefinitely; the helper bounds the
            // wait and treats the timeout as best-effort success.
            var sw = System.Diagnostics.Stopwatch.StartNew();

            MqttRelayLifecycle.DisconnectWithTimeout(
                disconnect: () => new TaskCompletionSource<object>().Task,
                timeout: TimeSpan.FromMilliseconds(50),
                onFault: _ => { });

            sw.Stop();
            Assert.That(sw.Elapsed, Is.LessThan(TimeSpan.FromSeconds(2)),
                "Hung disconnect must bail out near the configured timeout.");
        }

        [Test]
        public void DisconnectWithTimeout_does_not_throw_when_disconnect_factory_throws_synchronously()
        {
            // A factory that throws before returning a Task represents a
            // misbehaving client; the helper must catch synchronously
            // and route the exception to the fault logger.
            string loggedFault = null;

            Assert.DoesNotThrow(() => MqttRelayLifecycle.DisconnectWithTimeout(
                disconnect: () => throw new InvalidOperationException("sync throw"),
                timeout: TimeSpan.FromSeconds(1),
                onFault: ex => loggedFault = ex.Message));

            Assert.That(loggedFault, Is.EqualTo("sync throw"));
        }

        [Test]
        public void DisconnectWithTimeout_no_ops_when_disconnect_factory_is_null()
        {
            // The shutdown path must tolerate a null disconnect factory
            // (for example when _mqttClient is null because the worker
            // never ran).
            Assert.DoesNotThrow(() => MqttRelayLifecycle.DisconnectWithTimeout(
                disconnect: null,
                timeout: TimeSpan.FromSeconds(1),
                onFault: _ => { }));
        }
    }
}
