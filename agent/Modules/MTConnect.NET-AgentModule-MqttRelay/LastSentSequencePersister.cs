// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Threading;

namespace MTConnect
{
    /// <summary>
    /// Tracks the MqttRelay last-sent observation sequence in memory
    /// and flushes to durable storage only when explicitly asked.
    /// Event handlers call <see cref="Update"/> on the hot path;
    /// disk IO happens out-of-band via <see cref="TryFlush"/> from a
    /// timer tick, the shutdown path, and each buffered-relay batch
    /// boundary, so the handler stream never blocks on
    /// <c>File.WriteAllText</c>.
    ///
    /// All read/write of the value field uses
    /// <see cref="Interlocked"/> so the persister is safe to share
    /// between event-handler threads and a flush timer thread on 32-
    /// bit hosts where bare 64-bit access can tear.
    /// </summary>
    internal sealed class LastSentSequencePersister
    {
        private long _value;
        private int _dirty; // 0 = clean, 1 = dirty (Interlocked-managed)

        /// <summary>
        /// Returns the current in-memory sequence value with an
        /// atomic 64-bit read. Does not clear the dirty bit; only
        /// <see cref="TryFlush"/> establishes durable persistence.
        /// </summary>
        public ulong Read()
        {
            return unchecked((ulong)Interlocked.Read(ref _value));
        }

        /// <summary>
        /// Whether the in-memory value has been modified since the
        /// last successful flush. The flush timer can use this to
        /// skip writes when the relay is idle.
        /// </summary>
        public bool IsDirty
        {
            get { return Interlocked.CompareExchange(ref _dirty, 0, 0) != 0; }
        }

        /// <summary>
        /// Records a new last-sent sequence value (last write wins)
        /// and marks the persister dirty so the next flush will
        /// emit a write.
        /// </summary>
        public void Update(ulong value)
        {
            Interlocked.Exchange(ref _value, unchecked((long)value));
            Interlocked.Exchange(ref _dirty, 1);
        }

        /// <summary>
        /// Seeds the in-memory value from durable storage at startup
        /// without marking the persister dirty. The initial value
        /// already matches disk so a flush would be redundant.
        /// </summary>
        public void Initialize(ulong value)
        {
            Interlocked.Exchange(ref _value, unchecked((long)value));
            Interlocked.Exchange(ref _dirty, 0);
        }

        /// <summary>
        /// Emits a write through <paramref name="write"/> if and only
        /// if the persister is dirty, then clears the dirty bit. If
        /// the writer throws, the dirty bit is preserved and the
        /// exception propagates so the caller logs and retries on
        /// the next tick. Returns <c>true</c> when a write is
        /// actually emitted.
        /// </summary>
        public bool TryFlush(Action<ulong> write)
        {
            if (write == null) return false;
            if (Interlocked.CompareExchange(ref _dirty, 0, 0) == 0) return false;

            var snapshot = Read();
            // Clear dirty *after* a successful write so that a thrown
            // writer leaves the persister dirty for the next attempt.
            write(snapshot);
            Interlocked.Exchange(ref _dirty, 0);
            return true;
        }
    }
}
