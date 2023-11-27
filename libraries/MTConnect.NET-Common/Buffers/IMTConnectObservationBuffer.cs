// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace MTConnect.Buffers
{
    /// <summary>
    /// Buffer interface used to store MTConnect Observation Data
    /// </summary>
    public interface IMTConnectObservationBuffer : IDisposable
    {
        /// <summary>
        /// Get a unique identifier for the Buffer
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Get the configured size of the Buffer in the number of maximum number of DataItems the buffer can hold at one time.
        /// </summary>
        int BufferSize { get; }

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


        IDictionary<int, BufferObservation> CurrentObservations { get; }

        IDictionary<int, IEnumerable<BufferObservation>> CurrentConditions { get; }


        /// <summary>
        /// Increment the Agent's Sequence number by one
        /// </summary>
        void IncrementSequence();

        /// <summary>
        /// Increment the Agent's Sequence number by the specified count
        /// </summary>
        void IncrementSequence(int count);


        /// <summary>
        /// Get a list of the Current Observations based on the specified BufferKeys
        /// </summary>
        /// <param name="bufferKeys">A list of Keys (DeviceUuid and DataItemId) to match observations in the buffer</param>
        /// <returns>An object that implements the IStreamingResults interface containing the query results</returns>
        IObservationBufferResults GetCurrentObservations(IEnumerable<int> bufferKeys);

        /// <summary>
        /// Get a list of the latest Observations at the specified sequence based on the specified BufferKeys
        /// </summary>
        /// <param name="bufferKeys">A list of Keys (DeviceUuid and DataItemId) to match observations in the buffer</param>
        /// <param name="at">The sequence number to include in the results</param>
        /// <returns>An object that implements the IStreamingResults interface containing the query results</returns>
        IObservationBufferResults GetCurrentObservations(IEnumerable<int> bufferKeys, long at);

        /// <summary>
        /// Get a list of Observations based on the specified search parameters
        /// </summary>
        /// <param name="bufferKeys">A list of Keys (DeviceUuid and DataItemId) to match observations in the buffer</param>
        /// <param name="from">The minimum sequence number to include in the results</param>
        /// <param name="to">The maximum sequence number to include in the results</param>
        /// <param name="count">The maximum number of Observations to include in the result</param>
        /// <returns>An object that implements the IStreamingResults interface containing the query results</returns>
        IObservationBufferResults GetObservations(IEnumerable<int> bufferKeys, long from = -1, long to = -1, int count = 0);


        /// <summary>
        /// Add a new Observation to the Buffer
        /// </summary>
        /// <param name="observation">The Observation to Add</param>
        /// <returns>A boolean value indicating whether the Observation was added to the Buffer successfully (true) or not (false)</returns>
        bool AddObservation(ref BufferObservation observation);
    }
}