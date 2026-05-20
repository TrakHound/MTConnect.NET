// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Assets.Json;
using MTConnect.Devices;
using MTConnect.Devices.Json;
using MTConnect.Errors;
using MTConnect.Streams;
using MTConnect.Streams.Json;
using MTConnect.Streams.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MTConnect.Formatters
{
    /// <summary>
    /// <see cref="IResponseDocumentFormatter"/> that serializes and
    /// deserializes complete MTConnect response documents (devices, streams,
    /// assets, and errors) to and from JSON using the Json surrogate types.
    /// </summary>
    public class JsonResponseDocumentFormatter : IResponseDocumentFormatter
    {
        /// <summary>
        /// The identifier of this formatter, <c>JSON</c>.
        /// </summary>
        public string Id => "JSON";

        /// <summary>
        /// The MIME content type produced by this formatter,
        /// <c>application/json</c>.
        /// </summary>
        public string ContentType => "application/json";


        /// <summary>
        /// Serializes an MTConnectDevices response document to a JSON stream,
        /// honoring the <c>indentOutput</c> option.
        /// </summary>
        public FormatWriteResult Format(IDevicesResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");
            var jsonOptions = indentOutput ? JsonFunctions.IndentOptions : JsonFunctions.DefaultOptions;

            var outputStream = new MemoryStream();
            JsonSerializer.Serialize(outputStream, new JsonDevicesDocument(document), jsonOptions);
            if (outputStream != null && outputStream.Length > 0)
            {
                return FormatWriteResult.Successful(outputStream, ContentType);
            }

            return FormatWriteResult.Error();
        }

        /// <summary>
        /// Serializes an MTConnectStreams response document to a JSON stream,
        /// honoring the <c>indentOutput</c> option.
        /// </summary>
        public FormatWriteResult Format(ref IStreamsResponseOutputDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");
            var jsonOptions = indentOutput ? JsonFunctions.IndentOptions : JsonFunctions.DefaultOptions;

            var outputStream = new MemoryStream();
            JsonSerializer.Serialize(outputStream, new JsonStreamsDocument(document), jsonOptions);
            if (outputStream != null && outputStream.Length > 0)
            {
                return FormatWriteResult.Successful(outputStream, ContentType);
            }

            return FormatWriteResult.Error();
        }

        /// <summary>
        /// Serializes an MTConnectAssets response document to a JSON stream,
        /// honoring the <c>indentOutput</c> option.
        /// </summary>
        public FormatWriteResult Format(IAssetsResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");
            var jsonOptions = indentOutput ? JsonFunctions.IndentOptions : JsonFunctions.DefaultOptions;

            var outputStream = new MemoryStream();
            JsonSerializer.Serialize(outputStream, new JsonAssetsDocument(document), jsonOptions);
            if (outputStream != null && outputStream.Length > 0)
            {
                return FormatWriteResult.Successful(outputStream, ContentType);
            }

            return FormatWriteResult.Error();
        }

        /// <summary>
        /// Serializes an MTConnectError response document to a JSON stream,
        /// honoring the <c>indentOutput</c> option.
        /// </summary>
        public FormatWriteResult Format(IErrorResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");
            var jsonOptions = indentOutput ? JsonFunctions.IndentOptions : JsonFunctions.DefaultOptions;

            var outputStream = new MemoryStream();
            JsonSerializer.Serialize(outputStream, document, jsonOptions);
            if (outputStream != null && outputStream.Length > 0)
            {
                return FormatWriteResult.Successful(outputStream, ContentType);
            }

            return FormatWriteResult.Error();
        }


        /// <summary>
        /// Deserializes an MTConnectDevices response document from a JSON
        /// stream.
        /// </summary>
        public FormatReadResult<IDevicesResponseDocument> CreateDevicesResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Document
            var document = JsonSerializer.Deserialize<JsonDevicesDocument>(content);
            var success = document != null;

            return new FormatReadResult<IDevicesResponseDocument>(document.ToDocument(), success);
        }

        /// <summary>
        /// Deserializes an MTConnectStreams response document from a JSON
        /// stream.
        /// </summary>
        public FormatReadResult<IStreamsResponseDocument> CreateStreamsResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Document
            var document = JsonSerializer.Deserialize<JsonStreamsDocument>(content);
            var success = document != null;

            return new FormatReadResult<IStreamsResponseDocument>(document.ToStreamsDocument(), success);
        }

        /// <summary>
        /// Deserializes an MTConnectAssets response document from a JSON
        /// stream.
        /// </summary>
        public FormatReadResult<IAssetsResponseDocument> CreateAssetsResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Document
            var document = JsonSerializer.Deserialize<AssetsResponseDocument>(content);
            var success = document != null;

            return new FormatReadResult<IAssetsResponseDocument>(document, success);
        }

        /// <summary>
        /// Deserializes an MTConnectError response document from a JSON
        /// stream.
        /// </summary>
        public FormatReadResult<IErrorResponseDocument> CreateErrorResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Document
            var document = JsonSerializer.Deserialize<ErrorResponseDocument>(content);
            var success = document != null;

            return new FormatReadResult<IErrorResponseDocument>(document, success);
        }


        private static T GetFormatterOption<T>(IEnumerable<KeyValuePair<string, string>> options, string key)
        {
            if (!options.IsNullOrEmpty())
            {
                var x = options.FirstOrDefault(o => o.Key == key).Value;
                if (!string.IsNullOrEmpty(x))
                {
                    try
                    {
                        return (T)Convert.ChangeType(x, typeof(T));
                    }
                    catch { }
                }
            }

            return default;
        }

        private static IEnumerable<T> GetFormatterOptions<T>(IEnumerable<KeyValuePair<string, string>> options, string key)
        {
            var l = new List<T>();

            if (!options.IsNullOrEmpty())
            {
                var x = options.Where(o => o.Key == key);
                if (!x.IsNullOrEmpty())
                {
                    foreach (var y in x)
                    {
                        if (!string.IsNullOrEmpty(y.Value))
                        {
                            try
                            {
                                var obj = (T)Convert.ChangeType(y.Value, typeof(T));
                                l.Add(obj);
                            }
                            catch { }
                        }
                    }
                }
            }

            return l;
        }
    }
}