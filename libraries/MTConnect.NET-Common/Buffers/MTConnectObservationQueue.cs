// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Buffers
{
    internal class MTConnectObservationQueue
    {
        private readonly Dictionary<ulong, BufferObservation> _items = new Dictionary<ulong, BufferObservation>();
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
        /// Take (n) number of StoredObservations and remove from the Queue
        /// </summary>
        public IEnumerable<BufferObservation> Take(int count = 1)
        {
            lock (_lock)
            {
                var items = _items.Take(count);
                if (!items.IsNullOrEmpty())
                {
                    var x = new List<BufferObservation>();

                    foreach (var item in items)
                    {
                        x.Add(item.Value);              
                    }

                    // Remove Items from Queue
                    foreach (var item in items) _items.Remove(item.Key);

                    return x;
                }
            }

            return null;
        }

        public bool Add(BufferObservation observation)
        {
            if (observation.IsValid)
            {
                var hash = observation.CreateHash();

                lock (_lock)
                {
                    if (_items.Count > _limit) return false;

                    if (_items.TryGetValue(hash, out var _))
                    {
                        _items.Remove(hash);
                        _items.Add(hash, observation);
                        return true;
                    }
                    else
                    {
                        _items.Add(hash, observation);
                        return true;
                    }
                }
            }

            return false;
        }   
    }
}