// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Agents.Buffer
{
    class DataItemResults
    {
        public long FirstSequence { get; set; }

        public long LastSequence { get; set; }

        public long NextSequence { get; set; }

        public IEnumerable<StoredObservation> DataItems { get; set; }


        public DataItemResults(long firstSequence, long lastSequence, long nextSequence, IEnumerable<StoredObservation> dataItems)
        {
            FirstSequence = firstSequence;
            LastSequence = lastSequence;
            NextSequence = nextSequence;
            DataItems = dataItems;
        }
    }
}
