// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTConnect
{
    internal static class JsonFunctions
    {
        public static string ToJson(object obj, bool indent = false)
        {
            if (obj != null)
            {
                try
                {

#if NET5_0_OR_GREATER
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = indent,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                        PropertyNameCaseInsensitive = true,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    };
#else
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = indent,
                        IgnoreNullValues = true,
                        PropertyNameCaseInsensitive = true,
                    };
#endif

                    return JsonSerializer.Serialize(obj, options);
                }
                catch { }
            }

            return null;
        }

        public static T FromJson<T>(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(json);
                }
                catch { }
            }

            return default;
        }
    }
}
