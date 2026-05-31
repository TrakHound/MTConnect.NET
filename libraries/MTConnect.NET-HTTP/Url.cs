// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Web;

namespace MTConnect.Http
{
    /// <summary>
    /// Small URL-manipulation helpers used by the MTConnect HTTP client and server to build the
    /// agent endpoint URLs (base + request path + query string). These are intentionally light
    /// and string-based so they work uniformly across .NET Framework, .NET Standard, and .NET 5+
    /// where <see cref="System.Uri"/> behaviour can subtly differ.
    /// </summary>
    public static class Url
    {
        /// <summary>
        /// Joins a base URL and a path segment with a single forward slash, trimming any trailing
        /// slashes from <paramref name="baseUrl"/> and any leading slashes from
        /// <paramref name="path"/> so the result never contains a duplicated separator.
        /// </summary>
        /// <param name="baseUrl">The base URL (typically the agent endpoint, e.g. <c>http://localhost:5000</c>). If null or empty it is returned unchanged.</param>
        /// <param name="path">The relative path to append (e.g. <c>probe</c>, <c>device/current</c>). If empty, the empty path itself is returned.</param>
        /// <returns>The combined URL with exactly one slash between the parts.</returns>
        public static string Combine(string baseUrl, string path)
        {
            if (baseUrl == null || baseUrl.Length == 0)
            {
                return baseUrl;
            }

            if (path.Length == 0)
            {
                return path;
            }

            baseUrl = baseUrl.TrimEnd('/', '\\');
            path = path.TrimStart('/', '\\');

            return $"{baseUrl}/{path}";
        }

        /// <summary>
        /// Inserts an explicit port into the authority portion of an HTTP(S) URL, leaving any
        /// path or query intact. The port is appended to the hostname segment immediately after
        /// the <c>://</c> scheme separator; if <paramref name="url"/> already contains a path the
        /// port is spliced before that path. URLs without a scheme are treated as bare authorities
        /// and have <c>:port</c> appended.
        /// </summary>
        /// <param name="url">The URL to modify. If null, empty, or <paramref name="port"/> is non-positive, the original value is returned.</param>
        /// <param name="port">The TCP port to inject. Values less than or equal to zero are ignored.</param>
        public static string AddPort(string url, int port)
        {
            if (!string.IsNullOrEmpty(url) && port > 0)
            {
                if (url.Contains("/"))
                {
                    // Get index of "://" (ex. http://, https://)
                    var protocolIndex = url.IndexOf("://");
                    if (protocolIndex < 0) protocolIndex = 0;

                    // Check if URL contains a path after the hostname
                    var pathIndex = url.IndexOf("/", protocolIndex + 3);
                    if (pathIndex > 0)
                    {
                        var server = url.Substring(0, pathIndex);
                        var path = url.Substring(pathIndex);

                        var p = path.Split('/');
                        if (p.Length > 1)
                        {
                            p[0] = $"{p[0]}:{port}";
                        }

                        return $"{server}{string.Join("/", p)}";
                    }
                }

                return $"{url}:{port}";
            }

            return url;
        }

        /// <summary>
        /// Appends a single <c>name=value</c> pair to the query string of <paramref name="url"/>,
        /// URL-encoding the value with <see cref="HttpUtility.UrlEncode(string)"/>. If the URL
        /// already contains a <c>?</c> the pair is concatenated with <c>&amp;</c>; otherwise a
        /// new query string is started. A null or empty <paramref name="parameterValue"/> (or its
        /// <c>ToString</c> result) leaves the URL untouched, which is the behaviour MTConnect
        /// callers rely on when optional request parameters such as <c>from</c>, <c>to</c>, or
        /// <c>interval</c> have not been set.
        /// </summary>
        /// <param name="url">The base URL to extend.</param>
        /// <param name="parameterName">The query parameter name. Ignored if null or empty.</param>
        /// <param name="parameterValue">The query parameter value; its <see cref="object.ToString"/> is URL-encoded.</param>
        public static string AddQueryParameter(string url, string parameterName, object parameterValue)
        {
            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(parameterName) && parameterValue != null)
            {
                var valueStr = parameterValue.ToString();
                if (!string.IsNullOrEmpty(valueStr))
                {
                    string path;
                    string query = null;

                    var i = url.IndexOf('?');
                    if (i > 0)
                    {
                        path = url.Substring(0, i);
                        query = url.Substring(i);
                    }
                    else
                    {
                        path = url;
                    }

                    if (!string.IsNullOrEmpty(path))
                    {
                        if (!string.IsNullOrEmpty(query))
                        {
                            query += '&' + parameterName + '=' + HttpUtility.UrlEncode(valueStr);
                        }
                        else
                        {
                            query = '?' + parameterName + '=' + HttpUtility.UrlEncode(valueStr);
                        }

                        return path + query;
                    }
                }
            }

            return url;
        }
    }
}
