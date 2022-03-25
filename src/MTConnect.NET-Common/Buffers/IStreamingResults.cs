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
    public interface IStreamingResults
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
        IEnumerable<StoredObservation> Observations { get; }
    }
}
