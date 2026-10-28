// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Observations;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Buffers
{
    /// <summary>
    /// Circular Ephemeral Buffer used to store MTConnect Observation Data
    /// </summary>
    public class MTConnectObservationBuffer : IMTConnectObservationBuffer
    {
        /// <summary>
        /// No longer used. Requests no longer force garbage collections; kept for API compatibility.
        /// </summary>
        public const int HighPriorityGC = 5000; // Minimum number of Observations requested that will trigger a High Priority Garbage Collection

        private readonly string _id = Guid.NewGuid().ToString();

        private static readonly string _levelNormal = ConditionLevel.NORMAL.ToString();
        private static readonly string _levelWarning = ConditionLevel.WARNING.ToString();
        private static readonly string _levelFault = ConditionLevel.FAULT.ToString();
        private static readonly string _levelUnavailable = ConditionLevel.UNAVAILABLE.ToString();

        // True only when a derived class overrides OnCurrentConditionChange, so the full
        // current-condition copy passed to it is not built for every condition observation otherwise
        private bool? _hasConditionChangeOverride;
        private readonly object _lock = new object();
        private ulong _sequence = 1;

        private readonly IDictionary<int, BufferObservation> _currentObservations = new Dictionary<int, BufferObservation>();
        private readonly IDictionary<int, IEnumerable<BufferObservation>> _currentConditions = new Dictionary<int, IEnumerable<BufferObservation>>();
        private CircularBuffer _archiveObservations;

        /// <summary>
        /// Get a unique identifier for the Buffer
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// Get the configured size of the Buffer in the number of maximum number of Observations the buffer can hold at one time.
        /// </summary>
        public uint BufferSize { get; set; } = 150000;

        /// <summary>
        /// A number representing the sequence number assigned to the oldest Observation stored in the buffer
        /// </summary>
        public ulong FirstSequence
        {
            get
            {
                lock (_lock)
                {
                    // Empty (or disposed) archive: nothing has been stored yet
                    if (_archiveObservations == null || _archiveObservations.Size == 0) return 1;

                    var sequence = _archiveObservations[0]._sequence;
                    return sequence > 0 ? sequence : 1;
                }
            }
        }

        /// <summary>
        /// A number representing the sequence number assigned to the last Observation that was added to the buffer
        /// </summary>
        public ulong LastSequence
        {
            get
            {
                lock (_lock) return _sequence > 1 ? _sequence - 1 : 1;
            }
        }

        /// <summary>
        /// A number representing the sequence number of the Observation that is the next piece of data to be retrieved from the buffer
        /// </summary>
        public ulong NextSequence
        {
            get
            {
                lock (_lock) return _sequence;
            }
        }


        /// <summary>
        /// A snapshot copy of the latest non-condition observation for each buffer key, indexed by the buffer key hash.
        /// </summary>
        public IDictionary<int, BufferObservation> CurrentObservations
        {
            get
            {
                lock (_lock) return new Dictionary<int, BufferObservation>(_currentObservations);
            }
        }

        /// <summary>
        /// A snapshot copy of the active condition observations for each condition buffer key, indexed by the buffer key hash.
        /// </summary>
        public IDictionary<int, IEnumerable<BufferObservation>> CurrentConditions
        {
            get
            {
                lock (_lock) return new Dictionary<int, IEnumerable<BufferObservation>>(_currentConditions);
            }
        }


        /// <summary>
        /// Initializes the buffer with the default observation capacity.
        /// </summary>
        public MTConnectObservationBuffer()
        {
            _archiveObservations = new CircularBuffer(BufferSize);
        }

        /// <summary>
        /// Initializes the buffer, taking its observation capacity from the supplied agent configuration when provided.
        /// </summary>
        /// <param name="configuration">The agent configuration whose observation buffer size is applied; ignored when null.</param>
        public MTConnectObservationBuffer(IAgentConfiguration configuration)
        {
            if (configuration != null)
            {
                BufferSize = configuration.ObservationBufferSize;
            }

            _archiveObservations = new CircularBuffer(BufferSize);
        }

        /// <summary>
        /// Releases the circular archive and clears the current observation and condition snapshots.
        /// </summary>
        public virtual void Dispose()
        {
            _archiveObservations = null;
            _currentObservations.Clear();
            _currentConditions.Clear();
        }


        /// <summary>
        /// Extension point invoked when the active condition set for a key changes; overridden by durable buffers to persist the change. The base implementation does nothing.
        /// </summary>
        /// <param name="observations">The new active condition set.</param>
        protected virtual void OnCurrentConditionChange(IEnumerable<BufferObservation> observations) { }

        private bool HasConditionChangeOverride()
        {
            if (_hasConditionChangeOverride == null)
            {
                var method = GetType().GetMethod(nameof(OnCurrentConditionChange), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                _hasConditionChangeOverride = method != null && method.DeclaringType != typeof(MTConnectObservationBuffer);
            }

            return _hasConditionChangeOverride.Value;
        }

        /// <summary>
        /// Extension point invoked after the current observation for a key is updated; overridden by durable buffers to persist the change. The base implementation does nothing.
        /// </summary>
        /// <param name="observation">The observation that became current.</param>
        protected virtual void OnCurrentObservationAdd(ref BufferObservation observation) { }

        /// <summary>
        /// Extension point invoked after the active condition set for a key is established; overridden by durable buffers to persist the change. The base implementation does nothing.
        /// </summary>
        /// <param name="observations">The active condition set.</param>
        protected virtual void OnCurrentConditionAdd(ref IEnumerable<BufferObservation> observations) { }

        /// <summary>
        /// Extension point invoked after an observation is appended to the archive; overridden by durable buffers to persist the entry. The base implementation does nothing.
        /// </summary>
        /// <param name="observation">The observation appended to the archive.</param>
        protected virtual void OnBufferObservationAdd(ref BufferObservation observation) { }



        #region "Sequence"

        /// <summary>
        /// Sets the Agent's Sequence to the specified value
        /// </summary>
        protected void SetSequence(ulong sequence)
        {
            _sequence = sequence;
        }

        /// <summary>
        /// Increment the Agent's Sequence number by one
        /// </summary>
        public void IncrementSequence()
        {
            lock (_lock)
            {
                _sequence++;
            }
        }

        /// <summary>
        /// Increment the Agent's Sequence number by the specified count
        /// </summary>
        public void IncrementSequence(uint count)
        {
            lock (_lock)
            {
                _sequence += count;
            }
        }

        #endregion

        #region "Read"

        #region "Internal"

        /// <summary>
        /// Returns a flat copy of every current non-condition observation held in the buffer.
        /// </summary>
        protected IEnumerable<BufferObservation> GetCurrentObservations()
        {
            // Snapshot under the buffer lock so a concurrent AddObservation can't tear the read
            lock (_lock) return new List<BufferObservation>(_currentObservations.Values);
        }

        /// <summary>
        /// Returns a flat copy of every active condition observation held in the buffer, flattening the per-key condition sets.
        /// </summary>
        protected IEnumerable<BufferObservation> GetCurrentConditions()
        {
            var x = new List<BufferObservation>();

            // Snapshot under the buffer lock so a concurrent AddObservation can't tear the read
            lock (_lock)
            {
                var conditions = _currentConditions.Values;
                if (!conditions.IsNullOrEmpty())
                {
                    foreach (var condition in conditions) x.AddRange(condition);
                }
            }

            return x;
        }

        /// <summary>
        /// Returns the logical buffer indexes (grouped by key, in the order of <paramref name="keys"/>) of the entries in
        /// <paramref name="slotKeys"/> (a snapshot of the keys of logical indexes <paramref name="startIndex"/> onward) that match.
        /// </summary>
        private static int[] GetIndexes(int[] slotKeys, int slotCount, int startIndex, int[] keys, uint count = 0)
        {
            if (slotKeys != null && slotCount > 0 && keys != null && keys.Length > 0)
            {
                int oi = 0; // Snapshot Iterator
                var oil = slotCount - 1;

                uint max = (uint)(oil - oi + 1); // Max Observations length
                if (count > 0) max = Math.Min(count, max); // Ensure max length doesn't exceed 'count' parameter

                // Map each BufferKey to its position in 'keys' (first occurrence wins)
                var keyLookup = CreateKeyLookup(keys);

                // Single pass over the range: record the key position and buffer index of each match
                var matchKeys = new List<int>();
                var matchIndexes = new List<int>();
                var keyCounts = new int[keys.Length];

                while (oi <= oil && (uint)matchIndexes.Count < max)
                {
                    if (keyLookup.TryGetValue(slotKeys[oi], out var k))
                    {
                        matchKeys.Add(k);
                        matchIndexes.Add(startIndex + oi);
                        keyCounts[k]++;
                    }

                    oi++;
                }

                // If any Observations were Matched
                if (matchIndexes.Count > 0)
                {
                    // Group the matches by key position (the order of 'keys'), keeping buffer order within each key.
                    // BufferKeys are ordered in ascending order, so additional sorting should never be required
                    var offsets = new int[keys.Length];
                    var offset = 0;
                    for (var k = 0; k < keys.Length; k++)
                    {
                        offsets[k] = offset;
                        offset += keyCounts[k];
                    }

                    var indexes = new int[matchIndexes.Count];
                    for (var m = 0; m < matchIndexes.Count; m++)
                    {
                        indexes[offsets[matchKeys[m]]++] = matchIndexes[m];
                    }

                    return indexes;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns, for each key, the index of the most recent observation at or before <paramref name="atIndex"/>
        /// (or -1 when none), using a single reverse scan of the buffer.
        /// </summary>
        private static int[] GetLatestIndexes(CircularBuffer observations, int[] keys, int atIndex)
        {
            var indexes = new int[keys.Length];
            for (var i = 0; i < indexes.Length; i++) indexes[i] = -1;

            // Bound by Size, not Capacity: slots past Size have not been written yet
            if (observations != null && observations.Size > 0 && atIndex < observations.Size && keys.Length > 0)
            {
                // Positions of each key in 'keys' (a key can appear more than once)
                var keyLookup = new Dictionary<int, List<int>>(keys.Length);
                for (var k = 0; k < keys.Length; k++)
                {
                    if (!keyLookup.TryGetValue(keys[k], out var positions))
                    {
                        positions = new List<int>(1);
                        keyLookup.Add(keys[k], positions);
                    }
                    positions.Add(k);
                }

                var remaining = keyLookup.Count;

                // Loop through the observations in reverse order (sequence desc) to find the most recent for each key
                for (var i = atIndex; i >= 0 && remaining > 0; i--)
                {
                    var key = observations[i]._key;
                    if (keyLookup.TryGetValue(key, out var positions))
                    {
                        foreach (var k in positions) indexes[k] = i;
                        keyLookup.Remove(key);
                        remaining--;
                    }
                }
            }

            return indexes;
        }

        // Sequence of the oldest observation in the archive (1 when it is empty). Read from the archive rather than
        // derived from the buffer size, since a reloaded durable buffer need not be full or contiguous
        private ulong ArchiveFirstSequence()
        {
            return _archiveObservations != null && _archiveObservations.Size > 0 ? _archiveObservations[0]._sequence : 1;
        }

        // True when a current observation is valid and not newer than "at" (or than the oldest buffered sequence when "at" is not set)
        private static bool IsAtOrBefore(in BufferObservation observation, ulong at, ulong firstSequence)
        {
            if (!observation.IsValid) return false;
            return at < 1 ? observation._sequence <= firstSequence : observation._sequence <= at;
        }

        private static Dictionary<int, int> CreateKeyLookup(int[] keys)
        {
            var keyLookup = new Dictionary<int, int>(keys.Length);
            for (var k = 0; k < keys.Length; k++)
            {
                if (!keyLookup.ContainsKey(keys[k])) keyLookup.Add(keys[k], k);
            }
            return keyLookup;
        }

        #endregion


        /// <summary>
        /// Get a list of the Current Observations based on the specified BufferKeys
        /// </summary>
        /// <param name="bufferKeys">A list of Keys (DeviceUuid and DataItemId) to match observations in the buffer</param>
        /// <returns>An object that implements the IStreamingResults interface containing the query results</returns>
        public IObservationBufferResults GetCurrentObservations(IEnumerable<int> bufferKeys)
        {
            if (!bufferKeys.IsNullOrEmpty())
            {
                // Order Buffer Keys (this effects the order of the resulting array and should always be in Ascending order)
                var oBufferKeys = bufferKeys.OrderBy(o => o).ToArray();

                ulong firstSequence = 0;
                ulong lastSequence = 0;
                ulong nextSequence = 0;

                var observations = new List<BufferObservation>();

                lock (_lock)
                {
                    firstSequence = ArchiveFirstSequence();
                    lastSequence = _sequence > 1 ? _sequence - 1 : 1;
                    nextSequence = _sequence;

                    foreach (var bufferKey in oBufferKeys)
                    {
                        // Get From Current Observations
                        if (_currentObservations.TryGetValue(bufferKey, out var observation))
                        {
                            observations.Add(observation);
                        }

                        // Get From Current Observations
                        if (_currentConditions.TryGetValue(bufferKey, out var conditions))
                        {
                            observations.AddRange(conditions);
                        }
                    }
                }

                var aObservations = observations.ToArray();
                var firstObservationSequence = aObservations.Length > 0 ? observations.Min(o => o.Sequence) : 1;
                var lastObservationSequence = aObservations.Length > 0 ? observations.Max(o => o.Sequence) : 1;

                return new ObservationBufferResults
                {
                    FirstSequence = firstSequence,
                    LastSequence = lastSequence,
                    NextSequence = nextSequence,
                    Observations = aObservations,
                    FirstObservationSequence = firstObservationSequence,
                    LastObservationSequence = lastObservationSequence,
                    ObservationCount = (uint)aObservations.Length,
                    IsValid = true
                };
            }

            return ObservationBufferResults.Invalid();
        }

        /// <summary>
        /// Get a list of the latest Observations at the specified sequence based on the specified BufferKeys
        /// </summary>
        /// <param name="bufferKeys">A list of Keys (DeviceUuid and DataItemId) to match observations in the buffer</param>
        /// <param name="at">The sequence number to include in the results</param>
        /// <returns>An object that implements the IStreamingResults interface containing the query results</returns>
        public IObservationBufferResults GetCurrentObservations(IEnumerable<int> bufferKeys, ulong at)
        {
            if (!bufferKeys.IsNullOrEmpty())
            {
                // Order Buffer Keys (this effects the order of the resulting array and should always be in Ascending order)
                var oBufferKeys = bufferKeys.OrderBy(o => o).ToArray();

                ulong firstSequence = 0;
                ulong lastSequence = 0;
                ulong nextSequence = 0;
                var observations = new List<BufferObservation>(oBufferKeys.Length);

                lock (_lock)
                {
                    firstSequence = ArchiveFirstSequence();
                    lastSequence = _sequence > 1 ? _sequence - 1 : 1;
                    nextSequence = _sequence;

                    // Index of the newest observation at or before "at" (-1 when "at" is older than the archive)
                    var atIndex = -1;
                    if (_archiveObservations != null && _archiveObservations.Size > 0)
                    {
                        atIndex = at < 1 ? 0 : _archiveObservations.LastIndexAtOrBefore(at);
                    }

                    // Get the latest observation for each key based on the "at" parameter
                    var indexes = atIndex >= 0 ? GetLatestIndexes(_archiveObservations, oBufferKeys, atIndex) : null;

                    for (var i = 0; i < oBufferKeys.Length; i++)
                    {
                        var index = indexes != null ? indexes[i] : -1;
                        if (index >= 0)
                        {
                            observations.Add(_archiveObservations[index]);
                        }
                        else
                        {
                            // Not in the archive up to "at": use the current value(s) when not newer than "at"
                            if (_currentObservations.TryGetValue(oBufferKeys[i], out var observation) && IsAtOrBefore(observation, at, firstSequence))
                            {
                                observations.Add(observation);
                            }

                            if (_currentConditions.TryGetValue(oBufferKeys[i], out var conditions))
                            {
                                foreach (var condition in conditions)
                                {
                                    if (IsAtOrBefore(condition, at, firstSequence)) observations.Add(condition);
                                }
                            }
                        }
                    }
                }

                // Keys with no observation at "at" are left out (not returned as empty entries)
                var aObservations = observations.ToArray();
                var firstObservationSequence = aObservations.Length > 0 ? aObservations.Min(o => o.Sequence) : 1;
                var lastObservationSequence = aObservations.Length > 0 ? aObservations.Max(o => o.Sequence) : 1;

                return new ObservationBufferResults
                {
                    FirstSequence = firstSequence,
                    LastSequence = lastSequence,
                    NextSequence = nextSequence,
                    Observations = aObservations,
                    FirstObservationSequence = firstObservationSequence,
                    LastObservationSequence = lastObservationSequence,
                    ObservationCount = (uint)aObservations.Length,
                    IsValid = true
                };
            }

            return ObservationBufferResults.Invalid();
        }

        /// <summary>
        /// Get a list of Observations based on the specified search parameters
        /// </summary>
        /// <param name="bufferKeys">A list of Keys (DeviceUuid and DataItemId) to match observations in the buffer</param>
        /// <param name="from">The minimum sequence number to include in the results</param>
        /// <param name="to">The maximum sequence number to include in the results</param>
        /// <param name="count">The maximum number of Observations to include in the result</param>
        /// <returns>An object that implements the IStreamingResults interface containing the query results</returns>
        public IObservationBufferResults GetObservations(IEnumerable<int> bufferKeys, ulong from = 0, ulong to = 0, uint count = 100)
        {
            if (_archiveObservations != null && !bufferKeys.IsNullOrEmpty())
            {
                long now = UnixDateTime.Now;
                ulong firstSequence = 0;
                ulong lastSequence = 0;
                ulong nextSequence = 0;
                uint observationCount = 0;
                BufferObservation[] observations = null;
                ulong firstObservationSequence = 0;
                ulong lastObservationSequence = 0;
                int lowestIndex = int.MaxValue;
                int highestIndex = 0;

                int[] slotKeys = null;
                var slotCount = 0;
                var startIndex = 0;
                long evicted = 0;
                CircularBuffer snapshotBuffer = null;
                ulong rangeEndSequence = 0; // Sequence of the last observation in the requested range (when "to" ends it early)
                var countLimited = false;
                var evictedDuringScan = false;

                // 1) Under the lock: determine the range and copy only its BufferKeys (a fast array copy)
                lock (_lock)
                {
                    firstSequence = ArchiveFirstSequence();
                    lastSequence = _sequence > 1 ? _sequence - 1 : 1;
                    nextSequence = _sequence;

                    // Determine Indexes by sequence (the archive can have gaps after a durable buffer is reloaded)
                    var fromIndex = 0;
                    var toIndex = -1;
                    if (_archiveObservations.Size > 0)
                    {
                        fromIndex = from > firstSequence ? _archiveObservations.FirstIndexAtOrAfter(from) : 0;
                        toIndex = to > 0 ? _archiveObservations.LastIndexAtOrBefore(to) : _archiveObservations.Size - 1;
                    }

                    if (_archiveObservations.Size > 0 && toIndex < _archiveObservations.Size && toIndex >= fromIndex)
                    {
                        startIndex = Math.Max(fromIndex, 0);
                        slotCount = toIndex - startIndex + 1;
                        if (slotCount > 0)
                        {
                            slotKeys = ArrayPool<int>.Shared.Rent(slotCount);
                            _archiveObservations.CopyKeys(startIndex, slotKeys, slotCount);
                            evicted = _archiveObservations.Evicted;
                            snapshotBuffer = _archiveObservations;
                            if (to > 0 && toIndex < _archiveObservations.Size - 1) rangeEndSequence = _archiveObservations[toIndex]._sequence;
                        }
                    }
                }

                if (slotKeys != null)
                {
                    int[] indexes;
                    try
                    {
                        // 2) Outside the lock: match the requested keys (ingest is not blocked by the scan)
                        // Order Buffer Keys (this effects the order of the resulting array and should always be in Ascending order)
                        var oBufferKeys = bufferKeys.OrderBy(o => o).ToArray();
                        indexes = GetIndexes(slotKeys, slotCount, startIndex, oBufferKeys, count);
                        countLimited = count > 0 && indexes != null && indexes.Length >= count;
                    }
                    finally
                    {
                        ArrayPool<int>.Shared.Return(slotKeys);
                    }

                    // 3) Under the lock: copy the matched observations, adjusting for any observations added since the
                    // snapshot (each eviction shifts logical indexes down by one; if a match was evicted, the query is redone)
                    if (indexes != null && indexes.Length > 0)
                    {
                        lock (_lock)
                        {
                            // The archive was replaced (e.g. cleared) since the snapshot
                            if (!ReferenceEquals(snapshotBuffer, _archiveObservations)) indexes = Array.Empty<int>();

                            var shift = _archiveObservations.Evicted - evicted;
                            var found = new BufferObservation[indexes.Length];
                            var foundCount = 0;

                            for (var i = 0; i < indexes.Length; i++)
                            {
                                var index = indexes[i] - shift;
                                if (index < 0)
                                {
                                    // Overwritten by new observations while the keys were matched outside the lock
                                    evictedDuringScan = true;
                                    break;
                                }
                                if (index >= _archiveObservations.Size) continue;

                                var i32 = (int)index;
                                if (i32 < lowestIndex) lowestIndex = i32;
                                if (i32 > highestIndex) highestIndex = i32;

                                found[foundCount++] = _archiveObservations[i32];
                            }

                            if (foundCount > 0)
                            {
                                if (foundCount < found.Length) Array.Resize(ref found, foundCount);
                                observations = found;
                                observationCount = (uint)foundCount;

                                firstObservationSequence = _archiveObservations[lowestIndex]._sequence;
                                lastObservationSequence = _archiveObservations[highestIndex]._sequence;

                                // When "count" cut the results short, the next request continues after the last one returned
                                if (countLimited) nextSequence = lastObservationSequence + 1;
                            }
                        }
                    }
                }

                // Observations in the range were overwritten during the scan: run the query again while holding the
                // lock (nothing can be overwritten then), so the results start where the header says they do
                if (evictedDuringScan)
                {
                    lock (_lock) return GetObservations(bufferKeys, from, to, count);
                }

                // When "to" ended the range before the newest observation, the next request continues after the range
                if (!countLimited && rangeEndSequence > 0 && rangeEndSequence + 1 < nextSequence) nextSequence = rangeEndSequence + 1;

                // Return a StreamingResults struct containing the observations found
                // as well as the sequences that were active when the observations were read
                return new ObservationBufferResults
                {
                    FirstSequence = firstSequence,
                    LastSequence = lastSequence,
                    NextSequence = nextSequence,
                    Observations = observations,
                    FirstObservationSequence = firstObservationSequence,
                    LastObservationSequence = lastObservationSequence,
                    ObservationCount = observationCount
                };
            }

            return ObservationBufferResults.Invalid();
        }

        #endregion

        #region "Add"

        #region "Internal"

        /// <summary>
        /// Wraps an observation as a <see cref="BufferObservation"/> and records it as the current value for its buffer key.
        /// </summary>
        /// <param name="bufferKey">The packed device/data-item buffer key.</param>
        /// <param name="sequence">The buffer sequence number to assign.</param>
        /// <param name="observation">The observation to make current.</param>
        protected void AddCurrentObservation(int bufferKey, ulong sequence, IObservation observation)
        {
            var bufferObservation = new BufferObservation(bufferKey, sequence, observation);
            AddCurrentObservation(bufferObservation);
        }

        /// <summary>
        /// Records the given buffer observation as the current value for its key, replacing any previous current value and notifying durable subclasses.
        /// </summary>
        /// <param name="observation">The buffer observation to make current.</param>
        protected void AddCurrentObservation(BufferObservation observation)
        {
            if (!observation.Values.IsNullOrEmpty())
            {
                // Get Reset Triggered Value from Observation
                var resetTriggered = observation.GetValue(ValueKeys.ResetTriggered).ConvertEnum<ResetTriggered>();

                // Check for UNAVAILABLE
                var isUnavailable = observation.GetValue(ValueKeys.Result) == Observation.Unavailable;

                _currentObservations.TryGetValue(observation._key, out var existingObservation);
                _currentObservations.Remove(observation._key);

                if (existingObservation.IsValid && !isUnavailable)
                {
                    if (resetTriggered == ResetTriggered.NOT_SPECIFIED)
                    {
                        // Update Observations based on Representation
                        switch (observation.Representation)
                        {
                            case DataItemRepresentation.DATA_SET:

                                // Update DataSet Values
                                var existingDataSetValues = GetDataSetValues(ref existingObservation);
                                if (!existingDataSetValues.IsNullOrEmpty())
                                {
                                    observation.Values = CombineDataSetValues(observation.Values, ref existingDataSetValues);
                                }

                                break;

                            case DataItemRepresentation.TABLE:

                                // Update Table Values
                                var existingTableValues = GetTableValues(ref existingObservation);
                                if (!existingTableValues.IsNullOrEmpty())
                                {
                                    observation.Values = CombineTableValues(observation.Values, ref existingTableValues);
                                }
                                break;
                        }
                    }
                }

                _currentObservations.Add(observation._key, observation);

                // Call Overridable Methods
                OnCurrentObservationAdd(ref observation);
            }
        }

        /// <summary>
        /// Wraps a condition observation as a <see cref="BufferObservation"/> and updates the active condition set for its buffer key.
        /// </summary>
        /// <param name="bufferKey">The packed device/data-item buffer key.</param>
        /// <param name="sequence">The buffer sequence number to assign.</param>
        /// <param name="observation">The condition observation to record.</param>
        protected void AddCurrentCondition(int bufferKey, ulong sequence, IObservation observation)
        {
            var bufferObservation = new BufferObservation(bufferKey, sequence, observation);
            AddCurrentCondition(bufferObservation);
        }

        /// <summary>
        /// Updates the active condition set for the observation's key, applying normal/fault accumulation semantics and notifying durable subclasses.
        /// </summary>
        /// <param name="observation">The condition buffer observation to record.</param>
        protected void AddCurrentCondition(BufferObservation observation)
        {
            if (!observation.Values.IsNullOrEmpty())
            {
                var bufferObservations = new List<BufferObservation>();

                _currentConditions.TryGetValue(observation._key, out var existingObservations);
                _currentConditions.Remove(observation._key);

                if (!existingObservations.IsNullOrEmpty())
                {
                    var conditionLevel = observation.GetValue(ValueKeys.Level);
                    var nativeCode = observation.GetValue(ValueKeys.NativeCode);

                    if (!(conditionLevel == _levelNormal && string.IsNullOrEmpty(nativeCode)) &&
                        conditionLevel != _levelUnavailable)
                    {
                        foreach (var existingObservation in existingObservations)
                        {
                            var existingLevel = existingObservation.GetValue(ValueKeys.Level);
                            var existingNativeCode = existingObservation.GetValue(ValueKeys.NativeCode);

                            if (existingNativeCode != nativeCode &&
                                existingLevel != _levelUnavailable &&
                                existingObservation.Sequence != observation.Sequence)
                            {
                                bufferObservations.Add(existingObservation);
                            }
                        }
                    }
                }

                // Add the new Observation
                bufferObservations.Add(observation);

                // If any WARNING or FAULT states present, then remove any NORMAL states
                // Current should only show the active states
                if (bufferObservations.Any(o =>
                        {
                            var level = o.GetValue(ValueKeys.Level);
                            return level == _levelWarning || level == _levelFault;
                        }))
                {
                    bufferObservations.RemoveAll(o => o.GetValue(ValueKeys.Level) == _levelNormal);
                }

                IEnumerable<BufferObservation> iBufferObservations = bufferObservations;

                // Add to stored List
                _currentConditions.Add(observation._key, iBufferObservations);

                // Call Overridable Method (only build the full copy when a derived class uses it)
                if (HasConditionChangeOverride()) OnCurrentConditionChange(GetCurrentConditions());
                OnCurrentConditionAdd(ref iBufferObservations);
            }
        }


        // Existing values (minus any whose Table entry is replaced), with each new value replacing the existing value
        // that has the same key or being appended; preserves the order of the previous RemoveAll/Add approach
        private static List<ObservationValue> MergeValues(ObservationValue[] existingValues, ObservationValue[] values, HashSet<string> replacedEntries)
        {
            var result = new List<ObservationValue>(existingValues.Length + values.Length);
            foreach (var existing in existingValues)
            {
                if (replacedEntries != null && replacedEntries.Contains(ValueKeys.GetTableKey(existing._key))) continue;
                result.Add(existing);
            }

            // Last write wins for a key; a replaced key moves to the end (as RemoveAll + Add did)
            var positions = new Dictionary<string, int>(result.Count + values.Length);
            for (var i = 0; i < result.Count; i++) if (result[i]._key != null) positions[result[i]._key] = i;

            var removed = new bool[result.Count + values.Length];
            foreach (var value in values)
            {
                if (value._key != null && positions.TryGetValue(value._key, out var position)) removed[position] = true;

                positions[value._key ?? string.Empty] = result.Count;
                result.Add(value);
            }

            var merged = new List<ObservationValue>(result.Count);
            for (var i = 0; i < result.Count; i++)
            {
                if (!removed[i]) merged.Add(result[i]);
            }

            return merged;
        }

        // Replaces any value with the same key (moving it to the end)
        private static void SetValue(List<ObservationValue> values, ObservationValue value)
        {
            values.RemoveAll(o => o._key == value._key);
            values.Add(value);
        }

        private static ObservationValue[] CombineDataSetValues(ObservationValue[] values, ref ObservationValue[] existingValues)
        {
            if (!values.IsNullOrEmpty() && !existingValues.IsNullOrEmpty())
            {
                // New values replace existing values with the same key (single pass via a key index)
                var returnValues = MergeValues(existingValues, values, null);

                // Set Count
                var count = 0;
                foreach (var value in returnValues)
                {
                    if (ValueKeys.IsDataSetKey(value._key)) count++;
                }

                // Add Count Value
                SetValue(returnValues, new ObservationValue(ValueKeys.Count, count));

                // Sort by Key Asc (needed for processing later on, and maybe better performance when searching)
                return BufferObservation.SortValues(returnValues);
            }

            return values;
        }

        private static ObservationValue[] CombineTableValues(ObservationValue[] values, ref ObservationValue[] existingValues)
        {
            if (!values.IsNullOrEmpty() && !existingValues.IsNullOrEmpty())
            {
                // Table entries present in the new values replace the whole existing entry (all of its cells)
                HashSet<string> replacedEntries = null;
                foreach (var value in values)
                {
                    if (value._key != null && value._key.StartsWith(ValueKeys.TablePrefix))
                    {
                        if (replacedEntries == null) replacedEntries = new HashSet<string>();
                        replacedEntries.Add(ValueKeys.GetTableKey(value._key));
                    }
                }

                var returnValues = MergeValues(existingValues, values, replacedEntries);

                // Set Count (number of distinct Table entries)
                var entries = new HashSet<string>();
                foreach (var value in returnValues)
                {
                    if (value._key != null && value._key.StartsWith(ValueKeys.TablePrefix)) entries.Add(ValueKeys.GetTableKey(value._key));
                }

                // Add Count Value
                SetValue(returnValues, new ObservationValue(ValueKeys.Count, entries.Count));

                // Sort by Key Asc (needed for processing later on, and maybe better performance when searching)
                return BufferObservation.SortValues(returnValues);
            }

            return values;
        }

        private static ObservationValue[] GetDataSetValues(ref BufferObservation observation)
        {
            if (!observation.Values.IsNullOrEmpty())
            {
                var x = observation.Values.Where(o => o._key != null && o._key.StartsWith(ValueKeys.DataSetPrefix));
                if (x != null) return x.ToArray();
            }

            return null;
        }

        private static ObservationValue[] GetTableValues(ref BufferObservation observation)
        {
            if (!observation.Values.IsNullOrEmpty())
            {
                var x = observation.Values.Where(o => o._key != null && o._key.StartsWith(ValueKeys.TablePrefix));
                if (x != null) return x.ToArray();
            }

            return null;
        }


        /// <summary>
        /// Wraps an observation as a <see cref="BufferObservation"/> and appends it to the circular archive.
        /// </summary>
        /// <param name="bufferKey">The packed device/data-item buffer key.</param>
        /// <param name="sequence">The buffer sequence number to assign.</param>
        /// <param name="observation">The observation to archive.</param>
        protected void AddBufferObservation(int bufferKey, ulong sequence, IObservation observation)
        {
            var bufferObservation = new BufferObservation(bufferKey, sequence, observation);
            AddBufferObservation(ref bufferObservation);
        }

        /// <summary>
        /// Appends a single buffer observation to the circular archive and notifies durable subclasses.
        /// </summary>
        /// <param name="observation">The buffer observation to archive, passed by reference to avoid copying.</param>
        protected void AddBufferObservation(ref BufferObservation observation)
        {
            // CircularBuffer is not thread-safe; subclasses can call this without holding _lock (re-entrant when they do)
            lock (_lock)
            {
                WriteObservation(ref observation);

                // Call Overridable Method
                OnBufferObservationAdd(ref observation);
            }
        }

        /// <summary>
        /// Appends a batch of buffer observations to the circular archive in order, notifying durable subclasses for each.
        /// </summary>
        /// <param name="observations">The buffer observations to archive, passed by reference to avoid copying.</param>
        protected void AddBufferObservations(ref BufferObservation[] observations)
        {
            if (observations != null && observations.Length > 0)
            {
                // CircularBuffer is not thread-safe; subclasses (e.g. the file buffer load) call this without holding _lock
                lock (_lock)
                {
                    for (var i = 0; i < observations.Length; i++)
                    {
                        WriteObservation(ref observations[i]);

                        // Call Overridable Method
                        OnBufferObservationAdd(ref observations[i]);
                    }
                }
            }
        }

        private void WriteObservation(ref BufferObservation observation)
        {
            if (_archiveObservations != null) _archiveObservations.Add(ref observation);
        }

        #endregion


        /// <summary>
        /// Add a new Observation to the Buffer
        /// </summary>
        /// <param name="observation">The Observation to Add</param>
        /// <returns>The Sequence number that the Observation was added to the Buffer at. A zero is returned if the Observation failed to be added</returns>
        public virtual ulong AddObservation(ref BufferObservation observation)
        {
            if (observation._key >= 0 && !observation._values.IsNullOrEmpty() && observation._timestamp > 0)
            {
                lock (_lock)
                {
                    // Get the Sequence to Add Observation at
                    ulong sequence = _sequence++;

                    observation._sequence = sequence;

                    if (observation.Category == DataItemCategory.CONDITION)
                    {
                        // Add to Current Conditions
                        AddCurrentCondition(observation);
                    }
                    else
                    {
                        // Add to Current Observations
                        AddCurrentObservation(observation);
                    }

                    // Add to Buffer                   
                    AddBufferObservation(ref observation);

                    return sequence;
                }
            }

            return 0;
        }

        #endregion

    }
}