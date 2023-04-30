using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

namespace Ceen.Httpd
{
	/// <summary>
	/// Implementation of a simple regexp based router
	/// </summary>
	public class Router : IRouter
	{
		/// <summary>
		/// Regex for mathcing wildcard/globbing characters
		/// </summary>
		private static readonly Regex WILDCARD_MATCHER = new Regex("\\*|\\?|[^\\*\\?]+");

		/// <summary>
		/// List of rules
		/// </summary>
		public IList<KeyValuePair<Regex, IHttpModule>> Rules { get; set; }

		/// <summary>
		/// Creates a new router
		/// </summary>
		public Router()
			: this(new KeyValuePair<string, IHttpModule>[0])
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Httpd.Router"/> class.
		/// </summary>
		/// <param name="items">The list of routes to use.</param>
		public Router(params KeyValuePair<string, IHttpModule>[] items)
			: this(items.AsEnumerable())
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Ceen.Httpd.Router"/> class.
		/// </summary>
		/// <param name="rules">The routing rules.</param>
		public Router(IEnumerable<KeyValuePair<string, IHttpModule>> rules)
		{
			Rules = rules.Select(x => new KeyValuePair<Regex, IHttpModule>(ToRegex(x.Key), x.Value)).ToList();
		}

		/// <summary>
		/// Parses a string and determines if it is a regular expression or not
		/// </summary>
		/// <returns>The parsed regular expression.</returns>
		/// <param name="value">The string to parse.</param>
		public static Regex ToRegex(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;
			
			if (value.StartsWith("[", StringComparison.Ordinal) && value.EndsWith("]", StringComparison.Ordinal))
				return new Regex(value.Substring(1, value.Length - 2));
			else
				return WildcardExpandToRegex(value);
		}

		/// <summary>
		/// Expands a wildcard expression to a regular expression
		/// </summary>
		/// <returns>The regular expression.</returns>
		/// <param name="value">The wildcard expression.</param>
		public static Regex WildcardExpandToRegex(string value)
		{
			return new Regex(WILDCARD_MATCHER.Replace(value, (match) => {
				if (match.Value == "*")
					return ".*";
				else if (match.Value == "?")
					return ".";
				else
					return Regex.Escape(match.Value);
			}));
		}


		/// <summary>
		/// Add the specified route and handler.
		/// </summary>
		/// <param name="route">The route to match.</param>
		/// <param name="handler">The handler to use.</param>
		public void Add(string route, IHttpModule handler)
		{
			Rules.Add(new KeyValuePair<Regex, IHttpModule>(ToRegex(route), handler));
		}

		/// <summary>
		/// Add the specified route and handler.
		/// </summary>
		/// <param name="route">The route to match.</param>
		/// <param name="handler">The handler to use.</param>
		public void Add(Regex route, IHttpModule handler)
		{
			Rules.Add(new KeyValuePair<Regex, IHttpModule>(route, handler));
		}

		/// <summary>
		/// Process the specified request.
		/// </summary>
		/// <param name="context">The http context.</param>
		/// <returns><c>True</c> if the processing was handled, false otherwise</returns>
		public async Task<bool> Process(IHttpContext context)
		{
			foreach (var rule in Rules)
			{
				if (rule.Key != null)
				{
					var m = rule.Key.Match(context.Request.Path);
					if (!m.Success || m.Length != context.Request.Path.Length)
						continue;
				}

				context.Request.RequireHandler(rule.Value.GetType().GetCustomAttributes(typeof(RequireHandlerAttribute), true).OfType<RequireHandlerAttribute>());

				if (await rule.Value.HandleAsync(context))
					return true;

				context.Request.PushHandlerOnStack(rule.Value);
			}

			return false;
		}
	}
}

