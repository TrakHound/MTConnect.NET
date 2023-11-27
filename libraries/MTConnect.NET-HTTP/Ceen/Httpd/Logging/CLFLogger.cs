using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;

namespace Ceen.Httpd.Logging
{
	/// <summary>
	/// Implementation of a logger for the Combined Log Format
	/// </summary>
	public class CLFLogger : ILogger, IDisposable
	{
		/// <summary>
		/// The stream to write to
		/// </summary>
		protected StreamWriter m_stream;
		/// <summary>
		/// The lock used to provide exclusive access to the stream
		/// </summary>
		protected readonly AsyncLock m_lock = new AsyncLock();
		/// <summary>
		/// <c>True</c>if the stream should be closed when the logger is disposed
		/// </summary>
		protected readonly bool m_closeOnDispose;
		/// <summary>
		/// <c>True</c> if the logging should be in combined format, <c>false</c> otherwise
		/// </summary>
		protected readonly bool m_useCombinedFormat;
		/// <summary>
		/// <c>True</c> if the logging should contain cookies in the combined format, <c>false</c> otherwise
		/// </summary>
		protected readonly bool m_logCookies;
		/// <summary>
		/// The log format string
		/// </summary>
		protected readonly string m_logFormatString;

		/// <summary>
		/// Cached instance of the timezone for use in the log output
		/// </summary>
		private static readonly string TIME_ZONE_SPECIFIER = new DateTime().ToString("zzz").Replace(":", "");


		/// <summary>
		/// Initializes a new instance of the <see cref="Ceen.Httpd.Logging.CLFLogger"/> class.
		/// </summary>
		/// <param name="filename">The file to write log data into.</param>
		/// <param name="useCombinedFormat"><c>True</c> if the logging should be in combined format, <c>false</c> otherwise.</param>
		/// <param name="logCookies"><c>True</c> if the logging should contain cookies in the combined format, <c>false</c> otherwise.</param>
		public CLFLogger(string filename, bool useCombinedFormat = true, bool logCookies = false)
			: this(File.Open(filename, FileMode.Append, FileAccess.Write, FileShare.Read), useCombinedFormat, logCookies, true)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Ceen.Httpd.Logging.CLFLogger"/> class.
		/// </summary>
		/// <param name="destination">The stream to write to.</param>
		/// <param name="useCombinedFormat"><c>True</c> if the logging should be in combined format, <c>false</c> otherwise.</param>
		/// <param name="logCookies"><c>True</c> if the logging should contain cookies in the combined format, <c>false</c> otherwise.</param>
		/// <param name="closeOnDispose"><c>True</c>if the stream should be closed when the logger is disposed.</param>
		public CLFLogger(Stream destination, bool useCombinedFormat = true, bool logCookies = false, bool closeOnDispose = false)
		{
			m_stream = new StreamWriter(destination);
			m_closeOnDispose = closeOnDispose;
			m_useCombinedFormat = useCombinedFormat;
			m_logCookies = logCookies;

			var logstr = m_useCombinedFormat ? "{0} {1} {2} {3} \"{4} {5} {6}\" {7} {8} \"{9}\" \"{10}\"" : "{0} {1} {2} {3} \"{4} {5} {6}\" {7} {8}";
			if (m_useCombinedFormat && m_logCookies)
				logstr += " \"{11}\"";

			m_logFormatString = logstr;

		}

		/// <summary>
		/// Gets the log line in the combined log format.
		/// </summary>
		/// <returns>The combined log line.</returns>
		/// <param name="context">The http context.</param>
		/// <param name="ex">The exception.</param>
		/// <param name="started">Timestamp for when the request started.</param>
		/// <param name="duration">Duration of the request processing.</param>
		public string GetCombinedLogLine(IHttpContext context, Exception ex, DateTime started, TimeSpan duration)
		{
			string remoteAddr;

			if (context.Request.RemoteEndPoint is System.Net.IPEndPoint)
				remoteAddr = ((System.Net.IPEndPoint)context.Request.RemoteEndPoint).Address.ToString();
			else
				remoteAddr = context.Request.RemoteEndPoint.ToString();

			string referer = null;
			string cookies = null;
			string useragent = null;

			if (m_useCombinedFormat)
			{
				referer = context.Request.Headers["Referer"];
				useragent = context.Request.Headers["User-Agent"];

				if (string.IsNullOrWhiteSpace(referer))
					referer = "-";
				if (string.IsNullOrWhiteSpace(useragent))
					useragent = "-";

				if (m_logCookies)
				{
					cookies = context.Request.Headers["Cookie"];
					if (string.IsNullOrWhiteSpace(cookies))
						cookies = "-";
				}

			}

			var statuscode = context.Response == null ? HttpStatusCode.InternalServerError : context.Response.StatusCode;
			var streamlength = context.Response == null ? -1 : context.Response.GetResponseStream().Length;

			return string.Format(
				m_logFormatString,
				remoteAddr,
				"-",
				"-",
				string.Format("[{0} {1}]", started.ToString("dd/MMM/yyyy:HH:mm:ss", CultureInfo.InvariantCulture), TIME_ZONE_SPECIFIER),
				context.Request.Method, context.Request.Path + context.Request.RawQueryString ?? string.Empty, context.Request.HttpVersion,
				(int)statuscode,
				streamlength,
				referer,
				useragent,
				cookies
			);
		}

		/// <summary>
		/// Dispose the specified isDisposing.
		/// </summary>
		/// <param name="isDisposing">If set to <c>true</c> is disposing.</param>
		protected virtual void Dispose(bool isDisposing)
		{
			if (m_closeOnDispose && m_stream != null)
				try {m_stream.Dispose(); }

			catch { }
			finally { m_stream = null; }
		}

		#region IDisposable implementation

		/// <summary>
		/// Releases all resource used by the <see cref="Ceen.Httpd.Logging.CLFLogger"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Ceen.Httpd.Logging.CLFLogger"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="Ceen.Httpd.Logging.CLFLogger"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="Ceen.Httpd.Logging.CLFLogger"/> so
		/// the garbage collector can reclaim the memory that the <see cref="Ceen.Httpd.Logging.CLFLogger"/> was occupying.</remarks>
		public void Dispose()
		{
			Dispose(true);
		}

		#endregion

		#region ILogger implementation

		/// <summary>
		/// Logs the request to the stream.
		/// </summary>
		/// <returns>The awaitable task.</returns>
		/// <param name="context">The http context.</param>
		/// <param name="ex">The exception.</param>
		/// <param name="started">Timestamp for when the request started.</param>
		/// <param name="duration">Duration of the request processing.</param>
		public virtual Task LogRequestCompletedAsync(IHttpContext context, Exception ex, DateTime started, TimeSpan duration)
		{
			return Task.Run(async () => {
				if (m_stream == null)
					throw new ObjectDisposedException(this.GetType().FullName);

				var logmsg = GetCombinedLogLine(context, ex, started, duration);

				using (await m_lock.LockAsync())
				{
					await m_stream.WriteLineAsync(logmsg);
					await m_stream.FlushAsync();
				}
			});
		}

		#endregion
	}
}

