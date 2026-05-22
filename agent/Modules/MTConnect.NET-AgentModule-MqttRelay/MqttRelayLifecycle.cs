// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;

namespace MTConnect
{
    /// <summary>
    /// Centralises the MqttRelay agent module shutdown policy.
    /// <see cref="StopServers"/> stops the per-topic-structure servers
    /// (each <c>Stop()</c> is independently null-safe and exception-
    /// safe so a throw on one path cannot leave the other server
    /// running, and <c>TopicStructure=Entity</c>'s null
    /// <c>_documentServer</c> reference does not crash shutdown).
    /// <see cref="DisconnectWithTimeout"/> bounds the MQTT-client
    /// disconnect so a hung broker cannot block the agent from
    /// terminating. Both methods are unit-testable without
    /// instantiating an MTConnect agent broker.
    /// </summary>
    internal static class MqttRelayLifecycle
    {
        /// <summary>
        /// Invokes the supplied per-server stop actions, tolerating
        /// either or both being <c>null</c>. A throw from one action
        /// does not prevent the other from running.
        /// </summary>
        /// <param name="documentStop">
        /// Optional action stopping the Document-mode server.
        /// </param>
        /// <param name="entityStop">
        /// Optional action stopping the Entity-mode server.
        /// </param>
        public static void StopServers(Action documentStop, Action entityStop)
        {
            if (documentStop != null)
            {
                try { documentStop(); } catch { }
            }

            if (entityStop != null)
            {
                try { entityStop(); } catch { }
            }
        }

        /// <summary>
        /// Awaits an MQTT-client disconnect with a bounded timeout.
        ///
        /// * Synchronous throw from <paramref name="disconnect"/> is
        ///   captured and routed to <paramref name="onFault"/> instead
        ///   of escaping the shutdown path.
        /// * A faulted Task is surfaced to <paramref name="onFault"/>
        ///   with its inner exception.
        /// * A disconnect that does not complete within
        ///   <paramref name="timeout"/> is treated as best-effort
        ///   success so a misbehaving broker cannot hang shutdown.
        /// * A null <paramref name="disconnect"/> is a no-op (the
        ///   worker may never have created an MQTT client).
        /// </summary>
        /// <param name="disconnect">
        /// Factory that begins the disconnect and returns its
        /// <see cref="Task"/>. May be <c>null</c>.
        /// </param>
        /// <param name="timeout">
        /// Bound on how long shutdown is willing to wait. A timeout
        /// elapses silently because the agent is already shutting down.
        /// </param>
        /// <param name="onFault">
        /// Logger callback invoked with the exception when the
        /// disconnect throws synchronously or its Task faults.
        /// </param>
        public static void DisconnectWithTimeout(
            Func<Task> disconnect,
            TimeSpan timeout,
            Action<Exception> onFault)
        {
            if (disconnect == null) return;

            Task task;
            try
            {
                task = disconnect();
            }
            catch (Exception ex)
            {
                if (onFault != null) onFault(ex);
                return;
            }

            if (task == null) return;

            try
            {
                if (!task.Wait(timeout))
                {
                    // Bounded wait elapsed; agent is shutting down so
                    // a hung broker cannot block process exit. Best
                    // effort: leave the task to be GC'd. Attach a
                    // continuation so its eventual fault is still
                    // surfaced rather than swallowed by the finalizer.
                    if (onFault != null)
                    {
                        task.ContinueWith(
                            t => onFault(t.Exception),
                            TaskContinuationOptions.OnlyOnFaulted);
                    }
                    return;
                }
            }
            catch (AggregateException agg)
            {
                if (onFault != null)
                {
                    var inner = agg.InnerException ?? agg;
                    onFault(inner);
                }
            }
            catch (Exception ex)
            {
                if (onFault != null) onFault(ex);
            }
        }
    }
}
