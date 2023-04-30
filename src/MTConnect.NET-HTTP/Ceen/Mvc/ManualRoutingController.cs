using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Ceen.Mvc
{
    /// <summary>
    /// A controller that does not rely on attributes,
    /// but generates the routes at runtime
    /// </summary>
    public class ManualRoutingController : Controller
    {
        /// <summary>
        /// The list of dynamic routes
        /// </summary>
        public IEnumerable<PartialParsedRoute> Routes => m_routes;

        /// <summary>
        /// The partially parsed routes
        /// </summary>
        private List<PartialParsedRoute> m_routes = new List<PartialParsedRoute>();

        /// <summary>
        /// Dictionary of auto-created instances
        /// </summary>
        private readonly Dictionary<Type, Controller> m_instances = new Dictionary<Type, Controller>();

        /// <summary>
        /// The current type, if using WireWith
        /// </summary>
        public Controller Current { get; private set; }

        /// <summary>
        /// Parameterless (default) constructor
        /// </summary>
        public ManualRoutingController()
        {
        }

        /// <summary>
        /// Creates a controller with a preconfigured set of routes
        /// </summary>
        /// <param name="routes">The routes to add</param>
        public ManualRoutingController(IEnumerable<PartialParsedRoute> routes)
            : this()
        {
            if (routes == null)
                throw new ArgumentNullException(nameof(routes));
            m_routes.AddRange(routes);
        }

        /// <summary>
        /// Adds a dynamic route to the list of routes
        /// </summary>
        /// <param name="route"></param>
        public ManualRoutingController AddRoute(PartialParsedRoute route)
        {
            m_routes.Add(route ?? throw new ArgumentNullException(nameof(route)));
            return this;
        }

        /// <summary>
        /// Adds a new fully specified route
        /// </summary>
        /// <param name="path">The full path</param>
        /// <param name="verbs">The verbs allowed for the method</param>
        /// <parma name="instance">The instace to invoke the method on</param>
        /// <param name="method">The method to invoke</param>
        public ManualRoutingController AddRoute(string path, string[] verbs, Controller instance, MethodInfo method)
        {
            return AddRoute(new PartialParsedRoute()
            {
                MethodPath = path,
                Verbs = verbs,
                Controller = instance,
                Method = method ?? throw new ArgumentNullException(nameof(method))
            });
        }

        /// <summary>
        /// Adds a new fully specified route
        /// </summary>
        /// <param name="path">The full path to use for the method prefixed with the verb</param>
        /// <param name="method">The method to invoke</param>
        public ManualRoutingController AddDelegateRoute(string path, Delegate method)
        {
            var items = path?.Split(new char[] { ' ' }, 2);
            if (items == null || items.Length != 2 || items[0].IndexOf("/") >= 0 || !items[1].StartsWith("/"))
                throw new ArgumentException($"The path must be of the form \"VERB /path\"", nameof(path));

            var verbs = items[0].Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            verbs = verbs.Any(x => x == "*") ? null : verbs;

            if (method.Target != null && !(method.Target is Controller))
                throw new ArgumentException("The method target must either be null (static method) or an instance deriving from Controller");

            return AddRoute(items[1], verbs, (Controller)method.Target, method.GetMethodInfo());
        }

        /// <summary>
        /// Sets the base type to use for Wire calls without a type
        /// </summary>
        /// <typeparam name="T">The type to set</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController WireWith<T>()
            where T : Controller
            => WireWith(typeof(T));

        /// <summary>
        /// Sets the base type to use for Wire calls without a type
        /// </summary>
        /// <param name="instance">The instance to use</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController WireWith(Controller instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            Current = instance;

            return this;
        }

        /// <summary>
        /// Sets the base type to use for Wire calls without a type
        /// </summary>
        /// <param name="type">The type to set</param>
        /// <returns>The route instance</returns>
        public ManualRoutingController WireWith(Type type)
        {
            if (!typeof(Controller).IsAssignableFrom(type))
                throw new ArgumentException($"The type {type} does not derive from {nameof(Controller)}");

            if (!m_instances.TryGetValue(type, out var inst))
                m_instances.Add(type, inst = (Controller)Activator.CreateInstance(type));
            return WireWith(inst);
        }

        /// <summary>
        /// Wires a path to a method, using the current controller set with WireWith and looking up the method name
        /// </summary>
        /// <param name="path">The path to wire up</param>
        /// <param name="method">The name of the method to invoke</param>
        /// <param name="argumenttypes">The argumnent types, if needed to resolve overloads</param>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire(string path, string method, params Type[] argumenttypes)
        {
            if (Current == null)
                throw new ArgumentException($"Must call {nameof(WireWith)} to set the type context");

            // Capture the context as it is reset by the Wire method
            var c = Current;
            Wire(Current, path, method, argumenttypes);
            // Then set it back
            Current = c;

            return this;
        }

        /// <summary>
        /// Wires a path to a method, looking up the method name on the supplied type
        /// </summary>
        /// <param name="path">The path to wire up</param>
        /// <param name="method">The name of the method to invoke</param>
        /// <param name="argumenttypes">The argumnent types, if needed to resolve overloads</param>
        /// <typeparam name="T">The type to use for lookup</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire<T>(string path, string method, params Type[] argumenttypes)
            where T : Controller
            => Wire(typeof(T), path, method, argumenttypes);


        /// <summary>
        /// Wires a path to a method, looking up the method name on the supplied type
        /// </summary>
        /// <param name="type">The type to use for lookup</param>
        /// <param name="path">The path to wire up</param>
        /// <param name="method">The name of the method to invoke</param>
        /// <param name="argumenttypes">The argumnent types, if needed to resolve overloads</param>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire(Controller instance, string path, string method, params Type[] argumenttypes)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentNullException(nameof(method));

            var type = instance.GetType();
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Where(x => x.Name == method)
                .ToArray();

            var m =
                methods.Length == 1
                ? type.GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                : type.GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy, null, argumenttypes, null);

            if (m == null)
                throw new ArgumentException($"No method matching \"{method}{(methods.Length == 1 ? string.Empty : "(" + string.Join(",", argumenttypes.Select(x => x.Name)) + ")")}\" was found in {type}");

            var delegatetype = System.Linq.Expressions.Expression.GetDelegateType(
                m.GetParameters().Select(x => x.ParameterType)
                .Concat(new [] { m.ReturnType })
                .ToArray()
            );

            // Clear this to avoid weird logic where Wire can reference an older WireWith
            Current = null;
            return AddDelegateRoute(path, Delegate.CreateDelegate(delegatetype, instance, m, true));            
        }

        /// <summary>
        /// Wires a path to a method, looking up the method name on the supplied type
        /// </summary>
        /// <param name="type">The type to use for lookup</param>
        /// <param name="path">The path to wire up</param>
        /// <param name="method">The name of the method to invoke</param>
        /// <param name="argumenttypes">The argumnent types, if needed to resolve overloads</param>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire(Type type, string path, string method, params Type[] argumenttypes)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!typeof(Controller).IsAssignableFrom(type))
                throw new ArgumentException($"The type {type} does not derive from {nameof(Controller)}", nameof(type));

            if (!m_instances.TryGetValue(type, out var inst))
                m_instances.Add(type, inst = (Controller)Activator.CreateInstance(type));

            return Wire(inst, path, method, argumenttypes);
        }

        /// <summary>
        /// Wires all methods in a controller under the given prefix
        /// </summary>
        /// <param name="prefix">The prefix to use</param>
        /// <param name="config">The optional configuration to use</param>
        /// <typeparam name="T">The type to wire up</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController WireController<T>(string prefix, ControllerRouterConfig config = null)
            where T : Controller
            => WireController(typeof(T), prefix, config);

        /// <summary>
        /// Wires all methods in a controller under the given prefix
        /// </summary>
        /// <param name="type">The type to wire up, must be a Controller subclass</param>
        /// <param name="prefix">The prefix to use</param>
        /// <param name="config">The optional configuration to use</param>
        /// <returns>The route instance</returns>
        public ManualRoutingController WireController(Type type, string prefix, ControllerRouterConfig config = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!typeof(Controller).IsAssignableFrom(type))
                throw new ArgumentException($"The type {type} does not derive from {nameof(Controller)}", nameof(type));
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentNullException(nameof(prefix));
            if (!m_instances.TryGetValue(type, out var inst))
                m_instances.Add(type, inst = (Controller)Activator.CreateInstance(type));

            return WireController(inst, prefix, config);
        }

        /// <summary>
        /// Wires all methods in a controller under the given prefix
        /// </summary>
        /// <param name="instance">The controller to wire up</param>
        /// <param name="prefix">The prefix to use</param>
        /// <param name="config">The optional configuration to use</param>
        /// <param name="controllerpath">An optional controller path to use</param>
        /// <returns>The route instance</returns>
        public ManualRoutingController WireController(Controller instance, string prefix, ControllerRouterConfig config = null, string controllerpath = null)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentNullException(nameof(prefix));

            config = config ?? new ControllerRouterConfig();

            // Extract all target methods, but remove their controller and interface fragments
            foreach(var r in ControllerRouter.ParseControllers(new [] { instance }, config))
                AddRoute(new PartialParsedRoute() {
                    InterfacePath = prefix,
                    ControllerPath = controllerpath ?? $"{{{config.ActionGroupName}=index}}",
                    Controller = r.Controller,
                    MethodPath = r.MethodPath,
                    Method = r.Method,
                    Verbs = r.Verbs
                });

            return this;
        }

        #region Typed function overloads
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire(string path, Func<IResult> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire<T1>(string path, Func<T1, IResult> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire<T1, T2>(string path, Func<T1, T2, IResult> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire<T1, T2, T3>(string path, Func<T1, T2, T3, IResult> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire<T1, T2, T3, T4>(string path, Func<T1, T2, T3, T4, IResult> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire<T1, T2, T3, T4, T5>(string path, Func<T1, T2, T3, T4, T5, IResult> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6>(string path, Func<T1, T2, T3, T4, T5, T6, IResult> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7>(string path, Func<T1, T2, T3, T4, T5, T6, T7, IResult> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, IResult> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, IResult> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, IResult> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, IResult> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, IResult> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <typeparam name="T13">The 13th function argument type</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, IResult> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <typeparam name="T13">The 13th function argument type</typeparam>
        /// <typeparam name="T14">The 14th function argument type</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, IResult> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <typeparam name="T13">The 13th function argument type</typeparam>
        /// <typeparam name="T14">The 14th function argument type</typeparam>
        /// <typeparam name="T15">The 15th function argument type</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, IResult> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <typeparam name="T13">The 13th function argument type</typeparam>
        /// <typeparam name="T14">The 14th function argument type</typeparam>
        /// <typeparam name="T15">The 15th function argument type</typeparam>
        /// <typeparam name="T16">The 16th function argument type</typeparam>
        /// <returns>The route instance</returns>
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, IResult> func)
            => AddDelegateRoute(path, func);

        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire(string path, Func<Task<IResult>> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1>(string path, Func<T1, Task<IResult>> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2>(string path, Func<T1, T2, Task<IResult>> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3>(string path, Func<T1, T2, T3, Task<IResult>> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4>(string path, Func<T1, T2, T3, T4, Task<IResult>> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5>(string path, Func<T1, T2, T3, T4, T5, Task<IResult>> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6>(string path, Func<T1, T2, T3, T4, T5, T6, Task<IResult>> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7>(string path, Func<T1, T2, T3, T4, T5, T6, T7, Task<IResult>> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<IResult>> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<IResult>> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<IResult>> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task<IResult>> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Task<IResult>> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <typeparam name="T13">The 13th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Task<IResult>> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <typeparam name="T13">The 13th function argument type</typeparam>
        /// <typeparam name="T14">The 14th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Task<IResult>> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <typeparam name="T13">The 13th function argument type</typeparam>
        /// <typeparam name="T14">The 14th function argument type</typeparam>
        /// <typeparam name="T15">The 15th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Task<IResult>> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <typeparam name="T13">The 13th function argument type</typeparam>
        /// <typeparam name="T14">The 14th function argument type</typeparam>
        /// <typeparam name="T15">The 15th function argument type</typeparam>
        /// <typeparam name="T16">The 16th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, Task<IResult>> func)
            => AddDelegateRoute(path, func);

        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire(string path, Func<Task> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1>(string path, Func<T1, Task> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2>(string path, Func<T1, T2, Task> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3>(string path, Func<T1, T2, T3, Task> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4>(string path, Func<T1, T2, T3, T4, Task> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5>(string path, Func<T1, T2, T3, T4, T5, Task> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6>(string path, Func<T1, T2, T3, T4, T5, T6, Task> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7>(string path, Func<T1, T2, T3, T4, T5, T6, T7, Task> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Task> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <typeparam name="T13">The 13th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Task> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <typeparam name="T13">The 13th function argument type</typeparam>
        /// <typeparam name="T14">The 14th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Task> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <typeparam name="T13">The 13th function argument type</typeparam>
        /// <typeparam name="T14">The 14th function argument type</typeparam>
        /// <typeparam name="T15">The 15th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Task> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <typeparam name="T13">The 13th function argument type</typeparam>
        /// <typeparam name="T14">The 14th function argument type</typeparam>
        /// <typeparam name="T15">The 15th function argument type</typeparam>
        /// <typeparam name="T16">The 16th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string path, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, Task> func)
            => AddDelegateRoute(path, func);


        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire(string path, Action func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1>(string path, Action<T1> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2>(string path, Action<T1, T2> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3>(string path, Action<T1, T2, T3> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4>(string path, Action<T1, T2, T3, T4> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5>(string path, Action<T1, T2, T3, T4, T5> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6>(string path, Action<T1, T2, T3, T4, T5, T6> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7>(string path, Action<T1, T2, T3, T4, T5, T6, T7> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8>(string path, Action<T1, T2, T3, T4, T5, T6, T7, T8> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string path, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string path, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string path, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string path, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <typeparam name="T13">The 13th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string path, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <typeparam name="T13">The 13th function argument type</typeparam>
        /// <typeparam name="T14">The 14th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string path, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <typeparam name="T13">The 13th function argument type</typeparam>
        /// <typeparam name="T14">The 14th function argument type</typeparam>
        /// <typeparam name="T15">The 15th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string path, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func)
            => AddDelegateRoute(path, func);
        /// <summary>
        /// Wires a path to the given function
        /// </summary>
        /// <param name="path">The path to wire</param>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="T1">The 1st function argument type</typeparam>
        /// <typeparam name="T2">The 2nd function argument type</typeparam>
        /// <typeparam name="T3">The 3rd function argument type</typeparam>
        /// <typeparam name="T4">The 4th function argument type</typeparam>
        /// <typeparam name="T5">The 5th function argument type</typeparam>
        /// <typeparam name="T6">The 6th function argument type</typeparam>
        /// <typeparam name="T7">The 7th function argument type</typeparam>
        /// <typeparam name="T8">The 8th function argument type</typeparam>
        /// <typeparam name="T9">The 9th function argument type</typeparam>
        /// <typeparam name="T10">The 10th function argument type</typeparam>
        /// <typeparam name="T11">The 11th function argument type</typeparam>
        /// <typeparam name="T12">The 12th function argument type</typeparam>
        /// <typeparam name="T13">The 13th function argument type</typeparam>
        /// <typeparam name="T14">The 14th function argument type</typeparam>
        /// <typeparam name="T15">The 15th function argument type</typeparam>
        /// <typeparam name="T16">The 16th function argument type</typeparam>
        /// <returns>The route instance</returns>        
        public ManualRoutingController Wire<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string path, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> func)
            => AddDelegateRoute(path, func);

        #endregion        
    }
}
