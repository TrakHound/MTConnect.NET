using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ceen
{
    /// <summary>
    /// Implements a basic least-recently-used cache
    /// </summary>
    public class LRUCache<T>
    {
        /// <summary>
        /// The lookup table giving O(1) access to the values
        /// </summary>
        private readonly Dictionary<string, KeyValuePair<T, long>> m_lookup = new Dictionary<string, KeyValuePair<T, long>>();
        /// <summary>
        /// The most-recent-used list that is updated in O(n)
        /// </summary>
        private readonly List<string> m_mru = new List<string>();

        /// <summary>
        /// The handler method that is used to return the size of each element
        /// </summary>
        private readonly Func<string, T, Task<long>> m_sizecalculator;
        /// <summary>
        /// The handler invoked when an item is expired
        /// </summary>
        private readonly Func<string, T, bool, Task> m_expirehandler;

        /// <summary>
        /// The lock guarding the cache
        /// </summary>
        private readonly AsyncLock m_lock = new AsyncLock();

        /// <summary>
        /// The size of the elements in the cache
        /// </summary>
        private long m_size;
        /// <summary>
        /// The maximum allowed size of the cache
        /// </summary>
        private readonly long m_sizelimit;
        /// <summary>
        /// The maximum number of elements in the cache
        /// </summary>
        private readonly long m_countlimit;

        /// <summary>
        /// Gets maximum allowed size of the cache
        /// </summary>
        public long SizeLimit => m_sizelimit;

        /// <summary>
        /// Gets the maximum number of elements in the cache.
        /// </summary>
        public long CountLimit => m_countlimit;

        /// <summary>
        /// Creates a new least-recent-used cache
        /// </summary>
        /// <param name="sizelimit">The limit for the size of the cache.</param>
        /// <param name="countlimit">The limit for the number of items in the cache.</param>
        /// <param name="expirationHandler">A callback method invoked when items are expired from the cache.</param>
        /// <param name="sizeHandler">A callback handler used to compute the size of elements added to and removed from the queue.</param>
        public LRUCache(long sizelimit = long.MaxValue, long countlimit = long.MaxValue, Func<string, T, bool, Task> expirationHandler = null, Func<string, T, Task<long>> sizeHandler = null)
        {
            if (sizelimit != long.MaxValue && sizeHandler == null)
                throw new Exception("Must supply a size handler to enforce the cache size");

            m_sizelimit = sizelimit;
            m_countlimit = countlimit;
            m_expirehandler = expirationHandler;
            m_sizecalculator = sizeHandler ?? ((k, v) => Task.FromResult(0L));
        }

        /// <summary>
        /// Adds or replaces a cache element
        /// </summary>
        /// <returns><c>true</c> if the value was new, <c>false</c> otherwise</returns>
        /// <param name="key">The element key.</param>
        /// <param name="value">The element value.</param>
        public async Task<bool> AddOrReplaceAsync(string key, T value)
        {
            using (await m_lock.LockAsync())
            {
                var p = m_lookup.TryGetValue(key, out var vt);
                if (p)
                {
                    m_size -= vt.Value;
                    m_mru.Remove(key);
                    await (m_expirehandler?.Invoke(key, vt.Key, false) ?? Task.FromResult(true));
                }

                var s = await m_sizecalculator(key, value);
                m_size += s;
                m_lookup[key] = new KeyValuePair<T, long>(value, s);
                m_mru.Add(key);

                await ExpireOverLimitAsync();

                return !p;
            }
        }

        /// <summary>
        /// Expires items that are outside the limits
        /// </summary>
        /// <returns>An awaitable task.</returns>
        private async Task ExpireOverLimitAsync()
        {
            while (m_mru.Count > 0 && m_size >= m_sizelimit || m_mru.Count >= m_countlimit)
            {
                var k = m_mru[0];
                m_mru.RemoveAt(0);
                var v = m_lookup[k];
                m_size -= v.Value;
                m_lookup.Remove(k);
                await (m_expirehandler?.Invoke(k, v.Key, true) ?? Task.FromResult(true));
            }
        }

        /// <summary>
        /// Expires all items in the cache
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public Task ClearAsync()
        {
            return ClearAsync((key, value) => true);
        }

        /// <summary>
        /// Expires items in the cache
        /// </summary>
        /// <param name="predicate">A predicate function returning true for items to remove</param>
        /// <returns>An awaitable task.</returns>
        public async Task ClearAsync(Func<string, T, bool> predicate)
        {
            using (await m_lock.LockAsync())
            {
                for (var i = m_mru.Count - 1; i > 0; i--)
                {
                    var k = m_mru[i];
                    var v = m_lookup[k];
                    if (predicate(k, v.Key))
                    {
                        m_size -= v.Value;
                        m_mru.RemoveAt(i);
                        m_lookup.Remove(k);
                        await (m_expirehandler?.Invoke(k, v.Key, true) ?? Task.FromResult(true));
                    }
                }

                // Just to be sure
                m_mru.Clear();
                m_lookup.Clear();
                m_size = 0;
            }
        }

        /// <summary>
        /// Tries to get the value from the cache
        /// </summary>
        /// <returns><c>true</c>, if the value was found, <c>false</c> otherwise.</returns>
        /// <param name="key">The key to look for.</param>
        /// <param name="value">The value for the key, or the default.</param>
        public bool TryGetValue(string key, out T value)
        {
            if (m_lookup.TryGetValue(key, out var n))
            {
                value = n.Key;
                return true;
            }

            value = default(T);
            return false;
        }

        /// <summary>
        /// Attempts to get the value. If the value does not match the <paramref name="predicate"/> it is expired and nothing is returned.
        /// </summary>
        /// <returns>A flag indicating if any results are returned, and the result, if any.</returns>
        /// <param name="key">The key to look for.</param>
        /// <param name="predicate">The predicate method, returns true if the item is invalid.</param>
        public async Task<KeyValuePair<bool, T>> TryGetUnlessAsync(string key, Func<string, T, Task<bool>> predicate)
        {
            using (await m_lock.LockAsync())
            {
                if (m_lookup.TryGetValue(key, out var n))
                {
                    if (!await predicate(key, n.Key))
                        return new KeyValuePair<bool, T>(true, n.Key);

                    m_mru.Remove(key);
                    m_size -= n.Value;
                    m_lookup.Remove(key);
                    await (m_expirehandler?.Invoke(key, n.Key, true) ?? Task.FromResult(true));
                }
            }

            return new KeyValuePair<bool, T>(false, default(T));
        }

        /// <summary>
        /// Attempts to get the value. If the value does not match the <paramref name="predicate"/> it is expired and nothing is returned.
        /// </summary>
        /// <returns>A flag indicating if any results are returned, and the result, if any.</returns>
        /// <param name="key">The key to look for.</param>
        /// <param name="predicate">The predicate method, returns true if the item is invalid.</param>
        public async Task<KeyValuePair<bool, T>> TryGetUnlessAsync(string key, Func<string, T, bool> predicate)
        {
            using (await m_lock.LockAsync())
            {
                if (m_lookup.TryGetValue(key, out var n))
                {
                    if (!predicate(key, n.Key))
                        return new KeyValuePair<bool, T>(true, n.Key);

                    m_mru.Remove(key);
                    m_size -= n.Value;
                    m_lookup.Remove(key);
                    await (m_expirehandler?.Invoke(key, n.Key, true) ?? Task.FromResult(true));
                }
            }

            return new KeyValuePair<bool, T>(false, default(T));
        }

        /// <summary>
        /// Gets the element from the cache with the specified key, or the default value
        /// </summary>
        /// <param name="key">The key to look for.</param>
        public T this[string key]
        {
            get
            {
                TryGetValue(key, out var n);
                return n;
            }
        }

    }
}
