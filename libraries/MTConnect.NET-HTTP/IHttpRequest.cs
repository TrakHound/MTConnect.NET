using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace MTConnect.Http
{
    /// <summary>
    /// Interface for a http request.
    /// </summary>
    public interface IHttpRequest
    {
        /// <summary>
        /// Gets the HTTP Request line as sent by the client
        /// </summary>
        string RawHttpRequestLine { get; }
        /// <summary>
        /// The HTTP method or Verb
        /// </summary>
        string Method { get; }
        /// <summary>
        /// The path of the query, not including the query string
        /// </summary>
        string Path { get; }
        /// <summary>
        /// Gets the original path before internal rewrite.
        /// </summary>
        string OriginalPath { get; }
        /// <summary>
        /// The query string
        /// </summary>
        string RawQueryString { get; }
        /// <summary>
        /// Gets a parsed representation of the query string.
        /// Duplicate values are not represented, instead only the latest is stored
        /// </summary>
        IDictionary<string, string> QueryString { get; }
        /// <summary>
        /// Gets the headers found in the request.
        /// Duplicate values are not represented, instead only the latest is stored
        /// </summary>
        IDictionary<string, string> Headers { get; }
        /// <summary>
        /// Gets the form data, if any.
        /// Duplicate values are not represented, instead only the latest is stored
        /// </summary>
        IDictionary<string, string> Form { get; }
        /// <summary>
        /// Gets the cookies supplied, if any.
        /// Duplicate values are not represented, instead only the latest is stored
        /// </summary>
        IDictionary<string, string> Cookies { get; }
        /// <summary>
        /// Gets the http version string.
        /// </summary>
        string HttpVersion { get; }
        /// <summary>
        /// Gets or sets a user identifier attached to the request.
        /// This can be set by handlers processing the request to simplify dealing with logged in users.
        /// Handlers should only set this is the user is authenticated.
        /// This value can be logged.
        /// </summary>
        string UserID { get; set; }
        /// <summary>
        /// Gets or sets a session tracking ID.
        /// This value can be logged and used to group requests from a single session
        /// better than simply grouping by IP address
        /// </summary>
        string SessionID { get; set; }
        /// <summary>
        /// Gets a value indicating what connection security is used.
        /// </summary>
        SslProtocols SslProtocol { get; }
        /// <summary>
        /// Gets the remote endpoint
        /// </summary>
        EndPoint RemoteEndPoint { get; }
        /// <summary>
        /// Gets the client SSL certificate, if any
        /// </summary>
        X509Certificate ClientCertificate { get; }
        /// <summary>
        /// The taskid used for logging and tracing the connection
        /// </summary>
        string LogConnectionID { get; }
        /// <summary>
        /// The taskid used for logging and tracing the request
        /// </summary>
        string LogRequestID { get; }

        /// <summary>
        /// The stream representing the body of the request
        /// </summary>
        Stream Body { get; }

        /// <summary>
        /// Gets the HTTP Content-Type header value
        /// </summary>
        /// <value>The type of the content.</value>
        string ContentType { get; }

        /// <summary>
        /// Gets the HTTP Content-Length header value
        /// </summary>
        /// <value>The length of the content.</value>
        int ContentLength { get; }

        /// <summary>
        /// Gets the HTTP request hostname, can be null for a HTTP/1.0 request
        /// </summary>
        string Hostname { get; }

        /// <summary>
        /// Gets a dictionary with items attached to the current request.
        /// </summary>
        IDictionary<string, object> RequestState { get; }
    }
}
