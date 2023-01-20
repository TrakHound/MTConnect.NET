// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
        long FirstSequence { get; }

        /// <summary>
        /// Gets the Last Sequence available when the result set was processed
        /// </summary>
        long LastSequence { get; }

        /// <summary>
        /// Gets the Next Sequence available when the result set was processed
        /// </summary>
        long NextSequence { get; }

        /// <summary>
        /// Gets the List of Stored Observations contained in the result set
        /// </summary>
        BufferObservation[] Observations { get; }

        long FirstObservationSequence { get; }

        long LastObservationSequence { get; }

        int ObservationCount { get; }

        bool IsValid { get; }
    }
}
