// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect
{
    /// <summary>
    /// Helpers for the MqttRelay buffered-observation diagnostic log
    /// lines. The "missed observations" figure printed when a relay
    /// reconnects is computed against unsigned broker sequence numbers,
    /// so the helper guards the unsigned subtraction from underflow.
    /// </summary>
    internal static class RelayBufferDiagnostics
    {
        /// <summary>
        /// Computes the count of buffered observations the operator
        /// reasonably expects to see relayed when the connection
        /// recovers. Returns <c>0</c> when <paramref name="lastSent"/>
        /// is at or above <paramref name="to"/>; that case is
        /// degenerate (a stale persisted last-sent-sequence value or a
        /// rolled broker sequence) so a meaningful "missed" figure
        /// cannot be produced and the diagnostic should not print a
        /// huge spurious number.
        /// </summary>
        /// <param name="to">
        /// The broker's current <c>LastSequence</c>.
        /// </param>
        /// <param name="lastSent">
        /// The persisted <c>last-sent-sequence</c>.
        /// </param>
        public static long ComputeMissed(ulong to, ulong lastSent)
        {
            if (lastSent >= to) return 0;
            return (long)(to - lastSent);
        }
    }
}
