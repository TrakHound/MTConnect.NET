// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Agents.Configuration;
using MTConnect.Observations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTConnect.Buffers
{
    /// <summary>
    /// Buffer used to store Streaming Data
    /// </summary>
    public class MTConnectStreamingBuffer : IMTConnectStreamingBuffer
    {
        private readonly string _id = Guid.NewGuid().ToString();
        private object _lock = new object();
        private long _sequence = 1;
        private long _bufferIndex = 0;

        private readonly ConcurrentDictionary<string, IEnumerable<StoredObservation>> _currentObservations = new ConcurrentDictionary<string, IEnumerable<StoredObservation>>();
        private readonly StoredObservation[] _archiveObservations;

        /// <summary>
        /// Get a unique identifier for the Buffer
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// Get the configured size of the Buffer in the number of maximum number of Observations the buffer can hold at one time.
        /// </summary>
        public long BufferSize { get; set; } = 131072;

        /// <summary>
        /// A number representing the sequence number assigned to the oldest Observation stored in the buffer
        /// </summary>
        public long FirstSequence => _archiveObservations[0].Sequence;

        /// <summary>
        /// A number representing the sequence number assigned to the last Observation that was added to the buffer
        /// </summary>
        public long LastSequence => _sequence > 1 ? _sequence - 1 : 1;

        /// <summary>
        /// A number representing the sequence number of the Observation that is the next piece of data to be retrieved from the buffer
        /// </summary>
        public long NextSequence => _sequence;


        public MTConnectStreamingBuffer()
        {
            _archiveObservations = new StoredObservation[BufferSize];
        }

        public MTConnectStreamingBuffer(MTConnectAgentConfiguration configuration)
        {
            if (configuration != null)
            {
                BufferSize = configuration.BufferSize;
                _archiveObservations = new StoredObservation[configuration.BufferSize];
            }
            else
            {
                _archiveObservations = new StoredObservation[BufferSize];
            }
        }


        #region "Sequence"

        /// <summary>
        /// Increment the Agent's Sequence number by one
        /// </summary>
        public void IncrementSequence()
        {
            _sequence++;
        }

        /// <summary>
        /// Increment the Agent's Sequence number by the specified count
        /// </summary>
        public void IncrementSequence(int count)
        {
            _sequence += count;
        }

        #endregion

        #region "Get"

        #region "Internal"

        private StreamingResults GetObservations(
            IEnumerable<StreamingQuery> queries,
            IEnumerable<string> dataItemIds = null,
            long from = -1,
            long to = -1,
            long at = -1,
            int count = 0
            )
        {
            var objs = new List<StoredObservation>();
            long firstSequence = 0;
            long lastSequence = 0;
            long nextSequence = 0;
            int itemCount = 1;
            StoredObservation[] observations;

            lock (_lock)
            {
                firstSequence = Math.Max(1, _sequence - BufferSize);
                lastSequence = _sequence;
                nextSequence = _sequence + 1;

                var firstItem = _archiveObservations[0];
                var length = _sequence - firstItem.Sequence;

                observations = new StoredObservation[length];

                Array.Copy(_archiveObservations, 0, observations, 0, length);
            }

            if (!observations.IsNullOrEmpty())
            {
                foreach (var query in queries)
                {
                    foreach (var dataItemId in query.DataItemIds)
                    {
                        var dataItemObservations = GetStoredObservations(observations, query.DeviceName, dataItemId, dataItemIds, from, to, at);
                        if (!dataItemObservations.IsNullOrEmpty())
                        {
                            foreach (var observation in dataItemObservations)
                            {
                                objs.Add(observation);
                                itemCount++;
                                if (count > 0 && itemCount > count) break;
                            }
                        }

                        if (itemCount > count) break;
                    }

                    if (itemCount > count) break;
                }
            }

            return new StreamingResults(
                firstSequence,
                lastSequence,
                nextSequence,
                objs);
        }


        private string GetCurrentValue(string deviceName, string dataItemId, string valueType = ValueTypes.CDATA)
        {
            if (!string.IsNullOrEmpty(deviceName) && !string.IsNullOrEmpty(dataItemId) && !string.IsNullOrEmpty(valueType))
            {
                var hash = StoredObservation.CreateHash(deviceName, dataItemId);
                _currentObservations.TryGetValue(hash, out var observations);

                if (!observations.IsNullOrEmpty())
                {
                    var observation = observations.FirstOrDefault(o => o.ValueType == valueType);

                    return observation.Value != null ? observation.Value.ToString() : null;
                }
            }

            return null;
        }

        private StreamingResults GetCurrentObservations(IEnumerable<StreamingQuery> queries)
        {
            var observations = new List<StoredObservation>();
            long firstSequence = 0;
            long lastSequence = 0;
            long nextSequence = 0;

            lock (_lock)
            {
                firstSequence = Math.Max(1, _sequence - BufferSize);
                lastSequence = _sequence;
                nextSequence = _sequence + 1;

                foreach (var query in queries)
                {
                    if (!query.DataItemIds.IsNullOrEmpty())
                    {
                        foreach (var dataItemId in query.DataItemIds)
                        {
                            observations.AddRange(GetCurrentObservations(query.DeviceName, dataItemId));
                        }
                    }
                }
            }

            return new StreamingResults(
                firstSequence,
                lastSequence,
                nextSequence,
                observations);
        }

        private IEnumerable<StoredObservation> GetCurrentObservations(string deviceName, string dataItemId)
        {
            if (!string.IsNullOrEmpty(dataItemId))
            {
                var objs = new List<StoredObservation>();

                var hash = StoredObservation.CreateHash(deviceName, dataItemId);
                if (_currentObservations.TryGetValue(hash, out var x)) objs.AddRange(x);

                return objs;
            }

            return Enumerable.Empty<StoredObservation>();
        }

        private static IEnumerable<StoredObservation> GetStoredObservations(
            StoredObservation[] observations,
            string deviceName,
            string dataItemId,
            IEnumerable<string> dataItemIds = null,
            long from = -1,
            long to = -1,
            long at = -1,
            int count = 0
            )
        {
            if (!observations.IsNullOrEmpty() && !string.IsNullOrEmpty(dataItemId))
            {
                var objs = new List<StoredObservation>();

                IEnumerable<StoredObservation> y = null;

                if (from > 0 && to > 0)
                {
                    y = observations.Where(o => o.DeviceName == deviceName && o.DataItemId == dataItemId && o.Sequence >= from && o.Sequence < to);
                }
                else if (from > 0)
                {
                    y = observations.Where(o => o.DeviceName == deviceName && o.DataItemId == dataItemId && o.Sequence >= from);
                }
                else if (to > 0)
                {
                    y = observations.Where(o => o.DeviceName == deviceName && o.DataItemId == dataItemId && o.Sequence < to);
                }
                else if (at > 0)
                {
                    y = observations.Where(o => o.DeviceName == deviceName && o.DataItemId == dataItemId && o.Sequence == at);
                }
                else
                {
                    y = observations.Where(o => o.DeviceName == deviceName && o.DataItemId == dataItemId);
                }

                if (y != null) objs.AddRange(y);

                return objs;
            }

            return Enumerable.Empty<StoredObservation>();
        }

        #endregion


        /// <summary>
        /// Get a list of Observations based on the specified search parameters
        /// </summary>
        /// <param name="deviceName">The name of the Device</param>
        /// <param name="dataItemIds">A list of DataItemId's used to filter results</param>
        /// <param name="from">The minimum sequence number to include in the results</param>
        /// <param name="to">The maximum sequence number to include in the results</param>
        /// <param name="at">The sequence number to include in the results</param>
        /// <param name="count">The maximum number of Observations to include in the result</param>
        /// <returns>An object that implements the IStreamingResults interface containing the query results</returns>
        public IStreamingResults GetObservations(string deviceName, IEnumerable<string> dataItemIds = null, long from = -1, long to = -1, long at = -1, int count = 0)
        {
            if (!string.IsNullOrEmpty(deviceName))
            {
                return GetObservations(new List<string> { deviceName }, dataItemIds, from, to, at, count);
            }

            return null; ;
        }

        /// <summary>
        /// Get a list of Observations based on the specified search parameters
        /// </summary>
        /// <param name="deviceName">The name of the Device</param>
        /// <param name="dataItemIds">A list of DataItemId's used to filter results</param>
        /// <param name="from">The minimum sequence number to include in the results</param>
        /// <param name="to">The maximum sequence number to include in the results</param>
        /// <param name="at">The sequence number to include in the results</param>
        /// <param name="count">The maximum number of Observations to include in the result</param>
        /// <returns>An object that implements the IStreamingResults interface containing the query results</returns>
        public async Task<IStreamingResults> GetObservationsAsync(string deviceName, IEnumerable<string> dataItemIds = null, long from = -1, long to = -1, long at = -1, int count = 0)
        {
            if (!string.IsNullOrEmpty(deviceName))
            {
                return await GetObservationsAsync(new List<string> { deviceName }, dataItemIds, from, to, at, count);
            }

            return null; ;
        }

        /// <summary>
        /// Get a list of Observations based on the specified search parameters
        /// </summary>
        /// <param name="deviceNames">A list of Device Names to include in the results</param>
        /// <param name="dataItemIds">A list of DataItemId's used to filter results</param>
        /// <param name="from">The minimum sequence number to include in the results</param>
        /// <param name="to">The maximum sequence number to include in the results</param>
        /// <param name="at">The sequence number to include in the results</param>
        /// <param name="count">The maximum number of Observations to include in the result</param>
        /// <returns>An object that implements the IStreamingResults interface containing the query results</returns>
        public IStreamingResults GetObservations(IEnumerable<string> deviceNames, IEnumerable<string> dataItemIds = null, long from = -1, long to = -1, long at = -1, int count = 0)
        {
            if (!deviceNames.IsNullOrEmpty())
            {
                // Get List of DataItemIds for all Devices
                var queries = new List<StreamingQuery>();
                foreach (var deviceName in deviceNames)
                {
                    queries.Add(new StreamingQuery(deviceName, dataItemIds));
                }

                // Get the Observations stored in the Buffer based on the StreamingQueries
                StreamingResults results;
                if (from > 0 || to > 0 || at > 0)
                {
                    results = GetObservations(queries, dataItemIds, from, to, count: count);
                }
                else
                {
                    results = GetCurrentObservations(queries);
                }

                return results;
            }

            return null; ;
        }

        /// <summary>
        /// Get a list of Observations based on the specified search parameters
        /// </summary>
        /// <param name="deviceNames">A list of Device Names to include in the results</param>
        /// <param name="dataItemIds">A list of DataItemId's used to filter results</param>
        /// <param name="from">The minimum sequence number to include in the results</param>
        /// <param name="to">The maximum sequence number to include in the results</param>
        /// <param name="at">The sequence number to include in the results</param>
        /// <param name="count">The maximum number of Observations to include in the result</param>
        /// <returns>An object that implements the IStreamingResults interface containing the query results</returns>
        public async Task<IStreamingResults> GetObservationsAsync(IEnumerable<string> deviceNames, IEnumerable<string> dataItemIds = null, long from = -1, long to = -1, long at = -1, int count = 0)
        {
            if (!deviceNames.IsNullOrEmpty())
            {
                // Get List of DataItemIds for all Devices
                var queries = new List<StreamingQuery>();
                foreach (var deviceName in deviceNames)
                {
                    queries.Add(new StreamingQuery(deviceName, dataItemIds));
                }

                // Get the Observations stored in the Buffer based on the StreamingQueries
                StreamingResults results;
                if (from > 0 || to > 0 || at > 0)
                {
                    results = GetObservations(queries, dataItemIds, from, to, count: count);
                }
                else if (count > 0)
                {
                    results = GetObservations(queries, dataItemIds, count: count);
                }
                else
                {
                    results = GetCurrentObservations(queries);
                }

                return results;
            }

            return null; ;
        }

        #endregion

        #region "Add"

        #region "Internal"

        private void AddCurrentObservations(string deviceName, string dataItemId, IEnumerable<StoredObservation> observations)
        {
            if (_currentObservations != null && !string.IsNullOrEmpty(deviceName) && !string.IsNullOrEmpty(dataItemId))
            {
                var hash = StoredObservation.CreateHash(deviceName, dataItemId);

                _currentObservations.TryGetValue(hash, out var existingObservations);
                if (!observations.IsNullOrEmpty() && !existingObservations.IsNullOrEmpty())
                {
                    var existingTimestamp = existingObservations.FirstOrDefault().Timestamp;
                    var timestamp = observations.FirstOrDefault().Timestamp;

                    // If new observations are newer than existing then add to buffer
                    if (timestamp > existingTimestamp)
                    {
                        _currentObservations.TryRemove(hash, out var _);
                        _currentObservations.TryAdd(hash, observations);
                    }
                }
                else
                {
                    _currentObservations.TryRemove(hash, out var _);
                    _currentObservations.TryAdd(hash, observations);
                }
            }          
        }

        private void AddBufferObservation(StoredObservation observation)
        {
            if (_bufferIndex >= BufferSize - 1)
            {
                lock (_lock)
                {
                    var a = _archiveObservations;
                    Array.Copy(a, 1, _archiveObservations, 0, a.Length - 1);
                    _archiveObservations[_archiveObservations.Length - 1] = observation;
                    _bufferIndex++;
                }
            }
            else
            {
                lock (_lock)
                {
                    _archiveObservations[_bufferIndex] = observation;
                    _bufferIndex++;
                }
            }
        }

        private void AddBufferObservations(IEnumerable<StoredObservation> observations)
        {
            if (!observations.IsNullOrEmpty())
            {
                foreach (var observation in observations) AddBufferObservation(observation);
            }
        }

        #endregion

        /// <summary>
        /// Add a new Observation to the Buffer
        /// </summary>
        /// <param name="deviceName">The name of the Device the data is associated with</param>
        /// <param name="dataItemId">The ID of the DataItem</param>
        /// <param name="valueType">The ValueType that the Data represents</param>
        /// <param name="value">The Value of the Data</param>
        /// <param name="timestamp">The timestamp (in UnixTime milliseconds) that represents when the data was recorded</param>
        /// <param name="sequence">The sequence number to add the DataItem at</param>
        /// <returns>A boolean value indicating whether the Observation was added to the Buffer successfully (true) or not (false)</returns>
        public bool AddObservation(string deviceName, string dataItemId, string valueType, object value, long timestamp, long sequence = 0)
        {
            if (!string.IsNullOrEmpty(deviceName) && !string.IsNullOrEmpty(dataItemId) && !string.IsNullOrEmpty(valueType))
            {
                var currentValue = GetCurrentValue(deviceName, dataItemId, valueType);
                var newValue = value != null ? value.ToString() : null;

                if (currentValue != newValue)
                {
                    var seq = sequence > 0 ? sequence : _sequence++;

                    var objs = new List<StoredObservation>
                        {
                            new StoredObservation
                            {
                                DeviceName = deviceName,
                                DataItemId = dataItemId,
                                ValueType = valueType,
                                Value = newValue,
                                Timestamp = timestamp,
                                Sequence = seq
                            }
                        };

                    var currentObservationsToAdd = new List<StoredObservation>();
                    currentObservationsToAdd.AddRange(objs);

                    // Add Existing Observations to current if sequence the same (needed for TIME_SERIES, DATA_SET, and TABLE)
                    var currentObservations = GetCurrentObservations(deviceName, dataItemId);
                    if (!currentObservations.IsNullOrEmpty())
                    {
                        foreach (var observation in currentObservations)
                        {
                            currentObservationsToAdd.Add(observation);
                            //if (observation.Sequence == seq) currentObservationsToAdd.Add(observation);
                        }
                    }

                    // Add to Current Observations
                    AddCurrentObservations(deviceName, dataItemId, currentObservationsToAdd);

                    // Add to Buffer                   
                    AddBufferObservations(currentObservationsToAdd);
                    //AddBufferObservations(objs);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Add a new Observation to the Buffer
        /// </summary>
        /// <param name="deviceName">The name of the Device the data is associated with</param>
        /// <param name="dataItemId">The ID of the Observation</param>
        /// <param name="valueType">The ValueType that the Observation represents</param>
        /// <param name="value">The Value of the Observation</param>
        /// <param name="timestamp">The timestamp (in UnixTime milliseconds) that represents when the Observation was recorded</param>
        /// <param name="sequence">The sequence number to add the Observation at</param>
        /// <returns>A boolean value indicating whether the Observation was added to the Buffer successfully (true) or not (false)</returns>
        public async Task<bool> AddObservationAsync(string deviceName, string dataItemId, string valueType, object value, long timestamp, long sequence = 0)
        {
            return AddObservation(deviceName, dataItemId, valueType, value, timestamp, sequence);
        }

        #endregion

    }
}
