// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Web;

namespace MTConnect
{
    public static class Url
    {
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
