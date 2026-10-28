// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Buffers
{
    internal class MTConnectObservationQueue
    {
        // First in, first out, so observations are taken in the order they were added (sequence order)
        private readonly Queue<BufferObservation> _items = new Queue<BufferObservation>();
        private readonly object _lock = new object();
        private readonly int _limit;


        /// <summary>
        /// Gets the current number of Items in the Buffer Queue
        /// </summary>
        public long Count
        {
            get
            {
                lock (_lock) return _items.Count;
            }
        }


        public MTConnectObservationQueue(int limit = 50000)
        {
            _limit = limit;
        }


        /// <summary>
        /// Take (n) number of StoredObservations, oldest first, and remove them from the Queue
        /// </summary>
        public IEnumerable<BufferObservation> Take(int count = 1)
        {
            lock (_lock)
            {
                if (_items.Count > 0 && count > 0)
                {
                    var x = new List<BufferObservation>(System.Math.Min(count, _items.Count));
                    while (x.Count < count && _items.Count > 0) x.Add(_items.Dequeue());
                    return x;
                }
            }

            return null;
        }

        public bool Add(BufferObservation observation)
        {
            if (observation.IsValid)
            {
                lock (_lock)
                {
                    if (_items.Count >= _limit) return false;

                    _items.Enqueue(observation);
                    return true;
                }
            }

            return false;
        }
    }
}
