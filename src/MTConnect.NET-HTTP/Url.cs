// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
                    var p = url.Split('/');
                    if (p.Length > 1)
                    {
                        p[0] = $"{p[0]}:{port}";
                    }

                    return string.Join("/", p);
                }

                return $"{url}:{port}";
            }

            return url;
        }
    }
}
