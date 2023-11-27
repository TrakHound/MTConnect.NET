using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ceen
{
    /// <summary>
    /// Helper class for providing a context for each run
    /// </summary>
    public static class LoaderContext
    {
        /// <summary>
        /// Interface for allowing the caller to freeze the scope
        /// </summary>
        public interface ILoaderContextInstance : IDisposable
        {
            /// <summary>
            /// Freezes the scope
            /// </summary>
            void Freeze();
        }

        /// <summary>
        /// Helper class to keep all instances
        /// </summary>
        private class ContextKeeper : ILoaderContextInstance
        {
            /// <summary>
            /// The instances created
            /// </summary>
            public Dictionary<Type, object> Instances = new Dictionary<Type, object>();

            /// <summary>
            /// The variable keeping track of the freeze state
            /// </summary>
            public bool Frozen;

            /// <summary>
            /// Freezes the scope
            /// </summary>
            public void Freeze() => Frozen = true;

            /// <summary>
            /// Disposes all references to allow the GC to clean up
            /// </summary>
            public void Dispose()
            {
                Instances = null;
            }
        }

        /// <summary>
        /// The loader scope, using AsyncLocal
        /// </summary>
        private static readonly System.Threading.AsyncLocal<ContextKeeper> m_activeContext 
            = new System.Threading.AsyncLocal<ContextKeeper>() { 
                Value = new ContextKeeper() 
            };

        /// <summary>
        /// Ensures that there is a single instance of the given type
        /// </summary>
        /// <typeparam name="T">The type of the item</typeparam>
        /// <returns>The item</returns>
        public static T EnsureSingletonInstance<T>()
            where T : new()
            => EnsureSingletonInstance(() => new T());

        /// <summary>
        /// Ensures that there is a single instance of the given type
        /// </summary>
        /// <param name="creator">The method used to create the instance</param>
        /// <typeparam name="T">The type of the item</typeparam>
        /// <returns>The item</returns>
        public static T EnsureSingletonInstance<T>(Func<T> creator)
        {
            if (creator == null)
                throw new ArgumentNullException(nameof(creator));
            var c = m_activeContext.Value;
            if (c.Frozen)
                throw new ArgumentException("Cannot register instance after the context is frozen");
            if (c.Instances.TryGetValue(typeof(T), out var n))
                return (T)n;
            
            var inst = creator();
            if (inst == null)
                throw new ArgumentException($"Creator function did not return an instance for type {typeof(T)}");
            
            // Re-check, in case the creation of the item registers itself
            if (c.Instances.TryGetValue(typeof(T), out n))
                return (T)n;

            // Then add it
            c.Instances.Add(typeof(T), inst);
            return inst;
        }

        /// <summary>
        /// Registers the singleton, throws an exception if there is already an instance registered 
        /// </summary>
        /// <param name="item">The instance to register as the only instance in the loader scope</param>
        /// <typeparam name="T">The type of the item</typeparam>
        /// <returns>The item</returns>
        public static T RegisterSingletonInstance<T>(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var c = m_activeContext.Value;
            if (c.Frozen)
                throw new ArgumentException("Cannot register instance after the context is frozen");
            if (c.Instances.ContainsKey(typeof(T)))
                throw new Exception($"This scope already has an instance of type {typeof(T)}");
            c.Instances.Add(typeof(T), item);

            return item;
        }

        /// <summary>
        /// Gets the singleton registered for the type
        /// </summary>
        /// <typeparam name="T">The type of the item</typeparam>
        /// <returns>The singleton instance</returns>
        public static T SingletonInstance<T>()
        {
            var c = m_activeContext.Value;
            if (c.Instances.TryGetValue(typeof(T), out var n))
                return (T)n;

            throw new Exception($"This scope does not have an instance of {typeof(T)}");
        }

        /// <summary>
        /// Sets the current active context
        /// </summary>
        /// <param name="current">The context to set</param>
        public static ILoaderContextInstance StartContext()
            => m_activeContext.Value = new ContextKeeper();
    }

    /// <summary>
    /// Helper class for providing the current execution context via the call context
    /// </summary>
    public static class Context
    {
        /// <summary>
        /// The scope data, using AsyncLocal
        /// </summary>
        private static readonly System.Threading.AsyncLocal<IHttpContext> m_activeContext = new System.Threading.AsyncLocal<IHttpContext>();

        /// <summary>
        /// Gets the current active context
        /// </summary>
        public static IHttpContext Current => m_activeContext.Value;

        /// <summary>
        /// Sets the current active context
        /// </summary>
        /// <param name="current">The context to set</param>
        public static void SetCurrentContext(IHttpContext current)
        {
            m_activeContext.Value = current;
        }

        /// <summary>
        /// Gets the current active request
        /// </summary>
        public static IHttpRequest Request => m_activeContext.Value?.Request;
        /// <summary>
        /// Gets the current active response
        /// </summary>
        public static IHttpResponse Response => m_activeContext.Value?.Response;

        /// <summary>
        /// Gets the current user ID
        /// </summary>
        public static string UserID => m_activeContext.Value?.Request.UserID;

        /// <summary>
        /// Gets the current active session, can be null if no session module is loaded
        /// </summary>
        public static IDictionary<string, string> Session => m_activeContext.Value?.Session;

        /// <summary>
        /// Gets the current request's log data
        /// </summary>
        public static IDictionary<string, string> LogData => m_activeContext.Value?.LogData;

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="level">The level to log</param>
        /// <param name="message">The message to log</param>
        /// <param name="ex">The exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task LogMessageAsync(LogLevel level, string message, Exception ex)
        {
            return Current.LogMessageAsync(level, message, ex);
        }

        /// <summary>
        /// Logs a debug message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="ex">The exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task LogDebugAsync(string message, Exception ex = null)
        {
            return Current.LogDebugAsync(message, ex);
        }

        /// <summary>
        /// Logs a debug message
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task LogDebugAsync(Exception ex)
        {
            return Current.LogDebugAsync(null, ex);
        }        

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="ex">The exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task LogErrorAsync(string message, Exception ex = null)
        {
            return Current.LogErrorAsync(message, ex);
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task LogErrorAsync(Exception ex)
        {
            return Current.LogErrorAsync(null, ex);
        }

        /// <summary>
        /// Logs an information message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="ex">The exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task LogInformationAsync(string message, Exception ex = null)
        {
            return Current.LogInformationAsync(message, ex);
        }

        /// <summary>
        /// Logs an information message
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task LogInformationAsync(Exception ex)
        {
            return Current.LogInformationAsync(null, ex);
        }

        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="ex">The exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task LogWarningAsync(string message, Exception ex = null)
        {
            return Current.LogWarningAsync(message, ex);
        }

        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task LogWarningAsync(Exception ex)
        {
            return Current.LogWarningAsync(null, ex);
        }


        /// <summary>
        /// Gets a named module of the given type
        /// </summary>
        /// <param name="self">The context instance</param>
        /// <param name="name">The name of the module to find</param>
        /// <param name="comparer">The string comparer</param>
        /// <typeparam name="T">The type of item to return</typeparam>
        /// <returns>The first match</returns>
        public static T GetNamedItem<T>(this IHttpContext self, string name, StringComparison comparer = StringComparison.OrdinalIgnoreCase)
            => GetNamedItem<T>(self.LoadedModules, name);

        /// <summary>
        /// Gets a named module of the given type
        /// </summary>
        /// <param name="self">The module info instance</param>
        /// <param name="name">The name of the module to find</param>
        /// <param name="comparer">The string comparer</param>
        /// <typeparam name="T">The type of item to return</typeparam>
        /// <returns>The first match</returns>
        public static T GetNamedItem<T>(this ILoadedModuleInfo self, string name, StringComparison comparer = StringComparison.OrdinalIgnoreCase)
            => GetItemsOfType<INamedModule>(self)
                .Where(x => string.Equals(x.Name, name, comparer))
                .OfType<T>()
                .FirstOrDefault();

        /// <summary>
        /// Gets all items assignable to a specific type
        /// </summary>
        /// <param name="self">The module info instance</param>
        /// <typeparam name="T">The type of items to return</typeparam>
        /// <returns>The items matchin the given type</returns>
        public static IEnumerable<T> GetItemsOfType<T>(this IHttpContext self)
            => GetItemsOfType<T>(self.LoadedModules);

        /// <summary>
        /// Gets all items assignable to a specific type
        /// </summary>
        /// <param name="self">The module info instance</param>
        /// <typeparam name="T">The type of items to return</typeparam>
        /// <returns>The items matchin the given type</returns>
        public static IEnumerable<T> GetItemsOfType<T>(this ILoadedModuleInfo self)
            => new T[0]
                .Concat(self?.Handlers?.Select(x => x.Value).OfType<T>() ?? new T[0])
                .Concat(self?.Loggers?.OfType<T>() ?? new T[0])
                .Concat(self?.Modules?.OfType<T>() ?? new T[0])
                .Concat(self?.PostProcessors?.OfType<T>() ?? new T[0]);

    }
}
