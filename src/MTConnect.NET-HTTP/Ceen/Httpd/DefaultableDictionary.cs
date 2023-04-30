#region Copyright
/*
Copyright (c) 2011 John Sonmez

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


This project is a simple decorator for an IDictionary object and an extension method to make it easy to use.

The DefaultableDictionary will allow for creating a wrapper around a dictionary that provides a default value when trying to access a key that does not exist or enumerating through all the values in an IDictionary.

Example: var dictionary = new Dictionary<string, int>().WithDefaultValue(5);
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Ceen.Httpd
{
	/// <summary>
	/// Implements default values for lookup in a standard dictionary
	/// </summary>
	public class DefaultableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		/// <summary>
		/// The backing dictionary
		/// </summary>
		private readonly IDictionary<TKey, TValue> dictionary;
		/// <summary>
		/// The value to return if no matching entry was found
		/// </summary>
		private readonly TValue defaultValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Common.DefaultableDictionary`2"/> class.
		/// </summary>
		/// <param name="dictionary">The backing dictionary.</param>
		/// <param name="defaultValue">The default value.</param>
		public DefaultableDictionary(IDictionary<TKey, TValue> dictionary, TValue defaultValue = default(TValue))
		{
			this.dictionary = dictionary;
			this.defaultValue = defaultValue;
		}

		/// <summary>
		/// Gets an enumerator.
		/// </summary>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}

		/// <summary>
		/// Gets an enumerator.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Add the specified item to the dictionary.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(KeyValuePair<TKey, TValue> item)
		{
			dictionary.Add(item);
		}

		/// <summary>
		/// Clear all entries.
		/// </summary>
		public void Clear()
		{
			dictionary.Clear();
		}

		/// <summary>
		/// Returns <c>true</c> if the dictionary contains the item, <c>false</c> otherwise
		/// </summary>
		/// <param name="item">The item to look for.</param>
		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return dictionary.Contains(item);
		}

		/// <summary>
		/// Copies all entries to the array
		/// </summary>
		/// <param name="array">The target array.</param>
		/// <param name="arrayIndex">The target index where copying starts.</param>
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			dictionary.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes an item from the collection
		/// </summary>
		/// <param name="item"><c>true</c> if the item was removed, <c>false</c> otherwise.</param>
		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return dictionary.Remove(item);
		}

		/// <summary>
		/// Gets the number of entries.
		/// </summary>
		public int Count
		{
			get { return dictionary.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Ceen.Common.DefaultableDictionary`2"/> is read only.
		/// </summary>
		/// <value><c>true</c> if is read only; otherwise, <c>false</c>.</value>
		public bool IsReadOnly
		{
			get { return dictionary.IsReadOnly; }
		}

		/// <summary>
		/// Checks if an entry with the key exists.
		/// </summary>
		/// <returns><c>true</c>, if key was found, <c>false</c> otherwise.</returns>
		/// <param name="key">The key to look for.</param>
		public bool ContainsKey(TKey key)
		{
			return dictionary.ContainsKey(key);
		}

		/// <summary>
		/// Add the specified key and value to the dictionary.
		/// </summary>
		/// <param name="key">The key to add.</param>
		/// <param name="value">The value to add.</param>
		public void Add(TKey key, TValue value)
		{
			dictionary.Add(key, value);
		}

		/// <summary>
		/// Remove the item with the specified key.
		/// </summary>
		/// <param name="key">The key for the item to remove.</param>
		/// <returns><c>true</c>, if key was found, <c>false</c> otherwise.</returns>
		public bool Remove(TKey key)
		{
			return dictionary.Remove(key);
		}

		/// <summary>
		/// Tries to get get value for the specified key.
		/// </summary>
		/// <returns><c>true</c>, if key was found, <c>false</c> otherwise.</returns>
		/// <param name="key">The key to find.</param>
		/// <param name="value">The resulting value.</param>
		public bool TryGetValue(TKey key, out TValue value)
		{
			if (!dictionary.TryGetValue(key, out value))
			{
				value = defaultValue;
				return false;
			}

			return true;
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Ceen.Common.DefaultableDictionary`2"/> with the specified key.
		/// </summary>
		/// <param name="key">The key to look for.</param>
		public TValue this[TKey key]
		{
			get
			{
				if (!dictionary.ContainsKey(key))
					return defaultValue;
				
				try
				{
					return dictionary[key];
				}
				catch (KeyNotFoundException)
				{
					return defaultValue;
				}
			}

			set { dictionary[key] = value; }
		}

		/// <summary>
		/// Gets all the keys for this dictionary.
		/// </summary>
		public ICollection<TKey> Keys
		{
			get { return dictionary.Keys; }
		}

		/// <summary>
		/// Gets all the values for this dictionary.
		/// </summary>
		public ICollection<TValue> Values 
		{ 
			get { return dictionary.Values; } 
		}
	}

	/// <summary>
	/// Defaultable dictionary extensions.
	/// </summary>
	public static class DefaultableDictionaryExtensions
	{
		/// <summary>
		/// Returns a defaultable dictionary that wraps the given dictionary
		/// </summary>
		/// <returns>The wrapped dictionary value.</returns>
		/// <param name="dictionary">The wrapped dictionary.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <typeparam name="TValue">The key type parameter.</typeparam>
		/// <typeparam name="TKey">The data type parameter.</typeparam>
		public static IDictionary<TKey, TValue> WithDefaultValue<TValue, TKey>(this IDictionary<TKey, TValue> dictionary, TValue defaultValue = default(TValue))
		{
			return new DefaultableDictionary<TKey, TValue>(dictionary, defaultValue);
		}
	}
}

