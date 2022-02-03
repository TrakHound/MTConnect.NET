// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using MTConnect.Agents;

namespace MTConnect.Buffers
{
    /// <summary>
    /// Result set used to retrieve Streaming Data from an IMTConnectObservationBuffer
    /// </summary>
    public class StreamingResults : IStreamingResults
    {
        /// <summary>
        /// Gets the First Sequence available when the result set was processed
        /// </summary>
        public long FirstSequence { get; }

        /// <summary>
        /// Gets the Last Sequence available when the result set was processed
        /// </summary>
        public long LastSequence { get; }

        /// <summary>
        /// Gets the Next Sequence available when the result set was processed
        /// </summary>
        public long NextSequence { get; }

        /// <summary>
        /// Gets the List of Stored Observations contained in the result set
        /// </summary>
        public IEnumerable<StoredObservation> Observations { get; }


        public StreamingResults(long firstSequence, long lastSequence, long nextSequence, IEnumerable<StoredObservation> observations)
        {
            FirstSequence = firstSequence;
            LastSequence = lastSequence;
            NextSequence = nextSequence;
            Observations = observations;
        }
    }
}
