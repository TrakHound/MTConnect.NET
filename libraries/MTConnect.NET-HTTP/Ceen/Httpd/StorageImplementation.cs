using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ceen.Httpd
{
	/// <summary>
	/// AppDomain wrapper for accessing an IStorageEntry in another domain
	/// </summary>
	internal class StorageEntryWrapper : IStorageEntry
	{
		/// <summary>
		/// The remote instance of the StorageCreator
		/// </summary>
		private readonly object m_wrapped;

		/// <summary>
		/// The instance as a dictionary
		/// </summary>
		private readonly IDictionary<string, string> m_dict;

		/// <summary>
		/// The Expires property
		/// </summary>
		private readonly System.Reflection.PropertyInfo m_expires;

		/// <summary>
		/// The Name property
		/// </summary>
		private readonly System.Reflection.PropertyInfo m_name;

		/// <summary>
		/// The indexer property
		/// </summary>
		private readonly System.Reflection.PropertyInfo m_index;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Httpd.StorageEntryWrapper"/> class.
		/// </summary>
		/// <param name="owner">The owner to wrap.</param>
		public StorageEntryWrapper(object owner)
		{
			m_wrapped = owner;
			m_dict = (IDictionary<string, string>)owner;
			m_expires = m_wrapped.GetType().GetProperty(nameof(Expires));
			m_name = m_wrapped.GetType().GetProperty(nameof(Name));

			var iface = m_wrapped.GetType().GetInterfaces().FirstOrDefault(x => x.FullName == typeof(IStorageEntry).FullName);
			if (iface == null)
				throw new Exception($"The given type ${m_wrapped.GetType()} does not implement {typeof(IStorageEntry)}");

			m_index = typeof(IDictionary<string, string>).GetProperties().FirstOrDefault(x => x.GetIndexParameters().Length == 1);

			if (new[] { m_wrapped, m_expires, m_name, m_index }.Any(x => x == null))
				throw new Exception($"Something changed in {typeof(IStorageEntry)}");
		}

		/// <summary>
		/// Gets the element with the specified key, or null.
		/// Set to null to delete the item.
		/// </summary>
		/// <param name="key">The item key.</param>
		public string this[string key]
		{
			get
			{
				return (string)m_index.GetValue(m_wrapped, new object[] { key });
			}

			set
			{
				m_index.SetValue(m_wrapped, value, new object[] { key });
			}
		}

		/// <summary>
		/// Gets or sets the time the dictionary expires
		/// </summary>
		public DateTime Expires
		{
			get
			{
				return (DateTime)m_expires.GetValue(m_wrapped, null);
			}
			set
			{
				m_expires.SetValue(m_wrapped, value, null);
			}
		}

		/// <summary>
		/// Gets the name of the storage element
		/// </summary>
		public string Name
		{
			get
			{
				return (string)m_name.GetValue(m_wrapped, null);
			}
		}

		/// <summary>
		///Gets the number of elements in the collection
		/// </summary>
		public int Count { get { return m_dict.Count; } }

		/// <summary>
		/// Gets a value indicating whether this instance is read only.
		/// </summary>
		public bool IsReadOnly { get { return m_dict.IsReadOnly; } }

		/// <summary>
		/// Gets the keys for this collection
		/// </summary>
		public ICollection<string> Keys { get { return m_dict.Keys; } }

		/// <summary>
		/// Gets the values for this collection
		/// </summary>
		public ICollection<string> Values { get { return m_dict.Values; } }

		/// <summary>
		/// Adds an item to the collection
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(KeyValuePair<string, string> item)
		{
			m_dict.Add(item);
		}

		/// <summary>
		/// Adds an item to the collection
		/// </summary>
		/// <param name="key">The key to use.</param>
		/// <param name="value">The value to use.</param>
		public void Add(string key, string value)
		{
			m_dict.Add(key, value);
		}

		/// <summary>
		/// Removes all elements from this collection
		/// </summary>
		public void Clear()
		{
			m_dict.Clear();
		}

		/// <summary>
		/// Returns a value indicating if the given item was found in the collection
		/// </summary>
		/// <param name="item">The item to look for.</param>
		public bool Contains(KeyValuePair<string, string> item)
		{
			return m_dict.Contains(item);
		}

		/// <summary>
		/// Returns a value indicating if the given item was found in the collection
		/// </summary>
		/// <returns><c>true</c>, if key was found, <c>false</c> otherwise.</returns>
		/// <param name="key">The key to look for.</param>
		public bool ContainsKey(string key)
		{
			return m_dict.ContainsKey(key);
		}

		/// <summary>
		/// Copies the elements to an array
		/// </summary>
		/// <param name="array">The target array.</param>
		/// <param name="arrayIndex">The target array index.</param>
		public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
		{
			m_dict.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return m_dict.GetEnumerator();
		}

		/// <summary>
		/// Remove the specified item.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public bool Remove(KeyValuePair<string, string> item)
		{
			return m_dict.Remove(item);
		}

		/// <summary>
		/// Remove the item with the specified key.
		/// </summary>
		/// <param name="key">The key to look for.</param>
		public bool Remove(string key)
		{
			return m_dict.Remove(key);
		}

		/// <summary>
		/// Tries the get value for the given key.
		/// </summary>
		/// <returns><c>true</c>, if get value was found, <c>false</c> otherwise.</returns>
		/// <param name="key">The key to look for.</param>
		/// <param name="value">The resulting value.</param>
		public bool TryGetValue(string key, out string value)
		{
			return m_dict.TryGetValue(key, out value);
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_dict.GetEnumerator();
		}
	}

	/// <summary>
	/// Wrapper helper to invoke methods across the AppDomain boundary
	/// </summary>
	public class StorageCreatorAccessor : IStorageCreator
	{
		/// <summary>
		/// The remote instance of the StorageCreator
		/// </summary>
		private readonly object m_wrapped;
		/// <summary>
		/// The GetStorageAsync method
		/// </summary>
		private readonly System.Reflection.MethodInfo m_getStorageCallback;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Httpd.StorageCreatorAccessor"/> class.
		/// </summary>
		/// <param name="owner">The storage creator instance from the other AppDomain.</param>
		public StorageCreatorAccessor(object owner)
		{
			m_wrapped = owner;

			m_getStorageCallback = m_wrapped.GetType().GetMethod(nameof(MemoryStorageCreator.GetStorageCallback));

			if (new[] { m_wrapped, m_getStorageCallback }.Any(x => x == null))
				throw new Exception($"Something changed in {typeof(MemoryStorageCreator)}");
		}

		/// <summary>
		/// Gets or creates a storage module with the given name
		/// </summary>
		/// <returns>The storage module or null.</returns>
		/// <param name="name">The name of the module to get.</param>
		/// <param name="key">The session key of the module, or null.</param>
		/// <param name="ttl">The module time-to-live, zero or less means no expiration.</param>
		/// <param name="autocreate">Automatically create storage if not found</param>
		public async Task<IStorageEntry> GetStorageAsync(string name, string key, int ttl, bool autocreate)
		{
			var appdomaintask = new AppDomainTask();
			m_getStorageCallback.Invoke(m_wrapped, new object[] { name, key, ttl, autocreate, appdomaintask });

			var res = await appdomaintask.ResultAsync();
			if (res == null)
				return null;
			
			return new StorageEntryWrapper(res);
		}
	}

	/// <summary>
	/// A storage entry that uses a defaultable dictionary for storage
	/// </summary>
	internal class MemoryStorageEntry : MarshalByRefObject, IStorageEntry
	{
        /// <summary>
        /// The storage entry
        /// </summary>
        private readonly DefaultableDictionary<string, string> m_dict = new DefaultableDictionary<string, string>(new Dictionary<string, string>(2));

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Ceen.Httpd.MemoryStorageEntry"/> class.
        /// </summary>
        /// <param name="name">The storage element name.</param>
        public MemoryStorageEntry(string name)
		{
			this.Name = name;
		}

		/// <summary>
		/// Gets or sets the and element with the specified key.
		/// </summary>
		/// <param name="key">The key to look for.</param>
		public string this[string key]
		{
			get { return m_dict[key]; }
			set { m_dict[key] = value; }
		}

		/// <summary>
		/// Gets or sets the time when the storage entry expires
		/// </summary>
		/// <value>The expires.</value>
		public DateTime Expires { get; set; }

		/// <summary>
		/// Gets the name of this storage entry
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///Gets the number of elements in the collection
		/// </summary>
		public int Count { get { return m_dict.Count; } }

		/// <summary>
		/// Gets a value indicating whether this instance is read only.
		/// </summary>
		public bool IsReadOnly { get { return m_dict.IsReadOnly; } }

		/// <summary>
		/// Gets the keys for this collection
		/// </summary>
		public ICollection<string> Keys { get { return m_dict.Keys; } }

		/// <summary>
		/// Gets the values for this collection
		/// </summary>
		public ICollection<string> Values { get { return m_dict.Values; } }

		/// <summary>
		/// Adds an item to the collection
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(KeyValuePair<string, string> item)
		{
			m_dict.Add(item);
		}

		/// <summary>
		/// Adds an item to the collection
		/// </summary>
		/// <param name="key">The key to use.</param>
		/// <param name="value">The value to use.</param>
		public void Add(string key, string value)
		{
			m_dict.Add(key, value);
		}

		/// <summary>
		/// Removes all elements from this collection
		/// </summary>
		public void Clear()
		{
			m_dict.Clear();
		}

		/// <summary>
		/// Returns a value indicating if the given item was found in the collection
		/// </summary>
		/// <param name="item">The item to look for.</param>
		public bool Contains(KeyValuePair<string, string> item)
		{
			return m_dict.Contains(item);
		}

		/// <summary>
		/// Returns a value indicating if the given item was found in the collection
		/// </summary>
		/// <returns><c>true</c>, if key was found, <c>false</c> otherwise.</returns>
		/// <param name="key">The key to look for.</param>
		public bool ContainsKey(string key)
		{
			return m_dict.ContainsKey(key);
		}

		/// <summary>
		/// Copies the elements to an array
		/// </summary>
		/// <param name="array">The target array.</param>
		/// <param name="arrayIndex">The target array index.</param>
		public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
		{
			m_dict.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return m_dict.GetEnumerator();
		}

		/// <summary>
		/// Remove the specified item.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public bool Remove(KeyValuePair<string, string> item)
		{
			return m_dict.Remove(item);
		}

		/// <summary>
		/// Remove the item with the specified key.
		/// </summary>
		/// <param name="key">The key to look for.</param>
		public bool Remove(string key)
		{
			return m_dict.Remove(key);
		}

		/// <summary>
		/// Tries the get value for the given key.
		/// </summary>
		/// <returns><c>true</c>, if get value was found, <c>false</c> otherwise.</returns>
		/// <param name="key">The key to look for.</param>
		/// <param name="value">The resulting value.</param>
		public bool TryGetValue(string key, out string value)
		{
			return m_dict.TryGetValue(key, out value);
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_dict.GetEnumerator();
		}
	}

	/// <summary>
	/// A storage creator instance that uses simple in-memory dictionaries
	/// </summary>
	public class MemoryStorageCreator : MarshalByRefObject, IStorageCreator, IDisposable
	{
		/// <summary>
		/// Synchronization lock
		/// </summary>
		private readonly AsyncLock m_lock = new AsyncLock();

		/// <summary>
		/// The storage item
		/// </summary>
		private readonly Dictionary<string, Dictionary<string, IStorageEntry>> m_storage = new Dictionary<string, Dictionary<string, IStorageEntry>>();

		/// <summary>
		/// Gets or sets an external creator, used to supply custom storage.
		/// Parameters are (name, key, ttl)
		/// </summary>
		public Func<string, string, int, Task<IStorageEntry>> ExternalCreator { get; set; }

		/// <summary>
		/// Gets or sets the expiration check interval.
		/// </summary>
		public TimeSpan ExpireCheckInterval
		{
			get { return m_expirationHandler.Interval; }
			set { m_expirationHandler.Interval = value; }
		}

		/// <summary>
		/// Gets or sets the high water mark, where expires are triggered.
		/// </summary>
		public long HighWaterMark { get; set; } = 10000;

		/// <summary>
		/// The expiration handler.
		/// </summary>
		private PeriodicTask m_expirationHandler;

		/// <summary>
		/// The minimum high water mark
		/// </summary>
		private readonly long m_minhighwater;

		/// <summary>
		/// The number of items created
		/// </summary>
		private long m_createcount;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Httpd.MemoryStorageCreator"/> class.
		/// </summary>
		public MemoryStorageCreator()
		{
			m_minhighwater = HighWaterMark;
			m_expirationHandler = new PeriodicTask(DoExpireAsync, TimeSpan.FromMinutes(10));
		}

		/// <summary>
		/// Perform the actual expiration
		/// </summary>
		private async Task<long> DoExpireAsync(bool forced)
		{
			var count = 0L;
			using (await m_lock.LockAsync())
			{
				foreach (var kp in m_storage)
				{
					var expiredkeys = kp.Value
										.Where(x => x.Value.Expires.Ticks > 0 && DateTime.Now > x.Value.Expires)
										.Select(x => x.Key)
										.ToList();
					foreach (var key in expiredkeys)
						kp.Value.Remove(key);
					
					count += kp.Value.Count;
				}

				m_createcount = count;

				if (count * 1.1 > HighWaterMark)
					HighWaterMark = HighWaterMark * 2;
				else if (count < HighWaterMark / 4)
					HighWaterMark = Math.Max(m_minhighwater, HighWaterMark / 2);
			}

			return count;
		}


		/// <summary>
		/// Gets or creates a storage module with the given name
		/// </summary>
		/// <returns>The storage module or null.</returns>
		/// <param name="name">The name of the module to get.</param>
		/// <param name="key">The session key of the module, or null.</param>
		/// <param name="ttl">The module time-to-live, zero or less means no expiration.</param>
		/// <param name="autocreate">Automatically create storage if not found</param>
		/// <param name="handler">The callback handler</param>
		public void GetStorageCallback(string name, string key, int ttl, bool autocreate, AppDomainTask handler)
		{
			AppDomainTask.HandleTask(GetStorageAsync(name, key, ttl, autocreate), handler);
		}

		/// <summary>
		/// Gets or creates a storage module with the given name
		/// </summary>
		/// <returns>The storage module or null.</returns>
		/// <param name="name">The name of the module to get.</param>
		/// <param name="key">The session key of the module, or null.</param>
		/// <param name="ttl">The module time-to-live, zero or less means no expiration.</param>
		/// <param name="autocreate">Automatically create storage if not found</param>
		public async Task<IStorageEntry> GetStorageAsync(string name, string key, int ttl, bool autocreate)
		{
			// Copy ref to avoid race when setting a new creator
			var ec = ExternalCreator;

			IStorageEntry res = null;
			key = key ?? string.Empty;

			using (await m_lock.LockAsync())
			{
				Dictionary<string, IStorageEntry> dict;
				if (!m_storage.TryGetValue(name, out dict))
					dict = m_storage[name] = new Dictionary<string, IStorageEntry>();

				if (!dict.TryGetValue(key, out res))
				{
					if (!autocreate)
						return null;

					if (ec != null)
						res = await ec(name, key, ttl);
					
					if (res == null)
						res = new MemoryStorageEntry(name);

					dict[key] = res;
					m_createcount++;
				}
				else if (res.Expires.Ticks > 0 && DateTime.Now > res.Expires)
				{
					dict.Remove(key);
					if (!autocreate)
					{
						m_createcount--;
						return null;
					}

					res = null;
					if (ec != null)
						res = await ec(name, key, ttl);

					if (res == null)
						res = new MemoryStorageEntry(name);

					dict[key] = res;
				}

				if (ttl > 0)
					res.Expires = DateTime.Now.AddSeconds(ttl);

				if (m_createcount > HighWaterMark)
					m_expirationHandler.RunNow();

				return res;
			}
		}

		/// <summary>
		/// Releases all resource used by the <see cref="T:Ceen.Httpd.MemoryStorageCreator"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="T:Ceen.Httpd.MemoryStorageCreator"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="T:Ceen.Httpd.MemoryStorageCreator"/> in an unusable state.
		/// After calling <see cref="Dispose"/>, you must release all references to the
		/// <see cref="T:Ceen.Httpd.MemoryStorageCreator"/> so the garbage collector can reclaim the memory that the
		/// <see cref="T:Ceen.Httpd.MemoryStorageCreator"/> was occupying.</remarks>
		public void Dispose()
		{
			if (m_expirationHandler != null)
				m_expirationHandler.Dispose();
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="T:Ceen.Httpd.MemoryStorageCreator"/> is reclaimed by garbage collection.
		/// </summary>
		~MemoryStorageCreator()
		{
			Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
