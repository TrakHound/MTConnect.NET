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
        /// Non-generic overload of <see cref="Raise{T}(EventHandler{T}, object, T, EventHandler{Exception})"/> for
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

        /// <summary>
        /// Raises an event of any delegate shape with per-delegate fault
        /// isolation. The caller supplies the per-subscriber invocation lambda
        /// so no per-signature overload is required; this lets the helper cover
        /// custom delegate types (e.g. <c>delegate void Foo(IPAddress)</c>,
        /// <c>delegate void Bar(Source, IDevice)</c>) that the typed
        /// <see cref="EventHandler"/> / <see cref="EventHandler{T}"/> overloads
        /// cannot. Same contract as those overloads: a throwing subscriber
        /// cannot starve later subscribers, the
        /// <paramref name="internalError"/> sink is itself iterated
        /// per-delegate so a faulting fault-reporter cannot starve later fault
        /// reporters either, and a secondary throw from an internalError
        /// subscriber is terminal.
        /// </summary>
        /// <typeparam name="TDelegate">The delegate type of the event being raised. Must derive from <see cref="Delegate"/>.</typeparam>
        /// <param name="handler">The event handler whose invocation list is iterated; a null handler is a safe no-op covering the no-subscriber case.</param>
        /// <param name="invoke">The per-subscriber invocation lambda. Called once per delegate in <paramref name="handler"/>'s invocation list, inside the per-delegate try/catch.</param>
        /// <param name="internalError">The fault-routing sink. Each subscriber fault is routed through every delegate on this sink; pass <c>null</c> to swallow faults at the per-delegate boundary (consistent with the pre-isolation null-conditional behaviour).</param>
        /// <param name="sender">The sender object passed to <paramref name="internalError"/> when routing a fault. Pass the class instance whose event is being raised, or <c>null</c> if no sender is associated.</param>
        /// <remarks>
        /// Prefer the typed <see cref="Raise{T}(EventHandler{T}, object, T, EventHandler{Exception})"/>
        /// or <see cref="Raise(EventHandler, object, EventArgs, EventHandler{Exception})"/> overloads
        /// when the event uses <see cref="EventHandler"/> or <see cref="EventHandler{T}"/>;
        /// they avoid the call-site cast and the lambda allocation. Use this overload
        /// only for events declared with a custom delegate signature (i.e.
        /// <c>public event MyHandler Foo;</c> where <c>MyHandler</c> is not an
        /// <see cref="EventHandler"/> / <see cref="EventHandler{T}"/>).
        /// </remarks>
        public static void Raise<TDelegate>(TDelegate handler, Action<TDelegate> invoke,
                                            EventHandler<Exception> internalError = null,
                                            object sender = null)
            where TDelegate : Delegate
        {
            if (handler == null) return;

            foreach (var subscriber in handler.GetInvocationList())
            {
                try
                {
                    invoke((TDelegate)(object)subscriber);
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
