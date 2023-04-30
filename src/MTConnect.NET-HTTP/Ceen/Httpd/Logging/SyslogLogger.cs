using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Ceen.Httpd.Logging
{
	/// <summary>
	/// Implementation of a syslogger, using SyslogNet.Client by reflection
	/// </summary>
	public class SyslogLogger : CLFLogger
	{
		/// <summary>
		/// The serializer instance
		/// </summary>
		private readonly object m_serializer;
		/// <summary>
		/// The sender instance
		/// </summary>
		private readonly object m_sender;
		/// <summary>
		/// The message type used to construct messages
		/// </summary>
		private readonly Type m_message_t;
		/// <summary>
		/// The send method to invoke
		/// </summary>
		private readonly MethodInfo m_sendMethod;
		/// <summary>
		/// The logging facility to use
		/// </summary>
		private readonly object m_facility;
		/// <summary>
		/// The logging severity level
		/// </summary>
		private readonly object m_severity;
		/// <summary>
		/// The name to use as the log source
		/// </summary>
		private readonly string m_logsource;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Httpd.Logging.SyslogLogger"/> class.
		/// </summary>
		public SyslogLogger()
			: this("SystemDaemons", "Informational", "ceenhttpd")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Httpd.Logging.SyslogLogger"/> class.
		/// </summary>
		/// <param name="facility">The log facility to use.</param>
		/// <param name="severity">The log severity to use.</param>
		/// <param name="logsource">The log source name to use.</param>
		public SyslogLogger(string facility, string severity, string logsource)
			: base(new MemoryStream(), true, false)
		{
			// Don't use the base stream
			m_stream.Dispose();
			m_stream = null;

			// Fire up the reflection
			var serializer_t = Type.GetType("SyslogNet.Client.Serialization.SyslogLocalMessageSerializer, SyslogNet.Client");
			var sender_t = Type.GetType("SyslogNet.Client.Transport.SyslogLocalSender, SyslogNet.Client");
			m_message_t = Type.GetType("SyslogNet.Client.SyslogMessage, SyslogNet.Client");
			var iserializer_t = Type.GetType("SyslogNet.Client.Serialization.ISyslogMessageSerializer, SyslogNet.Client");

			var facility_t = Type.GetType("SyslogNet.Client.Facility, SyslogNet.Client");
			var severity_t = Type.GetType("SyslogNet.Client.Severity, SyslogNet.Client");

			if (new[] { serializer_t, sender_t, m_message_t, iserializer_t, facility_t, severity_t }.Any(x => x == null))
				throw new Exception("Unable to load SyslogNet.Client assembly, please add a reference to SyslogNet.Client version 0.3.2 or newer");

			m_sendMethod = sender_t.GetMethod("Send", new Type[] { m_message_t, iserializer_t });
			m_serializer = Activator.CreateInstance(serializer_t);
			m_sender = Activator.CreateInstance(sender_t);

			if (new object[] { m_sendMethod, m_serializer, m_sender }.Any(x => x == null))
				throw new Exception("Failed to set up syslog interface, maybe the SyslogNet.Client library has changed since 0.3.2?");

			m_facility = Enum.Parse(facility_t, facility, true);
			m_severity = Enum.Parse(severity_t, severity, true);
			m_logsource = logsource;
		}

		/// <summary>
		/// Logs the request to syslog.
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
				var msg = Activator.CreateInstance(m_message_t, new object[]
				{
						m_facility,
						m_severity,
						m_logsource,
						GetCombinedLogLine(context, ex, started, duration)
				});

				m_sendMethod.Invoke(m_sender, new object[] { msg, m_serializer });
			});
		}
	}
}
