// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MTConnect.AgentModule.MqttRelay.Tests
{
    /// <summary>
    /// Pins the MqttRelay Worker outer-catch logging policy. The
    /// Worker do/while loop in Module.cs previously had:
    ///
    ///   catch (TaskCanceledException) { }
    ///   catch (Exception) { }
    ///
    /// The bare empty catch on the outer loop swallowed any unexpected
    /// exception escaping the inner try/catch (for example a throw
    /// inside the inner finally block or an oversight in connection
    /// handling). The operator saw nothing, the relay quietly entered
    /// the reconnect-delay branch, and the underlying defect went
    /// undiagnosed for the lifetime of the agent.
    ///
    /// WorkerLoopExceptionLogger encodes the policy:
    ///
    /// * TaskCanceledException is the orderly-shutdown signal; do not
    ///   log it (would spam the operator on every stop).
    /// * Any other exception is genuinely unexpected at this scope;
    ///   log it at Warning so the underlying defect is visible.
    /// </summary>
    [TestFixture]
    public class WorkerLoopExceptionLoggerTests
    {
        [Test]
        public void Log_skips_TaskCanceledException()
        {
            var logged = false;

            WorkerLoopExceptionLogger.Log(
                exception: new TaskCanceledException(),
                onLog: _ => logged = true);

            Assert.That(logged, Is.False,
                "TaskCanceledException is the orderly-shutdown signal; do not spam the log on every stop.");
        }

        [Test]
        public void Log_writes_unexpected_exception_to_callback()
        {
            string logged = null;

            WorkerLoopExceptionLogger.Log(
                exception: new InvalidOperationException("boom"),
                onLog: msg => logged = msg);

            Assert.That(logged, Is.Not.Null);
            Assert.That(logged, Does.Contain("boom"),
                "The unexpected-exception message must include the exception text so the operator can diagnose the defect.");
        }

        [Test]
        public void Log_includes_exception_type_name()
        {
            string logged = null;

            WorkerLoopExceptionLogger.Log(
                exception: new InvalidOperationException("boom"),
                onLog: msg => logged = msg);

            Assert.That(logged, Does.Contain(nameof(InvalidOperationException)),
                "Type name aids in classifying the defect from log scrapes.");
        }

        [Test]
        public void Log_no_ops_when_exception_is_null()
        {
            var logged = false;

            WorkerLoopExceptionLogger.Log(
                exception: null,
                onLog: _ => logged = true);

            Assert.That(logged, Is.False,
                "A null exception cannot be logged usefully; do not invoke the callback.");
        }

        [Test]
        public void Log_no_ops_when_callback_is_null()
        {
            // Defensive: the helper must not throw when the logger is
            // not wired (would defeat the purpose of catching the
            // unexpected exception).
            Assert.DoesNotThrow(() => WorkerLoopExceptionLogger.Log(
                exception: new InvalidOperationException("boom"),
                onLog: null));
        }

        [Test]
        public void Log_treats_subclass_of_TaskCanceledException_as_cancellation()
        {
            // OperationCanceledException is the parent type; a
            // cancelation token expiring throws TaskCanceledException
            // (a subclass). Future runtime versions could extend the
            // hierarchy; the policy must treat any cancelation as
            // an orderly-shutdown signal.
            var logged = false;

            WorkerLoopExceptionLogger.Log(
                exception: new OperationCanceledException(),
                onLog: _ => logged = true);

            Assert.That(logged, Is.False,
                "OperationCanceledException is also an orderly-shutdown signal; do not log.");
        }
    }
}
