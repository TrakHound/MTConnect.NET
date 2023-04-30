using System;
using System.Threading.Tasks;

namespace Ceen.Httpd.Logging
{
	/// <summary>
	/// Outputs Common Log Format to STDOUT
	/// </summary>
	public sealed class CLFStdOut : CLFLogger
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Httpd.Logging.CFLStdOut"/> class.
		/// </summary>
		public CLFStdOut()
			: base(Console.OpenStandardOutput())
		{
		}
	}

	/// <summary>
	/// Outputs Common Log Format to STDERR
	/// </summary>
	public sealed class CLFStdErr : CLFLogger
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Httpd.Logging.CFLStdErr"/> class.
		/// </summary>
		public CLFStdErr()
			: base(Console.OpenStandardError())
		{
		}
	}

	/// <summary>
	/// Logger that outputs exception messages to stdout
	/// </summary>
	public sealed class StdErrErrors : IMessageLogger
	{
		/// <summary>
		/// A static cached instance of the StdErr stream
		/// </summary>
		private static readonly System.IO.StreamWriter _stderr;

		/// <summary>
		/// Static initializer
		/// </summary>
		static StdErrErrors()
		{
			_stderr = new System.IO.StreamWriter(Console.OpenStandardError(), System.Text.Encoding.UTF8, 1024, true);
			_stderr.AutoFlush = true;
		}

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="ex">The exception being logged, may be null.</param>
        /// <param name="loglevel">The log level</param>
        /// <param name="message">The message to log</param>
        /// <param name="when">The time the log data was received</param>
        /// <returns>An awaitable task</returns>
        public Task LogMessageAsync(IHttpContext context, Exception ex, LogLevel loglevel, string message, DateTime when)
        {
            if (ex != null)
                _stderr.WriteLine(ex);
			return Task.FromResult(true);
        }

        /// <summary>
        /// Logs a completed request.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        /// <param name="context">The execution context.</param>
        /// <param name="ex">The exception being logged, may be null.</param>
        /// <param name="started">The time the request started.</param>
        /// <param name="duration">The request duration.</param>
        public Task LogRequestCompletedAsync(IHttpContext context, Exception ex, DateTime started, TimeSpan duration)
        {
            if (ex != null)
                _stderr.WriteLine(ex);
            return Task.FromResult(true);
        }
    }

	/// <summary>
	/// Logger that outputs exception messages to stdout
	/// </summary>
	public sealed class StdOutErrors : IMessageLogger
	{
        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="ex">The exception being logged, may be null.</param>
        /// <param name="loglevel">The log level</param>
        /// <param name="message">The message to log</param>
        /// <param name="when">The time the log data was received</param>
        /// <returns>An awaitable task</returns>
        public Task LogMessageAsync(IHttpContext context, Exception ex, LogLevel loglevel, string message, DateTime when)
        {
            if (ex != null)
                Console.WriteLine(ex);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Logs a completed request.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        /// <param name="context">The execution context.</param>
        /// <param name="ex">The exception being logged, may be null.</param>
        /// <param name="started">The time the request started.</param>
        /// <param name="duration">The request duration.</param>
        public Task LogRequestCompletedAsync(IHttpContext context, Exception ex, DateTime started, TimeSpan duration)
        {
            if (ex != null)
                Console.WriteLine(ex);
            return Task.FromResult(true);
        }
    }
}
