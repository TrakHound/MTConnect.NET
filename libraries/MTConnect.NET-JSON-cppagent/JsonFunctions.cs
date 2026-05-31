// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTConnect
{
    /// <summary>
    /// Helpers for configuring the cppagent-compatible JSON serializer
    /// (option sets and one-shot convert wrappers around
    /// <see cref="JsonSerializer"/>). The shared option sets switch off
    /// indentation by default, ignore properties with default values,
    /// allow numbers to be read from strings (a common cppagent edge),
    /// and keep property lookup case-insensitive to tolerate equipment
    /// payload variation.
    /// </summary>
    public static class JsonFunctions
    {
        /// <summary>
        /// Default serializer options used when no <c>indentOutput</c>
        /// option is requested. Produces compact JSON, omits properties
        /// at their default value, allows numbers to be read from
        /// strings, and ignores property-name casing.
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
        /// Pretty-printed serializer options used when the
        /// <c>indentOutput</c> formatter option is enabled; otherwise
        /// identical to <see cref="DefaultOptions"/>.
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
        /// Serializes <paramref name="obj"/> to a JSON string using the
        /// cppagent option defaults, optionally with an extra converter
        /// and pretty-printing. Returns <c>null</c> on any
        /// serialization failure or when the input is null.
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
        /// Serializes <paramref name="obj"/> to a UTF-8 byte array
        /// using the cppagent option defaults, optionally with an extra
        /// converter and pretty-printing. Returns <c>null</c> on any
        /// serialization failure or when the input is null.
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
        /// Serializes <paramref name="obj"/> into a fresh
        /// <see cref="MemoryStream"/> using the cppagent option
        /// defaults, optionally with an extra converter and
        /// pretty-printing. Returns <c>null</c> on any serialization
        /// failure or when the input is null.
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