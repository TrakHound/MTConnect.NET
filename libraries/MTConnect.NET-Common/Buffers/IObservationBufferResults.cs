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

        ulong FirstObservationSequence { get; }

        ulong LastObservationSequence { get; }

        uint ObservationCount { get; }

        bool IsValid { get; }
    }
}