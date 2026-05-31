// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
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
    /// HTTP response document formatter that serializes and deserializes
    /// MTConnect response documents using the cppagent-compatible JSON
    /// shape. Routes each document family (Probe/Devices, Streams, Assets,
    /// Error) through its dedicated surrogate model and obeys the
    /// <c>indentOutput</c> formatter option to switch between compact and
    /// pretty-printed output.
    /// </summary>
    public class JsonHttpResponseDocumentFormatter : IResponseDocumentFormatter
    {
        /// <summary>
        /// The formatter identifier exposed to the agent's content-type
        /// negotiation, distinguishing this formatter from the plain
        /// MTConnect JSON formatter.
        /// </summary>
        public virtual string Id => "JSON-cppagent";

        /// <summary>
        /// The HTTP <c>Content-Type</c> emitted by this formatter.
        /// </summary>
        public virtual string ContentType => "application/json";


        /// <summary>
        /// Serializes a Probe/Devices response document using the
        /// cppagent-compatible JSON surrogate.
        /// </summary>
        public virtual FormatWriteResult Format(IDevicesResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");
            var jsonOptions = indentOutput ? JsonFunctions.IndentOptions : JsonFunctions.DefaultOptions;

            var outputStream = new MemoryStream();
            JsonSerializer.Serialize(outputStream, new JsonDevicesResponseDocument(document), jsonOptions);
            if (outputStream != null && outputStream.Length > 0)
            {
                return FormatWriteResult.Successful(outputStream, ContentType);
            }

            return FormatWriteResult.Error();
        }

        /// <summary>
        /// Serializes a Streams response document using the
        /// cppagent-compatible JSON surrogate. Accepts the output document
        /// by reference to allow downstream consumers to mutate the source
        /// without copying.
        /// </summary>
        public virtual FormatWriteResult Format(ref IStreamsResponseOutputDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");
            var jsonOptions = indentOutput ? JsonFunctions.IndentOptions : JsonFunctions.DefaultOptions;

            var outputStream = new MemoryStream();
            JsonSerializer.Serialize(outputStream, new JsonStreamsResponseDocument(document), jsonOptions);
            if (outputStream != null && outputStream.Length > 0)
            {
                return FormatWriteResult.Successful(outputStream, ContentType);
            }

            return FormatWriteResult.Error();
        }

        /// <summary>
        /// Serializes an Assets response document using the
        /// cppagent-compatible JSON surrogate.
        /// </summary>
        public virtual FormatWriteResult Format(IAssetsResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Indent Option passed to Formatter
            var indentOutput = GetFormatterOption<bool>(options, "indentOutput");
            var jsonOptions = indentOutput ? JsonFunctions.IndentOptions : JsonFunctions.DefaultOptions;

            var outputStream = new MemoryStream();
            JsonSerializer.Serialize(outputStream, new JsonAssetsResponseDocument(document), jsonOptions);
            if (outputStream != null && outputStream.Length > 0)
            {
                return FormatWriteResult.Successful(outputStream, ContentType);
            }

            return FormatWriteResult.Error();
        }

        /// <summary>
        /// Serializes an Error response document as JSON. The error
        /// document has no cppagent-specific surrogate shape so it is
        /// written using its native model.
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
        /// Deserializes a Probe/Devices response document from a JSON
        /// stream and reconstructs the strongly-typed model.
        /// </summary>
        public virtual FormatReadResult<IDevicesResponseDocument> CreateDevicesResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Document
            var document = JsonSerializer.Deserialize<JsonDevicesResponseDocument>(content);
            var success = document != null;

            return new FormatReadResult<IDevicesResponseDocument>(document.ToDocument(), success);
        }

        /// <summary>
        /// Deserializes a Streams response document from a JSON stream and
        /// reconstructs the strongly-typed model.
        /// </summary>
        public virtual FormatReadResult<IStreamsResponseDocument> CreateStreamsResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Document
            var document = JsonSerializer.Deserialize<JsonStreamsResponseDocument>(content);
            var success = document != null;

            return new FormatReadResult<IStreamsResponseDocument>(document.ToStreamsDocument(), success);
        }

        /// <summary>
        /// Deserializes an Assets response document from a JSON stream and
        /// reconstructs the strongly-typed model.
        /// </summary>
        public virtual FormatReadResult<IAssetsResponseDocument> CreateAssetsResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Document
            var document = JsonSerializer.Deserialize<JsonAssetsResponseDocument>(content);
            var success = document != null;

            return new FormatReadResult<IAssetsResponseDocument>(document.ToAssetsDocument(), success);
        }

        /// <summary>
        /// Deserializes an Error response document from a JSON stream using
        /// the native error model.
        /// </summary>
        public FormatReadResult<IErrorResponseDocument> CreateErrorResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Document
            var document = JsonSerializer.Deserialize<ErrorResponseDocument>(content);
            var success = document != null;

            return new FormatReadResult<IErrorResponseDocument>(document, success);
        }


        /// <summary>
        /// Reads a single scalar formatter option by key and converts it to
        /// the requested type, returning <c>default(T)</c> on absence or
        /// conversion failure.
        /// </summary>
        protected static T GetFormatterOption<T>(IEnumerable<KeyValuePair<string, string>> options, string key)
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

        /// <summary>
        /// Reads every occurrence of a multi-valued formatter option by key
        /// and converts each value to the requested type, skipping any
        /// value that fails conversion.
        /// </summary>
        protected static IEnumerable<T> GetFormatterOptions<T>(IEnumerable<KeyValuePair<string, string>> options, string key)
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