using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace Ceen
{
    /// <summary>
    /// Extension methods for the Mvc module
    /// </summary>
    internal static class MvcExtensionMethods
	{
		/// <summary>
		/// Creates a route instance from an assembly
		/// </summary>
		/// <returns>The route.</returns>
		/// <param name="assembly">The assembly to use.</param>
		/// <param name="config">An optional config.</param>
		/// <returns>The router</returns>
		public static Ceen.Mvc.ControllerRouter ToRoute(this Assembly assembly, Ceen.Mvc.ControllerRouterConfig config = null)
		{
			return new Mvc.ControllerRouter(config ?? new Mvc.ControllerRouterConfig(), assembly);
		}

		/// <summary>
		/// Creates a route instance from a list of assemblies
		/// </summary>
		/// <returns>The route.</returns>
		/// <param name="assemblies">The assemblies to use.</param>
		/// <param name="config">An optional config.</param>
		/// <returns>The router</returns>
		public static Ceen.Mvc.ControllerRouter ToRoute(this IEnumerable<Assembly> assemblies, Ceen.Mvc.ControllerRouterConfig config = null)
		{
			return new Mvc.ControllerRouter(config ?? new Mvc.ControllerRouterConfig(), assemblies);
		}

		/// <summary>
		/// Creates a route instance from a list of types
		/// </summary>
		/// <returns>The route.</returns>
		/// <param name="types">The types to use.</param>
		/// <param name="config">An optional config.</param>
		/// <returns>The router</returns>
		public static Ceen.Mvc.ControllerRouter ToRoute(this IEnumerable<Type> types, Ceen.Mvc.ControllerRouterConfig config = null)
		{
			return new Mvc.ControllerRouter(config ?? new Mvc.ControllerRouterConfig(), types);
		}

		/// <summary>
		/// Creates a route instance from a list of types
		/// </summary>
		/// <returns>The route.</returns>
		/// <param name="instances">The declared instances to use.</param>
		/// <param name="config">An optional config.</param>
		/// <returns>The router</returns>
		public static Ceen.Mvc.ControllerRouter ToRoute(this IEnumerable<Ceen.Mvc.Controller> instances, Ceen.Mvc.ControllerRouterConfig config = null)
			=> new Mvc.ControllerRouter(
				config ?? new Mvc.ControllerRouterConfig(), 
				instances
			);

		/// <summary>
		/// Creates a route instance from a list of routes
		/// </summary>
		/// <param name="routes">The routes to use</param>
		/// <param name="config">The config to use</param>
		/// <returns>The router</returns>
		public static Ceen.Mvc.ControllerRouter ToRoute(this IEnumerable<Ceen.Mvc.PartialParsedRoute> routes, Ceen.Mvc.ControllerRouterConfig config = null)
			=> new Mvc.ControllerRouter(
				config ?? new Mvc.ControllerRouterConfig(), 
				new Ceen.Mvc.ManualRoutingController(
					routes
				)
			);

		/// <summary>
		/// Creates a route instance for a manual routing controller
		/// </summary>
		/// <param name="self">The manual routing controller</param>
		/// <param name="config">The config to use</param>
		/// <returns>The controller</returns>
		public static Ceen.Mvc.ControllerRouter ToRoute(this Ceen.Mvc.ManualRoutingController self, Ceen.Mvc.ControllerRouterConfig config = null)
			=> new Mvc.ControllerRouter(
				config ?? new Mvc.ControllerRouterConfig(),
				self
			);

	}
}

namespace Ceen.Mvc
{
    /// <summary>
    /// Some common Linq methods
    /// </summary>
    internal static class LinqHelpers
	{
		/// <summary>
		/// Builds a dictionary where the identical key values overwrite instead of throwing exceptions
		/// </summary>
		/// <returns>The dictionary.</returns>
		/// <param name="self">The list to build from.</param>
		/// <param name="keyselector">The function to extract the key.</param>
		/// <param name="target">An optional target dictionary</param>
		/// <typeparam name="TKey">The key type parameter.</typeparam>
		/// <typeparam name="TItem">The value type parameter.</typeparam>
		public static Dictionary<TKey, TItem> ToSafeDictionary<TKey, TItem>(this IEnumerable<TItem> self, Func<TItem, TKey> keyselector, Dictionary<TKey, TItem> target = null)
		{
			var res = target ?? new Dictionary<TKey, TItem>();
			foreach (var x in self)
				res[keyselector(x)] = x;
			return res;
		}

		/// <summary>
		/// Builds a dictionary where the identical key values overwrite instead of throwing exceptions
		/// </summary>
		/// <returns>The dictionary.</returns>
		/// <param name="self">The list to build from.</param>
		/// <param name="keyselector">The function to extract the key.</param>
		/// <param name="valueselector">The function to extract the value.</param>
		/// <param name="target">An optional target dictionary</param>
		/// <typeparam name="TKey">The key type parameter.</typeparam>
		/// <typeparam name="TValue">The value type parameter.</typeparam>
		/// <typeparam name="TItem">The source type parameter.</typeparam>
		public static Dictionary<TKey, TValue> ToSafeDictionary<TKey, TValue, TItem>(this IEnumerable<TItem> self, Func<TItem, TKey> keyselector, Func<TItem, TValue> valueselector, Dictionary<TKey, TValue> target)
		{
			var res = target ?? new Dictionary<TKey, TValue>();
			foreach (var x in self)
				res[keyselector(x)] = valueselector(x);
			return res;
		}

		/// <summary>
		/// Returns distinct items, by sorting the list and removing duplicate values
		/// </summary>
		/// <param name="self">The list to filter.</param>
		/// <param name="comparevalue">The function to extract the compare parameter.</param>
		/// <param name="comparer">An optional key comparer.</param>
		/// <typeparam name="TItem">The value type parameter.</typeparam>
		/// <typeparam name="TKey">The distinct value type parameter.</typeparam>
		public static IEnumerable<TItem> Distinct<TItem, TKey>(this IEnumerable<TItem> self, Func<TItem, TKey> comparevalue, IComparer<TKey> comparer = null)
		{
			var prev = default(TKey);
			var first = true;
			var cmp = comparer ?? Comparer<TKey>.Default;

			foreach (var item in self.OrderBy(x => comparevalue(x)))
			{
				var k = comparevalue(item);
				if (first || cmp.Compare(prev, k) != 0)
				{
					first = false;
					prev = k;
					yield return item;
				}
			}
		}

		/// <summary>
		/// Gets the covering parent interfaces for a given type
		/// </summary>
		/// <returns>The parent interfaces.</returns>
		/// <param name="self">The type to examine.</param>
		/// <typeparam name="TFilter">An filter to limit the results to a specific type</typeparam>
		public static IEnumerable<Type> GetParentInterfaces<TFilter>(this Type self)
		{
			return GetParentInterfaces(self, typeof(TFilter));
		}

		/// <summary>
		/// Gets the covering parent interfaces for a given type
		/// </summary>
		/// <returns>The parent interfaces.</returns>
		/// <param name="self">The type to examine.</param>
		/// <param name="basetypefilter">An optional filter to limit the results to a specific type.</param>
		public static IEnumerable<Type> GetParentInterfaces(this Type self, Type basetypefilter = null)
		{
			var all = self.GetInterfaces().AsEnumerable();
			if (basetypefilter != null)
				all = all.Where(x => basetypefilter.IsAssignableFrom(x));
			
			return all.Except(all.SelectMany(t => t.GetInterfaces()));
		}

		/// <summary>
		/// Gets a list of parent interfaces
		/// </summary>
		/// <returns>The sequence of parent interfaces.</returns>
		/// <param name="self">The type to start with.</param>
		/// <typeparam name="TFilter">An filter to limit the results to a specific type</typeparam>
		public static IEnumerable<Type> GetParentInterfaceSequence<TFilter>(this Type self)
		{
			return GetParentInterfaceSequence(self, typeof(TFilter));
		}

		/// <summary>
		/// Gets a list of parent interfaces
		/// </summary>
		/// <returns>The sequence of parent interfaces.</returns>
		/// <param name="self">The type to start with.</param>
		/// <param name="basetypefilter">An optional filter to limit the results to a specific type.</param>
		public static IEnumerable<Type> GetParentInterfaceSequence(this Type self, Type basetypefilter = null)
		{
			var cur = self;
			while (cur != null)
			{
				yield return cur;

				var parents = cur.GetParentInterfaces(basetypefilter);

				if (parents.Count() > 1)
					throw new Exception($"Error building prefix map, the type {cur.FullName} (from {self.FullName} has multiple parents");
				cur = parents.FirstOrDefault();

				if (cur == basetypefilter)
					basetypefilter = null;
			}
		}

		/// <summary>
		/// Searches the type inheritance tree until it finds the desired attribute.
		/// This approach allows a derived class to override attributes, but defaults to inheriting.
		/// </summary>
		/// <param name="self">The type to examine</param>
		/// <typeparam name="T">The attribute type to find</typeparam>
		/// <returns>The attributes found</returns>
		public static IEnumerable<T> SearchCustomAttributes<T>(this Type self)
			where T : Attribute
		{
			if (self == null)
				throw new ArgumentNullException(nameof(self));
			
			do
			{
				var res = self.GetCustomAttributes(typeof(T), false);
				if (res != null && res.Length > 0)
					return res.Cast<T>();

				self = self.BaseType;
			} while(self != null);

			return new T[0];
		}

		/// <summary>
		/// Searches the type inheritance tree until it finds the desired attribute.
		/// This approach allows a derived class to override attributes, but defaults to inheriting.
		/// </summary>
		/// <param name="self">The type to examine</param>
		/// <typeparam name="T">The attribute type to find</typeparam>
		/// <returns>The attributes found</returns>
		public static IEnumerable<T> SearchCustomAttributes<T>(this MethodInfo self)
			where T : Attribute
		{
			if (self == null)
				throw new ArgumentNullException(nameof(self));
			
			do
			{
				var res = self.GetCustomAttributes(typeof(T), false);
				if (res != null && res.Length > 0)
					return res.Cast<T>();

				var next = self.GetBaseDefinition();
				if (self == next)
					break;
				self = next;
			} while(self != null);

			return new T[0];
		}		

		/// <summary>
		/// Gets the name of an item
		/// </summary>
		/// <returns>The item name.</returns>
		/// <param name="self">The type to get the name for.</param>
		/// <param name="config">The configuration to use</param>
		public static string GetItemName(this Type self, ControllerRouterConfig config)
		{
			var nameattr = self.SearchCustomAttributes<NameAttribute>().Cast<NameAttribute>().FirstOrDefault();
			// Extract an assigned name
			if (nameattr != null)
				return nameattr.Name;
			
			var name = self.Name;
			if (typeof(Controller).IsAssignableFrom(self) && self.IsClass)
			{
				if (config.ControllerSuffixRemovals != null)
					foreach (var rm in config.ControllerSuffixRemovals)
						while (!string.IsNullOrWhiteSpace(rm) && name.EndsWith(rm, StringComparison.InvariantCultureIgnoreCase))
							name = name.Substring(0, name.Length - rm.Length);
				if (config.ControllerPrefixRemovals != null)
					foreach (var rm in config.ControllerPrefixRemovals)
						while (!string.IsNullOrWhiteSpace(rm) && name.StartsWith(rm, StringComparison.InvariantCultureIgnoreCase))
							name = name.Substring(rm.Length);
			}
			else if (typeof(IControllerPrefix).IsAssignableFrom(self) && self.IsInterface)
			{
				if (config.InterfaceSuffixRemovals != null)
					foreach (var rm in config.InterfaceSuffixRemovals)
						while (!string.IsNullOrWhiteSpace(rm) && name.EndsWith(rm, StringComparison.InvariantCultureIgnoreCase))
							name = name.Substring(0, name.Length - rm.Length);
				if (config.InterfacePrefixRemovals != null)
					foreach (var rm in config.InterfacePrefixRemovals)
						while (!string.IsNullOrWhiteSpace(rm) && name.StartsWith(rm, StringComparison.InvariantCultureIgnoreCase))
							name = name.Substring(rm.Length);
			}

			if (config.LowerCaseNames)
				name = name.ToLowerInvariant();

			return name;
		}

		/// <summary>
		/// Gets the name of an item
		/// </summary>
		/// <returns>The item name.</returns>
		/// <param name="self">The type to get the name for.</param>
		/// <param name="config">The configuration to use</param>
		public static string GetItemName(this MethodInfo self, ControllerRouterConfig config)
		{
			var nameattr = self.SearchCustomAttributes<NameAttribute>().Cast<NameAttribute>().FirstOrDefault();
			// Extract an assigned name
			if (nameattr != null)
				return nameattr.Name;

			var name = self.Name;
			if (config.LowerCaseNames)
				name = name.ToLowerInvariant();

			return name;
		}
	}

    /// <summary>
    /// Represents the parsed route before binding any variables
    /// </summary>
    internal class PartialParsedRoute
	{
		/// <summary>
		/// The controller instance to use
		/// </summary>
		public Controller Controller;
		/// <summary>
		/// The path fragment on the controller
		/// </summary>
		public string ControllerPath;
		/// <summary>
		/// The path fragment on the interface
		/// </summary>
		public string InterfacePath;
		/// <summary>
		/// The path fragment on the method
		/// </summary>
		public string MethodPath;
		/// <summary>
		/// The method to invoke
		/// </summary>
		public MethodInfo Method;
		/// <summary>
		/// The verbs accepted by the method
		/// </summary>
		public string[] Verbs;
	}

    /// <summary>
    /// Router that can route to a set of controllers
    /// </summary>
    internal class ControllerRouter : IRouter, IHttpModule
	{
		/// <summary>
		/// The template used to locate controllers
		/// </summary>
		private readonly ControllerRouterConfig m_config;

		/// <summary>
		/// The route parser
		/// </summary>
		private readonly RouteParser m_routeparser;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.ControllerRouter"/> class.
		/// </summary>
		/// <param name="config">The configuration to use</param>
		/// <param name="assembly">The assembly to scan for controllers.</param>
		public ControllerRouter(ControllerRouterConfig config, Assembly assembly)
			: this(config, assembly == null ? new Assembly[0] : new [] { assembly })
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.ControllerRouter"/> class.
		/// </summary>
		/// <param name="config">The configuration to use</param>
		/// <param name="assemblies">The assemblies to scan for controllers.</param>
		public ControllerRouter(ControllerRouterConfig config, IEnumerable<Assembly> assemblies)
			: this(config, assemblies.SelectMany(x => x.GetTypes()).Where(x => x != null && typeof(Controller).IsAssignableFrom(x) && !x.IsAbstract && !x.IsGenericTypeDefinition))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.ControllerRouter"/> class.
		/// </summary>
		/// <param name="config">The configuration to use</param>
		/// <param name="types">The types to use, must all derive from <see cref="T:Ceen.Mvc.Controller"/>.</param>
		public ControllerRouter(ControllerRouterConfig config, params Type[] types)
			: this(config, (types ?? new Type[0]).AsEnumerable())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.ControllerRouter"/> class.
		/// </summary>
		/// <param name="config">The configuration to use</param>
		/// <param name="types">The types to use, must all derive from <see cref="T:Ceen.Mvc.Controller"/>.</param>
		public ControllerRouter(ControllerRouterConfig config, IEnumerable<Type> types)
		{
			if (types == null)
				throw new ArgumentNullException($"{types}");
			if (config == null)
				throw new ArgumentNullException($"{config}");

			// Make sure the caller cannot edit the config afterwards
			m_config = config.Clone();

			var variables = new RouteParser(m_config.Template, !m_config.CaseSensitive, null).Variables;

			if (variables.Count(x => x.Key == m_config.ControllerGroupName) != 1)
				throw new ArgumentException($"The template must contain exactly 1 named group called {m_config.ControllerGroupName}");
			if (variables.Count(x => x.Key == m_config.ActionGroupName) != 1)
				throw new ArgumentException($"The template must contain exactly 1 named group called {m_config.ActionGroupName}");

			types = types.Where(x => x != null).Distinct().ToArray();
			if (types.Count() == 0)
				throw new ArgumentException($"Attempted to load routes from an assembly, or set of types with no routes", nameof(types));

			var wrong = types.FirstOrDefault(x => !typeof(Controller).IsAssignableFrom(x));
			if (wrong != null)
				throw new ArgumentException($"The type \"{wrong.FullName}\" does not derive from {typeof(Controller).FullName}");

            wrong = types.FirstOrDefault(x => x.IsAbstract || x.IsGenericTypeDefinition);
            if (wrong != null)
                throw new ArgumentException($"The type \"{wrong.FullName}\" cannot be instantiated");

            m_routeparser = BuildParse(types.Select(x => (Controller)Activator.CreateInstance(x)).ToArray(), m_config);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.ControllerRouter"/> class.
		/// </summary>
		/// <param name="config">The configuration to use</param>
		/// <param name="instances">The instances to use.</param>
		public ControllerRouter(ControllerRouterConfig config, params Controller[] instances)
			: this(config, (instances ?? new Controller[0]).AsEnumerable())
		{
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.ControllerRouter"/> class.
		/// </summary>
		/// <param name="config">The configuration to use</param>
		/// <param name="instances">The instances to use.</param>
		public ControllerRouter(ControllerRouterConfig config, IEnumerable<Ceen.Mvc.Controller> instances)
		{
			if (instances == null || instances.Any(x => x == null))
				throw new ArgumentNullException($"{instances}");
			if (config == null)
				throw new ArgumentNullException($"{config}");

			// Make sure the caller cannot edit the config afterwards
			m_config = config.Clone();

			var variables = new RouteParser(m_config.Template, !m_config.CaseSensitive, null).Variables;

			if (variables.Count(x => x.Key == m_config.ControllerGroupName) != 1)
				throw new ArgumentException($"The template must contain exactly 1 named group called {m_config.ControllerGroupName}");
			if (variables.Count(x => x.Key == m_config.ActionGroupName) != 1)
				throw new ArgumentException($"The template must contain exactly 1 named group called {m_config.ActionGroupName}");

			if (instances.Count() == 0)
				throw new ArgumentException($"No instances were added as routes", nameof(instances));

			var duplicates = instances.GroupBy(x => x.GetType()).Where(x => x.Count() > 1);
			if (duplicates.Any())
				throw new ArgumentException($"The type \"{duplicates.First().Key}\" has {duplicates.First().Count()} instances");

            m_routeparser = BuildParse(instances, m_config);
		}		

		/// <summary>
		/// Creates partial routes for all controllers in the sequence
		/// </summary>
		/// <param name="controllers">The controllers to create partial routes for</param>
		/// <param name="config">The configuration to use</param>
		/// <returns>The partial routes</returns>
		public static PartialParsedRoute[] ParseControllers(IEnumerable<Controller> controllers, ControllerRouterConfig config)
		{
			var controller_routes = controllers
				.Where(x => !(x is ManualRoutingController))
				.SelectMany(x =>
				{
					string name;
					var nameattr = x.GetType().SearchCustomAttributes<NameAttribute>().Cast<NameAttribute>().FirstOrDefault();
					// Extract controller name
					if (nameattr != null)
						name = nameattr.Name;
					else
					{
						name = x.GetType().Name;
						if (config.ControllerSuffixRemovals != null)
							foreach (var rm in config.ControllerSuffixRemovals)
								while (!string.IsNullOrWhiteSpace(rm) && name.EndsWith(rm, StringComparison.InvariantCultureIgnoreCase))
									name = name.Substring(0, name.Length - rm.Length);

						if (config.LowerCaseNames)
							name = name.ToLowerInvariant();
					}

					var routes = x.GetType().SearchCustomAttributes<RouteAttribute>().Cast<RouteAttribute>().Select(y => y.Route);
					
					// Add default route, if there are no route attributes
					if (routes.Count() == 0)
						routes = new[] { string.Empty };

					return routes.Distinct().Select(y => new
					{
						Controller = x,
						ControllerRoute = y
					});
				}
			).ToArray();

			var interface_expanded_routes =
				controller_routes.SelectMany(x =>
				{
					var interfaces = x.Controller.GetType().GetParentInterfaces<IControllerPrefix>();
					var interfacenames = interfaces
						.Select(y => x.Controller.GetType().GetParentInterfaceSequence(y).Reverse().Where(z => z != x.Controller.GetType() && z != typeof(IControllerPrefix)))
						.Select(y => string.Join("/", y.Select(z => z.GetItemName(config))));

					if (interfacenames.Count() == 0)
						interfacenames = new[] { string.Empty };

					return interfacenames.Distinct().Select(y => new
					{
						Controller = x.Controller,
						ControllerRoute = x.ControllerRoute,
						InterfacePath = (y ?? string.Empty)
					});
                }
            ).ToArray();


			var target_method_routes =
				interface_expanded_routes.SelectMany(x =>
				{
					return
						x.Controller.GetType()
							.GetMethods(BindingFlags.Public | BindingFlags.Instance)
							// Only target the methods that return something useful
							.Where(y => 
								y.ReturnType == typeof(void) 
								|| typeof(IResult).IsAssignableFrom(y.ReturnType) 
								|| typeof(Task).IsAssignableFrom(y.ReturnType)
							)
							// Remove properties and others
							.Where(y => !y.IsSpecialName)
							.Select(y => new
							{
								Controller = x.Controller,
								ControllerRoute = x.ControllerRoute,
								InterfacePath = x.InterfacePath,
								Method = y
							});
				}
            ).ToArray();

			return
				target_method_routes.SelectMany(x =>
				{
					var routes = x.Method.SearchCustomAttributes<RouteAttribute>().Cast<RouteAttribute>().Select(y => y.Route);

					// Add default route, if there are no route attributes
					if (routes.Count() == 0)
						routes = new[] { string.Empty };

					// Get any accepted verbs
					var verbs = x.Method.SearchCustomAttributes<HttpVerbFilterAttribute>().Cast<HttpVerbFilterAttribute>().Select(b => b.Verb.ToUpperInvariant()).ToArray();

					return routes.Select(y => new PartialParsedRoute()
					{
						Controller = x.Controller,
						ControllerPath = x.ControllerRoute,
						InterfacePath = x.InterfacePath,
						Method = x.Method,
						MethodPath = y,		
						Verbs = verbs
					});				
				}
			)
			.ToArray();
		}

		private static RouteParser BuildParse(IEnumerable<Controller> controllers, ControllerRouterConfig config)
		{
			var target_routes =
				ParseControllers(controllers, config)

				// Attach custom controller routes
				.Concat(
					controllers.OfType<ManualRoutingController>().SelectMany(x => x.Routes)
				)
				.ToArray();

			// Now we have the cartesian product of all route/controller/action pairs, then build the target strings
			var tmp = new RouteParser(config.Template, !config.CaseSensitive, null);
			var defaultcontrollername = tmp.GetDefaultValue(config.ControllerGroupName);
			var defaultactionname = tmp.GetDefaultValue(config.ActionGroupName);

			var target_strings =
				target_routes.SelectMany(x =>
				{
					// Provide a hook point for the dynamic routing option
					if (x.Controller is IIDynamicConfiguredController idc)
						x = idc.PatchRoute(x);

					var entry = new RouteEntry(x.Controller, x.Method, x.Verbs, null);

					var path = new RouteParser("/", !config.CaseSensitive, entry);

					if (!string.IsNullOrWhiteSpace(x.InterfacePath))
					{
						if (x.InterfacePath.StartsWith("/", StringComparison.Ordinal))
							path = new RouteParser(x.InterfacePath, !config.CaseSensitive, entry);
						else							
							path = path.Append(x.InterfacePath, !config.CaseSensitive, entry);
					}

					var ct = string.IsNullOrWhiteSpace(x.ControllerPath) ? config.Template : x.ControllerPath;

					if (!string.IsNullOrWhiteSpace(ct))
					{
						if (ct.StartsWith("/", StringComparison.Ordinal))
							path = new RouteParser(ct, !config.CaseSensitive, entry);
						else
						{
							if (!path.Path.EndsWith("/", StringComparison.Ordinal))
								ct = "/" + ct;
							path = path.Append(ct, !config.CaseSensitive, entry);
						}
					}

					var mt = x.MethodPath;
					if (!string.IsNullOrWhiteSpace(mt))
					{
						if (mt.StartsWith("/", StringComparison.Ordinal))
							path = new RouteParser(mt, !config.CaseSensitive, entry);
						else
						{
							path = path.Bind(config.ActionGroupName, string.Empty, !config.CaseSensitive, true);
							if (!path.Path.EndsWith("/", StringComparison.Ordinal))
								mt = "/" + mt;
						
							path = path.Append(mt, !config.CaseSensitive, entry);
						}
					}

					var controllername = x.Controller.GetType().GetItemName(config);
					var actionname = x.Method.GetItemName(config);

					var controllernames = new List<string>();
					var actionnames = new List<string>();

					if (!config.HideDefaultController || controllername != defaultcontrollername)
						controllernames.Add(controllername);
					if (!config.HideDefaultAction || actionname != defaultactionname)
						actionnames.Add(actionname);
					if (!string.IsNullOrEmpty(defaultcontrollername) && controllername == defaultcontrollername)
						controllernames.Add(string.Empty);
					if (!string.IsNullOrEmpty(defaultactionname) && actionname == defaultactionname)
						actionnames.Add(string.Empty);

				// Cartesian product with controller and action names bound
				return controllernames
					.SelectMany(y => actionnames.Select(
						z => path
							.Bind(config.ControllerGroupName, y, !config.CaseSensitive, true)
							.Bind(config.ActionGroupName, z, !config.CaseSensitive, true)							
							.PrunePath()
							.ReplaceTarget(x.Controller, x.Method, x.Verbs)
							)
		                );
				}
			)
            .Distinct(x => x.ToString())
         	.ToArray();

			var merged = RouteParser.Merge(target_strings);

			if (config.Debug)
			{
				Console.WriteLine("All target paths:");
				foreach (var x in target_strings)
					Console.WriteLine(x);

				Console.WriteLine("Map structure:");
				Console.WriteLine(merged.ToString());
			}

			return merged;
		}

		/// <summary>
		/// Handles a request
		/// </summary>
		/// <returns>The awaitable task.</returns>
		/// <param name="context">The exexcution context.</param>
		public Task<bool> HandleAsync(IHttpContext context)
		{
			return Process(context);
		}

		/// <summary>
		/// Attempts to route the request to a controller instance.
		/// </summary>
		/// <param name="context">The exexcution context.</param>
		public async Task<bool> Process(IHttpContext context)
		{
			var anymatches = false;
			var test = m_routeparser
				.MatchRequest(context.Request.Path)
				.Where(x => { anymatches = true; return x.Key.AcceptsVerb(context.Request.Method); })
				.OrderBy(x => x.Value.Count)
				.ToList();

			// If we do not handle this entry, pass the request down the stack
			if (!anymatches)
				return false;

			// If we filtered all targets based on verb,
			// we are handling the path, but the verb is not allowed
			if (test.Count == 0)
			{
				context.Response.StatusCode = HttpStatusCode.MethodNotAllowed;
				context.Response.StatusMessage = HttpStatusMessages.DefaultMessage(HttpStatusCode.MethodNotAllowed);
				return true;
			}

			var handler = test.First();
			await HandleWithMethod(context, handler.Key.Action, handler.Key.Controller, handler.Value);
			return true;
		}

		/// <summary>
		/// Handles the actual method invocation, once a method has been selected
		/// </summary>
		/// <returns>The awaitable task.</returns>
		/// <param name="context">The execution context.</param>
		/// <param name="method">The method to invoke.</param>
		/// <param name="controller">The controller instance to use.</param>
		/// <param name="urlmatch">The parent url match</param>
		private async Task HandleWithMethod(IHttpContext context, MethodEntry method, Controller controller, Dictionary<string, string> urlmatch)
		{
			// Make sure dependencies are met
			if (controller != null)
				context.Request.RequireHandler(controller.GetType().SearchCustomAttributes<RequireHandlerAttribute>());
			context.Request.RequireHandler(method.Method.SearchCustomAttributes<RequireHandlerAttribute>());
			
			// Apply each argument in turn
			var values = new object[method.ArgumentCount];
            var hasreadbody = false;

			foreach (var m in urlmatch)
				context.Request.RequestState[m.Key] = m.Value;

			for (var ix = 0; ix < values.Length; ix++)
			{
				var e = method.Parameters[ix];
				string val;
                IMultipartItem file;

                if (typeof(IHttpContext).IsAssignableFrom(e.Parameter.ParameterType))
                    values[ix] = context;
                else if (typeof(IHttpRequestInternal).IsAssignableFrom(e.Parameter.ParameterType))
                    values[ix] = context.Request;
                else if (typeof(IHttpResponse).IsAssignableFrom(e.Parameter.ParameterType))
                    values[ix] = context.Response;
                else if (e.Source.HasFlag(ParameterSource.Url) && urlmatch != null && urlmatch.TryGetValue(e.Name, out val))
                    ApplyArgument(method.Method, e, val, values);
                else if (e.Source.HasFlag(ParameterSource.Header) && context.Request.Headers.TryGetValue(e.Name, out val))
                    ApplyArgument(method.Method, e, val, values);
                else if (e.Source.HasFlag(ParameterSource.Form) && context.Request.Form.TryGetValue(e.Name, out val))
                    ApplyArgument(method.Method, e, val, values);
                else if (e.Source.HasFlag(ParameterSource.Form) && ((file = context.Request.Files.FirstOrDefault(x => string.Equals(x.Name, e.Name, StringComparison.OrdinalIgnoreCase))) != null))
                    ApplyArgument(method.Method, e, await RequestUtility.ReadAllAsStringAsync(file.Data, RequestUtility.GetEncodingForContentType(file.Headers["Content-Type"]), context.Request.TimeoutCancellationToken), values);
                else if (e.Source.HasFlag(ParameterSource.Query) && context.Request.QueryString.TryGetValue(e.Name, out val))
                    ApplyArgument(method.Method, e, val, values);
                else if (e.Source.HasFlag(ParameterSource.Body) && !hasreadbody && RequestUtility.IsJsonRequest(context.Request.ContentType))
                {
                    // We can have at most one body param
                    hasreadbody = true;
                    var str = await RequestUtility.ReadAllAsStringAsync(context.Request.Body, RequestUtility.GetEncodingForContentType(context.Request.ContentType), context.Request.TimeoutCancellationToken);
                    ApplyArgument(method.Method, e, str, values);
                }
				else if (e.Required)
					throw new HttpException(HttpStatusCode.BadRequest, $"Missing mandatory parameter {e.Name}");
				else if (e.Parameter.HasDefaultValue)
					values[e.ArgumentIndex] = e.Parameter.DefaultValue;
				else
					values[e.ArgumentIndex] = e.Parameter.ParameterType.IsValueType ? Activator.CreateInstance(e.Parameter.ParameterType) : null;

				// Provide the bound variables to the processing module
				if (!e.IsContextParameter)
					context.Request.RequestState[e.Name] = values[e.ArgumentIndex];
			}

			var res = method.Method.Invoke(controller, values);
			if (res == null)
				return;

			if (res is IResult)
				await((IResult)res).Execute(context);
			else if (res is Task<IResult>)
			{
				res = await(Task<IResult>)res;
				if (res as IResult != null)
					await((IResult)res).Execute(context);
			}
			else if (res is Task)
				await(Task)res;
		}

		/// <summary>
		/// Applies the argument to the value list.
		/// </summary>
		/// <param name="entry">The argument entry.</param>
		/// <param name="method">The method to apply to.</param>
		/// <param name="value">The argument value.</param>
		/// <param name="values">The list of values to process.</param>
		private static void ApplyArgument(MethodInfo method, ParameterEntry entry, string value, object[] values)
		{
			var argtype = method.GetParameters()[entry.ArgumentIndex].ParameterType;
			try
			{
				if (argtype.IsEnum)
					values[entry.ArgumentIndex] = Enum.Parse(argtype, value, true);
                else if (argtype.IsPrimitive || argtype == typeof(string))
                    values[entry.ArgumentIndex] = Convert.ChangeType(value, argtype);
                else
                    values[entry.ArgumentIndex] = JsonSerializer.Deserialize(value, argtype);
			}
			catch (Exception)
			{
				throw new HttpException(HttpStatusCode.BadRequest, $"The value \"{value}\" for {entry.Name} is not a valid {argtype.Name.ToLower()}");
			}
		}
	}
}
