// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect
{
    /// <summary>
    /// Shared event multicast helper that iterates the invocation list with
    /// per-delegate fault isolation. A throwing subscriber cannot starve later
    /// subscribers of the same event; subscriber faults are routed through the
    /// caller-supplied <c>internalError</c> sink, which is itself iterated
    /// per-delegate so a throwing fault reporter cannot starve later fault
    /// reporters either. If the <c>internalError</c> handler itself throws, the
    /// secondary fault is terminal and swallowed — there is no further sink to
    /// route it to without risking the same starvation loop.
    /// </summary>
    public static class MulticastIsolation
    {
        /// <summary>
        /// Raises a generic event with per-delegate fault isolation. If any
        /// subscriber throws, the fault is routed through
        /// <paramref name="internalError"/> without interrupting fan-out to
        /// subsequent subscribers. The <paramref name="internalError"/> handler
        /// itself is iterated with the same per-delegate try/catch so a
        /// throwing fault-reporter cannot starve later fault subscribers
        /// either.
        /// </summary>
        public static void Raise<T>(EventHandler<T> handler, object sender, T arg,
                                    EventHandler<Exception> internalError)
        {
            if (handler == null) return;

            foreach (var subscriber in handler.GetInvocationList())
            {
                try
                {
                    ((EventHandler<T>)subscriber).Invoke(sender, arg);
                }
                catch (Exception ex)
                {
                    RaiseInternalError(internalError, sender, ex);
                }
            }
        }

        /// <summary>
        /// Non-generic overload of <see cref="Raise{T}"/> for
        /// <see cref="EventHandler"/> events that carry no typed payload.
        /// Same contract: a throwing subscriber cannot starve later subscribers
        /// and a faulting <paramref name="internalError"/> handler cannot break
        /// the fan-out either.
        /// </summary>
        public static void Raise(EventHandler handler, object sender, EventArgs arg,
                                 EventHandler<Exception> internalError)
        {
            if (handler == null) return;

            foreach (var subscriber in handler.GetInvocationList())
            {
                try
                {
                    ((EventHandler)subscriber).Invoke(sender, arg);
                }
                catch (Exception ex)
                {
                    RaiseInternalError(internalError, sender, ex);
                }
            }
        }

        // Iterate the InternalError invocation list so a throwing fault
        // reporter cannot starve later fault reporters. A secondary throw from
        // an InternalError subscriber itself is terminal: there is no further
        // sink to route it to without resurrecting the same starvation bug.
        private static void RaiseInternalError(EventHandler<Exception> internalError,
                                               object sender, Exception ex)
        {
            if (internalError == null) return;

            foreach (var subscriber in internalError.GetInvocationList())
            {
                try
                {
                    ((EventHandler<Exception>)subscriber).Invoke(sender, ex);
                }
                catch
                {
                    // Terminal: cannot route a fault about the fault without
                    // risking the starvation loop this helper exists to close.
                }
            }
        }
    }
}
