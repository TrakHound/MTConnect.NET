using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Ceen
{
	/// <summary>
	/// A class that allows a base class to store common settings, that can then be overridden in the sub-classes
	/// </summary>
	public abstract class DerivedSettings<TBaseClass>
	{
		/// <summary>
		/// The base dictionary with all the settings
		/// </summary>
		// NOTE: Since this is a generic type, we get one _basesettings instance pr. TBaseClass as intended
		protected static readonly Dictionary<string, object> _basesettings = new Dictionary<string, object>();

		/// <summary>
		/// The instance setting overrides
		/// </summary>
		protected readonly Dictionary<string, object> m_setttings = new Dictionary<string, object>();

		/// <summary>
		/// Gets the value for a given property.
		/// </summary>
		/// <returns>The value to get.</returns>
		/// <param name="propname">The name of the property.</param>
		/// <typeparam name="T">The property type.</typeparam>
		protected T GetValue<T>([CallerMemberName] string propname = null)
		{
			if (propname == null)
				throw new ArgumentNullException(nameof(propname));

			if (this.GetType() != typeof(TBaseClass))
			{
				object vx;
				if (m_setttings.TryGetValue(propname, out vx))
				{
					if (vx == null)
						return default(T);
					else
						return (T)vx;
				}
			}

			object v;
			if (_basesettings.TryGetValue(propname, out v))
				if (v == null)
					return default(T);
				else
					return (T)v;

			throw new ArgumentException($"No such property ${propname}");
		}

		/// <summary>
		/// Sets the value for a given property.
		/// </summary>
		/// <param name="propname">The property name.</param>
		/// <param name="o">The value to set.</param>
		protected void SetValue(object o, [CallerMemberName] string propname = null)
		{
			if (this.GetType() != typeof(TBaseClass))
				m_setttings[propname] = o;
			else
				_basesettings[propname] = o;
		}
	}
}
