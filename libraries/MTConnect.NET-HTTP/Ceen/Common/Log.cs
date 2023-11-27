using System;
using System.Threading.Tasks;

namespace Ceen
{
    /// <summary>
    /// Utility class for simple logging
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Logs an exception error
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task ErrorAsync(Exception ex)
        {
            return Context.Current?.LogMessageAsync(LogLevel.Error, null, ex) ?? Task.FromResult(true);
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="ex">The optional exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task ErrorAsync(string message, Exception ex = null)
        {
            return Context.Current?.LogMessageAsync(LogLevel.Error, message, ex) ?? Task.FromResult(true);
        }

        /// <summary>
        /// Logs an exception warning
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task WarningAsync(Exception ex)
        {
            return Context.Current?.LogMessageAsync(LogLevel.Warning, null, ex) ?? Task.FromResult(true);
        }

        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="ex">The optional exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task WarningAsync(string message, Exception ex = null)
        {
            return Context.Current?.LogMessageAsync(LogLevel.Warning, message, ex) ?? Task.FromResult(true);
        }

        /// <summary>
        /// Logs an exception for information use
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task InformationAsync(Exception ex)
        {
            return Context.Current?.LogMessageAsync(LogLevel.Information, null, ex) ?? Task.FromResult(true);
        }

        /// <summary>
        /// Logs an informaiton message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="ex">The optional exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task InformationAsync(string message, Exception ex = null)
        {
            return Context.Current?.LogMessageAsync(LogLevel.Information, message, ex) ?? Task.FromResult(true);
        }

        /// <summary>
        /// Logs an exception for debugging
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task DebugAsync(Exception ex)
        {
            return Context.Current?.LogMessageAsync(LogLevel.Debug, null, ex) ?? Task.FromResult(true);
        }

        /// <summary>
        /// Logs a debug message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="ex">The optional exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task DebugAsync(string message, Exception ex = null)
        {
            return Context.Current?.LogMessageAsync(LogLevel.Debug, message, ex) ?? Task.FromResult(true);
        }        
    }
}
