// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
        public long FirstSequence { get; set; }

        /// <summary>
        /// Gets the Last Sequence available when the result set was processed
        /// </summary>
        public long LastSequence { get; set; }

        /// <summary>
        /// Gets the Next Sequence available when the result set was processed
        /// </summary>
        public long NextSequence { get; set; }

        /// <summary>
        /// Gets the List of Stored Observations contained in the result set
        /// </summary>
        public BufferObservation[] Observations { get; set; }

        public long FirstObservationSequence { get; set; }

        public long LastObservationSequence { get; set; }

        public int ObservationCount { get; set; }

        public bool IsValid { get; set; }


        public static ObservationBufferResults Invalid()
        {
            var x = new ObservationBufferResults();
            x.IsValid = false;
            return x;
        }
    }
}
