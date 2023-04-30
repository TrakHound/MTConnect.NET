using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.Globalization;
using System.Threading.Tasks;

namespace Ceen.Httpd
{
	/// <summary>
	/// Configuration of a server instance
	/// </summary>
	public class ServerConfig : ILoadedModuleInfo
	{
		/// <summary>
		/// The socket backlog.
		/// </summary>
		public int SocketBacklog { get; set; } = 5;
		/// <summary>
		/// The maximum size of the request line.
		/// </summary>
		public int MaxRequestLineSize { get; set; } = 8 * 1024;
		/// <summary>
		/// The maximum size of the request header.
		/// </summary>
		public int MaxRequestHeaderSize { get; set; } = 64 * 1024;
		/// <summary>
		/// The maximum number of active requests.
		/// </summary>
		public int MaxActiveRequests { get; set; } = 500000;
		/// <summary>
		/// The maximum number of internal redirects
		/// </summary>
		public int MaxInternalRedirects { get; set; } = 5;

		/// <summary>
		/// The maximum size of a POST request with url encoded data.
		/// This is also the maximum size allowed for automatically
		/// decoding multipart form data.
		/// </summary>
		public int MaxUrlEncodedFormSize { get; set; } = 5 * 1024 * 1024;

		/// <summary>
		/// Allow automatic parsing of multipart form data
		/// </summary>
		public bool AutoParseMultipartFormData { get; set; } = true;

		/// <summary>
		/// The maximum size of a POST request
		/// </summary>
		public long MaxPostSize { get; set; } = 100 * 1024 * 1024;

		/// <summary>
		/// A flag indicating if the X-HTTP-Method-Override header is supported
		/// </summary>
		public bool AllowHttpMethodOverride { get; set; } = true;

		/// <summary>
		/// A flag indicating if requests with HTTP/1.0 are allowed
		/// </summary>
		public bool AllowLegacyHttp { get; set; } = false;

		/// <summary>
		/// A value indicating the name of the header, 
		/// the proxy uses to communicate the source IP of the request.
		/// Commonly this is set to &quot;X-Real-IP&quot; or &quot;X-Forwarded-For&quot;
		/// Leave blank to disable.
		/// </summary>
		public string AllowedSourceIPHeaderValue { get; set; } = null;

		/// <summary>
		/// The request idle timeout in seconds.
		/// </summary>
		public int RequestIdleTimeoutSeconds { get; set; } = 5;
		/// <summary>
		/// The request header read timeout in seconds.
		/// </summary>
		public int RequestHeaderReadTimeoutSeconds { get; set; } = 10;
		/// <summary>
		/// The maximum number of requests to server with a single connection.
		/// </summary>
		public int KeepAliveMaxRequests { get; set; } = 30;
		/// <summary>
		/// The keep-alive timeout in seconds
		/// </summary>
		public int KeepAliveTimeoutSeconds { get; set; } = 10;
        /// <summary>
        /// The maximum number of seconds a process may be running
        /// </summary>
        /// <value>The max processing time seconds.</value>
        public int MaxProcessingTimeSeconds { get; set; } = 30;
		/// <summary>
		/// The router instance to use for handling requests
		/// </summary>
		public IRouter Router { get; set; }
		/// <summary>
		/// The logger instances to use
		/// </summary>
		public IList<ILogger> Loggers { get; set; }
		/// <summary>
		/// The loaded module instance
		/// </summary>
		public IList<IModule> Modules { get; set; }
        /// <summary>
        /// The loaded post-processor instances
        /// </summary>
        public IList<IPostProcessor> PostProcessors { get; set; }
        /// <summary>
        /// A callback method for injecting headers into the responses
        /// </summary>
        public Action<IHttpResponse> AddDefaultResponseHeaders { get; set; }

		/// <summary>
		/// Gets or sets the default name of the server reported in response headers.
		/// </summary>
		/// <value>The default name of the server.</value>
		public string DefaultServerName { get; set; }
			
		/// <summary>
		/// The server certificate if used for serving SSL requests
		/// </summary>
		public X509Certificate SSLCertificate { get; set; }
		/// <summary>
		/// True if a client SSL certificate should be requested
		/// </summary>
		public bool SSLRequireClientCert { get; set; } = false;
		/// <summary>
		/// List the allowed SSL versions
		/// </summary>
		public SslProtocols SSLEnabledProtocols { get; set; } = SslProtocols.Tls12;
		/// <summary>
		/// Value indicating if SSL certificates are checked against a revocation list
		/// </summary>
		public bool SSLCheckCertificateRevocation { get; set; } = true;

		/// <summary>
		/// A callback handler for debugging the internal server state
		/// </summary>
		public DebugLogDelegate DebugLogHandler;

		/// <summary>
		/// The storage creator
		/// </summary>
		public IStorageCreator Storage { get; set; }

		/// <summary>
		/// The loader context for this instance
		/// </summary>
		public IDisposable LoaderContext;
		
		/// <summary>
		/// Static initializer for the <see cref="T:Ceen.Httpd.ServerConfig"/> class.
		/// </summary>
		static ServerConfig()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Httpd.ServerConfig"/> class.
		/// </summary>
		public ServerConfig()
		{
			var version = typeof(ServerConfig).Assembly.GetName().Version;
			DefaultServerName = string.Format("ceenhttpd/{0}.{1}", version.Major, version.Minor);

			AddDefaultResponseHeaders = DefaultHeaders;
		}

		/// <summary>
		/// Loads a certificate instance
		/// </summary>
		/// <param name="path">The path to the file with the certificate.</param>
		/// <param name="password">The certificate password.</param>
		public void LoadCertificate(string path, string password)
		{
			this.SSLCertificate = new X509Certificate2(path, password ?? "");
		}

		/// <summary>
		/// Adds default headers to the output.
		/// </summary>
		/// <param name="response">The response to update.</param>
		public void DefaultHeaders(IHttpResponse response)
		{
			response.AddHeader("Date", DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture));

			if (!string.IsNullOrWhiteSpace(DefaultServerName))
				response.AddHeader("Server", DefaultServerName);
		}

		/// <summary>
		/// Adds a logger instance to the server
		/// </summary>
		/// <returns>The server configuration.</returns>
		/// <param name="logger">The logger module to add.</param>
		public ServerConfig AddLogger(LogDelegate logger)
		{
			return AddLogger(new Logging.FunctionLogger(logger));
		}

		/// <summary>
		/// Adds a logger instance to the server
		/// </summary>
		/// <returns>The server configuration.</returns>
		/// <param name="logger">The logger module to add.</param>
		public ServerConfig AddLogger(ILogger logger)
		{
			if (logger == null)
				throw new ArgumentNullException(nameof(logger));
			if (Loggers == null)
				Loggers = new List<ILogger>();

			Loggers.Add(logger);
			return this;
		}

		/// <summary>
		/// Adds a route to this configuration
		/// </summary>
		/// <returns>The server configuration.</returns>
		/// <param name="handler">The handler function that will execute the operation.</param>
		public ServerConfig AddRoute(HttpHandlerDelegate handler)
		{
			if (handler == null)
				throw new ArgumentNullException(nameof(handler));
			return AddRoute(null, new Handler.FunctionHandler(handler));
		}

		/// <summary>
		/// Adds a route to this configuration
		/// </summary>
		/// <returns>The server configuration.</returns>
		/// <param name="handler">The handler function that will execute the operation.</param>
		public ServerConfig AddRoute(IHttpModule handler)
		{
			if (handler == null)
				throw new ArgumentNullException(nameof(handler));
			return AddRoute(null, handler);
		}

		/// <summary>
		/// Adds a route to this configuration
		/// </summary>
		/// <returns>The server configuration.</returns>
		/// <param name="route">The expression used to pre-filter requests before invoking the handler.</param>
		/// <param name="handler">The handler function that will execute the operation.</param>
		public ServerConfig AddRoute(string route, HttpHandlerDelegate handler)
		{
			if (handler == null)
				throw new ArgumentNullException(nameof(handler));
			return AddRoute(route, new Handler.FunctionHandler(handler));
		}

		/// <summary>
		/// Adds a route to this configuration
		/// </summary>
		/// <returns>The server configuration.</returns>
		/// <param name="route">The expression used to pre-filter requests before invoking the handler.</param>
		/// <param name="handler">The handler module that will execute the operation.</param>
		public ServerConfig AddRoute(string route, IHttpModule handler)
		{
			if (handler == null)
				throw new ArgumentNullException(nameof(handler));

			Router rt;
			if (this.Router == null)
				this.Router = rt = new Router();
			else if (this.Router is Router)
				rt = this.Router as Router;
			else
				throw new Exception($"Cannot use the AddRoute method unless the {nameof(Router)} is an instance of {typeof(Router).FullName}");

			rt.Add(route, handler);
			return this;		
		}

		/// <summary>
		/// Adds a module instance to the server
		/// </summary>
		/// <returns>The server configuration.</returns>
		/// <param name="module">The module to add.</param>
		public ServerConfig AddModule(IModule module)
		{
			if (module == null)
				throw new ArgumentNullException(nameof(module));
			if (Modules == null)
				Modules = new List<IModule>();

			Modules.Add(module);
			return this;
		}

        /// <summary>
        /// Adds a post-processor instance to the server
        /// </summary>
        /// <returns>The server configuration.</returns>
        /// <param name="postprocessor">The post-processor to add.</param>
        public ServerConfig AddPostProcessor(IPostProcessor postprocessor)
        {
            if (postprocessor == null)
                throw new ArgumentNullException(nameof(postprocessor));
            if (PostProcessors == null)
                PostProcessors = new List<IPostProcessor>();

            PostProcessors.Add(postprocessor);
            return this;
        }

		/// <summary>
		/// Calls all shutdown modules
		/// </summary>
		/// <returns>A combined task</returns>
		public async Task ShutdownAsync()
		{
			var res = Ceen.Context
				.GetItemsOfType<IWithShutdown>(this)
				.Select(x => x.ShutdownAsync())
				.ToArray();

			if (res.Length != 0)
				await Task.WhenAll(res);
			
			if (LoaderContext != null)
				LoaderContext.Dispose();
		}


        /// <summary>
        /// The handlers loaded by the router
        /// </summary>
        IEnumerable<KeyValuePair<string, IHttpModule>> ILoadedModuleInfo.Handlers 
			=> (Router as Router)?.Rules.Select(x => new KeyValuePair<string, IHttpModule>(x.Key?.ToString(), x.Value));

        /// <summary>
        /// The logger instances
        /// </summary>
        IEnumerable<ILogger> ILoadedModuleInfo.Loggers 
			=> Loggers;

        /// <summary>
        /// The loaded modules
        /// </summary>
        IEnumerable<IModule> ILoadedModuleInfo.Modules 
			=> Modules;

        /// <summary>
        /// The loaded post-processors
        /// </summary>
        IEnumerable<IPostProcessor> ILoadedModuleInfo.PostProcessors 
			=> PostProcessors;
    }
}

