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
    public class JsonHttpResponseDocumentFormatter : IResponseDocumentFormatter
    {
        public virtual string Id => "JSON-cppagent";

        public virtual string ContentType => "application/json";


        public FormatWriteResult Format(IDevicesResponseDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
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

        public FormatWriteResult Format(ref IStreamsResponseOutputDocument document, IEnumerable<KeyValuePair<string, string>> options = null)
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


        public FormatReadResult<IDevicesResponseDocument> CreateDevicesResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Document
            var document = JsonSerializer.Deserialize<JsonDevicesResponseDocument>(content);
            var success = document != null;

            return new FormatReadResult<IDevicesResponseDocument>(document.ToDocument(), success);
        }

        public FormatReadResult<IStreamsResponseDocument> CreateStreamsResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Document
            var document = JsonSerializer.Deserialize<JsonStreamsResponseDocument>(content);
            var success = document != null;

            return new FormatReadResult<IStreamsResponseDocument>(document.ToStreamsDocument(), success);
        }

        public virtual FormatReadResult<IAssetsResponseDocument> CreateAssetsResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Document
            var document = JsonSerializer.Deserialize<JsonAssetsResponseDocument>(content);
            var success = document != null;

            return new FormatReadResult<IAssetsResponseDocument>(document.ToAssetsDocument(), success);
        }

        public FormatReadResult<IErrorResponseDocument> CreateErrorResponseDocument(Stream content, IEnumerable<KeyValuePair<string, string>> options = null)
        {
            // Read Document
            var document = JsonSerializer.Deserialize<ErrorResponseDocument>(content);
            var success = document != null;

            return new FormatReadResult<IErrorResponseDocument>(document, success);
        }


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