// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTConnect
{
    public static class JsonFunctions
    {
        public static JsonSerializerOptions DefaultOptions
        {
            get
            {
                return new JsonSerializerOptions
                {
                    WriteIndented = false,
#if NET5_0_OR_GREATER
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString,
#endif
                    PropertyNameCaseInsensitive = true,
                    MaxDepth = 1000
                };
            }
        }

        public static JsonSerializerOptions IndentOptions
        {
            get
            {
                return new JsonSerializerOptions
                {
                    WriteIndented = true,
#if NET5_0_OR_GREATER
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString,
#endif
                    PropertyNameCaseInsensitive = true,
                    MaxDepth = 1000
                };
            }
        }


        public static string Convert(object obj, JsonConverter converter = null, bool indented = false)
        {
            if (obj != null)
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = indented,
#if NET5_0_OR_GREATER
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString,
#endif
                        PropertyNameCaseInsensitive = true,
                        MaxDepth = 1000
                    };

                    if (converter != null) options.Converters.Add(converter);

                    return JsonSerializer.Serialize(obj, options);
                }
                catch { }
            }

            return null;
        }
    }
}