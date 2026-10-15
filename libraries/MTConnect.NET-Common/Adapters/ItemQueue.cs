// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Adapters
{
    /// <summary>
    /// A bounded first-in, first-out queue of items keyed by a unique id (adding an existing id replaces that item
    /// and moves it to the back).
    /// </summary>
    internal class ItemQueue<T>
    {
        private readonly LinkedList<KeyValuePair<ulong, T>> _items = new LinkedList<KeyValuePair<ulong, T>>();
        private readonly Dictionary<ulong, LinkedListNode<KeyValuePair<ulong, T>>> _nodes = new Dictionary<ulong, LinkedListNode<KeyValuePair<ulong, T>>>();
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
        /// Take (n) number of items, oldest first, and remove them from the Queue
        /// </summary>
        public List<KeyValuePair<ulong, T>> Take(int count = 1)
        {
            lock (_lock)
            {
                if (_items.Count == 0) return null;

                var taken = new List<KeyValuePair<ulong, T>>(System.Math.Min(count, _items.Count));
                while (taken.Count < count && _items.First != null)
                {
                    var node = _items.First;
                    _items.RemoveFirst();
                    _nodes.Remove(node.Value.Key);
                    taken.Add(node.Value);
                }

                return taken;
            }
        }

        /// <summary>
        /// Puts previously taken items back at the front of the Queue (in their original order), e.g. after a failed send.
        /// Items whose id has been added again in the meantime are skipped.
        /// </summary>
        public void Requeue(IList<KeyValuePair<ulong, T>> items)
        {
            if (items == null) return;

            lock (_lock)
            {
                for (var i = items.Count - 1; i >= 0; i--)
                {
                    var item = items[i];
                    if (_nodes.ContainsKey(item.Key)) continue;

                    _nodes[item.Key] = _items.AddFirst(item);
                }
            }
        }

        public bool Add(ulong key, T item)
        {
            if (item != null)
            {
                lock (_lock)
                {
                    if (_nodes.TryGetValue(key, out var existing))
                    {
                        _items.Remove(existing);
                    }
                    else if (_items.Count >= _limit)
                    {
                        return false;
                    }

                    _nodes[key] = _items.AddLast(new KeyValuePair<ulong, T>(key, item));
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            lock (_lock)
            {
                _items.Clear();
                _nodes.Clear();
            }
        }
    }
}
