// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect
{
    internal static class Url
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
    }
}
