// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Buffers
{
    /// <summary>
    /// Result set used to retrieve Streaming Data from an IMTConnectObservationBuffer
    /// </summary>
    public struct ObservationBufferResults : IObservationBufferResults
    {
        /// <summary>
        /// Gets the First Sequence available when the result set was processed
        /// </summary>
        public ulong FirstSequence { get; set; }

        /// <summary>
        /// Gets the Last Sequence available when the result set was processed
        /// </summary>
        public ulong LastSequence { get; set; }

        /// <summary>
        /// Gets the Next Sequence available when the result set was processed
        /// </summary>
        public ulong NextSequence { get; set; }

        /// <summary>
        /// Gets the List of Stored Observations contained in the result set
        /// </summary>
        public BufferObservation[] Observations { get; set; }

        /// <summary>
        /// Gets the sequence number of the first observation actually included in this result set.
        /// </summary>
        public ulong FirstObservationSequence { get; set; }

        /// <summary>
        /// Gets the sequence number of the last observation actually included in this result set.
        /// </summary>
        public ulong LastObservationSequence { get; set; }

        /// <summary>
        /// Gets the number of observations contained in this result set.
        /// </summary>
        public uint ObservationCount { get; set; }

        /// <summary>
        /// Indicates whether the requested range could be satisfied; false when the buffer no longer holds the requested sequences.
        /// </summary>
        public bool IsValid { get; set; }


        /// <summary>
        /// Creates an empty result set flagged as invalid, used to signal that the requested range could not be served.
        /// </summary>
        public static ObservationBufferResults Invalid()
        {
            var x = new ObservationBufferResults();
            x.IsValid = false;
            return x;
        }
    }
}