// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Observations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTConnect.Buffers
{
    /// <summary>
    /// Buffer interface used to store MTConnect Observation Data
    /// </summary>
    public interface IMTConnectObservationBuffer
    {
        /// <summary>
        /// Get a unique identifier for the Buffer
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Get the configured size of the Buffer in the number of maximum number of DataItems the buffer can hold at one time.
        /// </summary>
        long BufferSize { get; }

        /// <summary>
        /// A number representing the sequence number assigned to the oldest piece of Streaming Data stored in the buffer
        /// </summary>
        long FirstSequence { get; }

        /// <summary>
        /// A number representing the sequence number assigned to the last piece of Streaming Data that was added to the buffer
        /// </summary>
        long LastSequence { get; }

        /// <summary>
        /// A number representing the sequence number of the piece of Streaming Data that is the next piece of data to be retrieved from the buffer
        /// </summary>
        long NextSequence { get; }


        /// <summary>
        /// Increment the Agent's Sequence number by one
        /// </summary>
        void IncrementSequence();

        /// <summary>
        /// Increment the Agent's Sequence number by the specified count
        /// </summary>
        void IncrementSequence(int count);


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
        IStreamingResults GetObservations(string deviceUuid, IEnumerable<string> dataItemIds = null, long from = -1, long to = -1, long at = -1, int count = 0);

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
        Task<IStreamingResults> GetObservationsAsync(string deviceUuid, IEnumerable<string> dataItemIds = null, long from = -1, long to = -1, long at = -1, int count = 0);

        /// <summary>
        /// Get a list of Observations based on the specified search parameters
        /// </summary>
        /// <param name="deviceUuids">A list of Device UUIDs to include in the results</param>
        /// <param name="dataItemIds">A list of DataItemId's used to filter results</param>
        /// <param name="from">The minimum sequence number to include in the results</param>
        /// <param name="to">The maximum sequence number to include in the results</param>
        /// <param name="at">The sequence number to include in the results</param>
        /// <param name="count">The maximum number of Observations to include in the result</param>
        /// <returns>An object that implements the IStreamingResults interface containing the query results</returns>
        IStreamingResults GetObservations(IEnumerable<string> deviceUuids, IEnumerable<string> dataItemIds = null, long from = -1, long to = -1, long at = -1, int count = 0);

        /// <summary>
        /// Get a list of Observations based on the specified search parameters
        /// </summary>
        /// <param name="deviceUuids">A list of Device UUIDs to include in the results</param>
        /// <param name="dataItemIds">A list of DataItemId's used to filter results</param>
        /// <param name="from">The minimum sequence number to include in the results</param>
        /// <param name="to">The maximum sequence number to include in the results</param>
        /// <param name="at">The sequence number to include in the results</param>
        /// <param name="count">The maximum number of Observations to include in the result</param>
        /// <returns>An object that implements the IStreamingResults interface containing the query results</returns>
        Task<IStreamingResults> GetObservationsAsync(IEnumerable<string> deviceUuids, IEnumerable<string> dataItemIds = null, long from = -1, long to = -1, long at = -1, int count = 0);


        /// <summary>
        /// Add a new Observation to the Buffer
        /// </summary>
        /// <param name="deviceUuid">The UUID of the Device the data is associated with</param>
        /// <param name="dataItem">The DataItem the Observation is associated with</param>
        /// <param name="observation">The Observation to Add</param>
        /// <returns>A boolean value indicating whether the Observation was added to the Buffer successfully (true) or not (false)</returns>
        bool AddObservation(string deviceUuid, IDataItem dataItem, IObservation observation);

        /// <summary>
        /// Add a new Observation to the Buffer
        /// </summary>
        /// <param name="deviceUuid">The UUID of the Device the data is associated with</param>
        /// <param name="dataItem">The DataItem the Observation is associated with</param>
        /// <param name="observation">The Observation to Add</param>
        /// <returns>A boolean value indicating whether the Observation was added to the Buffer successfully (true) or not (false)</returns>
        Task<bool> AddObservationAsync(string deviceUuid, IDataItem dataItem, IObservation observation);
    }
}
