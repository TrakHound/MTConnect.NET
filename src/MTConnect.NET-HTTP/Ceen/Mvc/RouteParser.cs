using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ceen.Mvc
{
	/// <summary>
	/// Class for parsing route descriptions
	/// </summary>
	internal class RouteParser
	{
		/// <summary>
		/// Common interface for fragments
		/// </summary>
		private interface IFragment
		{
			/// <summary>
			/// Checks if the fragment matches
			/// </summary>
			/// <param name="fragment">The fragment to check.</param>
			/// <returns>The number of characters matched, negative numbers indicate no match</returns>
			int Matches(string fragment);

			/// <summary>
			/// Gets the children of this entry.
			/// </summary>
			IFragment[] Children { get; }

			/// <summary>
			/// Returns a copy of the instance with the children entry set to the designated child
			/// </summary>
			/// <returns>The copy of this entry.</returns>
			/// <param name="children">The children to attach.</param>
			IFragment AttachChildren(IFragment[] children);
		}

		/// <summary>
		/// Represents a literal token
		/// </summary>
		private struct Literal : IFragment
		{
			/// <summary>
			/// The literal value for this fragment
			/// </summary>
			private readonly string m_value;
			/// <summary>
			/// The string comparer used for this instance
			/// </summary>
			private readonly StringComparison m_stringComparer;
			/// <summary>
			/// The list of child entries
			/// </summary>
			private readonly IFragment[] m_children;

			/// <summary>
			/// Gets a value indicating whether this <see cref="T:Ceen.Mvc.RouteParser2.Literal"/> is case sensitive.
			/// </summary>
			public bool IsCaseSensitive { get { return m_stringComparer == StringComparison.OrdinalIgnoreCase; } }
			/// <summary>
			/// Gets the literal value.
			/// </summary>
			public string Value { get { return m_value; } }
			/// <summary>
			/// Gets the children.
			/// </summary>
			public IFragment[] Children { get { return m_children;} }

			/// <summary>
			/// Initializes a new instance of the <see cref="T:Ceen.Mvc.RouteParser2.Literal"/> struct.
			/// </summary>
			/// <param name="value">The literal value to use.</param>
			/// <param name="caseinsensitive">If set to <c>true</c>, matches will be case insensitive.</param>
			/// <param name="children">The children associated with this entry.</param>
			public Literal(string value, bool caseinsensitive, IFragment[] children)
			{
				m_value = value;
				m_stringComparer = caseinsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
				m_children = children;
			}

			/// <summary>
			/// Matches the fragment with the literal value.
			/// </summary>
			/// <param name="fragment">The fragment to match.</param>
			/// <returns>The number of charaters matched, negative numbers indicate no match</returns>
			public int Matches(string fragment)
			{
				return fragment.StartsWith(m_value, m_stringComparer) ? m_value.Length : -1;
			}

			/// <summary>
			/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Ceen.Mvc.RouteParser2.Literal"/>.
			/// </summary>
			/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:Ceen.Mvc.RouteParser2.Literal"/>.</returns>
			public override string ToString()
			{
				var v = m_value;

				if (this.Children != null)
					v = string.Join("|", this.Children.Select(x => v + x.ToString()));

				return v;
			}

			/// <summary>
			/// Creates a clone of this entry, but assigned to new children
			/// </summary>
			/// <returns>The cloned copy.</returns>
			/// <param name="children">The new children.</param>
			public IFragment AttachChildren(IFragment[] children)
			{
				return new Literal(m_value, m_stringComparer == StringComparison.OrdinalIgnoreCase, children);
			}
		}

		/// <summary>
		/// A bound variable
		/// </summary>
		private struct BoundVariable : IFragment
		{
			/// <summary>
			/// The name of the variable
			/// </summary>
			private readonly string m_name;
			/// <summary>
			/// The bound value for the variable
			/// </summary>
			private readonly string m_value;
			/// <summary>
			/// The string comparer used for this instance
			/// </summary>
			private readonly StringComparison m_stringComparer;
			/// <summary>
			/// The children.
			/// </summary>
			private readonly IFragment[] m_children;

			/// <summary>
			/// Gets a value indicating whether this <see cref="T:Ceen.Mvc.RouteParser2.BoundVariable"/> is case sensitive.
			/// </summary>
			public bool IsCaseSensitive { get { return m_stringComparer == StringComparison.OrdinalIgnoreCase; } }
			/// <summary>
			/// Gets the bound value.
			/// </summary>
			public string Value { get { return m_value; } }
			/// <summary>
			/// Gets the bound name
			/// </summary>
			public string Name { get { return m_name; } }
			/// <summary>
			/// Gets the children.
			/// </summary>
			public IFragment[] Children { get { return m_children; } }

			/// <summary>
			/// Initializes a new instance of the <see cref="T:Ceen.Mvc.RouteParser2.BoundVariable"/> struct.
			/// </summary>
			/// <param name="name">The name of the variable.</param>
			/// <param name="value">The bound value.</param>
			/// <param name="caseinsensitive">If set to <c>true</c>, comapres are performed case insensitive.</param>
			/// <param name="children">The children to assign.</param>
			public BoundVariable(string name, string value, bool caseinsensitive, IFragment[] children)
			{
				m_name = name;
				m_value = value;
				m_stringComparer = caseinsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
				m_children = children;
			}

			/// <summary>
			/// Checks if the fragment matches
			/// </summary>
			/// <param name="fragment">The fragment to check.</param>
			/// <returns>The number of characters matched, negative numbers indicate no match</returns>
			public int Matches(string fragment)
			{
				return string.Equals(m_value, fragment, m_stringComparer) ? m_value.Length : -1;
			}

			/// <summary>
			/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Ceen.Mvc.RouteParser2.BoundVariable"/>.
			/// </summary>
			/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:Ceen.Mvc.RouteParser2.BoundVariable"/>.</returns>
			public override string ToString()
			{
				var v = $"[{m_name}={m_value}]";

				if (this.Children != null)
					v = string.Join("|", this.Children.Select(x => v + x.ToString()));

				return v;
			}

			/// <summary>
			/// Returns a copy of the instance with the children entry set to the designated child
			/// </summary>
			/// <returns>The copy of this entry.</returns>
			/// <param name="children">The children to attach.</param>
			public IFragment AttachChildren(IFragment[] children)
			{
				return new BoundVariable(m_name, m_value, m_stringComparer == StringComparison.OrdinalIgnoreCase, children);
			}
		}

		/// <summary>
		/// Represents a variable
		/// </summary>
		private struct Variable : IFragment
		{
			/// <summary>
			/// The variable name
			/// </summary>
			private readonly string m_name;
			/// <summary>
			/// The variable default value, if any
			/// </summary>
			private readonly string m_defaultvalue;
			/// <summary>
			/// A flag indicating if the variable is optional
			/// </summary>
			private readonly bool m_optional;
			/// <summary>
			/// A flag indicating if this variable slurps up the rest of the url
			/// </summary>
			private readonly bool m_slurp;
			/// <summary>
			/// Additional constraints for matching
			/// </summary>
			private readonly string m_constraint;
			/// <summary>
			/// A string delimiting the variable
			/// </summary>
			private readonly string m_delimiter;
			/// <summary>
			/// The assigned children.
			/// </summary>
			private readonly IFragment[] m_children;

			/// <summary>
			/// Gets the children.
			/// </summary>
			public IFragment[] Children { get { return m_children; } }

			/// <summary>
			/// Initializes a new instance of the <see cref="T:Ceen.Mvc.RouteParser2.Variable"/> struct.
			/// </summary>
			/// <param name="name">Name.</param>
			/// <param name="defaultvalue">Defaultvalue.</param>
			/// <param name="optional">If set to <c>true</c> optional.</param>
			/// <param name="contraint">Contraint.</param>
			/// <param name="slurp">If set to <c>true</c> slurp.</param>
			/// <param name="children">Children.</param>
			public Variable(string name, string defaultvalue, bool optional, string contraint, bool slurp, IFragment[] children)
			{
				m_name = name;
				m_defaultvalue = defaultvalue;
				m_optional = optional;
				m_constraint = contraint;
				m_slurp = slurp;
				m_children = children;
				if (children == null || m_children.Length == 0)
					m_delimiter = "/";
				else
				{
					var firstliteral = m_children.Where(x => x is Literal).Select(x => ((Literal)x).Value).FirstOrDefault();
					if (string.IsNullOrWhiteSpace(firstliteral))
						m_delimiter = "/";
					else
						m_delimiter = firstliteral.Substring(0);
				}
			}

			/// <summary>
			/// Checks if the fragment matches
			/// </summary>
			/// <param name="fragment">The fragment to check.</param>
			/// <returns>The number of characters matched, negative numbers indicate no match</returns>
			public int Matches(string fragment)
			{
				if (string.IsNullOrWhiteSpace(fragment))
					return m_optional ? 0 : -1;

				if (m_slurp)
					return fragment.Length;

				var ix = fragment.IndexOf(m_delimiter, StringComparison.Ordinal);
				return ix < 0 ? fragment.Length : ix;
			}

			/// <summary>
			/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Ceen.Mvc.RouteParser2.Variable"/>.
			/// </summary>
			/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:Ceen.Mvc.RouteParser2.Variable"/>.</returns>
			public override string ToString()
			{
				var ext = string.IsNullOrWhiteSpace(m_defaultvalue) ? "" : "=" + m_defaultvalue;
				var cst = string.IsNullOrWhiteSpace(m_constraint) ? "" : ":" + m_constraint;
				var str = "{" + string.Format(string.Format("{0}{1}{2}{3}", m_slurp ? "*" : "", m_name, cst, ext)) + "}";

				if (this.Children != null)
					str = string.Join("|", this.Children.Select(x => str + x.ToString()));

				return str;
			}

			/// <summary>
			/// Gets the name of the variable.
			/// </summary>
			public string Name { get { return m_name; } }
			/// <summary>
			/// Gets a value indicating if the variable is optional
			/// </summary>
			public bool Optional { get { return m_optional; } }
			/// <summary>
			/// Gets the default value
			/// </summary>
			public string DefaultValue { get { return m_defaultvalue; } }

			/// <summary>
			/// Returns a copy of the instance with the children entry set to the designated child
			/// </summary>
			/// <returns>The copy of this entry.</returns>
			/// <param name="children">The children to attach.</param>
			public IFragment AttachChildren(IFragment[] children)
			{
				return new Variable(m_name, m_defaultvalue, m_optional, m_constraint, m_slurp, children);
			}
		}

		/// <summary>
		/// Represents a terminator
		/// </summary>
		private struct Result : IFragment
		{
			/// <summary>
			/// The target route this entry points to
			/// </summary>
			public readonly RouteEntry Route;

			/// <summary>
			/// Initializes a new instance of the <see cref="T:Ceen.Mvc.RouteParser2.Result"/> struct.
			/// </summary>
			/// <param name="entry">The entry to represent.</param>
			public Result(RouteEntry entry)
			{
				Route = entry;
			}

			/// <summary>
			/// Checks if the fragment matches
			/// </summary>
			/// <param name="fragment">The fragment to check.</param>
			/// <returns>The number of characters matched, negative numbers indicate no match</returns>
			public int Matches(string fragment)
			{
				return 0;
			}
			
			/// <summary>
			/// Returns a copy of the instance with the children entry set to the designated child
			/// </summary>
			/// <returns>The copy of this entry.</returns>
			/// <param name="children">The children to attach.</param>
			public IFragment AttachChildren(IFragment[] children)
			{
				throw new Exception("Cannot attach children to a result node");
			}

			/// <summary>
			/// Gets the children.
			/// </summary>
			public IFragment[] Children { get { return null; } }

			/// <summary>
			/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Ceen.Mvc.RouteParser2.Result"/>.
			/// </summary>
			/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:Ceen.Mvc.RouteParser2.Result"/>.</returns>
			public override string ToString()
			{
				return string.Format(" => {0}, {1}", Route.Controller == null ? Route.Action.Method.DeclaringType.FullName : Route.Controller.GetType().FullName, Route.Action.Method);
			}
		}

		/// <summary>
		/// Represents a choice between different fragments
		/// </summary>
		private struct Choice : IFragment
		{
			/// <summary>
			/// Teh children representing the choices
			/// </summary>
			private readonly IFragment[] m_children;

			/// <summary>
			/// Gets the children.
			/// </summary>
			public IFragment[] Children { get { return m_children; } }

			/// <summary>
			/// Initializes a new instance of the <see cref="T:Ceen.Mvc.RouteParser2.Choice"/> struct.
			/// </summary>
			/// <param name="choices">The choices to use.</param>
			public Choice(IFragment[] choices)
			{
				if (choices == null || choices.Length == 0)
					throw new ArgumentNullException(nameof(choices));
				m_children = choices;
			}

			/// <summary>
			/// Checks if the fragment matches
			/// </summary>
			/// <param name="fragment">The fragment to check.</param>
			/// <returns>The number of characters matched, negative numbers indicate no match</returns>
			public int Matches(string fragment)
			{
				return 0;
			}

			/// <summary>
			/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Ceen.Mvc.RouteParser2.Choice"/>.
			/// </summary>
			/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:Ceen.Mvc.RouteParser2.Choice"/>.</returns>
			public override string ToString()
			{
				return string.Join("|", m_children.Select(x => x.ToString()));
			}

			/// <summary>
			/// Returns a copy of the instance with the children entry set to the designated child
			/// </summary>
			/// <returns>The copy of this entry.</returns>
			/// <param name="children">The children to attach.</param>
			public IFragment AttachChildren(IFragment[] children)
			{
				throw new InvalidOperationException($"Cannot set child on a choice");
			}
		}

		/// <summary>
		/// The regluar expression that matches variables
		/// </summary>
		private static readonly Regex CURLY_MATCH = new Regex(@"((?<!\{)\{(?!{))(?<name>[^\}]+)\}(?!\})");

		/// <summary>
		/// The regular expression for matching a variable's components
		/// </summary>
		private static readonly Regex VARIABLE_MATCH = new Regex(@"(?<slurp>\*)?(?<name>\w+)(?<optional>\?)?(:(?<constraint>[^\=\?]+))?(?<optional>\?)?(\=(?<default>.*))?");

		/// <summary>
		/// The list of fragments in this route
		/// </summary>
		private readonly IFragment m_root;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.RouteParser"/> class.
		/// </summary>
		/// <param name="route">The route specification to parse.</param>
		public RouteParser(string route, bool caseinsensitive, RouteEntry target)
		{
			var fragments = new List<IFragment>();

			var ix = 0;
			var slurp = false;
			foreach (Match m in CURLY_MATCH.Matches(route))
			{
				if (slurp)
					throw new Exception($"Cannot have trailing data after slurp: {m.Value}");

				if (ix != m.Index)
					fragments.Add(new Literal(route.Substring(ix, m.Index - ix), caseinsensitive, null));

				var mv = VARIABLE_MATCH.Match(m.Groups["name"].Value);
				if (!mv.Success)
					throw new ArgumentException($"Failed to parse {m.Groups["name"].Value} as a valid variable expression");

				var v = new Variable(mv.Groups["name"].Value, mv.Groups["default"].Value, mv.Groups["optional"].Success, mv.Groups["constraint"].Value, mv.Groups["slurp"].Success, null);

				if (fragments.Count > 0 && !(fragments.Last() is Literal))
					throw new Exception(string.Format("Must have literal spacer between {0} and {1}", fragments[fragments.Count - 2], v));

				fragments.Add(v);

				ix = m.Index + m.Length;
			}

			if (ix != route.Length)
				fragments.Add(new Literal(route.Substring(ix, route.Length - ix), caseinsensitive, null));

			fragments.Add(new Result(target));

			m_root = LinkList(fragments);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.RouteParser2"/> class.
		/// </summary>
		/// <param name="root">The root fragment instance.</param>
		private RouteParser(IFragment root)
		{
			m_root = root;
		}

		/// <summary>
		/// Merges a list of entries into a new tree
		/// </summary>
		/// <param name="entries">The entries to merge.</param>
		private static IFragment Merge(List<IFragment> entries)
		{
			if (entries.Count == 0)
				throw new Exception("Cannot work with empty list");
			if (entries.Count == 1)
				return entries.First();

			if (entries.All(x => x is Literal))
			{
				var roots = entries.OrderBy(x => ((Literal)x).Value).Cast<Literal>().ToList();
				var shortest = roots.First();

				// Get the longest shared prefix
				var sb = new StringBuilder();
				for (var n = 0; n < shortest.Value.Length; n++)
				{
					var cmp = shortest.Value[n];
					if (roots.All(x => x.Value[n] == cmp))
						sb.Append(cmp);
					else
						break;
				}

				if (sb.Length > 0)
				{
					var childchoices =
						roots
							.Where(x => x.Value.Length > sb.Length)
							.Select(x => new Literal(x.Value.Substring(sb.Length), shortest.IsCaseSensitive, x.Children))
							.GroupBy(
								x => x.Value.Length == 0 ? string.Empty : x.Value.Substring(0, 1),
								x => x
							)
							.Select(x => Merge(x.Cast<IFragment>().ToList()))
							.Union(
								   roots
								.Where(x => x.Value.Length == sb.Length)
								   .SelectMany(x => x.Children)
								   .GroupBy(x => x.GetType(), (key, list) => Merge(list.ToList())).ToArray()
							)
							.ToArray();

					return new Literal(sb.ToString(), shortest.IsCaseSensitive, childchoices);
				}
			}
			else if (entries.All(x => x is Variable))
			{
				// TODO: This does not work, if some variables are optional, and others not....
				var first = (Variable)entries.First();
				if (entries.All(x => ((Variable)x).Name == first.Name))
				{
					return first.AttachChildren(
						entries
						.SelectMany(x => x.Children)
						.GroupBy(x => x.GetType(), (key, list) => Merge(list.ToList()))
						.ToArray());
				}
				else
				{
					return new Choice(entries.GroupBy(x => ((Variable)x).Name, (key, list) => Merge(list.ToList())).ToArray());
				}
			}
			else if (entries.All(x => x is Result))
			{
				var lst = entries.Cast<Result>().ToList();
				var allverbs = lst.Where(x => x.Route.AcceptsAllVerbs).ToList();
				if (allverbs.Count > 1)	
					throw new Exception(string.Format("Unable to disambiguate the routes for: {0}{1}", Environment.NewLine, string.Join(Environment.NewLine, allverbs.Select(x => x.Route.Action.Method))));

				var specificverbs = lst.Where(x => !x.Route.AcceptsAllVerbs);
				var verblist = specificverbs.SelectMany(x => x.Route.Verbs.Distinct().Select(y => new { Verb = y, Route = x.Route }));
				var duplicates = verblist.GroupBy(x => x.Verb).Where(x => x.Count() > 1).Select(x => x.ToList()).FirstOrDefault();
				if (duplicates != null)
					throw new Exception(string.Format("Unable to disambiguate the routes for HTTP verb {2}: {0}{1}", Environment.NewLine, string.Join(Environment.NewLine, duplicates.Select(x => x.Route.Action.Method)), duplicates.First().Verb));

				// "false" is first, so items without wildcard http are seen first
				return new Choice(lst.OrderBy(x => x.Route.AcceptsAllVerbs).Cast<IFragment>().ToArray());
			}
			else
			{
				var f = entries.First();
				if (entries.All(x => x.GetType() == f.GetType()))
					return new Choice(entries.ToArray());
			}

			return new Choice(entries.GroupBy(x => x.GetType(), (key, list) => Merge(list.ToList())).ToArray());
		}

		/// <summary>
		/// Merges a list of routes
		/// </summary>
		/// <param name="entries">The routes to merge.</param>
		public static RouteParser Merge(IEnumerable<RouteParser> entries)
		{
			if (entries.Count() == 0)
				throw new Exception("Cannot work with empty list");
			if (entries.Count() == 1)
				return entries.First();
			
			var roots = entries.Select(x => x.m_root).ToList();
			if (roots.Any(x => !(x is Literal)))
				throw new Exception("Invalid initial values");

			if (roots.Any(x => !((Literal)x).Value.StartsWith("/", StringComparison.Ordinal)))
				throw new Exception("Invalid initial values");

			return new RouteParser(Merge(roots));
		}

		/// <summary>
		/// A state keeper to track the current position in the matching sequence
		/// </summary>
		private struct LookupStackEntry
		{
			/// <summary>
			/// The fragment this entry represents
			/// </summary>
			public readonly IFragment Fragment;
			/// <summary>
			/// The string offset matched so far
			/// </summary>
			public readonly int Offset;
			/// <summary>
			/// The list of variables picked up so far
			/// </summary>
			public readonly Dictionary<string, string> Variables;
			/// <summary>
			/// The number of pre-matched characters
			/// </summary>
			public readonly int MatchedCharacters;

			/// <summary>
			/// Initializes a new instance of the <see cref="T:Ceen.Mvc.RouteParser2.LookupStackEntry"/> struct.
			/// </summary>
			/// <param name="fragment">The fragment to use.</param>
			/// <param name="offset">The string offset.</param>
			/// <param name="variables">The variable dictionary.</param>
			public LookupStackEntry(IFragment fragment, int offset, Dictionary<string, string> variables, int prematched)
			{
				Fragment = fragment;
				Offset = offset;
				Variables = variables;
				MatchedCharacters = prematched;

			}
		}

		/// <summary>
		/// Attempts to match a request to routes
		/// </summary>
		/// <returns>The matched potential routes.</returns>
		/// <param name="request">Teh request to match.</param>
		internal IEnumerable<KeyValuePair<RouteEntry, Dictionary<string, string>>> MatchRequest(string request)
		{
			if (!string.IsNullOrWhiteSpace(request))
			{
				var work = new Stack<LookupStackEntry>();
				var reuse = true;
				var prev = new LookupStackEntry(m_root, 0, new Dictionary<string, string>(), -1);

				while (work.Count > 0 || reuse)
				{
					// If we re-use, don't bother with the stack
					var e = reuse ? prev : work.Pop();
					reuse = false;

					// If we have pre-matched, don't match again
					var m = e.MatchedCharacters < 0 ? e.Fragment.Matches(request.Substring(e.Offset)) : e.MatchedCharacters;
					if (m >= 0)
					{
						if (e.Fragment is Result)
						{
							// Otherwise we have unmatched trailing chars
							if (e.Offset + m == request.Length)
								yield return new KeyValuePair<RouteEntry, Dictionary<string, string>>(((Result)e.Fragment).Route, e.Variables);
						}
						else if (e.Fragment.Children != null)
						{
							// If this entry introduced variables, update the dictionary
							var dict = e.Variables;
							if (e.Fragment is Variable)
							{
								dict = new Dictionary<string, string>(dict);
								dict[((Variable)e.Fragment).Name] = request.Substring(e.Offset, m);
							}
							else if (e.Fragment is BoundVariable)
							{
								dict = new Dictionary<string, string>(dict);
								dict[((BoundVariable)e.Fragment).Name] = request.Substring(e.Offset, m);
							}

							// Add to work stack, re-using if possible
							var offset = e.Offset + m;
							var frag = request.Substring(offset);
							foreach(var c in e.Fragment.Children)
							{
								var m2 = c.Matches(frag);
								if (m2 < 0)
									continue;

								if (!reuse)
								{
									prev = new LookupStackEntry(c, offset, dict, m2);
									reuse = true;
								}
								else
								{
									work.Push(new LookupStackEntry(c, offset, dict, m2));
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Clones the list, by assigning each element with children from the previous entry
		/// </summary>
		/// <returns>The cloned list.</returns>
		/// <param name="cur">The root entry.</param>
		/// <param name="conv">An optional conversion/filter function.</param>
		private static IFragment CloneList(IFragment cur, Func<IFragment, IFragment> conv = null)
		{
			var lst = new List<IFragment>();
			conv = conv ?? (x => x);

			while (cur != null)
			{
				var cv = conv(cur);
				if (cv == null)
					break;
				
				lst.Add(cv);
				
				if (cur.Children == null)
					cur = null;
				else if (cur.Children.Length != 1)
					throw new Exception("Cannot modify choice entry");
				else
					cur = cur.Children.First();
			}

			return LinkList(lst);
		}

		/// <summary>
		/// Builds a sequential list from a root fragment
		/// </summary>
		/// <returns>The sequential list.</returns>
		/// <param name="f">The root fragment</param>
		private static List<IFragment> BuildList(IFragment f)
		{
			var lst = new List<IFragment>();
			var r = f;
			while (r != null)
			{
				lst.Add(r);

				if (r.Children == null)
					r = null;
				else if (r.Children.Length != 1)
					throw new Exception("Cannot modify choice entry");
				else
					r = r.Children.First();
			}

			return lst;
		}

		/// <summary>
		/// Re-links a list by assigning the children
		/// </summary>
		/// <returns>The root elemnt.</returns>
		/// <param name="lst">The list to link.</param>
		private static IFragment LinkList(List<IFragment> lst)
		{
			if (lst.Count == 0)
				throw new ArgumentException(nameof(lst), "Cannot use an empty list");

			if (!(lst[lst.Count - 1] is Result))
			{
				lst[lst.Count - 1] = lst[lst.Count - 1].AttachChildren(null);

				if (lst.Count == 1)
					return lst.First().AttachChildren(null);
			}

			for (var i = lst.Count - 1; i > 0; i--)
				lst[i - 1] = lst[i - 1].AttachChildren(new[] { lst[i] });

			return lst.First();
		}

		/// <summary>
		/// Constructs a new route by appending a path to this instance
		/// </summary>
		/// <param name="path">The path to append.</param>
		/// <param name="caseinsensitive">If set to <c>true</c>, comapres are performed case insensitive.</param>
		/// <param name="target">The target method.</param>
		/// <returns>The combined route</returns>
		public RouteParser Append(string path, bool caseinsensitive, RouteEntry target)
		{
			return Append(new RouteParser(path, caseinsensitive, target));
		}

		/// <summary>
		/// Appends a route to the end of this route
		/// </summary>
		/// <param name="suffix">The route to append.</param>
		/// <returns>The combined route</returns>
		public RouteParser Append(RouteParser suffix)
		{
			var lst = BuildList(this.m_root);
			lst.RemoveAt(lst.Count - 1);
			lst.AddRange(BuildList(suffix.m_root));

			if (!(lst.Last() is Result))
				throw new InvalidOperationException("Cannot append an entry that does not terminate");

			return new RouteParser(LinkList(lst));
		}

		/// <summary>
		/// Prunes the list, by removing empty entries and joining adjacent literals
		/// </summary>
		/// <returns>The pruned list.</returns>
		/// <param name="lst">The list to prune</param>
		private List<IFragment> PruneList(List<IFragment> lst)
		{
			IFragment prev = null;
			for (var i = 0; i < lst.Count; i++)
			{
				var cur = lst[i];
				if (prev != null)
				{					
					if (i != 0 && cur is Literal && string.IsNullOrEmpty(((Literal)cur).Value))
					{
						lst.RemoveAt(i);
						i--;
						continue;
					}

					if (prev is Literal && cur is Literal)
					{
						var s1 = ((Literal)prev).Value;
						var s2 = ((Literal)cur).Value;

						var joinchar = s1.EndsWith("/", StringComparison.Ordinal) || s2.EndsWith("/", StringComparison.Ordinal) ? "/" : "";
						var s3 = s1.TrimEnd(new[] { '/' }) + joinchar + s2.TrimStart(new[] { '/' });

						prev = lst[i - 1] = new Literal(s3, ((Literal)prev).IsCaseSensitive, null);

						// Remove it and continue
						lst.RemoveAt(i);
						i--;
						continue;
					}
					else if (prev is Literal && cur is Result)
					{
						var s = ((Literal)prev).Value.TrimEnd(new[] { '/' });

						// Fixup if we attempt to remove the root slash
						if (lst.Count == 2 && s.Length == 0)
							s += '/';
						
						if (string.IsNullOrEmpty(s))
							lst.RemoveAt(i - 1);
						else
							prev = lst[i - 1] = new Literal(s, ((Literal)prev).IsCaseSensitive, null);
						continue;
					}
				}

				prev = cur;
			}

			return lst;
		}

		/// <summary>
		/// Constructs a new route by pruning this instance
		/// </summary>
		/// <returns>The pruned route.</returns>
		public RouteParser PrunePath()
		{
			return new RouteParser(LinkList(PruneList(BuildList(this.m_root))));
		}

		/// <summary>
		/// Bind the named variable(s) to the given value
		/// </summary>
		/// <param name="name">The name of the variable to bind.</param>
		/// <param name="value">The literal value to bind to.</param>
		/// <param name="iscasesensitive">If set to <c>true</c>, compares are performed case sensitive.</param>
		/// <param name="asliteral">If set to <c>true</c>, binding is done with literal values.</param>
		public RouteParser Bind(string name, string value, bool iscasesensitive, bool asliteral)
		{
			var lst = BuildList(this.m_root);
			for (var i = 0; i < lst.Count; i++)
			{
				var x = lst[i];
				if (x is Variable && ((Variable)x).Name == name)
				{
					if (asliteral)
						lst[i] = new Literal(value, iscasesensitive, x.Children);
					else
						lst[i] = new BoundVariable(name, value, iscasesensitive, x.Children);

					//break; // If we want a single match
				}
			}
				
			var rp = new RouteParser(LinkList(lst));

			return rp;
		}

		/// <summary>
		/// Gets the default value for a variable from this route
		/// </summary>
		/// <returns>The default value.</returns>
		/// <param name="name">The name of the variable.</param>
		public string GetDefaultValue(string name)
		{
			var r = this.m_root;
			while (r != null)
			{
				if (r is Variable && ((Variable)r).Name == name)
					return ((Variable)r).DefaultValue;
				
				if (r.Children == null)
					r = null;
				else if (r.Children.Length != 1)
					throw new Exception("Cannot search choice entry");
				else if (r.Children.First() is Result)
					r = null;
				else
					r = r.Children.First();				
			}

			return null;
		}

		public IEnumerable<KeyValuePair<string, bool>> Variables
		{
			get
			{
				var lst = BuildList(this.m_root);
				return
					lst
						.Where(x => x is Variable)
						.Select(x => new KeyValuePair<string, bool>(((Variable)x).Name, ((Variable)x).Optional))
						.Union(
							lst
							.Where(x => x is BoundVariable)
							.Select(x => new KeyValuePair<string, bool>(((BoundVariable)x).Name, false))
						)
						.Distinct(x => x.Key)
						;
			}
		}

		/// <summary>
		/// Replaces the target entry with one based on the given input
		/// </summary>
		/// <returns>The new route.</returns>
		/// <param name="controller">The controller to use.</param>
		/// <param name="method">The method to call.</param>
		/// <param name="verbs">The accepted verbs.</param>
		public RouteParser ReplaceTarget(Controller controller, System.Reflection.MethodInfo method, string[] verbs)
		{
			var lst = BuildList(this.m_root);
			lst = lst.Take(lst.Count - 1).ToList();
			lst.Add(new Result(new RouteEntry(controller, method, verbs, Variables)));
			return new RouteParser(LinkList(lst));
		}

		/// <summary>
		/// Returns a string-like representation of this entry
		/// </summary>
		public string Path { get { return CloneList(m_root, x => (x is Result) ? null : x).ToString(); } }

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Ceen.Mvc.RouteParser2"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:Ceen.Mvc.RouteParser2"/>.</returns>
		public override string ToString()
		{
			return string.Join(Environment.NewLine, GetAllRoutes());
		}

		/// <summary>
		/// Enumerates all routes and builds a path-like string for each path
		/// </summary>
		/// <returns>The strings representing the routes.</returns>
		private IEnumerable<string> GetAllRoutes()
		{
			var s = new Stack<KeyValuePair<string, IFragment>>();
			s.Push(new KeyValuePair<string, IFragment>(string.Empty, m_root));

			while (s.Count > 0)
			{
				var c = s.Pop();
				var str = c.Key;
				if (c.Value is Literal)
					str += ((Literal)c.Value).Value;
				else if (c.Value is Variable)
					str += "{" + ((Variable)c.Value).Name + "}";
				else if (c.Value is BoundVariable)
					str += "{" + ((BoundVariable)c.Value).Name + "}";
				else if (c.Value is Result)
					str += ((Result)c.Value).ToString();
				else if (!(c.Value is Choice))
					throw new Exception(string.Format("Unable to work with {0}", c.Value.GetType()));

				if (c.Value.Children == null)
					yield return str;
				else
					foreach (var n in c.Value.Children)
						s.Push(new KeyValuePair<string, IFragment>(str, n));
				

			}
		}
	}
}
