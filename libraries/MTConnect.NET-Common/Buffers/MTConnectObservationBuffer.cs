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
        public const int HighPriorityGC = 5000; // Minimum number of Observations requested that will trigger a High Priority Garbage Collection

        private readonly string _id = Guid.NewGuid().ToString();
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
                lock (_lock) return _archiveObservations[0].Sequence > 0 ? _archiveObservations[0].Sequence : 1;
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


        public IDictionary<int, BufferObservation> CurrentObservations
        {
            get
            {
                lock (_lock) return _currentObservations;
            }
        }

        public IDictionary<int, IEnumerable<BufferObservation>> CurrentConditions
        {
            get
            {
                lock (_lock) return _currentConditions;
            }
        }


        public MTConnectObservationBuffer()
        {
            _archiveObservations = new CircularBuffer(BufferSize);
        }

        public MTConnectObservationBuffer(IAgentConfiguration configuration)
        {
            if (configuration != null)
            {
                BufferSize = configuration.ObservationBufferSize;
            }

            _archiveObservations = new CircularBuffer(BufferSize);
        }

        public void Dispose()
        {
            _archiveObservations = null;
            _currentObservations.Clear();
            _currentConditions.Clear();
        }


        protected virtual void OnCurrentConditionChange(IEnumerable<BufferObservation> observations) { }

        protected virtual void OnCurrentObservationAdd(ref BufferObservation observation) { }

        protected virtual void OnCurrentConditionAdd(ref IEnumerable<BufferObservation> observations) { }

        protected virtual void OnBufferObservationAdd(ref BufferObservation observation) { }



        #region "Sequence"

        /// <summary>
        /// Sets the Agent's Sequence to the specified value
        /// </summary>
        protected void SetSequence(ulong sequence)
        {
            lock (_lock)
            {
                _sequence = sequence;
            }
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

        protected IEnumerable<BufferObservation> GetCurrentObservations()
        {
            var x = new List<BufferObservation>();

            lock (_lock)
            {
                var observations = _currentObservations.Values;
                if (!observations.IsNullOrEmpty()) x.AddRange(observations);
            }

            return x;
        }

        protected IEnumerable<BufferObservation> GetCurrentConditions()
        {
            var x = new List<BufferObservation>();

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

        private static int[] GetIndexes(ref CircularBuffer observations, int[] keys, int fromIndex, int toIndex, uint count = 0)
        {
            if (observations != null && observations.Capacity > 0 && keys != null && keys.Length > 0)
            {
                uint totalCount = 0; // Total number of Observations that were matched
                int oi = fromIndex; // Observations Iterator
                var oil = Math.Min(toIndex, observations.Capacity - 1);

                uint max = observations.Capacity; // Max Observations length
                if (count > 0) max = Math.Min(count, max); // Ensure max length doesn't exceed array length

                int bki; // BufferKey Iterator
                int bkmax = keys.Length; // Max BufferKey length

                // Create two-dimensional array to hold the indexes of each matched observation for each BufferKey
                var keyIndexes = new int[keys.Length, max];

                // Create two-dimensional array to hold the positions that were written in the keyIndexes array
                var keyPositions = new int[keys.Length, 1];


                // Loop through observations[]
                while (oi <= oil)
                {
                    // Reset BufferKey iterator
                    bki = 0;

                    // Loop through BufferKeys
                    while (bki < bkmax)
                    {
                        // Match BufferKey
                        if (observations[oi]._key == keys[bki])
                        {
                            // Get the Key position (last position in keyIndexes that was written)
                            var p = keyPositions[bki, 0];

                            // Write the Index to the keyIndexes array
                            keyIndexes[bki, p] = oi;

                            // Update the Key position
                            keyPositions[bki, 0] = p + 1;

                            totalCount++; // Increment the total found count
                            break; // Don't continue to loop through BufferKeys if already found
                        }

                        if (totalCount >= max) break; // Break if total found exceeds 'count' parameter
                        bki++; // Increment BufferKey iterator
                    }

                    if (totalCount >= max) break; // Break if total found exceeds 'count' parameter
                    oi++; // Increment observation iterator
                }

                // If any Observations were Matched
                if (totalCount > 0)
                {
                    // Single dimensional array to return containing the indexes of matched observations in the array passed as a parameter
                    var indexes = new int[totalCount];
                    var i = 0; // indexes iterator
                    bki = 0; // Reset BufferKey iterator

                    int p; // iterator for keyIndexes position
                    int pl; // max length of keyIndexes to iterate over

                    // Loop through the BufferKeys
                    // BufferKeys are ordered in ascending order.
                    // This order is maintained in the returned array so that additional sorting should never be required
                    while (bki < bkmax)
                    {
                        p = 0; // Reset position iterator
                        pl = keyPositions[bki, 0]; // Set max position length (number of observations found for that BufferKey

                        // Loop through found observations for this BufferKey
                        while (p < pl)
                        {
                            // Write the index of the observation to the return array
                            indexes[i] = keyIndexes[bki, p];

                            p++; // increment the position iterator
                            i++; // increment the indexes iterator
                        }

                        bki++; // Increment BufferKey iterator
                    }

                    return indexes;
                }
            }

            return null;
        }

        private static int GetLatestIndex(ref CircularBuffer observations, int key, int atIndex)
        {
            var index = -1;

            if (observations != null && observations.Capacity > 0 && atIndex <= observations.Capacity - 1)
            {
                // Loop through the observations in reverse order (sequence desc)
                // This find the most recent observation in the buffer
                for (var i = atIndex; i >= 0; i--)
                {
                    // Find the matching Buffer Key
                    if (observations[i]._key == key)
                    {
                        // If found, set the index and break from the loop
                        index = i;
                        break;
                    }
                }
            }

            return index;
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
                    firstSequence = _sequence > BufferSize ? _sequence - BufferSize : 1;
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
                var observations = new BufferObservation[bufferKeys.Count()];
                CircularBuffer bufferObservations = null;

                lock (_lock)
                {
                    firstSequence = _sequence > BufferSize ? _sequence - BufferSize : 1;
                    lastSequence = _sequence > 1 ? _sequence - 1 : 1;
                    nextSequence = _sequence;

                    // Determine Indexes
                    int atIndex;
                    if (at < 1) atIndex = 0;
                    else atIndex = (int)(at - firstSequence);

                    if (_archiveObservations.Size > 0)
                    {
                        bufferObservations = _archiveObservations;

                        var indexes = new int[oBufferKeys.Length];
                        for (var i = 0; i < oBufferKeys.Length; i++)
                        {
                            // Get the latest observation based on the "at" parameter
                            indexes[i] = GetLatestIndex(ref bufferObservations, oBufferKeys[i], atIndex);
                        }

                        for (var i = 0; i < oBufferKeys.Length; i++)
                        {
                            var index = indexes[i];
                            if (index >= 0)
                            {
                                observations[i] = _archiveObservations[index];
                            }
                            else
                            {
                                // Get From Current Observations
                                if (_currentObservations.TryGetValue(oBufferKeys[i], out var observation))
                                {
                                    if (observation.IsValid)
                                    {
                                        if (at < 1 && observation.Sequence <= firstSequence) observations[i] = observation;
                                        else if (observation.Sequence <= at) observations[i] = observation;
                                    }
                                }

                                // Get From Current Observations
                                if (_currentConditions.TryGetValue(oBufferKeys[i], out var conditions))
                                {
                                    if (observation.IsValid)
                                    {
                                        if (at < 1 && observation.Sequence <= firstSequence) observations[i] = observation;
                                        else if (observation.Sequence <= at) observations[i] = observation;
                                    }
                                }
                            }
                        }
                    }
                }

                var aObservations = observations.ToArray();
                var firstObservationSequence = observations.Min(o => o.Sequence);
                var lastObservationSequence = observations.Max(o => o.Sequence);

                return new ObservationBufferResults
                {
                    FirstSequence = firstSequence,
                    LastSequence = lastSequence,
                    NextSequence = nextSequence,
                    Observations = aObservations,
                    FirstObservationSequence = firstObservationSequence,
                    LastObservationSequence = lastObservationSequence,
                    ObservationCount = (uint)aObservations.Length
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
                CircularBuffer bufferObservations = null;
                ulong firstObservationSequence = 0;
                ulong lastObservationSequence = 0;
                int lowestIndex = int.MaxValue;
                int highestIndex = 0;

                lock (_lock)
                {
                    firstSequence = _sequence > BufferSize ? _sequence - BufferSize : 1;
                    lastSequence = _sequence > 1 ? _sequence - 1 : 1;
                    nextSequence = _sequence;

                    // Determine Indexes
                    int fromIndex = from > firstSequence ? (int)(from - firstSequence) : 0;
                    int toIndex = (int)(lastSequence - firstSequence);
                    if (to > 0)
                    {
                        if (to == from) toIndex = fromIndex;
                        else
                        {
                            toIndex = (int)(to - firstSequence);
                            if (toIndex < 0) toIndex = _archiveObservations.Size - 1;
                        }
                    }

                    //if (_archiveObservations.Size > 0 && toIndex <= _archiveObservations.Size && toIndex >= fromIndex)
                    if (_archiveObservations.Size > 0 && toIndex < _archiveObservations.Size && toIndex >= fromIndex)
                    {
                        bufferObservations = _archiveObservations;

                        // Order Buffer Keys (this effects the order of the resulting array and should always be in Ascending order)
                        var oBufferKeys = bufferKeys.OrderBy(o => o).ToArray();

                        // Get list of indexes that match the requested Keys
                        var indexes = GetIndexes(ref bufferObservations, oBufferKeys, fromIndex, toIndex, count);
                        if (indexes != null && indexes.Length > 0)
                        {
                            // Initialize array to return using the number of indexes found as the size
                            observations = new BufferObservation[indexes.Length];
                            var observationIndex = 0; // return array iterator

                            for (var i = 0; i < indexes.Length; i++)
                            {
                                // Get the index of the observation in the _readBuffer array
                                var index = indexes[i];
                                if (index < lowestIndex) lowestIndex = index;
                                if (index > highestIndex) highestIndex = index;

                                // Write the observation at the _readBuffer index to the return array
                                observations[observationIndex] = _archiveObservations[index];

                                observationIndex++; // Increment the return array iterator
                                observationCount++; // Increment the total observations matched count
                            }

                            firstObservationSequence = _archiveObservations[lowestIndex].Sequence;
                            lastObservationSequence = _archiveObservations[highestIndex].Sequence;
                        }
                    }
                }

                // Trigger Garbage Collector to Collect()
                if (observationCount > HighPriorityGC) GarbageCollector.HighPriorityCollect();
                else GarbageCollector.LowPriorityCollect();

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

        protected void AddCurrentObservation(int bufferKey, ulong sequence, IObservation observation)
        {
            var bufferObservation = new BufferObservation(bufferKey, sequence, observation);
            AddCurrentObservation(bufferObservation);
        }

        protected void AddCurrentObservation(BufferObservation observation)
        {
            if (!observation.Values.IsNullOrEmpty())
            {
                // Get Reset Triggered Value from Observation
                var resetTriggered = observation.GetValue(ValueKeys.ResetTriggered).ConvertEnum<ResetTriggered>();

                // Check for UNAVAILABLE
                var isUnavailable = observation.GetValue(ValueKeys.Result) == Observation.Unavailable;

                BufferObservation existingObservation;
                lock (_lock)
                {
                    _currentObservations.TryGetValue(observation._key, out existingObservation);
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
                }

                // Call Overridable Methods
                OnCurrentObservationAdd(ref observation);
            }
        }

        protected void AddCurrentCondition(int bufferKey, ulong sequence, IObservation observation)
        {
            var bufferObservation = new BufferObservation(bufferKey, sequence, observation);
            AddCurrentCondition(bufferObservation);
        }

        protected void AddCurrentCondition(BufferObservation observation)
        {
            if (!observation.Values.IsNullOrEmpty())
            {
                var bufferObservations = new List<BufferObservation>();

                IEnumerable<BufferObservation> existingObservations;
                IEnumerable<BufferObservation> iBufferObservations;
                lock (_lock)
                {
                    _currentConditions.TryGetValue(observation._key, out existingObservations);
                    _currentConditions.Remove(observation._key);

                    if (!existingObservations.IsNullOrEmpty())
                    {
                        var conditionLevel = observation.GetValue(ValueKeys.Level);
                        var nativeCode = observation.GetValue(ValueKeys.NativeCode);

                        if (!(conditionLevel == ConditionLevel.NORMAL.ToString() && string.IsNullOrEmpty(nativeCode)) &&
                            conditionLevel != ConditionLevel.UNAVAILABLE.ToString())
                        {
                            foreach (var existingObservation in existingObservations)
                            {
                                var existingLevel = existingObservation.GetValue(ValueKeys.Level);
                                var existingNativeCode = existingObservation.GetValue(ValueKeys.NativeCode);

                                if (existingNativeCode != nativeCode &&
                                    existingLevel != ConditionLevel.UNAVAILABLE.ToString() &&
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
                            o.GetValue(ValueKeys.Level) == ConditionLevel.WARNING.ToString() ||
                            o.GetValue(ValueKeys.Level) == ConditionLevel.FAULT.ToString()))
                    {
                        bufferObservations.RemoveAll(o =>
                            o.GetValue(ValueKeys.Level) == ConditionLevel.NORMAL.ToString());
                    }

                    iBufferObservations = bufferObservations;

                    // Add to stored List
                    _currentConditions.Add(observation._key, iBufferObservations);
                }

                // Call Overridable Method
                OnCurrentConditionChange(GetCurrentConditions());
                OnCurrentConditionAdd(ref iBufferObservations);
            }
        }


        private static ObservationValue[] CombineDataSetValues(ObservationValue[] values, ref ObservationValue[] existingValues)
        {
            if (!values.IsNullOrEmpty() && !existingValues.IsNullOrEmpty())
            {
                var returnValues = new List<ObservationValue>();
                returnValues.AddRange(existingValues);

                // Add Existing Values (if not in new values)
                foreach (var value in values)
                {
                    returnValues.RemoveAll(o => o._key == value._key);
                    returnValues.Add(value);
                }

                // Set Count
                var count = 0;
                foreach (var value in returnValues)
                {
                    if (ValueKeys.IsDataSetKey(value._key)) count++;
                }

                // Add Count Value
                returnValues.RemoveAll(o => o._key == ValueKeys.Count);
                returnValues.Add(new ObservationValue(ValueKeys.Count, count));

                // Sort by Key Asc (needed for processing later on, and maybe better performance when searching)
                return returnValues.OrderBy(o => o._key).ToArray();
            }

            return values;
        }

        private static ObservationValue[] CombineTableValues(ObservationValue[] values, ref ObservationValue[] existingValues)
        {
            if (!values.IsNullOrEmpty() && !existingValues.IsNullOrEmpty())
            {
                var returnValues = new List<ObservationValue>();
                returnValues.AddRange(existingValues);

                // Remove duplicate Values
                var tableValues = values.Where(o => o._key.StartsWith(ValueKeys.TablePrefix));
                if (!tableValues.IsNullOrEmpty())
                {
                    var entryKeys = tableValues.Select(o => ValueKeys.GetTableKey(o._key)).Distinct();
                    foreach (var entryKey in entryKeys)
                    {
                        returnValues.RemoveAll(o => ValueKeys.GetTableKey(o._key) == entryKey);
                    }
                }

                // Add new Values
                foreach (var value in values)
                {
                    returnValues.RemoveAll(o => o._key == value._key);
                    returnValues.Add(value);
                }

                // Set Count
                var count = 0;
                tableValues = returnValues.Where(o => o._key.StartsWith(ValueKeys.TablePrefix));
                if (!tableValues.IsNullOrEmpty())
                {
                    var entryKeys = tableValues.Select(o => ValueKeys.GetTableKey(o._key)).Distinct();
                    foreach (var entryKey in entryKeys)
                    {
                        count++;
                    }
                }

                // Add Count Value
                returnValues.RemoveAll(o => o._key == ValueKeys.Count);
                returnValues.Add(new ObservationValue(ValueKeys.Count, count));

                // Sort by Key Asc (needed for processing later on, and maybe better performance when searching)
                return returnValues.OrderBy(o => o._key).ToArray();
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


        protected void AddBufferObservation(int bufferKey, ulong sequence, IObservation observation)
        {
            var bufferObservation = new BufferObservation(bufferKey, sequence, observation);
            AddBufferObservation(ref bufferObservation);
        }

        protected void AddBufferObservation(ref BufferObservation observation)
        {
            WriteObservation(ref observation);

            // Call Overridable Method
            OnBufferObservationAdd(ref observation);
        }

        protected void AddBufferObservations(ref BufferObservation[] observations)
        {
            if (observations != null && observations.Length > 0)
            {
                for (var i = 0; i < observations.Length; i++)
                {
                    WriteObservation(ref observations[i]);

                    // Call Overridable Method
                    OnBufferObservationAdd(ref observations[i]);
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
                // Get the Sequence to Add Observation at
                ulong sequence;
                lock (_lock) sequence = _sequence++;

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

            return 0;
        }

        #endregion

    }
}