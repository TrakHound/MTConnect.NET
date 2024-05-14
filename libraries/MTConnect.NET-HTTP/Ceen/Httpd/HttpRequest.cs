using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.IO;
using System.Linq;
using System.Security.Authentication;

namespace Ceen.Httpd
{
    /// <summary>
    /// Representation of the values in a HTTP request
    /// </summary>
    internal class HttpRequest : IHttpRequestInternal, IDisposable
    {
        /// <summary>
        /// The string indicating HTTP version 1.1
        /// </summary>
        public const string HTTP_VERSION_1_1 = "HTTP/1.1";
        /// <summary>
        /// The string indicating HTTP version 1.0
        /// </summary>
        public const string HTTP_VERSION_1_0 = "HTTP/1.0";

        /// <summary>
        /// Gets the HTTP Request line as sent by the client
        /// </summary>
        public string RawHttpRequestLine { get; private set; }
        /// <summary>
        /// The HTTP method or Verb
        /// </summary>
        public string Method { get; private set; }
        /// <summary>
        /// The path of the query, not including the query string
        /// </summary>
        public string Path { get; internal set; }
        /// <summary>
        /// The original path of the request, before internal path rewriting
        /// </summary>
        public string OriginalPath { get; internal set; }
        /// <summary>
        /// The query string
        /// </summary>
        public string RawQueryString { get; private set; }
        /// <summary>
        /// Gets a parsed representation of the query string.
        /// Duplicate values are not represented, instead only the latest is stored
        /// </summary>
        public IDictionary<string, string> QueryString { get; private set; }
        /// <summary>
        /// Gets the headers found in the request.
        /// Duplicate values are not represented, instead only the latest is stored
        /// </summary>
        public IDictionary<string, string> Headers { get; private set; }
        /// <summary>
        /// Gets the form data, if any.
        /// Duplicate values are not represented, instead only the latest is stored
        /// </summary>
        public IDictionary<string, string> Form { get; private set; }
        /// <summary>
        /// Gets the cookies supplied, if any.
        /// Duplicate values are not represented, instead only the latest is stored
        /// </summary>
        public IDictionary<string, string> Cookies { get; private set; }
        /// <summary>
        /// Gets the posted files, if any.
        /// Duplicate values are not represented, instead only the latest is stored
        /// </summary>
        public IList<IMultipartItem> Files { get; private set; }
        /// <summary>
        /// Gets the headers found in the request.
        /// Duplicate values are not represented, instead only the latest is stored
        /// </summary>
        public IDictionary<string, object> RequestState { get; private set; }
        /// <summary>
        /// Gets the http version string.
        /// </summary>
        public string HttpVersion { get; private set; }
		/// <summary>
		/// Gets or sets a user identifier attached to the request.
		/// This can be set by handlers processing the request to simplify dealing with logged in users.
		/// Handlers should only set this is the user is authenticated.
		/// This value can be logged.
		/// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// Gets or sets a session tracking ID.
        /// This value can be logged and used to group requests from a single session
        /// better than simply grouping by IP address
        /// </summary>
        public string SessionID { get; set; }
        /// <summary>
        /// Gets a value indicating what connection security is used.
        /// </summary>
        public SslProtocols SslProtocol { get; private set; }
        /// <summary>
        /// Gets the remote endpoint
        /// </summary>
        public System.Net.EndPoint RemoteEndPoint { get; private set; }
        /// <summary>
        /// Gets the client SSL certificate, if any
        /// </summary>
        public X509Certificate ClientCertificate { get; private set; }
        /// <summary>
        /// The taskid used for logging and tracing the request
        /// </summary>
        public string LogConnectionID { get; private set; }
        /// <summary>
        /// The taskid used for logging and tracing the request
        /// </summary>
        public string LogRequestID { get; private set; }

        /// <summary>
        /// The stream representing the body of the request
        /// </summary>
        public Stream Body { get; private set; }

        /// <summary>
        /// The cancellation token source
        /// </summary>
        private readonly CancellationTokenSource m_cancelRequest;

        /// <summary>
        /// Gets the request cancellation token that is triggered if the request times out
        /// </summary>
        public CancellationToken TimeoutCancellationToken { get { return m_cancelRequest.Token; } }

        /// <summary>
        /// Helper method that throws a timeout exception if the processing time has been exceeded
        /// </summary>
        public void ThrowIfTimeout()
        {
            if (TimeoutCancellationToken.IsCancellationRequested)
                throw new HttpException(HttpStatusCode.RequestTimeout);
        }


        /// <summary>
        /// The method to call to obtain the remote client connected state
        /// </summary>
        private readonly Func<bool> m_connectedMethod;

        /// <summary>
        /// The processing timeout duration
        /// </summary>
        private TimeSpan m_processingtimeout;

        /// <summary>
        /// The processing timeout ID
        /// </summary>
        private CancellationTokenSource m_processingtimeoutcancellation = new CancellationTokenSource();

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Ceen.IHttpRequest"/> is connected.
        /// </summary>
        /// <value><c>true</c> if is connected; otherwise, <c>false</c>.</value>
        public bool IsConnected
        {
            get
            {
                if (m_connectedMethod != null && !m_connectedMethod())
                    return false;

                return true;
            }
        }

        /// <summary>
		/// Gets the time the request processing started
		/// </summary>
        public DateTime RequestProcessingStarted { get; private set; }

        /// <summary>
        /// Gets the HTTP Content-Type header value
        /// </summary>
        /// <value>The type of the content.</value>
        public string ContentType
        {
            get
            {
                return Headers["Content-Type"];
            }
        }

		/// <summary>
		/// Gets the HTTP request hostname, can be null for a HTTP/1.0 request
		/// </summary>
        public string Hostname
        {
            get
            {
                return Headers["Host"];
            }
        }        

        /// <summary>
        /// Gets the HTTP Content-Length header value
        /// </summary>
        /// <value>The length of the content.</value>
        public int ContentLength
        {
            get
            {
                int contentlength;
                if (!int.TryParse(Headers["Content-Length"], out contentlength))
                    contentlength = -1;

                return contentlength;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ceen.Httpd.HttpRequest"/> class.
        /// </summary>
        /// <param name="remoteEndpoint">The remote endpoint.</param>
        /// <param name="logconnectionid">The logging ID for the task</param>
        /// <param name="clientCert">The client SSL certificate.</param>
        /// <param name="sslProtocol">The SSL protocol used</param>
        /// <param name="logrequestid">The ID of the request for logging purposes</param>
        /// <param name="connected">The method providing the remote client connected state</param>
        public HttpRequest(System.Net.EndPoint remoteEndpoint, string logconnectionid, string logrequestid, X509Certificate clientCert, SslProtocols sslProtocol, Func<bool> connected)
        {
            m_cancelRequest = new CancellationTokenSource();
            m_connectedMethod = connected;
            RemoteEndPoint = remoteEndpoint;
            ClientCertificate = clientCert;
            SslProtocol = sslProtocol;
            LogConnectionID = logconnectionid;
            LogRequestID = logrequestid;
            Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase).WithDefaultValue(null);
            QueryString = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase).WithDefaultValue(null);
            Form = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase).WithDefaultValue(null);
            Cookies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase).WithDefaultValue(null);
            Files = new List<IMultipartItem>();
            RequestState = new Dictionary<string, object>();
        }

        /// <summary>
        /// Handles a line from the input stream.
        /// </summary>
        /// <param name="line">The line being read.</param>
        private void HandleLine(string line)
        {
            // Check and parse the HTTP request line
            if (this.HttpVersion == null)
            {
                var components = line.Split(new char[] { ' ' }, 4);
                if (components.Length != 3)
                    throw new HttpException(HttpStatusCode.BadRequest);

                if (components[2] != HTTP_VERSION_1_1 && components[2] != HTTP_VERSION_1_0)
                    throw new HttpException(HttpStatusCode.HTTPVersionNotSupported);

                if (string.IsNullOrWhiteSpace(components[0]) || string.IsNullOrWhiteSpace(components[1]))
                    throw new HttpException(HttpStatusCode.BadRequest);

                string qs = null;
                var path = components[1];
                var qix = path.IndexOf("?", StringComparison.Ordinal);
                if (qix >= 0)
                {
                    qs = path.Substring(qix);
                    path = path.Substring(0, qix);
                }

                this.RawHttpRequestLine = line;
                this.Method = components[0];
                this.OriginalPath = this.Path = path;
                this.RawQueryString = qs;
                this.HttpVersion = components[2];

                ParseQueryString(qs, this.QueryString);
            }
            else
            {
                var components = line.Split(new char[] { ':' }, 2);
                if (components.Length != 2 || string.IsNullOrWhiteSpace(components[0]))
                    throw new HttpException(HttpStatusCode.BadRequest);

                // Setup cookie collection automatically
                if (string.Equals(components[0].Trim(), "cookie", StringComparison.OrdinalIgnoreCase))
                    foreach (var k in RequestUtility.SplitHeaderLine((components[1] ?? string.Empty).Trim()))
                        Cookies[k.Key] = Uri.UnescapeDataString(k.Value);

                var key = components[0].Trim();
                var value = (components[1] ?? string.Empty).Trim();

                this.Headers[key] = value;
            }
        }

        /// <summary>
        /// Parses the query string.
        /// </summary>
        /// <param name="qs">The query string.</param>
        /// <param name="target">The dictionary target.</param>
        private static void ParseQueryString(string qs, IDictionary<string, string> target)
        {
            var fr = qs ?? string.Empty;
            if (fr.StartsWith("?", StringComparison.Ordinal))
                fr = fr.Substring(1);

            foreach (var frag in fr.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = frag.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                target[QueryStringSerializer.UnescapeDataString(parts[0])] = parts.Length == 1 ? null : QueryStringSerializer.UnescapeDataString(parts[1]);
            }
        }

        /// <summary>
        /// Parses the stream items, if sent as multipart encoded
        /// </summary>
        /// <param name="itemparser">The parser method</param>
        /// <param name="reader">The stream to read from.</param>
        /// <param name="config">The server configuration.</param>
        /// <param name="idletime">The maximum idle time.</param>
        /// <param name="timeouttask">A task that signals request timeout.</param>
        /// <param name="stoptask">A task that signals server stop.</param>
        /// <returns>An awaitable task</returns>
        private async Task ParseMultiPart(Func<IDictionary<string, string>, Stream, Task> itemparser, BufferedStreamReader reader, ServerConfig config, TimeSpan idletime, Task timeouttask, Task stoptask)
        {
            if ((this.ContentType ?? "").StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
            {
                if (this.ContentLength > config.MaxPostSize)
                    throw new HttpException(HttpStatusCode.PayloadTooLarge);

                var startpos = reader.Position;
                var trail = new byte[2];
                var parts = this.ContentType.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var bndpart = parts.FirstOrDefault(x => x.Trim().StartsWith("boundary", StringComparison.OrdinalIgnoreCase)) ?? string.Empty;
                var boundary = bndpart.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                if (string.IsNullOrWhiteSpace(boundary))
                    throw new HttpException(HttpStatusCode.BadRequest);

                // Since we have read the headers, we have consumed the initial CRLF
                // so we adjust the initial boundary reading to skip the CRLF
                var itemboundary = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary);
                var tmp = await reader.RepeatReadAsync(itemboundary.Length - 2, idletime, timeouttask, stoptask);
                if (!Enumerable.SequenceEqual(itemboundary.Skip(2), tmp))
                    throw new HttpException(HttpStatusCode.BadRequest);

                await reader.RepeatReadAsync(trail, 0, 2, idletime, timeouttask, stoptask);

                if (trail[0] != '\r' || trail[1] != '\n')
                    throw new HttpException(HttpStatusCode.BadRequest);

                do
                {
                    var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase).WithDefaultValue(null);

                    await reader.ReadHeaders(
                        config.MaxRequestLineSize,
                        config.MaxRequestHeaderSize,
                        idletime,
                        line =>
                        {
                            var components = line.Split(new char[] { ':' }, 2);
                            if (components.Length != 2 || string.IsNullOrWhiteSpace(components[0]))
                                throw new HttpException(HttpStatusCode.BadRequest);

                            headers[components[0].Trim()] = (components[1] ?? string.Empty).Trim();
                        },
                        timeouttask,
                        stoptask
                    );

                    await itemparser(headers, reader.GetDelimitedSubStream(itemboundary, idletime, timeouttask, stoptask));
                    await reader.RepeatReadAsync(trail, 0, 2, idletime, timeouttask, stoptask);
                }
                while (trail[0] == '\r' && trail[1] == '\n');


                if (trail[0] != '-' || trail[1] != '-')
                     throw new HttpException(HttpStatusCode.BadRequest);

                await reader.RepeatReadAsync(trail, 0, 2, idletime, timeouttask, stoptask);
                if (trail[0] != '\r' || trail[1] != '\n')
                    throw new HttpException(HttpStatusCode.BadRequest);

                if (this.ContentLength > 0 && this.ContentLength != (reader.Position - startpos))
                    throw new HttpException(HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Parses url encoded form data
        /// </summary>
        /// <returns>An awaitable task.</returns>
        /// <param name="reader">The stream to read from.</param>
        /// <param name="config">The server configuration.</param>
        /// <param name="idletime">The maximum idle time.</param>
        /// <param name="timeouttask">A task that signals request timeout.</param>
        /// <param name="stoptask">A task that signals server stop.</param>
        internal async Task ParseFormData(BufferedStreamReader reader, ServerConfig config, TimeSpan idletime, Task timeouttask, Task stoptask)
        {
            if (this.IsContentType("application/x-www-form-urlencoded"))
            {
                if (this.ContentLength != 0)
                {
                    if (this.ContentLength > config.MaxUrlEncodedFormSize)
                        throw new HttpException(HttpStatusCode.PayloadTooLarge);

                    var enc = this.GetEncodingForContentType();
                    ParseQueryString(
                        enc.GetString(
                            this.ContentLength > 0
                            ? await reader.RepeatReadAsync(this.ContentLength, idletime, timeouttask, stoptask)
                            : await reader.ReadUntilCrlfAsync(config.MaxRequestLineSize, config.MaxUrlEncodedFormSize, idletime, timeouttask, stoptask)
                        ), this.Form);
                }

                this.Body = new LimitedBodyStream(reader, 0, idletime, timeouttask, stoptask);
            }
            else if (RequestUtility.IsMultipartRequest(this.ContentType) && this.ContentLength > 0 && this.ContentLength < config.MaxUrlEncodedFormSize && config.AutoParseMultipartFormData)
            {
                await ParseMultiPart(
                    async (headers, stream) =>
                    {
                        var dispositionItems = RequestUtility.SplitHeaderLine(headers["Content-Disposition"]);
                        if (!string.Equals(dispositionItems.FirstOrDefault().Key, "form-data", StringComparison.OrdinalIgnoreCase))
                            throw new HttpException(HttpStatusCode.BadRequest);

                        var name = RequestUtility.GetHeaderComponent(headers["Content-Disposition"], "name");
                        if (string.IsNullOrWhiteSpace("name"))
                            throw new HttpException(HttpStatusCode.BadRequest);

                        var filename = RequestUtility.GetHeaderComponent(headers["Content-Disposition"], "filename");
                        var charset = RequestUtility.GetHeaderComponent(headers["Content-Type"], "charset") ?? "ascii";

                        // The logic here is that if the multipart entry has a "filename" value,
                        // we treat it as a logical file, otherwise we treat it as a (string) value 
                        if (string.IsNullOrWhiteSpace(filename))
                        {
                            using (var sr = new StreamReader(stream, RequestUtility.GetEncodingForCharset(charset)))
                            {
                                var rtask = sr.ReadToEndAsync();
                                var rt = await Task.WhenAny(timeouttask, stoptask, rtask);
                                if (rt != rtask)
                                {
                                    if (rt == stoptask)
                                        throw new TaskCanceledException();
                                    else
                                        throw new HttpException(HttpStatusCode.RequestTimeout);
                                }

                                this.Form[name] = rtask.Result;
                            }
                        }
                        else
                        {
                            var me = new MultipartItem(headers)
                            {
                                Name = name,
                                Filename = filename,
                                Data = new MemoryStream()
                            };

                            Task rtask;
                            Task rt;

                            using (var cs = new CancellationTokenSource(idletime))
                            {
                                rtask = stream.CopyToAsync(me.Data, 8 * 1024, cs.Token);
                                rt = await Task.WhenAny(timeouttask, stoptask, rtask);
                            }

                            if (rt != rtask)
                            {
                                if (rt == stoptask)
                                    throw new TaskCanceledException();
                                else
                                    throw new HttpException(HttpStatusCode.RequestTimeout);
                            }

                            rtask.GetAwaiter().GetResult();
                            me.Data.Position = 0;

                            this.Files.Add(me);
                        }
                    },
                    reader,
                    config,
                    idletime,
                    timeouttask,
                    stoptask
                );

                this.Body = new LimitedBodyStream(reader, 0, idletime, timeouttask, stoptask);
            }
            else
            {
                this.Body = new LimitedBodyStream(reader, this.ContentLength, idletime, timeouttask, stoptask);
            }
        }

        /// <summary>
        /// Parses a HTTP header by reading the input stream
        /// </summary>
        /// <returns>An awaitable task.</returns>
        /// <param name="reader">The stream to read from.</param>
        /// <param name="config">The server configuration.</param>
        /// <param name="idletime">The maximum idle time.</param>
        /// <param name="timeouttask">A task that signals request timeout.</param>
        /// <param name="stoptask">A task that signals server stop.</param>
        internal async Task Parse(BufferedStreamReader reader, ServerConfig config, TimeSpan idletime, Task timeouttask, Task stoptask)
        {
            await reader.ReadHeaders(
                config.MaxRequestLineSize,
                config.MaxRequestHeaderSize,
                idletime,
                HandleLine,
                timeouttask,
                stoptask
            );

            if (this.ContentLength > config.MaxPostSize)
                throw new HttpException(HttpStatusCode.PayloadTooLarge);
            
            // Disable HTTP/1.0 unless explictly allowed
            if (!config.AllowLegacyHttp && this.HttpVersion == HTTP_VERSION_1_0)
                throw new HttpException(HttpStatusCode.HTTPVersionNotSupported);

            // Enforce HTTP/1.1 requiring a header
            if (this.HttpVersion != HTTP_VERSION_1_0 && string.IsNullOrWhiteSpace(this.Headers["Host"]))
                throw new HttpException(HttpStatusCode.BadRequest, "Host header missing");                

            if (config.AllowHttpMethodOverride)
            {
                string newmethod;
                this.Headers.TryGetValue("X-HTTP-Method-Override", out newmethod);
                if (!string.IsNullOrWhiteSpace(newmethod))
                    this.Method = newmethod;
            }

            if (!string.IsNullOrWhiteSpace(config.AllowedSourceIPHeaderValue))
            {
                string realip;
                this.Headers.TryGetValue(config.AllowedSourceIPHeaderValue, out realip);
                if (!string.IsNullOrWhiteSpace(realip))
                    this.RemoteEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(realip), ((System.Net.IPEndPoint)this.RemoteEndPoint).Port);
            }

            await ParseFormData(reader, config, idletime, timeouttask, stoptask);
        }

        /// <summary>
        /// The list of handled request modules
        /// </summary>
        private List<IHttpModule> m_handlerStack = new List<IHttpModule>();

        /// <summary>
        /// Gets the handlers that have processed this request
        /// </summary>
        public IEnumerable<IHttpModule> HandlerStack { get { return m_handlerStack; } }

        /// <summary>
        /// Clears the handler stack.
        /// </summary>
        internal void ClearHandlerStack()
        {
            m_handlerStack.Clear();
        }

        /// <summary>
        /// Registers a handler on the request stack
        /// </summary>
        public void PushHandlerOnStack(IHttpModule handler)
        {
            m_handlerStack.Add(handler);
        }

        /// <summary>
        /// Enforces that the handler stack obeys the requirements
        /// </summary>
        /// <param name="attributes">The list of attributes to check.</param>
        public void RequireHandler(IEnumerable<RequireHandlerAttribute> attributes)
        {
            if (attributes == null)
                return;

            foreach (var attr in attributes)
            {
                var any =
                    attr.AllowDerived
                        ? m_handlerStack.Any(x => attr.RequiredType.IsAssignableFrom(x.GetType()))
                        : m_handlerStack.Any(x => attr.RequiredType == x.GetType());

                if (!any)
                    throw new RequirementFailedException($"Did not find any handlers of type {attr.RequiredType.FullName} while processing path {this.Path}. The handler stack contains: {string.Join(", ", m_handlerStack.Select(x => x.GetType().FullName))}");
            }
        }

        /// <summary>
        /// Enforces that the given type has processed the request
        /// </summary>
        /// <param name="handler">The type to check for.</param>
        /// <param name="allowderived">A flag indicating if the type match must be exact, or if derived types are accepted</param>
        public void RequireHandler(Type handler, bool allowderived = true)
        {
            RequireHandler(new[] { new RequireHandlerAttribute(handler) { AllowDerived = allowderived } });
        }

        /// <summary>
        /// Sets the processing timeout value.
        /// </summary>
        /// <param name="maxtime">The maximum processing time.</param>
        internal void SetProcessingTimeout(TimeSpan maxtime)
        {
            RequestProcessingStarted = DateTime.Now;

            m_processingtimeout = maxtime;
            ResetProcessingTimeout();
        }

        /// <summary>
        /// Resets the processing timeout.
        /// </summary>
        public void ResetProcessingTimeout()
        {
            // Stop any existing timeout
            m_processingtimeoutcancellation.Cancel();

            if (m_processingtimeout.Ticks > 0)
            {
                // Setup cancellation for the timeout
                m_processingtimeoutcancellation = new CancellationTokenSource();

                // Prepare a task to trigger the actual timeout
                Task
                    .Delay(m_processingtimeout, m_processingtimeoutcancellation.Token)
                    .ContinueWith(
                        _ => m_cancelRequest.Cancel(),
                        TaskContinuationOptions.OnlyOnRanToCompletion
                    );
            }
        }

        public void Dispose()
        {
            m_processingtimeoutcancellation.Dispose();
        }
    }
}

