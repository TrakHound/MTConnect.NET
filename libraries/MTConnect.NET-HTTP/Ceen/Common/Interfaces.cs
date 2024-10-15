using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.Threading;
using MTConnect.Http;

namespace Ceen
{
    /// <summary>
    /// Interface for a multipart-item, from a multi-part form request
    /// </summary>
    internal interface IMultipartItem
	{
		/// <summary>
		/// The headers associated with the item
		/// </summary>
		/// <value>The headers.</value>
		IDictionary<string, string> Headers { get; }

		/// <summary>
		/// Gets or sets the form name.
		/// </summary>
		/// <value>The name.</value>
		string Name { get; }
		/// <summary>
		/// Gets or sets the filename.
		/// </summary>
		/// <value>The filename.</value>
		string Filename { get; }
		/// <summary>
		/// Gets the Content-Type header value.
		/// </summary>
		/// <value>The type of the content.</value>
		string ContentType { get; }
		/// <summary>
		/// The data for this entry
		/// </summary>
		/// <value>The data.</value>
		Stream Data { get; }
	}

	/// <summary>
	/// An interface describing a response cookie
	/// </summary>
	public interface IResponseCookie
	{
		/// <summary>
		/// List of settings attached to the cookie
		/// </summary>
		/// <value>The settings.</value>
		IDictionary<string, string> Settings { get; }

		/// <summary>
		/// The name of the cookie
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// The value of the cookie
		/// </summary>
		string Value { get; set; }

		/// <summary>
		/// Gets or sets the cookie path
		/// </summary>
		string Path { get; set; }

		/// <summary>
		/// Gets or sets the cookie domain
		/// </summary>
		string Domain { get; set; }

		/// <summary>
		/// Gets or sets the cookie expiration date
		/// </summary>
		DateTime? Expires { get; set; }

		/// <summary>
		/// Gets or sets the cookie max age.
		/// Zero or negative values means un-set
		/// </summary>
		long MaxAge { get; set; }

		/// <summary>
		/// Gets or sets the cookie secure flag
		/// </summary>
		bool Secure { get; set; }

		/// <summary>
		/// Gets or sets the cookie HttpOnly flag
		/// </summary>
		bool HttpOnly { get; set; }
	}

    /// <summary>
    /// Interface for a http request.
    /// </summary>
    internal interface IHttpRequestInternal : IHttpRequest
    {
		/// <summary>
		/// Gets the posted files, if any.
		/// Duplicate values are not represented, instead only the latest is stored
		/// </summary>
		IList<IMultipartItem> Files { get; }
	
		/// <summary>
		/// Gets the handlers that have processed this request
		/// </summary>
		IEnumerable<IHttpModule> HandlerStack { get; }

		/// <summary>
		/// Registers a handler on the request stack
		/// </summary>
		void PushHandlerOnStack(IHttpModule handler);

		/// <summary>
		/// Enforces that the handler stack obeys the requirements
		/// </summary>
		/// <param name="attributes">The list of attributes to check.</param>
		void RequireHandler(IEnumerable<RequireHandlerAttribute> attributes);

		/// <summary>
		/// Enforces that the given type has processed the request
		/// </summary>
		/// <param name="handler">The type to check for.</param>
		/// <param name="allowderived">A flag indicating if the type match must be exact, or if derived types are accepted</param>
		void RequireHandler(Type handler, bool allowderived = true);

		/// <summary>
		/// Gets the request cancellation token that is triggered if the request times out
		/// </summary>
		CancellationToken TimeoutCancellationToken { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Ceen.IHttpRequest"/> is connected.
        /// </summary>
        /// <value><c>true</c> if is connected; otherwise, <c>false</c>.</value>
        bool IsConnected { get; }

        /// <summary>
        /// Gets the time the request processing started
        /// </summary>
        DateTime RequestProcessingStarted { get; }

        /// <summary>
        /// Resets the processing timeout.
        /// </summary>
        void ResetProcessingTimeout();

        /// <summary>
        /// Helper method that throws a timeout exception if the processing time has been exceeded
        /// </summary>
        void ThrowIfTimeout();
    }

    /// <summary>
    /// Interface for a http response.
    /// </summary>
    internal interface IHttpResponse
	{
		/// <summary>
		/// Gets or sets the HTTP version to report.
		/// </summary>
		/// <value>The http version.</value>
		string HttpVersion { get; set; }
		/// <summary>
		/// Gets or sets the status code to report.
		/// </summary>
		/// <value>The status code.</value>
		HttpStatusCode StatusCode { get; set; }
		/// <summary>
		/// Gets or sets the status message to report.
		/// If this is <c>null</c>, the default message for
		/// the HTTP status code is used
		/// </summary>
		/// <value>The status message.</value>
		string StatusMessage { get; set; }
		/// <summary>
		/// Gets a value indicating whether the sent headers are sent to the client.
		/// Once the headers are sent, the header collection can no longer be modified
		/// </summary>
		/// <value><c>true</c> if this instance has sent headers; otherwise, <c>false</c>.</value>
		bool HasSentHeaders { get; }
		/// <summary>
		/// Dictionary with headers that are sent as part of the response.
		/// Cannot be modified after the headers have been sent.
		/// </summary>
		/// <value>The headers.</value>
		IDictionary<string, string> Headers { get; }

		/// <summary>
		/// Gets a list of cookies that are set with the response.
		/// Cannot be modified after the headers have been sent.
		/// </summary>
		/// <value>The cookies.</value>
		IList<IResponseCookie> Cookies { get; }

		/// <summary>
		/// Adds a cookie to the output
		/// </summary>
		/// <returns>The new cookie.</returns>
		/// <param name="name">The name of the cookie.</param>
		/// <param name="value">The cookie value.</param>
		/// <param name="path">The optional path limiter.</param>
		/// <param name="domain">The optional domain limiter.</param>
		/// <param name="expires">The optional expiration date.</param>
		/// <param name="maxage">The optional maximum age.</param>
		/// <param name="secure">A flag for making the cookie available over SSL only.</param>
		/// <param name="httponly">A flag indicating if the cookie should be hidden from the scripting environment.</param>
		/// <param name="samesite">The samesite attribute for the cookie</param>
		IResponseCookie AddCookie(string name, string value, string path = null, string domain = null, DateTime? expires = null, long maxage = -1, bool secure = false, bool httponly = false, string samesite = null);

		/// <summary>
		/// Adds a header to the output, use null to delete a header.
		/// This method throws an exception if the headers are already sent
		/// </summary>
		/// <param name="key">The header name.</param>
		/// <param name="value">The header value.</param>
		void AddHeader(string key, string value);

		/// <summary>
		/// Performs an internal redirect
		/// </summary>
		/// <param name="path">The new path to use.</param>
		void InternalRedirect(string path);

		/// <summary>
		/// Gets a value indicating if an internal redirect has been requested
		/// </summary>
		bool IsRedirectingInternally { get; }

		/// <summary>
		/// Gets or sets the Content-Type header
		/// </summary>
		/// <value>The type of the content.</value>
		string ContentType { get; set; }

		/// <summary>
		/// Gets or sets the Content-Length header
		/// </summary>
		/// <value>The length of the content.</value>
		long ContentLength { get; set; }

        /// <summary>
        /// Gets or sets the Keep-Alive header
        /// </summary>
        /// <value><c>true</c> if keep alive; otherwise, <c>false</c>.</value>
        bool KeepAlive { get; set; }

		/// <summary>
		/// Flush all headers async.
		/// This method can be called multiple times if desired.
		/// </summary>
		/// <returns>The headers async.</returns>
		Task FlushHeadersAsync();

		/// <summary>
		/// Copies the stream to the output. Note that the stream is copied from the current position to the end, and the stream must report the length.
		/// </summary>
		/// <returns>The awaitable task</returns>
		/// <param name="data">The stream to copy.</param>
		/// <param name="contenttype">An optional content type to set. Throws an exception if the headers are already sent.</param>
		Task WriteAllAsync(Stream data, string contenttype = null);

		/// <summary>
		/// Writes the byte array to the output.
		/// </summary>
		/// <returns>The awaitable task</returns>
		/// <param name="data">The data to write.</param>
		/// <param name="contenttype">An optional content type to set. Throws an exception if the headers are already sent.</param>
		Task WriteAllAsync(byte[] data, string contenttype = null);

		/// <summary>
		/// Writes the string to the output using UTF-8 encoding.
		/// </summary>
		/// <returns>The awaitable task</returns>
		/// <param name="data">The data to write.</param>
		/// <param name="contenttype">An optional content type to set. Throws an exception if the headers are already sent.</param>
		Task WriteAllAsync(string data, string contenttype = null);

		/// <summary>
		/// Writes the string to the output.
		/// </summary>
		/// <returns>The awaitable task</returns>
		/// <param name="data">The data to write.</param>
		/// <param name="encoding">The encoding to apply.</param>
		/// <param name="contenttype">An optional content type to set. Throws an exception if the headers are already sent.</param>
		Task WriteAllAsync(string data, Encoding encoding, string contenttype = null);

		/// <summary>
		/// Writes the json string to the output with UTF-8 encoding.
		/// </summary>
		/// <returns>The awaitable task</returns>
		/// <param name="data">The JSON data to write.</param>
		Task WriteAllJsonAsync(string data);

		/// <summary>
		/// Performs a 302 redirect
		/// </summary>
		/// <param name="newurl">The target url.</param>
		void Redirect(string newurl);

		/// <summary>
		/// Sets headers that instruct the client and proxies to avoid caching the response
		/// </summary>
		void SetNonCacheable();

        /// <summary>
        /// Sets headers that instruct the client and proxies to allow caching for a limited time
        /// </summary>
        /// <param name="duration">The time the client is allowed to cache the response</param>
        /// <param name="isPublic">A flag indicating if the response is public and can be cached by proxies</param>
        void SetExpires(TimeSpan duration, bool isPublic = true);

        /// <summary>
        /// Sets headers that instruct the client and proxies to allow caching for a limited time
        /// </summary>
        /// <param name="until">The time the client is no longer allowed to use the cached response</param>
        /// <param name="isPublic">A flag indicating if the response is public and can be cached by proxies</param>
        void SetExpires(DateTime until, bool isPublic = true);

		/// <summary>
		/// Gets the response stream.
		/// To avoid buffering the contents, make sure the
		/// Content-Length header is set before writing to the stream
		/// </summary>
		/// <returns>The response stream.</returns>
		Stream GetResponseStream();
	}

    /// <summary>
    /// Interface for a http handler
    /// </summary>
    internal interface IHttpContext
	{
		/// <summary>
		/// Gets the request.
		/// </summary>
		IHttpRequestInternal Request { get; }

		/// <summary>
		/// Gets the response
		/// </summary>
		IHttpResponse Response { get; }

		/// <summary>
		/// Gets the storage creator
		/// </summary>
		IStorageCreator Storage { get; }
	
		/// <summary>
		/// Gets or sets the session storage.
		/// Note that this can be null if there is no session module loaded.
		/// </summary>
		IDictionary<string, string> Session { get; set; }

		/// <summary>
		/// Additional data that can be used in a logging module to tag the request or response
		/// </summary>
		IDictionary<string, string> LogData { get; }

		/// <summary>
		/// Information about loaded modules
		/// </summary>
		ILoadedModuleInfo LoadedModules { get; }

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="level">The level to log</param>
        /// <param name="message">The message to log</param>
        /// <param name="ex">The exception to log</param>
        /// <returns>An awaitable task</returns>
        Task LogMessageAsync(LogLevel level, string message, Exception ex);
	}

    /// <summary>
    /// Interface for querying the current running server about the loaded modules
    /// </summary>
    internal interface ILoadedModuleInfo
	{
        /// <summary>
        /// The handlers loaded by the router
        /// </summary>
        IEnumerable<KeyValuePair<string, IHttpModule>> Handlers { get; }
        /// <summary>
        /// The logger instances
        /// </summary>
        IEnumerable<ILogger> Loggers { get; }
        /// <summary>
        /// The loaded modules
        /// </summary>
        IEnumerable<IModule> Modules { get; }
        /// <summary>
        /// The loaded post-processors
        /// </summary>
        IEnumerable<IPostProcessor> PostProcessors { get; }
    }

    /// <summary>
    /// Interface for giving a module a unique name,
    /// allowing for easier access through the <see name="ILoadedModuleInfo" />
    /// </summary>
    internal interface INamedModule
	{
		/// <summary>
		/// The name of the module
		/// </summary>
		string Name { get; }
	}

    /// <summary>
    /// Interface for implementing a routing provider
    /// </summary>
    internal interface IRouter
    {
        /// <summary>
        /// Process the request for the specified context.
        /// </summary>
        /// <param name="context">The context to use.</param>
        /// <param name="cancellationToken">The token indicating to stop handling.</param>
        /// <returns>A value indicating if the request is now processed</returns>
        Task<bool> Process(IHttpContext context, CancellationToken cancellationToken);
	}

    /// <summary>
    /// Shared interface for marking a module or logger as needing setup
    /// </summary>
    internal interface IWithSetup
	{
        /// <summary>
        /// Method called after module is configured
        /// </summary>
        void AfterConfigure();
    }

    /// <summary>
    /// Shared interface for marking a module or logger as needing to shutdown
    /// </summary>
    internal interface IWithShutdown
    {
        /// <summary>
        /// Method called when module needs to shut down
        /// </summary>
		/// <returns>An awaitable task</returns>
        Task ShutdownAsync();
    }

    /// <summary>
    /// Basic interface for a request handler
    /// </summary>
    internal interface IHttpModule
	{
		/// <summary>
		/// Process the request for the specified context.
		/// </summary>
		/// <param name="context">The context to use.</param>
		/// <param name="cancellationToken">The token indicating to stop handling.</param>
		/// <returns>A value indicating if the request is now processed</returns>
		Task<bool> HandleAsync(IHttpContext context, CancellationToken cancellationToken);
	}

    /// <summary>
    /// A module based on <see cref="IHttpModule"/> which is notified after being configured.
    /// Items that use this interface can do one-time setups in this call.
    /// </summary>
    internal interface IHttpModuleWithSetup : IHttpModule, IWithSetup
    {
    }

    /// <summary>
    /// Interface for implementing a storage creator
    /// </summary>
    internal interface IStorageCreator
	{
		/// <summary>
		/// Gets or creates a storage module with the given name
		/// </summary>
		/// <returns>The storage module or null.</returns>
		/// <param name="name">The name of the module to get.</param>
		/// <param name="key">The session key of the module, or null.</param>
		/// <param name="ttl">The module time-to-live, zero or less means no expiration.</param>
		/// <param name="autocreate">Automatically create storage if not found</param>
		Task<IStorageEntry> GetStorageAsync(string name, string key, int ttl, bool autocreate);
	}

    /// <summary>
    /// Interface for storing data
    /// </summary>
    internal interface IStorageEntry : IDictionary<string, string>
	{
		/// <summary>
		/// Gets the name of the storage element
		/// </summary>
		string Name { get; }
		/// <summary>
		/// Gets or sets the time the dictionary expires
		/// </summary>
		DateTime Expires { get; set; }
	}

    /// <summary>
    /// The log levels
    /// </summary>
    internal enum LogLevel
	{
		/// <summary>The message is a debug message</summary>
		Debug,
		/// <summary>The message is an informational message</summary>
		Information,
		/// <summary>The message is a warning message</summary>
		Warning,
		/// <summary>The message is an error message</summary>
		Error
	}

    /// <summary>
    /// Interface for implementing a logging provider
    /// </summary>
    internal interface ILogger
	{
		/// <summary>
		/// Logs a completed request.
		/// </summary>
		/// <returns>An awaitable task.</returns>
		/// <param name="context">The execution context.</param>
		/// <param name="ex">The exception being logged, may be null.</param>
		/// <param name="started">The time the request started.</param>
		/// <param name="duration">The request duration.</param>
		Task LogRequestCompletedAsync(IHttpContext context, Exception ex, DateTime started, TimeSpan duration);
	}

    /// <summary>
    /// Interface for a logger that also accepts messages during requests
    /// </summary>
    internal interface IMessageLogger : ILogger
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
        Task LogMessageAsync(IHttpContext context, Exception ex, LogLevel loglevel, string message, DateTime when);
	}


    /// <summary>
    /// Interface for logging requests before they are processed
    /// </summary>
    internal interface IStartLogger : ILogger
	{
		/// <summary>
		/// Logs the start of a request.
		/// </summary>
		/// <returns>An awaitable task.</returns>
		/// <param name="request">The request being started.</param>
		Task LogRequestStartedAsync(IHttpRequestInternal request);
	}

    /// <summary>
    /// Extensions to a logger module that requires configuration
    /// </summary>
    internal interface ILoggerWithSetup : ILogger, IWithSetup
	{
    }

    /// <summary>
    /// Marker interface for a generic module
    /// </summary>
    internal interface IModule
	{
	}

    /// <summary>
    /// Extension to a generic module that requires configuration
    /// </summary>
    internal interface IModuleWithSetup : IModule, IWithSetup
    {
    }

    /// <summary>
    /// Basic interface for a post-processing handler
    /// </summary>
    internal interface IPostProcessor
    {
        /// <summary>
        /// Process the request for the specified context.
        /// </summary>
        /// <param name="context">The context to use.</param>
        /// <returns>An awaitable task</returns>
        Task HandleAsync(IHttpContext context);
    }

    /// <summary>
    /// Extension for a post-processing handler that requires configuration
    /// </summary>
    internal interface IPostProcessorWithSetup : IPostProcessor, IWithSetup
	{
	}

}
