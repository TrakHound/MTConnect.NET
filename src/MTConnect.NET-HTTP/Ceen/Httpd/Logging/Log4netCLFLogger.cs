using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ceen;
using Ceen.Httpd;
using Ceen.Httpd.Logging;

namespace Ceen.Httpd.Logging
{
	/// <summary>
	/// Implementation of a logger that writes messages in the combined log format to log4net
	/// </summary>
	public class Log4netCLFLogger : CLFLogger
	{
		/// <summary>
		/// The log instance
		/// </summary>
		private readonly object m_log;
		/// <summary>
		/// The info method for the logger
		/// </summary>
		private readonly System.Reflection.MethodInfo m_info;
		/// <summary>
		/// The error method for the logger
		/// </summary>
		private readonly System.Reflection.MethodInfo m_error;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Httpd.Logging.Log4netCLFLogger"/> class.
		/// </summary>
		public Log4netCLFLogger()
			: this(typeof(Log4netCLFLogger).FullName)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:DuplicatiHttpMirror.Log4netCLFLogger"/> class.
		/// </summary>
		/// <param name="logreporttype">The typename of the type to report the logger for</param>
		public Log4netCLFLogger(string logreporttype)
			: base(new MemoryStream(), true, false)
		{
			var targettype = 
				Type.GetType(logreporttype, false)
					??
				AppDomain.CurrentDomain.GetAssemblies()
					.Select(x => x.GetType(logreporttype))
					.FirstOrDefault(x => x != null);
			
			if (targettype == null)
				throw new Exception($"Failed to load target type: {logreporttype}");

			var type = Type.GetType("log4net.ILog, log4net, Culture=neutral");
			if (type == null)
			{
				m_log = null;
				m_info = null;
				m_error = null;
			}
			else
			{
				m_info = type.GetMethod("Info", new Type[] { typeof(object) });
				m_error = type.GetMethod("Error", new Type[] { typeof(object), typeof(Exception) });
				var logman = Type.GetType("log4net.LogManager, log4net, Culture=neutral");
				if (logman == null)
					m_log = null;
				else
				{
					var logmethod = logman.GetMethod("GetLogger", new Type[] { typeof(Type) });
					if (logmethod == null)
						m_log = null;
					else
						m_log = logmethod.Invoke(null, new object[] { targettype });
				}
			}

			if (m_log == null || m_info == null || m_error == null)
				throw new Exception("Unable to load Log4net assembly");
		}

		/// <summary>
		/// Logs the request to log4net.
		/// </summary>
		/// <returns>The awaitable task.</returns>
		/// <param name="context">The http context.</param>
		/// <param name="ex">The exception.</param>
		/// <param name="started">Timestamp for when the request started.</param>
		/// <param name="duration">Duration of the request processing.</param>
		public override Task LogRequestCompletedAsync(IHttpContext context, Exception ex, DateTime started, TimeSpan duration)
		{
			return Task.Run(() =>
			{
				var line = GetCombinedLogLine(context, ex, started, duration);
				if (ex == null)
					m_info.Invoke(m_log, new object[] { line });
				else
					m_error.Invoke(m_log, new object[] { line, ex });
			});
		}
	}
}
