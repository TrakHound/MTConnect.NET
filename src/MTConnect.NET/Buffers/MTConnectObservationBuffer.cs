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
    public class MTConnectObservationBuffer : IMTConnectObservationBuffer
    {
        private readonly string _id = Guid.NewGuid().ToString();
        private object _lock = new object();
        private long _sequence = 1;
        private long _bufferIndex = 0;

        private readonly ConcurrentDictionary<string, StoredObservation> _currentObservations = new ConcurrentDictionary<string, StoredObservation>();
        private readonly ConcurrentDictionary<string, IEnumerable<StoredObservation>> _currentConditions = new ConcurrentDictionary<string, IEnumerable<StoredObservation>>();
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


        public MTConnectObservationBuffer()
        {
            _archiveObservations = new StoredObservation[BufferSize];
        }

        public MTConnectObservationBuffer(MTConnectAgentConfiguration configuration)
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
                            // Create a Hash using DeviceName and DataItemId
                            var hash = StoredObservation.CreateHash(query.DeviceName, dataItemId);

                            // Get From Current Observations
                            if (_currentObservations.TryGetValue(hash, out var observation))
                            {
                                observations.Add(observation);
                            }

                            // Get From Current Observations
                            if (_currentConditions.TryGetValue(hash, out var conditions))
                            {
                                observations.AddRange(conditions);
                            }
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

        private void AddCurrentObservation(StoredObservation observation)
        {
            if (_currentObservations != null && !string.IsNullOrEmpty(observation.DeviceName) && !string.IsNullOrEmpty(observation.DataItemId))
            {
                // Create a Hash using the DeviceName and the DataItemId
                var hash = StoredObservation.CreateHash(observation.DeviceName, observation.DataItemId);

                if (!observation.Values.IsNullOrEmpty())
                {
                    if (_currentObservations.TryRemove(hash, out var existingObservation))
                    {
                        // Update Observations based on Representation
                        switch (observation.DataItemRepresentation)
                        {
                            case Devices.DataItemRepresentation.DATA_SET:

                                // Update DataSet Values
                                var existingDataSetValues = GetDataSetValues(existingObservation);
                                if (!existingDataSetValues.IsNullOrEmpty())
                                {
                                    observation.Values = CombineDataSetValues(observation.Values, existingDataSetValues);
                                }
                                break;
                        }
                    }

                    _currentObservations.TryAdd(hash, observation);
                }
            }
        }

        private void AddCurrentCondition(StoredObservation observation)
        {
            if (_currentConditions != null && !string.IsNullOrEmpty(observation.DeviceName) && !string.IsNullOrEmpty(observation.DataItemId))
            {
                // Create a Hash using the DeviceName and the DataItemId
                var hash = StoredObservation.CreateHash(observation.DeviceName, observation.DataItemId);

                if (!observation.Values.IsNullOrEmpty())
                {
                    var observations = new List<StoredObservation>();

                    if (_currentConditions.TryRemove(hash, out var existingObservations))
                    {
                        // Only Add Existing Condition Observations if Not NORMAL or UNAVAILABLE
                        var conditionLevel = observation.Values.FirstOrDefault(o => o.ValueType == ValueTypes.Level).Value;
                        if (conditionLevel != Streams.ConditionLevel.NORMAL.ToString() &&
                            conditionLevel != Streams.ConditionLevel.UNAVAILABLE.ToString())
                        {
                            foreach (var existingObservation in existingObservations)
                            {
                                // Don't include existing NORMAL or UNAVAILABLE
                                conditionLevel = existingObservation.Values.FirstOrDefault(o => o.ValueType == ValueTypes.Level).Value;
                                if (conditionLevel != Streams.ConditionLevel.NORMAL.ToString() &&
                                    conditionLevel != Streams.ConditionLevel.UNAVAILABLE.ToString())
                                {
                                    observations.Add(existingObservation);
                                }
                            }
                        }
                    }

                    // Add the new Observation
                    observations.Add(observation);

                    // Add to stored List
                    _currentConditions.TryAdd(hash, observations);
                }
            }
        }

        private static IEnumerable<ObservationValue> CombineDataSetValues(IEnumerable<ObservationValue> values, IEnumerable<ObservationValue> existingValues)
        {
            if (!values.IsNullOrEmpty() && !existingValues.IsNullOrEmpty())
            {
                var returnValues = new List<ObservationValue>();
                returnValues.AddRange(existingValues);

                foreach (var value in values)
                {
                    returnValues.RemoveAll(o => o.ValueType == value.ValueType);
                    returnValues.Add(value);
                }

                return returnValues;
            }

            return values;
        }

        private static IEnumerable<ObservationValue> GetDataSetValues(StoredObservation observation)
        {
            if (!observation.Values.IsNullOrEmpty())
            {
                return observation.Values.Where(o => o.ValueType != null && o.ValueType.StartsWith(ValueTypes.DataSetPrefix));
            }

            return Enumerable.Empty<ObservationValue>();
        }

        private static IEnumerable<ObservationValue> GetTableValues(StoredObservation observation)
        {
            if (!observation.Values.IsNullOrEmpty())
            {
                return observation.Values.Where(o => o.ValueType != null && o.ValueType.StartsWith(ValueTypes.TablePrefix));
            }

            return Enumerable.Empty<ObservationValue>();
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
        /// <param name="observation">The Observation to Add</param>
        /// <param name="sequence">The sequence number to add the DataItem at</param>
        /// <returns>A boolean value indicating whether the Observation was added to the Buffer successfully (true) or not (false)</returns>
        public bool AddObservation(string deviceName, Devices.DataItem dataItem, IObservation observation)
        {
            if (!string.IsNullOrEmpty(deviceName) && dataItem != null && observation != null)
            {
                var storedObservation = new StoredObservation
                {
                    DeviceName = deviceName,
                    DataItemId = dataItem.Id,
                    DataItemCategory = dataItem.DataItemCategory,
                    DataItemRepresentation = dataItem.Representation,
                    Values = observation.Values,
                    Sequence = _sequence++,
                    Timestamp = observation.Timestamp
                };

                if (dataItem.DataItemCategory == Devices.DataItemCategory.CONDITION)
                {
                    // Add to Current Conditions
                    AddCurrentCondition(storedObservation);
                }
                else
                {
                    // Add to Current Observations
                    AddCurrentObservation(storedObservation);
                }

                // Add to Buffer                   
                AddBufferObservation(storedObservation);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Add a new Observation to the Buffer
        /// </summary>
        /// <param name="deviceName">The name of the Device the data is associated with</param>
        /// <param name="dataItemId">The ID of the Observation</param>
        /// <param name="observation">The Observation to Add</param>
        /// <param name="sequence">The sequence number to add the Observation at</param>
        /// <returns>A boolean value indicating whether the Observation was added to the Buffer successfully (true) or not (false)</returns>
        public async Task<bool> AddObservationAsync(string deviceName, Devices.DataItem dataItem, IObservation observation)
        {
            return AddObservation(deviceName, dataItem, observation);
        }

        #endregion

    }
}
