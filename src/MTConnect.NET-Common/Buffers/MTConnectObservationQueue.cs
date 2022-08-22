// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Buffers
{
    public class MTConnectObservationQueue
    {
        private readonly Dictionary<long, BufferObservation> _items = new Dictionary<long, BufferObservation>();
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
            else
            {

            }

            return false;
        }   
    }
}
