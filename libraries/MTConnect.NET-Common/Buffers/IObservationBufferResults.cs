// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Buffers
{
    /// <summary>
    /// Result set used to retrieve Streaming Data from an IMTConnectObservationBuffer
    /// </summary>
    public interface IObservationBufferResults
    {
        /// <summary>
        /// Gets the First Sequence available when the result set was processed
        /// </summary>
        ulong FirstSequence { get; }

        /// <summary>
        /// Gets the Last Sequence available when the result set was processed
        /// </summary>
        ulong LastSequence { get; }

        /// <summary>
        /// Gets the Next Sequence available when the result set was processed
        /// </summary>
        ulong NextSequence { get; }

        /// <summary>
        /// Gets the List of Stored Observations contained in the result set
        /// </summary>
        BufferObservation[] Observations { get; }

        /// <summary>
        /// Gets the sequence number of the first observation actually included in this result set.
        /// </summary>
        ulong FirstObservationSequence { get; }

        /// <summary>
        /// Gets the sequence number of the last observation actually included in this result set.
        /// </summary>
        ulong LastObservationSequence { get; }

        /// <summary>
        /// Gets the number of observations contained in this result set.
        /// </summary>
        uint ObservationCount { get; }

        /// <summary>
        /// Indicates whether the requested range could be satisfied; false when the buffer no longer holds the requested sequences.
        /// </summary>
        bool IsValid { get; }
    }
}