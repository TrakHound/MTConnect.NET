// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Agents.Configuration;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTConnect.Buffers
{
    /// <summary>
    /// Circular Ephemeral Buffer used to store MTConnect Observation Data
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
        public long LastSequence
        {
            get
            {
                lock (_lock)
                {
                    return _sequence > 1 ? _sequence - 1 : 1;
                }
            }
        }

        /// <summary>
        /// A number representing the sequence number of the Observation that is the next piece of data to be retrieved from the buffer
        /// </summary>
        public long NextSequence
        {
            get
            {
                lock (_lock)
                {
                    return _sequence;
                }
            }
        }


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
        /// Sets the Agent's Sequence to the specified value
        /// </summary>
        protected void SetSequence(long sequence)
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
        public void IncrementSequence(int count)
        {
            lock (_lock)
            {
                _sequence += count;
            }
        }

        #endregion

        #region "Get"

        #region "Internal"

        private StreamingResults GetObservations(
            IEnumerable<StreamingQuery> queries,
            long from = -1,
            long to = -1,
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
                lastSequence = _sequence > 1 ? _sequence - 1 : 1;
                nextSequence = _sequence;

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
                        var dataItemObservations = GetStoredObservations(observations, query.DeviceUuid, dataItemId, from, to);
                        if (!dataItemObservations.IsNullOrEmpty())
                        {
                            foreach (var observation in dataItemObservations)
                            {
                                objs.Add(observation);
                                itemCount++;
                                if (count > 0 && itemCount > count) break;
                            }
                        }

                        if (count > 0 && itemCount > count) break;
                    }

                    if (count > 0 && itemCount > count) break;
                }
            }

            return new StreamingResults(
                firstSequence,
                lastSequence,
                nextSequence,
                objs);
        }


        private IEnumerable<StoredObservation> GetCurrentObservations()
        {
            var x = new List<StoredObservation>();

            var observations = _currentObservations.Values;
            if (!observations.IsNullOrEmpty()) x.AddRange(observations);

            var conditions = _currentConditions.Values;
            if (!conditions.IsNullOrEmpty())
            {
                foreach (var condition in conditions) x.AddRange(condition);
            }

            return x;
        }

        private StreamingResults GetCurrentObservations(IEnumerable<StreamingQuery> queries, long at = 0)
        {
            var observations = new List<StoredObservation>();
            long firstSequence = 0;
            long lastSequence = 0;
            long nextSequence = 0;
            StoredObservation[] storedObservations;


            lock (_lock)
            {
                var firstItem = _archiveObservations[0];
                var length = _sequence - firstItem.Sequence;
                if (length > BufferSize) length = BufferSize;

                storedObservations = new StoredObservation[length];

                Array.Copy(_archiveObservations, 0, storedObservations, 0, length);
            }

            lock (_lock)
            {
                firstSequence = Math.Max(1, _sequence - BufferSize);
                lastSequence = _sequence > 1 ? _sequence - 1 : 1;
                nextSequence = _sequence;

                foreach (var query in queries)
                {
                    if (!query.DataItemIds.IsNullOrEmpty())
                    {
                        foreach (var dataItemId in query.DataItemIds)
                        {
                            // Create a Hash using DeviceName and DataItemId
                            var hash = StoredObservation.CreateHash(query.DeviceUuid, dataItemId);

                            if (at > 0)
                            {
                                var storedObservation = GetStoredObservation(storedObservations, query.DeviceUuid, dataItemId, at);
                                if (storedObservation.IsValid)
                                {
                                    observations.Add(storedObservation);
                                }
                                else
                                {
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
                            else
                            {
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
            }

            return new StreamingResults(
                firstSequence,
                lastSequence,
                nextSequence,
                observations);
        }

        private static IEnumerable<StoredObservation> GetStoredObservations(
            StoredObservation[] observations,
            string deviceUuid,
            string dataItemId,
            long from = -1,
            long to = -1,
            int count = 0
            )
        {
            if (!observations.IsNullOrEmpty() && !string.IsNullOrEmpty(dataItemId))
            {
                var objs = new List<StoredObservation>();

                IEnumerable<StoredObservation> y = null;

                if (from > 0 && to > 0)
                {
                    y = observations.Where(o => o.DeviceUuid == deviceUuid && o.DataItemId == dataItemId && o.Sequence >= from && o.Sequence <= to);
                }
                else if (from > 0)
                {
                    y = observations.Where(o => o.DeviceUuid == deviceUuid && o.DataItemId == dataItemId && o.Sequence >= from);
                }
                else if (to > 0)
                {
                    y = observations.Where(o => o.DeviceUuid == deviceUuid && o.DataItemId == dataItemId && o.Sequence <= to);
                }
                else
                {
                    y = observations.Where(o => o.DeviceUuid == deviceUuid && o.DataItemId == dataItemId);
                }

                if (y != null) objs.AddRange(y);

                return objs;
            }

            return Enumerable.Empty<StoredObservation>();
        }

        private static StoredObservation GetStoredObservation(
            StoredObservation[] observations,
            string deviceUuid,
            string dataItemId,
            long at
            )
        {
            if (!observations.IsNullOrEmpty() && !string.IsNullOrEmpty(deviceUuid) && !string.IsNullOrEmpty(dataItemId))
            {
                var obj = observations.Where(o => o.DeviceUuid == deviceUuid && o.DataItemId == dataItemId && o.Sequence <= at).OrderByDescending(o => o.Sequence).FirstOrDefault();
                if (obj.IsValid) return obj;
            }

            return new StoredObservation();
        }

        #endregion


        /// <summary>
        /// Get a list of Observations based on the specified search parameters
        /// </summary>
        /// <param name="deviceUuid">The UUID of the Device</param>
        /// <param name="dataItemIds">A list of DataItemId's used to filter results</param>
        /// <param name="from">The minimum sequence number to include in the results</param>
        /// <param name="to">The maximum sequence number to include in the results</param>
        /// <param name="at">The sequence number to include in the results</param>
        /// <param name="count">The maximum number of Observations to include in the result</param>
        /// <returns>An object that implements the IStreamingResults interface containing the query results</returns>
        public virtual IStreamingResults GetObservations(string deviceUuid, IEnumerable<string> dataItemIds = null, long from = -1, long to = -1, long at = -1, int count = 0)
        {
            if (!string.IsNullOrEmpty(deviceUuid))
            {
                return GetObservations(new List<string> { deviceUuid }, dataItemIds, from, to, at, count);
            }

            return null; ;
        }

        /// <summary>
        /// Get a list of Observations based on the specified search parameters
        /// </summary>
        /// <param name="deviceUuid">The UUID of the Device</param>
        /// <param name="dataItemIds">A list of DataItemId's used to filter results</param>
        /// <param name="from">The minimum sequence number to include in the results</param>
        /// <param name="to">The maximum sequence number to include in the results</param>
        /// <param name="at">The sequence number to include in the results</param>
        /// <param name="count">The maximum number of Observations to include in the result</param>
        /// <returns>An object that implements the IStreamingResults interface containing the query results</returns>
        public virtual async Task<IStreamingResults> GetObservationsAsync(string deviceUuid, IEnumerable<string> dataItemIds = null, long from = -1, long to = -1, long at = -1, int count = 0)
        {
            if (!string.IsNullOrEmpty(deviceUuid))
            {
                return await GetObservationsAsync(new List<string> { deviceUuid }, dataItemIds, from, to, at, count);
            }

            return null; ;
        }

        /// <summary>
        /// Get a list of Observations based on the specified search parameters
        /// </summary>
        /// <param name="deviceUuids">A list of Device Uuids to include in the results</param>
        /// <param name="dataItemIds">A list of DataItemId's used to filter results</param>
        /// <param name="from">The minimum sequence number to include in the results</param>
        /// <param name="to">The maximum sequence number to include in the results</param>
        /// <param name="at">The sequence number to include in the results</param>
        /// <param name="count">The maximum number of Observations to include in the result</param>
        /// <returns>An object that implements the IStreamingResults interface containing the query results</returns>
        public virtual IStreamingResults GetObservations(IEnumerable<string> deviceUuids, IEnumerable<string> dataItemIds = null, long from = -1, long to = -1, long at = -1, int count = 0)
        {
            if (!deviceUuids.IsNullOrEmpty())
            {
                // Get List of DataItemIds for all Devices
                var queries = new List<StreamingQuery>();
                foreach (var deviceUuid in deviceUuids)
                {
                    queries.Add(new StreamingQuery(deviceUuid, dataItemIds));
                }

                // Get the Observations stored in the Buffer based on the StreamingQueries
                StreamingResults results;
                if (from > 0 || to > 0 || at > 0)
                {
                    results = GetObservations(queries, from, to, count: count);
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
        /// <param name="deviceUuids">A list of Device Uuids to include in the results</param>
        /// <param name="dataItemIds">A list of DataItemId's used to filter results</param>
        /// <param name="from">The minimum sequence number to include in the results</param>
        /// <param name="to">The maximum sequence number to include in the results</param>
        /// <param name="at">The sequence number to include in the results</param>
        /// <param name="count">The maximum number of Observations to include in the result</param>
        /// <returns>An object that implements the IStreamingResults interface containing the query results</returns>
        public virtual async Task<IStreamingResults> GetObservationsAsync(IEnumerable<string> deviceUuids, IEnumerable<string> dataItemIds = null, long from = -1, long to = -1, long at = -1, int count = 0)
        {
            if (!deviceUuids.IsNullOrEmpty())
            {
                // Get List of DataItemIds for all Devices
                var queries = new List<StreamingQuery>();
                foreach (var deviceUuid in deviceUuids)
                {
                    queries.Add(new StreamingQuery(deviceUuid, dataItemIds));
                }

                // Get the Observations stored in the Buffer based on the StreamingQueries
                StreamingResults results;
                if (from > 0 || to > 0)
                {
                    results = GetObservations(queries, from, to, count);
                }
                else if (count > 0)
                {
                    results = GetObservations(queries, count: count);
                }
                else if (at > 0)
                {
                    results = GetCurrentObservations(queries, at);
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

        protected void AddCurrentObservation(StoredObservation observation)
        {
            if (_currentObservations != null && !string.IsNullOrEmpty(observation.DeviceUuid) && !string.IsNullOrEmpty(observation.DataItemId))
            {
                // Create a Hash using the DeviceUuid and the DataItemId
                var hash = StoredObservation.CreateHash(observation.DeviceUuid, observation.DataItemId);

                if (!observation.Values.IsNullOrEmpty())
                {
                    // Get Reset Triggered Value from Observation
                    var resetTriggered = observation.GetValue(ValueKeys.ResetTriggered).ConvertEnum<ResetTriggered>();

                    if (_currentObservations.TryRemove(hash, out var existingObservation))
                    {
                        if (resetTriggered == ResetTriggered.NOT_SPECIFIED)
                        {
                            // Update Observations based on Representation
                            switch (observation.DataItemRepresentation)
                            {
                                case DataItemRepresentation.DATA_SET:

                                    // Update DataSet Values
                                    var existingDataSetValues = GetDataSetValues(existingObservation);
                                    if (!existingDataSetValues.IsNullOrEmpty())
                                    {
                                        observation.Values = CombineDataSetValues(observation.Values, existingDataSetValues);
                                    }
                                    break;

                                case DataItemRepresentation.TABLE:

                                    // Update Table Values
                                    var existingTableValues = GetTableValues(existingObservation);
                                    if (!existingTableValues.IsNullOrEmpty())
                                    {
                                        observation.Values = CombineTableValues(observation.Values, existingTableValues);
                                    }
                                    break;
                            }
                        }
                    }

                    _currentObservations.TryAdd(hash, observation);

                    // Call Overridable Methods
                    OnCurrentChange(GetCurrentObservations());
                    OnCurrentObservationAdd(observation);
                }
            }
        }

        protected void AddCurrentCondition(StoredObservation observation)
        {
            if (_currentConditions != null && !string.IsNullOrEmpty(observation.DeviceUuid) && !string.IsNullOrEmpty(observation.DataItemId))
            {
                // Create a Hash using the DeviceUuid and the DataItemId
                var hash = StoredObservation.CreateHash(observation.DeviceUuid, observation.DataItemId);

                if (!observation.Values.IsNullOrEmpty())
                {
                    var observations = new List<StoredObservation>();

                    if (_currentConditions.TryRemove(hash, out var existingObservations))
                    {
                        // Only Add Existing Condition Observations if Not NORMAL or UNAVAILABLE
                        var conditionLevel = observation.Values.FirstOrDefault(o => o.Key == ValueKeys.Level).Value;
                        if (conditionLevel != ConditionLevel.NORMAL.ToString() &&
                            conditionLevel != ConditionLevel.UNAVAILABLE.ToString())
                        {
                            foreach (var existingObservation in existingObservations)
                            {
                                // Don't include existing NORMAL or UNAVAILABLE
                                conditionLevel = existingObservation.Values.FirstOrDefault(o => o.Key == ValueKeys.Level).Value;
                                if (conditionLevel != ConditionLevel.NORMAL.ToString() &&
                                    conditionLevel != ConditionLevel.UNAVAILABLE.ToString())
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

                    // Call Overridable Method
                    OnCurrentChange(GetCurrentObservations());
                    OnCurrentConditionAdd(observations);
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
                    returnValues.RemoveAll(o => o.Key == value.Key);
                    returnValues.Add(value);
                }

                return returnValues;
            }

            return values;
        }

        private static IEnumerable<ObservationValue> CombineTableValues(IEnumerable<ObservationValue> values, IEnumerable<ObservationValue> existingValues)
        {
            if (!values.IsNullOrEmpty() && !existingValues.IsNullOrEmpty())
            {
                var returnValues = new List<ObservationValue>();
                returnValues.AddRange(existingValues);

                var tableValues = values.Where(o => o.Key.StartsWith(ValueKeys.TablePrefix));
                if (!tableValues.IsNullOrEmpty())
                {
                    var entryKeys = tableValues.Select(o => ValueKeys.GetTableKey(o.Key)).Distinct();
                    foreach (var entryKey in entryKeys)
                    {
                        returnValues.RemoveAll(o => ValueKeys.GetTableKey(o.Key) == entryKey);
                    }
                }
                
                foreach (var value in values)
                {
                    returnValues.RemoveAll(o => o.Key == value.Key);
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
                return observation.Values.Where(o => o.Key != null && o.Key.StartsWith(ValueKeys.DataSetPrefix));
            }

            return Enumerable.Empty<ObservationValue>();
        }

        private static IEnumerable<ObservationValue> GetTableValues(StoredObservation observation)
        {
            if (!observation.Values.IsNullOrEmpty())
            {
                return observation.Values.Where(o => o.Key != null && o.Key.StartsWith(ValueKeys.TablePrefix));
            }

            return Enumerable.Empty<ObservationValue>();
        }


        protected void AddBufferObservation(StoredObservation observation)
        {
            long bufferIndex = 0;

            if (_bufferIndex >= BufferSize - 1)
            {
                lock (_lock)
                {
                    var a = _archiveObservations;
                    Array.Copy(a, 1, _archiveObservations, 0, a.Length - 1);
                    _archiveObservations[_archiveObservations.Length - 1] = observation;
                    _bufferIndex++;
                    bufferIndex = _bufferIndex;
                }
            }
            else
            {
                lock (_lock)
                {
                    _archiveObservations[_bufferIndex] = observation;
                    _bufferIndex++;
                    bufferIndex = _bufferIndex;
                }
            }

            // Call Overridable Method
            OnBufferObservationAdd(bufferIndex, observation);
        }

        protected void AddBufferObservations(IEnumerable<StoredObservation> observations)
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
        /// <param name="deviceUuid">The UUID of the Device the data is associated with</param>
        /// <param name="dataItem">The DataItem that the Observation represents</param>
        /// <param name="observation">The Observation to Add</param>
        /// <returns>A boolean value indicating whether the Observation was added to the Buffer successfully (true) or not (false)</returns>
        public virtual bool AddObservation(string deviceUuid, IDataItem dataItem, IObservation observation)
        {
            if (!string.IsNullOrEmpty(deviceUuid) && dataItem != null && observation != null)
            {
                var storedObservation = new StoredObservation
                {
                    DeviceUuid = deviceUuid,
                    DataItemId = dataItem.Id,
                    DataItemCategory = dataItem.Category,
                    DataItemRepresentation = dataItem.Representation,
                    Values = observation.Values,
                    Sequence = _sequence++,
                    Timestamp = observation.Timestamp.ToUnixTime()                 
                };

                if (dataItem.Category == DataItemCategory.CONDITION)
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
        /// <param name="deviceUuid">The UUID of the Device the data is associated with</param>
        /// <param name="dataItem">The DataItem that the Observation represents</param>
        /// <param name="observation">The Observation to Add</param>
        /// <returns>A boolean value indicating whether the Observation was added to the Buffer successfully (true) or not (false)</returns>
        public virtual async Task<bool> AddObservationAsync(string deviceUuid, IDataItem dataItem, IObservation observation)
        {
            return AddObservation(deviceUuid, dataItem, observation);
        }


        protected virtual void OnCurrentChange(IEnumerable<StoredObservation> observations) { }

        protected virtual void OnCurrentObservationAdd(StoredObservation observation) { }

        protected virtual void OnCurrentConditionAdd(IEnumerable<StoredObservation> observations) { }

        protected virtual void OnBufferObservationAdd(long bufferIndex, StoredObservation observation) { }

        #endregion

    }
}
