using System;
using System.Threading.Tasks;

namespace Ceen.Httpd
{
    /// <summary>
    /// A delegate for handling a log event. Note that the response and exception values may be <c>null</c>.
    /// </summary>
    internal delegate Task LogDelegate(IHttpContext context, Exception exception, DateTime started, TimeSpan duration);

    /// <summary>
    /// A delegate for handling a debug log event. Note that the response and exception values may be <c>null</c>.
    /// </summary>
    internal delegate void DebugLogDelegate(string message, string logtaskid, object data);

    /// <summary>
    /// A delegate for handling a HTTP request
    /// </summary>
    internal delegate Task<bool> HttpHandlerDelegate(IHttpContext context);
}
