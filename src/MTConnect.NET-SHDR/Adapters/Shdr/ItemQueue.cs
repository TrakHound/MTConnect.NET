// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Adapters.Shdr
{
    internal class ItemQueue<T>
    {
        private readonly Dictionary<ulong, T> _items = new Dictionary<ulong, T>();
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


        public ItemQueue(int limit = 50000)
        {
            _limit = limit;
        }


        /// <summary>
        /// Take (n) number of StoredObservations and remove from the Queue
        /// </summary>
        public IEnumerable<T> Take(int count = 1)
        {
            lock (_lock)
            {
                var items = _items.Take(count);
                if (!items.IsNullOrEmpty())
                {
                    var x = new List<T>();

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

        public bool Add(ulong key, T item)
        {
            if (item != null)
            {
                lock (_lock)
                {
                    if (_items.Count > _limit) return false;

                    if (_items.TryGetValue(key, out var _))
                    {
                        _items.Remove(key);
                        _items.Add(key, item);
                        return true;
                    }
                    else
                    {
                        _items.Add(key, item);
                        return true;
                    }
                }
            }

            return false;
        }

        public void Clear()
        {
            lock (_lock) _items.Clear();
        }
    }
}
