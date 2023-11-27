using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// A collection of <c>&lt;packagedElement /&gt;</c> elements of a specific <c>xmi:type</c>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PackagedElementCollection<T> : ICollection<T> where T : PackagedElement
    {
        private List<T> Items = new List<T>();

        private Dictionary<string, int> NameCache = new Dictionary<string, int>();
        private Dictionary<string, int> IdCache = new Dictionary<string, int>();
        private HashSet<int> Cache = new HashSet<int>();

        /// <summary>
        /// Gets the element by the <c>name</c> attribute
        /// </summary>
        /// <param name="name">Term to lookup the element by <c>name</c></param>
        /// <returns>First <c>&lt;packagedElement /&gt;</c> where the <c>name</c> matched. Returns <c>null</c> if no elements were found</returns>
        public T GetByName(string? name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            int index = -1;
            if (NameCache.TryGetValue(name!, out index))
                return Items.ElementAt(index);

            return Get((e) => e.Name!.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        /// <summary>
        /// Gets the element by the <c>xmi:id</c> attribute
        /// </summary>
        /// <param name="id">Term to lookup the element by <c>id</c></param>
        /// <returns>First <c>&lt;packagedElement /&gt;</c> where the <c>id</c> matched. Returns <c>null</c> if no elements were found</returns>
        public T GetById(string? id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));
            int index = -1;
            if (IdCache.TryGetValue(id!, out index))
                return Items.ElementAt(index);

            return Get((e) => e.Id!.Equals(id, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        private IEnumerable<T> Get(Func<PackagedElement, bool> eval)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Cache.Contains(i)) continue;
                var element = Items.ElementAt(i);
                if (element != null && eval(element))
                {
                    if (!string.IsNullOrEmpty(element.Name) && !NameCache.ContainsKey(element.Name!))
                        NameCache.Add(element.Name!, i);

                    if (!string.IsNullOrEmpty(element.Id) && !IdCache.ContainsKey(element.Id!))
                        IdCache.Add(element.Id!, i);

                    Cache.Add(i);

                    yield return element;
                }
            }
        }

        /// <inheritdoc/>
        public int Count => Items.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => true;

        /// <inheritdoc/>
        public void Add(T item)
        {
            int index = Items.Count;
            Items.Add(item);

            if (!string.IsNullOrEmpty(item.Name) && !NameCache.ContainsKey(item.Name!))
                NameCache.Add(item.Name!, index);

            if (!string.IsNullOrEmpty(item.Id) && !IdCache.ContainsKey(item.Id!))
                IdCache.Add(item.Id!, index);

            if (!Cache.Contains(index))
                Cache.Add(index);
        }

        /// <inheritdoc/>
        public void Clear()
            => Items.Clear();

        /// <inheritdoc/>
        public bool Contains(T item)
            => Items.Contains(item);

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
            => Items.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
            => Items.GetEnumerator();
        
        /// <inheritdoc/>
        public bool Remove(T item)
        {
            int index = Items.IndexOf(item);

            if (Items.Remove(item))
            {
                string[] keys = NameCache.Where(o => o.Value > index)
                    .Select(kvp => kvp.Key)
                    .ToArray();
                Cache.RemoveWhere(o => o > index);
                for (int i = index; i < Items.Count; i++)
                {
                    NameCache[NameCache.ElementAt(i).Key]--;
                    IdCache[IdCache.ElementAt(i).Key]--;
                }

                return true;
            }

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    
        /// <summary>
        /// Deserializes the XmlElement(s) into the provided <typeparamref name="T"/>.
        /// </summary>
        /// <param name="elements">Collection of XmlElement(s) to deserialize.</param>
        /// <param name="type">Reference to the <c>xmi:type</c> to filter the <paramref name="elements"/> by.</param>
        /// <returns><inheritdoc cref="PackagedElementCollection{T}"/></returns>
        public static PackagedElementCollection<T> Deserialize(XmlElement[]? elements, string type)
        {
            var result = new PackagedElementCollection<T>();
            if (elements == null)
                return result;

            XmlRootAttribute xRoot = new XmlRootAttribute
            {
                ElementName = XmiHelper.XmiStructure.PACKAGED_ELEMENT,
                IsNullable = true,
                Namespace = ""
            };
            XmlSerializer serial = new XmlSerializer(typeof(T), xRoot);
            foreach (var element in elements)
            {
                if (element.LocalName != XmiHelper.XmiStructure.PACKAGED_ELEMENT || !element.GetAttribute("type", XmiHelper.XmiNamespace).Equals(type))
                    continue;

                using var xReader = new XmlNodeReader(element);
                object? deserializedObject = serial.Deserialize(xReader);
                if (deserializedObject == null)
                    continue;
                if (!(deserializedObject is T typedObject))
                    continue;

                result.Add(typedObject);
            }

            return result;
        }
    }
}