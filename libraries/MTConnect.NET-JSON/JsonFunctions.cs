// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTConnect
{
    /// <summary>
    /// JSON serialization helpers shared by the MTConnect JSON surrogate
    /// types. Provides the default and indented <see cref="JsonSerializerOptions"/>
    /// presets, ISO 8601 timestamp formatting, and convenience methods for
    /// serializing to a string, byte array, or stream.
    /// </summary>
    public static class JsonFunctions
    {
        /// <summary>
        /// The default <see cref="JsonSerializerOptions"/> used by MTConnect
        /// JSON serialization: compact output, default-valued properties
        /// omitted on write (on net5+), numbers read from strings (on net5+),
        /// case-insensitive property names, and a depth limit of 1000.
        /// </summary>
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

        /// <summary>
        /// The indented variant of <see cref="DefaultOptions"/>, used when the
        /// formatter's <c>indentOutput</c> option is set.
        /// </summary>
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


        /// <summary>
        /// Formats <paramref name="timestamp"/> as a round-trip ISO 8601
        /// string (the <c>o</c> format).
        /// </summary>
        public static string GetTimestamp(DateTime timestamp)
        {
            return timestamp.ToString("o");
        }

        /// <summary>
        /// Formats <paramref name="timestamp"/> as a round-trip ISO 8601
        /// string, normalizing to UTC when the offset is zero.
        /// </summary>
        public static string GetTimestamp(DateTimeOffset timestamp)
        {
            if (timestamp.Offset != TimeSpan.Zero)
            {
                return timestamp.ToString("o");
            }
            else
            {
                return timestamp.UtcDateTime.ToString("o");
            }
        }

        /// <summary>
        /// Serializes <paramref name="obj"/> to a JSON string using the MTConnect
        /// default options, optionally indented and optionally extended with a
        /// custom converter. Returns null on any serialization error.
        /// </summary>
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

        /// <summary>
        /// Serializes <paramref name="obj"/> to UTF-8 encoded JSON bytes using
        /// the MTConnect default options, optionally indented and optionally
        /// extended with a custom converter. Returns null on any serialization
        /// error.
        /// </summary>
        public static byte[] ConvertBytes(object obj, JsonConverter converter = null, bool indented = false)
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

                    return JsonSerializer.SerializeToUtf8Bytes(obj, options);
                }
                catch { }
            }

            return null;
        }

        /// <summary>
        /// Serializes <paramref name="obj"/> to a JSON stream using the
        /// MTConnect default options, optionally indented and optionally
        /// extended with a custom converter. Returns null on any serialization
        /// error.
        /// </summary>
        public static Stream ConvertStream(object obj, JsonConverter converter = null, bool indented = false)
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

                    var outputStream = new MemoryStream();
                    JsonSerializer.Serialize(outputStream, obj, options);
                    return outputStream;
                }
                catch { }
            }

            return null;
        }
    }
}