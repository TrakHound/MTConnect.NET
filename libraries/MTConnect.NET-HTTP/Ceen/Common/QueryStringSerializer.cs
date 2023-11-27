using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ceen
{
    /// <summary>
    /// Helper class to serialize and deserialize an object into a query string
    /// </summary>
	public static class QueryStringSerializer
	{
		/// <summary>
		/// Decodes querystring values with &quot;+&quot; values as spaces
		/// </summary>
		/// <param name="data">The encoded string to decode</param>
		/// <returns>The decoded string</returns>
		public static string UnescapeDataString(string data)
			=> Uri.UnescapeDataString(data?.Replace('+', ' '));

		/// <summary>
		/// Builds a lookup table for a given type
		/// </summary>
		/// <param name="type">The type to get the lookup table for</param>
		/// <returns>A lookup table</returns>
		public static Dictionary<string, MemberInfo> GetLookup(Type type)
		{
            var table = new Dictionary<string, MemberInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (var n in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                table[n.Name] = n;
            foreach (var n in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                table[n.Name] = n;

			return table;
        }

        /// <summary>
        /// Deserializes a string representation of a property or field to its native representation
        /// </summary>
        /// <returns>The native object.</returns>
        /// <param name="value">The string value to deserialize.</param>
        /// <param name="entrytype">The type of the field or property.</param>
        public static object DeserializeElement(string value, Type entrytype)
        {
            if (entrytype == typeof(string))
                return value;
            else if (entrytype == typeof(DateTime))
                return new DateTime(long.Parse(value));
            else if (entrytype.IsEnum)
                return Enum.Parse(entrytype, value, false);
            else if (entrytype.IsPrimitive)
                return Convert.ChangeType(value, entrytype);
            else
                throw new ArgumentException($"The field type is not supported: {entrytype.FullName}");
        }

        /// <summary>
        /// Serializes a property or field value to a string presentation
        /// </summary>
        /// <returns>The serialized representation.</returns>
        /// <param name="member">The value to serialize as a string.</param>
        /// <param name="instance">The type of the field or property.</param>
        public static string SerializeElement(MemberInfo member, object instance)
        {
            if (member is PropertyInfo)
                return SerializeElement(((PropertyInfo)member).GetValue(instance), ((PropertyInfo)member).PropertyType);
            else
                return SerializeElement(((FieldInfo)member).GetValue(instance), ((FieldInfo)member).FieldType);
        }

        /// <summary>
        /// Serializes a property or field value to a string presentation
        /// </summary>
        /// <returns>The serialized representation.</returns>
        /// <param name="value">The value to serialize as a string.</param>
        /// <param name="entrytype">The type of the field or property.</param>
        public static string SerializeElement(object value, Type entrytype)
        {
            if (entrytype == typeof(string))
                return value == null ? string.Empty : (string)value;
            else if (entrytype == typeof(DateTime))
                return ((DateTime)value).Ticks.ToString();
            else if (entrytype.IsEnum || entrytype.IsPrimitive)
                return entrytype.ToString();
            else
                throw new ArgumentException($"The field type is not supported: {entrytype.FullName}");
        }

        /// <summary>
        /// Serialize the specified item.
        /// </summary>
        /// <param name="item">The item to serialize.</param>
        /// <param name="type">The type to use for serialization, defaults to the object type.</param>
        /// <param name="lookup">A pre-built lookup table to use, or null to generate one from the type.</param>
        public static string Serialize(object item, Type type = null, Dictionary<string, MemberInfo> lookup = null)
        {
			if (item == null)
				return null;

			if (type == null)
				type = item.GetType();

			if(lookup == null)
				lookup = GetLookup(type);

            return
                "?"
                +
                string.Join(
                    "&",
                    lookup
                    .Values
                    .Select(x => Uri.EscapeDataString(SerializeElement(x, item)))
                );
        }				
	}

	/// <summary>
	/// Helper class to serialize and deserialize an object into a query string
	/// </summary>
	public static class QueryStringSerializer<T>
		where T : new()
	{
		/// <summary>
		/// A lookup table for the type, stores a table for each type being serialized
		/// </summary>
		private readonly static Dictionary<string, MemberInfo> _lookup;

		/// <summary>
		/// Initializes the <see cref="T:Ceen.QueryStringSerializer`1"/> class.
		/// </summary>
		static QueryStringSerializer()
		{
			_lookup = QueryStringSerializer.GetLookup(typeof(T));
		}

		/// <summary>
		/// Deserializes a string representation of a property or field to its native representation
		/// </summary>
		/// <returns>The native object.</returns>
		/// <param name="value">The string value to deserialize.</param>
		/// <param name="entrytype">The type of the field or property.</param>
		public static object DeserializeElement(string value, Type entrytype)
			=> QueryStringSerializer.DeserializeElement(value, entrytype);

		/// <summary>
		/// Serializes a property or field value to a string presentation
		/// </summary>
		/// <returns>The serialized representation.</returns>
		/// <param name="member">The value to serialize as a string.</param>
		/// <param name="instance">The type of the field or property.</param>
		public static string SerializeElement(MemberInfo member, object instance)
			=> QueryStringSerializer.SerializeElement(member, instance);

		/// <summary>
		/// Serializes a property or field value to a string presentation
		/// </summary>
		/// <returns>The serialized representation.</returns>
		/// <param name="value">The value to serialize as a string.</param>
		/// <param name="entrytype">The type of the field or property.</param>
		public static string SerializeElement(object value, Type entrytype)
			=> QueryStringSerializer.SerializeElement(value, entrytype);

		/// <summary>
		/// Serialize the specified item.
		/// </summary>
		/// <param name="item">The item to serialize.</param>
		public static string Serialize(T item)
			=> QueryStringSerializer.Serialize(item, typeof(T), _lookup);

		/// <summary>
		/// Deserialize the specified value.
		/// </summary>
		/// <param name="input">The serialized string.</param>
		/// <param name="ignoreextras"><c>True</c> if extra data is ignored, if this is <c>false</c> an exception is thrown if extra data is found</param>
		public static T Deserialize(string input, bool ignoreextras)
		{
			if (string.IsNullOrWhiteSpace(input))
				return default(T);

			var item = new T();
			input = input.TrimStart('?');
			foreach (var el in input.Split('?'))
			{
				if (el == null)
					continue;
				var splitix = el.IndexOf('=');
				if (splitix <= 0)
					continue;

				var key = QueryStringSerializer.UnescapeDataString(el.Substring(0, splitix));
				var value = QueryStringSerializer.UnescapeDataString(el.Substring(splitix + 1));

				MemberInfo member;
				if (!_lookup.TryGetValue(key, out member))
				{
					if (!ignoreextras)
						throw new ArgumentException($"The item {key} is not found in {typeof(T).FullName}");
				}
				else
				{
					if (member is PropertyInfo)
						((PropertyInfo)member).SetValue(item, DeserializeElement(value, ((PropertyInfo)member).PropertyType), null);
					else 
						((FieldInfo)member).SetValue(item, DeserializeElement(value, ((FieldInfo)member).FieldType));
				}
			}

			return item;
		}
	}
}
