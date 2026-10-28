// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Threading;

namespace MTConnect
{
    /// <summary>
    /// Durable-relay delivery state. Tracks whether an observation may not have been delivered
    /// (a publish failure or a queue overflow) since the last successful catch-up, and the highest
    /// sequence the current session's catch-up has already delivered. While a gap is open the
    /// last-sent sequence must not advance, so the next catch-up resends from the last contiguous
    /// delivered sequence.
    /// </summary>
    internal sealed class RelaySequenceTracker
    {
        private int _gap;
        private long _catchUpThrough;


        /// <summary>True when an observation may not have been delivered since the last catch-up.</summary>
        public bool HasGap => Interlocked.CompareExchange(ref _gap, 0, 0) == 1;

        /// <summary>Highest sequence delivered by the current session's catch-up.</summary>
        public ulong CatchUpThrough => unchecked((ulong)Interlocked.Read(ref _catchUpThrough));


        /// <summary>Records that an observation may not have been delivered.</summary>
        public void MarkGap() => Interlocked.Exchange(ref _gap, 1);

        /// <summary>Clears the gap (everything after the last-sent sequence is being, or has been, resent).</summary>
        public void ClearGap() => Interlocked.Exchange(ref _gap, 0);

        /// <summary>Records that the catch-up delivered every sequence up to <paramref name="sequence"/>.</summary>
        public void SetCatchUpThrough(ulong sequence) => Interlocked.Exchange(ref _catchUpThrough, unchecked((long)sequence));

        /// <summary>True when <paramref name="sequence"/> was already delivered by this session's catch-up.</summary>
        public bool IsDelivered(ulong sequence) => sequence <= CatchUpThrough;

        /// <summary>
        /// Reports a completed publish (in enqueue order). Returns <c>true</c> when the last-sent sequence
        /// should be advanced to the item's sequence.
        /// </summary>
        public bool Completed(bool success, bool skipped)
        {
            if (skipped) return false;

            if (!success)
            {
                MarkGap();
                return false;
            }

            return !HasGap;
        }
    }
}
