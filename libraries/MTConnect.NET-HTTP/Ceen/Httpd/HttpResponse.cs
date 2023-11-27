using System.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace Ceen.Httpd
{
	/// <summary>
	/// Interface for providing a HTTP response
	/// </summary>
	internal class HttpResponse : IHttpResponse
	{
		/// <summary>
		/// Dictionary wrapper to keep track of what headers to emit
		/// </summary>
		private class HttpResponseHeaders : IDictionary<string, string>
		{
			/// <summary>
			/// The wrapped parent
			/// </summary>
			private readonly HttpResponse m_parent;

			/// <summary>
			/// Initializes a new instance of the <see cref="T:Ceen.Httpd.HttpResponse.HttpResponseHeaders"/> class.
			/// </summary>
			/// <param name="parent">The parent to wrap.</param>
			public HttpResponseHeaders(HttpResponse parent)
			{
				m_parent = parent;
			}

			#region IDictionary implementation
			public bool ContainsKey(string key)
			{
				return m_parent.m_headers.ContainsKey(key);
			}
			public void Add(string key, string value)
			{
				m_parent.AddHeader(key, value);
			}
			public bool Remove(string key)
			{
				var hasit = ContainsKey(key);
				m_parent.AddHeader(key, null);
				return hasit;
			}
			public bool TryGetValue(string key, out string value)
			{
				return m_parent.m_headers.TryGetValue(key, out value);
			}
			public string this[string index]
			{
				get
				{
					string s;
					if (!TryGetValue(index, out s))
						s = null;

					return s;
				}
				set
				{
					m_parent.AddHeader(index, value);
				}
			}
			public ICollection<string> Keys
			{
				get
				{
					return m_parent.m_headers.Keys;
				}
			}
			public ICollection<string> Values
			{
				get
				{
					return m_parent.m_headers.Values;
				}
			}
			#endregion
			#region ICollection implementation
			public void Add(KeyValuePair<string, string> item)
			{
				m_parent.AddHeader(item.Key, item.Value);
			}
			public void Clear()
			{
				if (m_parent.HasSentHeaders)
					m_parent.AddHeader("dummy", "dummy"); // Trigger exeception
				m_parent.m_headers.Clear();
			}
			public bool Contains(KeyValuePair<string, string> item)
			{
				string value;
				return TryGetValue(item.Key, out value) && value == item.Value;
			}
			public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}
			public bool Remove(KeyValuePair<string, string> item)
			{
				if (Contains(item) || m_parent.HasSentHeaders)
				{
					Remove(item.Key);
					return true;
				}

				return false;
			}
			public int Count
			{
				get
				{
					return m_parent.m_headers.Count;
				}
			}
			public bool IsReadOnly
			{
				get
				{
					return m_parent.HasSentHeaders;
				}
			}
			#endregion
			#region IEnumerable implementation
			public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
			{
				return m_parent.m_headers.GetEnumerator();
			}
			#endregion
			#region IEnumerable implementation
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
			#endregion
		}

		/// <summary>
		/// Gets or sets the HTTP version to report.
		/// </summary>
		/// <value>The http version.</value>
		public string HttpVersion { get; set; }
		/// <summary>
		/// Gets or sets the status code to report.
		/// </summary>
		/// <value>The status code.</value>
		public HttpStatusCode StatusCode { get; set; }
		/// <summary>
		/// Gets or sets the status message to report.
		/// If this is <c>null</c>, the default message for
		/// the HTTP status code is used
		/// </summary>
		/// <value>The status message.</value>
		public string StatusMessage { get; set; }
		/// <summary>
		/// Gets a value indicating whether the sent headers are sent to the client.
		/// Once the headers are sent, the header collection can no longer be modified
		/// </summary>
		/// <value><c>true</c> if this instance has sent headers; otherwise, <c>false</c>.</value>
		public bool HasSentHeaders { get { return m_hasSentHeaders; } }
		/// <summary>
		/// Dictionary with headers that are sent as part of the response.
		/// Cannot be modified after the headers have been sent.
		/// </summary>
		/// <value>The headers.</value>
		public IDictionary<string, string> Headers { get { return m_headerwrapper; } }

		/// <summary>
		/// Gets a list of cookies that are set with the response.
		/// Cannot be modified after the headers have been sent.
		/// </summary>
		/// <value>The cookies.</value>
		public IList<IResponseCookie> Cookies { get; private set; }

		/// <summary>
		/// The underlying output stream
		/// </summary>
		private Stream m_stream;
		/// <summary>
		/// The intercepting output stream exposed from this instance
		/// </summary>
		private ResponseOutputStream m_outstream;
		/// <summary>
		/// The wrapped output stream
		/// </summary>
		private Stream m_wrappedoutstream;
		/// <summary>
		/// The internal storage for the response headers
		/// </summary>
		private Dictionary<string, string> m_headers;
		/// <summary>
		/// The value indicating if the headers have been sent
		/// </summary>
		private bool m_hasSentHeaders = false;
		/// <summary>
		/// The wrapper class for controlling headers
		/// </summary>
		private HttpResponseHeaders m_headerwrapper;
		/// <summary>
		/// The server configuration
		/// </summary>
		private readonly ServerConfig m_serverconfig;
		/// <summary>
		/// The internal redirect path
		/// </summary>
		private string m_internalredirectpath;
		/// <summary>
		/// The number of internal redirects used
		/// </summary>
		private int m_internalredirects;

		/// <summary>
		/// The CRLF line termination string
		/// </summary>
		private const string CRLF = "\r\n";

		/// <summary>
		/// Initializes a new instance of the <see cref="Ceen.Httpd.HttpResponse"/> class.
		/// </summary>
		/// <param name="stream">The underlying stream.</param>
		/// <param name="config">The server configuration.</param>
		public HttpResponse(Stream stream, ServerConfig config)
		{
			m_stream = stream;
			m_serverconfig = config;
			m_headerwrapper = new HttpResponseHeaders(this);
			Cookies = new List<IResponseCookie>();
			Reset();
		}

		/// <summary>
		/// Resets this instance so it can be re-used
		/// </summary>
		private void Reset()
		{
			this.HttpVersion = "HTTP/1.1";
			this.StatusCode = HttpStatusCode.OK;

			m_headers = new Dictionary<string, string>();
			m_outstream = new ResponseOutputStream(m_stream, this);
			m_wrappedoutstream = m_outstream;
			m_hasSentHeaders = false;

			AddDefaultHeaders();
		}

		/// <summary>
		/// Adds the default headers
		/// </summary>
		private void AddDefaultHeaders()
		{
			if (m_serverconfig.AddDefaultResponseHeaders != null)
				m_serverconfig.AddDefaultResponseHeaders(this);
		}

		/// <summary>
		/// Adds a header to the output, use null to delete a header.
		/// This method throws an exception if the headers are already sent
		/// </summary>
		/// <param name="key">The header name.</param>
		/// <param name="value">The header value.</param>
		public void AddHeader(string key, string value)
		{
			if (m_hasSentHeaders)
				throw new InvalidOperationException("Cannot set headers after they are sent");

			if (value == null)
				m_headers.Remove(key);
			else
				m_headers[key] = value;
		}

		/// <summary>
		/// Gets or sets the Content-Type header
		/// </summary>
		/// <value>The type of the content.</value>
		public string ContentType
		{
			get
			{
				m_headers.TryGetValue("Content-Type", out var v);
				return v;
			}
			set
			{
				AddHeader("Content-Type", value);
			}
		}

		/// <summary>
		/// Gets or sets the Content-Length header
		/// </summary>
		/// <value>The length of the content.</value>
		public long ContentLength
		{
			get
			{
				m_headers.TryGetValue("Content-Length", out var v);

				if (!long.TryParse(v, out var vv))
					return -1;
				
				return vv;
			}
			set
			{
				AddHeader("Content-Length", value == -1 ? null : value.ToString());
			}
		}

		/// <summary>
		/// Gets or sets the Keep-Alive header
		/// </summary>
		/// <value><c>true</c> if keep alive; otherwise, <c>false</c>.</value>
		public bool KeepAlive
		{
			get
			{
				m_headers.TryGetValue("Connection", out var v);
				return string.Equals("keep-alive", v, StringComparison.OrdinalIgnoreCase);
			}
			set
			{
				AddHeader("Connection", value ? "keep-alive" : "close");
			}
		}

		/// <summary>
		/// Helper property to check if the internal stream has written the number of bytes
		/// sent with Content-Length. Used to determine if keep-alive is possible
		/// </summary>
		/// <value><c>true</c> if this instance has written correct length; otherwise, <c>false</c>.</value>
		internal bool HasWrittenCorrectLength
		{
			get
			{
				if (ContentLength < 0)
					return false;

				return m_outstream.Length == ContentLength;
			}
		}

		/// <summary>
		/// Flush all headers async.
		/// This method can be called multiple times if desired.
		/// </summary>
		/// <returns>The headers async.</returns>
		public async Task FlushHeadersAsync()
		{
			if (!m_hasSentHeaders)
			{
				// Allow post-processor hook-ins
				if (m_serverconfig.PostProcessors != null)
					foreach(var p in m_serverconfig.PostProcessors)
						await p.HandleAsync(this.Context);

				if (string.IsNullOrWhiteSpace(this.StatusMessage))
					this.StatusMessage = HttpStatusMessages.DefaultMessage(this.StatusCode);

				var line = Encoding.ASCII.GetBytes(string.Format("{0} {1} {2}{3}", this.HttpVersion, (int)this.StatusCode, this.StatusMessage, CRLF));
				await m_stream.WriteAsync(line, 0, line.Length);

				foreach (var e in m_headers)
				{
					line = Encoding.ASCII.GetBytes(string.Format("{0}: {1}{2}", e.Key, e.Value, CRLF));
					await m_stream.WriteAsync(line, 0, line.Length);
				}

				foreach (var cookie in Cookies)
				{
					var sb = new StringBuilder();
					sb.Append("Set-Cookie: ");
					sb.Append(cookie.Name);
                    sb.Append("=");
					
                    if (!string.IsNullOrWhiteSpace(cookie.Value))
					{
						// URL encoding not required, but common practice
						sb.Append(Uri.EscapeDataString(cookie.Value));
					}

					foreach (var setting in cookie.Settings)
					{
						sb.Append("; ");
						sb.Append(setting.Key);
						if (!string.IsNullOrWhiteSpace(setting.Value))
						{
							sb.Append("=");
							sb.Append(setting.Value);
						}
					}

					sb.Append(CRLF);
					line = Encoding.ASCII.GetBytes(sb.ToString());

					await m_stream.WriteAsync(line, 0, line.Length);
				}

				if (Cookies is List<IResponseCookie>)
					Cookies = ((List<IResponseCookie>)Cookies).AsReadOnly();

				line = Encoding.ASCII.GetBytes(CRLF);
				await m_stream.WriteAsync(line, 0, line.Length);
			
				m_hasSentHeaders = true;
			}
		}

		/// <summary>
		/// Flushes all headers and sets the length to the amount of data currently buffered in the output
		/// </summary>
		/// <param name="token">The cancellation token to use</param>
		/// <returns>An awaitable task</returns>
		internal async Task FlushAndSetLengthAsync(CancellationToken token)
		{
			// Make sure any buffered data is flushed
			if (m_wrappedoutstream != m_outstream)
				await m_wrappedoutstream.FlushAsync(token);

			await m_outstream.SetLengthAndFlushAsync(true, token);
		}

		/// <summary>
		/// Flushes the underlying stream
		/// </summary>
		/// <param name="token">The cancellation token to use</param>
		/// <returns>An awaitable task</returns>
		internal Task FlushStreamAsync(CancellationToken token)
			=> m_outstream.FlushAsync(token);

		/// <summary>
		/// Flush the contents after an error occurred.
		/// </summary>
		/// <returns>An awaitable task</returns>
		internal async Task FlushAsErrorAsync()
		{
			if (!m_hasSentHeaders)
				await FlushHeadersAsync();
			m_outstream.Clear();
		}

		/// <summary>
		/// Copies the stream to the output. Note that the stream is copied from the current position to the end, and the stream must report the length.
		/// </summary>
		/// <returns>The awaitable task</returns>
		/// <param name="data">The stream to copy.</param>
		/// <param name="contenttype">An optional content type to set. Throws an exception if the headers are already sent.</param>
		public Task WriteAllAsync(Stream data, string contenttype = null)
		{
			if (contenttype != null)
				ContentType = contenttype;
			if (!HasSentHeaders)
				ContentLength = data.Length - data.Position;
			return data.CopyToAsync(m_wrappedoutstream);
		}

		/// <summary>
		/// Writes the byte array to the output.
		/// </summary>
		/// <returns>The awaitable task</returns>
		/// <param name="data">The data to write.</param>
		/// <param name="contenttype">An optional content type to set. Throws an exception if the headers are already sent.</param>
		public Task WriteAllAsync(byte[] data, string contenttype = null)
		{
			if (contenttype != null)
				ContentType = contenttype;
			if (!HasSentHeaders)
				ContentLength = data.Length;
			return m_wrappedoutstream.WriteAsync(data, 0, data.Length);
		}

		/// <summary>
		/// Writes the string to the output using UTF-8 encoding.
		/// </summary>
		/// <returns>The awaitable task</returns>
		/// <param name="data">The data to write.</param>
		/// <param name="encoding">The encoding to apply.</param>
		/// <param name="contenttype">An optional content type to set. Throws an exception if the headers are already sent.</param>
		public Task WriteAllAsync(string data, string contenttype = null)
		{
			return WriteAllAsync(System.Text.Encoding.UTF8.GetBytes(data), contenttype);
		}

		/// <summary>
		/// Writes the string to the output.
		/// </summary>
		/// <returns>The awaitable task</returns>
		/// <param name="data">The data to write.</param>
		/// <param name="encoding">The encoding to apply.</param>
		/// <param name="contenttype">An optional content type to set. Throws an exception if the headers are already sent.</param>
		public Task WriteAllAsync(string data, Encoding encoding, string contenttype = null)
		{
			return WriteAllAsync(encoding.GetBytes(data), contenttype);
		}

		/// <summary>
		/// Writes the json string to the output with UTF-8 encoding.
		/// </summary>
		/// <returns>The awaitable task</returns>
		/// <param name="data">The JSON data to write.</param>
		public Task WriteAllJsonAsync(string data)
		{
			if (!HasSentHeaders)
				ContentType = "application/json; charset=utf-8";
			return WriteAllAsync(System.Text.Encoding.UTF8.GetBytes(data));
		}

		/// <summary>
		/// Performs a 302 redirect
		/// </summary>
		/// <param name="newurl">The target url.</param>
		public void Redirect(string newurl)
		{
			Headers["Location"] = newurl;
			StatusCode = HttpStatusCode.Found;
			StatusMessage = HttpStatusMessages.DefaultMessage(StatusCode);
		}

		/// <summary>
		/// Sets headers that instruct the client and proxies to avoid caching the response
		/// </summary>
		public void SetNonCacheable()
		{
			Headers["Date"] = Headers["Expires"] = DateTime.Now.ToString("R", CultureInfo.InvariantCulture);
            Headers["Cache-Control"] = "no-cache, no-store, must-revalidate, max-age=0";
		}

        /// <summary>
        /// Sets headers that instruct the client and proxies to allow caching for a limited time
        /// </summary>
        /// <param name="duration">The time the client is allowed to cache the response</param>
        /// <param name="isPublic">A flag indicating if the response is public and can be cached by proxies</param>
        public void SetExpires(TimeSpan duration, bool isPublic = true)
        {
            Headers["Expires"] = (DateTime.Now + duration).ToString("R", CultureInfo.InvariantCulture);
            Headers["Cache-Control"] = $"{(isPublic ? "public" : "private")}, max-age={Math.Max(0, Math.Min(duration.TotalSeconds, int.MaxValue))}";
        }

        /// <summary>
        /// Sets headers that instruct the client and proxies to allow caching for a limited time
        /// </summary>
        /// <param name="until">The time the client is no longer allowed to use the cached response</param>
        /// <param name="isPublic">A flag indicating if the response is public and can be cached by proxies</param>
        public void SetExpires(DateTime until, bool isPublic = true)
        {
            SetExpires(until - DateTime.Now, isPublic);
        }

		/// <summary>
		/// Gets the response stream.
		/// To avoid buffering the contents, make sure the
		/// Content-Length header is set before writing to the stream
		/// </summary>
		/// <returns>The response stream.</returns>
		public Stream GetResponseStream()
		{
			return m_wrappedoutstream;
		}

		/// <summary>
		/// Changes the output stream to a wrapped stream
		/// </summary>
		/// <param name="stream">The stream that wraps the current output</param>
		public void SetOutputWrapperStream(Stream stream)
		{
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            
			if (m_hasSentHeaders)
				throw new InvalidOperationException("Cannot wrap the output stream after the headers have been sent");
			
			if (m_wrappedoutstream != m_outstream)
				throw new InvalidOperationException("Cannot re-wrap a wrapped output stream");

			// Copy any currently buffered content into the new wrapper
			m_outstream.Unbuffer(stream);
			m_wrappedoutstream = stream;

		}

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
		public IResponseCookie AddCookie(string name, string value, string path = null, string domain = null, DateTime? expires = null, long maxage = -1, bool secure = false, bool httponly = false, string samesite = null)
		{
			var cookie = new ResponseCookie(name, value) 
			{
				Path = path,
				Domain = domain,
				Expires = expires,
				MaxAge = maxage,
				Secure = secure,
				HttpOnly = httponly,
				SameSite = samesite
			};
			this.Cookies.Add(cookie);

			return cookie;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Ceen.Httpd.HttpResponse"/> is redirecting internally.
		/// </summary>
		public bool IsRedirectingInternally { get { return !string.IsNullOrWhiteSpace(m_internalredirectpath); } }

		/// <summary>
		/// The context this response belongs to
		/// </summary>
        internal HttpContext Context { get; set; }

        /// <summary>
        /// Performs an internal redirect
        /// </summary>
        /// <param name="path">The new path to use.</param>
        public void InternalRedirect(string path)
		{
			if (HasSentHeaders)
				throw new Exception("Cannot redirect after headers have been sent");
			if (IsRedirectingInternally)
				throw new Exception("An internal redirect is already in progress");
			if (string.IsNullOrWhiteSpace(path) || !path.StartsWith("/", StringComparison.Ordinal))
				throw new ArgumentException("The path must start with a forward slash '/'", nameof(path));
			if (m_internalredirects >= m_serverconfig.MaxInternalRedirects)
				throw new Exception($"Cannot perform more than {m_serverconfig.MaxInternalRedirects} redirects");

			m_internalredirects++;
			m_internalredirectpath = path;
		}

		/// <summary>
		/// Clears the internal redirect status.
		/// </summary>
		/// <returns>The redirected path</returns>
		public string ClearInternalRedirect()
		{
			var prev = m_internalredirectpath;
			m_internalredirectpath = null;
			return prev;
		}
	}
}

