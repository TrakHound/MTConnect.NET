// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;

namespace MTConnect
{
    /// <summary>
    /// Centralises the MqttRelay agent-module <c>async void</c>
    /// event-handler safety policy. An <c>async void</c> method that
    /// throws routes its exception to the synchronization context,
    /// which on the ThreadPool tears down the host process; the
    /// MqttRelay handlers run on broker-raised events, so a formatter
    /// throw (DataItem null-deref) or a synchronous broker call
    /// (broker shutting down) would crash the agent without a top-
    /// level guard. Wrapping every handler body in <see cref="Run"/>
    /// captures any exception (synchronous or async) and routes it to
    /// a logging callback so the host stays alive. The guard is
    /// unit-testable in isolation, so the policy lives away from the
    /// hard-to-mock Module class.
    /// </summary>
    internal static class AsyncVoidGuard
    {
        /// <summary>
        /// Awaits the supplied handler body and routes any exception
        /// to the logging callback. Never rethrows: the contract is
        /// "an async-void path must not crash the host". A null body
        /// is a no-op and a null or throwing logger is tolerated.
        /// </summary>
        /// <param name="body">
        /// The handler implementation. May be <c>null</c>.
        /// </param>
        /// <param name="onFault">
        /// Logger callback invoked with any exception thrown
        /// synchronously by <paramref name="body"/> or surfaced through
        /// its <see cref="Task"/>. May be <c>null</c>.
        /// </param>
        public static async Task Run(Func<Task> body, Action<Exception> onFault)
        {
            if (body == null) return;

            try
            {
                var task = body();
                if (task != null) await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (onFault != null)
                {
                    try { onFault(ex); } catch { }
                }
            }
        }
    }
}
