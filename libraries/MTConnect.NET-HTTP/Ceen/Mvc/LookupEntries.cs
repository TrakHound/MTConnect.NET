using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ceen.Mvc
{
	/// <summary>
	/// Encapsulation of a method parameter
	/// </summary>
	internal struct ParameterEntry
	{
		/// <summary>
		/// The index of the argument in the method
		/// </summary>
		public readonly int ArgumentIndex;

		/// <summary>
		/// The name of the parameter
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// Indicating if the value is required
		/// </summary>
		public readonly bool Required;

		/// <summary>
		/// The allowed sources for the parameter
		/// </summary>
		public readonly ParameterSource Source;
		/// <summary>
		/// The wrapped parameter
		/// </summary>
		public readonly ParameterInfo Parameter;

		/// <summary>
		/// A flag indicating if this parameter gets a context-like argument
		/// </summary>
		public readonly bool IsContextParameter;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.Controller.ParameterEntry"/> struct.
		/// </summary>
		/// <param name="par">The parameter to represent.</param>
		/// <param name="index">The argument index in the method.</param>
		/// <param name="name">The parameter name</param>
		/// <param name="source">The allowed parameter sources</param>
		/// <param name="require">A flag indicating if the parameter is required</param>
		public ParameterEntry(ParameterInfo par, string name, ParameterSource source, bool required, int index)
		{
			Parameter = par;
			ArgumentIndex = index;
			Name = name;
			Source = source;
			IsContextParameter = par.ParameterType == typeof(IHttpContext) || par.ParameterType == typeof(IHttpRequest) || par.ParameterType == typeof(IHttpResponse);
			Required = required;
		}
	}

	/// <summary>
	/// Encapsulation for a method
	/// </summary>
	internal struct MethodEntry
	{
		/// <summary>
		/// The wrapped method
		/// </summary>
		public readonly MethodInfo Method;
		/// <summary>
		/// The number of arguments
		/// </summary>
		public readonly int ArgumentCount;
		/// <summary>
		/// The list of method parameters
		/// </summary>
		public readonly ParameterEntry[] Parameters;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.Controller.MethodEntry"/> struct.
		/// </summary>
		/// <param name="method">The method to wrap.</param>
		/// <param name="variables">The variables to include</param>
		public MethodEntry(MethodInfo method, IEnumerable<KeyValuePair<string, bool>> variables)
		{
			Method = method;
			ArgumentCount = method.GetParameters().Length;

			var urlargs = (variables ?? new KeyValuePair<string, bool>[0]).ToLookup(x => x.Key, x => x.Value);

			var i = 0;
			Parameters = Method
				.GetParameters()
				.Select(par =>
				{
					var name_attr = par.GetCustomAttributes(typeof(NameAttribute), false).Cast<NameAttribute>().FirstOrDefault();
					var name = name_attr == null || string.IsNullOrWhiteSpace(name_attr.Name) ? par.Name : name_attr.Name;

					var par_attr = par.GetCustomAttributes(typeof(ParameterAttribute), false).Cast<ParameterAttribute>().FirstOrDefault();
					ParameterSource source;
					bool optional;

					if (par_attr == null)
					{
						if (urlargs.Contains(name))
						{
							source = ParameterSource.Url;
							optional = urlargs[name].First();
						}
						else
						{
							source = ParameterSource.Default;
							optional = par.HasDefaultValue;
						}
					}
					else
					{
						source = par_attr.Source;
						optional = !par_attr.Required;
                        if (!string.IsNullOrWhiteSpace(par_attr.Name))
                            name = par_attr.Name;
					}

					var pe = new ParameterEntry(par, name, source, !optional, i);

					i++;

					return pe;
				})
				.ToArray();;
		}

	}

	internal class RouteEntry
	{
		/// <summary>
		/// The controller for this entry
		/// </summary>
		public readonly Controller Controller;
		/// <summary>
		/// The method for this entry
		/// </summary>
		public readonly MethodEntry Action;
		/// <summary>
		/// The HTTP verbs for this method
		/// </summary>
		public readonly string[] Verbs;
		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Ceen.Mvc.RouteEntry2"/> accepts all verbs.
		/// </summary>
		/// <value><c>true</c> if accepts all verbs; otherwise, <c>false</c>.</value>
		public bool AcceptsAllVerbs { get { return Verbs == null || Verbs.Length == 0; } }

		/// <summary>
		/// Checks if this route accepts the given verb
		/// </summary>
		/// <returns><c>true</c>, if verb was accepted, <c>false</c> otherwise.</returns>
		/// <param name="verb">The verb to test.</param>
		public bool AcceptsVerb(string verb)
		{
			return AcceptsAllVerbs || Verbs.Contains(verb, StringComparer.Ordinal);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.RouteEntry"/> class.
		/// </summary>
		/// <param name="controller">The controller for this entry.</param>
		/// <param name="action">The action for this entry.</param>
		/// <param name="verbs">The HTTP verbs.</param>
		/// <param name="variables">The variables to include</param>
		public RouteEntry(Controller controller, MethodInfo action, string[] verbs, IEnumerable<KeyValuePair<string, bool>> variables)
		{
			Controller = controller;
			Action = new MethodEntry(action, variables);
			Verbs = verbs;
		}

	}
}
