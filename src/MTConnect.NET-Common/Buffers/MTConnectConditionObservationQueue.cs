// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MTConnect.Buffers
{
    class MTConnectConditionObservationQueue
    {
        private readonly int _limit;
        private readonly Dictionary<string, IEnumerable<BufferObservation>> _items = new Dictionary<string, IEnumerable<BufferObservation>>();
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


        public MTConnectConditionObservationQueue(int limit = 50000)
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
                        x.AddRange(item.Value);              
                    }

                    // Remove Items from Queue
                    foreach (var item in items) _items.Remove(item.Key);

                    return x;
                }
            }

            return null;
        }

        public bool Add(IEnumerable<BufferObservation> observations)
        {
            if (!observations.IsNullOrEmpty())
            {
                try
                {
                    var fileObservations = new List<object[]>();
                    foreach (var observation in observations)
                    {
                        fileObservations.Add(new FileObservation(observation).ToArray());
                    }

                    var json = JsonSerializer.Serialize(fileObservations);
                    var hash = json.ToMD5Hash();
                    if (!string.IsNullOrEmpty(hash))
                    {

                        lock (_lock)
                        {
                            if (_items.Count > _limit) return false;

                            if (_items.TryGetValue(hash, out var _))
                            {
                                _items.Remove(hash);
                                _items.Add(hash, observations);
                                return true;
                            }
                            else
                            {
                                _items.Add(hash, observations);
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
