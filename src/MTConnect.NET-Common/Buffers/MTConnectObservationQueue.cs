// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Agents;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MTConnect.Buffers
{
    public class MTConnectObservationQueue
    {
        private readonly int _limit;
        private readonly Dictionary<string, StoredObservation> _items = new Dictionary<string, StoredObservation>();
        private readonly object _lock = new object();


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
        public IEnumerable<StoredObservation> Take(int count = 1)
        {
            lock (_lock)
            {
                var items = _items.Take(count);
                if (!items.IsNullOrEmpty())
                {
                    var x = new List<StoredObservation>();

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

        public bool Add(StoredObservation observation)
        {
            if (observation.IsValid)
            {
                try
                {
                    var json = JsonSerializer.Serialize(new FileObservation(observation).ToArray());
                    var hash = json.ToMD5Hash();
                    if (!string.IsNullOrEmpty(hash))
                    {

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
                    }
                catch { }
            }

            return false;
        }   
    }
}
