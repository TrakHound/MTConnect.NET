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

        public ulong FirstObservationSequence { get; set; }

        public ulong LastObservationSequence { get; set; }

        public uint ObservationCount { get; set; }

        public bool IsValid { get; set; }


        public static ObservationBufferResults Invalid()
        {
            var x = new ObservationBufferResults();
            x.IsValid = false;
            return x;
        }
    }
}